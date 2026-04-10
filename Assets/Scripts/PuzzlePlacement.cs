using System.Collections.Generic;
using PuzzleGeneration;
using UnityEngine;

public static class PuzzlePlacement
{
    public static BoundingGrid GetBoundingGrid(PuzzleLayout layout)
    {
        var cells = new List<Cell>();
        var maxRow = 0;
        var maxCol = 0;

        foreach (var cut in layout.initialPieceCuts)
        {
            var currCell = new Cell
            {
                Row = cut.row,
                Col = cut.col
            };
            cells.Add(currCell);
            
            if (cut.row > maxRow) maxRow = cut.row;
            if (cut.col > maxCol) maxCol = cut.col;
        }
        
        var boundingWidth = layout.width * 3f;
        var boundingHeight = layout.height * 3f;

        return new BoundingGrid 
        {
            Cells = cells,
            CellWidth = boundingWidth / (maxCol + 1),
            CellHeight = boundingHeight / (maxRow + 1),
        };
    }
    
    public static void ShuffleCells(List<Cell> cells)
    {
        for (var i = cells.Count - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }
    }

    public static Vector3 RandomPositionInCell(
        Cell cell,
        BoundingGrid gridCellDimensions, 
        Piece piece
    )
    {
        var localBounds = piece.GetVertexBounds();
        
        var cellMinX = cell.Col * gridCellDimensions.CellWidth;
        var cellMinY = cell.Row * gridCellDimensions.CellHeight;
        var cellMaxX = cellMinX + gridCellDimensions.CellWidth;
        var cellMaxY = cellMinY + gridCellDimensions.CellHeight;

        var posX = Random.Range(cellMinX - localBounds.min.x, cellMaxX - localBounds.max.x);
        var posY = Random.Range(cellMinY - localBounds.min.y, cellMaxY - localBounds.max.y);

        return new Vector3(posX, posY);
    }

    public static Quaternion RandomRotationZ()
    {
        return Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
    }

    public struct BoundingGrid
    {
        public List<Cell> Cells;
        public float CellWidth;
        public float CellHeight;
    }
    
    public struct Cell
    {
        public float Row;
        public float Col;
    }
}
