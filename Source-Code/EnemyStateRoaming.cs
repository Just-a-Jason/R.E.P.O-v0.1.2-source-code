using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000095 RID: 149
public class EnemyStateRoaming : MonoBehaviour
{
	// Token: 0x060005C5 RID: 1477 RVA: 0x00038D70 File Offset: 0x00036F70
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00038D8C File Offset: 0x00036F8C
	private void Update()
	{
		if (GameDirector.instance.currentState < GameDirector.gameState.Main)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Roaming)
		{
			if (this.Active)
			{
				this.RoamingLevelPoint = null;
				this.RoamingCooldown = 0f;
				this.RoamingChangeCurrent = 0;
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
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		this.PlayerNear();
		this.PlayerFar();
		this.PlayerTurn();
		this.PickPath();
		this.Stuck();
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00038E68 File Offset: 0x00037068
	private void PlayerNear()
	{
		if (SemiFunc.EnemyForceLeave(this.Enemy))
		{
			this.PlayerFarTimer = 0f;
			this.PlayerFarTime = Random.Range(this.PlayerFarTimeMin, this.PlayerFarTimeMax);
			this.PlayerFarMove = true;
			this.RoamingChangeCurrent = 0;
			return;
		}
		if (this.Enemy.PlayerDistance.PlayerDistanceClosest <= this.PlayerNearDistance)
		{
			this.PlayerNearTimer += Time.deltaTime;
		}
		else
		{
			this.PlayerNearTimer -= this.PlayerNearDecrease * Time.deltaTime;
			this.PlayerNearTimer = Mathf.Max(this.PlayerNearTimer, 0f);
		}
		if (this.PlayerNearTimer >= this.PlayerNearTimeMax)
		{
			this.PlayerFarTimer = 0f;
			this.PlayerFarTime = Random.Range(this.PlayerFarTimeMin, this.PlayerFarTimeMax);
			this.PlayerFarMove = true;
			this.RoamingChangeCurrent = 0;
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00038F4C File Offset: 0x0003714C
	private void PlayerFar()
	{
		if (this.PlayerFarMove)
		{
			this.PlayerFarTimer += Time.deltaTime;
			if (this.PlayerFarTimer >= this.PlayerFarTime)
			{
				this.PlayerFarMove = false;
				this.PlayerFarTimer = 0f;
			}
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00038F88 File Offset: 0x00037188
	private void PlayerTurn()
	{
		if (this.RoamingOnScreenCooldownTimer > 0f)
		{
			this.RoamingOnScreenCooldownTimer -= Time.deltaTime;
			return;
		}
		if (this.RoamingTurnWaitTimer > 0f)
		{
			this.RoamingCooldown = 1f;
			this.RoamingTurnWaitTimer -= Time.deltaTime;
			if (this.RoamingTurnWaitTimer <= 0f)
			{
				this.RoamingChangeCurrent = Random.Range(this.RoamingChangeMin, this.RoamingChangeMax + 1);
				this.RoamingLevelPoint = this.Enemy.GetLevelPointAhead(this.RoamingTurnPlayer.transform.position);
				this.RoamingCooldown = 0f;
				this.RoamingOnScreenCooldownTimer = this.RoamingOnScreenCooldown;
				return;
			}
		}
		else if (this.Enemy.OnScreen.OnScreenAny)
		{
			this.RoamingOnScreenTimer += Time.deltaTime;
			if (this.RoamingOnScreenTimer >= this.RoamingOnScreenTime)
			{
				if (GameManager.instance.gameMode == 1)
				{
					List<PlayerAvatar> list = new List<PlayerAvatar>();
					foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
					{
						if (!playerAvatar.isDisabled && this.Enemy.OnScreen.OnScreenPlayer[playerAvatar.photonView.ViewID])
						{
							list.Add(playerAvatar);
						}
					}
					if (list.Count <= 0)
					{
						this.RoamingOnScreenTimer = 0f;
						return;
					}
					this.RoamingTurnPlayer = list[Random.Range(0, list.Count)];
				}
				else
				{
					this.RoamingTurnPlayer = PlayerController.instance.playerAvatarScript;
				}
				this.RoamingOnScreenTimer = 0f;
				this.RoamingTurnWaitTimer = this.RoamingTurnWaitTime;
				this.Enemy.NavMeshAgent.ResetPath();
				this.RoamingCooldown = 1f;
				return;
			}
		}
		else
		{
			this.RoamingOnScreenTimer -= Time.deltaTime;
			this.RoamingOnScreenTimer = Mathf.Clamp01(this.RoamingOnScreenTimer);
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00039198 File Offset: 0x00037398
	private void PickPath()
	{
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		if (!this.Enemy.NavMeshAgent.HasPath())
		{
			if (this.RoamingCooldown <= 0f || !this.RoamingLevelPoint)
			{
				LevelPoint levelPoint = this.RoamingLevelPoint;
				if (this.RoamingChangeCurrent <= 0 || !this.RoamingLevelPoint)
				{
					if (this.PlayerFarMove)
					{
						levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
					}
					else
					{
						levelPoint = LevelGenerator.Instance.LevelPathPoints[Random.Range(0, LevelGenerator.Instance.LevelPathPoints.Count)];
					}
					this.RoamingChangeCurrent = Random.Range(this.RoamingChangeMin, this.RoamingChangeMax + 1);
				}
				else
				{
					this.RoamingChangeCurrent--;
				}
				if (levelPoint)
				{
					Vector3 vector = levelPoint.transform.position;
					vector += Random.insideUnitSphere * Random.Range(this.RoamingPathRadiusMin, this.RoamingPathRadiusMax);
					if (this.Enemy.NavMeshAgent.CalculatePath(vector).status == NavMeshPathStatus.PathComplete)
					{
						this.RoamingCooldown = Random.Range(this.RoamingCooldownMin, this.RoamingCooldownMax);
						this.RoamingLevelPoint = levelPoint;
						this.RoamingTargetPosition = vector;
						this.Enemy.NavMeshAgent.SetDestination(this.RoamingTargetPosition);
						return;
					}
				}
			}
			else
			{
				this.RoamingCooldown -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0003930C File Offset: 0x0003750C
	private void Stuck()
	{
		if (this.Enemy.NavMeshAgent.HasPath())
		{
			this.Enemy.AttackStuckPhysObject.Check();
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
			if (this.PhysObjectHitCount >= this.PhysObjectHitMax)
			{
				this.PhysObjectHitImpulse = true;
				this.PhysObjectHitCount = 0;
				this.Enemy.NavMeshAgent.ResetPath();
				this.RoamingChangeCurrent = 1;
			}
		}
	}

	// Token: 0x04000976 RID: 2422
	private Enemy Enemy;

	// Token: 0x04000977 RID: 2423
	private PlayerController Player;

	// Token: 0x04000978 RID: 2424
	private bool Active;

	// Token: 0x04000979 RID: 2425
	[Header("Movement")]
	public float Speed;

	// Token: 0x0400097A RID: 2426
	public float Acceleration;

	// Token: 0x0400097B RID: 2427
	[Header("Roaming")]
	public float RoamingCooldownMin;

	// Token: 0x0400097C RID: 2428
	public float RoamingCooldownMax;

	// Token: 0x0400097D RID: 2429
	internal LevelPoint RoamingLevelPoint;

	// Token: 0x0400097E RID: 2430
	private Vector3 RoamingTargetPosition;

	// Token: 0x0400097F RID: 2431
	internal float RoamingCooldown;

	// Token: 0x04000980 RID: 2432
	[Space]
	public float RoamingPathRadiusMin;

	// Token: 0x04000981 RID: 2433
	public float RoamingPathRadiusMax;

	// Token: 0x04000982 RID: 2434
	[Space]
	public int RoamingChangeMin;

	// Token: 0x04000983 RID: 2435
	public int RoamingChangeMax;

	// Token: 0x04000984 RID: 2436
	internal int RoamingChangeCurrent;

	// Token: 0x04000985 RID: 2437
	private Vector3 RoamingStuckPosition;

	// Token: 0x04000986 RID: 2438
	[Space]
	public float RoamingTeleportChance;

	// Token: 0x04000987 RID: 2439
	[Space]
	public float RoamingOnScreenTime;

	// Token: 0x04000988 RID: 2440
	private float RoamingOnScreenTimer;

	// Token: 0x04000989 RID: 2441
	private float RoamingOnScreenCooldownTimer;

	// Token: 0x0400098A RID: 2442
	public float RoamingOnScreenCooldown;

	// Token: 0x0400098B RID: 2443
	private float RoamingTurnWaitTimer;

	// Token: 0x0400098C RID: 2444
	public float RoamingTurnWaitTime;

	// Token: 0x0400098D RID: 2445
	private PlayerAvatar RoamingTurnPlayer;

	// Token: 0x0400098E RID: 2446
	[Header("Player Near")]
	public float PlayerNearTimeMax;

	// Token: 0x0400098F RID: 2447
	public float PlayerNearDistance;

	// Token: 0x04000990 RID: 2448
	public float PlayerNearDecrease;

	// Token: 0x04000991 RID: 2449
	private float PlayerNearTimer;

	// Token: 0x04000992 RID: 2450
	[Header("Player Far")]
	public float PlayerFarTimeMin;

	// Token: 0x04000993 RID: 2451
	public float PlayerFarTimeMax;

	// Token: 0x04000994 RID: 2452
	private float PlayerFarTime;

	// Token: 0x04000995 RID: 2453
	public float PlayerFarDistance;

	// Token: 0x04000996 RID: 2454
	private float PlayerFarTimer;

	// Token: 0x04000997 RID: 2455
	private bool PlayerFarMove;

	// Token: 0x04000998 RID: 2456
	[Header("Phys Object")]
	public int PhysObjectHitMax = 3;

	// Token: 0x04000999 RID: 2457
	private bool PhysObjectHitImpulse;

	// Token: 0x0400099A RID: 2458
	private int PhysObjectHitCount;
}
