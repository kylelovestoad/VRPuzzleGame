using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class FileUploader : MonoBehaviour
{
    public Button uploadButton;
    
    void Start()
    {
        // // Make sure button is assigned
        // if (uploadButton != null)
        // {
        //     uploadButton.onClick.AddListener(OnUploadButtonClicked);
        // }
        OpenFileBrowser();
    }
    
    void OnUploadButtonClicked()
    {
        Debug.Log("Upload button clicked!");
        OpenFileBrowser();
    }
    
    void OpenFileBrowser()
    {
        // Common paths on Quest
        string downloadsPath = "/storage/emulated/0/Download/";
        string sdcardPath = "/sdcard/Download/";
        
        // Check which path exists
        if (Directory.Exists(downloadsPath))
        {
            LoadFilesFromPath(downloadsPath);
        }
        else if (Directory.Exists(sdcardPath))
        {
            LoadFilesFromPath(sdcardPath);
        }
        else
        {
            Debug.LogError("Cannot access storage!");
        }
    }
    
    void LoadFilesFromPath(string path)
    {
        try
        {
            string[] files = Directory.GetFiles(path);
            
            Debug.Log($"Found {files.Length} files:");
            foreach (string file in files)
            {
                Debug.Log($"File: {file}");
                
                if (file.EndsWith(".txt"))
                {
                    string content = File.ReadAllText(file);
                    Debug.Log($"File content: {content}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading files: {e.Message}");
        }
    }
}