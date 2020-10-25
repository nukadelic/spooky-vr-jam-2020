using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerPart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public LayerMask compareTo;

    // Update is called once per frame
    void Update()
    {
        Debug.Log( LayerMask.GetMask( compareTo.ToString() ) + " " + ( gameObject.layer ) );
    }
}
