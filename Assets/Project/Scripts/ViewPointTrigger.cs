using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointTrigger : MonoBehaviour
{

    private PunpkinController localController;
    public Transform thisViewpoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            localController = other.gameObject.GetComponentInParent<PunpkinController>();
            localController.viewPoint = thisViewpoint;
            localController.usingViewPoint = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {

            localController.viewPoint = null;
            localController.usingViewPoint = false;
        }
    }
}
