// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using NUnit.Framework;
// using Oculus.Interaction.Samples;
// using UnityEngine;
// using TMPro;
// using UI;
// using Persistence;
//
// namespace Tests.UI
// {
//     public class PuzzleCreationBehaviourTests
//     {
//         private GameObject _gameObject;
//         private PuzzleCreationBehaviour _behaviour;
//         private PuzzleFormBehaviour _form;
//
//         [SetUp]
//         public void SetUp()
//         {
//             LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
//
//             _gameObject = new GameObject("PuzzleCreationBehaviour");
//             _behaviour = _gameObject.AddComponent<PuzzleCreationBehaviour>();
//
//             var formObject = new GameObject("PuzzleForm");
//             formObject.transform.SetParent(_gameObject.transform);
//             _form = formObject.AddComponent<PuzzleFormBehaviour>();
//
//             typeof(PuzzleCreationBehaviour)
//                 .GetField("puzzleFormBehaviour", BindingFlags.NonPublic | BindingFlags.Instance)
//                 .SetValue(_behaviour, _form);
//
//             _form.nameInputField = GetInputField("NameInput");
//             _form.rowsInputField = GetInputField("RowsInput");
//             _form.columnsInputField = GetInputField("ColumnsInput");
//
//             var dropdownObject = new GameObject("Dropdown");
//             _form.dropdown = dropdownObject.AddComponent<DropDownGroup>();
//
//             _behaviour.PuzzleImage = new Texture2D(1, 1);
//             _behaviour.realImage = new Texture2D(1, 1);
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             Object.DestroyImmediate(_gameObject);
//
//             LocalSave.Instance.DB.DropCollection("puzzles");
//             LocalSave.Shutdown();
//         }
//
//         private TMP_InputField GetInputField(string name)
//         {
//             var inputFieldObject = new GameObject(name);
//             return inputFieldObject.AddComponent<TMP_InputField>();
//         }
//
//         [Test]
//         public void TestOnCreate()
//         {
//             _form.nameInputField.text = "DK Won";
//             _form.rowsInputField.text = "1";
//             _form.columnsInputField.text = "1";
//
//             typeof(DropDownGroup)
//                 .GetField("_selectedIndex", BindingFlags.NonPublic | BindingFlags.Instance)
//                 .SetValue(_form.dropdown, 0);
//
//             typeof(PuzzleCreationBehaviour)
//                 .GetMethod("OnCreate", BindingFlags.NonPublic | BindingFlags.Instance)
//                 ?.Invoke(_behaviour, null);
//
//             var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
//             Assert.AreEqual(1, all.Count);
//         }
//     }
// }