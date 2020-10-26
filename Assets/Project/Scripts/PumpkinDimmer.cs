using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinDimmer : MonoBehaviour
{

    public Light[] lights;
    public Transform proximityDim;
    [Range(0f, 10f)]
    public float maxDimDistance;
    [Range(0f, 50f)]
    public float maxIntensity;
    [Range(0f, 50f)]
    public float minIntensity;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = Mathf.Clamp(Vector3.Distance(transform.position, proximityDim.position) - 1f, 0f, maxDimDistance) / maxDimDistance * maxIntensity;
        }
    }


}
