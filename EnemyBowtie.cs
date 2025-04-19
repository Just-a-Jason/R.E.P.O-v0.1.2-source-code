using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000042 RID: 66
public class EnemyBowtie : MonoBehaviour
{
	// Token: 0x06000190 RID: 400 RVA: 0x000107D8 File Offset: 0x0000E9D8
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x06000191 RID: 401 RVA: 0x000107F4 File Offset: 0x0000E9F4
	private void Update()
	{
		this.HurtColliderLeaveLogic();
		this.SpringLogic();
		this.PlayerEyesLogic();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.grabAggroTimer > 0f)
			{
				this.grabAggroTimer -= Time.deltaTime;
			}
			if (!LevelGenerator.Instance.Generated)
			{
				return;
			}
			this.HorizontalRotationLogic();
			this.VerticalRotationLogic();
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyBowtie.State.Stun);
			}
			else if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyBowtie.State.Despawn);
			}
			switch (this.currentState)
			{
			case EnemyBowtie.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyBowtie.State.Idle:
				this.StateIdle();
				return;
			case EnemyBowtie.State.Roam:
				this.StateRoam();
				return;
			case EnemyBowtie.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyBowtie.State.PlayerNotice:
				this.StatePlayerNotice();
				return;
			case EnemyBowtie.State.Yell:
				this.StateYell();
				return;
			case EnemyBowtie.State.YellEnd:
				this.StateYellEnd();
				return;
			case EnemyBowtie.State.Leave:
				this.StateLeave();
				return;
			case EnemyBowtie.State.Stun:
				this.StateStun();
				return;
			case EnemyBowtie.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00010904 File Offset: 0x0000EB04
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0001093C File Offset: 0x0000EB3C
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
			this.UpdateState(EnemyBowtie.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x000109E4 File Offset: 0x0000EBE4
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
			this.UpdateState(EnemyBowtie.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00010B74 File Offset: 0x0000ED74
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
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyBowtie.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Leave);
		}
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00010C50 File Offset: 0x0000EE50
	private void StatePlayerNotice()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		this.enemy.NavMeshAgent.ResetPath();
		this.enemy.NavMeshAgent.Stop(0.1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.NavMeshAgent.Stop(0f);
			this.UpdateState(EnemyBowtie.State.Yell);
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00010CEC File Offset: 0x0000EEEC
	private void StateYell()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBowtie.State.YellEnd);
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00010D60 File Offset: 0x0000EF60
	private void StateYellEnd()
	{
		if (this.stateImpulse)
		{
			this.attacks++;
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.Jump.SurfaceJumpDisable(0.5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			if (this.attacks >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				this.attacks = 0;
				this.UpdateState(EnemyBowtie.State.Leave);
				return;
			}
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00010E00 File Offset: 0x0000F000
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
			this.stateTimer = 5f;
			this.stateImpulse = false;
			return;
		}
		this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00010F21 File Offset: 0x0000F121
	private void StateStun()
	{
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00010F37 File Offset: 0x0000F137
	private void StateDespawn()
	{
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00010F39 File Offset: 0x0000F139
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyBowtie.State.Idle);
		}
		if (this.anim.isActiveAndEnabled)
		{
			this.anim.OnSpawn();
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00010F70 File Offset: 0x0000F170
	public void OnHurt()
	{
		this.anim.GroanPause();
		this.anim.StunPause();
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
		if (this.currentState == EnemyBowtie.State.Yell)
		{
			this.UpdateState(EnemyBowtie.State.YellEnd);
		}
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00010FD8 File Offset: 0x0000F1D8
	public void OnDeath()
	{
		this.anim.GroanPause();
		this.anim.StunPause();
		this.anim.deathSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.anim.particleImpact.Play();
		this.anim.particleBits.Play();
		this.anim.particleEyes.Play();
		this.anim.particleDirectionalBits.transform.rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.anim.particleDirectionalBits.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00011118 File Offset: 0x0000F318
	public void OnVisionTriggered()
	{
		if (this.currentState != EnemyBowtie.State.Idle && this.currentState != EnemyBowtie.State.Roam && this.currentState != EnemyBowtie.State.Investigate)
		{
			return;
		}
		if (this.enemy.Jump.jumping)
		{
			return;
		}
		if (this.enemy.IsStunned())
		{
			return;
		}
		PlayerAvatar onVisionTriggeredPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
		if (Mathf.Abs(onVisionTriggeredPlayer.transform.position.y - this.enemy.transform.position.y) > 4f)
		{
			return;
		}
		this.playerTarget = onVisionTriggeredPlayer;
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
		this.UpdateState(EnemyBowtie.State.PlayerNotice);
		this.VerticalAimSet(100f);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00011212 File Offset: 0x0000F412
	public void OnInvestigate()
	{
		if (this.currentState == EnemyBowtie.State.Roam || this.currentState == EnemyBowtie.State.Idle || this.currentState == EnemyBowtie.State.Investigate)
		{
			this.UpdateState(EnemyBowtie.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0001124C File Offset: 0x0000F44C
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyBowtie.State.Leave)
			{
				this.grabAggroTimer = 60f;
				PlayerAvatar onGrabbedPlayerAvatar = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
				if (onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y > 1.15f || onGrabbedPlayerAvatar.transform.position.y - this.enemy.transform.position.y < -1f)
				{
					return;
				}
				this.playerTarget = onGrabbedPlayerAvatar;
				if (!this.enemy.IsStunned())
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("NoticeRPC", RpcTarget.All, new object[]
						{
							this.playerTarget.photonView.ViewID
						});
					}
					else
					{
						this.anim.NoticeSet(this.playerTarget.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyBowtie.State.PlayerNotice);
			}
		}
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00011364 File Offset: 0x0000F564
	private void UpdateState(EnemyBowtie.State _state)
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
		}
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x000113C0 File Offset: 0x0000F5C0
	private void HorizontalRotationLogic()
	{
		if (this.currentState == EnemyBowtie.State.Roam || this.currentState == EnemyBowtie.State.Investigate || this.currentState == EnemyBowtie.State.Leave)
		{
			if (this.enemy.NavMeshAgent.AgentVelocity.magnitude > 0.05f)
			{
				Quaternion quaternion = Quaternion.Euler(0f, Quaternion.LookRotation(this.enemy.NavMeshAgent.AgentVelocity.normalized).eulerAngles.y, 0f);
				this.horizontalRotationTarget = quaternion;
			}
		}
		else if (this.currentState == EnemyBowtie.State.PlayerNotice)
		{
			Quaternion quaternion2 = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.horizontalRotationTarget = quaternion2;
		}
		else if (this.currentState == EnemyBowtie.State.Yell)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			this.horizontalRotationTarget = Quaternion.Slerp(this.horizontalRotationTarget, b, 1f * Time.deltaTime);
		}
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.horizontalRotationSpring, this.horizontalRotationTarget, -1f);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00011548 File Offset: 0x0000F748
	private void VerticalRotationLogic()
	{
		if (this.currentState == EnemyBowtie.State.Yell)
		{
			this.VerticalAimSet(1f);
			this.verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalRotationSpring, this.verticalRotationTarget, -1f);
			return;
		}
		this.verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(this.verticalRotationSpring, Quaternion.identity, -1f);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x000115AC File Offset: 0x0000F7AC
	private void VerticalAimSet(float _lerp)
	{
		this.verticalRotationTransform.LookAt(this.playerTarget.transform);
		float num = 45f;
		float num2 = this.verticalRotationTransform.localEulerAngles.x;
		if (num2 < 180f)
		{
			num2 = Mathf.Clamp(num2, 0f, num);
		}
		else
		{
			num2 = Mathf.Clamp(num2, 360f - num, 360f);
		}
		this.verticalRotationTransform.localRotation = Quaternion.Euler(num2, 0f, 0f);
		Quaternion localRotation = this.verticalRotationTransform.localRotation;
		this.verticalRotationTransform.localRotation = Quaternion.identity;
		this.verticalRotationTarget = Quaternion.Lerp(this.verticalRotationTarget, localRotation, _lerp * Time.deltaTime);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00011660 File Offset: 0x0000F860
	private void HurtColliderLeaveLogic()
	{
		if (!this.enemy.Jump.jumping && this.currentState == EnemyBowtie.State.Leave)
		{
			this.hurtColliderLeave.gameObject.SetActive(true);
			return;
		}
		this.hurtColliderLeave.gameObject.SetActive(false);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x000116A0 File Offset: 0x0000F8A0
	private void SpringLogic()
	{
		this.headTransform.rotation = SemiFunc.SpringQuaternionGet(this.headSpring, this.HeadTargetTransform.rotation, -1f);
		this.eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeRightSpring, this.eyeRightTargetTransform.rotation, -1f);
		this.eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.eyeLeftSpring, this.eyeLeftTargetTransform.rotation, -1f);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x00011720 File Offset: 0x0000F920
	private void PlayerEyesLogic()
	{
		if (this.currentState == EnemyBowtie.State.PlayerNotice || this.currentState == EnemyBowtie.State.Yell || this.currentState == EnemyBowtie.State.YellEnd)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000117D4 File Offset: 0x0000F9D4
	[PunRPC]
	private void UpdateStateRPC(EnemyBowtie.State _state)
	{
		this.currentState = _state;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x000117DD File Offset: 0x0000F9DD
	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		this.anim.NoticeSet(_playerID);
	}

	// Token: 0x04000367 RID: 871
	private PhotonView photonView;

	// Token: 0x04000368 RID: 872
	public EnemyBowtie.State currentState;

	// Token: 0x04000369 RID: 873
	public float stateTimer;

	// Token: 0x0400036A RID: 874
	private bool stateImpulse;

	// Token: 0x0400036B RID: 875
	[Space]
	public EnemyBowtieAnim anim;

	// Token: 0x0400036C RID: 876
	public Transform hurtColliderLeave;

	// Token: 0x0400036D RID: 877
	[Space]
	public SpringQuaternion headSpring;

	// Token: 0x0400036E RID: 878
	public Transform headTransform;

	// Token: 0x0400036F RID: 879
	public Transform HeadTargetTransform;

	// Token: 0x04000370 RID: 880
	[Space]
	public SpringQuaternion eyeRightSpring;

	// Token: 0x04000371 RID: 881
	public Transform eyeRightTransform;

	// Token: 0x04000372 RID: 882
	public Transform eyeRightTargetTransform;

	// Token: 0x04000373 RID: 883
	[Space]
	public SpringQuaternion eyeLeftSpring;

	// Token: 0x04000374 RID: 884
	public Transform eyeLeftTransform;

	// Token: 0x04000375 RID: 885
	public Transform eyeLeftTargetTransform;

	// Token: 0x04000376 RID: 886
	[Space]
	public SpringQuaternion horizontalRotationSpring;

	// Token: 0x04000377 RID: 887
	private Quaternion horizontalRotationTarget;

	// Token: 0x04000378 RID: 888
	[Space]
	public Transform verticalRotationTransform;

	// Token: 0x04000379 RID: 889
	public SpringQuaternion verticalRotationSpring;

	// Token: 0x0400037A RID: 890
	private Quaternion verticalRotationTarget;

	// Token: 0x0400037B RID: 891
	private float roamWaitTimer;

	// Token: 0x0400037C RID: 892
	private Vector3 agentDestination;

	// Token: 0x0400037D RID: 893
	internal Enemy enemy;

	// Token: 0x0400037E RID: 894
	private PlayerAvatar playerTarget;

	// Token: 0x0400037F RID: 895
	private float grabAggroTimer;

	// Token: 0x04000380 RID: 896
	private int attacks;

	// Token: 0x020002CC RID: 716
	public enum State
	{
		// Token: 0x040023DB RID: 9179
		Spawn,
		// Token: 0x040023DC RID: 9180
		Idle,
		// Token: 0x040023DD RID: 9181
		Roam,
		// Token: 0x040023DE RID: 9182
		Investigate,
		// Token: 0x040023DF RID: 9183
		PlayerNotice,
		// Token: 0x040023E0 RID: 9184
		Yell,
		// Token: 0x040023E1 RID: 9185
		YellEnd,
		// Token: 0x040023E2 RID: 9186
		Leave,
		// Token: 0x040023E3 RID: 9187
		Stun,
		// Token: 0x040023E4 RID: 9188
		Despawn
	}
}
