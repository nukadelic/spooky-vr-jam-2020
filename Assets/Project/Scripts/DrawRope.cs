using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class DrawRope : MonoBehaviour
{
    Spline spline;
    SplineMeshTiling meshTiling;

    public Transform targetA;
    public Transform targetB;
    
    void Start()
    {
        spline = GetComponentInChildren<Spline>();
        meshTiling = GetComponentInChildren<SplineMeshTiling>();

        var c = spline.nodes.Count;

        //meshTiling.updateInPlayMode = true;

        while( spline.nodes.Count > 2 ) spline.RemoveNode( spline.nodes[ 0 ] );
        while( spline.nodes.Count < 2 ) spline.AddNode( new SplineNode( Vector3.zero, Vector3.up ) );
    }

    public void StopDrawing()
    {
        gameObject.SetActive( false );
    }

    public void SetPoints( Vector3 pointA, Vector3 pointB )
    {
        if( ! gameObject.activeSelf )
            gameObject.SetActive( true );

        var nodeA = spline.nodes[ 0 ];
        var nodeB = spline.nodes[ 1 ];
        
        var pA = pointA - this.transform.position;
        var pB = pointB - this.transform.position;

        var delta = pB - pA;

        nodeA.Position = pointA - this.transform.position;
        nodeB.Position = pointB - this.transform.position;
        
        if( pA.y > pB.y )
        {
            nodeA.Direction = pA + Vector3.down * delta.magnitude / 4f;
            nodeB.Direction = pB + delta / 2f;
        }
        else
        {
            nodeB.Direction = pB + Vector3.down * delta.magnitude / 4f;
            nodeA.Direction = pA + delta / 2f;
        }
        
        //spline.RefreshCurves();
    }
    
    void Update()
    {
        if( targetA != null && targetB != null )
        {
            SetPoints( targetA.transform.position, targetB.transform.position );
        }
    }
}
