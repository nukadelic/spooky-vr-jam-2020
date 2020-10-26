using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class PunpkinController : MonoBehaviour
{
    public CandyKenesis myCandyHoard;
    public XRInputs deviceBridge;
    public Transform cameraOffsetTransform;
    public Transform cameraRotationTransform;
    public Transform bodyTransform;
    public Transform viewPoint;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    private float currentRotation;
    public JumpDetect groundDetection;
    public Grappler grappler;
    private Vector3 force;


    protected virtual void OnEnable()
    {

        Application.onBeforeRender += OnBeforeRender;
    }

    protected virtual void OnDisable()
    {

        Application.onBeforeRender -= OnBeforeRender;
    }

    public bool canDoubleJump = false;
    public bool jumped = false;
    public bool doubleJumped = false;
    public void Update()
    {
        if (usingViewPoint)
        {
            var toPump = bodyTransform.position - viewPoint.position;
            toPump.y = 0f;
            var dirToPump = Quaternion.LookRotation(toPump, Vector3.up).eulerAngles;
            currentRotation = dirToPump.y;

        }
        else if (Mathf.Abs(deviceBridge.rightController_joystick.x) > 0.2f)
            currentRotation += deviceBridge.rightController_joystick.x * 3f;

        force = Vector3.zero;

        force.x = deviceBridge.leftController_joystick.x;
        force.z = deviceBridge.leftController_joystick.y;

        if (grappler.isGrappling)
        {
            force *= 0.5f;
        }
        force = Quaternion.Euler(0f, currentRotation, 0f) * force;

        if (groundDetection.touchingGround && deviceBridge.rightController_triggerIsDown)
        {
            jumped = true;
            canDoubleJump = false;
            groundDetection.touchingGround = false;
            force = Vector3.zero;
            var velocity = myCandyHoard.candyAnchor.velocity;
            velocity.y = 6f;
            velocity = math.clamp(velocity, -100f, 10f);
            myCandyHoard.candyAnchor.velocity = velocity;
            foreach (Rigidbody candy in myCandyHoard.candyHoard)
                candy.velocity = velocity;

        }
        else if (!groundDetection.touchingGround && canDoubleJump && !doubleJumped && deviceBridge.rightController_triggerIsDown)
        {
            canDoubleJump = false;
            doubleJumped = true;
            groundDetection.touchingGround = false;
            // we should put a direction modifier here based on right controller rotation 
            force = Vector3.zero;
            var velocity = myCandyHoard.candyAnchor.velocity;
            velocity.y = 6f;
            velocity = math.clamp(velocity, -100f, 10f);
            myCandyHoard.candyAnchor.velocity = velocity;
            foreach (Rigidbody candy in myCandyHoard.candyHoard)
                candy.velocity = velocity;

        }
        if (deviceBridge.rightController_triggerValue < 0.5f && jumped && !doubleJumped)
            canDoubleJump = true;

        myCandyHoard.force = force;

    }
    // called by JumpDetect to reset the jumping/double jumping bools

    public void Ground()
    {
        canDoubleJump = false;
        jumped = false;
        doubleJumped = false;
    }
    public bool usingViewPoint = false;
    protected virtual void OnBeforeRender()
    {
        if (usingViewPoint)
        {
            cameraRotationTransform.position = viewPoint.position + new Vector3(0f, bodyTransform.position.y, 0f);
            cameraOffsetTransform.localPosition = Vector3.zero;
        }
        else
        {
            cameraRotationTransform.position = bodyTransform.position;
            cameraOffsetTransform.localPosition = cameraOffset;
        }
            cameraRotationTransform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);

    }


    public Transform[] ghostArms;
    public void GhostHandanimator()
    {
        // the ghost has two left arms and cannot be rigged as is (not without some quaternion math that isn't worth it)


    }
}
