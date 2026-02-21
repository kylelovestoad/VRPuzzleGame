using UnityEngine;

namespace PuzzleGeneration
{
    public enum JigsawPieceEdge
    {
        Straight,
        SocketIn,
        SocketOut,
    }

    public static class JigsawPieceEdgeUtil
    {
        public static JigsawPieceEdge Match(this JigsawPieceEdge edge) => edge switch
        {
            JigsawPieceEdge.SocketIn  => JigsawPieceEdge.SocketOut,
            JigsawPieceEdge.SocketOut => JigsawPieceEdge.SocketIn
        };

        public static JigsawPieceEdge RandomSocket()
        {
            return Random.Range(0, 2) == 0 ? JigsawPieceEdge.SocketIn : JigsawPieceEdge.SocketOut;
        }
    }
}