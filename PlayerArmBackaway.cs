using System;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class PlayerArmBackaway : MonoBehaviour
{
	// Token: 0x06000E00 RID: 3584 RVA: 0x0007E618 File Offset: 0x0007C818
	private void Update()
	{
		if (this.ArmCollision.Blocked)
		{
			this.SpringTarget = this.BackAwayTarget;
		}
		else
		{
			this.SpringTarget = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, this.SpringTarget, this.SpringParams);
		base.transform.localPosition = new Vector3(0f, 0f, this.SpringCurrent);
	}

	// Token: 0x040016E4 RID: 5860
	public PlayerArmCollision ArmCollision;

	// Token: 0x040016E5 RID: 5861
	[Space]
	public float BackAwayTarget;

	// Token: 0x040016E6 RID: 5862
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x040016E7 RID: 5863
	public float SpringDamping = 0.5f;

	// Token: 0x040016E8 RID: 5864
	private float SpringTarget;

	// Token: 0x040016E9 RID: 5865
	private float SpringCurrent;

	// Token: 0x040016EA RID: 5866
	private float SpringVelocity;

	// Token: 0x040016EB RID: 5867
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
