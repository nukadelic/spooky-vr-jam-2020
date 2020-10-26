
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool inverse = true;

    Transform target;

    void Start() => target = Camera.current?.transform ?? Camera.main?.transform;
    
    void Update() 
    {
        if( ! inverse )
        {
            transform.LookAt( target );
        }
        else
        {
            var invTarget = target.position + ( transform.position - target.position ) * 2f;

            transform.LookAt( invTarget );
        }
    }
}
