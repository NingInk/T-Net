using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TNet;

public class CameraController : MonoBehaviour
{
    public Vector3 firstView;
    public Vector3 sitView;

    bool m_meditate = false;

    public bool Meditate
    {
        get => m_meditate;
        set
        {
            m_meditate = value;
            if (!value)
            {
                GoTOBack();
            }
        }
    }

    private void Update()
    {
        float angle = Vector3.Angle(transform.forward, Vector3.up);

        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(transform.parent.position, Vector3.up, mX * 3);

            if (angle < 20 && mY < 0 || angle > 150 && mY > 0 || 20 < angle && angle < 170)
                transform.RotateAround(transform.parent.position, transform.right, -mY * 3);
        }
        else if (Input.GetMouseButtonUp(1) && !Meditate /*&& transform.localEulerAngles.y != 0*/)
        {
            GoTOBack();
        }
        else if (Meditate)
        {
            transform.RotateAround(transform.parent.position, transform.parent.up, Time.deltaTime);
        }
    }
    private void LateUpdate()
    {
        float scaleForce = -Input.GetAxis("Mouse ScrollWheel");
        if ((Mathf.Abs(transform.localPosition.z) > 10 && scaleForce > 0) || (Mathf.Abs(transform.localPosition.z) < 2.5f && scaleForce < 0))
            return;
        if (scaleForce == 0) return;
        float dis = scaleForce * 50 * Time.deltaTime;
        transform.localPosition += (transform.localPosition).normalized * dis;
        transform.LookAt(transform.parent);
    }
    void GoTOBack()
    {
        float dis = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z),
                 Vector2.zero);

        transform.DOLocalMove(new Vector3(0, transform.localPosition.y, -dis), 0.5f).onUpdate += () => { transform.LookAt(transform.parent); };
    }


    public void SetToFirstView()
    {

    }
    public void SetToSitView(bool bo)
    {
        if (bo)
            Debug.Log("设置为坐下的视角");
        else
            Debug.Log("战立");
    }
    public void SetToMeditateView(bool bo)
    {
        Meditate = bo;
    }
}
