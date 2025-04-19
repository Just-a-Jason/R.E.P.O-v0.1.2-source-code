using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class EnemyCeilingEye : MonoBehaviour
{
	// Token: 0x060001C4 RID: 452 RVA: 0x0001218F File Offset: 0x0001038F
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000121AC File Offset: 0x000103AC
	private void Update()
	{
		this.RotationAnimation();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.TargetFailSafe();
			switch (this.currentState)
			{
			case EnemyCeilingEye.State.Idle:
				this.StateIdle();
				break;
			case EnemyCeilingEye.State.Move:
				this.StateMove();
				break;
			case EnemyCeilingEye.State.TargetLost:
				this.StateTargetLost();
				break;
			case EnemyCeilingEye.State.HasTarget:
				this.StateHasTarget();
				break;
			case EnemyCeilingEye.State.Spawn:
				this.StateSpawn();
				break;
			case EnemyCeilingEye.State.Despawn:
				this.StateDespawn();
				break;
			}
		}
		if (this.currentState != EnemyCeilingEye.State.HasTarget || !this.targetPlayer)
		{
			this.eyeBeamLeft.outro = true;
			this.eyeBeamRight.outro = true;
			return;
		}
		SemiFunc.PlayerEyesOverride(this.targetPlayer, this.enemy.CenterTransform.position, 0.1f, base.gameObject);
		PlayerAvatar playerAvatar = this.targetPlayer;
		if (playerAvatar.voiceChat)
		{
			playerAvatar.voiceChat.OverridePitch(1.25f, 1f, 2f, 0.1f, true);
		}
		playerAvatar.OverridePupilSize(2f, 5, 5f, 0.5f, 5f, 0.5f, 0.1f);
		playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.CeilingEye, 0.25f, 0);
		if (this.targetPlayer.isLocal)
		{
			Vector3 vector = this.targetPlayer.transform.position - this.enemy.CenterTransform.position;
			float num = Vector3.Dot(Vector3.down, vector.normalized);
			float strengthNoAim = 10f;
			if (num > 0.9f)
			{
				strengthNoAim = 5f;
			}
			CameraAim.Instance.AimTargetSoftSet(this.enemy.CenterTransform.position, 0.1f, 2f, strengthNoAim, base.gameObject, 100);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.5f, 1f, 1f, 0.5f, 0.1f, base.gameObject);
			CameraZoom.Instance.OverrideZoomSet(40f, 0.1f, 1f, 1f, base.gameObject, 50);
			return;
		}
		this.eyeBeamLeft.outro = false;
		this.eyeBeamRight.outro = false;
		this.eyeBeamLeft.lineTarget = this.targetPlayer.playerAvatarVisuals.playerEyes.pupilLeft;
		this.eyeBeamRight.lineTarget = this.targetPlayer.playerAvatarVisuals.playerEyes.pupilRight;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00012424 File Offset: 0x00010624
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(20f, 60f);
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		if (!this.enemy.EnemyParent.playerClose)
		{
			this.stateTimer -= Time.deltaTime;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyCeilingEye.State.Move);
		}
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00012495 File Offset: 0x00010695
	private void StateMove()
	{
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00012498 File Offset: 0x00010698
	private void StateHasTarget()
	{
		if (this.stateImpulse)
		{
			this.eyeDamageWaitTimer = 3f;
			this.stateImpulse = false;
		}
		if (this.eyeDamageWaitTimer <= 0f)
		{
			this.eyeDamageTimer -= Time.deltaTime;
			if (this.eyeDamageTimer <= 0f)
			{
				this.targetPlayer.playerHealth.HurtOther(2, this.targetPlayer.transform.position, false, SemiFunc.EnemyGetIndex(this.enemy));
				this.eyeDamageTimer = 1f;
			}
		}
		else
		{
			if (this.targetPlayer.isDisabled)
			{
				this.UpdateState(EnemyCeilingEye.State.TargetLost);
				return;
			}
			this.eyeDamageWaitTimer -= Time.deltaTime;
		}
		Vector3 position = this.targetPlayer.PlayerVisionTarget.VisionTransform.position;
		this.stateTimer -= Time.deltaTime;
		if (SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position, position, 20f, this.targetPlayer, true) || SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position + base.transform.right * 0.25f, position + Vector3.down * 0.1f, 20f, this.targetPlayer, true) || SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position - base.transform.right * 0.25f, position - Vector3.down * 0.1f, 20f, this.targetPlayer, true) || SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position + base.transform.up * 0.25f, position + Vector3.down * 0.1f, 20f, this.targetPlayer, true) || SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position - base.transform.up * 0.25f, position - Vector3.down * 0.1f, 20f, this.targetPlayer, true))
		{
			this.stateTimer = 0.5f;
		}
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyCeilingEye.State.TargetLost);
		}
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00012724 File Offset: 0x00010924
	private void StateTargetLost()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
			this.UpdateState(EnemyCeilingEye.State.Despawn);
		}
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00012788 File Offset: 0x00010988
	private void StateSpawn()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.stateImpulse)
			{
				this.stateTimer = 5f;
				this.stateImpulse = false;
			}
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyCeilingEye.State.Idle);
			}
		}
	}

	// Token: 0x060001CB RID: 459 RVA: 0x000127E3 File Offset: 0x000109E3
	private void StateDespawn()
	{
	}

	// Token: 0x060001CC RID: 460 RVA: 0x000127E8 File Offset: 0x000109E8
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.otherEnemyFetch)
			{
				this.otherEnemyFetch = false;
				foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
				{
					EnemyCeilingEye componentInChildren = enemyParent.GetComponentInChildren<EnemyCeilingEye>(true);
					if (componentInChildren && componentInChildren != this)
					{
						this.otherEnemies.Add(componentInChildren);
					}
				}
			}
			if (SemiFunc.EnemySpawn(this.enemy))
			{
				RaycastHit raycastHit;
				if (Physics.SphereCast(base.transform.position + Vector3.up * 0.1f + new Vector3(0f, 0.5f, 0f), 0.1f, Vector3.up, out raycastHit, 30f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					foreach (EnemyCeilingEye enemyCeilingEye in this.otherEnemies)
					{
						if (enemyCeilingEye.isActiveAndEnabled && Vector3.Distance(enemyCeilingEye.transform.position, raycastHit.point) <= 2f)
						{
							this.enemy.StateDespawn.Despawn();
							this.enemy.EnemyParent.DespawnedTimerSet(Random.Range(5f, 10f), true);
							return;
						}
					}
					base.transform.position = raycastHit.point;
					base.transform.rotation = Quaternion.LookRotation(raycastHit.normal);
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("UpdatePositionRPC", RpcTarget.All, new object[]
						{
							base.transform.position,
							base.transform.rotation
						});
					}
				}
				this.UpdateState(EnemyCeilingEye.State.Spawn);
			}
		}
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000129F8 File Offset: 0x00010BF8
	public void OnDeath()
	{
		this.deathImpulse = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00012A20 File Offset: 0x00010C20
	public void OnVisionTrigger()
	{
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (this.currentState == EnemyCeilingEye.State.Idle || this.currentState == EnemyCeilingEye.State.TargetLost)
		{
			PlayerAvatar onVisionTriggeredPlayer = this.enemy.Vision.onVisionTriggeredPlayer;
			if (SemiFunc.PlayerVisionCheckPosition(this.enemy.Vision.VisionTransform.position, onVisionTriggeredPlayer.PlayerVisionTarget.VisionTransform.position, this.enemy.Vision.VisionDistance, onVisionTriggeredPlayer, true))
			{
				if (this.targetPlayer != onVisionTriggeredPlayer)
				{
					using (List<EnemyCeilingEye>.Enumerator enumerator = this.otherEnemies.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.targetPlayer == onVisionTriggeredPlayer)
							{
								return;
							}
						}
					}
					this.targetPlayer = onVisionTriggeredPlayer;
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("TargetPlayerRPC", RpcTarget.All, new object[]
						{
							this.targetPlayer.photonView.ViewID
						});
					}
				}
				this.UpdateState(EnemyCeilingEye.State.HasTarget);
			}
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00012B44 File Offset: 0x00010D44
	public void TargetFailSafe()
	{
		if (this.currentState != EnemyCeilingEye.State.HasTarget)
		{
			this.targetPlayer = null;
			return;
		}
		if (this.currentState == EnemyCeilingEye.State.HasTarget && (!this.targetPlayer || this.targetPlayer.isDisabled))
		{
			this.UpdateState(EnemyCeilingEye.State.TargetLost);
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00012B84 File Offset: 0x00010D84
	private void UpdateState(EnemyCeilingEye.State _state)
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
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
			return;
		}
		this.UpdateStateRPC(this.currentState);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x00012BFC File Offset: 0x00010DFC
	public void RotationAnimation()
	{
		if (this.currentState == EnemyCeilingEye.State.Idle)
		{
			this.eyeTransform.localRotation = Quaternion.Slerp(this.eyeTransform.localRotation, Quaternion.identity, 5f * Time.deltaTime);
			return;
		}
		if (this.currentState == EnemyCeilingEye.State.HasTarget)
		{
			Vector3 forward = SemiFunc.ClampDirection(this.targetPlayer.transform.position - this.enemy.CenterTransform.position, base.transform.forward, 35f);
			this.eyeTransform.rotation = Quaternion.RotateTowards(this.eyeTransform.rotation, Quaternion.LookRotation(forward), 360f * Time.deltaTime);
		}
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00012CB0 File Offset: 0x00010EB0
	[PunRPC]
	private void UpdateStateRPC(EnemyCeilingEye.State _state)
	{
		this.currentState = _state;
		this.stateImpulse = true;
		if (this.currentState == EnemyCeilingEye.State.HasTarget)
		{
			if (this.targetPlayer.isLocal)
			{
				this.targetPlayer.physGrabber.ReleaseObject(0.1f);
				CameraAim.Instance.AimTargetSet(this.enemy.CenterTransform.position, 0.5f, 2f, base.gameObject, 100);
				CameraGlitch.Instance.PlayLong();
				return;
			}
		}
		else if (this.currentState == EnemyCeilingEye.State.Spawn && this.eyeAnim.isActiveAndEnabled)
		{
			this.eyeAnim.SetSpawn();
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x00012D50 File Offset: 0x00010F50
	[PunRPC]
	private void TargetPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				if (playerAvatar.isLocal)
				{
					playerAvatar.physGrabber.ReleaseObject(1f);
				}
				this.targetPlayer = playerAvatar;
			}
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00012DD0 File Offset: 0x00010FD0
	[PunRPC]
	private void UpdatePositionRPC(Vector3 _position, Quaternion _rotation)
	{
		base.transform.position = _position;
		base.transform.rotation = _rotation;
	}

	// Token: 0x040003AA RID: 938
	public CeilingEyeLine eyeBeamLeft;

	// Token: 0x040003AB RID: 939
	public CeilingEyeLine eyeBeamRight;

	// Token: 0x040003AC RID: 940
	public ParticleSystem eyeBeamParticles;

	// Token: 0x040003AD RID: 941
	[Header("References")]
	public Transform eyeTransform;

	// Token: 0x040003AE RID: 942
	public EnemyCeilingEyeAnim eyeAnim;

	// Token: 0x040003AF RID: 943
	internal Enemy enemy;

	// Token: 0x040003B0 RID: 944
	private bool otherEnemyFetch = true;

	// Token: 0x040003B1 RID: 945
	public List<EnemyCeilingEye> otherEnemies;

	// Token: 0x040003B2 RID: 946
	public EnemyCeilingEye.State currentState;

	// Token: 0x040003B3 RID: 947
	private bool stateImpulse;

	// Token: 0x040003B4 RID: 948
	private float stateTimer;

	// Token: 0x040003B5 RID: 949
	internal PlayerAvatar targetPlayer;

	// Token: 0x040003B6 RID: 950
	private PhotonView photonView;

	// Token: 0x040003B7 RID: 951
	internal bool deathImpulse;

	// Token: 0x040003B8 RID: 952
	private float eyeDamageTimer;

	// Token: 0x040003B9 RID: 953
	private float eyeDamageWaitTimer;

	// Token: 0x020002CD RID: 717
	public enum State
	{
		// Token: 0x040023E6 RID: 9190
		Idle,
		// Token: 0x040023E7 RID: 9191
		Move,
		// Token: 0x040023E8 RID: 9192
		TargetLost,
		// Token: 0x040023E9 RID: 9193
		HasTarget,
		// Token: 0x040023EA RID: 9194
		Spawn,
		// Token: 0x040023EB RID: 9195
		Despawn
	}
}
