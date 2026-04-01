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
    }
}