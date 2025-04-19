using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class PlayerAvatarLeftArm : MonoBehaviour
{
	// Token: 0x06000E62 RID: 3682 RVA: 0x000813E1 File Offset: 0x0007F5E1
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x000813F0 File Offset: 0x0007F5F0
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		if (this.playerAvatar.playerHealth.hurtFreeze)
		{
			return;
		}
		if (this.flashlightController.currentState > FlashlightController.State.Hidden && this.flashlightController.currentState < FlashlightController.State.Outro && !this.playerAvatar.playerAvatarVisuals.animInCrawl)
		{
			this.SetPose(this.flashlightPose);
			this.HeadAnimate(true);
			this.AnimatePose();
			return;
		}
		this.SetPose(this.basePose);
		this.HeadAnimate(false);
		this.AnimatePose();
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00081480 File Offset: 0x0007F680
	private void HeadAnimate(bool _active)
	{
		if (_active)
		{
			float num = this.playerAvatar.localCameraRotation.eulerAngles.x;
			if (num > 90f)
			{
				num -= 360f;
			}
			this.headRotation = Mathf.Lerp(this.headRotation, num * 0.5f, 20f * Time.deltaTime);
			return;
		}
		this.headRotation = Mathf.Lerp(this.headRotation, 0f, 20f * Time.deltaTime);
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000814FC File Offset: 0x0007F6FC
	private void AnimatePose()
	{
		if (this.poseLerp < 1f)
		{
			this.poseLerp += this.poseSpeed * Time.deltaTime;
			this.poseCurrent = Vector3.LerpUnclamped(this.poseOld, this.poseNew, this.poseCurve.Evaluate(this.poseLerp));
		}
		Quaternion rotation = this.leftArmTransform.rotation;
		this.leftArmTransform.localEulerAngles = new Vector3(this.poseCurrent.x, this.poseCurrent.y - this.headRotation, this.poseCurrent.z);
		Quaternion rotation2 = this.leftArmTransform.rotation;
		this.leftArmTransform.rotation = rotation;
		this.leftArmTransform.rotation = SemiFunc.SpringQuaternionGet(this.poseSpring, rotation2, -1f);
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000815CF File Offset: 0x0007F7CF
	private void SetPose(Vector3 _poseNew)
	{
		if (this.poseNew != _poseNew)
		{
			this.poseOld = this.poseCurrent;
			this.poseNew = _poseNew;
			this.poseLerp = 0f;
		}
	}

	// Token: 0x0400177B RID: 6011
	public PlayerAvatar playerAvatar;

	// Token: 0x0400177C RID: 6012
	public Transform leftArmTransform;

	// Token: 0x0400177D RID: 6013
	public FlashlightController flashlightController;

	// Token: 0x0400177E RID: 6014
	[Space]
	public AnimationCurve poseCurve;

	// Token: 0x0400177F RID: 6015
	public float poseSpeed;

	// Token: 0x04001780 RID: 6016
	private float poseLerp;

	// Token: 0x04001781 RID: 6017
	private Vector3 poseNew;

	// Token: 0x04001782 RID: 6018
	private Vector3 poseOld;

	// Token: 0x04001783 RID: 6019
	private Vector3 poseCurrent;

	// Token: 0x04001784 RID: 6020
	[Space]
	public Vector3 basePose;

	// Token: 0x04001785 RID: 6021
	public Vector3 flashlightPose;

	// Token: 0x04001786 RID: 6022
	public SpringQuaternion poseSpring;

	// Token: 0x04001787 RID: 6023
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001788 RID: 6024
	private float headRotation;
}
