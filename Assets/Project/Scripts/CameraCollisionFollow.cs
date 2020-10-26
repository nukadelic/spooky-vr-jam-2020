using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionFollow : MonoBehaviour
{
    public Transform followTarget;
    public PunpkinController myController;
    public Transform body;
    public bool useCameraCollisions = true;

    public void FixedUpdate()
    {
        if (useCameraCollisions)
        {
            int layerMask = 1 << 8;
            RaycastHit hit;
            if (Physics.Raycast(body.position, (followTarget.position - body.position).normalized, out hit, myController.cameraOffset.magnitude, layerMask))
            {
                transform.position = hit.point;
            }
        }
        else
        {
            followTarget.localPosition = myController.cameraOffset;
        }
    }
}
