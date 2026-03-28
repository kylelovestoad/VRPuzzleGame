using System;
using System.Threading.Tasks;
using Oculus.Platform;
using UnityEngine;

namespace Networking
{
    public class MetaQuestAuthenticationManager : MonoBehaviour
    {
        public string UserId  { get; private set; }
        public string Nonce   { get; private set; }
        public bool IsReady { get; private set; }

        async void Start()
        {
            await Initialize(
                onReady: () => Debug.Log("Ready"),
                onError: err => Debug.LogError(err)
            );
        }
        
        public async Task Initialize(Action onReady, Action<string> onError)
        {
            try
            {
                await InitializePlatform();
                UserId = await GetUserId();
                Nonce  = await GetNonce();

                IsReady = true;
                onReady?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        private static Task InitializePlatform()
        {
            var tcs = new TaskCompletionSource<bool>();
            Core.AsyncInitialize().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Oculus platform init failed: {msg.GetError().Message}"));
                else
                    tcs.SetResult(true);
            });
            return tcs.Task;
        }

        private static Task<string> GetUserId()
        {
            var tcs = new TaskCompletionSource<string>();
            Users.GetLoggedInUser().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Failed to get user ID: {msg.GetError().Message}"));
                else
                    tcs.SetResult(msg.Data.ID.ToString());
            });
            return tcs.Task;
        }

        private static Task<string> GetNonce()
        {
            var tcs = new TaskCompletionSource<string>();
            Users.GetUserProof().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Failed to get nonce: {msg.GetError().Message}"));
                else
                    tcs.SetResult(msg.Data.Value);
            });
            return tcs.Task;
        }
    }
}