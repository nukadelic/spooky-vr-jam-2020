using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCandy : MonoBehaviour
{

    private void Start( )
    {
        var c = GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
        GetComponentInChildren<MeshRenderer>().material.SetColor( "color_hdr" , c );
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 10)
            BeEaten(collision);

    }

    public void BeEaten(Collision col)
    {
        col.collider.gameObject.GetComponentInParent<PunpkinController>().AddCandyPoint();
        PlayConsumedAnimation();
        gameObject.SetActive(false);
    }
    public void PlayConsumedAnimation()
    {


    }
}
