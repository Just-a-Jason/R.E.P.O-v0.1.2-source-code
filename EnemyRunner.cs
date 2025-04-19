using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000070 RID: 112
public class EnemyRunner : MonoBehaviour
{
	// Token: 0x060003D7 RID: 983 RVA: 0x00025D38 File Offset: 0x00023F38
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00025D58 File Offset: 0x00023F58
	private void Update()
	{
		this.HeadSpringUpdate();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRunner.State.Despawn);
		}
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRunner.State.Stun);
		}
		switch (this.currentState)
		{
		case EnemyRunner.State.Spawn:
			this.StateSpawn();
			break;
		case EnemyRunner.State.Idle:
			this.StateIdle();
			break;
		case EnemyRunner.State.Roam:
			this.StateRoam();
			break;
		case EnemyRunner.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemyRunner.State.SeekPlayer:
			this.StateSeekPlayer();
			break;
		case EnemyRunner.State.Sneak:
			this.StateSneak();
			break;
		case EnemyRunner.State.Notice:
			this.StateNotice();
			break;
		case EnemyRunner.State.AttackPlayer:
			this.StateAttackPlayer();
			break;
		case EnemyRunner.State.AttackPlayerOver:
			this.StateAttackPlayerOver();
			break;
		case EnemyRunner.State.AttackPlayerBackToNavMesh:
			this.StateAttackPlayerBackToNavMesh();
			break;
		case EnemyRunner.State.StuckAttackNotice:
			this.StateStuckAttackNotice();
			break;
		case EnemyRunner.State.StuckAttack:
			this.StateStuckAttack();
			break;
		case EnemyRunner.State.LookUnderStart:
			this.StateLookUnderStart();
			break;
		case EnemyRunner.State.LookUnder:
			this.StateLookUnder();
			break;
		case EnemyRunner.State.LookUnderStop:
			this.StateLookUnderStop();
			break;
		case EnemyRunner.State.Stun:
			this.StateStun();
			break;
		case EnemyRunner.State.Leave:
			this.StateLeave();
			break;
		case EnemyRunner.State.Despawn:
			this.StateDespawn();
			break;
		}
		this.RotationLogic();
		this.TimerLogic();
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00025EA4 File Offset: 0x000240A4
	public void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00025EF4 File Offset: 0x000240F4
	public void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(2f, 6f);
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.StoreBackToNavMeshPosition();
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRunner.State.Leave);
		}
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00025F9C File Offset: 0x0002419C
	public void StateRoam()
	{
		if (this.stateImpulse)
		{
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
				this.agentDestination = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			this.StoreBackToNavMeshPosition();
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (this.enemy.Rigidbody.notMovingTimer > 3f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (!this.enemy.Jump.jumping && (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f))
			{
				this.UpdateState(EnemyRunner.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRunner.State.Leave);
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00026134 File Offset: 0x00024334
	public void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			this.StoreBackToNavMeshPosition();
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
				this.UpdateState(EnemyRunner.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyRunner.State.Leave);
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00026204 File Offset: 0x00024404
	public void StateSeekPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 20f;
			this.stateImpulse = false;
			this.targetPosition = base.transform.position;
			LevelPoint levelPointAhead = this.enemy.GetLevelPointAhead(this.targetPosition);
			if (levelPointAhead)
			{
				this.targetPosition = levelPointAhead.transform.position;
			}
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		this.StoreBackToNavMeshPosition();
		this.enemy.NavMeshAgent.OverrideAgent(1f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
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
			this.UpdateState(EnemyRunner.State.Roam);
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00026370 File Offset: 0x00024570
	public void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 0.9f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.AttackPlayer);
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x000263E8 File Offset: 0x000245E8
	public void StateSneak()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyRunner.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.enemy.NavMeshAgent.OverrideAgent(1f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.StoreBackToNavMeshPosition();
		this.stateTimer -= Time.deltaTime;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f || this.enemy.OnScreen.OnScreenAny)
		{
			this.UpdateState(EnemyRunner.State.Notice);
		}
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00026548 File Offset: 0x00024748
	public void StateAttackPlayer()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyRunner.State.SeekPlayer);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.agentSpeedCurrent = 0f;
			this.targetPosition = this.targetPlayer.transform.position;
			this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
			return;
		}
		this.StoreBackToNavMeshPosition();
		this.agentSpeedCurrent = Mathf.Lerp(this.agentSpeedCurrent, this.agentSpeed, Time.deltaTime * 2f);
		this.enemy.NavMeshAgent.OverrideAgent(this.agentSpeedCurrent, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (!this.enemy.NavMeshAgent.CanReach(this.targetPlayer.transform.position, 0.25f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			if (this.targetPlayer.transform.position.y > this.enemy.Rigidbody.transform.position.y - this.rbCheckHeightOffset)
			{
				this.enemy.Jump.StuckTrigger(this.targetPlayer.transform.position - this.enemy.Vision.VisionTransform.position);
			}
			NavMeshHit navMeshHit;
			if (!this.VisionBlocked() && !NavMesh.SamplePosition(this.targetPlayer.transform.position, out navMeshHit, 0.5f, -1) && this.targetPlayer.transform.position.y > this.feetTransform.position.y)
			{
				this.UpdateState(EnemyRunner.State.AttackPlayerOver);
				return;
			}
		}
		if (!this.enemy.Jump.jumping && this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.enemy.Jump.StuckTrigger(this.targetPlayer.transform.position - this.feetTransform.position);
		}
		if (this.stateTimer > 1.5f && this.targetPlayer.isCrawling && !this.targetPlayer.isTumbling && (double)Vector3.Distance(this.enemy.NavMeshAgent.GetPoint(), this.targetPlayer.transform.position) > 0.5 && Vector3.Distance(this.targetPlayer.transform.position, this.targetPlayer.LastNavmeshPosition) < 2f)
		{
			this.UpdateState(EnemyRunner.State.LookUnderStart);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.SeekPlayer);
			return;
		}
		if (this.targetPlayer.isDisabled)
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00026880 File Offset: 0x00024A80
	public void StateAttackPlayerOver()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyRunner.State.AttackPlayerBackToNavMesh);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPlayer.transform.position, this.agentSpeed * Time.deltaTime);
		if (!this.enemy.Jump.jumping && (this.targetPlayer.transform.position.y > this.enemy.Rigidbody.transform.position.y - this.rbCheckHeightOffset || this.enemy.Rigidbody.notMovingTimer > 2f))
		{
			this.enemy.Jump.StuckTrigger(this.targetPlayer.transform.position - this.feetTransform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPlayer.transform.position, this.agentSpeed);
			this.enemy.Rigidbody.OverrideFollowRotation(0.5f, 0.25f);
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.targetPlayer.transform.position, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyRunner.State.AttackPlayerBackToNavMesh);
			return;
		}
		if (this.VisionBlocked() || this.targetPlayer.isDisabled || this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f || this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemyRunner.State.AttackPlayerBackToNavMesh);
			}
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00026A70 File Offset: 0x00024C70
	public void StateAttackPlayerBackToNavMesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 30f;
		}
		this.stateTimer -= Time.deltaTime;
		if ((Vector3.Distance(base.transform.position, this.feetTransform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f) && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (this.feetTransform.position - this.backToNavMeshPosition).normalized;
			this.enemy.Jump.StuckTrigger(normalized);
			base.transform.position = this.feetTransform.position;
			base.transform.position += normalized * 2f;
			this.enemy.Rigidbody.OverrideFollowRotation(0.5f, 0.25f);
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		if (!this.enemy.Jump.jumping)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.backToNavMeshPosition, this.agentSpeed * Time.deltaTime);
		}
		NavMeshHit navMeshHit;
		if (Vector3.Distance(this.feetTransform.position, this.backToNavMeshPosition) <= 0.2f || NavMesh.SamplePosition(this.feetTransform.position, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyRunner.State.AttackPlayer);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
			this.UpdateState(EnemyRunner.State.Despawn);
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00026C38 File Offset: 0x00024E38
	public void StateStuckAttackNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.9f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.StuckAttack);
		}
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00026C88 File Offset: 0x00024E88
	public void StateStuckAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.stateTimer = 1.5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00026D18 File Offset: 0x00024F18
	public void StateLookUnderStart()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyRunner.State.SeekPlayer);
			return;
		}
		if (this.stateImpulse)
		{
			this.lookUnderPosition = this.targetPlayer.transform.position;
			this.lookUnderPositionNavmesh = this.targetPlayer.LastNavmeshPosition;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.OverrideAgent(3f, 10f, 0.2f);
		this.enemy.NavMeshAgent.SetDestination(this.lookUnderPositionNavmesh);
		if (Vector3.Distance(base.transform.position, this.lookUnderPositionNavmesh) < 0.5f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyRunner.State.LookUnder);
				return;
			}
		}
		else if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyRunner.State.SeekPlayer);
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00026E28 File Offset: 0x00025028
	public void StateLookUnder()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Vision.StandOverride(0.25f);
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.LookUnderStop);
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00026E8C File Offset: 0x0002508C
	public void StateLookUnderStop()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 0.9f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.SeekPlayer);
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00026EDC File Offset: 0x000250DC
	public void StateStun()
	{
		this.StoreBackToNavMeshPosition();
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00026F38 File Offset: 0x00025138
	public void StateLeave()
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
			this.stateImpulse = false;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		this.enemy.NavMeshAgent.OverrideAgent(1f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x000270B6 File Offset: 0x000252B6
	public void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x000270F2 File Offset: 0x000252F2
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyRunner.State.Spawn);
		}
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00027110 File Offset: 0x00025310
	public void OnHurt()
	{
		this.animator.sfxHurt.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyRunner.State.Leave)
		{
			this.UpdateState(EnemyRunner.State.Idle);
		}
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0002716C File Offset: 0x0002536C
	public void OnDeath()
	{
		this.hayParticlesBig.transform.position = this.enemy.CenterTransform.position;
		this.hayParticlesBig.Play();
		this.hayParticlesSmall.transform.position = this.enemy.CenterTransform.position;
		this.hayParticlesSmall.Play();
		this.bitsParticlesFar.transform.position = this.enemy.CenterTransform.position;
		this.bitsParticlesFar.Play();
		this.bitsParticlesShort.transform.position = this.enemy.CenterTransform.position;
		this.bitsParticlesShort.Play();
		this.animator.sfxDeath.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x000272D0 File Offset: 0x000254D0
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState == EnemyRunner.State.Idle || this.currentState == EnemyRunner.State.Roam || this.currentState == EnemyRunner.State.Investigate)
			{
				this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
				this.UpdateState(EnemyRunner.State.Investigate);
				return;
			}
			if (this.currentState == EnemyRunner.State.SeekPlayer)
			{
				this.targetPosition = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			}
		}
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x0002733C File Offset: 0x0002553C
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyRunner.State.Roam || this.currentState == EnemyRunner.State.Idle || this.currentState == EnemyRunner.State.Investigate || this.currentState == EnemyRunner.State.SeekPlayer)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (!this.enemy.OnScreen.OnScreenAny)
			{
				this.UpdateState(EnemyRunner.State.Sneak);
			}
			else
			{
				this.UpdateState(EnemyRunner.State.Notice);
			}
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemyRunner.State.AttackPlayer || this.currentState == EnemyRunner.State.AttackPlayerOver || this.currentState == EnemyRunner.State.Sneak)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.stateTimer = 2f;
				return;
			}
		}
		else if (this.currentState == EnemyRunner.State.LookUnderStart)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer && !this.targetPlayer.isCrawling)
			{
				this.UpdateState(EnemyRunner.State.AttackPlayer);
				return;
			}
		}
		else if (this.currentState == EnemyRunner.State.LookUnder && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			if (this.targetPlayer.isCrawling)
			{
				this.lookUnderPosition = this.targetPlayer.transform.position;
				return;
			}
			this.UpdateState(EnemyRunner.State.LookUnderStop);
		}
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000274C8 File Offset: 0x000256C8
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyRunner.State.Leave)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			string str = "OnGrabbed: ";
			PlayerAvatar playerAvatar = this.targetPlayer;
			SemiLogger.LogAxel(str + ((playerAvatar != null) ? playerAvatar.ToString() : null), null, null);
			this.UpdateState(EnemyRunner.State.Notice);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00027568 File Offset: 0x00025768
	private void UpdateState(EnemyRunner.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.enemy.Rigidbody.notMovingTimer = 0f;
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

	// Token: 0x060003F2 RID: 1010 RVA: 0x000275E8 File Offset: 0x000257E8
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.stuckAttackTarget = Vector3.zero;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		}
		if (this.stuckAttackTarget != Vector3.zero)
		{
			this.UpdateState(EnemyRunner.State.StuckAttackNotice);
			return;
		}
		this.UpdateState(EnemyRunner.State.Idle);
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x0002764A File Offset: 0x0002584A
	private void HeadSpringUpdate()
	{
		this.headSpringSource.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, this.headSpringTarget.rotation, -1f);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00027674 File Offset: 0x00025874
	private void RotationLogic()
	{
		if (this.currentState != EnemyRunner.State.LookUnderStop)
		{
			if (this.currentState == EnemyRunner.State.Notice || this.currentState == EnemyRunner.State.AttackPlayer || this.currentState == EnemyRunner.State.AttackPlayerOver)
			{
				if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
				{
					this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
					this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
				}
			}
			else if (this.currentState == EnemyRunner.State.StuckAttack)
			{
				if (Vector3.Distance(this.stuckAttackTarget, this.enemy.Rigidbody.transform.position) > 0.1f)
				{
					this.rotationTarget = Quaternion.LookRotation(this.stuckAttackTarget - this.enemy.Rigidbody.transform.position);
					this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
				}
			}
			else if (this.currentState == EnemyRunner.State.LookUnderStart || this.currentState == EnemyRunner.State.LookUnder)
			{
				if (Vector3.Distance(this.lookUnderPosition, base.transform.position) > 0.1f)
				{
					this.rotationTarget = Quaternion.LookRotation(this.lookUnderPosition - base.transform.position);
					this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
				}
			}
			else if (this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		if (this.currentState == EnemyRunner.State.Roam || this.currentState == EnemyRunner.State.Investigate)
		{
			this.rotationSpring.speed = 3f;
		}
		else
		{
			this.rotationSpring.speed = 10f;
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0002792C File Offset: 0x00025B2C
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f && this.targetPlayer)
		{
			this.visionTimer = 0.1f;
			Vector3 direction = this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Vision.VisionTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.Vision.VisionTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x000279D2 File Offset: 0x00025BD2
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
		this.sampleNavMeshTimer -= Time.deltaTime;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x000279F8 File Offset: 0x00025BF8
	private void StoreBackToNavMeshPosition()
	{
		if (this.sampleNavMeshTimer <= 0f)
		{
			this.sampleNavMeshTimer = 0.5f;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(base.transform.position, out navMeshHit, 0.5f, -1))
			{
				this.backToNavMeshPosition = navMeshHit.position;
			}
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00027A44 File Offset: 0x00025C44
	[PunRPC]
	private void UpdateStateRPC(EnemyRunner.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemyRunner.State.Spawn)
		{
			this.animator.OnSpawn();
		}
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00027A60 File Offset: 0x00025C60
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

	// Token: 0x0400067C RID: 1660
	public SpringQuaternion headSpring;

	// Token: 0x0400067D RID: 1661
	public Transform headSpringTarget;

	// Token: 0x0400067E RID: 1662
	public Transform headSpringSource;

	// Token: 0x0400067F RID: 1663
	public SpringQuaternion rotationSpring;

	// Token: 0x04000680 RID: 1664
	private Quaternion rotationTarget;

	// Token: 0x04000681 RID: 1665
	public EnemyRunner.State currentState;

	// Token: 0x04000682 RID: 1666
	private bool stateImpulse = true;

	// Token: 0x04000683 RID: 1667
	private float stateTimer;

	// Token: 0x04000684 RID: 1668
	internal PlayerAvatar targetPlayer;

	// Token: 0x04000685 RID: 1669
	public Enemy enemy;

	// Token: 0x04000686 RID: 1670
	public EnemyRunnerAnim animator;

	// Token: 0x04000687 RID: 1671
	public ParticleSystem hayParticlesBig;

	// Token: 0x04000688 RID: 1672
	public ParticleSystem hayParticlesSmall;

	// Token: 0x04000689 RID: 1673
	public ParticleSystem bitsParticlesFar;

	// Token: 0x0400068A RID: 1674
	public ParticleSystem bitsParticlesShort;

	// Token: 0x0400068B RID: 1675
	private PhotonView photonView;

	// Token: 0x0400068C RID: 1676
	public HurtCollider hurtCollider;

	// Token: 0x0400068D RID: 1677
	private float hurtColliderTimer;

	// Token: 0x0400068E RID: 1678
	private Vector3 agentDestination;

	// Token: 0x0400068F RID: 1679
	private Vector3 backToNavMeshPosition;

	// Token: 0x04000690 RID: 1680
	private Vector3 stuckAttackTarget;

	// Token: 0x04000691 RID: 1681
	private float agentSpeed = 3f;

	// Token: 0x04000692 RID: 1682
	private float agentSpeedCurrent;

	// Token: 0x04000693 RID: 1683
	private float rbCheckHeightOffset = 0.8f;

	// Token: 0x04000694 RID: 1684
	private Vector3 targetPosition;

	// Token: 0x04000695 RID: 1685
	private float visionTimer;

	// Token: 0x04000696 RID: 1686
	private bool visionPrevious;

	// Token: 0x04000697 RID: 1687
	public Transform feetTransform;

	// Token: 0x04000698 RID: 1688
	private float sampleNavMeshTimer;

	// Token: 0x04000699 RID: 1689
	private Vector3 lookUnderPosition;

	// Token: 0x0400069A RID: 1690
	private Vector3 lookUnderPositionNavmesh;

	// Token: 0x020002DB RID: 731
	public enum State
	{
		// Token: 0x04002478 RID: 9336
		Spawn,
		// Token: 0x04002479 RID: 9337
		Idle,
		// Token: 0x0400247A RID: 9338
		Roam,
		// Token: 0x0400247B RID: 9339
		Investigate,
		// Token: 0x0400247C RID: 9340
		SeekPlayer,
		// Token: 0x0400247D RID: 9341
		Sneak,
		// Token: 0x0400247E RID: 9342
		Notice,
		// Token: 0x0400247F RID: 9343
		AttackPlayer,
		// Token: 0x04002480 RID: 9344
		AttackPlayerOver,
		// Token: 0x04002481 RID: 9345
		AttackPlayerBackToNavMesh,
		// Token: 0x04002482 RID: 9346
		StuckAttackNotice,
		// Token: 0x04002483 RID: 9347
		StuckAttack,
		// Token: 0x04002484 RID: 9348
		LookUnderStart,
		// Token: 0x04002485 RID: 9349
		LookUnder,
		// Token: 0x04002486 RID: 9350
		LookUnderStop,
		// Token: 0x04002487 RID: 9351
		Stun,
		// Token: 0x04002488 RID: 9352
		Leave,
		// Token: 0x04002489 RID: 9353
		Despawn
	}
}
