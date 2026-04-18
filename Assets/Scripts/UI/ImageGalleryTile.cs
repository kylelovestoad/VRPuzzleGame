using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageGalleryTile : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private Image previewImage;
        [SerializeField] private Button selectButton;

        private Action _onClickAction;

        private void Awake()
        {
            if (selectButton != null)
                selectButton.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            if (selectButton != null)
                selectButton.onClick.RemoveListener(OnClick);
        }

        private void DisplayImage(
            string title, 
            Sprite sprite, 
            Action onClickAction
        )
        {
            titleLabel.text = title;
            previewImage.sprite = sprite;
            
            _onClickAction = onClickAction;
            
            gameObject.SetActive(true);
        }

        public void DisplayImage(string title, Texture2D texture, Action onClickAction)
        {
            Sprite sprite = null;
            if (texture != null)
            {
                sprite = UIUtils.PuzzleImageSprite(texture);
                Debug.Log($"[Quest] sprite rect={sprite.rect} | texture={texture.width}x{texture.height} | previewImage null={previewImage == null}");
            }

            DisplayImage(title, sprite, onClickAction);
        }

        private void OnClick()
        {
            _onClickAction?.Invoke();
        }
    }
}