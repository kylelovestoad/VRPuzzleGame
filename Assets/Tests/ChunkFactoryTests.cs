// using System.Reflection;
// using NUnit.Framework;
// using UnityEditor;
// using UnityEngine;
//
// public class ChunkFactoryTests
// {
//     private GameObject _chunkFactoryGameObject;
//     private ChunkFactory _chunkFactory;
//     
//     [SetUp]
//     public void SetUp()
//     {
//         _chunkFactoryGameObject = new GameObject("TestFactory");
//         _chunkFactory = _chunkFactoryGameObject.AddComponent<ChunkFactory>();
//         
//         Chunk prefab = AssetDatabase.LoadAssetAtPath<Chunk>("Assets/Prefabs/Chunk.prefab");
//         FieldInfo field = typeof(ChunkFactory).GetField("chunkPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
//         field.SetValue(_chunkFactory, prefab);
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         Object.DestroyImmediate(_chunkFactoryGameObject);
//     }
//
//     [Test]
//     public void TestCreateChunk()
//     {
//         Mesh mesh = new Mesh();
//         mesh.vertices = new [] { Vector3.zero };
//         mesh.RecalculateBounds();
//         
//         Material material = new Material(Shader.Find("Standard"));
//         
//         Chunk chunk = _chunkFactory.CreateSinglePieceChunk(Vector3.zero, Quaternion.identity, Vector3.zero, mesh, material);
//         Assert.IsNotNull(chunk);
//     }
// }
