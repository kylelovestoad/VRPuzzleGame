using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    [SerializeField] 
    private Piece piecePrefab;
    
    public Piece CreatePiece(
        Vector3 initialPosition,
        Quaternion initialRotation,
        Vector3 solutionLocation,
        Mesh mesh,
        Material material
    ) {
        Piece newPiece = Instantiate(piecePrefab, initialPosition, initialRotation);
        newPiece.InitializeVariant(solutionLocation, mesh, material);
        
        return newPiece;
    }
}
