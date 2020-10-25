using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyKenesis : MonoBehaviour
{
    public List<Rigidbody> candyHoard = new List<Rigidbody>();
    public Rigidbody candyAnchor;
    public XRInputs deviceBridge;
    public void RegisterCandy(Rigidbody candyRB)
    {
        candyHoard.Add(candyRB);
        CreateJoint(candyRB.GetComponent<Rigidbody>());
    }
    public void UnregisterCandy(Rigidbody candyRB)
    {
        candyHoard.Remove(candyRB);

    }
    void Start()
    {
        jd.positionDamper = 3f;
        jd.maximumForce = 10f;
        limit.limit = 0.28f;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            RegisterCandy(rb);
    }

    // Update is called once per frame
    public Vector3 force;
    void FixedUpdate()
    {

        foreach (Rigidbody candy in candyHoard)
            candy.AddForce(force * 20f); 
    }

    private JointDrive jd = new JointDrive();
    private SoftJointLimit limit = new SoftJointLimit();
    public void CreateJoint(Rigidbody rb)
    {
        var tempJoint = rb.gameObject.AddComponent<ConfigurableJoint>();
        rb.transform.position = candyAnchor.position;
        tempJoint.anchor = Vector3.zero;
        tempJoint.connectedBody = candyAnchor;

        tempJoint.xMotion = ConfigurableJointMotion.Limited;
        tempJoint.yMotion = ConfigurableJointMotion.Limited;
        tempJoint.zMotion = ConfigurableJointMotion.Limited;

        tempJoint.linearLimit = limit;

        tempJoint.angularXDrive = jd;
        tempJoint.angularYZDrive = jd;
        tempJoint.xDrive = jd;
        tempJoint.yDrive = jd;
        tempJoint.zDrive = jd;

        tempJoint.enableCollision = false;
        tempJoint.enablePreprocessing = false;
        
    }

    public float Wrap180(float value)
    {
        while (value > 180f)
        {
            value -= 360;
        }
        while (value < -180f)
        {
            value += 360f;
        }
        return value;
    }
}
