using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Piece : MonoBehaviour
{
    
    public List<Vector3> connectionPoints = new();
    
    private BoxCollider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    
}
