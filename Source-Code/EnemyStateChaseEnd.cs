using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class EnemyStateChaseEnd : MonoBehaviour
{
	// Token: 0x060005AE RID: 1454 RVA: 0x0003841C File Offset: 0x0003661C
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00038438 File Offset: 0x00036638
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseEnd)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Enemy.NavMeshAgent.ResetPath();
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(0f, 5f);
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x04000945 RID: 2373
	private Enemy Enemy;

	// Token: 0x04000946 RID: 2374
	private PlayerController Player;

	// Token: 0x04000947 RID: 2375
	private bool Active;

	// Token: 0x04000948 RID: 2376
	public float StateTimeMin;

	// Token: 0x04000949 RID: 2377
	public float StateTimeMax;

	// Token: 0x0400094A RID: 2378
	private float StateTimer;
}
