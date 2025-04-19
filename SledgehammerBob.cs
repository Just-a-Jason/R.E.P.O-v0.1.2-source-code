using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class SledgehammerBob : MonoBehaviour
{
	// Token: 0x0600070A RID: 1802 RVA: 0x00042B67 File Offset: 0x00040D67
	private void Start()
	{
		this.CameraBob = GameDirector.instance.CameraBob;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00042B7C File Offset: 0x00040D7C
	private void Update()
	{
		this.TargetPosY = this.CameraBob.transform.localRotation.z * -500f;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosY, Time.deltaTime, this.SpringFreqPosY, this.SpringDampingPosY);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosY, ref this.VelocityPosY, this.TargetPosY, this.SpringParamsPosY);
		this.TargetPosZ = this.CameraBob.transform.localPosition.y * -0.5f;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParamsPosZ, Time.deltaTime, this.SpringFreqPosZ, this.SpringDampingPosZ);
		SpringUtils.UpdateDampedSpringMotion(ref this.CurrentPosZ, ref this.VelocityPosZ, this.TargetPosZ, this.SpringParamsPosZ);
		base.transform.localRotation = Quaternion.Euler(-this.CurrentPosY, 0f, this.CurrentPosY);
		base.transform.localPosition = new Vector3(0f, 0f, this.CameraBob.transform.localPosition.y + this.CurrentPosZ);
	}

	// Token: 0x04000C13 RID: 3091
	public CameraBob CameraBob;

	// Token: 0x04000C14 RID: 3092
	[Space]
	public float SpringFreqPosZ = 15f;

	// Token: 0x04000C15 RID: 3093
	public float SpringDampingPosZ = 0.5f;

	// Token: 0x04000C16 RID: 3094
	private float TargetPosZ;

	// Token: 0x04000C17 RID: 3095
	private float CurrentPosZ;

	// Token: 0x04000C18 RID: 3096
	private float VelocityPosZ;

	// Token: 0x04000C19 RID: 3097
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	// Token: 0x04000C1A RID: 3098
	[Space]
	public float SpringFreqPosY = 15f;

	// Token: 0x04000C1B RID: 3099
	public float SpringDampingPosY = 0.5f;

	// Token: 0x04000C1C RID: 3100
	private float TargetPosY;

	// Token: 0x04000C1D RID: 3101
	private float CurrentPosY;

	// Token: 0x04000C1E RID: 3102
	private float VelocityPosY;

	// Token: 0x04000C1F RID: 3103
	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();
}
