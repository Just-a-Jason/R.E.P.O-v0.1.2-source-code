using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000097 RID: 151
public class EnemyStateSpawn : MonoBehaviour
{
	// Token: 0x060005D0 RID: 1488 RVA: 0x000395A0 File Offset: 0x000377A0
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x000395B0 File Offset: 0x000377B0
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.Spawn)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.WaitTimer = 2f;
			this.Active = true;
		}
		if (this.WaitTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
			this.WaitTimer = 0f;
			return;
		}
		this.WaitTimer -= Time.deltaTime;
	}

	// Token: 0x040009A4 RID: 2468
	private Enemy Enemy;

	// Token: 0x040009A5 RID: 2469
	private bool Active;

	// Token: 0x040009A6 RID: 2470
	private float WaitTimer;

	// Token: 0x040009A7 RID: 2471
	public UnityEvent OnSpawn;
}
