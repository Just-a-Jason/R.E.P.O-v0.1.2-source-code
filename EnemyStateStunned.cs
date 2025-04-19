using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000098 RID: 152
public class EnemyStateStunned : MonoBehaviour
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x00039634 File Offset: 0x00037834
	private void Start()
	{
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00039644 File Offset: 0x00037844
	private void Update()
	{
		if (this.stunTimer > 0f)
		{
			this.enemy.CurrentState = EnemyState.Stunned;
			this.stunTimer -= Time.deltaTime;
			if (this.stunTimer <= 0f)
			{
				this.enemy.CurrentState = EnemyState.Roaming;
			}
		}
		if (this.enemy.CurrentState != EnemyState.Stunned)
		{
			if (this.active)
			{
				if (this.enemy.HasRigidbody && this.enemy.HasNavMeshAgent)
				{
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				}
				this.onStunnedEnd.Invoke();
				this.active = false;
			}
			return;
		}
		if (!this.active)
		{
			this.onStunnedStart.Invoke();
			this.active = true;
		}
		this.enemy.DisableChase(0.25f);
		if (this.enemy.HasRigidbody)
		{
			this.enemy.Rigidbody.DisableFollowPosition(0.1f, this.enemy.Rigidbody.stunResetSpeed);
			this.enemy.Rigidbody.DisableFollowRotation(0.1f, this.enemy.Rigidbody.stunResetSpeed);
			this.enemy.Rigidbody.DisableNoGravity(0.1f);
			this.enemy.Rigidbody.physGrabObject.OverrideDrag(0.05f, 0.1f);
			this.enemy.Rigidbody.physGrabObject.OverrideAngularDrag(0.05f, 0.1f);
		}
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x000397D6 File Offset: 0x000379D6
	public void Spawn()
	{
		this.stunTimer = 0f;
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x000397E3 File Offset: 0x000379E3
	public void Set(float time)
	{
		if (time > this.stunTimer && this.enemy.TeleportedTimer <= 0f)
		{
			this.stunTimer = time;
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00039807 File Offset: 0x00037A07
	public void Reset()
	{
		this.stunTimer = 0.1f;
	}

	// Token: 0x040009A8 RID: 2472
	private Enemy enemy;

	// Token: 0x040009A9 RID: 2473
	private bool active;

	// Token: 0x040009AA RID: 2474
	[HideInInspector]
	public float stunTimer;

	// Token: 0x040009AB RID: 2475
	[Space]
	public UnityEvent onStunnedStart;

	// Token: 0x040009AC RID: 2476
	public UnityEvent onStunnedEnd;
}
