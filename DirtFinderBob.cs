using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class DirtFinderBob : MonoBehaviour
{
	// Token: 0x06000CA6 RID: 3238 RVA: 0x0006F816 File Offset: 0x0006DA16
	private void Start()
	{
		this.CameraBob = CameraBob.Instance;
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0006F824 File Offset: 0x0006DA24
	private void Update()
	{
		if (GameManager.Multiplayer() && !this.PlayerAvatar.isLocal)
		{
			return;
		}
		this.TargetPosY = this.CameraBob.transform.localRotation.y * this.PosYMultiplier;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosY, Time.deltaTime, this.SpringFreqPosY, this.SpringDampingPosY);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosY, ref this.VelocityPosY, this.TargetPosY, this.SpringParamsPosY);
		this.TargetPosZ = this.CameraBob.transform.localRotation.z * this.PosZMultiplier;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosZ, Time.deltaTime, this.SpringFreqPosZ, this.SpringDampingPosZ);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosZ, ref this.VelocityPosZ, this.TargetPosZ, this.SpringParamsPosZ);
		base.transform.localPosition = new Vector3(0f, this.CurrentPosY * 0.0025f + CameraJump.instance.transform.localPosition.y, 0f);
		base.transform.localRotation = Quaternion.Euler(CameraJump.instance.transform.localRotation.eulerAngles.x * 2f, 0f, 0f);
	}

	// Token: 0x0400140B RID: 5131
	public CameraBob CameraBob;

	// Token: 0x0400140C RID: 5132
	public PlayerAvatar PlayerAvatar;

	// Token: 0x0400140D RID: 5133
	[Space]
	public float PosZMultiplier = 1f;

	// Token: 0x0400140E RID: 5134
	public float SpringFreqPosZ = 15f;

	// Token: 0x0400140F RID: 5135
	public float SpringDampingPosZ = 0.5f;

	// Token: 0x04001410 RID: 5136
	private float TargetPosZ;

	// Token: 0x04001411 RID: 5137
	private float CurrentPosZ;

	// Token: 0x04001412 RID: 5138
	private float VelocityPosZ;

	// Token: 0x04001413 RID: 5139
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04001414 RID: 5140
	[Space]
	public float PosYMultiplier = 1f;

	// Token: 0x04001415 RID: 5141
	public float SpringFreqPosY = 15f;

	// Token: 0x04001416 RID: 5142
	public float SpringDampingPosY = 0.5f;

	// Token: 0x04001417 RID: 5143
	private float TargetPosY;

	// Token: 0x04001418 RID: 5144
	private float CurrentPosY;

	// Token: 0x04001419 RID: 5145
	private float VelocityPosY;

	// Token: 0x0400141A RID: 5146
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();
}
