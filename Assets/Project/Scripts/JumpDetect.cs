using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetect : MonoBehaviour
{
    public bool touchingGround = false;
    public Rigidbody myBody;
    public PunpkinController myController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8)
        {
            if (collision.GetContact(0).point.y - myBody.transform.position.y < -0.28f)
            {
                touchingGround = true;
                myController.Ground();
            }
        }
            
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8)
        {
            if (collision.GetContact(0).point.y - myBody.transform.position.y < -0.28f)
            {
                touchingGround = true;
                //myController.Ground();

            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == 8)
            touchingGround = false;
    }
}
