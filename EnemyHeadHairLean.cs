using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000063 RID: 99
public class EnemyHeadHairLean : MonoBehaviour
{
	// Token: 0x0600031E RID: 798 RVA: 0x0001E938 File Offset: 0x0001CB38
	private void Update()
	{
		if (this.RandomTimer <= 0f && this.Agent.velocity.magnitude > 0.1f)
		{
			this.RandomTimer = Random.Range(this.RandomTimeMin, this.RandomTimeMax);
			this.RandomCurrent = Random.Range(this.RandomMin, this.RandomMax);
		}
		else
		{
			this.RandomTimer -= Time.deltaTime;
		}
		float equilibriumPos = 0f;
		if (this.Agent.velocity.magnitude > 0.1f)
		{
			equilibriumPos = Mathf.Clamp(this.Agent.velocity.magnitude * this.Amount, -this.MaxAmount, this.MaxAmount) + this.RandomCurrent;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.SpringParams, Time.deltaTime, this.SpringFreq, this.SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.SpringCurrent, ref this.SpringVelocity, equilibriumPos, this.SpringParams);
		base.transform.localRotation = Quaternion.Euler(this.SpringCurrent, 0f, 0f);
	}

	// Token: 0x04000570 RID: 1392
	public NavMeshAgent Agent;

	// Token: 0x04000571 RID: 1393
	[Space]
	public float Amount = -500f;

	// Token: 0x04000572 RID: 1394
	public float MaxAmount = 20f;

	// Token: 0x04000573 RID: 1395
	[Space]
	public float RandomMin;

	// Token: 0x04000574 RID: 1396
	public float RandomMax;

	// Token: 0x04000575 RID: 1397
	private float RandomCurrent;

	// Token: 0x04000576 RID: 1398
	private float RandomTimer;

	// Token: 0x04000577 RID: 1399
	public float RandomTimeMin;

	// Token: 0x04000578 RID: 1400
	public float RandomTimeMax;

	// Token: 0x04000579 RID: 1401
	[Space]
	public float SpringFreq = 15f;

	// Token: 0x0400057A RID: 1402
	public float SpringDamping = 0.5f;

	// Token: 0x0400057B RID: 1403
	private float SpringTarget;

	// Token: 0x0400057C RID: 1404
	private float SpringCurrent;

	// Token: 0x0400057D RID: 1405
	private float SpringVelocity;

	// Token: 0x0400057E RID: 1406
	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();
}
