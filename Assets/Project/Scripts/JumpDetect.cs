using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetect : MonoBehaviour
{
    public bool touchingGround = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8)
            touchingGround = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8)
            touchingGround = false;
    }
}
