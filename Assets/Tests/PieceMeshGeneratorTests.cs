using NUnit.Framework;
using UnityEngine;

public class PieceMeshGeneratorTests
{
    private const float Tolerance = 1e-6f;
    
    [Test]
    public void TestCreateSquarePieceMesh()
    {
        Mesh mesh = PieceMeshGenerator.CreateSquarePieceMesh(0.1f, 0.01f);
        
        Assert.IsNotNull(mesh);
        
        Assert.AreEqual(8, mesh.vertices.Length);
        
        Assert.AreEqual(0.1f, mesh.bounds.size.x, Tolerance);
        Assert.AreEqual(0.1f, mesh.bounds.size.y, Tolerance);
        Assert.AreEqual(0.01f, mesh.bounds.size.z, Tolerance);
    }
}