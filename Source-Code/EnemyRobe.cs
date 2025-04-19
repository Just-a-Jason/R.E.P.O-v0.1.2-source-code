using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200006D RID: 109
public class EnemyRobe : MonoBehaviour
{
	// Token: 0x060003A1 RID: 929 RVA: 0x00023E71 File Offset: 0x00022071
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
		this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00023EA4 File Offset: 0x000220A4
	private void Update()
	{
		this.EndPieceLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.idleBreakTimer >= 0f)
		{
			this.idleBreakTimer -= Time.deltaTime;
			if (this.idleBreakTimer <= 0f && this.CanIdleBreak())
			{
				this.IdleBreak();
				this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
			}
		}
		this.RotationLogic();
		this.RigidbodyRotationSpeed();
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRobe.State.Stun);
		}
		else if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.UpdateState(EnemyRobe.State.Despawn);
		}
		switch (this.currentState)
		{
		case EnemyRobe.State.Spawn:
			this.StateSpawn();
			break;
		case EnemyRobe.State.Idle:
			this.StateIdle();
			break;
		case EnemyRobe.State.Roam:
			this.StateRoam();
			break;
		case EnemyRobe.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemyRobe.State.TargetPlayer:
			this.MoveTowardPlayer();
			this.StateTargetPlayer();
			break;
		case EnemyRobe.State.LookUnderStart:
			this.StateLookUnderStart();
			break;
		case EnemyRobe.State.LookUnder:
			this.StateLookUnder();
			break;
		case EnemyRobe.State.LookUnderAttack:
			this.StateLookUnderAttack();
			break;
		case EnemyRobe.State.LookUnderStop:
			this.StateLookUnderStop();
			break;
		case EnemyRobe.State.SeekPlayer:
			this.StateSeekPlayer();
			break;
		case EnemyRobe.State.Attack:
			this.StateAttack();
			break;
		case EnemyRobe.State.StuckAttack:
			this.StateStuckAttack();
			break;
		case EnemyRobe.State.Stun:
			this.StateStun();
			break;
		case EnemyRobe.State.Leave:
			this.StateLeave();
			break;
		case EnemyRobe.State.Despawn:
			this.StateDespawn();
			break;
		}
		if (this.currentState != EnemyRobe.State.TargetPlayer)
		{
			this.overrideAgentLerp = 0f;
		}
		if (this.currentState != EnemyRobe.State.TargetPlayer && this.isOnScreen)
		{
			this.isOnScreen = false;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("UpdateOnScreenRPC", RpcTarget.Others, new object[]
				{
					this.isOnScreen
				});
			}
		}
		if (this.isOnScreen && this.targetPlayer && this.targetPlayer.isLocal)
		{
			SemiFunc.DoNotLookEffect(base.gameObject, true, true, true, true, true, true);
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0002409C File Offset: 0x0002229C
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000240EC File Offset: 0x000222EC
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
			this.UpdateState(EnemyRobe.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00024198 File Offset: 0x00022398
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 5f;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(base.transform.position, 5f, 15f);
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
				this.agentDestination = navMeshHit.position;
			}
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 1f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00024330 File Offset: 0x00022530
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
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f)
			{
				this.AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				this.UpdateState(EnemyRobe.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Leave);
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000243FC File Offset: 0x000225FC
	private void StateTargetPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 30f);
		if (Vector3.Distance(this.enemy.CenterTransform.position, this.targetPlayer.transform.position) < 2f)
		{
			this.UpdateState(EnemyRobe.State.Attack);
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
			return;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.enemy.Vision.DisableVision(2f);
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
		if (this.stateTimer > 0.5f && this.targetPlayer.isCrawling && !this.targetPlayer.isTumbling && Vector3.Distance(this.enemy.NavMeshAgent.GetPoint(), this.targetPlayer.transform.position) > 0.5f && Vector3.Distance(this.targetPlayer.transform.position, this.targetPlayer.LastNavmeshPosition) < 3f)
		{
			this.UpdateState(EnemyRobe.State.LookUnderStart);
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00024554 File Offset: 0x00022754
	private void StateLookUnderStart()
	{
		if (this.stateImpulse)
		{
			this.lookUnderPosition = this.targetPlayer.transform.position;
			this.lookUnderPositionNavmesh = this.targetPlayer.LastNavmeshPosition;
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.OverrideAgent(3f, 10f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f, -1f);
		this.enemy.NavMeshAgent.SetDestination(this.lookUnderPositionNavmesh);
		if (Vector3.Distance(base.transform.position, this.lookUnderPositionNavmesh) < 1f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyRobe.State.LookUnder);
				return;
			}
		}
		else if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0002465C File Offset: 0x0002285C
	private void StateLookUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Vision.StandOverride(0.25f);
		Vector3 b = new Vector3(this.enemy.Rigidbody.transform.position.x, 0f, this.enemy.Rigidbody.transform.position.z);
		if (Vector3.Dot((new Vector3(this.targetPlayer.transform.position.x, 0f, this.targetPlayer.transform.position.z) - b).normalized, this.enemy.Rigidbody.transform.forward) > 0.75f && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 2.5f)
		{
			this.UpdateState(EnemyRobe.State.LookUnderAttack);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.LookUnderStop);
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0002479C File Offset: 0x0002299C
	private void StateLookUnderAttack()
	{
		if (this.stateImpulse)
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("LookUnderAttackImpulseRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.LookUnderAttackImpulseRPC();
			}
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemyRobe.State.LookUnderStop);
				return;
			}
			this.UpdateState(EnemyRobe.State.LookUnder);
		}
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00024824 File Offset: 0x00022A24
	private void StateLookUnderStop()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003AC RID: 940 RVA: 0x00024874 File Offset: 0x00022A74
	private void StateSeekPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 20f;
			this.stateImpulse = false;
			LevelPoint levelPointAhead = this.enemy.GetLevelPointAhead(this.targetPosition);
			if (levelPointAhead)
			{
				this.targetPosition = levelPointAhead.transform.position;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(3f, 3f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f, -1f);
		if (Vector3.Distance(base.transform.position, this.targetPosition) < 2f)
		{
			LevelPoint levelPointAhead2 = this.enemy.GetLevelPointAhead(this.targetPosition);
			if (levelPointAhead2)
			{
				this.targetPosition = levelPointAhead2.transform.position;
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer >= 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyRobe.State.Roam);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x000249D4 File Offset: 0x00022BD4
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.attackImpulse = true;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("AttackImpulseRPC", RpcTarget.Others, Array.Empty<object>());
			}
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 2f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.SeekPlayer);
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00024A94 File Offset: 0x00022C94
	private void StateStuckAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 1.5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Attack);
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x00024B2C File Offset: 0x00022D2C
	private void StateStun()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00024B94 File Offset: 0x00022D94
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 30f, 50f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			NavMeshHit navMeshHit;
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.agentDestination = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00024CE0 File Offset: 0x00022EE0
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00024D31 File Offset: 0x00022F31
	private void IdleBreak()
	{
		if (!GameManager.Multiplayer())
		{
			this.IdleBreakRPC();
			return;
		}
		this.photonView.RPC("IdleBreakRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00024D58 File Offset: 0x00022F58
	internal void UpdateState(EnemyRobe.State _state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
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

	// Token: 0x060003B4 RID: 948 RVA: 0x00024DC9 File Offset: 0x00022FC9
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyRobe.State.Spawn);
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00024DE8 File Offset: 0x00022FE8
	public void OnHurt()
	{
		this.robeAnim.sfxHurt.Play(this.robeAnim.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyRobe.State.Leave)
		{
			this.UpdateState(EnemyRobe.State.Idle);
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00024E42 File Offset: 0x00023042
	public void OnDeath()
	{
		this.deathImpulse = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00024E68 File Offset: 0x00023068
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Roam || this.currentState == EnemyRobe.State.Investigate || this.currentState == EnemyRobe.State.SeekPlayer)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			this.UpdateState(EnemyRobe.State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.TargetPlayer)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.stateTimer = Mathf.Max(this.stateTimer, 1f);
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnderStart)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer && !this.targetPlayer.isCrawling)
			{
				this.UpdateState(EnemyRobe.State.TargetPlayer);
				return;
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnder && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			if (this.targetPlayer.isCrawling)
			{
				this.lookUnderPosition = this.targetPlayer.transform.position;
				return;
			}
			this.UpdateState(EnemyRobe.State.LookUnderStop);
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00024FD0 File Offset: 0x000231D0
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Roam || this.currentState == EnemyRobe.State.Investigate)
			{
				this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
				this.UpdateState(EnemyRobe.State.Investigate);
				return;
			}
			if (this.currentState == EnemyRobe.State.SeekPlayer)
			{
				this.targetPosition = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			}
		}
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00025040 File Offset: 0x00023240
	public void OnGrabbed()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.grabAggroTimer > 0f)
		{
			return;
		}
		if (this.currentState == EnemyRobe.State.Leave)
		{
			this.grabAggroTimer = 60f;
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			this.UpdateState(EnemyRobe.State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x000250CB File Offset: 0x000232CB
	private bool CanIdleBreak()
	{
		return this.currentState == EnemyRobe.State.Idle || this.currentState == EnemyRobe.State.Investigate || this.currentState == EnemyRobe.State.Roam;
	}

	// Token: 0x060003BB RID: 955 RVA: 0x000250EC File Offset: 0x000232EC
	private void MoveTowardPlayer()
	{
		bool flag = false;
		if (this.enemy.OnScreen.GetOnScreen(this.targetPlayer))
		{
			flag = true;
			this.overrideAgentLerp += Time.deltaTime / 4f;
		}
		else
		{
			this.overrideAgentLerp -= Time.deltaTime / 0.01f;
		}
		if (flag != this.isOnScreen)
		{
			this.isOnScreen = flag;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("UpdateOnScreenRPC", RpcTarget.Others, new object[]
				{
					this.isOnScreen
				});
			}
		}
		this.overrideAgentLerp = Mathf.Clamp(this.overrideAgentLerp, 0f, 1f);
		float b = 25f;
		float b2 = 25f;
		float speed = Mathf.Lerp(this.enemy.NavMeshAgent.DefaultSpeed, b, this.overrideAgentLerp);
		float speed2 = Mathf.Lerp(this.enemy.Rigidbody.positionSpeedChase, b2, this.overrideAgentLerp);
		this.enemy.NavMeshAgent.OverrideAgent(speed, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(1f, speed2, -1f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00025254 File Offset: 0x00023454
	private void RotationLogic()
	{
		if (this.currentState == EnemyRobe.State.StuckAttack)
		{
			if (Vector3.Distance(this.stuckAttackTarget, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.stuckAttackTarget - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemyRobe.State.LookUnderStart || this.currentState == EnemyRobe.State.LookUnder || this.currentState == EnemyRobe.State.LookUnderAttack)
		{
			if (Vector3.Distance(this.lookUnderPosition, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.lookUnderPosition - base.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemyRobe.State.TargetPlayer || this.currentState == EnemyRobe.State.Attack)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - base.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
		{
			this.rotationTarget = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
			this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x000254B4 File Offset: 0x000236B4
	private void EndPieceLogic()
	{
		this.endPieceSource.rotation = SemiFunc.SpringQuaternionGet(this.endPieceSpring, this.endPieceTarget.rotation, -1f);
		this.endPieceTarget.localEulerAngles = new Vector3(-this.enemy.Rigidbody.physGrabObject.rbVelocity.y * 30f, 0f, 0f);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00025524 File Offset: 0x00023724
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.stuckAttackTarget = Vector3.zero;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		}
		if (this.stuckAttackTarget != Vector3.zero)
		{
			this.UpdateState(EnemyRobe.State.StuckAttack);
			return;
		}
		this.UpdateState(EnemyRobe.State.Idle);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00025588 File Offset: 0x00023788
	private void RigidbodyRotationSpeed()
	{
		if (this.currentState == EnemyRobe.State.Roam)
		{
			this.enemy.Rigidbody.rotationSpeedIdle = 1f;
			this.enemy.Rigidbody.rotationSpeedChase = 1f;
			return;
		}
		this.enemy.Rigidbody.rotationSpeedIdle = 2f;
		this.enemy.Rigidbody.rotationSpeedChase = 2f;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000255F3 File Offset: 0x000237F3
	[PunRPC]
	private void UpdateStateRPC(EnemyRobe.State _state)
	{
		this.currentState = _state;
		this.stateImpulse = true;
		if (this.currentState == EnemyRobe.State.Spawn)
		{
			this.robeAnim.SetSpawn();
		}
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00025618 File Offset: 0x00023818
	[PunRPC]
	private void TargetPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				this.targetPlayer = playerAvatar;
			}
		}
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00025680 File Offset: 0x00023880
	[PunRPC]
	private void UpdateOnScreenRPC(bool _onScreen)
	{
		this.isOnScreen = _onScreen;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00025689 File Offset: 0x00023889
	[PunRPC]
	private void AttackImpulseRPC()
	{
		this.attackImpulse = true;
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00025692 File Offset: 0x00023892
	[PunRPC]
	private void LookUnderAttackImpulseRPC()
	{
		this.lookUnderAttackImpulse = true;
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0002569B File Offset: 0x0002389B
	[PunRPC]
	private void IdleBreakRPC()
	{
		this.idleBreakTrigger = true;
	}

	// Token: 0x04000647 RID: 1607
	[Header("References")]
	public EnemyRobeAnim robeAnim;

	// Token: 0x04000648 RID: 1608
	internal Enemy enemy;

	// Token: 0x04000649 RID: 1609
	public EnemyRobe.State currentState;

	// Token: 0x0400064A RID: 1610
	private bool stateImpulse;

	// Token: 0x0400064B RID: 1611
	private float stateTimer;

	// Token: 0x0400064C RID: 1612
	internal PlayerAvatar targetPlayer;

	// Token: 0x0400064D RID: 1613
	private PhotonView photonView;

	// Token: 0x0400064E RID: 1614
	private float roamWaitTimer;

	// Token: 0x0400064F RID: 1615
	private Vector3 agentDestination;

	// Token: 0x04000650 RID: 1616
	private float overrideAgentLerp;

	// Token: 0x04000651 RID: 1617
	private Vector3 targetPosition;

	// Token: 0x04000652 RID: 1618
	public Transform eyeLocation;

	// Token: 0x04000653 RID: 1619
	internal bool isOnScreen;

	// Token: 0x04000654 RID: 1620
	internal bool attackImpulse;

	// Token: 0x04000655 RID: 1621
	internal bool deathImpulse;

	// Token: 0x04000656 RID: 1622
	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	// Token: 0x04000657 RID: 1623
	public float idleBreakTimeMax = 90f;

	// Token: 0x04000658 RID: 1624
	private float idleBreakTimer;

	// Token: 0x04000659 RID: 1625
	internal bool idleBreakTrigger;

	// Token: 0x0400065A RID: 1626
	[Space]
	public SpringQuaternion rotationSpring;

	// Token: 0x0400065B RID: 1627
	private Quaternion rotationTarget;

	// Token: 0x0400065C RID: 1628
	[Space]
	public SpringQuaternion endPieceSpring;

	// Token: 0x0400065D RID: 1629
	public Transform endPieceSource;

	// Token: 0x0400065E RID: 1630
	public Transform endPieceTarget;

	// Token: 0x0400065F RID: 1631
	private float grabAggroTimer;

	// Token: 0x04000660 RID: 1632
	private Vector3 lookUnderPositionNavmesh;

	// Token: 0x04000661 RID: 1633
	private Vector3 lookUnderPosition;

	// Token: 0x04000662 RID: 1634
	internal bool lookUnderAttackImpulse;

	// Token: 0x04000663 RID: 1635
	private Vector3 stuckAttackTarget;

	// Token: 0x020002DA RID: 730
	public enum State
	{
		// Token: 0x04002468 RID: 9320
		Spawn,
		// Token: 0x04002469 RID: 9321
		Idle,
		// Token: 0x0400246A RID: 9322
		Roam,
		// Token: 0x0400246B RID: 9323
		Investigate,
		// Token: 0x0400246C RID: 9324
		TargetPlayer,
		// Token: 0x0400246D RID: 9325
		LookUnderStart,
		// Token: 0x0400246E RID: 9326
		LookUnder,
		// Token: 0x0400246F RID: 9327
		LookUnderAttack,
		// Token: 0x04002470 RID: 9328
		LookUnderStop,
		// Token: 0x04002471 RID: 9329
		SeekPlayer,
		// Token: 0x04002472 RID: 9330
		Attack,
		// Token: 0x04002473 RID: 9331
		StuckAttack,
		// Token: 0x04002474 RID: 9332
		Stun,
		// Token: 0x04002475 RID: 9333
		Leave,
		// Token: 0x04002476 RID: 9334
		Despawn
	}
}
