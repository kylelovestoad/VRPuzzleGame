using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static float CrossProduct2D(Vector2 v0, Vector2 v1)
    {
        return v0.x * v1.y - v1.x * v0.y;
    }
    
    public static Vector2 Centroid(List<Vector2> borderPoints)
    {
        var pointsCount = borderPoints.Count;
        
        var area = 0f;
        var centroidX = 0f;
        var centroidY = 0f;

        for (var i = 0; i < pointsCount; i++)
        {
            var curr = borderPoints[i];
            var next = borderPoints[(i + 1) % pointsCount];

            var cross = Util.CrossProduct2D(curr, next);
            
            area += cross;
            centroidX += (curr.x + next.x) * cross;
            centroidY += (curr.y + next.y) * cross;
        }

        area *= 3f;
        centroidX /= area;
        centroidY /= area;

        return new Vector2(centroidX, centroidY);
    }
}
