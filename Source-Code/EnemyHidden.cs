using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000067 RID: 103
public class EnemyHidden : MonoBehaviour
{
	// Token: 0x06000326 RID: 806 RVA: 0x0001ECE6 File Offset: 0x0001CEE6
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.playerPickupPositionOriginal = this.playerPickupTransform.localPosition;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0001ED08 File Offset: 0x0001CF08
	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			this.RotationLogic();
			this.PlayerPickupTransformLogic();
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyHidden.State.Stun);
			}
			if (this.enemy.CurrentState == EnemyState.Despawn && !this.enemy.IsStunned())
			{
				this.UpdateState(EnemyHidden.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyHidden.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyHidden.State.Idle:
				this.StateIdle();
				return;
			case EnemyHidden.State.Roam:
				this.StateRoam();
				return;
			case EnemyHidden.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyHidden.State.PlayerNotice:
				this.StatePlayerNotice();
				return;
			case EnemyHidden.State.PlayerGoTo:
				this.StatePlayerGoTo();
				return;
			case EnemyHidden.State.PlayerPickup:
				this.StatePlayerPickup();
				return;
			case EnemyHidden.State.PlayerMove:
				this.StatePlayerMove();
				return;
			case EnemyHidden.State.PlayerRelease:
				this.StatePlayerRelease();
				return;
			case EnemyHidden.State.PlayerReleaseWait:
				this.StatePlayerReleaseWait();
				return;
			case EnemyHidden.State.Leave:
				this.StateLeave();
				return;
			case EnemyHidden.State.Stun:
				this.StateStun();
				return;
			case EnemyHidden.State.StunEnd:
				this.StateStunEnd();
				return;
			case EnemyHidden.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0001EE2A File Offset: 0x0001D02A
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.PlayerTumbleLogic();
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0001EE3C File Offset: 0x0001D03C
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
			this.UpdateState(EnemyHidden.State.Idle);
		}
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0001EE8C File Offset: 0x0001D08C
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
			this.UpdateState(EnemyHidden.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHidden.State.Leave);
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0001EF38 File Offset: 0x0001D138
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
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyHidden.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHidden.State.Leave);
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
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
			if (this.stateTimer <= 0f || !this.enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyHidden.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyHidden.State.Leave);
		}
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0001F184 File Offset: 0x0001D384
	private void StatePlayerNotice()
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
			this.UpdateState(EnemyHidden.State.PlayerGoTo);
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0001F208 File Offset: 0x0001D408
	private void StatePlayerGoTo()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
			this.agentSet = true;
		}
		this.stateTimer -= Time.deltaTime;
		if (!this.playerTarget || this.playerTarget.isDisabled || this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHidden.State.Leave);
			return;
		}
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.enemy.Jump.jumping)
		{
			this.enemy.NavMeshAgent.Disable(0.5f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.playerTarget.transform.position, 5f * Time.deltaTime);
			this.agentSet = true;
		}
		else if (!this.enemy.NavMeshAgent.IsDisabled())
		{
			if (!this.agentSet && this.enemy.NavMeshAgent.HasPath() && Vector3.Distance(this.enemy.Rigidbody.transform.position + Vector3.down * 0.75f, this.enemy.NavMeshAgent.GetDestination()) < 0.25f)
			{
				this.enemy.Jump.StuckTrigger(this.enemy.Rigidbody.transform.position - this.playerTarget.transform.position);
			}
			this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
			this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
			this.agentSet = false;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) < 1.5f)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyHidden.State.PlayerPickup);
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0001F428 File Offset: 0x0001D628
	private void StatePlayerPickup()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyHidden.State.Leave);
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHidden.State.PlayerMove);
		}
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0001F498 File Offset: 0x0001D698
	private void StatePlayerMove()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 5f;
			this.maxMoveTimer = 15f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 50f, 999f, false);
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
				this.stateTimer = 0f;
			}
			this.stateImpulse = false;
		}
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyHidden.State.Leave);
			return;
		}
		SemiFunc.EnemyCartJump(this.enemy);
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		this.enemy.Jump.GapJumpOverride(0.1f, 20f, 20f);
		this.maxMoveTimer -= Time.deltaTime;
		if (!this.enemy.NavMeshAgent.HasPath() || Vector3.Distance(base.transform.position, this.agentDestination) < 1f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) > 5f || this.stateTimer <= 0f || this.maxMoveTimer <= 0f)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyHidden.State.PlayerRelease);
		}
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0001F6D0 File Offset: 0x0001D8D0
	private void StatePlayerRelease()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			this.UpdateState(EnemyHidden.State.Leave);
			return;
		}
		if (this.stateTimer <= 0f || Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) > 5f)
		{
			this.UpdateState(EnemyHidden.State.PlayerReleaseWait);
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0001F774 File Offset: 0x0001D974
	private void StatePlayerReleaseWait()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHidden.State.Leave);
		}
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0001F7C4 File Offset: 0x0001D9C4
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
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		SemiFunc.EnemyCartJump(this.enemy);
		if (Vector3.Distance(base.transform.position, this.agentDestination) < 1f || this.stateTimer <= 0f)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyHidden.State.Idle);
		}
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0001F943 File Offset: 0x0001DB43
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyHidden.State.StunEnd);
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0001F95C File Offset: 0x0001DB5C
	private void StateStunEnd()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyHidden.State.Idle);
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0001F9A9 File Offset: 0x0001DBA9
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.enemy.EnemyParent.Despawn();
			this.UpdateState(EnemyHidden.State.Spawn);
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0001F9D1 File Offset: 0x0001DBD1
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyHidden.State.Spawn);
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0001F9EE File Offset: 0x0001DBEE
	public void OnHurt()
	{
		this.enemyHiddenAnim.Hurt();
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0001F9FB File Offset: 0x0001DBFB
	public void OnDeath()
	{
		this.enemyHiddenAnim.Death();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0001FA20 File Offset: 0x0001DC20
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.currentState == EnemyHidden.State.Idle || this.currentState == EnemyHidden.State.Roam || this.currentState == EnemyHidden.State.Investigate))
		{
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
			this.UpdateState(EnemyHidden.State.Investigate);
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0001FA6C File Offset: 0x0001DC6C
	public void OnVision()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentState == EnemyHidden.State.Idle || this.currentState == EnemyHidden.State.Roam || this.currentState == EnemyHidden.State.Investigate)
		{
			this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					this.playerTarget.photonView.ViewID
				});
			}
			this.UpdateState(EnemyHidden.State.PlayerNotice);
			return;
		}
		if (this.currentState == EnemyHidden.State.PlayerGoTo)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0001FB04 File Offset: 0x0001DD04
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
		if (this.currentState == EnemyHidden.State.Leave)
		{
			this.grabAggroTimer = 60f;
			this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdatePlayerTargetRPC", RpcTarget.All, new object[]
				{
					this.playerTarget.photonView.ViewID
				});
			}
			this.UpdateState(EnemyHidden.State.PlayerNotice);
		}
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0001FB90 File Offset: 0x0001DD90
	private void UpdateState(EnemyHidden.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.enemy.Rigidbody.StuckReset();
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

	// Token: 0x0600033E RID: 830 RVA: 0x0001FC0C File Offset: 0x0001DE0C
	private void RotationLogic()
	{
		if (this.currentState == EnemyHidden.State.PlayerNotice || this.currentState == EnemyHidden.State.PlayerGoTo)
		{
			if (Vector3.Distance(this.playerTarget.transform.position, base.transform.position) > 0.1f)
			{
				this.rotationTarget = Quaternion.LookRotation(this.playerTarget.transform.position - base.transform.position);
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

	// Token: 0x0600033F RID: 831 RVA: 0x0001FD40 File Offset: 0x0001DF40
	private void PlayerTumbleLogic()
	{
		if (this.currentState != EnemyHidden.State.PlayerPickup && this.currentState != EnemyHidden.State.PlayerMove && this.currentState != EnemyHidden.State.PlayerRelease)
		{
			return;
		}
		if (!this.playerTarget || this.playerTarget.isDisabled)
		{
			return;
		}
		if (!this.playerTarget.tumble.isTumbling)
		{
			this.playerTarget.tumble.TumbleRequest(true, false);
		}
		this.playerTarget.tumble.TumbleOverrideTime(3f);
		this.playerTarget.FallDamageResetSet(0.1f);
		this.playerTarget.tumble.physGrabObject.OverrideMass(1f, 0.1f);
		this.playerTarget.tumble.physGrabObject.OverrideAngularDrag(2f, 0.1f);
		this.playerTarget.tumble.physGrabObject.OverrideDrag(1f, 0.1f);
		this.playerTarget.tumble.OverrideEnemyHurt(0.1f);
		float num = 1f;
		if (this.playerTarget.tumble.physGrabObject.playerGrabbing.Count > 0)
		{
			num = 0.5f;
		}
		else if (this.currentState == EnemyHidden.State.PlayerRelease || this.currentState == EnemyHidden.State.PlayerPickup)
		{
			num = 0.75f;
		}
		Vector3 a = SemiFunc.PhysFollowPosition(this.playerTarget.tumble.transform.position, this.playerPickupTransform.position, this.playerTarget.tumble.rb.velocity, 10f * num);
		this.playerTarget.tumble.rb.AddForce(a * (10f * Time.fixedDeltaTime * num), ForceMode.Impulse);
		Vector3 a2 = SemiFunc.PhysFollowRotation(this.playerTarget.tumble.transform, this.playerPickupTransform.rotation, this.playerTarget.tumble.rb, 0.2f * num);
		this.playerTarget.tumble.rb.AddTorque(a2 * (1f * Time.fixedDeltaTime * num), ForceMode.Impulse);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0001FF50 File Offset: 0x0001E150
	private void PlayerPickupTransformLogic()
	{
		if (this.currentState == EnemyHidden.State.PlayerMove || this.currentState == EnemyHidden.State.PlayerPickup || this.currentState == EnemyHidden.State.PlayerRelease)
		{
			float num = (this.enemy.Rigidbody.velocity.magnitude + this.enemy.Rigidbody.rb.angularVelocity.magnitude) * 0.5f;
			num = Mathf.Clamp(num, 0f, 1f);
			float num2 = this.playerPickupCurveUp.Evaluate(this.playerPickupLerpUp) - 0.5f;
			float num3 = this.playerPickupCurveSide.Evaluate(this.playerPickupLerpSide) - 0.5f;
			this.playerPickupLerpUp += 2f * Time.deltaTime * num;
			if (this.playerPickupLerpUp > 1f)
			{
				this.playerPickupLerpUp -= 1f;
			}
			this.playerPickupLerpSide += 1f * Time.deltaTime * num;
			if (this.playerPickupLerpSide > 1f)
			{
				this.playerPickupLerpSide -= 1f;
			}
			this.playerPickupTransform.localPosition = Vector3.Lerp(this.playerPickupTransform.localPosition, new Vector3(this.playerPickupPositionOriginal.x + num3 * 0.2f, this.playerPickupPositionOriginal.y + num2 * 0.2f, this.playerPickupPositionOriginal.z), 50f * Time.deltaTime);
			return;
		}
		this.playerPickupLerpSide = 0f;
		this.playerPickupLerpUp = 0f;
		this.playerPickupTransform.localPosition = Vector3.Lerp(this.playerPickupTransform.localPosition, this.playerPickupPositionOriginal, 10f * Time.deltaTime);
	}

	// Token: 0x06000341 RID: 833 RVA: 0x00020107 File Offset: 0x0001E307
	[PunRPC]
	private void UpdateStateRPC(EnemyHidden.State _state)
	{
		this.currentState = _state;
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00020110 File Offset: 0x0001E310
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

	// Token: 0x04000599 RID: 1433
	[Space]
	public EnemyHidden.State currentState;

	// Token: 0x0400059A RID: 1434
	private bool stateImpulse;

	// Token: 0x0400059B RID: 1435
	private float stateTimer;

	// Token: 0x0400059C RID: 1436
	[Space]
	public Enemy enemy;

	// Token: 0x0400059D RID: 1437
	public EnemyHiddenAnim enemyHiddenAnim;

	// Token: 0x0400059E RID: 1438
	private PhotonView photonView;

	// Token: 0x0400059F RID: 1439
	[Space]
	public Transform playerPickupTransform;

	// Token: 0x040005A0 RID: 1440
	public AnimationCurve playerPickupCurveUp;

	// Token: 0x040005A1 RID: 1441
	public AnimationCurve playerPickupCurveSide;

	// Token: 0x040005A2 RID: 1442
	private float playerPickupLerpUp;

	// Token: 0x040005A3 RID: 1443
	private float playerPickupLerpSide;

	// Token: 0x040005A4 RID: 1444
	private Vector3 playerPickupPositionOriginal;

	// Token: 0x040005A5 RID: 1445
	[Space]
	public SpringQuaternion rotationSpring;

	// Token: 0x040005A6 RID: 1446
	private Quaternion rotationTarget;

	// Token: 0x040005A7 RID: 1447
	private Vector3 agentDestination;

	// Token: 0x040005A8 RID: 1448
	private PlayerAvatar playerTarget;

	// Token: 0x040005A9 RID: 1449
	private bool agentSet;

	// Token: 0x040005AA RID: 1450
	private float grabAggroTimer;

	// Token: 0x040005AB RID: 1451
	private float maxMoveTimer;

	// Token: 0x020002D5 RID: 725
	public enum State
	{
		// Token: 0x04002434 RID: 9268
		Spawn,
		// Token: 0x04002435 RID: 9269
		Idle,
		// Token: 0x04002436 RID: 9270
		Roam,
		// Token: 0x04002437 RID: 9271
		Investigate,
		// Token: 0x04002438 RID: 9272
		PlayerNotice,
		// Token: 0x04002439 RID: 9273
		PlayerGoTo,
		// Token: 0x0400243A RID: 9274
		PlayerPickup,
		// Token: 0x0400243B RID: 9275
		PlayerMove,
		// Token: 0x0400243C RID: 9276
		PlayerRelease,
		// Token: 0x0400243D RID: 9277
		PlayerReleaseWait,
		// Token: 0x0400243E RID: 9278
		Leave,
		// Token: 0x0400243F RID: 9279
		Stun,
		// Token: 0x04002440 RID: 9280
		StunEnd,
		// Token: 0x04002441 RID: 9281
		Despawn
	}
}
