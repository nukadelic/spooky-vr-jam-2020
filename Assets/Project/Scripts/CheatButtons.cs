using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatButtons : MonoBehaviour
{

    public Light globalLight;
    public bool lightSwitch;

    public Rigidbody bodyRB;
    public Rigidbody[] playerRBs;
    public Transform spawnPointOne;
    public Transform spawnPointTwo;
    public Transform spawnPointThree;
    // Start is called before the first frame update

    public void Update()
    {
        KeyChecks();
    }
    public void KeyChecks()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)){
            Vector3[] localPositions = new Vector3[playerRBs.Length];
            for (int i = 0; i < playerRBs.Length; i++)
            {
                localPositions[i] = bodyRB.transform.InverseTransformPoint(playerRBs[i].position);
                playerRBs[i].transform.position = spawnPointOne.TransformPoint(localPositions[i]);
            }
            bodyRB.transform.position = spawnPointOne.position;


        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Vector3[] localPositions = new Vector3[playerRBs.Length];
            for (int i = 0; i < playerRBs.Length; i++)
            {
                localPositions[i] = bodyRB.transform.InverseTransformPoint(playerRBs[i].position);
                playerRBs[i].transform.position = spawnPointTwo.TransformPoint(localPositions[i]);
            }
            bodyRB.transform.position = spawnPointTwo.position;


        }

    }

    public void Teleport(Rigidbody[] rbs)
    {
        foreach (Rigidbody rb in rbs)
            rb.transform.position = spawnPointOne.position;

    }
}
