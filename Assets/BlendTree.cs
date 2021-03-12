using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTree : MonoBehaviour
{

    float ver, hor;

    float speed = 4;



    public Transform target;

    Vector3 mPos;
    Quaternion mRot;
    Transform mTrans;

    public float rot_speed = 10;


    bool m_Jump;
    private void Awake()
    {
        mTrans = transform;
        mPos = mTrans.position;
        mRot = mTrans.rotation;
    }

    //void Update()
    //{

    //    //if (Input.GetKeyDown(KeyCode.Space))
    //    //{
    //    //    GetComponent<Animator>().SetTrigger("Jump");
    //    //}
    //    float mx = 0;
    //    if (Input.GetMouseButton(1))
    //    {
    //        mx = Input.GetAxis("Mouse X");
    //        Debug.Log(mx);
    //    }

    //    //float my = Input.GetAxis("Mouse Y");


    //    transform.eulerAngles += (Vector3.up * mx * Time.deltaTime * rot_speed);


    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        if (speed < 6)
    //            speed += Time.deltaTime * 3;
    //        if (speed > 6)
    //            speed = 6;
    //    }
    //    else
    //    {
    //        if (speed > 1.5f)
    //            speed -= Time.deltaTime * 3;
    //        if (speed < 1.5f)
    //            speed = 1.5f;
    //    }

    //    bool crouch = Input.GetKey(KeyCode.C);
    //    if (!m_Jump)
    //    {
    //        m_Jump = Input.GetKey(KeyCode.Space);
    //    }
    //    //Debug.Log("speed:" + speed);

    //    ver = Input.GetAxis("Vertical");
    //    hor = Input.GetAxis("Horizontal");
    //    //if (ver != 0)
    //    //    Debug.Log("Vertical:" + ver);
    //    //if (hor != 0)
    //    //    Debug.Log("Horizontal:" + hor);

    //    GetComponent<Animator>().SetFloat("Forward", ver * speed);
    //    GetComponent<Animator>().SetFloat("Turn", hor * speed / 2);
    //}
    private void LateUpdate()
    {
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
