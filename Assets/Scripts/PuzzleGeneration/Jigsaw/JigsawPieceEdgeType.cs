using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleGeneration.Jigsaw
{
    public enum JigsawPieceEdgeType
    {
        Straight,
        SocketUp,
        SocketDown,
        SocketLeft,
        SocketRight
    }
    
    public static class JigsawPieceEdgeUtil
    {
        // https://stackoverflow.com/questions/30617132/jigsaw-puzzle-pices-using-bezier-curve
        private static readonly Vector2[] SocketUpBaseControlPoints = {
            new(0f, 0f),
            new(0.35f, -0.15f),
            new(0.37f, -0.05f),

            new(0.40f, 0f),
            new(0.38f, 0.05f),

            new(0.20f, 0.20f),
            new(0.50f, 0.20f),

            new(0.80f, 0.20f),
            new(0.62f, 0.05f),

            new(0.60f, 0f),
            new(0.63f, -0.05f),

            new(0.65f, -0.15f),
            new(1f, 0f),
        };

        private static readonly Vector2[] SocketDownBaseControlPoints = ReflectSocketControlPointsY(SocketUpBaseControlPoints);
    
        private static readonly Vector2[] SocketLeftBaseControlPoints = SwappedSocketControlPoints(SocketUpBaseControlPoints);
    
        private static readonly Vector2[] SocketRightBaseControlPoints = ReflectSocketControlPointsX(SocketLeftBaseControlPoints);

        private static Vector2[] SwappedSocketControlPoints(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(point.y, point.x)).ToArray();
        }
    
        private static Vector2[] ReflectSocketControlPointsX(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(-point.x, point.y)).ToArray();
        }
    
        private static Vector2[] ReflectSocketControlPointsY(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(point.x, -point.y)).ToArray();
        }
        
        public static Vector2[] BaseControlPoints(this JigsawPieceEdgeType direction) => direction switch
        {
            JigsawPieceEdgeType.SocketUp    => SocketUpBaseControlPoints,
            JigsawPieceEdgeType.SocketDown  => SocketDownBaseControlPoints,
            JigsawPieceEdgeType.SocketLeft  => SocketLeftBaseControlPoints,
            JigsawPieceEdgeType.SocketRight => SocketRightBaseControlPoints,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
        
        public static JigsawPieceEdgeType RandomHorizontalSocket()
        {
            return Random.Range(0, 2) == 0 ? JigsawPieceEdgeType.SocketLeft : JigsawPieceEdgeType.SocketRight;
        }
        
        public static JigsawPieceEdgeType RandomVerticalSocket()
        {
            return Random.Range(0, 2) == 0 ? JigsawPieceEdgeType.SocketUp : JigsawPieceEdgeType.SocketDown;
        }
    }
}
