using Oculus.Platform.Models;
using UnityEngine;

public class HintManager
{
    private static readonly int CentroidPropertyID = Shader.PropertyToID("_centroid");

    private readonly Material _hintOutlineMaterial;

    private PieceHint? _hint0;
    private PieceHint? _hint1;

    public HintManager(Material hintOutlineMaterial)
    {
        _hintOutlineMaterial = hintOutlineMaterial;
    }

    public void ShowHint(Piece piece0, Piece piece1)
    {
        Debug.Assert(_hint0 == null && _hint1 == null, 
            "Hint must not be applied yet");
        
        _hint0 = CreatePieceHint(piece0);
        _hint1 = CreatePieceHint(piece1);

        piece0.SetOutlineMaterial(_hint0.Value.Material);
        piece1.SetOutlineMaterial(_hint1.Value.Material);
    }
    
    private PieceHint CreatePieceHint(Piece piece)
    {
        var material = new Material(_hintOutlineMaterial);
        
        var centroid = Util.Centroid(piece.BorderPoints);
        Debug.LogError("Centorid: " + centroid);
        
        material.SetVector(CentroidPropertyID, new Vector4(centroid.x, centroid.y, 0, 0));
        
        return new PieceHint(piece, material);
    }

    public void ClearHint()
    {
        Debug.Assert(_hint0 != null && _hint1 != null, 
            "Hint must be applied to both pieces");

        if (_hint0 == null) return;
        
        var material0 = _hint0.Value.Material;
        var material1 = _hint1.Value.Material;

        _hint0.Value.Piece.ClearOutlineMaterial(material0);
        _hint1.Value.Piece.ClearOutlineMaterial(material1);

        Object.Destroy(material0);
        Object.Destroy(material1);

        _hint0 = null;
        _hint1 = null;
    }

    private readonly struct PieceHint
    {
        public readonly Piece Piece;
        public readonly Material Material;

        public PieceHint(Piece piece, Material material)
        {
            Piece = piece;
            Material = material;
        }
    }
}
