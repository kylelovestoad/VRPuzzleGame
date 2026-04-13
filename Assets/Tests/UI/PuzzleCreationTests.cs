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
//
//         [SetUp]
//         public void SetUp()
//         {
//             LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
//
//             _gameObject = new GameObject("PuzzleCreationBehaviour");
//             _behaviour = _gameObject.AddComponent<PuzzleCreationBehaviour>();
//
//             _behaviour.nameInputField = GetInputField("NameInput");
//             _behaviour.rowsInputField = GetInputField("RowsInput");
//             _behaviour.columnsInputField = GetInputField("ColumnsInput");
//
//             var dropdownObject = new GameObject("Dropdown");
//             _behaviour.dropdown = dropdownObject.AddComponent<DropDownGroup>();
//
//             _behaviour.puzzleImage = new Texture2D(1, 1);
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
//             var inputField = inputFieldObject.AddComponent<TMP_InputField>();
//
//             return inputField;
//         }
//
//         [Test]
//         public void TestOnCreate()
//         {
//             _behaviour.nameInputField.text = "DK Won";
//             _behaviour.rowsInputField.text = "1";
//             _behaviour.columnsInputField.text = "1";
//             
//             typeof(DropDownGroup)
//                 .GetField("_selectedIndex", BindingFlags.NonPublic | BindingFlags.Instance)
//                 .SetValue(_behaviour.dropdown, 0);
//             
//             var collideMethod = typeof(PuzzleCreationBehaviour).GetMethod(
//                 "OnCreate",
//                 BindingFlags.NonPublic | BindingFlags.Instance
//             );
//             collideMethod?.Invoke(_behaviour, new object[] { });
//             
//             var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
//             Assert.AreEqual(1, all.Count);
//         }
//     }
// }
