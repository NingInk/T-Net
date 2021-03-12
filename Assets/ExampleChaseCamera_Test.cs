using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class ExampleChaseCamera_Test : TNBehaviour
{
    static public Transform target;

    Vector3 mPos;
    Quaternion mRot;
    Transform mTrans;

    public override void OnStart()
    {
        base.OnStart();

        if (tno == null || tno.isMine)
        {
            mTrans = transform;
            mPos = mTrans.position;
            mRot = mTrans.rotation;
        }
        else Destroy(this);
    }

    void Update()
    {
        Bounds bo = GetComponentInParent<Collider>().bounds;
        transform.position = bo.center;
        //transform.position = new Vector3(bo.center.x, bo.max.y, bo.center.z);
        if (target)
        {
            Transform t = transform;
            Vector3 forward = t.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 pos = t.position;
            Quaternion rot = Quaternion.LookRotation(forward);

            float delta = Time.deltaTime;
            mPos = Vector3.Lerp(mPos, pos, delta * 8f);
            mRot = Quaternion.Slerp(mRot, rot, delta * 4f);

            target.position = mPos;
            target.rotation = mRot;
        }
    }
}
