using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000092 RID: 146
public class EnemyStateDespawn : MonoBehaviour
{
	// Token: 0x060005B5 RID: 1461 RVA: 0x00038664 File Offset: 0x00036864
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00038674 File Offset: 0x00036874
	private void Update()
	{
		if (this.Enemy.MasterClient)
		{
			if (this.ChaseDespawn)
			{
				this.ChaseDespawnLogic();
			}
			if (this.StuckDespawn)
			{
				this.StuckDespawnLogic();
			}
		}
		if (this.Enemy.CurrentState != EnemyState.Despawn)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Active = true;
		}
		bool masterClient = this.Enemy.MasterClient;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x000386E4 File Offset: 0x000368E4
	public void Despawn()
	{
		this.OnDespawn.Invoke();
		this.Enemy.EnemyParent.Despawn();
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00038704 File Offset: 0x00036904
	private void ChaseDespawnLogic()
	{
		if (this.DespawnAfterChaseTime == 0f)
		{
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			this.ChaseTimer += Time.deltaTime;
			this.ChaseResetTimer = 10f;
			if (this.ChaseTimer >= this.DespawnAfterChaseTime)
			{
				this.Enemy.CurrentState = EnemyState.Despawn;
				this.ChaseTimer = 0f;
				return;
			}
		}
		else
		{
			if (this.ChaseResetTimer <= 0f)
			{
				this.ChaseTimer = 0f;
				return;
			}
			this.ChaseResetTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x000387BC File Offset: 0x000369BC
	private void StuckDespawnLogic()
	{
		if (this.Enemy.StuckCount >= this.StuckDespawnCount && this.Enemy.CurrentState != EnemyState.Despawn)
		{
			this.Enemy.Vision.DisableTimer = 0.25f;
			this.Enemy.CurrentState = EnemyState.Despawn;
		}
	}

	// Token: 0x04000952 RID: 2386
	private Enemy Enemy;

	// Token: 0x04000953 RID: 2387
	private bool Active;

	// Token: 0x04000954 RID: 2388
	public bool StuckDespawn = true;

	// Token: 0x04000955 RID: 2389
	public int StuckDespawnCount = 10;

	// Token: 0x04000956 RID: 2390
	[Space]
	public bool ChaseDespawn = true;

	// Token: 0x04000957 RID: 2391
	public float DespawnAfterChaseTime = 10f;

	// Token: 0x04000958 RID: 2392
	internal float ChaseTimer;

	// Token: 0x04000959 RID: 2393
	internal float ChaseResetTimer;

	// Token: 0x0400095A RID: 2394
	[Space]
	public UnityEvent OnDespawn;
}
