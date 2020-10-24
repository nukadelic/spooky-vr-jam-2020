using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunpkinController : MonoBehaviour
{
    public CandyKenesis myCandyHoard;
    public XRInputs deviceBridge;
    public Transform cameraOffsetTransform;
    public Transform cameraRotationTransform;
    public Transform bodyTransform;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    private float currentRotation;
    public JumpDetect groundDetection;

    private Vector3 force;


    protected virtual void OnEnable()
    {

        Application.onBeforeRender += OnBeforeRender;
    }

    protected virtual void OnDisable()
    {

        Application.onBeforeRender -= OnBeforeRender;
    }

    public void Update()
    {
        if (Mathf.Abs(deviceBridge.rightController_joystick.x) > 0.2f)
            currentRotation += deviceBridge.rightController_joystick.x * 3f;
        force = Vector3.zero;
        force.x = deviceBridge.leftController_joystick.x;
        force.z = deviceBridge.leftController_joystick.y;

        force = Quaternion.Euler(0f, currentRotation, 0f) * force;
        if (groundDetection.touchingGround && deviceBridge.rightController_triggerIsDown)
            force.y += 15f;
        myCandyHoard.force = force;

    }
    protected virtual void OnBeforeRender()
    {
        cameraRotationTransform.position = bodyTransform.position;
        cameraRotationTransform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);
        cameraOffsetTransform.localPosition = cameraOffset;
    }

}
