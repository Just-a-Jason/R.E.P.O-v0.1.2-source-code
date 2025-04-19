using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000083 RID: 131
public class EnemyUpscream : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x00032663 File Offset: 0x00030863
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00032680 File Offset: 0x00030880
	private void Update()
	{
		this.HeadLogic();
		this.EyeLogic();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyUpscream.State.Stun);
			}
			switch (this.currentState)
			{
			case EnemyUpscream.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyUpscream.State.Idle:
				this.StateIdle();
				this.IdleBreakLogic();
				break;
			case EnemyUpscream.State.Roam:
				this.StateRoam();
				this.AgentVelocityRotation();
				this.IdleBreakLogic();
				break;
			case EnemyUpscream.State.Investigate:
				this.StateInvestigate();
				this.AgentVelocityRotation();
				break;
			case EnemyUpscream.State.PlayerNotice:
				this.StatePlayerNotice();
				break;
			case EnemyUpscream.State.GoToPlayer:
				this.AgentVelocityRotation();
				this.StateGoToPlayer();
				break;
			case EnemyUpscream.State.Attack:
				this.StateAttack();
				break;
			case EnemyUpscream.State.Leave:
				this.StateLeave();
				break;
			case EnemyUpscream.State.IdleBreak:
				this.StateIdleBreak();
				break;
			case EnemyUpscream.State.Stun:
				this.StateStun();
				break;
			}
		}
		if (this.currentState == EnemyUpscream.State.Attack && this.targetPlayer)
		{
			if (this.targetPlayer.isLocal)
			{
				PlayerController.instance.InputDisable(0.1f);
				CameraAim.Instance.AimTargetSet(this.visionTransform.position, 0.1f, 5f, base.gameObject, 90);
				CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 5f, 5f, base.gameObject, 50);
				Color color = new Color(0.4f, 0f, 0f, 1f);
				PostProcessing.Instance.VignetteOverride(color, 0.75f, 1f, 3.5f, 2.5f, 0.5f, base.gameObject);
			}
			if (this.attackImpulse)
			{
				if (this.targetPlayer.isLocal)
				{
					this.targetPlayer.physGrabber.ReleaseObject(0.1f);
					CameraGlitch.Instance.PlayLong();
				}
				this.attackImpulse = false;
				this.upscreamAnim.animator.SetTrigger("Attack");
				return;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0003289F File Offset: 0x00030A9F
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x000328D8 File Offset: 0x00030AD8
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			if (this.previousState == EnemyUpscream.State.Spawn)
			{
				this.stateTimer = 0.5f;
			}
			else
			{
				this.stateTimer = Random.Range(3f, 8f);
			}
			this.stateImpulse = false;
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00032998 File Offset: 0x00030B98
	private void StateRoam()
	{
		float num = Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination());
		if (this.stateImpulse || !this.enemy.NavMeshAgent.HasPath() || num < 1f)
		{
			if (this.stateImpulse)
			{
				this.roamWaitTimer = 0f;
				this.stateImpulse = false;
			}
			if (this.roamWaitTimer <= 0f)
			{
				this.stateTimer = 5f;
				this.roamWaitTimer = Random.Range(0f, 5f);
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
					this.agentPoint = navMeshHit.position;
					this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
				}
			}
			else
			{
				this.roamWaitTimer -= Time.deltaTime;
			}
		}
		else
		{
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
				if (this.stateTimer <= 0f)
				{
					this.UpdateState(EnemyUpscream.State.Idle);
				}
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00032B7C File Offset: 0x00030D7C
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
			this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentPoint) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyUpscream.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x00032C58 File Offset: 0x00030E58
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.5f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
		}
		this.enemy.NavMeshAgent.Stop(0.5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyUpscream.State.GoToPlayer);
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x00032D04 File Offset: 0x00030F04
	private void StateGoToPlayer()
	{
		if (!this.enemy.Jump.jumping)
		{
			this.enemy.NavMeshAgent.SetDestination(this.targetPlayer.transform.position);
		}
		else
		{
			this.enemy.NavMeshAgent.Disable(0.1f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPlayer.transform.position, 5f * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) < 1.5f && !this.enemy.Jump.jumping && !this.enemy.IsStunned())
		{
			this.enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyUpscream.State.Attack);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (this.stateTimer <= 0f)
			{
				this.enemy.Jump.StuckReset();
				this.UpdateState(EnemyUpscream.State.Leave);
			}
			else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.targetPlayer.transform.position) > 1.5f)
			{
				this.enemy.Jump.StuckTrigger(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position);
			}
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Leave);
		}
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x00032F34 File Offset: 0x00031134
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.attacks++;
			this.stateTimer = 1.5f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
		}
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.1f)
			{
				this.UpdateState(EnemyUpscream.State.Leave);
				return;
			}
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00033064 File Offset: 0x00031264
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
			if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.agentPoint = navMeshHit.position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.stateImpulse = false;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.NavMeshAgent.SetDestination(this.agentPoint);
		this.enemy.NavMeshAgent.OverrideAgent(this.enemy.NavMeshAgent.DefaultSpeed + 2.5f, this.enemy.NavMeshAgent.DefaultAcceleration + 2.5f, 0.2f);
		this.enemy.Rigidbody.OverrideFollowPosition(1f, 10f, -1f);
		if (Vector3.Distance(base.transform.position, this.agentPoint) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0003321C File Offset: 0x0003141C
	private void StateIdleBreak()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0003329E File Offset: 0x0003149E
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyUpscream.State.Idle);
		}
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x000332B4 File Offset: 0x000314B4
	internal void UpdateState(EnemyUpscream.State _state)
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.currentState == _state)
		{
			return;
		}
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				_state
			});
			return;
		}
		this.UpdateStateRPC(_state);
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0003330C File Offset: 0x0003150C
	private void IdleBreakLogic()
	{
		if (this.idleBreakTimer >= 0f)
		{
			this.idleBreakTimer -= Time.deltaTime;
			if (this.idleBreakTimer <= 0f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyUpscream.State.IdleBreak);
				this.idleBreakTimer = Random.Range(this.idleBreakTimeMin, this.idleBreakTimeMax);
			}
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0003336E File Offset: 0x0003156E
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyUpscream.State.Spawn);
		}
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0003338B File Offset: 0x0003158B
	public void OnHurt()
	{
		this.upscreamAnim.hurtSound.Play(this.upscreamAnim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x000333C4 File Offset: 0x000315C4
	public void OnDeath()
	{
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00033464 File Offset: 0x00031664
	public void OnVision()
	{
		if (this.currentState == EnemyUpscream.State.Idle || this.currentState == EnemyUpscream.State.Roam || this.currentState == EnemyUpscream.State.IdleBreak || this.currentState == EnemyUpscream.State.Investigate)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
			if (!this.enemy.IsStunned())
			{
				if (GameManager.Multiplayer())
				{
					this.photonView.RPC("NoticeSetRPC", RpcTarget.All, new object[]
					{
						this.enemy.Vision.onVisionTriggeredID
					});
				}
				else
				{
					this.upscreamAnim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
				}
			}
			this.UpdateState(EnemyUpscream.State.PlayerNotice);
			return;
		}
		if (this.currentState == EnemyUpscream.State.GoToPlayer && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00033580 File Offset: 0x00031780
	public void OnInvestigate()
	{
		if (this.currentState == EnemyUpscream.State.Roam || this.currentState == EnemyUpscream.State.Idle || this.currentState == EnemyUpscream.State.IdleBreak || this.currentState == EnemyUpscream.State.Investigate)
		{
			this.UpdateState(EnemyUpscream.State.Investigate);
			this.agentPoint = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x000335D0 File Offset: 0x000317D0
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyUpscream.State.Leave)
			{
				this.grabAggroTimer = 60f;
				if (this.targetPlayer != this.enemy.Rigidbody.onGrabbedPlayerAvatar)
				{
					this.targetPlayer = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
				}
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeSetRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
					else
					{
						this.upscreamAnim.NoticeSet(this.targetPlayer.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyUpscream.State.PlayerNotice);
			}
		}
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x000336DC File Offset: 0x000318DC
	public void HeadLogic()
	{
		Quaternion targetRotation = this.headIdleTransform.rotation;
		if (this.targetPlayer && (this.currentState == EnemyUpscream.State.PlayerNotice || this.currentState == EnemyUpscream.State.GoToPlayer || this.currentState == EnemyUpscream.State.Attack) && !this.enemy.IsStunned())
		{
			Vector3 a = this.targetPlayer.PlayerVisionTarget.VisionTransform.position;
			if (this.targetPlayer.isLocal)
			{
				a = this.targetPlayer.localCameraPosition;
			}
			targetRotation = Quaternion.LookRotation(a - this.headTransform.position);
		}
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, targetRotation, -1f);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0003378C File Offset: 0x0003198C
	public void EyeLogic()
	{
		if (this.currentState == EnemyUpscream.State.PlayerNotice || this.currentState == EnemyUpscream.State.GoToPlayer || this.currentState == EnemyUpscream.State.Attack)
		{
			this.eyeLeftSpring.damping = 0.6f;
			this.eyeLeftSpring.speed = 15f;
			this.eyeRightSpring.damping = 0.6f;
			this.eyeRightSpring.speed = 15f;
			this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftTarget.rotation, -1f);
			this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightTarget.rotation, -1f);
			return;
		}
		this.eyeLeftSpring.damping = 0.2f;
		this.eyeLeftSpring.speed = 15f;
		this.eyeRightSpring.damping = 0.2f;
		this.eyeRightSpring.speed = 15f;
		this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftIdle.rotation, -1f);
		this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightIdle.rotation, -1f);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x000338D0 File Offset: 0x00031AD0
	private void AgentVelocityRotation()
	{
		if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.005f)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
			float num = 2f;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, num * Time.deltaTime);
		}
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00033959 File Offset: 0x00031B59
	[PunRPC]
	private void UpdateStateRPC(EnemyUpscream.State _state)
	{
		this.previousState = this.currentState;
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (this.currentState == EnemyUpscream.State.Spawn)
		{
			this.upscreamAnim.SetSpawn();
		}
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00033994 File Offset: 0x00031B94
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

	// Token: 0x0600052F RID: 1327 RVA: 0x000339FC File Offset: 0x00031BFC
	[PunRPC]
	private void NoticeSetRPC(int _playerID)
	{
		this.upscreamAnim.NoticeSet(_playerID);
	}

	// Token: 0x04000827 RID: 2087
	[Header("References")]
	public EnemyUpscreamAnim upscreamAnim;

	// Token: 0x04000828 RID: 2088
	internal Enemy enemy;

	// Token: 0x04000829 RID: 2089
	public ParticleSystem[] deathEffects;

	// Token: 0x0400082A RID: 2090
	public EnemyUpscream.State currentState;

	// Token: 0x0400082B RID: 2091
	public EnemyUpscream.State previousState;

	// Token: 0x0400082C RID: 2092
	private float stateTimer;

	// Token: 0x0400082D RID: 2093
	private bool attackImpulse;

	// Token: 0x0400082E RID: 2094
	private bool stateImpulse;

	// Token: 0x0400082F RID: 2095
	internal PlayerAvatar targetPlayer;

	// Token: 0x04000830 RID: 2096
	private Vector3 targetPosition;

	// Token: 0x04000831 RID: 2097
	public Transform visionTransform;

	// Token: 0x04000832 RID: 2098
	private float hasVisionTimer;

	// Token: 0x04000833 RID: 2099
	private Vector3 agentPoint;

	// Token: 0x04000834 RID: 2100
	private float roamWaitTimer;

	// Token: 0x04000835 RID: 2101
	private PhotonView photonView;

	// Token: 0x04000836 RID: 2102
	[Header("Head")]
	public SpringQuaternion headSpring;

	// Token: 0x04000837 RID: 2103
	public Transform headTransform;

	// Token: 0x04000838 RID: 2104
	public Transform headIdleTransform;

	// Token: 0x04000839 RID: 2105
	[Header("Eyes")]
	public SpringQuaternion eyeLeftSpring;

	// Token: 0x0400083A RID: 2106
	[Space(10f)]
	public Transform eyeLeftTransform;

	// Token: 0x0400083B RID: 2107
	public Transform eyeLeftIdle;

	// Token: 0x0400083C RID: 2108
	public Transform eyeLeftTarget;

	// Token: 0x0400083D RID: 2109
	[Space(10f)]
	public SpringQuaternion eyeRightSpring;

	// Token: 0x0400083E RID: 2110
	[Space(10f)]
	public Transform eyeRightTransform;

	// Token: 0x0400083F RID: 2111
	public Transform eyeRightIdle;

	// Token: 0x04000840 RID: 2112
	public Transform eyeRightTarget;

	// Token: 0x04000841 RID: 2113
	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	// Token: 0x04000842 RID: 2114
	public float idleBreakTimeMax = 90f;

	// Token: 0x04000843 RID: 2115
	private float idleBreakTimer;

	// Token: 0x04000844 RID: 2116
	private float grabAggroTimer;

	// Token: 0x04000845 RID: 2117
	private int attacks;

	// Token: 0x020002E4 RID: 740
	public enum State
	{
		// Token: 0x040024D6 RID: 9430
		Spawn,
		// Token: 0x040024D7 RID: 9431
		Idle,
		// Token: 0x040024D8 RID: 9432
		Roam,
		// Token: 0x040024D9 RID: 9433
		Investigate,
		// Token: 0x040024DA RID: 9434
		PlayerNotice,
		// Token: 0x040024DB RID: 9435
		GoToPlayer,
		// Token: 0x040024DC RID: 9436
		Attack,
		// Token: 0x040024DD RID: 9437
		Leave,
		// Token: 0x040024DE RID: 9438
		IdleBreak,
		// Token: 0x040024DF RID: 9439
		Stun
	}
}
