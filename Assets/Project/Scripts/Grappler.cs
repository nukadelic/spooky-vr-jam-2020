﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public Rigidbody grappleBody;
    public ConfigurableJoint grappleJoint;
    public Transform aimIndicator;
    public Transform aimerTransform;
    public XRInputs deviceBridge;
    public LineRenderer lr;
    public Transform anchor;
    public PunpkinController myController;
    private SoftJointLimit limit = new SoftJointLimit();
    public Transform bodyAnchor;
    public void Start()
    {
        limit.limit = 10f;
    }
    void FixedUpdate()
    {
        RunGrapple();
    }
    public void Update()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, bodyAnchor.position);
            lr.SetPosition(1, anchor.position);
        }
        aimIndicator.rotation =Quaternion.Euler(0f, myController.currentRotation, 0f)  *deviceBridge.rightController_rotation;
    }
    public bool isGrappling = false;
    public void RunGrapple()
    {
        if (isGrappling && !deviceBridge.rightController_gripIsDown)
        {
            DeGrapple();
        }
        else if (!isGrappling && deviceBridge.rightController_gripIsDown)
        {
            GrappleCollisionCheck();
        }else if (isGrappling)
        {
           // GrappleCollisionCheck();
        }

    }

    public void GrappleCollisionCheck()
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(grappleBody.position, aimerTransform.forward, out hit, 20f, layerMask))
        {
            Grapple(hit);
            isGrappling = true;
            //Debug.DrawRay(grappleBody.position, aimerTransform.forward * hit.distance, Color.red);
        }
    }

    public void DeGrapple()
    {
        anchor.gameObject.SetActive(false);
        isGrappling = false;

    }
    public void Grapple(RaycastHit hit)
    {
        anchor.position = hit.point;
        anchor.gameObject.SetActive(true);

        limit.limit = Vector3.Distance(hit.point, grappleBody.position);
        grappleJoint.linearLimit = limit;
    }

}
