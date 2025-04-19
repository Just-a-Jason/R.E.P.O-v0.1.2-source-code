using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000047 RID: 71
public class EnemyDuck : MonoBehaviour
{
	// Token: 0x060001EB RID: 491 RVA: 0x000135C2 File Offset: 0x000117C2
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060001EC RID: 492 RVA: 0x000135DC File Offset: 0x000117DC
	private void Update()
	{
		this.bodyTransform.rotation = SemiFunc.SpringQuaternionGet(this.bodySpring, this.bodyTargetTransform.rotation, -1f);
		this.HeadLookAtLogic();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!LevelGenerator.Instance.Generated)
			{
				return;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyDuck.State.Stun);
			}
			else if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyDuck.State.Despawn);
			}
			if (!this.playerTarget)
			{
				if (this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder)
				{
					this.UpdateState(EnemyDuck.State.Idle);
				}
				else if (this.currentState == EnemyDuck.State.ChaseNavmesh || this.currentState == EnemyDuck.State.ChaseTowards || this.currentState == EnemyDuck.State.ChaseMoveBack || this.currentState == EnemyDuck.State.Transform)
				{
					this.UpdateState(EnemyDuck.State.DeTransform);
				}
			}
			this.RotationLogic();
			this.TimerLogic();
			this.GravityLogic();
			this.TargetPositionLogic();
			this.FollowOffsetLogic();
			this.FlyBackConditionLogic();
			switch (this.currentState)
			{
			case EnemyDuck.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyDuck.State.Idle:
				this.StateIdle();
				return;
			case EnemyDuck.State.Roam:
				this.StateRoam();
				return;
			case EnemyDuck.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyDuck.State.Notice:
				this.StateNotice();
				return;
			case EnemyDuck.State.GoToPlayer:
				this.StateGoToPlayer();
				return;
			case EnemyDuck.State.GoToPlayerOver:
				this.StateGoToPlayerOver();
				return;
			case EnemyDuck.State.GoToPlayerUnder:
				this.StateGoToPlayerUnder();
				return;
			case EnemyDuck.State.FlyBackToNavmesh:
				this.StateFlyBackToNavmesh();
				return;
			case EnemyDuck.State.FlyBackToNavmeshStop:
				this.StateFlyBackToNavmeshStop();
				return;
			case EnemyDuck.State.MoveBackToNavmesh:
				this.StateMoveBackToNavMesh();
				return;
			case EnemyDuck.State.AttackStart:
				this.StateAttackStart();
				return;
			case EnemyDuck.State.Transform:
				this.StateTransform();
				return;
			case EnemyDuck.State.ChaseNavmesh:
				this.StateChaseNavmesh();
				return;
			case EnemyDuck.State.ChaseTowards:
				this.StateChaseTowards();
				return;
			case EnemyDuck.State.ChaseMoveBack:
				this.StateChaseMoveBack();
				return;
			case EnemyDuck.State.DeTransform:
				this.StateDeTransform();
				return;
			case EnemyDuck.State.Leave:
				this.StateLeave();
				return;
			case EnemyDuck.State.Stun:
				this.StateStun();
				return;
			case EnemyDuck.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060001ED RID: 493 RVA: 0x000137D0 File Offset: 0x000119D0
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.Idle);
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00013854 File Offset: 0x00011A54
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(2f, 5f);
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.Roam);
		}
		this.LeaveCheck(true);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x000138F0 File Offset: 0x00011AF0
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(base.transform.position, 10f, 25f);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGet(base.transform.position, 0f, 999f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.enemy.NavMeshAgent.SetDestination(navMeshHit.position);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			SemiFunc.EnemyCartJump(this.enemy);
			this.MoveBackPosition();
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyDuck.State.Idle);
			}
		}
		this.LeaveCheck(true);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x00013A64 File Offset: 0x00011C64
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			SemiFunc.EnemyCartJump(this.enemy);
			this.MoveBackPosition();
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyDuck.State.Idle);
			}
		}
		this.LeaveCheck(true);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x00013B30 File Offset: 0x00011D30
	private void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.GoToPlayer);
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x00013BB4 File Offset: 0x00011DB4
	private void StateGoToPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.annoyingJumpPauseTimer = 1f;
		}
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		SemiFunc.EnemyCartJump(this.enemy);
		this.MoveBackPosition();
		this.enemy.Vision.StandOverride(0.25f);
		if (this.stateTimer <= 0f || !this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyDuck.State.Idle);
			return;
		}
		NavMeshHit navMeshHit;
		if (!this.enemy.NavMeshAgent.CanReach(this.targetPosition, 1f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f && !this.VisionBlocked() && !NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
		{
			if (this.playerTarget.isCrawling && Mathf.Abs(this.targetPosition.y - this.enemy.Rigidbody.transform.position.y) < 0.3f && !this.enemy.Jump.jumping)
			{
				this.UpdateState(EnemyDuck.State.GoToPlayerUnder);
				return;
			}
			if (this.targetPosition.y > this.enemy.Rigidbody.transform.position.y)
			{
				this.UpdateState(EnemyDuck.State.GoToPlayerOver);
				return;
			}
		}
		this.AnnoyingJump();
		this.LeaveCheck(true);
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x00013D70 File Offset: 0x00011F70
	private void StateGoToPlayerUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			this.annoyingJumpPauseTimer = 1f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.Vision.StandOverride(0.25f);
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
		}
		else if (this.VisionBlocked() || !this.playerTarget || this.playerTarget.isDisabled)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
			}
		}
		else
		{
			this.stateTimer = 2f;
		}
		if (this.LeaveCheck(false))
		{
			this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00013E9C File Offset: 0x0001209C
	private void StateGoToPlayerOver()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			this.annoyingJumpPauseTimer = 1f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.Vision.StandOverride(0.25f);
		if (this.playerTarget.PlayerVisionTarget.VisionTransform.position.y > this.enemy.Rigidbody.transform.position.y + 1.5f)
		{
			if (!this.enemy.Jump.jumping)
			{
				Vector3 normalized = (this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).normalized;
				this.enemy.Jump.StuckTrigger(normalized);
				base.transform.position = this.enemy.Rigidbody.transform.position;
				base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, 2f);
			}
		}
		else
		{
			this.AnnoyingJump();
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.targetPosition, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
		}
		else if (this.VisionBlocked() || !this.playerTarget || this.playerTarget.isDisabled)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 1f)
			{
				this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
			}
		}
		else
		{
			this.stateTimer = 2f;
		}
		if (this.LeaveCheck(false))
		{
			this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000140C8 File Offset: 0x000122C8
	private void StateMoveBackToNavMesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 30f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		if (!this.enemy.Jump.jumping)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.Vision.StandOverride(0.25f);
		if ((Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f) && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (this.moveBackPosition - this.enemy.Rigidbody.transform.position).normalized;
			this.enemy.Jump.StuckTrigger(normalized);
			base.transform.position = this.enemy.Rigidbody.transform.position;
			base.transform.position += normalized * 2f;
		}
		this.stateTimer -= Time.deltaTime;
		NavMeshHit navMeshHit;
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.moveBackPosition) <= 0f || NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyDuck.State.GoToPlayer);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
			this.UpdateState(EnemyDuck.State.Despawn);
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x000142D0 File Offset: 0x000124D0
	private void StateFlyBackToNavmesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 30f;
			this.stateTicker = 0f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition + Vector3.up * 0.5f, 0.75f * Time.deltaTime);
		this.enemy.Rigidbody.OverrideFollowPosition(0.1f, 1f, -1f);
		this.enemy.Rigidbody.OverrideFollowRotation(0.1f, 0.25f);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, base.transform.position) > 2f)
		{
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		if (this.stateTicker <= 0f)
		{
			this.stateTicker = 0.25f;
			RaycastHit raycastHit;
			NavMeshHit navMeshHit;
			if (Physics.Raycast(this.enemy.Rigidbody.transform.position, Vector3.down, out raycastHit, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 0.5f, -1))
			{
				this.moveBackPosition = navMeshHit.position;
				this.UpdateState(EnemyDuck.State.FlyBackToNavmeshStop);
				return;
			}
		}
		else
		{
			this.stateTicker -= Time.deltaTime;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.Despawn);
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x00014494 File Offset: 0x00012694
	private void StateFlyBackToNavmeshStop()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.Idle);
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x00014518 File Offset: 0x00012718
	private void StateAttackStart()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 0.5f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.Rigidbody.GrabRelease();
			this.UpdateState(EnemyDuck.State.Transform);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00014578 File Offset: 0x00012778
	private void StateTransform()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1.4f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		Vector3 target = new Vector3(base.transform.position.x, this.playerTarget.PlayerVisionTarget.VisionTransform.position.y, base.transform.position.z);
		base.transform.position = Vector3.MoveTowards(base.transform.position, target, Time.deltaTime);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.ChaseNavmesh);
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00014640 File Offset: 0x00012840
	private void StateChaseNavmesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.OverrideAgent(10f, 10f, 0.1f);
		this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
		if (!this.VisionBlocked())
		{
			this.UpdateState(EnemyDuck.State.ChaseTowards);
			return;
		}
		this.MoveBackPosition();
		this.ChaseStop();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x000146B8 File Offset: 0x000128B8
	private void StateChaseTowards()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		if (this.VisionBlocked())
		{
			this.UpdateState(EnemyDuck.State.ChaseMoveBack);
			return;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.playerTarget.localCameraPosition + Vector3.down * 0.31f, 5f * Time.deltaTime);
		this.ChaseStop();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00014748 File Offset: 0x00012948
	private void StateChaseMoveBack()
	{
		if (this.stateImpulse)
		{
			RaycastHit raycastHit;
			NavMeshHit navMeshHit;
			if (Physics.Raycast(this.enemy.Rigidbody.transform.position, Vector3.down, out raycastHit, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 0.5f, -1))
			{
				this.moveBackPosition = navMeshHit.position;
			}
			this.stateImpulse = false;
			this.stateTimer = 10f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, 5f * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.stateTimer -= Time.deltaTime;
		NavMeshHit navMeshHit2;
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.moveBackPosition) <= 1f || NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit2, 0.5f, -1))
		{
			this.UpdateState(EnemyDuck.State.ChaseNavmesh);
			return;
		}
		this.ChaseStop();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x000148E0 File Offset: 0x00012AE0
	private void StateDeTransform()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyDuck.State.MoveBackToNavmesh);
		}
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00014964 File Offset: 0x00012B64
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			PlayerAvatar exists = null;
			float num = 999f;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				float num2 = Vector3.Distance(base.transform.position, playerAvatar.transform.position);
				if (num2 < 10f && num2 < num)
				{
					num = num2;
					exists = playerAvatar;
				}
			}
			if (exists)
			{
				if (this.enemy.Vision.onVisionTriggeredPlayer)
				{
					this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
					}
					this.UpdateState(EnemyDuck.State.AttackStart);
					return;
				}
			}
			else
			{
				this.UpdateState(EnemyDuck.State.Idle);
			}
		}
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00014A74 File Offset: 0x00012C74
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 30f, 50f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 1f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.agentDestination = navMeshHit.position;
				flag = true;
			}
			if (flag)
			{
				this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
				this.enemy.Rigidbody.notMovingTimer = 0f;
				this.stateImpulse = false;
				return;
			}
		}
		else
		{
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			SemiFunc.EnemyCartJump(this.enemy);
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyDuck.State.Idle);
			}
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00014BEC File Offset: 0x00012DEC
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00014C40 File Offset: 0x00012E40
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyDuck.State.Idle || this.currentState == EnemyDuck.State.Roam || this.currentState == EnemyDuck.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyDuck.State.Investigate);
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x00014C8C File Offset: 0x00012E8C
	public void OnVision()
	{
		if ((this.currentState == EnemyDuck.State.Idle || this.currentState == EnemyDuck.State.Roam || this.currentState == EnemyDuck.State.Investigate) && !this.enemy.Jump.jumping)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
					{
						this.playerTarget.photonView.ViewID
					});
				}
				this.UpdateState(EnemyDuck.State.Notice);
				return;
			}
		}
		else if (this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00014D48 File Offset: 0x00012F48
	public void OnHurt()
	{
		this.anim.soundHurtPauseTimer = 0.5f;
		this.anim.hurtSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		if (!this.enemy.IsStunned() && this.playerTarget && (this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder))
		{
			this.UpdateState(EnemyDuck.State.AttackStart);
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00014DD8 File Offset: 0x00012FD8
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		this.featherParticles.transform.position = this.enemy.CenterTransform.position;
		this.featherParticles.Play();
		this.anim.deathSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00014EC4 File Offset: 0x000130C4
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState != EnemyDuck.State.AttackStart && this.currentState != EnemyDuck.State.Transform && this.currentState != EnemyDuck.State.ChaseNavmesh && this.currentState != EnemyDuck.State.ChaseTowards && this.currentState != EnemyDuck.State.ChaseMoveBack && this.currentState != EnemyDuck.State.DeTransform && this.currentState != EnemyDuck.State.Stun && this.currentState != EnemyDuck.State.Despawn)
		{
			this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					this.playerTarget.photonView.ViewID
				});
			}
			this.UpdateState(EnemyDuck.State.AttackStart);
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00014F88 File Offset: 0x00013188
	public void OnObjectHurt()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemy.Health.onObjectHurtPlayer && this.currentState != EnemyDuck.State.AttackStart && this.currentState != EnemyDuck.State.Transform && this.currentState != EnemyDuck.State.ChaseNavmesh && this.currentState != EnemyDuck.State.ChaseTowards && this.currentState != EnemyDuck.State.ChaseMoveBack && this.currentState != EnemyDuck.State.DeTransform && this.currentState != EnemyDuck.State.Stun && this.currentState != EnemyDuck.State.Despawn)
		{
			this.playerTarget = this.enemy.Health.onObjectHurtPlayer;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					this.playerTarget.photonView.ViewID
				});
			}
			this.UpdateState(EnemyDuck.State.AttackStart);
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00015068 File Offset: 0x00013268
	private void UpdateState(EnemyDuck.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				this.currentState
			});
			return;
		}
		this.UpdateStateRPC(this.currentState);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000150D1 File Offset: 0x000132D1
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyDuck.State.Spawn);
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000150F0 File Offset: 0x000132F0
	public void TargetPositionLogic()
	{
		if (this.currentState != EnemyDuck.State.GoToPlayer && this.currentState != EnemyDuck.State.GoToPlayerOver && this.currentState != EnemyDuck.State.GoToPlayerUnder)
		{
			return;
		}
		if (!this.playerTarget)
		{
			return;
		}
		Vector3 vector;
		if (this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerUnder || this.currentState == EnemyDuck.State.GoToPlayerOver)
		{
			vector = this.playerTarget.transform.position + this.playerTarget.transform.forward * 1.5f;
		}
		else
		{
			vector = this.playerTarget.transform.position + this.playerTarget.transform.forward * this.targetForwardOffset;
		}
		if (this.pitCheckTimer <= 0f)
		{
			this.pitCheckTimer = 0.1f;
			this.pitCheck = !Physics.Raycast(vector + Vector3.up, Vector3.down, 4f, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		else
		{
			this.pitCheckTimer -= Time.deltaTime;
		}
		if (this.pitCheck)
		{
			vector = this.playerTarget.transform.position;
		}
		this.targetPosition = Vector3.Lerp(this.targetPosition, vector, 20f * Time.deltaTime);
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00015240 File Offset: 0x00013440
	private void AnnoyingJump()
	{
		if (this.enemy.Jump.jumping || this.annoyingJumpPauseTimer > 0f)
		{
			return;
		}
		if (this.playerTarget.PlayerVisionTarget.VisionTransform.position.y > this.enemy.Rigidbody.transform.position.y && this.enemy.Rigidbody.timeSinceStun > 2f)
		{
			Vector3 vector = this.playerTarget.localCameraTransform.position + this.playerTarget.localCameraTransform.forward;
			vector = new Vector3(this.enemy.Rigidbody.transform.position.x, vector.y, this.enemy.Rigidbody.transform.position.z);
			float num = vector.y - this.enemy.CenterTransform.position.y;
			if (!this.enemy.OnScreen.GetOnScreen(this.playerTarget) && num > 1f && !this.playerTarget.isMoving)
			{
				this.enemy.Jump.StuckTrigger(this.targetPosition - this.enemy.Vision.VisionTransform.position);
			}
		}
	}

	// Token: 0x0600020B RID: 523 RVA: 0x000153A4 File Offset: 0x000135A4
	private void RotationLogic()
	{
		if (this.playerTarget && (this.currentState == EnemyDuck.State.Notice || this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder))
		{
			if ((!this.VisionBlocked() && !this.playerTarget.isMoving && this.enemy.Rigidbody.velocity.magnitude < 0.5f) || this.enemy.Jump.jumping)
			{
				this.rotationTarget = Quaternion.LookRotation(this.playerTarget.transform.position - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
			else if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.enemy.Rigidbody.velocity.normalized);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.playerTarget && (this.currentState == EnemyDuck.State.ChaseNavmesh || this.currentState == EnemyDuck.State.ChaseTowards || this.currentState == EnemyDuck.State.Transform))
		{
			this.rotationTarget = Quaternion.LookRotation(this.playerTarget.transform.position - this.enemy.Rigidbody.transform.position);
			this.rotationTarget.eulerAngles = new Vector3(this.rotationTarget.eulerAngles.x, this.rotationTarget.eulerAngles.y, this.rotationTarget.eulerAngles.z);
		}
		else if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
		{
			this.rotationTarget = Quaternion.LookRotation(this.enemy.Rigidbody.velocity.normalized);
			if (this.currentState != EnemyDuck.State.ChaseMoveBack)
			{
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00015634 File Offset: 0x00013834
	private void GravityLogic()
	{
		if (this.currentState == EnemyDuck.State.ChaseNavmesh || this.currentState == EnemyDuck.State.ChaseTowards || this.currentState == EnemyDuck.State.ChaseMoveBack || this.currentState == EnemyDuck.State.Transform || this.currentState == EnemyDuck.State.FlyBackToNavmesh)
		{
			this.enemy.Rigidbody.gravity = false;
			return;
		}
		this.enemy.Rigidbody.gravity = true;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00015698 File Offset: 0x00013898
	private void MoveBackPosition()
	{
		if (this.moveBackTimer <= 0f)
		{
			this.moveBackTimer = 0.1f;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(base.transform.position, out navMeshHit, 0.5f, -1) && Physics.Raycast(base.transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.moveBackPosition = navMeshHit.position;
			}
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00015714 File Offset: 0x00013914
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
		{
			this.visionTimer = 0.25f;
			Vector3 direction = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.CenterTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.CenterTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000157A0 File Offset: 0x000139A0
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
		this.moveBackTimer -= Time.deltaTime;
		this.annoyingJumpPauseTimer -= Time.deltaTime;
		if (this.currentState == EnemyDuck.State.ChaseNavmesh || this.currentState == EnemyDuck.State.ChaseTowards || this.currentState == EnemyDuck.State.ChaseMoveBack)
		{
			this.chaseTimer += Time.deltaTime;
		}
		else
		{
			this.chaseTimer = 0f;
		}
		if (this.currentState == EnemyDuck.State.Spawn)
		{
			this.targetedPlayerTime = 0f;
		}
		if (this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder)
		{
			this.targetedPlayerTime += Time.deltaTime;
			return;
		}
		this.targetedPlayerTime -= 5f * Time.deltaTime;
		this.targetedPlayerTime = Mathf.Max(0f, this.targetedPlayerTime);
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00015890 File Offset: 0x00013A90
	private void HeadLookAtLogic()
	{
		bool flag = false;
		if ((this.currentState == EnemyDuck.State.Notice || this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder) && this.playerTarget && !this.playerTarget.isDisabled)
		{
			flag = true;
		}
		if (flag)
		{
			Vector3 vector = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.headLookAtTarget.position;
			vector = SemiFunc.ClampDirection(vector, this.headLookAtTarget.forward, 60f);
			this.headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(this.headLookAtSpring, Quaternion.LookRotation(vector), -1f);
			return;
		}
		this.headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(this.headLookAtSpring, this.headLookAtTarget.rotation, -1f);
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00015967 File Offset: 0x00013B67
	private bool LeaveCheck(bool _setLeave)
	{
		if (SemiFunc.EnemyForceLeave(this.enemy) || this.targetedPlayerTime >= this.targetedPlayerTimeMax)
		{
			if (_setLeave)
			{
				this.UpdateState(EnemyDuck.State.Leave);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00015992 File Offset: 0x00013B92
	public void IdleBreakerSet()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("IdleBreakerSetRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.IdleBreakerSetRPC();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000159B8 File Offset: 0x00013BB8
	private void ChaseStop()
	{
		if (this.chaseTimer >= 10f || !this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyDuck.State.DeTransform);
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x000159EC File Offset: 0x00013BEC
	private void FollowOffsetLogic()
	{
		if (this.currentState == EnemyDuck.State.ChaseNavmesh || this.currentState == EnemyDuck.State.ChaseMoveBack || this.currentState == EnemyDuck.State.FlyBackToNavmesh)
		{
			this.followOffsetTransform.localPosition = Vector3.Lerp(this.followOffsetTransform.localPosition, Vector3.up * 0.75f, 5f * Time.deltaTime);
			return;
		}
		if (this.currentState == EnemyDuck.State.FlyBackToNavmeshStop)
		{
			this.followOffsetTransform.localPosition = Vector3.zero;
			return;
		}
		this.followOffsetTransform.localPosition = Vector3.Lerp(this.followOffsetTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00015A94 File Offset: 0x00013C94
	private void FlyBackConditionLogic()
	{
		if ((this.currentState == EnemyDuck.State.Idle || this.currentState == EnemyDuck.State.Roam || this.currentState == EnemyDuck.State.Investigate || this.currentState == EnemyDuck.State.Notice || this.currentState == EnemyDuck.State.GoToPlayer || this.currentState == EnemyDuck.State.GoToPlayerOver || this.currentState == EnemyDuck.State.GoToPlayerUnder || this.currentState == EnemyDuck.State.MoveBackToNavmesh) && this.enemy.Rigidbody.transform.position.y - this.moveBackPosition.y < -4f)
		{
			this.UpdateState(EnemyDuck.State.FlyBackToNavmesh);
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00015B1E File Offset: 0x00013D1E
	[PunRPC]
	private void UpdateStateRPC(EnemyDuck.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemyDuck.State.Spawn)
		{
			this.anim.OnSpawn();
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00015B3C File Offset: 0x00013D3C
	[PunRPC]
	private void UpdatePlayerTargetRPC(int _photonViewID)
	{
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _photonViewID)
			{
				this.playerTarget = playerAvatar;
				break;
			}
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00015BA0 File Offset: 0x00013DA0
	[PunRPC]
	private void IdleBreakerSetRPC()
	{
		this.idleBreakerTrigger = true;
	}

	// Token: 0x040003D0 RID: 976
	private PhotonView photonView;

	// Token: 0x040003D1 RID: 977
	public EnemyDuck.State currentState;

	// Token: 0x040003D2 RID: 978
	private bool stateImpulse;

	// Token: 0x040003D3 RID: 979
	public float stateTimer;

	// Token: 0x040003D4 RID: 980
	private float stateTicker;

	// Token: 0x040003D5 RID: 981
	private Vector3 targetPosition;

	// Token: 0x040003D6 RID: 982
	private float pitCheckTimer;

	// Token: 0x040003D7 RID: 983
	private bool pitCheck;

	// Token: 0x040003D8 RID: 984
	private Vector3 agentDestination;

	// Token: 0x040003D9 RID: 985
	private bool visionPrevious;

	// Token: 0x040003DA RID: 986
	private float visionTimer;

	// Token: 0x040003DB RID: 987
	private Vector3 moveBackPosition;

	// Token: 0x040003DC RID: 988
	private float moveBackTimer;

	// Token: 0x040003DD RID: 989
	private float targetForwardOffset = 1.5f;

	// Token: 0x040003DE RID: 990
	public Transform followOffsetTransform;

	// Token: 0x040003DF RID: 991
	[Space]
	public SpringQuaternion bodySpring;

	// Token: 0x040003E0 RID: 992
	public Transform bodyTransform;

	// Token: 0x040003E1 RID: 993
	public Transform bodyTargetTransform;

	// Token: 0x040003E2 RID: 994
	[Space]
	public EnemyDuckAnim anim;

	// Token: 0x040003E3 RID: 995
	public Enemy enemy;

	// Token: 0x040003E4 RID: 996
	public ParticleSystem featherParticles;

	// Token: 0x040003E5 RID: 997
	private PlayerAvatar playerTarget;

	// Token: 0x040003E6 RID: 998
	[Space]
	private Quaternion rotationTarget;

	// Token: 0x040003E7 RID: 999
	public SpringQuaternion rotationSpring;

	// Token: 0x040003E8 RID: 1000
	[Space]
	public SpringQuaternion headLookAtSpring;

	// Token: 0x040003E9 RID: 1001
	public Transform headLookAtTarget;

	// Token: 0x040003EA RID: 1002
	public Transform headLookAtSource;

	// Token: 0x040003EB RID: 1003
	private float targetedPlayerTime;

	// Token: 0x040003EC RID: 1004
	private float targetedPlayerTimeMax = 120f;

	// Token: 0x040003ED RID: 1005
	internal bool idleBreakerTrigger;

	// Token: 0x040003EE RID: 1006
	private float chaseTimer;

	// Token: 0x040003EF RID: 1007
	private float annoyingJumpPauseTimer;

	// Token: 0x020002CE RID: 718
	public enum State
	{
		// Token: 0x040023ED RID: 9197
		Spawn,
		// Token: 0x040023EE RID: 9198
		Idle,
		// Token: 0x040023EF RID: 9199
		Roam,
		// Token: 0x040023F0 RID: 9200
		Investigate,
		// Token: 0x040023F1 RID: 9201
		Notice,
		// Token: 0x040023F2 RID: 9202
		GoToPlayer,
		// Token: 0x040023F3 RID: 9203
		GoToPlayerOver,
		// Token: 0x040023F4 RID: 9204
		GoToPlayerUnder,
		// Token: 0x040023F5 RID: 9205
		FlyBackToNavmesh,
		// Token: 0x040023F6 RID: 9206
		FlyBackToNavmeshStop,
		// Token: 0x040023F7 RID: 9207
		MoveBackToNavmesh,
		// Token: 0x040023F8 RID: 9208
		AttackStart,
		// Token: 0x040023F9 RID: 9209
		Transform,
		// Token: 0x040023FA RID: 9210
		ChaseNavmesh,
		// Token: 0x040023FB RID: 9211
		ChaseTowards,
		// Token: 0x040023FC RID: 9212
		ChaseMoveBack,
		// Token: 0x040023FD RID: 9213
		DeTransform,
		// Token: 0x040023FE RID: 9214
		Leave,
		// Token: 0x040023FF RID: 9215
		Stun,
		// Token: 0x04002400 RID: 9216
		Despawn
	}
}
