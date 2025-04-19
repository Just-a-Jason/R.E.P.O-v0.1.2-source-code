using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class EnemyStateChaseSlow : MonoBehaviour
{
	// Token: 0x060005B1 RID: 1457 RVA: 0x000384F4 File Offset: 0x000366F4
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00038504 File Offset: 0x00036704
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseSlow)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.ChaseAhead();
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (Vector3.Distance(base.transform.position, this.Enemy.NavMeshAgent.Agent.destination) < 1f)
		{
			this.ChaseAhead();
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.ChaseEnd;
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x000385F0 File Offset: 0x000367F0
	private void ChaseAhead()
	{
		LevelPoint levelPointAhead = this.Enemy.GetLevelPointAhead(this.Enemy.StateChase.ChasePosition);
		if (levelPointAhead)
		{
			this.Enemy.StateChase.ChasePosition = levelPointAhead.transform.position;
		}
		this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.ChasePosition);
	}

	// Token: 0x0400094B RID: 2379
	private Enemy Enemy;

	// Token: 0x0400094C RID: 2380
	private bool Active;

	// Token: 0x0400094D RID: 2381
	public float Speed;

	// Token: 0x0400094E RID: 2382
	public float Acceleration;

	// Token: 0x0400094F RID: 2383
	[Space]
	public float StateTimeMin;

	// Token: 0x04000950 RID: 2384
	public float StateTimeMax;

	// Token: 0x04000951 RID: 2385
	private float StateTimer;
}
