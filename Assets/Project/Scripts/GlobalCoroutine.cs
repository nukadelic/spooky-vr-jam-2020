using UnityEngine;
using System.Collections;

public class GlobalCoroutine : MonoBehaviour
{
    public static GlobalCoroutine instance;

    void Start( )
    {
        if( instance != null )
        {
            Destroy( gameObject );
            return;
        }

        instance = this;

        DontDestroyOnLoad( gameObject );

        //GetComponent<AudioSource>().Play();
    }
}
