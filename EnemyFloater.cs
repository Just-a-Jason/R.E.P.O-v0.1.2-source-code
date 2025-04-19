using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000049 RID: 73
public class EnemyFloater : MonoBehaviour
{
	// Token: 0x06000229 RID: 553 RVA: 0x00016506 File Offset: 0x00014706
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00016514 File Offset: 0x00014714
	private void Update()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.FloatingAnimation();
		if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned() && this.currentState == EnemyFloater.State.Idle)
		{
			this.UpdateState(EnemyFloater.State.Despawn);
		}
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyFloater.State.Stun);
		}
		switch (this.currentState)
		{
		case EnemyFloater.State.Spawn:
			this.StateSpawn();
			break;
		case EnemyFloater.State.Idle:
			this.StateIdle();
			break;
		case EnemyFloater.State.Roam:
			this.StateRoam();
			break;
		case EnemyFloater.State.Investigate:
			this.StateInvestigate();
			break;
		case EnemyFloater.State.Notice:
			this.StateNotice();
			break;
		case EnemyFloater.State.GoToPlayer:
			this.StateGoToPlayer();
			break;
		case EnemyFloater.State.Sneak:
			this.StateSneak();
			break;
		case EnemyFloater.State.ChargeAttack:
			this.StateChargeAttack();
			break;
		case EnemyFloater.State.DelayAttack:
			this.StateDelayAttack();
			break;
		case EnemyFloater.State.Attack:
			this.StateAttack();
			break;
		case EnemyFloater.State.Stun:
			this.StateStun();
			break;
		case EnemyFloater.State.Leave:
			this.StateLeave();
			break;
		case EnemyFloater.State.Despawn:
			this.StateDespawn();
			break;
		}
		this.RotationLogic();
		this.TimerLogic();
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00016628 File Offset: 0x00014828
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
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00016678 File Offset: 0x00014878
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
			this.UpdateState(EnemyFloater.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00016710 File Offset: 0x00014910
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
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
			{
				this.UpdateState(EnemyFloater.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0001688C File Offset: 0x00014A8C
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
				this.UpdateState(EnemyFloater.State.Idle);
				return;
			}
			if (Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				this.UpdateState(EnemyFloater.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Leave);
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00016958 File Offset: 0x00014B58
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
			if (Vector3.Distance(this.feetTransform.position, this.targetPlayer.transform.position) < 2.5f)
			{
				this.UpdateState(EnemyFloater.State.ChargeAttack);
				return;
			}
			this.UpdateState(EnemyFloater.State.GoToPlayer);
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00016A00 File Offset: 0x00014C00
	public void StateGoToPlayer()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.targetPosition = this.targetPlayer.transform.position;
		this.enemy.NavMeshAgent.SetDestination(this.targetPosition);
		this.enemy.NavMeshAgent.OverrideAgent(2f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f && this.stateTimer > 1.5f)
		{
			this.UpdateState(EnemyFloater.State.ChargeAttack);
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00016AF4 File Offset: 0x00014CF4
	public void StateSneak()
	{
		if (!this.targetPlayer)
		{
			this.UpdateState(EnemyFloater.State.Idle);
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
		this.enemy.NavMeshAgent.OverrideAgent(1.5f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
			return;
		}
		if (Vector3.Distance(this.feetTransform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f || this.enemy.OnScreen.OnScreenAny)
		{
			this.UpdateState(EnemyFloater.State.Notice);
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x00016C30 File Offset: 0x00014E30
	public void StateChargeAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 7f;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.DelayAttack);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00016CA8 File Offset: 0x00014EA8
	public void StateDelayAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Attack);
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00016CF8 File Offset: 0x00014EF8
	public void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.attackCount++;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attackCount >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attackCount = 0;
				this.UpdateState(EnemyFloater.State.Leave);
				return;
			}
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00016D84 File Offset: 0x00014F84
	public void StateStun()
	{
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00016DDC File Offset: 0x00014FDC
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
		this.enemy.NavMeshAgent.OverrideAgent(1.5f, this.enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00016F50 File Offset: 0x00015150
	public void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.feetTransform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00016F8C File Offset: 0x0001518C
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyFloater.State.Spawn);
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00016FAC File Offset: 0x000151AC
	public void OnHurt()
	{
		this.animator.sfxHurt.Play(this.animator.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyFloater.State.Leave)
		{
			this.UpdateState(EnemyFloater.State.Idle);
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00017008 File Offset: 0x00015208
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
		this.animator.SfxDeath();
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x00017140 File Offset: 0x00015340
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyFloater.State.Idle || this.currentState == EnemyFloater.State.Roam || this.currentState == EnemyFloater.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyFloater.State.Investigate);
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0001718C File Offset: 0x0001538C
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyFloater.State.Roam || this.currentState == EnemyFloater.State.Idle || this.currentState == EnemyFloater.State.Investigate)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (!this.enemy.OnScreen.OnScreenAny)
			{
				this.UpdateState(EnemyFloater.State.Sneak);
			}
			else
			{
				this.UpdateState(EnemyFloater.State.Notice);
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
		else if ((this.currentState == EnemyFloater.State.GoToPlayer || this.currentState == EnemyFloater.State.Sneak) && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00017270 File Offset: 0x00015470
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyFloater.State.Leave)
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
				this.UpdateState(EnemyFloater.State.Notice);
			}
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00017384 File Offset: 0x00015584
	private void UpdateState(EnemyFloater.State _state)
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

	// Token: 0x0600023F RID: 575 RVA: 0x00017404 File Offset: 0x00015604
	private void FloatingAnimation()
	{
		float num = 0.1f;
		float num2 = 0.4f;
		float t = this.followParentCurve.Evaluate(this.followParentLerp);
		float num3 = Mathf.Lerp(-num, num, t);
		float num4 = 0f;
		Vector3 localPosition = new Vector3(this.followParentTransform.localPosition.x, num3 + num4, this.followParentTransform.localPosition.z);
		this.followParentLerp += Time.deltaTime * num2;
		if (this.followParentLerp > 1f)
		{
			this.followParentLerp = 0f;
		}
		this.followParentTransform.localPosition = localPosition;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x000174A4 File Offset: 0x000156A4
	private void RotationLogic()
	{
		if (this.currentState == EnemyFloater.State.Notice)
		{
			if (this.targetPlayer && Vector3.Distance(this.targetPlayer.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
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

	// Token: 0x06000241 RID: 577 RVA: 0x000175F1 File Offset: 0x000157F1
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00017605 File Offset: 0x00015805
	[PunRPC]
	private void UpdateStateRPC(EnemyFloater.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemyFloater.State.Spawn)
		{
			this.animator.OnSpawn();
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00017624 File Offset: 0x00015824
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

	// Token: 0x06000244 RID: 580 RVA: 0x0001768C File Offset: 0x0001588C
	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		this.animator.NoticeSet(_playerID);
	}

	// Token: 0x0400040A RID: 1034
	public EnemyFloater.State currentState;

	// Token: 0x0400040B RID: 1035
	public float stateTimer;

	// Token: 0x0400040C RID: 1036
	public EnemyFloaterAnim animator;

	// Token: 0x0400040D RID: 1037
	public ParticleSystem particleDeathImpact;

	// Token: 0x0400040E RID: 1038
	public ParticleSystem particleDeathBitsFar;

	// Token: 0x0400040F RID: 1039
	public ParticleSystem particleDeathBitsShort;

	// Token: 0x04000410 RID: 1040
	public ParticleSystem particleDeathSmoke;

	// Token: 0x04000411 RID: 1041
	public SpringQuaternion rotationSpring;

	// Token: 0x04000412 RID: 1042
	private Quaternion rotationTarget;

	// Token: 0x04000413 RID: 1043
	private bool stateImpulse = true;

	// Token: 0x04000414 RID: 1044
	internal PlayerAvatar targetPlayer;

	// Token: 0x04000415 RID: 1045
	public Enemy enemy;

	// Token: 0x04000416 RID: 1046
	private PhotonView photonView;

	// Token: 0x04000417 RID: 1047
	private Vector3 agentDestination;

	// Token: 0x04000418 RID: 1048
	private Vector3 backToNavMeshPosition;

	// Token: 0x04000419 RID: 1049
	private Vector3 stuckAttackTarget;

	// Token: 0x0400041A RID: 1050
	private Vector3 targetPosition;

	// Token: 0x0400041B RID: 1051
	private float visionTimer;

	// Token: 0x0400041C RID: 1052
	private bool visionPrevious;

	// Token: 0x0400041D RID: 1053
	public Transform feetTransform;

	// Token: 0x0400041E RID: 1054
	public Transform followParentTransform;

	// Token: 0x0400041F RID: 1055
	public AnimationCurve followParentCurve;

	// Token: 0x04000420 RID: 1056
	private float followParentLerp;

	// Token: 0x04000421 RID: 1057
	private float grabAggroTimer;

	// Token: 0x04000422 RID: 1058
	private int attackCount;

	// Token: 0x020002CF RID: 719
	public enum State
	{
		// Token: 0x04002402 RID: 9218
		Spawn,
		// Token: 0x04002403 RID: 9219
		Idle,
		// Token: 0x04002404 RID: 9220
		Roam,
		// Token: 0x04002405 RID: 9221
		Investigate,
		// Token: 0x04002406 RID: 9222
		Notice,
		// Token: 0x04002407 RID: 9223
		GoToPlayer,
		// Token: 0x04002408 RID: 9224
		Sneak,
		// Token: 0x04002409 RID: 9225
		ChargeAttack,
		// Token: 0x0400240A RID: 9226
		DelayAttack,
		// Token: 0x0400240B RID: 9227
		Attack,
		// Token: 0x0400240C RID: 9228
		Stun,
		// Token: 0x0400240D RID: 9229
		Leave,
		// Token: 0x0400240E RID: 9230
		Despawn
	}
}
