// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using NUnit.Framework;
// using UnityEditor;
// using UnityEngine;
//
// public class ChunkTests
// {
//     private const float Tolerance = 1e-6f;
//
//     private GameObject _chunkPrefab;
//     private GameObject _chunkObject;
//     private Chunk _chunk;
//     private Mesh _simpleMesh;
//     private Material _simpleMaterial;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _chunkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chunk.prefab");
//         
//         _chunkObject = Object.Instantiate(_chunkPrefab);
//         _chunk = _chunkObject.GetComponent<Chunk>();
//         
//         _simpleMesh = new Mesh();
//         _simpleMesh.vertices = new [] { Vector3.zero };
//         _simpleMesh.RecalculateBounds();
//         
//         _simpleMaterial = new Material(Shader.Find("Standard"));
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         Object.DestroyImmediate(_chunkObject);
//     }
//
//     #region Initialization Tests
//
//     [Test]
//     public void PrefabComponentsExist()
//     {
//         BoxCollider boxCollider = _chunk.GetComponent<BoxCollider>();
//         Assert.IsNotNull(boxCollider, "BoxCollider should exist on chunk");
//
//         Rigidbody rigidbody = _chunk.GetComponent<Rigidbody>();
//         Assert.IsNotNull(rigidbody, "Rigidbody should exist on chunk");
//     }
//     
//     [Test]
//     public void MakesChunkWithSinglePieceInitially()
//     {
//         Vector3 solutionLocation = Vector3.zero;
//
//         MethodInfo awakeMethod = typeof(Chunk).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
//         awakeMethod?.Invoke(_chunk, null);
//         
//         _chunk.InitializeSinglePieceChunk(solutionLocation, _simpleMesh, _simpleMaterial);
//         
//         FieldInfo pieces = typeof(Chunk).GetField("_pieces", BindingFlags.NonPublic | BindingFlags.Instance);
//         dynamic p = pieces.GetValue(_chunk);
//         
//         int count = p.Count;
//         
//         Assert.AreEqual(count, 1);
//     }
//     
//     #endregion
//     
//     #region Updating Chunk Tests
//     
//     [Test]
//     public void UpdatesChunkSizeWhenMorePiecesAdded()
//     {
//         Vector3 solutionLocation = Vector3.zero;
//         
//         GameObject newPieceObject = _chunkObject.transform.Find("Piece").gameObject;
//         Piece piece = newPieceObject.AddComponent<Piece>();
//         piece.transform.position = new Vector3(0, 0, 0);
//         
//         Mesh mesh = new Mesh();
//         mesh.vertices = new Vector3[]
//         {
//             new(0, 0, 0), 
//             new(0, 1, 0), 
//             new(1, 0, 0), 
//             new(1, 1, 0)
//         };
//         mesh.RecalculateBounds();
//         
//         MethodInfo awakeMethod = typeof(Chunk).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
//         awakeMethod?.Invoke(_chunk, null);
//         
//         _chunk.InitializeSinglePieceChunk(solutionLocation, mesh, _simpleMaterial);
//         
//         MethodInfo updateBoxColliderMethod = typeof(Chunk).GetMethod("UpdateBoxCollider", BindingFlags.NonPublic | BindingFlags.Instance);
//         
//         List<Piece> pieces = new List<Piece> { piece };
//         updateBoxColliderMethod?.Invoke(_chunk, new object[] { pieces });
//         
//         BoxCollider boxCollider = _chunk.GetComponent<BoxCollider>();
//         
//         Assert.AreEqual(boxCollider.size.x, 1.02f, Tolerance);
//         Assert.AreEqual(boxCollider.size.y, 1.02f, Tolerance);
//         Assert.AreEqual(boxCollider.size.z, 0.02f, Tolerance);
//         
//         Assert.AreEqual(boxCollider.center.x, 0.5f, Tolerance);
//         Assert.AreEqual(boxCollider.center.y, 0.5f, Tolerance);
//         Assert.AreEqual(boxCollider.center.z, 0.0f, Tolerance);
//         
//         Object.DestroyImmediate(newPieceObject);
//     }
//     
//     [Test]
//     public void CombiningChunks()
//     {
//         Vector3 solutionLocation = Vector3.zero;
//         
//         GameObject newPieceObject = _chunkObject.transform.Find("Piece").gameObject;
//         Piece piece = newPieceObject.AddComponent<Piece>();
//         piece.transform.position = new Vector3(0, 0, 0);
//         
//         Mesh mesh = new Mesh();
//         mesh.vertices = new Vector3[]
//         {
//             new(0, 0, 0), 
//             new(0, 1, 0), 
//             new(1, 0, 0), 
//             new(1, 1, 0)
//         };
//         mesh.RecalculateBounds();
//         
//         MethodInfo awakeMethod = typeof(Chunk).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
//         awakeMethod?.Invoke(_chunk, null);
//         
//         _chunk.InitializeSinglePieceChunk(solutionLocation, mesh, _simpleMaterial);
//         
//         GameObject otherChunkObject = Object.Instantiate(_chunkPrefab);
//         Chunk otherChunk = otherChunkObject.GetComponent<Chunk>();
//         awakeMethod?.Invoke(otherChunk, null);
//         
//         GameObject otherPieceObject = otherChunkObject.transform.Find("Piece").gameObject;
//         Piece otherPiece = otherPieceObject.AddComponent<Piece>();
//         otherPiece.transform.position = new Vector3(1, 0, 0);
//         
//         otherChunk.InitializeSinglePieceChunk(new Vector3(1, 0, 0), mesh, _simpleMaterial);
//         
//         MethodInfo combineMethod = typeof(Chunk).GetMethod("Combine", BindingFlags.NonPublic | BindingFlags.Instance);
//         combineMethod?.Invoke(_chunk, new object[] { otherChunk });
//         
//         Assert.AreEqual(2, _chunk.transform.childCount, "Combine chunk should contains both pieces");
//         
//         FieldInfo piecesField = typeof(Chunk).GetField("_pieces", BindingFlags.NonPublic | BindingFlags.Instance);
//         IList pieces = (IList) piecesField.GetValue(_chunk);
//
//         Assert.AreEqual(2, pieces.Count, "Should have 2 pieces");
//     }
//     
//     #endregion
// }
