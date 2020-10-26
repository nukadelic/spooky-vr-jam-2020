
using UnityEngine;
using UnityEngine.SpatialTracking;

class SpaceButton : MonoBehaviour
{
    Vector3 deltaRot;

    void Start( )
    {
        deltaRot = new Vector3(
            Random.Range( 1f, 10f ),
            Random.Range( 1f, 10f ),
            Random.Range( 1f, 10f )
        );
    }

    void Update( )
    {
        transform.Rotate( deltaRot * Time.deltaTime, Space.Self );        
    }

    private void OnTriggerEnter( Collider other )
    {
        var driver = other.GetComponent<TrackedPoseDriver>();

        if( driver == null ) return;

        //if( other.GetComponent<TrackedPoseDriver> )

        if( other.tag == "Hand" )
        {

        }
    }

    private void OnTriggerExit( Collider other )
    {
        var driver = other.GetComponent<TrackedPoseDriver>();

        if( driver == null ) return;
    }
}
