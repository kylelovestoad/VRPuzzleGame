// using System;
// using JetBrains.Annotations;
// using UnityEngine;
//
// namespace Persistence
// {
//     [Serializable]
//     public record PuzzleData
//     {
//         public PuzzleMetadata metadata;
//         [CanBeNull] public PuzzleSaveData saveData;
//
//         public PuzzleData(PuzzleMetadata metadata, PuzzleSaveData saveData)
//         {
//             this.metadata = metadata;
//             this.saveData = saveData;
//         }
//
//         public long? CurrentConnections() => saveData?.CurrentConnections(metadata.PieceCount);
//         public float? PercentComplete() => saveData?.PercentComplete(metadata.PieceCount);
//     }
// }