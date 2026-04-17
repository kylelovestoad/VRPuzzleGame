using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace UI
{
    public sealed class QuestPhotoGallery : MonoBehaviour
    {
        private const string ReadMediaImages = "android.permission.READ_MEDIA_IMAGES";

        public List<Texture2D> LoadedPhotos { get; } = new();

        public event Action OnExit;
        
        public event Action<Texture2D> OnSelectPhoto;

        public Button exitButton;
        
        public GameObject photoContainer;

        public ImageGalleryTile photoTile;
        
        private void Start()
        {
            // exitButton.onClick.AddListener(Exit);;
        }

        private void OnDestroy()
        {
            // exitButton.onClick.RemoveListener(Exit);
        }

        private void OnDisable()
        {
            // ClearPhotos();
        }

        private void Exit()
        {
            OnExit?.Invoke();
        }

        public void FillPhotos()
        {
            if (Permission.HasUserAuthorizedPermission(ReadMediaImages))
            {
                Debug.Log("[QuestPhotoLoader] Permission already granted.");
                LoadAllPhotos();
                return;
            }

            Debug.Log("[QuestPhotoLoader] Requesting READ_MEDIA_IMAGES...");

            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += OnPermissionGranted;
            callbacks.PermissionDenied += OnPermissionDenied;

            Permission.RequestUserPermission(ReadMediaImages, callbacks);
        }

        private static void OnPermissionGranted(string permissionName)
        {
            Debug.Log($"[QuestPhotoLoader] Permission granted: {permissionName}");
        }

        private void OnPermissionDenied(string permissionName)
        {
            Debug.LogWarning($"[QuestPhotoLoader] Permission denied: {permissionName}");
        }
        
        private List<string> QueryMediaStoreImageUris()
        {
            var uris = new List<string>();

            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var resolver = activity.Call<AndroidJavaObject>("getContentResolver");
                using var mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
                using var externalUri = mediaStore.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");

                string[] projection = { "_id", "_display_name" };

                using var cursor = resolver.Call<AndroidJavaObject>(
                    "query", externalUri, projection, null, null, "date_added DESC"
                );

                if (cursor == null)
                {
                    Debug.LogError("[QuestPhotoLoader] MediaStore cursor is null.");
                    return uris;
                }

                var count = cursor.Call<int>("getCount");
                Debug.Log($"[QuestPhotoLoader] MediaStore returned {count} image(s).");

                if (count == 0) return uris;

                var idColIndex = cursor.Call<int>("getColumnIndexOrThrow", "_id");

                using var contentUris = new AndroidJavaClass("android.content.ContentUris");

                while (cursor.Call<bool>("moveToNext"))
                {
                    var id = cursor.Call<long>("getLong", idColIndex);
                    using var itemUri = contentUris.CallStatic<AndroidJavaObject>(
                        "withAppendedId", externalUri, id
                    );
                    uris.Add(itemUri.Call<string>("toString"));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestPhotoLoader] MediaStore query failed: {e.Message}");
            }

            return uris;
        }

        private void LoadAllPhotos()
        {
            LoadedPhotos.Clear();

            var uris = QueryMediaStoreImageUris();

            if (uris.Count == 0)
            {
                Debug.LogWarning("[QuestPhotoLoader] No photos found via MediaStore.");
                return;
            }

            Debug.Log($"[QuestPhotoLoader] Loading {uris.Count} photo(s)...");
            StartCoroutine(LoadTexturesSequentially(uris));
        }

        private IEnumerator LoadTexturesSequentially(List<string> uris)
        {
            foreach (string uri in uris)
            {
                yield return null;
                LoadTextureFromContentUri(uri);
            }

            Debug.Log($"[QuestPhotoLoader] Done. {LoadedPhotos.Count} photo(s) loaded.");
            FillPhotos(LoadedPhotos);
        }

        private void LoadTextureFromContentUri(string contentUriString)
        {
            var tempPath = Path.Combine(Application.temporaryCachePath, "quest_photo_temp.jpg");

            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var resolver = activity.Call<AndroidJavaObject>("getContentResolver");

                using var uriClass = new AndroidJavaClass("android.net.Uri");
                using var uri = uriClass.CallStatic<AndroidJavaObject>("parse", contentUriString);

                using var inputStream = resolver.Call<AndroidJavaObject>("openInputStream", uri);
                if (inputStream == null)
                {
                    Debug.LogError($"[QuestPhotoLoader] openInputStream returned null for {contentUriString}");
                    return;
                }

                using var fos = new AndroidJavaObject("java.io.FileOutputStream", tempPath);

                const int chunkSize = 65536;
                var javaBuffer = AndroidJNI.NewSByteArray(chunkSize);

                try
                {
                    var inputStreamClass = AndroidJNI.FindClass("java/io/InputStream");
                    var readMethodId = AndroidJNI.GetMethodID(inputStreamClass, "read", "([BII)I");
                    var fosClass = AndroidJNI.FindClass("java/io/FileOutputStream");
                    var writeMethodId = AndroidJNI.GetMethodID(fosClass, "write", "([BII)V");
                    var flushMethodId = AndroidJNI.GetMethodID(fosClass, "flush", "()V");

                    var inputObj = inputStream.GetRawObject();
                    var fosObj = fos.GetRawObject();

                    int bytesRead;
                    var readArgs = new jvalue[]
                    {
                        new() { l = javaBuffer },
                        new() { i = 0 },
                        new() { i = chunkSize }
                    };

                    while ((bytesRead = AndroidJNI.CallIntMethod(inputObj, readMethodId, readArgs)) != -1)
                    {
                        var writeArgs = new jvalue[]
                        {
                            new() { l = javaBuffer },
                            new() { i = 0 },
                            new() { i = bytesRead }
                        };
                        AndroidJNI.CallVoidMethod(fosObj, writeMethodId, writeArgs);
                    }

                    AndroidJNI.CallVoidMethod(fosObj, flushMethodId, Array.Empty<jvalue>());
                }
                finally
                {
                    AndroidJNI.DeleteLocalRef(javaBuffer);
                    inputStream.Call("close");
                    fos.Call("close");
                }

                Debug.Log($"[QuestPhotoLoader] Wrote temp file: {tempPath} ({new FileInfo(tempPath).Length} bytes)");
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestPhotoLoader] Stream → file failed for {contentUriString}: {e.Message}\n{e.StackTrace}");
                return;
            }

            try
            {
                var imageBytes = File.ReadAllBytes(tempPath);
                Debug.Log($"[QuestPhotoLoader] Read {imageBytes.Length} bytes from temp file.");

                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (texture.LoadImage(imageBytes))
                {
                    texture.name = contentUriString;
                    LoadedPhotos.Add(texture);
                    Debug.Log($"[QuestPhotoLoader] Loaded: {contentUriString} ({texture.width}x{texture.height})");
                }
                else
                {
                    Debug.LogError($"[QuestPhotoLoader] LoadImage failed for {contentUriString}");
                    Destroy(texture);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestPhotoLoader] Texture decode failed: {e.Message}");
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
        

        private void FillPhotos(List<Texture2D> photos)
        {
            Debug.Log($"[QuestPhotoLoader] FillPhotos called with {photos.Count} photo(s).");

            foreach (var photo in photos)
            {
                
                if (photoTile == null)
                {
                    Debug.LogError("[QuestPhotoLoader] photoTile is null");
                    return; 
                }
                
                var tile = Instantiate(
                    photoTile,
                    photoContainer.transform,
                    false
                );
                
                tile.DisplayImage(
                    null, 
                    photo,
                    () => {
                        OnSelectPhoto?.Invoke(photo);
                        Exit();
                    });
            }
        }
        
        private void ClearPhotos()
        {
            var tiles = photoContainer.GetComponentsInChildren<ImageGalleryTile>();
            foreach (var tile in tiles)
            {
                if (Application.isPlaying) Destroy(tile.gameObject);
                else DestroyImmediate(tile.gameObject);
            }
        }

    }
}