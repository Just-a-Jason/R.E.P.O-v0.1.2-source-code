using System;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class EnemyHeadBotTilt : MonoBehaviour
{
	// Token: 0x06000324 RID: 804 RVA: 0x0001EBF8 File Offset: 0x0001CDF8
	private void Update()
	{
		float equilibriumPos = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, base.transform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, equilibriumPos, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(0f, -this.SpringCurrent * 0.5f, this.SpringCurrent);
		this.ForwardPrev = base.transform.forward;
	}

	// Token: 0x04000590 RID: 1424
	private Vector3 ForwardPrev;

	// Token: 0x04000591 RID: 1425
	[Space]
	public float Amount = -500f;

	// Token: 0x04000592 RID: 1426
	public float MaxAmount = 20f;

	// Token: 0x04000593 RID: 1427
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x04000594 RID: 1428
	public float SpringDamping = 0.5f;

	// Token: 0x04000595 RID: 1429
	private float SpringTarget;

	// Token: 0x04000596 RID: 1430
	private float SpringCurrent;

	// Token: 0x04000597 RID: 1431
	private float SpringVelocity;

	// Token: 0x04000598 RID: 1432
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
