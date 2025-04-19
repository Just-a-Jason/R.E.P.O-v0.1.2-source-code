using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000093 RID: 147
public class EnemyStateInvestigate : MonoBehaviour
{
	// Token: 0x060005BB RID: 1467 RVA: 0x00038836 File Offset: 0x00036A36
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00038844 File Offset: 0x00036A44
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
		this.Roaming = base.GetComponent<EnemyStateRoaming>();
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0003886C File Offset: 0x00036A6C
	private void Update()
	{
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Investigate)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.PhysObjectHitImpulse = true;
			this.PhysObjectHitCount = 0;
			this.Active = true;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		bool flag = this.Enemy.AttackStuckPhysObject.Check();
		if (this.Enemy.AttackStuckPhysObject.Active)
		{
			if (this.PhysObjectHitImpulse)
			{
				this.PhysObjectHitCount++;
				this.PhysObjectHitImpulse = false;
			}
		}
		else
		{
			this.PhysObjectHitImpulse = true;
		}
		if (!this.Enemy.NavMeshAgent.Agent.hasPath || (flag && !this.Enemy.AttackStuckPhysObject.Active) || this.PhysObjectHitCount >= this.PhysObjectHitMax)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
			this.Roaming.RoamingLevelPoint = this.InvestigateLevelPoint;
			if (this.PhysObjectHitCount >= this.PhysObjectHitMax)
			{
				this.Roaming.RoamingChangeCurrent = 0;
				return;
			}
			this.Roaming.RoamingChangeCurrent = Random.Range(this.Roaming.RoamingChangeMin, this.Roaming.RoamingChangeMax + 1);
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x000389E4 File Offset: 0x00036BE4
	public void Set(Vector3 position)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.PhotonView.RPC("SetRPC", RpcTarget.All, new object[]
				{
					position
				});
			}
		}
		else
		{
			this.SetRPC(position);
		}
		if (this.OnlyEvent)
		{
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.Roaming || this.Enemy.CurrentState == EnemyState.Investigate || this.Enemy.CurrentState == EnemyState.ChaseEnd)
		{
			this.Enemy.CurrentState = EnemyState.Investigate;
			float num = float.MaxValue;
			LevelPoint investigateLevelPoint = null;
			foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
			{
				float num2 = Vector3.Distance(position, levelPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
					investigateLevelPoint = levelPoint;
				}
			}
			this.InvestigateLevelPoint = investigateLevelPoint;
			this.Enemy.NavMeshAgent.SetDestination(position);
			return;
		}
		if (this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			this.Enemy.StateChase.ChasePosition = position;
			this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.ChasePosition);
		}
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00038B28 File Offset: 0x00036D28
	[PunRPC]
	public void SetRPC(Vector3 position)
	{
		this.onInvestigateTriggeredPosition = position;
		this.onInvestigateTriggered.Invoke();
	}

	// Token: 0x0400095B RID: 2395
	private Enemy Enemy;

	// Token: 0x0400095C RID: 2396
	private PlayerController Player;

	// Token: 0x0400095D RID: 2397
	private bool Active;

	// Token: 0x0400095E RID: 2398
	private EnemyStateRoaming Roaming;

	// Token: 0x0400095F RID: 2399
	private PhotonView PhotonView;

	// Token: 0x04000960 RID: 2400
	public float rangeMultiplier = 1f;

	// Token: 0x04000961 RID: 2401
	[Space]
	public bool OnlyEvent = true;

	// Token: 0x04000962 RID: 2402
	private bool PhysObjectHitImpulse;

	// Token: 0x04000963 RID: 2403
	private int PhysObjectHitCount;

	// Token: 0x04000964 RID: 2404
	public int PhysObjectHitMax = 3;

	// Token: 0x04000965 RID: 2405
	[Header("Movement")]
	public float Speed;

	// Token: 0x04000966 RID: 2406
	public float Acceleration;

	// Token: 0x04000967 RID: 2407
	[Header("Event")]
	public UnityEvent onInvestigateTriggered;

	// Token: 0x04000968 RID: 2408
	internal Vector3 onInvestigateTriggeredPosition;

	// Token: 0x04000969 RID: 2409
	private LevelPoint InvestigateLevelPoint;
}
