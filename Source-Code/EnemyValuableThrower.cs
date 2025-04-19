using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000085 RID: 133
public class EnemyValuableThrower : MonoBehaviour
{
	// Token: 0x06000541 RID: 1345 RVA: 0x00034015 File Offset: 0x00032215
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x00034030 File Offset: 0x00032230
	private void Update()
	{
		if (this.currentState == EnemyValuableThrower.State.GetValuable || this.currentState == EnemyValuableThrower.State.GoToTarget || this.currentState == EnemyValuableThrower.State.PickUpTarget || this.currentState == EnemyValuableThrower.State.TargetPlayer || this.currentState == EnemyValuableThrower.State.Throw)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			if (this.enemy.IsStunned() || !LevelGenerator.Instance.Generated)
			{
				return;
			}
			switch (this.currentState)
			{
			case EnemyValuableThrower.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyValuableThrower.State.Idle:
				this.StateIdle();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.Roam:
				this.StateRoam();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.Investigate:
				this.StateInvestigate();
				this.AgentVelocityRotation();
				break;
			case EnemyValuableThrower.State.PlayerNotice:
				this.StatePlayerNotice();
				this.PlayerLookAt();
				break;
			case EnemyValuableThrower.State.GetValuable:
				this.StateGetValuable();
				break;
			case EnemyValuableThrower.State.GoToTarget:
				this.ValuableFailsafe();
				this.TargetFailsafe();
				this.AgentVelocityRotation();
				this.StateGoToTarget();
				break;
			case EnemyValuableThrower.State.PickUpTarget:
				this.DropOnStun();
				this.TargetFailsafe();
				this.ValuableTargetFollow();
				this.StatePickUpTarget();
				break;
			case EnemyValuableThrower.State.TargetPlayer:
				this.DropOnStun();
				this.TargetFailsafe();
				this.PlayerLookAt();
				this.ValuableTargetFollow();
				this.StateTargetPlayer();
				break;
			case EnemyValuableThrower.State.Throw:
				this.DropOnStun();
				this.TargetFailsafe();
				this.PlayerLookAt();
				this.ValuableTargetFollow();
				this.StateThrow();
				break;
			case EnemyValuableThrower.State.Leave:
				this.AgentVelocityRotation();
				this.StateLeave();
				break;
			}
			this.pickupTargetParent.position = this.enemy.Rigidbody.transform.position;
			Quaternion b = Quaternion.Euler(0f, this.enemy.Rigidbody.transform.rotation.eulerAngles.y, 0f);
			this.pickupTargetParent.rotation = Quaternion.Slerp(this.pickupTargetParent.rotation, b, 5f * Time.deltaTime);
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x000342D4 File Offset: 0x000324D4
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00034324 File Offset: 0x00032524
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = Random.Range(2f, 6f);
			this.stateImpulse = false;
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x000343D0 File Offset: 0x000325D0
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
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
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00034560 File Offset: 0x00032760
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
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if ((this.stateTimer <= 0f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.agentDestination) < 2f) && !this.enemy.Jump.jumping)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyValuableThrower.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00034658 File Offset: 0x00032858
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.5f;
			this.stateImpulse = false;
		}
		this.enemy.NavMeshAgent.ResetPath();
		this.enemy.NavMeshAgent.Stop(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x000346E0 File Offset: 0x000328E0
	private void StateGetValuable()
	{
		if (this.currentState != EnemyValuableThrower.State.GetValuable)
		{
			return;
		}
		this.valuableTarget = null;
		PhysGrabObject exists = null;
		PhysGrabObject exists2 = null;
		float num = 999f;
		float num2 = 999f;
		Collider[] array = Physics.OverlapSphere(this.playerTarget.transform.position, 10f, LayerMask.GetMask(new string[]
		{
			"PhysGrabObject"
		}));
		for (int i = 0; i < array.Length; i++)
		{
			ValuableObject componentInParent = array[i].GetComponentInParent<ValuableObject>();
			if (componentInParent && componentInParent.volumeType <= ValuableVolume.Type.Big)
			{
				float num3 = Vector3.Distance(this.playerTarget.transform.position, componentInParent.transform.position);
				NavMeshHit navMeshHit;
				if (NavMesh.SamplePosition(componentInParent.transform.position, out navMeshHit, 1f, -1))
				{
					if (num3 < num2)
					{
						num2 = num3;
						exists2 = componentInParent.physGrabObject;
					}
				}
				else if (num3 < num)
				{
					num = num3;
					exists = componentInParent.physGrabObject;
				}
			}
		}
		if (exists2)
		{
			this.valuableTarget = exists2;
		}
		else if (exists)
		{
			this.valuableTarget = exists;
		}
		if (!this.valuableTarget)
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
			return;
		}
		this.UpdateState(EnemyValuableThrower.State.GoToTarget);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00034814 File Offset: 0x00032A14
	private void StateGoToTarget()
	{
		if (this.enemy.IsStunned() || !this.valuableTarget)
		{
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.valuableTarget.transform.position);
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.stateImpulse = false;
			return;
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.valuableTarget.transform.position) < 1.25f)
		{
			this.enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyValuableThrower.State.PickUpTarget);
		}
		else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (this.stateTimer <= 0f)
			{
				this.enemy.Jump.StuckReset();
				this.UpdateState(EnemyValuableThrower.State.Leave);
			}
			else if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.valuableTarget.centerPoint) > 1.5f)
			{
				this.enemy.Jump.StuckTrigger(this.valuableTarget.transform.position - this.enemy.Rigidbody.transform.position);
				this.enemy.Rigidbody.DisableFollowPosition(1f, 10f);
			}
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyValuableThrower.State.Leave);
			}
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00034A28 File Offset: 0x00032C28
	private void StatePickUpTarget()
	{
		if (this.currentState != EnemyValuableThrower.State.PickUpTarget)
		{
			return;
		}
		if (this.stateImpulse)
		{
			foreach (PhysGrabber physGrabber in this.valuableTarget.playerGrabbing.ToList<PhysGrabber>())
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObject(0.1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						0.1f
					});
				}
			}
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			Quaternion to = Quaternion.Euler(0f, Quaternion.LookRotation(this.pickUpPosition - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
			this.pickUpPosition = this.valuableTarget.midPoint;
			this.stateTimer = 999f;
			this.stateImpulse = false;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.TargetPlayer);
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00034BAC File Offset: 0x00032DAC
	private void StateTargetPlayer()
	{
		if (this.currentState != EnemyValuableThrower.State.TargetPlayer)
		{
			return;
		}
		if (this.stateImpulse)
		{
			this.stateTimer = 10f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 direction = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position;
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(this.enemy.Rigidbody.transform.position, direction, out raycastHit, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct());
		if (flag && (raycastHit.transform.CompareTag("Player") || raycastHit.transform.GetComponent<PlayerTumble>()))
		{
			flag = false;
		}
		if (!flag && Vector3.Distance(base.transform.position, this.playerTarget.transform.position) < 3f)
		{
			this.enemy.NavMeshAgent.SetDestination(base.transform.position - base.transform.forward * 3f);
		}
		else if (flag || Vector3.Distance(base.transform.position, this.playerTarget.transform.position) > 5f)
		{
			this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
		}
		else
		{
			this.enemy.NavMeshAgent.ResetPath();
			if (this.stateTimer <= 8f)
			{
				this.UpdateState(EnemyValuableThrower.State.Throw);
			}
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyValuableThrower.State.Throw);
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00034D60 File Offset: 0x00032F60
	private void StateThrow()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.attacks++;
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		if (!this.valuableTarget)
		{
			this.stateTimer = Mathf.Clamp(this.stateTimer, this.stateTimer, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attacks = 0;
				this.UpdateState(EnemyValuableThrower.State.Leave);
				return;
			}
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00034E24 File Offset: 0x00033024
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 30f, 60f, false);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			}
			if (levelPoint)
			{
				this.agentDestination = levelPoint.transform.position;
			}
			else
			{
				this.enemy.EnemyParent.SpawnedTimerSet(0f);
			}
			this.stateTimer = 10f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyValuableThrower.State.Idle);
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00034F31 File Offset: 0x00033131
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyValuableThrower.State.Spawn);
		}
		if (this.anim.isActiveAndEnabled)
		{
			this.anim.OnSpawn();
		}
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00034F66 File Offset: 0x00033166
	public void OnHurt()
	{
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x00034FA0 File Offset: 0x000331A0
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.anim.particleImpact.Play();
		this.anim.particleBits.Play();
		this.anim.particleDirectionalBits.transform.rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.anim.particleDirectionalBits.Play();
		this.anim.deathSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x000350B4 File Offset: 0x000332B4
	public void OnVisionTriggered()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.currentState != EnemyValuableThrower.State.Idle && this.currentState != EnemyValuableThrower.State.Roam && this.currentState != EnemyValuableThrower.State.Investigate)
			{
				return;
			}
			if (this.playerTarget != this.enemy.Vision.onVisionTriggeredPlayer)
			{
				this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
				if (GameManager.Multiplayer())
				{
					this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.Others, new object[]
					{
						this.playerTarget.photonView.ViewID
					});
				}
			}
			if (!this.enemy.IsStunned())
			{
				if (GameManager.Multiplayer())
				{
					this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
					{
						this.enemy.Vision.onVisionTriggeredID
					});
				}
				else
				{
					this.anim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
				}
			}
			this.UpdateState(EnemyValuableThrower.State.PlayerNotice);
		}
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x000351B7 File Offset: 0x000333B7
	public void OnInvestigate()
	{
		if (this.currentState == EnemyValuableThrower.State.Roam || this.currentState == EnemyValuableThrower.State.Idle || this.currentState == EnemyValuableThrower.State.Investigate)
		{
			this.UpdateState(EnemyValuableThrower.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x000351F4 File Offset: 0x000333F4
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyValuableThrower.State.Leave)
			{
				this.grabAggroTimer = 60f;
				if (this.playerTarget != this.enemy.Rigidbody.onGrabbedPlayerAvatar)
				{
					this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.Others, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
					}
				}
				this.UpdateState(EnemyValuableThrower.State.PlayerNotice);
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
						return;
					}
					this.anim.NoticeSet(this.playerTarget.photonView.ViewID);
				}
			}
		}
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00035300 File Offset: 0x00033500
	private void UpdateState(EnemyValuableThrower.State _state)
	{
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				this.currentState
			});
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00035354 File Offset: 0x00033554
	private void ValuableTargetFollow()
	{
		if (!this.valuableTarget)
		{
			return;
		}
		if (Vector3.Distance(this.valuableTarget.transform.position, this.pickupTarget.position) > 2f)
		{
			this.valuableTarget = null;
			this.UpdateState(EnemyValuableThrower.State.Leave);
			return;
		}
		Vector3 midPoint = this.valuableTarget.midPoint;
		midPoint.y = this.valuableTarget.transform.position.y;
		Vector3 position = this.pickupTarget.position;
		this.valuableTarget.OverrideZeroGravity(0.1f);
		this.valuableTarget.OverrideMass(0.5f, 0.1f);
		this.valuableTarget.OverrideIndestructible(0.1f);
		this.valuableTarget.OverrideBreakEffects(0.1f);
		if (Mathf.Abs(midPoint.y - position.y) > 0.25f)
		{
			Vector3 vector = this.enemy.Rigidbody.transform.position + this.enemy.Rigidbody.transform.forward;
			position = new Vector3(vector.x, position.y, vector.z);
		}
		Vector3 a = SemiFunc.PhysFollowPosition(midPoint, position, this.valuableTarget.rb.velocity, 5f);
		this.valuableTarget.rb.AddForce(a * (5f * Time.fixedDeltaTime), ForceMode.Impulse);
		Vector3 a2 = SemiFunc.PhysFollowRotation(this.valuableTarget.transform, this.pickupTarget.rotation, this.valuableTarget.rb, 0.5f);
		this.valuableTarget.rb.AddTorque(a2 * (5f * Time.fixedDeltaTime), ForceMode.Impulse);
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00035514 File Offset: 0x00033714
	private void AgentVelocityRotation()
	{
		if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.05f)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 5f * Time.deltaTime);
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0003559C File Offset: 0x0003379C
	private void PlayerLookAt()
	{
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x00035621 File Offset: 0x00033821
	private void ValuableFailsafe()
	{
		if (!this.valuableTarget)
		{
			this.UpdateState(EnemyValuableThrower.State.GetValuable);
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00035637 File Offset: 0x00033837
	private void TargetFailsafe()
	{
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyValuableThrower.State.Leave);
		}
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0003565B File Offset: 0x0003385B
	private void DropOnStun()
	{
		if (this.enemy.IsStunned())
		{
			this.UpdateState(EnemyValuableThrower.State.GoToTarget);
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00035671 File Offset: 0x00033871
	public void ResetStateTimer()
	{
		this.stateTimer = 0f;
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00035680 File Offset: 0x00033880
	public void Throw()
	{
		if (!this.valuableTarget)
		{
			return;
		}
		if (!this.playerTarget)
		{
			return;
		}
		foreach (PhysGrabber physGrabber in this.valuableTarget.playerGrabbing.ToList<PhysGrabber>())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
		}
		Vector3 vector = this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.valuableTarget.centerPoint;
		vector = Vector3.Lerp(base.transform.forward, vector, 0.5f);
		this.valuableTarget.ResetMass();
		float num = 20f * this.valuableTarget.rb.mass;
		num = Mathf.Min(num, 100f);
		this.valuableTarget.ResetIndestructible();
		this.valuableTarget.rb.AddForce(vector * num, ForceMode.Impulse);
		this.valuableTarget.rb.AddTorque(this.valuableTarget.transform.right * 0.5f, ForceMode.Impulse);
		this.valuableTarget.impactDetector.PlayerHurtMultiplier(5f, 2f);
		this.valuableTarget = null;
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x00035810 File Offset: 0x00033A10
	[PunRPC]
	private void UpdateStateRPC(EnemyValuableThrower.State _state)
	{
		this.currentState = _state;
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00035819 File Offset: 0x00033A19
	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		this.anim.NoticeSet(_playerID);
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00035828 File Offset: 0x00033A28
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

	// Token: 0x04000858 RID: 2136
	private PhotonView photonView;

	// Token: 0x04000859 RID: 2137
	public EnemyValuableThrower.State currentState;

	// Token: 0x0400085A RID: 2138
	private bool stateImpulse;

	// Token: 0x0400085B RID: 2139
	public float stateTimer;

	// Token: 0x0400085C RID: 2140
	private Vector3 agentDestination;

	// Token: 0x0400085D RID: 2141
	private int attacks;

	// Token: 0x0400085E RID: 2142
	[Space]
	public EnemyValuableThrowerAnim anim;

	// Token: 0x0400085F RID: 2143
	public Transform pickupTargetParent;

	// Token: 0x04000860 RID: 2144
	public Transform pickupTarget;

	// Token: 0x04000861 RID: 2145
	private Enemy enemy;

	// Token: 0x04000862 RID: 2146
	private PlayerAvatar playerTarget;

	// Token: 0x04000863 RID: 2147
	private PhysGrabObject valuableTarget;

	// Token: 0x04000864 RID: 2148
	private Vector3 pickUpPosition;

	// Token: 0x04000865 RID: 2149
	private float grabAggroTimer;

	// Token: 0x020002E5 RID: 741
	public enum State
	{
		// Token: 0x040024E1 RID: 9441
		Spawn,
		// Token: 0x040024E2 RID: 9442
		Idle,
		// Token: 0x040024E3 RID: 9443
		Roam,
		// Token: 0x040024E4 RID: 9444
		Investigate,
		// Token: 0x040024E5 RID: 9445
		PlayerNotice,
		// Token: 0x040024E6 RID: 9446
		GetValuable,
		// Token: 0x040024E7 RID: 9447
		GoToTarget,
		// Token: 0x040024E8 RID: 9448
		PickUpTarget,
		// Token: 0x040024E9 RID: 9449
		TargetPlayer,
		// Token: 0x040024EA RID: 9450
		Throw,
		// Token: 0x040024EB RID: 9451
		Leave
	}
}
