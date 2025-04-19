using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000081 RID: 129
public class EnemyTumbler : MonoBehaviour
{
	// Token: 0x060004E5 RID: 1253 RVA: 0x0003081E File Offset: 0x0002EA1E
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0003082C File Offset: 0x0002EA2C
	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.HopLogic();
			this.RotationLogic();
			if (this.visionTimer > 0f)
			{
				this.visionTimer -= Time.deltaTime;
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyTumbler.State.Stunned);
			}
			else if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyTumbler.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyTumbler.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyTumbler.State.Idle:
				this.StateIdle();
				return;
			case EnemyTumbler.State.Roam:
				this.StateRoam();
				return;
			case EnemyTumbler.State.Notice:
				this.StateNotice();
				return;
			case EnemyTumbler.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyTumbler.State.MoveToPlayer:
				this.StateMoveToPlayer();
				return;
			case EnemyTumbler.State.Tell:
				this.StateTell();
				return;
			case EnemyTumbler.State.Tumble:
				this.StateTumble();
				return;
			case EnemyTumbler.State.TumbleEnd:
				this.StateTumbleEnd();
				return;
			case EnemyTumbler.State.BackToNavmesh:
				this.StateBackToNavmesh();
				return;
			case EnemyTumbler.State.Leave:
				this.StateLeave();
				return;
			case EnemyTumbler.State.Stunned:
				this.StateStunned();
				return;
			case EnemyTumbler.State.Dead:
				this.StateDead();
				return;
			case EnemyTumbler.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00030944 File Offset: 0x0002EB44
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
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00030994 File Offset: 0x0002EB94
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
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
			this.UpdateState(EnemyTumbler.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00030A34 File Offset: 0x0002EC34
	private void StateRoam()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = Random.Range(3f, 8f);
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
			if (!this.enemy.Jump.jumping && (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f))
			{
				this.UpdateState(EnemyTumbler.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00030BD0 File Offset: 0x0002EDD0
	private void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.MoveToPlayer);
		}
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00030C54 File Offset: 0x0002EE54
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			if (!this.enemy.Jump.jumping)
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
				this.enemy.NavMeshAgent.ResetPath();
			}
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			this.stateTimer = 5f;
			this.enemy.Rigidbody.notMovingTimer = 0f;
			this.stateImpulse = false;
		}
		else
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			SemiFunc.EnemyCartJump(this.enemy);
			if (this.enemy.Rigidbody.notMovingTimer > 3f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (!this.enemy.Jump.jumping && (this.stateTimer <= 0f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f))
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyTumbler.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Leave);
		}
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00030DB8 File Offset: 0x0002EFB8
	private void StateMoveToPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.agentDestination = this.targetPlayer.transform.position;
		if (this.enemy.Grounded.grounded)
		{
			this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 position = this.targetPlayer.transform.position;
		position.y = this.enemy.Rigidbody.transform.position.y;
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, position) < 7f && !this.enemy.Jump.jumping && !this.VisionBlocked())
		{
			this.UpdateState(EnemyTumbler.State.Tell);
			return;
		}
		if (this.stateTimer <= 0f || this.enemy.Rigidbody.notMovingTimer > 3f)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00030ED0 File Offset: 0x0002F0D0
	private void StateTell()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.Tumble);
		}
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00030F54 File Offset: 0x0002F154
	private void StateTumble()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
			Vector3 normalized = Vector3.Lerp(this.targetPlayer.transform.position - this.enemy.Rigidbody.transform.position, Vector3.up, 0.6f).normalized;
			this.enemy.Rigidbody.rb.AddForce(normalized * 40f, ForceMode.Impulse);
			this.enemy.Rigidbody.rb.AddTorque(this.enemy.Rigidbody.transform.right * 8f, ForceMode.Impulse);
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.enemy.Rigidbody.DisableFollowPosition(0.2f, 10f);
		this.enemy.Rigidbody.DisableFollowRotation(0.2f, 10f);
		if (this.enemy.Rigidbody.rb.velocity.magnitude < 1f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		base.transform.position = this.enemy.Rigidbody.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.backToNavmeshPosition = navMeshHit.position;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.TumbleEnd);
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00031130 File Offset: 0x0002F330
	private void StateTumbleEnd()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.BackToNavmesh);
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x000311B4 File Offset: 0x0002F3B4
	private void StateBackToNavmesh()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			base.transform.position = this.enemy.Rigidbody.transform.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.enemy.NavMeshAgent.Warp(navMeshHit.position);
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00031248 File Offset: 0x0002F448
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
				this.agentDestination = navMeshHit.position;
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
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00031394 File Offset: 0x0002F594
	private void StateStunned()
	{
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 1f, -1))
		{
			this.backToNavmeshPosition = navMeshHit.position;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = this.enemy.Rigidbody.transform.position;
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyTumbler.State.BackToNavmesh);
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0003141C File Offset: 0x0002F61C
	private void StateDead()
	{
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00031420 File Offset: 0x0002F620
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.enemy.NavMeshAgent.ResetPath();
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00031471 File Offset: 0x0002F671
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyTumbler.State.Spawn);
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00031490 File Offset: 0x0002F690
	public void OnHurt()
	{
		this.enemyTumblerAnim.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyTumbler.State.Leave)
		{
			this.UpdateState(EnemyTumbler.State.Idle);
		}
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000314E8 File Offset: 0x0002F6E8
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00031568 File Offset: 0x0002F768
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyTumbler.State.Idle || this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyTumbler.State.Investigate);
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000315B4 File Offset: 0x0002F7B4
	public void OnVision()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Idle || this.currentState == EnemyTumbler.State.Investigate)
		{
			this.targetPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			this.UpdateState(EnemyTumbler.State.Notice);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
				return;
			}
		}
		else if (this.currentState == EnemyTumbler.State.MoveToPlayer && this.targetPlayer == this.enemy.Vision.onVisionTriggeredPlayer)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00031674 File Offset: 0x0002F874
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
		if (this.currentState == EnemyTumbler.State.Leave)
		{
			this.grabAggroTimer = 60f;
			this.targetPlayer = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
			this.UpdateState(EnemyTumbler.State.Notice);
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
				{
					this.targetPlayer.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x000316FF File Offset: 0x0002F8FF
	public void OnHurtColliderImpactAny()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("OnHurtColliderImpactAnyRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
			this.OnHurtColliderImpactAnyRPC();
		}
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0003172C File Offset: 0x0002F92C
	public void OnHurtColliderImpactPlayer()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.OnHurtColliderImpactPlayerRPC(this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID);
			return;
		}
		this.photonView.RPC("OnHurtColliderImpactPlayerRPC", RpcTarget.All, new object[]
		{
			this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID
		});
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00031790 File Offset: 0x0002F990
	private void UpdateState(EnemyTumbler.State _state)
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

	// Token: 0x060004FE RID: 1278 RVA: 0x00031810 File Offset: 0x0002FA10
	private void RotationLogic()
	{
		if ((this.currentState == EnemyTumbler.State.Notice || this.currentState == EnemyTumbler.State.MoveToPlayer || this.currentState == EnemyTumbler.State.Tell) && !this.VisionBlocked())
		{
			Quaternion quaternion = Quaternion.Euler(0f, Quaternion.LookRotation(this.targetPlayer.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.mainMeshTargetRotation = quaternion;
		}
		else
		{
			Vector3 agentVelocity = this.enemy.NavMeshAgent.AgentVelocity;
			agentVelocity.y = 0f;
			if (agentVelocity.magnitude > 1f)
			{
				this.mainMeshTargetRotation = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.Rigidbody.rb.velocity.normalized).eulerAngles.y, 0f);
			}
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.mainMeshSpring, this.mainMeshTargetRotation, -1f);
		this.headTargetCodeTransform.localEulerAngles = new Vector3(this.enemy.Rigidbody.rb.velocity.y * 5f, 0f, 0f);
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, this.headTargetTransform.rotation, -1f);
		this.hatTransform.rotation = SemiFunc.SpringQuaternionGet(this.hatSpring, this.hatTargetTransform.rotation, -1f);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000319B0 File Offset: 0x0002FBB0
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
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

	// Token: 0x06000500 RID: 1280 RVA: 0x00031A48 File Offset: 0x0002FC48
	private void HopLogic()
	{
		bool flag = this.currentState == EnemyTumbler.State.BackToNavmesh;
		if (this.currentState == EnemyTumbler.State.Roam || this.currentState == EnemyTumbler.State.Investigate || this.currentState == EnemyTumbler.State.MoveToPlayer || this.currentState == EnemyTumbler.State.Leave || flag)
		{
			float d = 1f;
			if (this.currentState == EnemyTumbler.State.MoveToPlayer)
			{
				d = 2f;
			}
			if (this.enemy.Grounded.grounded && !this.enemy.Jump.jumping)
			{
				this.enemy.NavMeshAgent.Stop(0.1f);
				if (this.groundedPrevious != this.enemy.Grounded.grounded)
				{
					if (flag)
					{
						base.transform.position = this.enemy.Rigidbody.transform.position;
					}
					else
					{
						this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
					}
				}
			}
			if (this.hopMoveTimer <= 0f)
			{
				Vector3 steeringTarget = this.enemy.NavMeshAgent.Agent.steeringTarget;
				Vector3 normalized = (this.enemy.NavMeshAgent.Agent.steeringTarget - this.enemy.Rigidbody.physGrabObject.centerPoint).normalized;
				steeringTarget.y = this.enemy.Rigidbody.physGrabObject.centerPoint.y;
				Vector3 normalized2 = (steeringTarget - this.enemy.Rigidbody.physGrabObject.centerPoint).normalized;
				bool flag2 = false;
				bool flag3 = false;
				int num = 10;
				float d2 = 0.5f;
				float maxDistance = 2f;
				if (!flag)
				{
					Vector3 vector = this.enemy.Rigidbody.physGrabObject.centerPoint + normalized2 * d2;
					bool flag4 = false;
					for (int i = 0; i < num; i++)
					{
						if (Physics.Raycast(vector, Vector3.down, maxDistance, SemiFunc.LayerMaskGetVisionObstruct()))
						{
							if (flag4)
							{
								flag2 = true;
							}
						}
						else
						{
							if (i < 3)
							{
								flag4 = true;
							}
							flag3 = true;
						}
						vector += normalized2 * d2;
					}
					this.enemy.NavMeshAgent.Stop(0f);
				}
				if (flag2)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 30f + normalized * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * 5f);
					this.hopMoveTimer = 2.25f;
				}
				else if (!flag && Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 1f)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.NavMeshAgent.GetPoint());
					this.hopMoveTimer = 0.75f;
				}
				else if (flag3)
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
					this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * 0.5f);
					this.hopMoveTimer = 0.75f;
				}
				else
				{
					this.enemy.Rigidbody.rb.AddForce(Vector3.up * 25f + normalized2 * 10f, ForceMode.Impulse);
					if (flag)
					{
						base.transform.position = Vector3.MoveTowards(base.transform.position, this.backToNavmeshPosition, 2f);
					}
					else
					{
						this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.physGrabObject.centerPoint + normalized * d);
					}
					this.hopMoveTimer = 1.25f;
				}
				this.enemy.Jump.JumpingSet(true);
				this.enemy.Rigidbody.WarpDisable(2f);
				this.enemy.Grounded.GroundedDisable(0.25f);
			}
			else
			{
				this.hopMoveTimer -= Time.deltaTime;
			}
		}
		this.groundedPrevious = this.enemy.Grounded.grounded;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00031F01 File Offset: 0x00030101
	[PunRPC]
	private void UpdateStateRPC(EnemyTumbler.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemyTumbler.State.Spawn)
		{
			this.enemyTumblerAnim.OnSpawn();
		}
		if (this.currentState == EnemyTumbler.State.Tumble)
		{
			this.enemyTumblerAnim.OnTumble();
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00031F34 File Offset: 0x00030134
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

	// Token: 0x06000503 RID: 1283 RVA: 0x00031F9C File Offset: 0x0003019C
	[PunRPC]
	private void OnHurtColliderImpactAnyRPC()
	{
		this.enemyTumblerAnim.SfxOnHurtColliderImpactAny();
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00031FAC File Offset: 0x000301AC
	[PunRPC]
	private void OnHurtColliderImpactPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				playerAvatar.tumble.OverrideEnemyHurt(3f);
				break;
			}
		}
	}

	// Token: 0x040007F1 RID: 2033
	public bool debugSpawn;

	// Token: 0x040007F2 RID: 2034
	public EnemyTumbler.State currentState;

	// Token: 0x040007F3 RID: 2035
	private bool stateImpulse;

	// Token: 0x040007F4 RID: 2036
	private float stateTimer;

	// Token: 0x040007F5 RID: 2037
	internal PlayerAvatar targetPlayer;

	// Token: 0x040007F6 RID: 2038
	public Enemy enemy;

	// Token: 0x040007F7 RID: 2039
	public EnemyTumblerAnim enemyTumblerAnim;

	// Token: 0x040007F8 RID: 2040
	private PhotonView photonView;

	// Token: 0x040007F9 RID: 2041
	public HurtCollider hurtCollider;

	// Token: 0x040007FA RID: 2042
	private float hurtColliderTimer;

	// Token: 0x040007FB RID: 2043
	private float roamWaitTimer;

	// Token: 0x040007FC RID: 2044
	private Vector3 roamPoint;

	// Token: 0x040007FD RID: 2045
	private Vector3 backToNavmeshPosition;

	// Token: 0x040007FE RID: 2046
	private Vector3 agentDestination;

	// Token: 0x040007FF RID: 2047
	private Quaternion lookDirection;

	// Token: 0x04000800 RID: 2048
	private float visionTimer;

	// Token: 0x04000801 RID: 2049
	private bool visionPrevious;

	// Token: 0x04000802 RID: 2050
	private bool groundedPrevious;

	// Token: 0x04000803 RID: 2051
	private float hopMoveTimer;

	// Token: 0x04000804 RID: 2052
	[Space]
	public SpringQuaternion headSpring;

	// Token: 0x04000805 RID: 2053
	public Transform headTransform;

	// Token: 0x04000806 RID: 2054
	public Transform headTargetTransform;

	// Token: 0x04000807 RID: 2055
	public Transform headTargetCodeTransform;

	// Token: 0x04000808 RID: 2056
	[Space]
	public SpringQuaternion hatSpring;

	// Token: 0x04000809 RID: 2057
	public Transform hatTransform;

	// Token: 0x0400080A RID: 2058
	public Transform hatTargetTransform;

	// Token: 0x0400080B RID: 2059
	[Space]
	public SpringQuaternion mainMeshSpring;

	// Token: 0x0400080C RID: 2060
	private Quaternion mainMeshTargetRotation;

	// Token: 0x0400080D RID: 2061
	private float grabAggroTimer;

	// Token: 0x020002E3 RID: 739
	public enum State
	{
		// Token: 0x040024C7 RID: 9415
		Spawn,
		// Token: 0x040024C8 RID: 9416
		Idle,
		// Token: 0x040024C9 RID: 9417
		Roam,
		// Token: 0x040024CA RID: 9418
		Notice,
		// Token: 0x040024CB RID: 9419
		Investigate,
		// Token: 0x040024CC RID: 9420
		MoveToPlayer,
		// Token: 0x040024CD RID: 9421
		Tell,
		// Token: 0x040024CE RID: 9422
		Tumble,
		// Token: 0x040024CF RID: 9423
		TumbleEnd,
		// Token: 0x040024D0 RID: 9424
		BackToNavmesh,
		// Token: 0x040024D1 RID: 9425
		Leave,
		// Token: 0x040024D2 RID: 9426
		Stunned,
		// Token: 0x040024D3 RID: 9427
		Dead,
		// Token: 0x040024D4 RID: 9428
		Despawn
	}
}
