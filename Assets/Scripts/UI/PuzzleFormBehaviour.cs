using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using Oculus.Interaction.Samples;
using Persistence;
using PuzzleGeneration;
using UnityEngine.Serialization;
using Void = EditorAttributes.Void;

namespace UI
{
    public class PuzzleFormBehaviour : MonoBehaviour
    {
        [FoldoutGroup("UI",
            nameof(nameInputField),
            nameof(rowsInputField),
            nameof(columnsInputField),
            nameof(dropdown),
            nameof(dropdownList)
        )]
        [SerializeField] 
        private Void groupHolder;

        [SerializeField, HideProperty] 
        public TMP_InputField nameInputField;
        
        [SerializeField, HideProperty] 
        public TMP_InputField rowsInputField;
        
        [SerializeField, HideProperty] 
        public TMP_InputField columnsInputField;
        
        [SerializeField, HideProperty] 
        public DropDownGroup dropdown;
        
        [SerializeField, HideProperty] 
        public GameObject dropdownList;
        
        public Button imageButton;
        
        public Image image;
        
        [SerializeField]
        private Texture2D imageTexture;
        
        public event Action<Action<Texture2D>> OnImagePickerOpen;

        private void Start()
        {
            imageButton.onClick.AddListener(OnImageButtonClicked);
        }

        private void OnImageButtonClicked()
        {
            OnImagePickerOpen?.Invoke(OnImageSet);
        }

        private void OnImageSet(Texture2D texture)
        {
            image.sprite = UIUtils.PuzzleImageSprite(texture);
            imageTexture = texture;
        }

        public bool TryGetFormInput(out PuzzleForm input)
        {
            input = null;

            if (string.IsNullOrWhiteSpace(nameInputField.text)) return false;
            if (!Enum.IsDefined(typeof(PieceShape), dropdown.SelectedIndex)) return false;
            if (!int.TryParse(rowsInputField.text, out var rows)) return false;
            if (!int.TryParse(columnsInputField.text, out var columns)) return false;

            input = new PuzzleForm(nameInputField.text, (PieceShape)dropdown.SelectedIndex, rows, columns, imageTexture);
            return true;
        }
        
        public void FillForm(PuzzleMetadata metadata)
        {
            Debug.Log($"Filling form for {metadata} {dropdown.GetComponentInChildren<TMP_Dropdown>() != null}");
            
            Debug.Log($"Shape Index: {(int) metadata.layout.shape}");
            
            nameInputField.text = metadata.name;
            rowsInputField.text = metadata.layout.rows.ToString();
            columnsInputField.text = metadata.layout.cols.ToString();
            
            Debug.Log($"Piece Shape Num Options: {dropdownList.GetComponentsInChildren<Toggle>()}");
            
            dropdownList.GetComponentsInChildren<Toggle>()[(int) metadata.layout.shape].isOn = true;
            
            OnImageSet(metadata.PuzzleImage);
        }
    }
}
