using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Networking.API
{
    public class MetaQuestAuthenticationManager : MonoBehaviour
    {
        [CanBeNull] public string AccessToken { get; private set; }
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

                IsReady = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Application.Quit();
            }
        }

        private Task CheckEntitlement()
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

        private Task InitializePlatform()
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

        public Task<User> GetUser()
        {
            var tcs = new TaskCompletionSource<User>();
            Users.GetLoggedInUser().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Failed to get user: {msg.GetError().Message}"));
                else
                    tcs.SetResult(msg.Data);
            });
            return tcs.Task;
        }

        public Task<string> GetAccessToken()
        {
            var tcs = new TaskCompletionSource<string>();
            Users.GetAccessToken().OnComplete(msg =>
            {
                if (msg.IsError)
                    tcs.SetException(new Exception($"Failed to get access token: {msg.GetError().Message}"));
                else
                    tcs.SetResult(msg.Data);
            });
            return tcs.Task;
        }
    }
}