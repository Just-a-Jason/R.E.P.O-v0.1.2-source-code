using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020000A6 RID: 166
public class EnemySlowWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x0600065B RID: 1627 RVA: 0x0003D49B File Offset: 0x0003B69B
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0003D4A9 File Offset: 0x0003B6A9
	private void Start()
	{
		this.visionDotStandingDefault = this.enemy.Vision.VisionDotStanding;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0003D4C4 File Offset: 0x0003B6C4
	private void Update()
	{
		this.HeadLookAt();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Despawn);
		}
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Stun);
		}
		switch (this.currentState)
		{
		case EnemySlowWalker.State.Spawn:
			this.StateSpawn();
			break;
		case EnemySlowWalker.State.Idle:
			this.StateIdle();
			break;
		case EnemySlowWalker.State.Roam:
			this.StateRoam();
			break;
		case EnemySlowWalker.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemySlowWalker.State.Notice:
			this.StateNotice();
			break;
		case EnemySlowWalker.State.GoToPlayer:
			this.StateGoToPlayer();
			break;
		case EnemySlowWalker.State.Sneak:
			this.StateSneak();
			break;
		case EnemySlowWalker.State.Attack:
			this.StateAttack();
			break;
		case EnemySlowWalker.State.StuckAttack:
			this.StateStuckAttack();
			break;
		case EnemySlowWalker.State.LookUnderStart:
			this.StateLookUnderStart();
			break;
		case EnemySlowWalker.State.LookUnderIntro:
			this.StateLookUnderIntro();
			break;
		case EnemySlowWalker.State.LookUnder:
			this.StateLookUnder();
			break;
		case EnemySlowWalker.State.LookUnderAttack:
			this.StateLookUnderAttack();
			break;
		case EnemySlowWalker.State.LookUnderStop:
			this.StateLookUnderStop();
			break;
		case EnemySlowWalker.State.Stun:
			this.StateStun();
			break;
		case EnemySlowWalker.State.Leave:
			this.StateLeave();
			break;
		case EnemySlowWalker.State.Despawn:
			this.StateDespawn();
			break;
		}
		this.RotationLogic();
		this.TimerLogic();
		this.VisionDotLogic();
		this.AttackOffsetLogic();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0003D60C File Offset: 0x0003B80C
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
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0003D65C File Offset: 0x0003B85C
	public void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0003D6F4 File Offset: 0x0003B8F4
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
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (this.enemy.Rigidbody.notMovingTimer > 3f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f)
			{
				this.AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemySlowWalker.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0003D878 File Offset: 0x0003BA78
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
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemySlowWalker.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Leave);
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0003D944 File Offset: 0x0003BB44
	public void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.GoToPlayer);
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x0003D9BC File Offset: 0x0003BBBC
	public void StateGoToPlayer()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
			this.stateTimer = 10f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 8f && this.stateTimer > 1.5f && this.enemy.Jump.timeSinceJumped > 1f)
		{
			this.UpdateState(EnemySlowWalker.State.Attack);
			return;
		}
		if (this.stateTimer > 9f && this.targetPlayer.isCrawling && !this.targetPlayer.isTumbling && Vector3.Distance(this.enemy.NavMeshAgent.GetPoint(), this.targetPlayer.transform.position) > 0.5f && Vector3.Distance(this.targetPlayer.transform.position, this.targetPlayer.LastNavmeshPosition) < 3f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderStart);
			return;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
		}
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x0003DB74 File Offset: 0x0003BD74
	public void StateSneak()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
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
		this.enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 5f)
		{
			this.UpdateState(EnemySlowWalker.State.GoToPlayer);
			return;
		}
		if (this.enemy.OnScreen.OnScreenAny)
		{
			this.UpdateState(EnemySlowWalker.State.Notice);
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0003DCAC File Offset: 0x0003BEAC
	public void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 5f;
			this.attackCount++;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0003DD34 File Offset: 0x0003BF34
	public void StateStuckAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Stop(0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0003DDC4 File Offset: 0x0003BFC4
	public void StateLookUnderStart()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.lookUnderPosition = this.targetPlayer.transform.position;
			this.lookUnderLookAtPosition = this.lookUnderPosition;
			this.lookUnderPositionNavmesh = this.targetPlayer.LastNavmeshPosition;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.SetDestination(this.lookUnderPositionNavmesh);
		if (Vector3.Distance(base.transform.position, this.lookUnderPositionNavmesh) < 0.5f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemySlowWalker.State.LookUnderIntro);
				return;
			}
		}
		else if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0003DEC4 File Offset: 0x0003C0C4
	public void StateLookUnderIntro()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnder);
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0003DF14 File Offset: 0x0003C114
	public void StateLookUnder()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Vision.StandOverride(0.25f);
		if (this.targetPlayer.isCrawling && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 3f && Vector3.Dot(this.lookAtTransform.forward, this.targetPlayer.transform.position - this.lookAtTransform.position) > 0.5f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderAttack);
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.LookUnderStop);
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0003DFF8 File Offset: 0x0003C1F8
	public void StateLookUnderAttack()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.6f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemySlowWalker.State.LookUnderStop);
				return;
			}
			this.UpdateState(EnemySlowWalker.State.LookUnder);
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0003E05C File Offset: 0x0003C25C
	public void StateLookUnderStop()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0003E0AC File Offset: 0x0003C2AC
	public void StateStun()
	{
		if (this.stateImpulse)
		{
			if (!this.enemy.Rigidbody.grabbed)
			{
				this.enemy.Rigidbody.rb.AddTorque(-base.transform.right * 15f, ForceMode.Impulse);
			}
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0003E154 File Offset: 0x0003C354
	public void StateLeave()
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
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0003E29E File Offset: 0x0003C49E
	public void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0003E2DA File Offset: 0x0003C4DA
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemySlowWalker.State.Spawn);
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0003E2F8 File Offset: 0x0003C4F8
	public void OnHurt()
	{
		this.animator.sfxHurt.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemySlowWalker.State.Leave)
		{
			this.UpdateState(EnemySlowWalker.State.Idle);
		}
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0003E354 File Offset: 0x0003C554
	public void OnDeath()
	{
		this.particleDeathImpact.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathImpact.Play();
		this.particleDeathBitsFar.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathBitsFar.Play();
		this.particleDeathBitsShort.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathBitsShort.Play();
		this.particleDeathSmoke.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathSmoke.Play();
		this.animator.sfxDeath.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0003E4B8 File Offset: 0x0003C6B8
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemySlowWalker.State.Idle || this.currentState == EnemySlowWalker.State.Roam || this.currentState == EnemySlowWalker.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemySlowWalker.State.Investigate);
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0003E504 File Offset: 0x0003C704
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemySlowWalker.State.Roam || this.currentState == EnemySlowWalker.State.Idle || this.currentState == EnemySlowWalker.State.Investigate)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (!this.enemy.OnScreen.OnScreenAny)
			{
				this.UpdateState(EnemySlowWalker.State.Sneak);
			}
			else
			{
				this.UpdateState(EnemySlowWalker.State.Notice);
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
		else if (this.currentState == EnemySlowWalker.State.GoToPlayer || this.currentState == EnemySlowWalker.State.Sneak)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.stateTimer = 10f;
				return;
			}
		}
		else if (this.currentState == EnemySlowWalker.State.LookUnderStart)
		{
			if (this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer && !this.targetPlayer.isCrawling)
			{
				this.UpdateState(EnemySlowWalker.State.GoToPlayer);
				return;
			}
		}
		else if ((this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderIntro || this.currentState == EnemySlowWalker.State.LookUnderAttack) && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			if (this.targetPlayer.isCrawling)
			{
				this.lookUnderLookAtPosition = this.targetPlayer.transform.position;
				return;
			}
			this.UpdateState(EnemySlowWalker.State.LookUnderStop);
		}
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0003E690 File Offset: 0x0003C890
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemySlowWalker.State.Leave)
			{
				this.grabAggroTimer = 60f;
				PlayerAvatar onGrabbedPlayerAvatar = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y > 1.15f || onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y < -1f)
				{
					return;
				}
				this.targetPlayer = onGrabbedPlayerAvatar;
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
					else
					{
						this.NoticeRPC(this.targetPlayer.photonView.ViewID);
					}
				}
				this.UpdateState(EnemySlowWalker.State.Notice);
			}
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0003E7A4 File Offset: 0x0003C9A4
	private void UpdateState(EnemySlowWalker.State _state)
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

	// Token: 0x06000676 RID: 1654 RVA: 0x0003E824 File Offset: 0x0003CA24
	private void RotationLogic()
	{
		if (this.currentState == EnemySlowWalker.State.Notice)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.currentState == EnemySlowWalker.State.LookUnderStart || this.currentState == EnemySlowWalker.State.LookUnderIntro || this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderAttack)
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
		if (this.currentState == EnemySlowWalker.State.Attack)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f && this.stateTimer > 2.5f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
				this.rotationTarget.eulerAngles = new Vector3(0f, this.rotationTarget.eulerAngles.y, 0f);
			}
			this.horizontalRotationSpring.speed = 15f;
			this.horizontalRotationSpring.damping = 0.8f;
		}
		else
		{
			this.horizontalRotationSpring.speed = 5f;
			this.horizontalRotationSpring.damping = 0.7f;
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0003EB04 File Offset: 0x0003CD04
	private void HeadLookAt()
	{
		if (this.currentState == EnemySlowWalker.State.LookUnder || this.currentState == EnemySlowWalker.State.LookUnderAttack)
		{
			Vector3 vector = this.lookUnderLookAtPosition - this.lookAtTransform.position;
			vector = SemiFunc.ClampDirection(vector, base.transform.forward, 60f);
			Quaternion localRotation = this.lookAtTransform.localRotation;
			this.lookAtTransform.rotation = Quaternion.LookRotation(vector);
			Quaternion localRotation2 = this.lookAtTransform.localRotation;
			localRotation2.eulerAngles = new Vector3(0f, localRotation2.eulerAngles.y, 0f);
			this.lookAtTransform.localRotation = Quaternion.Lerp(localRotation, localRotation2, Time.deltaTime * 10f);
			this.enemy.Vision.VisionTransform.rotation = this.lookAtTransform.rotation;
		}
		else
		{
			this.lookAtTransform.localRotation = Quaternion.Lerp(this.lookAtTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
			this.enemy.Vision.VisionTransform.localRotation = Quaternion.identity;
		}
		this.animator.SpringLogic();
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x0003EC30 File Offset: 0x0003CE30
	private void VisionDotLogic()
	{
		if (this.currentState == EnemySlowWalker.State.LookUnder)
		{
			this.enemy.Vision.VisionDotStanding = 0f;
			return;
		}
		this.enemy.Vision.VisionDotStanding = this.visionDotStandingDefault;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0003EC68 File Offset: 0x0003CE68
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0003EC7C File Offset: 0x0003CE7C
	private void AttackOffsetLogic()
	{
		if (this.currentState != EnemySlowWalker.State.Attack)
		{
			this.attackOffsetActive = false;
		}
		if (this.attackOffsetActive)
		{
			this.attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.attackOffsetTransform.localPosition.z, 1.5f, Time.deltaTime * 4f));
			this.enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 40f);
			return;
		}
		this.attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.attackOffsetTransform.localPosition.z, 0f, Time.deltaTime * 1f));
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0003ED40 File Offset: 0x0003CF40
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.stuckAttackTarget = Vector3.zero;
		if (this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		}
		if (this.stuckAttackTarget != Vector3.zero)
		{
			this.UpdateState(EnemySlowWalker.State.StuckAttack);
			return;
		}
		this.UpdateState(EnemySlowWalker.State.Idle);
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0003EDA1 File Offset: 0x0003CFA1
	[PunRPC]
	private void UpdateStateRPC(EnemySlowWalker.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemySlowWalker.State.Spawn)
		{
			this.animator.OnSpawn();
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0003EDC0 File Offset: 0x0003CFC0
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

	// Token: 0x0600067E RID: 1662 RVA: 0x0003EE28 File Offset: 0x0003D028
	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x0003EE2A File Offset: 0x0003D02A
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.lookUnderLookAtPosition);
			return;
		}
		this.lookUnderLookAtPosition = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x04000ABB RID: 2747
	public EnemySlowWalker.State currentState;

	// Token: 0x04000ABC RID: 2748
	public float stateTimer;

	// Token: 0x04000ABD RID: 2749
	public EnemySlowWalkerAnim animator;

	// Token: 0x04000ABE RID: 2750
	public ParticleSystem particleDeathImpact;

	// Token: 0x04000ABF RID: 2751
	public ParticleSystem particleDeathBitsFar;

	// Token: 0x04000AC0 RID: 2752
	public ParticleSystem particleDeathBitsShort;

	// Token: 0x04000AC1 RID: 2753
	public ParticleSystem particleDeathSmoke;

	// Token: 0x04000AC2 RID: 2754
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x04000AC3 RID: 2755
	private Quaternion rotationTarget;

	// Token: 0x04000AC4 RID: 2756
	private bool stateImpulse = true;

	// Token: 0x04000AC5 RID: 2757
	internal PlayerAvatar targetPlayer;

	// Token: 0x04000AC6 RID: 2758
	public Enemy enemy;

	// Token: 0x04000AC7 RID: 2759
	private PhotonView photonView;

	// Token: 0x04000AC8 RID: 2760
	private Vector3 agentDestination;

	// Token: 0x04000AC9 RID: 2761
	private Vector3 backToNavMeshPosition;

	// Token: 0x04000ACA RID: 2762
	private Vector3 stuckAttackTarget;

	// Token: 0x04000ACB RID: 2763
	private Vector3 targetPosition;

	// Token: 0x04000ACC RID: 2764
	private float visionTimer;

	// Token: 0x04000ACD RID: 2765
	private bool visionPrevious;

	// Token: 0x04000ACE RID: 2766
	public Transform feetTransform;

	// Token: 0x04000ACF RID: 2767
	private float grabAggroTimer;

	// Token: 0x04000AD0 RID: 2768
	private int attackCount;

	// Token: 0x04000AD1 RID: 2769
	private Vector3 lookUnderPosition;

	// Token: 0x04000AD2 RID: 2770
	private Vector3 lookUnderLookAtPosition;

	// Token: 0x04000AD3 RID: 2771
	private Vector3 lookUnderPositionNavmesh;

	// Token: 0x04000AD4 RID: 2772
	internal bool lookUnderAttackImpulse;

	// Token: 0x04000AD5 RID: 2773
	public Transform lookAtTransform;

	// Token: 0x04000AD6 RID: 2774
	private float visionDotStandingDefault;

	// Token: 0x04000AD7 RID: 2775
	internal bool attackOffsetActive;

	// Token: 0x04000AD8 RID: 2776
	public Transform attackOffsetTransform;

	// Token: 0x020002F1 RID: 753
	public enum State
	{
		// Token: 0x0400250F RID: 9487
		Spawn,
		// Token: 0x04002510 RID: 9488
		Idle,
		// Token: 0x04002511 RID: 9489
		Roam,
		// Token: 0x04002512 RID: 9490
		Investigate,
		// Token: 0x04002513 RID: 9491
		Notice,
		// Token: 0x04002514 RID: 9492
		GoToPlayer,
		// Token: 0x04002515 RID: 9493
		Sneak,
		// Token: 0x04002516 RID: 9494
		Attack,
		// Token: 0x04002517 RID: 9495
		StuckAttack,
		// Token: 0x04002518 RID: 9496
		LookUnderStart,
		// Token: 0x04002519 RID: 9497
		LookUnderIntro,
		// Token: 0x0400251A RID: 9498
		LookUnder,
		// Token: 0x0400251B RID: 9499
		LookUnderAttack,
		// Token: 0x0400251C RID: 9500
		LookUnderStop,
		// Token: 0x0400251D RID: 9501
		Stun,
		// Token: 0x0400251E RID: 9502
		Leave,
		// Token: 0x0400251F RID: 9503
		Despawn
	}
}
