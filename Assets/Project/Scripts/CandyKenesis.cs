using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyKenesis : MonoBehaviour
{
    public List<Rigidbody> candyHoard = new List<Rigidbody>();
    public Rigidbody candyAnchor;

    public void RegisterCandy(Rigidbody candyRB)
    {
        candyHoard.Add(candyRB);
        CreateJoint(candyRB.GetComponent<Rigidbody>());
    }
    public void UnregisterCandy(Transform candyTrans)
    {
        candyHoard.Remove(candyTrans);

    }
    void Start()
    {
        jd.positionDamper = 1f;
        limit.limit = 0.38f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach()
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
}
