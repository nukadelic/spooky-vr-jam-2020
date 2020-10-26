using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class DrawRope : MonoBehaviour
{
    public Spline spline;
    public SplineMeshTiling meshTiling;

    // Start is called before the first frame update
    void Start()
    {
        spline = GetComponentInChildren<Spline>();
        meshTiling = GetComponentInChildren<SplineMeshTiling>();

        var c = spline.nodes.Count;

        //while( spline.nodes.Count > 0 )
        {
            //spline.AddNode( new SplineNode() )
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
