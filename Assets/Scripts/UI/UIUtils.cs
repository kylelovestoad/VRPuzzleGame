using System;
using Persistence;
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
    }
}