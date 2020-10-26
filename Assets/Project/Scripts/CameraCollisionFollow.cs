using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionFollow : MonoBehaviour
{
    public Transform followTarget;
    public PunpkinController myController;
    public Transform body;
    public Transform hmd;
    public bool useCameraCollisions = true;

    public void FixedUpdate()
    {
        if (useCameraCollisions && !myController.usingViewPoint)
        {
            int layerMask = 1 << 8;
            RaycastHit hit;
            var body2cam = (followTarget.position + hmd.localPosition) - body.position;
            if (Physics.Raycast(body.position, ((followTarget.position + hmd.localPosition) - body.position).normalized, out hit, myController.cameraOffset.magnitude, layerMask))
            {
                transform.position = followTarget.position + ((hit.point - body.position) - body2cam);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
