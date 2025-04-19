using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class PlayerCollisionController : MonoBehaviour
{
	// Token: 0x06000E90 RID: 3728 RVA: 0x00083B00 File Offset: 0x00081D00
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.GroundedDisableTimer > 0f)
		{
			this.GroundedDisableTimer -= Time.deltaTime;
		}
		base.transform.position = this.FollowTarget.position + this.Offset;
		base.transform.rotation = this.FollowTarget.rotation;
		if (PlayerController.instance.playerAvatarScript.fallDamageResetState || SemiFunc.MenuLevel())
		{
			this.ResetFalling();
		}
		PlayerTumble tumble = PlayerController.instance.playerAvatarScript.tumble;
		if (tumble && !this.Grounded && (!tumble.isTumbling || (float)tumble.physGrabObject.playerGrabbing.Count <= 0f))
		{
			if (GameDirector.instance.currentState == GameDirector.gameState.Main && this.fallLastY - base.transform.position.y > 0f)
			{
				this.fallDistance += Mathf.Abs(base.transform.position.y - this.fallLastY);
			}
			this.fallLastY = base.transform.position.y;
			if (PlayerController.instance.featherTimer > 0f || PlayerController.instance.antiGravityTimer > 0f)
			{
				this.fallDistance = 0f;
			}
		}
		else
		{
			this.fallLastY = base.transform.position.y;
			this.fallDistance = 0f;
		}
		if (LevelGenerator.Instance.Generated)
		{
			PlayerController.instance.playerAvatarScript.isGrounded = this.Grounded;
		}
		float b = 0f;
		bool flag = false;
		if (tumble && tumble.isTumbling)
		{
			if (tumble.physGrabObject.rbVelocity.magnitude > 6f)
			{
				this.tumbleVelocityTime += Time.deltaTime;
				if (this.tumbleVelocityTime > 0.5f || tumble.physGrabObject.rbVelocity.magnitude > 8f)
				{
					if (tumble.physGrabObject.rbVelocity.magnitude > 15f)
					{
						this.fallLoopStopTimer = 0f;
					}
					flag = true;
				}
			}
			else
			{
				this.tumbleVelocityTime = 0f;
			}
			b = Mathf.Clamp(tumble.physGrabObject.rbVelocity.magnitude / 20f, 0.8f, 1.25f);
		}
		this.fallLoopPitch = Mathf.Lerp(this.fallLoopPitch, b, 10f * Time.deltaTime);
		if (this.fallLoopStopTimer > 0f)
		{
			this.volume = 0f;
			this.fallLoopStopTimer -= Time.deltaTime;
			this.soundFallLoop.PlayLoop(false, 2f, 20f, this.fallLoopPitch);
			return;
		}
		if (!flag)
		{
			this.volume = 0f;
		}
		else
		{
			this.volume = Mathf.Lerp(this.volume, 1f, 0.75f * Time.deltaTime);
		}
		this.soundFallLoop.PlayLoop(flag, 5f, 5f, this.fallLoopPitch);
		this.soundFallLoop.LoopVolume = this.volume;
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x00083E36 File Offset: 0x00082036
	public void StopFallLoop()
	{
		this.fallLoopStopTimer = 1f;
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00083E43 File Offset: 0x00082043
	public void ResetFalling()
	{
		this.StopFallLoop();
		this.fallDistance = 0f;
	}

	// Token: 0x0400180C RID: 6156
	public Transform FollowTarget;

	// Token: 0x0400180D RID: 6157
	public Vector3 Offset;

	// Token: 0x0400180E RID: 6158
	public PlayerCollisionGrounded CollisionGrounded;

	// Token: 0x0400180F RID: 6159
	[Space]
	public float GroundedDisableTimer;

	// Token: 0x04001810 RID: 6160
	public bool Grounded;

	// Token: 0x04001811 RID: 6161
	internal float fallDistance;

	// Token: 0x04001812 RID: 6162
	private float fallLastY;

	// Token: 0x04001813 RID: 6163
	private float tumbleVelocityTime;

	// Token: 0x04001814 RID: 6164
	private float fallLoopPitch;

	// Token: 0x04001815 RID: 6165
	private float fallLoopStopTimer;

	// Token: 0x04001816 RID: 6166
	private float volume;

	// Token: 0x04001817 RID: 6167
	public Sound soundFallLoop;
}
