using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class BlendTreeTest : MonoBehaviour
{
    /// <summary>
    /// Maximum number of updates per second when synchronizing input axes.
    /// The actual number of updates may be less if nothing is changing.
    /// </summary>

    [Range(1f, 20f)]
    public float inputUpdates = 10f;

    /// <summary>
    /// Maximum number of updates per second when synchronizing the rigidbody.
    /// </summary>

    [Range(0.25f, 5f)]
    public float rigidbodyUpdates = 1f;

    /// <summary>
    /// We want to cache the network object (TNObject) we'll use for network communication.
    /// If the script was derived from TNBehaviour, this wouldn't have been necessary.
    /// </summary>

    [System.NonSerialized]
    public TNObject tno;

    protected Vector2 mLastInput;
    protected float mLastInputSend = 0f;
    protected float mNextRB = 0f;
    float ver, hor;

    float speed;


    Animator ani;
    protected void Awake()
    {
        ani = GetComponent<Animator>();
        tno = GetComponent<TNObject>();
    }

    void Update()
    {
        // Only the player that actually owns this car should be controlling it
        // 只有真正拥有这辆车的玩家才能控制它
        if (!tno.isMine) return;


        // Objects get marked as destroyed while being transferred from one channel to another
        // 从一个通道转移到另一个通道时，对象被标记为已销毁
        if (tno.hasBeenDestroyed) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (speed < 1)
                speed += Time.deltaTime;
            if (speed > 1)
                speed = 1;
        }
        else
        {
            if (speed > 0.5f)
                speed -= Time.deltaTime;
            if (speed < 0.5f)
                speed = 0.5f;
        }

        Debug.Log("speed:" + speed);

        ver = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");


        float time = Time.time;
        float delta = time - mLastInputSend;
        float delay = 1f / inputUpdates;


        // Don't send updates more than 20 times per second
        // 每秒发送更新不要超过20次
        if (delta > 0.05f)
        {
            // The closer we are to the desired send time, the smaller is the deviation required to send an update.
            // 我们越接近期望的发送时间，发送更新所需的偏差就越小。
            float threshold = Mathf.Clamp01(delta - delay) * 0.5f;
            // If the deviation is significant enough, send the update to other players
            // 如果偏差足够明显，就将更新发送给其他玩家
            if (Tools.IsNotEqual(mLastInput.x, ver, threshold) ||
                Tools.IsNotEqual(mLastInput.y, hor, threshold))
            {
                Debug.Log("----------------------------------");

                mLastInputSend = time;
                mLastInput = new Vector2(ver, hor);
                tno.Send("SetAxis", Target.AllSaved, ver, hor, speed);
            }
        }
    }



    [RFC]
    protected void SetAxis(float ver, float hor, float speed)
    {
        ani.SetFloat("ver", ver * speed);
        ani.SetFloat("hor", hor * speed);
    }
}
