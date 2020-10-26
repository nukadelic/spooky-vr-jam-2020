using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    public static HandScript Left;
    public static HandScript Right;

    public bool IsLeftHand = true;

    void Start()
    {
        if( IsLeftHand ) Left = this;
        else Right = this;
    }
}
