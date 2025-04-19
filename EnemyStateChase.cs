using System;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class EnemyStateChase : MonoBehaviour
{
	// Token: 0x060005A8 RID: 1448 RVA: 0x00037C24 File Offset: 0x00035E24
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00037C40 File Offset: 0x00035E40
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Chase)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer = 0f;
			this.ChasePosition = this.Enemy.TargetPlayerAvatar.transform.position;
			this.VisionTimer = this.VisionTime;
			this.ChaseCanReachSet = false;
			this.SawPlayerHide = false;
			this.CantReachTime = 0f;
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (this.Enemy.Vision.VisionTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID])
		{
			this.VisionTimer = this.VisionTime;
		}
		else if (this.VisionTimer > 0f)
		{
			this.VisionTimer -= Time.deltaTime;
		}
		if (this.VisionTimer > 0f)
		{
			if (this.ChaseOnlyOnNavmesh || this.Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer <= 0.25f)
			{
				this.Enemy.NavMeshAgent.Enable();
				this.Enemy.NavMeshAgent.SetDestination(this.Enemy.TargetPlayerAvatar.LastNavmeshPosition);
				if (this.ChaseCanReachSet)
				{
					Vector3 point = this.Enemy.NavMeshAgent.GetPoint();
					if (Vector3.Distance(point, this.Enemy.TargetPlayerAvatar.transform.position) > 0.5f)
					{
						this.ChaseCanReach = false;
					}
					else
					{
						this.ChaseCanReach = true;
					}
					if (this.Enemy.TargetPlayerAvatar.isCrawling && !this.ChaseCanReach)
					{
						this.SawPlayerHidePosition = this.Enemy.TargetPlayerAvatar.transform.position;
						this.SawPlayerNavmeshPosition = this.Enemy.TargetPlayerAvatar.LastNavmeshPosition;
						this.SawPlayerHide = true;
					}
					this.ChasePosition = point;
				}
				this.ChaseCanReachSet = true;
			}
			else
			{
				this.Enemy.NavMeshAgent.Disable(0.1f);
				base.transform.position = Vector3.MoveTowards(base.transform.position, this.Enemy.TargetPlayerAvatar.transform.position, this.Speed * Time.deltaTime);
			}
		}
		else
		{
			if (this.SawPlayerHide)
			{
				this.Enemy.CurrentState = EnemyState.LookUnder;
				return;
			}
			this.Enemy.NavMeshAgent.SetDestination(this.ChasePosition);
			if (Vector3.Distance(base.transform.position, this.ChasePosition) < 1f)
			{
				LevelPoint levelPointAhead = this.Enemy.GetLevelPointAhead(this.ChasePosition);
				if (levelPointAhead)
				{
					this.Enemy.NavMeshAgent.SetDestination(levelPointAhead.transform.position);
				}
				this.ChasePosition = this.Enemy.NavMeshAgent.GetDestination();
			}
			this.ChaseCanReach = true;
			this.ChaseCanReachSet = false;
		}
		if (this.ChaseCanReach && this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] >= this.VisionsToReset)
		{
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
		}
		if (!this.ChaseCanReach)
		{
			this.CantReachTime += Time.deltaTime;
			if (this.CantReachTime > 2f)
			{
				this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
				this.Enemy.CurrentState = EnemyState.ChaseSlow;
				return;
			}
		}
		else
		{
			this.CantReachTime = 0f;
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.ChaseSlow;
		}
		if (this.Enemy.TargetPlayerAvatar.isDisabled)
		{
			this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x04000929 RID: 2345
	private Enemy Enemy;

	// Token: 0x0400092A RID: 2346
	private PlayerController Player;

	// Token: 0x0400092B RID: 2347
	private bool Active;

	// Token: 0x0400092C RID: 2348
	public float Speed;

	// Token: 0x0400092D RID: 2349
	public float Acceleration;

	// Token: 0x0400092E RID: 2350
	[Space]
	public float StateTimeMin;

	// Token: 0x0400092F RID: 2351
	public float StateTimeMax;

	// Token: 0x04000930 RID: 2352
	private float StateTimer;

	// Token: 0x04000931 RID: 2353
	[Space]
	public float VisionTime;

	// Token: 0x04000932 RID: 2354
	[HideInInspector]
	public float VisionTimer;

	// Token: 0x04000933 RID: 2355
	public int VisionsToReset;

	// Token: 0x04000934 RID: 2356
	[HideInInspector]
	public Vector3 ChasePosition = Vector3.zero;

	// Token: 0x04000935 RID: 2357
	[HideInInspector]
	public bool ChaseCanReach = true;

	// Token: 0x04000936 RID: 2358
	private bool ChaseCanReachSet;

	// Token: 0x04000937 RID: 2359
	private bool SawPlayerHide;

	// Token: 0x04000938 RID: 2360
	internal Vector3 SawPlayerNavmeshPosition;

	// Token: 0x04000939 RID: 2361
	internal Vector3 SawPlayerHidePosition;

	// Token: 0x0400093A RID: 2362
	private float CantReachTime;

	// Token: 0x0400093B RID: 2363
	[Space]
	public bool ChaseOnlyOnNavmesh = true;
}
