using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunpkinController : MonoBehaviour
{
    public CandyKenesis myCandyHoard;
    public XRInputs deviceBridge;
    public Transform cameraOffsetTransform;
    public Transform bodyTransform;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    protected virtual void OnEnable()
    {
        
        Application.onBeforeRender += OnBeforeRender;
    }

    protected virtual void OnDisable()
    {

        Application.onBeforeRender -= OnBeforeRender;
    }


    protected virtual void OnBeforeRender()
    {
        cameraOffsetTransform.position = bodyTransform.position + cameraOffset;
    }

}
