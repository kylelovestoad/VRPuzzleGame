using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Networking
{
    public class MetaQuestAuthenticationManager : MonoBehaviour
    {
        [CanBeNull] public User User { get; private set; }
        public bool IsReady { get; private set; }

        async void Start()
        {
            await Initialize();
        }
        
        public async Task Initialize()
        {
            try
            {
                await InitializePlatform();
                await CheckEntitlement();
                User = await GetUser();

                IsReady = true;
                if (User != null)
                    Debug.Log($"Ready with user: {User.DisplayName} : {User.ID} : {User.OculusID}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Application.Quit();
            }
        }

        private static Task CheckEntitlement()
        {
            var tcs = new TaskCompletionSource<bool>();
            Entitlements.IsUserEntitledToApplication().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Entitlement check failed: {msg.GetError().Message}"));
                else
                    tcs.SetResult(true);
            });
            return tcs.Task;
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

        private static Task<User> GetUser()
        {
            var tcs = new TaskCompletionSource<User>();
            Users.GetLoggedInUser().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Failed to get user ID: {msg.GetError().Message}"));
                else
                    tcs.SetResult(msg.Data);
            });
            return tcs.Task;
        }

        public Task<string> GetNonce()
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