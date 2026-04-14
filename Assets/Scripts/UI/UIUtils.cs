using System;
using System.Collections.Generic;
using Networking.API;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

namespace UI
{
    public static class UIUtils
    {
        public static Sprite PuzzleImageSprite(Texture2D puzzleImageTexture)
        {
            var puzzleImageSprite = Sprite.Create(
                puzzleImageTexture,
                new Rect(0, 0, puzzleImageTexture.width, puzzleImageTexture.height), 
                Vector2.zero
            );
            
            return puzzleImageSprite;
        }

        
        public static string AsTimeString(float time)
        {
            return TimeSpan
                .FromSeconds(time)
                .ToString(@"m\:ss");
        }
        
        public static string AsTimeStringMillis(float time)
        {
            return TimeSpan
                .FromSeconds(time)
                .ToString(@"m\:ss\.fff");
        }

        public static async void CreatePuzzleForCurrentUser(
            string puzzleName,
            PuzzleLayout layout,
            Texture2D puzzleImage
            )
        {
            if (Application.isPlaying)
            {
                var user = await PuzzleServerApi.Instance.Manager.GetUser();
                           
                LocalSave.Instance.Create(new PuzzleSaveData(
                   null,
                   null,
                   puzzleName,
                   user.ID.ToString(),
                   user.DisplayName,
                   layout,
                   new List<ChunkSaveData>(),
                   puzzleImage
                )); 
            }
            else
            {
                LocalSave.Instance.Create(new PuzzleSaveData(
                    null,
                    null,
                    puzzleName,
                    "0",
                    "test",
                    layout,
                    new List<ChunkSaveData>(),
                    puzzleImage
                )); 
            }
        }
    }
}