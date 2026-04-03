using System.Collections.Generic;

public record PieceMissingConnections(Piece CurrPiece, List<int> UnconnectedNeighborIndices)
{
    public Piece CurrPiece { get; } = CurrPiece;
    public List<int> UnconnectedNeighborIndices { get; } = UnconnectedNeighborIndices;
}
