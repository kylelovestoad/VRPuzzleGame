namespace PuzzleGeneration.Jigsaw
{
    public record JigsawPieceBorder(
        float BoxWidth,
        float BoxHeight, 
        JigsawPieceEdgeType Top,
        JigsawPieceEdgeType Bottom,
        JigsawPieceEdgeType Left,
        JigsawPieceEdgeType Right
    ) {
        public float BoxWidth { get; } = BoxWidth;
        public float BoxHeight { get; } = BoxHeight;
        public JigsawPieceEdgeType Top { get; } = Top;
        public JigsawPieceEdgeType Bottom { get; } = Bottom;
        public JigsawPieceEdgeType Left { get; } = Left;
        public JigsawPieceEdgeType Right { get; } = Right;
    }
}