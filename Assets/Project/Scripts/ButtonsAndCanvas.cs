using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsAndCanvas : MonoBehaviour
{
    public Button button;
    
    void Start()
    {
        button?.onClick.AddListener( BtnClick );
    }

    void BtnClick()
    {
        var canvas = GetComponentInChildren<Canvas>();

        canvas.transform.position += Vector3.up * 0.05f;
    }

    void Update()
    {
        
    }
}
