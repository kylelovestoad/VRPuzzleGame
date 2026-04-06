using UnityEngine;

public class HintManager
{
    private static readonly int CentroidPropertyID = Shader.PropertyToID("_centroid");

    private readonly Material _hintFrontMaterial;
    private readonly Material _hintBackAndSidesMaterial;
    

    private PieceHint? _hint0;
    private PieceHint? _hint1;

    public HintManager(Material hintFrontMaterial, Material hintBackAndSidesMaterial)
    {
        _hintFrontMaterial = hintFrontMaterial;
        _hintBackAndSidesMaterial = hintBackAndSidesMaterial;
    }

    public void ShowHint(Piece piece0, Piece piece1)
    {
        Debug.Assert(_hint0 == null && _hint1 == null, 
            "Hint must not be applied yet");
        
        _hint0 = CreatePieceHint(piece0);
        _hint1 = CreatePieceHint(piece1);
        
        var hint0 = _hint0.Value;
        var hint1 = _hint1.Value;

        piece0.SetMaterials(hint0.FrontMaterial, hint0.BackAndSidesMaterial);
        piece1.SetMaterials(hint1.FrontMaterial, hint1.BackAndSidesMaterial);
    }
    
    private PieceHint CreatePieceHint(Piece piece)
    {
        var frontMaterial = new Material(_hintFrontMaterial);
        var backAndSidesMaterial = new Material(_hintBackAndSidesMaterial);
        
        var centroid = Util.Centroid(piece.BorderPoints);
        Debug.LogError("Centorid: " + centroid);
        
        frontMaterial.SetVector(CentroidPropertyID, new Vector4(centroid.x, centroid.y, 0, 0));
        backAndSidesMaterial.SetVector(CentroidPropertyID, new Vector4(centroid.x, centroid.y, 0, 0));
        
        return new PieceHint(piece, frontMaterial, backAndSidesMaterial);
    }

    private void ClearHint()
    {
        Debug.Assert(_hint0 != null && _hint1 != null, 
            "Hint must be applied to both pieces");

        if (_hint0 == null) return;
        
        var hint0 =  _hint0.Value;
        var hint1 = _hint1.Value;
        
        // var material0 = _hint0.Value.BackAndSidesMaterial;
        // var material1 = _hint1.Value.BackAndSidesMaterial;

        var piece0 = _hint0.Value.Piece;
        var piece1 = _hint1.Value.Piece;

        piece0.ResetMaterials();
        piece1.ResetMaterials();

        Object.Destroy(hint0.FrontMaterial);
        Object.Destroy(hint0.FrontMaterial);
        Object.Destroy(hint1.BackAndSidesMaterial);
        Object.Destroy(hint1.BackAndSidesMaterial);

        _hint0 = null;
        _hint1 = null;
    }

    public void ClearHintIfConnected(Piece[] connectedPieces)
    {
        if (_hint0 == null) return;
        
        Debug.Assert(_hint0 != null && _hint1 != null, 
            "Hint must be applied to both pieces");
        
        var seen = 0;

        foreach (var piece in connectedPieces)
        {
            if (piece == _hint0.Value.Piece) seen++;
            if (piece == _hint1.Value.Piece) seen++;

            if (seen == 2)
            {
                ClearHint();
                return;
            }
        }
    }

    private readonly struct PieceHint
    {
        public readonly Piece Piece;
        public readonly Material FrontMaterial;
        public readonly Material BackAndSidesMaterial;

        public PieceHint(Piece piece, Material frontMaterial, Material backAndSidesMaterial)
        {
            Piece = piece;
            FrontMaterial = frontMaterial;
            BackAndSidesMaterial = backAndSidesMaterial;
        }
    }
}
