using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    public static bool IsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3( 0, 1, 0.5f );

        //gameObject.SetActive( false );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
