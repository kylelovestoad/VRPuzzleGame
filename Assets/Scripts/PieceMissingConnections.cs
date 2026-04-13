using System.Collections.Generic;

public readonly struct PieceMissingConnections
{
    public readonly Piece CurrPiece;
    public readonly List<int> UnconnectedNeighborIndices;

    public PieceMissingConnections(Piece currPiece, List<int> unconnectedNeighborIndices)
    {
        CurrPiece = currPiece;
        UnconnectedNeighborIndices = unconnectedNeighborIndices;
    }
}
