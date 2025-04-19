using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000038 RID: 56
public class EnemyAnimal : MonoBehaviour
{
	// Token: 0x060000D9 RID: 217 RVA: 0x00008111 File Offset: 0x00006311
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000812C File Offset: 0x0000632C
	private void Update()
	{
		if (this.currentState == EnemyAnimal.State.PlayerNotice || this.currentState == EnemyAnimal.State.GoToPlayer || this.currentState == EnemyAnimal.State.WreakHavoc)
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
			switch (this.currentState)
			{
			case EnemyAnimal.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyAnimal.State.Idle:
				this.StateIdle();
				return;
			case EnemyAnimal.State.Roam:
				this.StateRoam();
				return;
			case EnemyAnimal.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyAnimal.State.PlayerNotice:
				this.StatePlayerNotice();
				this.PlayerLookAt();
				return;
			case EnemyAnimal.State.GoToPlayer:
				this.StateGoToPlayer();
				return;
			case EnemyAnimal.State.WreakHavoc:
				this.StateWreakHavoc();
				return;
			case EnemyAnimal.State.Leave:
				this.StateLeave();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x0000827C File Offset: 0x0000647C
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
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000082CC File Offset: 0x000064CC
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
			this.UpdateState(EnemyAnimal.State.Roam);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00008374 File Offset: 0x00006574
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
			this.UpdateState(EnemyAnimal.State.Idle);
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00008504 File Offset: 0x00006704
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
			this.enemy.NavMeshAgent.OverrideAgent(4f, 12f, 0.25f);
			if (this.enemy.Rigidbody.notMovingTimer > 2f)
			{
				this.stateTimer -= Time.deltaTime;
			}
			if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(this.enemy);
				this.UpdateState(EnemyAnimal.State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00008600 File Offset: 0x00006800
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
			this.UpdateState(EnemyAnimal.State.GoToPlayer);
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00008688 File Offset: 0x00006888
	private void StateGoToPlayer()
	{
		this.enemy.NavMeshAgent.SetDestination(this.playerTarget.transform.position);
		if (this.stateImpulse)
		{
			this.stateTimer = 10f;
			this.stateImpulse = false;
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) < 3f)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.UpdateState(EnemyAnimal.State.WreakHavoc);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 1f && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.playerTarget.transform.position) > 1.5f)
		{
			this.enemy.Jump.StuckTrigger(this.playerTarget.transform.position - this.enemy.Rigidbody.transform.position);
			this.enemy.Rigidbody.DisableFollowPosition(1f, 10f);
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00008818 File Offset: 0x00006A18
	private void StateWreakHavoc()
	{
		if (this.stateImpulse)
		{
			this.havocTimer = 0f;
			this.stateTimer = 20f;
			this.stateImpulse = false;
		}
		if (this.havocTimer <= 0f || Vector3.Distance(base.transform.position, this.enemy.NavMeshAgent.GetDestination()) < 0.25f)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointInTargetRoomGet(this.playerTarget.RoomVolumeCheck, 1f, 10f, this.ignorePoint);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointInTargetRoomGet(this.playerTarget.RoomVolumeCheck, 0f, 999f, this.ignorePoint);
			}
			NavMeshHit navMeshHit;
			if (!levelPoint || !NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1))
			{
				this.UpdateState(EnemyAnimal.State.Leave);
				return;
			}
			if (Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.ignorePoint = levelPoint;
				this.agentDestination = navMeshHit.position;
				this.enemy.NavMeshAgent.SetDestination(this.agentDestination);
			}
			this.havocTimer = 2f;
		}
		this.enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		this.havocTimer -= Time.deltaTime;
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyAnimal.State.Leave);
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000089C8 File Offset: 0x00006BC8
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, 25f, 50f, false);
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
		if (this.enemy.Rigidbody.notMovingTimer > 2f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		this.enemy.NavMeshAgent.OverrideAgent(6f, 12f, 0.25f);
		if (this.stateTimer <= 0f || Vector3.Distance(base.transform.position, this.agentDestination) < 1f)
		{
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00008AE9 File Offset: 0x00006CE9
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			this.UpdateState(EnemyAnimal.State.Spawn);
		}
		if (this.enemyAnimalAnim.isActiveAndEnabled)
		{
			this.enemyAnimalAnim.SetSpawn();
		}
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00008B20 File Offset: 0x00006D20
	public void OnHurt()
	{
		this.enemyAnimalAnim.hurtSound.Play(this.enemyAnimalAnim.transform.position, 1f, 1f, 1f, 1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.currentState == EnemyAnimal.State.Leave)
		{
			this.UpdateState(EnemyAnimal.State.Idle);
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00008B7C File Offset: 0x00006D7C
	public void OnDeath()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.enemyAnimalAnim.particleImpact.Play();
		this.enemyAnimalAnim.particleBits.Play();
		Quaternion rotation = Quaternion.LookRotation(-this.enemy.Health.hurtDirection.normalized);
		this.enemyAnimalAnim.particleDirectionalBits.transform.rotation = rotation;
		this.enemyAnimalAnim.particleDirectionalBits.Play();
		this.enemyAnimalAnim.particleLegBits.transform.rotation = rotation;
		this.enemyAnimalAnim.particleLegBits.Play();
		this.enemyAnimalAnim.deathSound.Play(this.enemyAnimalAnim.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00008CB8 File Offset: 0x00006EB8
	public void OnVision()
	{
		if (this.currentState != EnemyAnimal.State.Idle && this.currentState != EnemyAnimal.State.Roam && this.currentState != EnemyAnimal.State.Investigate)
		{
			return;
		}
		this.playerTarget = this.enemy.Vision.onVisionTriggeredPlayer;
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
				this.enemyAnimalAnim.NoticeSet(this.enemy.Vision.onVisionTriggeredID);
			}
		}
		this.UpdateState(EnemyAnimal.State.PlayerNotice);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00008D5E File Offset: 0x00006F5E
	public void OnInvestigate()
	{
		if (this.currentState == EnemyAnimal.State.Roam || this.currentState == EnemyAnimal.State.Idle || this.currentState == EnemyAnimal.State.Investigate)
		{
			this.UpdateState(EnemyAnimal.State.Investigate);
			this.agentDestination = this.enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00008D98 File Offset: 0x00006F98
	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.grabAggroTimer > 0f)
			{
				return;
			}
			if (this.currentState == EnemyAnimal.State.Leave)
			{
				this.grabAggroTimer = 60f;
				this.playerTarget = this.enemy.Rigidbody.onGrabbedPlayerAvatar;
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
						this.enemyAnimalAnim.NoticeSet(this.playerTarget.photonView.ViewID);
					}
				}
				this.UpdateState(EnemyAnimal.State.PlayerNotice);
			}
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00008E54 File Offset: 0x00007054
	private void UpdateState(EnemyAnimal.State _nextState)
	{
		this.stateTimer = 0f;
		this.stateImpulse = true;
		this.currentState = _nextState;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.Others, new object[]
			{
				_nextState
			});
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00008EA4 File Offset: 0x000070A4
	private void PlayerLookAt()
	{
		Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00008F29 File Offset: 0x00007129
	[PunRPC]
	private void UpdateStateRPC(EnemyAnimal.State _nextState)
	{
		this.currentState = _nextState;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00008F32 File Offset: 0x00007132
	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		this.enemyAnimalAnim.NoticeSet(_playerID);
	}

	// Token: 0x04000222 RID: 546
	private Enemy enemy;

	// Token: 0x04000223 RID: 547
	private PhotonView photonView;

	// Token: 0x04000224 RID: 548
	public EnemyAnimalAnim enemyAnimalAnim;

	// Token: 0x04000225 RID: 549
	public GameObject welts;

	// Token: 0x04000226 RID: 550
	public EnemyAnimal.State currentState;

	// Token: 0x04000227 RID: 551
	private float havocTimer;

	// Token: 0x04000228 RID: 552
	private LevelPoint ignorePoint;

	// Token: 0x04000229 RID: 553
	private float stateTimer;

	// Token: 0x0400022A RID: 554
	private bool stateImpulse;

	// Token: 0x0400022B RID: 555
	private Vector3 agentDestination;

	// Token: 0x0400022C RID: 556
	private PlayerAvatar playerTarget;

	// Token: 0x0400022D RID: 557
	private float grabAggroTimer;

	// Token: 0x020002C5 RID: 709
	public enum State
	{
		// Token: 0x040023A5 RID: 9125
		Spawn,
		// Token: 0x040023A6 RID: 9126
		Idle,
		// Token: 0x040023A7 RID: 9127
		Roam,
		// Token: 0x040023A8 RID: 9128
		Investigate,
		// Token: 0x040023A9 RID: 9129
		PlayerNotice,
		// Token: 0x040023AA RID: 9130
		GoToPlayer,
		// Token: 0x040023AB RID: 9131
		WreakHavoc,
		// Token: 0x040023AC RID: 9132
		Leave
	}
}
