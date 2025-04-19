using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200003F RID: 63
public class EnemyBeamer : MonoBehaviour, IPunObservable
{
	// Token: 0x06000153 RID: 339 RVA: 0x0000CED6 File Offset: 0x0000B0D6
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
	private void Update()
	{
		this.VerticalAimLogic();
		this.LaserLogic();
		this.MoveFastLogic();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!LevelGenerator.Instance.Generated)
			{
				return;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyBeamer.State.Stun);
			}
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyBeamer.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyBeamer.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyBeamer.State.Idle:
				this.StateIdle();
				break;
			case EnemyBeamer.State.Roam:
				this.StateRoam();
				break;
			case EnemyBeamer.State.Investigate:
				this.StateInvestigate();
				break;
			case EnemyBeamer.State.AttackStart:
				this.StateAttackStart();
				break;
			case EnemyBeamer.State.Attack:
				this.StateAttack();
				break;
			case EnemyBeamer.State.AttackEnd:
				this.StateAttackEnd();
				break;
			case EnemyBeamer.State.MeleeStart:
				this.StateMeleeStart();
				break;
			case EnemyBeamer.State.Melee:
				this.StateMelee();
				break;
			case EnemyBeamer.State.Seek:
				this.StateSeek();
				break;
			case EnemyBeamer.State.Leave:
				this.StateLeave();
				break;
			case EnemyBeamer.State.Stun:
				this.StateStun();
				break;
			case EnemyBeamer.State.Despawn:
				this.StateDespawn();
				break;
			}
			this.RotationLogic();
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000D008 File Offset: 0x0000B208
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
			this.UpdateState(EnemyBeamer.State.Idle);
		}
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000D08C File Offset: 0x0000B28C
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(4f, 8f);
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
			this.UpdateState(EnemyBeamer.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBeamer.State.Leave);
		}
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000D138 File Offset: 0x0000B338
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 999f;
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
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (!this.enemy.Jump.jumping && Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			this.UpdateState(EnemyBeamer.State.Idle);
		}
		else if (this.enemy.Rigidbody.notMovingTimer >= 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBeamer.State.Leave);
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000D2BC File Offset: 0x0000B4BC
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 999f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			if (!this.enemy.Jump.jumping && (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.agentDestination) < 1f))
			{
				this.UpdateState(EnemyBeamer.State.Idle);
			}
			else if (this.enemy.Rigidbody.notMovingTimer >= 3f)
			{
				this.AttackNearestPhysObjectOrGoToIdle();
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBeamer.State.Leave);
		}
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000D3B8 File Offset: 0x0000B5B8
	private void StateAttackStart()
	{
		if (this.stateImpulse)
		{
			this.aimHorizontalResult = 0f;
			this.stateImpulse = false;
			this.stateTimer = 1.5f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.seekDestination = this.playerTarget.transform.position;
			this.aimHorizontalLerp = 0f;
		}
		this.aimHorizontalResult = Mathf.Lerp(0f, -this.aimHorizontalSpread, this.aimHorizontalCurve.Evaluate(this.aimHorizontalLerp));
		this.aimHorizontalLerp += 1.5f * Time.deltaTime;
		this.aimHorizontalTarget = Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position);
		this.aimHorizontalTarget = Quaternion.Euler(0f, this.aimHorizontalTarget.eulerAngles.y, 0f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBeamer.State.Attack);
		}
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000D508 File Offset: 0x0000B708
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 0.5f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.aimHorizontalLerp = 0f;
		}
		this.aimHorizontalResult = Mathf.Lerp(-this.aimHorizontalSpread, this.aimHorizontalSpread, this.aimHorizontalCurve.Evaluate(this.aimHorizontalLerp));
		this.aimHorizontalLerp += this.aimHorizontalSpeed * Time.deltaTime;
		if (this.aimHorizontalLerp >= 1f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyBeamer.State.AttackEnd);
			}
		}
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000D5E4 File Offset: 0x0000B7E4
	private void StateAttackEnd()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.aimHorizontalLerp = 0f;
		}
		this.aimHorizontalResult = Mathf.Lerp(this.aimHorizontalSpread, 0f, this.aimHorizontalCurve.Evaluate(this.aimHorizontalLerp));
		this.aimHorizontalLerp += 1.5f * Time.deltaTime;
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBeamer.State.Seek);
		}
	}

	// Token: 0x0600015C RID: 348 RVA: 0x0000D6B4 File Offset: 0x0000B8B4
	private void StateMeleeStart()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 0.5f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.seekDestination = this.meleeTarget;
		}
		if (this.meleePlayer)
		{
			this.meleeTarget = this.playerTarget.transform.position;
			this.seekDestination = this.meleeTarget;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBeamer.State.Melee);
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000D76C File Offset: 0x0000B96C
	private void StateMelee()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBeamer.State.Seek);
		}
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000D7F0 File Offset: 0x0000B9F0
	private void StateSeek()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 20f;
			this.stateImpulse = false;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			if (Vector3.Distance(base.transform.position, this.seekDestination) >= 3f)
			{
				this.moveFast = true;
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("MoveFastRPC", RpcTarget.Others, new object[]
					{
						this.moveFast
					});
				}
			}
		}
		this.enemy.NavMeshAgent.SetDestination(this.seekDestination);
		if (this.moveFast)
		{
			this.enemy.NavMeshAgent.OverrideAgent(this.enemy.NavMeshAgent.DefaultSpeed * 2f, this.enemy.NavMeshAgent.DefaultAcceleration * 2f, 0.1f);
		}
		if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			LevelPoint levelPointAhead = this.enemy.GetLevelPointAhead(this.seekDestination);
			if (levelPointAhead)
			{
				this.seekDestination = levelPointAhead.transform.position;
			}
			if (this.moveFast)
			{
				this.moveFast = false;
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("MoveFastRPC", RpcTarget.Others, new object[]
					{
						this.moveFast
					});
				}
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer >= 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBeamer.State.Idle);
		}
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000D9AC File Offset: 0x0000BBAC
	public void StateLeave()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 999f;
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
			this.enemy.Vision.DisableVision(5f);
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			this.UpdateState(EnemyBeamer.State.Idle);
			return;
		}
		if (this.enemy.Rigidbody.notMovingTimer >= 3f)
		{
			this.AttackNearestPhysObjectOrGoToIdle();
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000DB14 File Offset: 0x0000BD14
	private void StateStun()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyBeamer.State.Idle);
		}
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000DB7C File Offset: 0x0000BD7C
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			this.stateImpulse = false;
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000DBCD File Offset: 0x0000BDCD
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyBeamer.State.Spawn);
		}
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000DBEC File Offset: 0x0000BDEC
	public void OnVision()
	{
		if (this.enemy.Jump.jumping)
		{
			return;
		}
		if (this.currentState == EnemyBeamer.State.Roam || this.currentState == EnemyBeamer.State.Idle || this.currentState == EnemyBeamer.State.Seek || this.currentState == EnemyBeamer.State.Leave || this.currentState == EnemyBeamer.State.Investigate)
		{
			this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
			if (this.playerTarget && Vector3.Distance(base.transform.position, this.playerTarget.transform.position) < 2.5f && Mathf.Abs(base.transform.position.y - this.playerTarget.transform.position.y) < 1f)
			{
				this.meleeTarget = this.playerTarget.transform.position;
				this.meleePlayer = true;
				this.UpdateState(EnemyBeamer.State.MeleeStart);
			}
			else if (this.laserCooldown <= 0f)
			{
				this.UpdateState(EnemyBeamer.State.AttackStart);
			}
			else
			{
				this.seekDestination = this.playerTarget.transform.position;
				this.UpdateState(EnemyBeamer.State.Seek);
			}
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					this.playerTarget.photonView.ViewID
				});
				return;
			}
		}
		else if ((this.currentState == EnemyBeamer.State.AttackStart || this.currentState == EnemyBeamer.State.Attack || this.currentState == EnemyBeamer.State.AttackEnd) && this.playerTarget == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.seekDestination = this.playerTarget.transform.position;
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000DD9C File Offset: 0x0000BF9C
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyBeamer.State.Idle || this.currentState == EnemyBeamer.State.Roam || this.currentState == EnemyBeamer.State.Seek || this.currentState == EnemyBeamer.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyBeamer.State.Investigate);
		}
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000DDF4 File Offset: 0x0000BFF4
	public void OnHurt()
	{
		this.anim.soundHurt.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		this.anim.soundHurtPauseTimer = 0.5f;
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyBeamer.State.Leave)
		{
			this.UpdateState(EnemyBeamer.State.Idle);
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000DE60 File Offset: 0x0000C060
	public void OnDeath()
	{
		this.anim.soundDeath.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		this.particleDeathSmoke.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathSmoke.Play();
		this.particleDeathBody.transform.position = this.enemy.CenterTransform.position;
		this.particleDeathBody.Play();
		this.particleDeathNose.transform.position = this.laserStartTransform.position;
		this.particleDeathNose.Play();
		this.particleDeathHat.transform.position = this.hatTransform.position;
		this.particleDeathHat.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000DFC4 File Offset: 0x0000C1C4
	private void UpdateState(EnemyBeamer.State _state)
	{
		if (_state == this.currentState)
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

	// Token: 0x06000168 RID: 360 RVA: 0x0000E02D File Offset: 0x0000C22D
	private void AttackNearestPhysObjectOrGoToIdle()
	{
		this.meleeTarget = SemiFunc.EnemyGetNearestPhysObject(this.enemy);
		if (this.meleeTarget != Vector3.zero)
		{
			this.meleePlayer = false;
			this.UpdateState(EnemyBeamer.State.Melee);
			return;
		}
		this.UpdateState(EnemyBeamer.State.Idle);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000E068 File Offset: 0x0000C268
	private void RotationLogic()
	{
		if (this.currentState == EnemyBeamer.State.AttackStart || this.currentState == EnemyBeamer.State.Attack || this.currentState == EnemyBeamer.State.AttackEnd)
		{
			this.horizontalRotationTarget = Quaternion.Euler(this.aimHorizontalTarget.eulerAngles.x, this.aimHorizontalTarget.eulerAngles.y + this.aimHorizontalResult, this.aimHorizontalTarget.eulerAngles.z);
		}
		else if (this.currentState == EnemyBeamer.State.MeleeStart)
		{
			if (Vector3.Distance(this.meleeTarget, this.enemy.Rigidbody.transform.position) > 0.1f)
			{
				this.horizontalRotationTarget = Quaternion.LookRotation(this.meleeTarget - this.enemy.Rigidbody.transform.position);
				this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
			}
		}
		else if (this.enemy.NavMeshAgent.AgentVelocity.normalized.magnitude > 0.1f)
		{
			this.horizontalRotationTarget = Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized);
			this.horizontalRotationTarget.eulerAngles = new Vector3(0f, this.horizontalRotationTarget.eulerAngles.y, 0f);
		}
		if (this.currentState == EnemyBeamer.State.Spawn || this.currentState == EnemyBeamer.State.Idle || this.currentState == EnemyBeamer.State.Roam || this.currentState == EnemyBeamer.State.Investigate || this.currentState == EnemyBeamer.State.Leave)
		{
			this.horizontalRotationSpring.speed = 5f;
			this.horizontalRotationSpring.damping = 0.7f;
		}
		else if (this.currentState == EnemyBeamer.State.AttackStart || this.currentState == EnemyBeamer.State.Attack || this.currentState == EnemyBeamer.State.AttackEnd)
		{
			this.horizontalRotationSpring.speed = 15f;
			this.horizontalRotationSpring.damping = 0.8f;
		}
		else
		{
			this.horizontalRotationSpring.speed = 10f;
			this.horizontalRotationSpring.damping = 0.8f;
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.horizontalRotationTarget, -1f);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000E2A0 File Offset: 0x0000C4A0
	private void VerticalAimLogic()
	{
		if (this.currentState != EnemyBeamer.State.AttackStart && this.currentState != EnemyBeamer.State.Attack && this.currentState != EnemyBeamer.State.AttackEnd)
		{
			this.aimVerticalTarget = Quaternion.identity;
		}
		else if (this.currentState == EnemyBeamer.State.AttackStart || (this.currentState != EnemyBeamer.State.Attack && this.aimHorizontalLerp < 0.1f))
		{
			Quaternion b = Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.laserRayTransform.position);
			if (this.aimVerticalTarget == Quaternion.identity)
			{
				this.aimVerticalTarget = b;
			}
			else
			{
				this.aimVerticalTarget = Quaternion.Lerp(this.aimVerticalTarget, b, 2f * Time.deltaTime);
			}
			Quaternion rotation = this.laserRayTransform.rotation;
			this.laserRayTransform.rotation = this.aimVerticalTarget;
			this.aimVerticalTarget = this.laserRayTransform.localRotation;
			this.aimVerticalTarget = Quaternion.Euler(this.laserRayTransform.eulerAngles.x, 0f, 0f);
			this.laserRayTransform.rotation = rotation;
		}
		this.aimVerticalTransform.localRotation = Quaternion.Lerp(this.aimVerticalTransform.localRotation, this.aimVerticalTarget, 20f * Time.deltaTime);
		this.laserRayTransform.localRotation = this.aimVerticalTarget;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x0000E3FC File Offset: 0x0000C5FC
	private void LaserLogic()
	{
		if (this.currentState != EnemyBeamer.State.Attack && this.currentState != EnemyBeamer.State.AttackStart && this.currentState != EnemyBeamer.State.AttackEnd)
		{
			this.laserCooldown -= Time.deltaTime;
		}
		if (this.currentState == EnemyBeamer.State.Attack)
		{
			this.laserCooldown = 3f;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Transform transform = this.laserStartTransform;
				Vector3 direction = this.laserRayTransform.position - this.laserStartTransform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.laserStartTransform.position, direction, out raycastHit, direction.magnitude, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					transform = this.laserRayTransform;
				}
				else if (Physics.OverlapSphere(this.laserStartTransform.position, 0.25f, LayerMask.GetMask(new string[]
				{
					"Default"
				})).Length != 0)
				{
					transform = this.laserRayTransform;
				}
				if (this.hitPositionTimer <= 0f)
				{
					this.hitPositionTimer = 0.05f;
					RaycastHit raycastHit2;
					if (Physics.Raycast(transform.position, transform.forward, out raycastHit2, this.laserRange, LayerMask.GetMask(new string[]
					{
						"Default"
					})))
					{
						this.hitPosition = raycastHit2.point;
						this.hitPositionImpact = true;
					}
					else
					{
						this.hitPosition = transform.position + transform.forward * this.laserRange;
						this.hitPositionImpact = false;
					}
				}
				else
				{
					this.hitPositionTimer -= Time.deltaTime;
				}
				this.hitPositionSmooth = Vector3.Lerp(this.hitPositionSmooth, this.hitPosition, 20f * Time.deltaTime);
			}
			else
			{
				this.hitPositionSmooth = Vector3.MoveTowards(this.hitPositionSmooth, this.hitPosition, this.hitPositionClientDistance * Time.deltaTime * ((float)PhotonNetwork.SerializationRate * 0.8f));
			}
			if (this.hitPositionStartImpulse)
			{
				this.hitPositionSmooth = this.hitPosition;
				this.hitPositionStartImpulse = false;
			}
			Vector3 vector = this.laserRayTransform.position - this.hitPosition;
			Vector3 vector2 = this.laserRayTransform.position - this.hitPositionSmooth;
			if (vector.magnitude < vector2.magnitude)
			{
				vector2 = Vector3.ClampMagnitude(vector2, vector.magnitude);
				this.hitPositionSmooth = this.laserRayTransform.position - vector2;
			}
			this.laser.LaserActive(this.laserStartTransform.position, this.hitPositionSmooth, this.hitPositionImpact);
			return;
		}
		this.hitPositionTimer = 0f;
		this.hitPositionStartImpulse = true;
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0000E684 File Offset: 0x0000C884
	private void MoveFastLogic()
	{
		if (this.currentState != EnemyBeamer.State.Seek && this.moveFast)
		{
			this.moveFast = false;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("MoveFastRPC", RpcTarget.Others, new object[]
				{
					this.moveFast
				});
			}
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000E6D6 File Offset: 0x0000C8D6
	[PunRPC]
	private void UpdateStateRPC(EnemyBeamer.State _state)
	{
		this.currentState = _state;
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000E6E0 File Offset: 0x0000C8E0
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

	// Token: 0x0600016F RID: 367 RVA: 0x0000E744 File Offset: 0x0000C944
	[PunRPC]
	private void MeleeTriggerRPC()
	{
		this.anim.meleeImpulse = true;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000E752 File Offset: 0x0000C952
	[PunRPC]
	private void MoveFastRPC(bool _moveFast)
	{
		this.moveFast = _moveFast;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000E75C File Offset: 0x0000C95C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.aimVerticalTarget);
			stream.SendNext(this.hitPosition);
			stream.SendNext(this.hitPositionImpact);
			return;
		}
		this.aimVerticalTarget = (Quaternion)stream.ReceiveNext();
		this.hitPosition = (Vector3)stream.ReceiveNext();
		this.hitPositionImpact = (bool)stream.ReceiveNext();
		this.hitPositionClientDistance = Vector3.Distance(this.hitPositionSmooth, this.hitPosition);
	}

	// Token: 0x040002D0 RID: 720
	public EnemyBeamer.State currentState;

	// Token: 0x040002D1 RID: 721
	public float stateTimer;

	// Token: 0x040002D2 RID: 722
	private bool stateImpulse;

	// Token: 0x040002D3 RID: 723
	private float stateTicker;

	// Token: 0x040002D4 RID: 724
	internal Enemy enemy;

	// Token: 0x040002D5 RID: 725
	internal PhotonView photonView;

	// Token: 0x040002D6 RID: 726
	public EnemyBeamerAnim anim;

	// Token: 0x040002D7 RID: 727
	public Transform aimVerticalTransform;

	// Token: 0x040002D8 RID: 728
	public Transform hatTransform;

	// Token: 0x040002D9 RID: 729
	public Transform bottomTransform;

	// Token: 0x040002DA RID: 730
	public SemiLaser laser;

	// Token: 0x040002DB RID: 731
	public Transform laserStartTransform;

	// Token: 0x040002DC RID: 732
	public Transform laserRayTransform;

	// Token: 0x040002DD RID: 733
	private Quaternion aimVerticalTarget;

	// Token: 0x040002DE RID: 734
	private Quaternion aimHorizontalTarget;

	// Token: 0x040002DF RID: 735
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x040002E0 RID: 736
	private Quaternion horizontalRotationTarget = Quaternion.identity;

	// Token: 0x040002E1 RID: 737
	[Space]
	public AnimationCurve aimHorizontalCurve;

	// Token: 0x040002E2 RID: 738
	public float aimHorizontalSpread;

	// Token: 0x040002E3 RID: 739
	public float aimHorizontalSpeed;

	// Token: 0x040002E4 RID: 740
	private float aimHorizontalLerp;

	// Token: 0x040002E5 RID: 741
	private float aimHorizontalResult;

	// Token: 0x040002E6 RID: 742
	internal PlayerAvatar playerTarget;

	// Token: 0x040002E7 RID: 743
	private Vector3 agentDestination;

	// Token: 0x040002E8 RID: 744
	private Vector3 seekDestination;

	// Token: 0x040002E9 RID: 745
	private Vector3 meleeTarget;

	// Token: 0x040002EA RID: 746
	private bool meleePlayer;

	// Token: 0x040002EB RID: 747
	private float laserCooldown;

	// Token: 0x040002EC RID: 748
	private float laserRange = 10f;

	// Token: 0x040002ED RID: 749
	private Vector3 hitPosition;

	// Token: 0x040002EE RID: 750
	private Vector3 hitPositionSmooth;

	// Token: 0x040002EF RID: 751
	private float hitPositionClientDistance;

	// Token: 0x040002F0 RID: 752
	private bool hitPositionStartImpulse;

	// Token: 0x040002F1 RID: 753
	private bool hitPositionImpact;

	// Token: 0x040002F2 RID: 754
	private float hitPositionTimer;

	// Token: 0x040002F3 RID: 755
	public ParticleSystem particleDeathSmoke;

	// Token: 0x040002F4 RID: 756
	public ParticleSystem particleDeathBody;

	// Token: 0x040002F5 RID: 757
	public ParticleSystem particleDeathNose;

	// Token: 0x040002F6 RID: 758
	public ParticleSystem particleDeathHat;

	// Token: 0x040002F7 RID: 759
	public ParticleSystem particleBottomSmoke;

	// Token: 0x040002F8 RID: 760
	internal bool moveFast;

	// Token: 0x020002C9 RID: 713
	public enum State
	{
		// Token: 0x040023C6 RID: 9158
		Spawn,
		// Token: 0x040023C7 RID: 9159
		Idle,
		// Token: 0x040023C8 RID: 9160
		Roam,
		// Token: 0x040023C9 RID: 9161
		Investigate,
		// Token: 0x040023CA RID: 9162
		AttackStart,
		// Token: 0x040023CB RID: 9163
		Attack,
		// Token: 0x040023CC RID: 9164
		AttackEnd,
		// Token: 0x040023CD RID: 9165
		MeleeStart,
		// Token: 0x040023CE RID: 9166
		Melee,
		// Token: 0x040023CF RID: 9167
		Seek,
		// Token: 0x040023D0 RID: 9168
		Leave,
		// Token: 0x040023D1 RID: 9169
		Stun,
		// Token: 0x040023D2 RID: 9170
		Despawn
	}
}
