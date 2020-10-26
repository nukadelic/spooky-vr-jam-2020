using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCandy : MonoBehaviour
{


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
