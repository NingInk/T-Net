//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using TNet;

/// <summary>
/// Extended car that adds TNet-based multiplayer support.
/// </summary>

[RequireComponent(typeof(TNObject))]
public class ExampleCar : ExampleCarNoNetworking
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

	protected override void Awake ()
	{
		base.Awake();
		tno = GetComponent<TNObject>();
	}

	/// <summary>
	/// Only the car's owner should be updating the movement axes, and the result should be sync'd with other players.
	/// </summary>

	protected override void Update ()
	{
		// Only the player that actually owns this car should be controlling it
		// 只有真正拥有这辆车的玩家才能控制它
		if (!tno.isMine) return;

		// Update the input axes
		// 更新输入轴
		base.Update();

		// Objects get marked as destroyed while being transferred from one channel to another
		// 从一个通道转移到另一个通道时，对象被标记为已销毁
		if (tno.hasBeenDestroyed) return;

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
			if (Tools.IsNotEqual(mLastInput.x, mInput.x, threshold) ||
				Tools.IsNotEqual(mLastInput.y, mInput.y, threshold))
			{
				mLastInputSend = time;
				mLastInput = mInput;
				tno.Send("SetAxis", Target.OthersSaved, mInput);
			}
		}

		// Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
		// Faster-paced games will require more frequent updates.
		// 由于输入频繁发送，rigidbody只需要每几秒钟更正一次。
		// 快节奏的游戏需要更频繁的更新。
		if (mNextRB < time)
		{
			mNextRB = time + 1f / rigidbodyUpdates;
			tno.Send("SetRB", Target.OthersSaved, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
		}
	}

	/// <summary>
	/// RFC for the input will be called several times per second.
	/// </summary>

	[RFC]
	protected void SetAxis (Vector2 v) { mInput = v; }

	/// <summary>
	/// RFC for the rigidbody will be called once per second by default.
	/// </summary>

	[RFC]
	protected void SetRB (Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
	{
		mRb.position = pos;
		mRb.rotation = rot;
		mRb.velocity = vel;
		mRb.angularVelocity = angVel;
	}
}
