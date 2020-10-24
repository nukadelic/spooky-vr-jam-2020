using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    // temp 

    [Header("Callibiration")]

    [Range(-0.2f,0.2f)] public float offset_pos_x = 0f;
    [Range(-0.2f,0.2f)] public float offset_pos_y = 0f;
    [Range(-0.2f,0.2f)] public float offset_pos_z = 0f;
    
    [Range(-90f,90f)] public float offset_angle_x = 0f;
    [Range(-90f,90f)] public float offset_angle_y = 0f;
    [Range(-90f,90f)] public float offset_angle_z = 0f;

    Animator animator;

   
    public bool isLeftHand = true;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3
        (
            offset_pos_x,
            offset_pos_y,
            offset_pos_z
        );

        transform.localRotation = Quaternion.Euler
        (
            offset_angle_x, 
            offset_angle_y,
            offset_angle_z
        );

        var grid_value = isLeftHand ? 
            XRInputs.instance.leftController_gripIsDown : 
            XRInputs.instance.rightController_gripIsDown;

        animator.SetBool( "isGrabbing", grid_value );
    }
}