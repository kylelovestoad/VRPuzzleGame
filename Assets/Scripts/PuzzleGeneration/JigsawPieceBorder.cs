namespace PuzzleGeneration
{
    public record JigsawPieceBorder(
        JigsawPieceEdge Top,
        JigsawPieceEdge Bottom,
        JigsawPieceEdge Left,
        JigsawPieceEdge Right)
    {
        public JigsawPieceEdge Top { get; } = Top;
        public JigsawPieceEdge Bottom { get; } = Bottom;
        public JigsawPieceEdge Left { get; } = Left;
        public JigsawPieceEdge Right { get; } = Right;
    }
}