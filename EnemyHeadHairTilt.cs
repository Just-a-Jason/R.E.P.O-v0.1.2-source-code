using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class EnemyHeadHairTilt : MonoBehaviour
{
	// Token: 0x06000322 RID: 802 RVA: 0x0001EAB0 File Offset: 0x0001CCB0
	private void Update()
	{
		float num = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, this.EnemyTransform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		if (this.RandomTimer <= 0f && num > 0.1f)
		{
			this.RandomTimer = Random.Range(this.RandomTimeMin, this.RandomTimeMax);
			this.RandomCurrent = Random.Range(this.RandomMin, this.RandomMax);
		}
		else
		{
			this.RandomTimer -= Time.deltaTime;
		}
		num += this.RandomCurrent;
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, num, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(0f, this.SpringCurrent, 0f);
		this.ForwardPrev = this.EnemyTransform.forward;
	}

	// Token: 0x04000580 RID: 1408
	public Transform EnemyTransform;

	// Token: 0x04000581 RID: 1409
	private Vector3 ForwardPrev;

	// Token: 0x04000582 RID: 1410
	[Space]
	public float Amount = -500f;

	// Token: 0x04000583 RID: 1411
	public float MaxAmount = 20f;

	// Token: 0x04000584 RID: 1412
	[Space]
	public float RandomMin;

	// Token: 0x04000585 RID: 1413
	public float RandomMax;

	// Token: 0x04000586 RID: 1414
	private float RandomCurrent;

	// Token: 0x04000587 RID: 1415
	private float RandomTimer;

	// Token: 0x04000588 RID: 1416
	public float RandomTimeMin;

	// Token: 0x04000589 RID: 1417
	public float RandomTimeMax;

	// Token: 0x0400058A RID: 1418
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x0400058B RID: 1419
	public float SpringDamping = 0.5f;

	// Token: 0x0400058C RID: 1420
	private float SpringTarget;

	// Token: 0x0400058D RID: 1421
	private float SpringCurrent;

	// Token: 0x0400058E RID: 1422
	private float SpringVelocity;

	// Token: 0x0400058F RID: 1423
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
