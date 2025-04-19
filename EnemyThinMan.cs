using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class EnemyThinMan : MonoBehaviour
{
	// Token: 0x060004BC RID: 1212 RVA: 0x0002EF82 File Offset: 0x0002D182
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0002EF9C File Offset: 0x0002D19C
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			switch (this.currentState)
			{
			case EnemyThinMan.State.Stand:
				this.StateStand();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.OnScreen:
				this.StateOnScreen();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Notice:
				this.StateNotice();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Attack:
				this.StateAttack();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.TentacleExtend:
				this.StateTentacleExtend();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Damage:
				this.StateDamage();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Despawn:
				this.StateDespawn();
				break;
			case EnemyThinMan.State.Stunned:
				this.StateStunned();
				break;
			}
			if (this.enemy.IsStunned())
			{
				this.enemy.EnemyParent.SpawnedTimerSet(0f);
				this.UpdateState(EnemyThinMan.State.Stunned);
			}
		}
		this.TeleportLogic();
		this.SetFollowTargetToPosition();
		this.TentacleLogic();
		this.LocalEffect();
		this.HurtColliderLogic();
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0002F0A0 File Offset: 0x0002D2A0
	private void StateStand()
	{
		if (this.stateImpulse)
		{
			this.SetTarget(null);
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		if (!this.playerTarget)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (!playerAvatar.isDisabled && this.enemy.OnScreen.GetOnScreen(playerAvatar))
				{
					this.SetTarget(playerAvatar);
					this.UpdateState(EnemyThinMan.State.OnScreen);
					return;
				}
			}
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		if (this.teleportRoamTimer > 0f)
		{
			this.teleportRoamTimer -= Time.deltaTime;
		}
		else if (this.Teleport(false, false))
		{
			this.SetRoamTimer();
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.Teleport(false, true);
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0002F198 File Offset: 0x0002D398
	private void StateOnScreen()
	{
		if (this.stateImpulse)
		{
			this.tpTimer = Random.Range(0f, 5f);
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		bool flag = false;
		if (this.enemy.OnScreen.GetOnScreen(this.playerTarget))
		{
			this.stateTimer = 0.2f;
			flag = true;
		}
		if (this.tpTimer > 0f)
		{
			this.tpTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (this.tentacleLerp >= 1f)
			{
				this.UpdateState(EnemyThinMan.State.Notice);
				return;
			}
			if (this.tentacleLerp > 0.05f && this.tentacleLerp < 0.15f && this.tpTimer <= 0f)
			{
				if (Random.Range(0f, 1f) < 0.5f)
				{
					if (this.Teleport(false, false))
					{
						this.tpTimer = 5f;
					}
				}
				else
				{
					this.tpTimer = 5f;
				}
			}
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.Stand);
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0002F2B9 File Offset: 0x0002D4B9
	private void StateNotice()
	{
		if (!GameManager.Multiplayer())
		{
			this.NoticeRPC();
		}
		else
		{
			this.photonView.RPC("NoticeRPC", RpcTarget.All, Array.Empty<object>());
		}
		this.UpdateState(EnemyThinMan.State.Attack);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0002F2E8 File Offset: 0x0002D4E8
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.TentacleExtend);
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0002F338 File Offset: 0x0002D538
	private void StateTentacleExtend()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.1f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.Damage);
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0002F388 File Offset: 0x0002D588
	private void StateDamage()
	{
		this.playerTarget.playerHealth.HurtOther(30, this.playerTarget.transform.position, false, SemiFunc.EnemyGetIndex(this.enemy));
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ActivateHurtColliderRPC", RpcTarget.All, new object[]
			{
				this.playerTarget.transform.position
			});
		}
		else
		{
			this.ActivateHurtColliderRPC(this.playerTarget.transform.position);
		}
		this.UpdateState(EnemyThinMan.State.Despawn);
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0002F418 File Offset: 0x0002D618
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.4f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0002F473 File Offset: 0x0002D673
	private void StateStunned()
	{
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0002F478 File Offset: 0x0002D678
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.otherEnemyFetch)
			{
				this.otherEnemyFetch = false;
				foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
				{
					EnemyThinMan componentInChildren = enemyParent.GetComponentInChildren<EnemyThinMan>(true);
					if (componentInChildren && componentInChildren != this)
					{
						this.otherEnemies.Add(componentInChildren);
					}
				}
			}
			if (SemiFunc.EnemySpawnIdlePause())
			{
				this.lastTeleportPosition = base.transform.position;
				SemiFunc.EnemySpawn(this.enemy);
				this.teleportPosition = base.transform.position;
				this.teleporting = true;
				this.UpdateState(EnemyThinMan.State.Stand);
				return;
			}
			if (this.Teleport(true, false))
			{
				this.SetRoamTimer();
				this.UpdateState(EnemyThinMan.State.Stand);
				return;
			}
			this.enemy.EnemyParent.Despawn();
			this.enemy.EnemyParent.DespawnedTimerSet(3f, false);
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0002F588 File Offset: 0x0002D788
	public void OnHurt()
	{
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0002F5C0 File Offset: 0x0002D7C0
	private void UpdateState(EnemyThinMan.State _nextState)
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

	// Token: 0x060004C9 RID: 1225 RVA: 0x0002F610 File Offset: 0x0002D810
	private bool Teleport(bool _spawn, bool _leave = false)
	{
		List<LevelPoint> list = new List<LevelPoint>();
		if (_leave)
		{
			list.Add(SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f));
		}
		else
		{
			list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 3f, 30f);
			if (list == null)
			{
				list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 3f, 50f);
				if (list == null)
				{
					list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 0f, 999f);
				}
			}
		}
		if (list == null)
		{
			return false;
		}
		bool flag = Random.Range(0, 100) < 3;
		if (this.playerTarget)
		{
			flag = (Random.Range(0, 100) < 30);
		}
		if (flag && !_leave)
		{
			list = SemiFunc.LevelPointsGetAllCloseToPlayers();
		}
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		LevelPoint levelPoint = list[Random.Range(0, list.Count)];
		RaycastHit raycastHit;
		if (Physics.Raycast(levelPoint.transform.position + Vector3.up * 0.1f, Vector3.up, out raycastHit, 3.5f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			return false;
		}
		foreach (EnemyThinMan enemyThinMan in this.otherEnemies)
		{
			if (enemyThinMan.isActiveAndEnabled && Vector3.Distance(enemyThinMan.rb.position, levelPoint.transform.position) <= 2f)
			{
				return false;
			}
		}
		if (Vector3.Distance(levelPoint.transform.position, this.lastTeleportPosition) < 1f)
		{
			return false;
		}
		if (SemiFunc.EnemyPhysObjectBoundingBoxCheck(base.transform.position, levelPoint.transform.position, this.enemy.Rigidbody.rb))
		{
			return false;
		}
		this.lastTeleportPosition = base.transform.position;
		this.teleportPosition = levelPoint.transform.position;
		this.teleporting = true;
		if (!_spawn)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
				{
					this.lastTeleportPosition,
					true
				});
			}
			else
			{
				this.TeleportEffectRPC(this.lastTeleportPosition, true);
			}
		}
		else
		{
			this.enemy.EnemyTeleported(this.teleportPosition);
		}
		return true;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0002F884 File Offset: 0x0002DA84
	private void TentacleLogic()
	{
		if (this.currentState == EnemyThinMan.State.OnScreen)
		{
			this.tentacleLerp += this.anim.tentacleSpeed * Time.deltaTime;
		}
		else if (this.currentState == EnemyThinMan.State.Attack || this.currentState == EnemyThinMan.State.TentacleExtend)
		{
			if (this.currentState == EnemyThinMan.State.TentacleExtend)
			{
				this.tentacleLerp -= 10f * Time.deltaTime;
			}
		}
		else if (this.currentState == EnemyThinMan.State.Stunned)
		{
			this.tentacleLerp -= 0.4f * Time.deltaTime;
		}
		else
		{
			this.tentacleLerp -= this.anim.tentacleSpeed * 0.5f * Time.deltaTime;
		}
		this.tentacleLerp = Mathf.Clamp(this.tentacleLerp, 0f, 1f);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0002F954 File Offset: 0x0002DB54
	private void TeleportLogic()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.teleportTimer <= 0f)
			{
				if (this.teleporting)
				{
					this.enemy.EnemyTeleported(this.teleportPosition);
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
						{
							this.teleportPosition,
							false
						});
					}
					else
					{
						this.TeleportEffectRPC(this.teleportPosition, false);
					}
					this.teleporting = false;
					return;
				}
			}
			else
			{
				this.teleportTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0002F9F0 File Offset: 0x0002DBF0
	private void PlayerLookAt()
	{
		if (this.playerTarget)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0002FA82 File Offset: 0x0002DC82
	private void SetFollowTargetToPosition()
	{
		this.enemy.transform.position = this.teleportPosition;
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0002FA9A File Offset: 0x0002DC9A
	public void SmokeEffect(Vector3 pos)
	{
		this.anim.particleSmokeCalmFill.Play();
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0002FAAC File Offset: 0x0002DCAC
	private void Rattle()
	{
		this.anim.notice.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.anim.rattleImpulse = true;
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0002FAEA File Offset: 0x0002DCEA
	private void LocalEffect()
	{
		if (this.currentState == EnemyThinMan.State.OnScreen && this.playerTarget && this.playerTarget.isLocal)
		{
			SemiFunc.DoNotLookEffect(base.gameObject, true, true, true, true, true, false);
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0002FB20 File Offset: 0x0002DD20
	private void SetRoamTimer()
	{
		this.teleportRoamTimer = Random.Range(8f, 22f);
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0002FB37 File Offset: 0x0002DD37
	private void HurtColliderLogic()
	{
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.SetActive(false);
			}
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0002FB71 File Offset: 0x0002DD71
	[PunRPC]
	private void UpdateStateRPC(EnemyThinMan.State _nextState)
	{
		this.currentState = _nextState;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0002FB7A File Offset: 0x0002DD7A
	[PunRPC]
	private void NoticeRPC()
	{
		this.anim.NoticeSet();
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0002FB88 File Offset: 0x0002DD88
	[PunRPC]
	public void TeleportEffectRPC(Vector3 position, bool intro)
	{
		this.SmokeEffect(position);
		if (intro)
		{
			this.anim.teleportIn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.anim.teleportOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.anim.rattleImpulse = true;
		this.teleportTimer = 0.1f;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0002FC18 File Offset: 0x0002DE18
	private void SetTarget(PlayerAvatar _player)
	{
		if (_player == this.playerTarget)
		{
			return;
		}
		this.playerTarget = _player;
		bool flag = true;
		int num = -1;
		if (!this.playerTarget)
		{
			flag = false;
		}
		if (flag)
		{
			this.Rattle();
			num = this.playerTarget.photonView.ViewID;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetTargetRPC", RpcTarget.Others, new object[]
			{
				num,
				flag
			});
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0002FC98 File Offset: 0x0002DE98
	[PunRPC]
	public void SetTargetRPC(int playerID, bool hasTarget)
	{
		if (!hasTarget)
		{
			this.playerTarget = null;
			return;
		}
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == playerID)
			{
				this.playerTarget = playerAvatar;
				break;
			}
		}
		this.Rattle();
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0002FD0C File Offset: 0x0002DF0C
	[PunRPC]
	public void ActivateHurtColliderRPC(Vector3 _position)
	{
		this.hurtCollider.transform.position = _position;
		this.hurtCollider.transform.rotation = Quaternion.LookRotation(this.enemy.Vision.VisionTransform.position - _position);
		this.hurtCollider.SetActive(true);
		this.hurtColliderTimer = 0.25f;
	}

	// Token: 0x040007AA RID: 1962
	private PhotonView photonView;

	// Token: 0x040007AB RID: 1963
	private Enemy enemy;

	// Token: 0x040007AC RID: 1964
	public EnemyThinManAnim anim;

	// Token: 0x040007AD RID: 1965
	public GameObject tentacleR1;

	// Token: 0x040007AE RID: 1966
	public GameObject tentacleR2;

	// Token: 0x040007AF RID: 1967
	public GameObject tentacleR3;

	// Token: 0x040007B0 RID: 1968
	public GameObject tentacleL1;

	// Token: 0x040007B1 RID: 1969
	public GameObject tentacleL2;

	// Token: 0x040007B2 RID: 1970
	public GameObject tentacleL3;

	// Token: 0x040007B3 RID: 1971
	public GameObject extendedTentacles;

	// Token: 0x040007B4 RID: 1972
	public GameObject head;

	// Token: 0x040007B5 RID: 1973
	public GameObject hurtCollider;

	// Token: 0x040007B6 RID: 1974
	private float hurtColliderTimer;

	// Token: 0x040007B7 RID: 1975
	public float tentacleLerp;

	// Token: 0x040007B8 RID: 1976
	public EnemyThinMan.State currentState;

	// Token: 0x040007B9 RID: 1977
	private float stateTimer;

	// Token: 0x040007BA RID: 1978
	private bool stateImpulse;

	// Token: 0x040007BB RID: 1979
	private float tpTimer;

	// Token: 0x040007BC RID: 1980
	public Rigidbody rb;

	// Token: 0x040007BD RID: 1981
	internal PlayerAvatar playerTarget;

	// Token: 0x040007BE RID: 1982
	private bool otherEnemyFetch = true;

	// Token: 0x040007BF RID: 1983
	public List<EnemyThinMan> otherEnemies;

	// Token: 0x040007C0 RID: 1984
	private Vector3 teleportPosition;

	// Token: 0x040007C1 RID: 1985
	private Vector3 lastTeleportPosition;

	// Token: 0x040007C2 RID: 1986
	private float teleportTimer;

	// Token: 0x040007C3 RID: 1987
	private bool teleporting;

	// Token: 0x040007C4 RID: 1988
	private float teleportRoamTimer;

	// Token: 0x020002E2 RID: 738
	public enum State
	{
		// Token: 0x040024BE RID: 9406
		Stand,
		// Token: 0x040024BF RID: 9407
		OnScreen,
		// Token: 0x040024C0 RID: 9408
		Notice,
		// Token: 0x040024C1 RID: 9409
		Attack,
		// Token: 0x040024C2 RID: 9410
		TentacleExtend,
		// Token: 0x040024C3 RID: 9411
		Damage,
		// Token: 0x040024C4 RID: 9412
		Despawn,
		// Token: 0x040024C5 RID: 9413
		Stunned
	}
}
