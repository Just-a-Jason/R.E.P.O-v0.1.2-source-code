using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001A6 RID: 422
public class PlayerAvatar : MonoBehaviour, IPunObservable
{
	// Token: 0x06000E0F RID: 3599 RVA: 0x0007EA30 File Offset: 0x0007CC30
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.collider = base.GetComponentInChildren<Collider>();
		this.isDisabled = false;
		base.transform.position = Vector3.zero + Vector3.forward * 2f;
		this.playerAvatarCollision = base.GetComponent<PlayerAvatarCollision>();
		GameDirector.instance.PlayerList.Add(this);
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0007EAC4 File Offset: 0x0007CCC4
	private void OnDestroy()
	{
		GameDirector.instance.PlayerList.Remove(this);
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			enemyParent.Enemy.PlayerRemoved(this.photonView.ViewID);
		}
		Object.Destroy(base.transform.parent.gameObject);
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0007EB50 File Offset: 0x0007CD50
	private void Start()
	{
		this.overridePupilSizeSpring.speed = 15f;
		this.overridePupilSizeSpring.damping = 0.3f;
		this.localCamera = Camera.main;
		this.deadTimer = this.deadTime;
		this.deadVoiceTimer = this.deadVoiceTime;
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			base.StartCoroutine(this.WaitForSteamID());
			this.playerTransform = PlayerController.instance.transform;
			this.playerTransform.position = base.transform.position;
			PlayerController.instance.playerAvatar = base.gameObject;
			PlayerController.instance.playerAvatarScript = base.gameObject.GetComponent<PlayerAvatar>();
			if (PlayerAvatar.instance)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			PlayerAvatar.instance = this;
		}
		this.SoundSetup(this.jumpSound);
		this.SoundSetup(this.extraJumpSound);
		this.SoundSetup(this.landSound);
		this.SoundSetup(this.slideSound);
		this.SoundSetup(this.standToCrouchSound);
		this.SoundSetup(this.crouchToStandSound);
		this.SoundSetup(this.crouchToCrawlSound);
		this.SoundSetup(this.crawlToCrouchSound);
		this.SoundSetup(this.tumbleStartSound);
		this.SoundSetup(this.tumbleStopSound);
		this.SoundSetup(this.tumbleBreakFreeSound);
		this.AddToStatsManager();
		if (SemiFunc.IsMasterClient() && LevelGenerator.Instance.Generated)
		{
			LevelGenerator.Instance.PlayerSpawn();
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0007ECE0 File Offset: 0x0007CEE0
	private IEnumerator WaitForSteamID()
	{
		while (this.steamID == null)
		{
			yield return null;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
		else if (!SemiFunc.IsMainMenu())
		{
			this.PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
		yield break;
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0007ECEF File Offset: 0x0007CEEF
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		if (StatsManager.instance.playerUpgradeMapPlayerCount.ContainsKey(this.steamID))
		{
			this.upgradeMapPlayerCount = StatsManager.instance.playerUpgradeMapPlayerCount[this.steamID];
		}
		WorldSpaceUIParent.instance.PlayerName(this);
		yield break;
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0007ED00 File Offset: 0x0007CF00
	private void AddToStatsManager()
	{
		string text = SemiFunc.PlayerGetName(this);
		string text2 = SteamClient.SteamId.Value.ToString();
		if (GameManager.Multiplayer() && GameManager.instance.localTest)
		{
			int num = 0;
			Player[] playerList = PhotonNetwork.PlayerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i].IsLocal)
				{
					text = text + " " + num.ToString();
					text2 += num.ToString();
				}
				num++;
			}
		}
		if (GameManager.Multiplayer())
		{
			if (this.photonView.IsMine)
			{
				this.photonView.RPC("AddToStatsManagerRPC", RpcTarget.AllBuffered, new object[]
				{
					text,
					text2
				});
				return;
			}
		}
		else
		{
			this.AddToStatsManagerRPC(text, text2);
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0007EDC4 File Offset: 0x0007CFC4
	private void FinalHealCheck()
	{
		if (!this.isLocal)
		{
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		if (SemiFunc.FPSImpulse5() && RoundDirector.instance.allExtractionPointsCompleted && this.RoomVolumeCheck.inTruck && !this.finalHeal)
		{
			this.FinalHeal();
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0007EE17 File Offset: 0x0007D017
	private void FinalHeal()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FinalHealRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.FinalHealRPC();
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x0007EE40 File Offset: 0x0007D040
	[PunRPC]
	public void FinalHealRPC()
	{
		if (this.finalHeal)
		{
			return;
		}
		if (this.isLocal)
		{
			this.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 2f, 1);
			TruckScreenText.instance.MessageSendCustom("", this.playerName + " {arrowright}{truck}{check}\n {point}{shades}{pointright}<b><color=#00FF00>+25</color></b>{heart}", 0);
			this.playerHealth.Heal(25, true);
		}
		TruckHealer.instance.Heal(this);
		this.truckReturn.Play(this.PlayerVisionTarget.VisionTransform.position, 1f, 1f, 1f, 1f);
		this.truckReturnGlobal.Play(this.PlayerVisionTarget.VisionTransform.position, 1f, 1f, 1f, 1f);
		this.playerAvatarVisuals.effectGetIntoTruck.gameObject.SetActive(true);
		this.finalHeal = true;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0007EF28 File Offset: 0x0007D128
	[PunRPC]
	public void AddToStatsManagerRPC(string _playerName, string _steamID)
	{
		this.playerName = _playerName;
		this.steamID = _steamID;
		if (!SemiFunc.IsMultiplayer() || (SemiFunc.IsMultiplayer() && this.photonView.IsMine))
		{
			PlayerController.instance.PlayerSetName(this.playerName, this.steamID);
		}
		if (StatsManager.instance)
		{
			StatsManager.instance.PlayerAdd(_steamID, _playerName);
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0007EF8C File Offset: 0x0007D18C
	[PunRPC]
	public void UpdateMyPlayerVoiceChat(int photonViewID)
	{
		this.voiceChat = PhotonView.Find(photonViewID).GetComponent<PlayerVoiceChat>();
		this.voiceChat.playerAvatar = this;
		if (this.voiceChat.TTSinstantiated)
		{
			this.voiceChat.ttsVoice.playerAvatar = this;
		}
		if (!SemiFunc.MenuLevel())
		{
			this.voiceChat.ToggleLobby(false);
		}
		this.voiceChatFetched = true;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0007EFEE File Offset: 0x0007D1EE
	[PunRPC]
	public void ResetPhysPusher()
	{
		this.playerPhysPusher.Reset = true;
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0007EFFC File Offset: 0x0007D1FC
	public void SetDisabled()
	{
		if (GameManager.Multiplayer())
		{
			if (this.photonView.IsMine)
			{
				this.photonView.RPC("SetDisabledRPC", RpcTarget.All, Array.Empty<object>());
				PlayerVoiceChat.instance.OverridePitchCancel();
				return;
			}
		}
		else
		{
			this.SetDisabledRPC();
		}
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0007F039 File Offset: 0x0007D239
	[PunRPC]
	public void SetDisabledRPC()
	{
		this.isDisabled = true;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0007F042 File Offset: 0x0007D242
	public void UpdateState(bool isCrouching, bool isSprinting, bool isCrawling, bool isSliding, bool isMoving)
	{
		this.SetState(isCrouching, isSprinting, isCrawling, isSliding, isMoving);
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0007F054 File Offset: 0x0007D254
	private void FixedUpdate()
	{
		this.OverridePupilSizeTick();
		this.OverrideAnimationSpeedTick();
		if (SemiFunc.IsMultiplayer())
		{
			this.playerPingTimer -= Time.deltaTime;
			if (this.playerPingTimer <= 0f)
			{
				this.playerPing = PhotonNetwork.GetPing();
				this.playerPingTimer = 6f;
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			if (this.spawned)
			{
				this.clientPosition = this.spawnPosition;
				this.clientPositionCurrent = this.spawnPosition;
				this.clientRotation = this.spawnRotation;
				this.clientRotationCurrent = this.spawnRotation;
				base.transform.position = this.spawnPosition;
				base.transform.rotation = this.spawnRotation;
				this.rb.MovePosition(base.transform.position);
				this.rb.MoveRotation(base.transform.rotation);
				if (PlayerController.instance.playerAvatarScript == this)
				{
					PlayerController.instance.transform.position = this.spawnPosition;
					PlayerController.instance.transform.rotation = this.spawnRotation;
				}
				if (this.spawnImpulse)
				{
					if (this.spawnFrames <= 0)
					{
						if (GameManager.Multiplayer())
						{
							LevelGenerator.Instance.PhotonView.RPC("PlayerSpawnedRPC", RpcTarget.All, Array.Empty<object>());
						}
						else
						{
							LevelGenerator.Instance.playerSpawned++;
						}
						this.spawnImpulse = false;
						return;
					}
					this.spawnFrames--;
				}
			}
			return;
		}
		if (this.spawnDoneImpulse)
		{
			if (PlayerController.instance.playerAvatarScript == this)
			{
				if (TruckScreenText.instance && !SemiFunc.MenuLevel())
				{
					Vector3 position = TruckScreenText.instance.transform.position;
					Vector3 eulerAngles = Quaternion.LookRotation(position - base.transform.position).eulerAngles;
					CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
					CameraAim.Instance.AimTargetSet(position, 0.3f, 4f, base.gameObject, 0);
				}
				else
				{
					CameraAim.Instance.CameraAimSpawn(this.spawnRotation.eulerAngles.y);
				}
				if (SemiFunc.MenuLevel())
				{
					PlayerController.instance.rb.isKinematic = false;
				}
			}
			this.rb.isKinematic = false;
			this.spawnDoneImpulse = false;
		}
		if (this.photonView.IsMine || !SemiFunc.IsMultiplayer())
		{
			this.rbVelocityRaw = PlayerController.instance.rb.velocity;
			this.rb.MovePosition(base.transform.position);
			this.rb.MoveRotation(base.transform.rotation);
			return;
		}
		this.rb.MovePosition(this.clientPositionCurrent);
		this.rb.MoveRotation(this.clientRotationCurrent);
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0007F328 File Offset: 0x0007D528
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		this.FinalHealCheck();
		this.OverrideAnimationSpeedLogic();
		this.OverridePupilSizeLogic();
		if (GameManager.Multiplayer() && GameDirector.instance.currentState >= GameDirector.gameState.Main)
		{
			if (this.voiceChatFetched)
			{
				if (!this.isDisabled)
				{
					this.voiceChat.transform.position = Vector3.Lerp(this.voiceChat.transform.position, this.PlayerVisionTarget.VisionTransform.transform.position, 30f * Time.deltaTime);
				}
			}
			else if (this.photonView.IsMine && PlayerVoiceChat.instance)
			{
				this.photonView.RPC("UpdateMyPlayerVoiceChat", RpcTarget.AllBuffered, new object[]
				{
					PlayerVoiceChat.instance.photonView.ViewID
				});
			}
		}
		if (this.photonView.IsMine || GameManager.instance.gameMode == 0)
		{
			if (this.playerTransform)
			{
				base.transform.position = this.playerTransform.position;
				base.transform.rotation = this.playerTransform.rotation;
			}
			this.localCameraRotation = PlayerController.instance.cameraGameObject.transform.rotation;
			this.localCameraPosition = PlayerController.instance.cameraGameObject.transform.position;
			this.localCameraTransform.position = PlayerController.instance.cameraGameObjectLocal.transform.position;
			this.localCameraTransform.rotation = PlayerController.instance.cameraGameObjectLocal.transform.rotation;
			this.InputDirection = PlayerController.instance.InputDirection;
		}
		else
		{
			this.clientPositionCurrent = this.clientPosition;
			this.clientRotationCurrent = this.clientRotation;
			this.localCameraTransform.position = Vector3.Lerp(this.localCameraTransform.position, this.localCameraPosition, 20f * Time.deltaTime);
			this.localCameraTransform.rotation = Quaternion.Lerp(this.localCameraTransform.rotation, this.localCameraRotation, 20f * Time.deltaTime);
		}
		if (this.deadSet)
		{
			if (this.isLocal && this.deadEnemyLookAtTransform)
			{
				CameraAim.Instance.AimTargetSet(this.deadEnemyLookAtTransform.position, 1f, 80f, this.deadEnemyLookAtTransform.gameObject, 0);
			}
			this.deadTimer -= Time.deltaTime;
			if (this.deadVoiceTimer > 0f)
			{
				this.deadVoiceTimer -= Time.deltaTime;
				if (this.deadVoiceTimer <= 0f && this.voiceChatFetched)
				{
					this.voiceChat.ToggleLobby(true);
				}
			}
			if (this.deadTimer <= 0f)
			{
				this.PlayerDeathDone();
			}
		}
		if (this.tumble)
		{
			this.isTumbling = this.tumble.isTumbling;
		}
		if (this.isTumbling)
		{
			this.collider.enabled = false;
		}
		else
		{
			this.collider.enabled = true;
		}
		this.LastNavMeshPositionTimer += Time.deltaTime;
		RaycastHit raycastHit;
		NavMeshHit navMeshHit;
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 2f, LayerMask.GetMask(new string[]
		{
			"Default",
			"NavmeshOnly",
			"PlayerOnlyCollision"
		})) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 0.5f, -1))
		{
			this.LastNavmeshPosition = navMeshHit.position;
			this.LastNavMeshPositionTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (base.transform.position.y < -50f)
			{
				this.PlayerDeath(-1);
			}
			this.FallDamageResetLogic();
		}
		if (this.enemyVisionFreezeTimer > 0f)
		{
			this.enemyVisionFreezeTimer -= Time.deltaTime;
		}
		if (this.isLocal && PlayerController.instance.CollisionController.fallDistance >= 8f)
		{
			float fallDistance = PlayerController.instance.CollisionController.fallDistance;
			float num = 5f;
			float num2 = 4f;
			if (fallDistance > num)
			{
				int damage = 5;
				float time = 0.5f;
				if (fallDistance > num + num2 * 4f)
				{
					damage = 100;
					time = 2f;
				}
				else if (fallDistance > num + num2 * 3f)
				{
					damage = 50;
					time = 2f;
				}
				else if (fallDistance > num + num2 * 2f)
				{
					damage = 25;
					time = 3f;
				}
				else if (fallDistance > num + num2)
				{
					damage = 15;
					time = 3f;
				}
				this.tumble.TumbleRequest(true, false);
				this.tumble.TumbleOverrideTime(time);
				if (SemiFunc.FPSImpulse15())
				{
					this.tumble.ImpactHurtSet(0.5f, damage);
				}
			}
		}
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0007F819 File Offset: 0x0007DA19
	public void SetState(bool crouching, bool sprinting, bool crawling, bool sliding, bool moving)
	{
		this.isCrouching = crouching;
		this.isSprinting = sprinting;
		this.isCrawling = crawling;
		this.isSliding = sliding;
		this.isMoving = moving;
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0007F840 File Offset: 0x0007DA40
	private void OverrideAnimationSpeedActivate(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		if (!this.isLocal)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("OverrideAnimationSpeedActivateRPC", RpcTarget.All, new object[]
			{
				active,
				_speedMulti,
				_in,
				_out,
				_time
			});
			return;
		}
		this.OverrideAnimationSpeedActivateRPC(active, _speedMulti, _in, _out, _time);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0007F8B1 File Offset: 0x0007DAB1
	[PunRPC]
	public void OverrideAnimationSpeedActivateRPC(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		this.overrideAnimationSpeedActive = active;
		this.overrrideAnimationSpeedTimer = _time;
		this.overrrideAnimationSpeedTarget = _speedMulti;
		this.overrrideAnimationSpeedIn = _in;
		this.overrrideAnimationSpeedOut = _out;
		this.overrideAnimationSpeedTime = _time;
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0007F8E0 File Offset: 0x0007DAE0
	public void OverrideAnimationSpeed(float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		float num = this.overrrideAnimationSpeedTarget;
		this.overrrideAnimationSpeedTimer = _time;
		this.overrrideAnimationSpeedTarget = _speedMulti;
		this.overrrideAnimationSpeedIn = _in;
		this.overrrideAnimationSpeedOut = _out;
		this.overrideAnimationSpeedTime = _time;
		if (SemiFunc.IsMultiplayer() && (!this.overrideAnimationSpeedActive || num != _speedMulti))
		{
			this.OverrideAnimationSpeedActivate(true, _speedMulti, _in, _out, _time);
		}
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0007F938 File Offset: 0x0007DB38
	private void OverrideAnimationSpeedTick()
	{
		if (this.overrrideAnimationSpeedTimer > 0f)
		{
			this.overrrideAnimationSpeedTimer -= Time.fixedDeltaTime;
			if (this.overrrideAnimationSpeedTimer <= 0f && SemiFunc.IsMultiplayer() && this.overrideAnimationSpeedActive)
			{
				this.OverrideAnimationSpeedActivate(false, this.overrrideAnimationSpeedTarget, this.overrrideAnimationSpeedIn, this.overrrideAnimationSpeedOut, this.overrideAnimationSpeedTime);
			}
		}
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0007F9A0 File Offset: 0x0007DBA0
	private void OverrideAnimationSpeedLogic()
	{
		if (!this.playerAvatarVisuals)
		{
			return;
		}
		if (this.overrrideAnimationSpeedTimer <= 0f && this.playerAvatarVisuals.animationSpeedMultiplier == 1f)
		{
			return;
		}
		if (!this.isLocal && this.overrideAnimationSpeedActive)
		{
			this.OverrideAnimationSpeed(this.overrrideAnimationSpeedTarget, this.overrrideAnimationSpeedIn, this.overrrideAnimationSpeedOut, this.overrideAnimationSpeedTime);
		}
		if (this.overrrideAnimationSpeedTimer > 0f)
		{
			this.overrideAnimationSpeedLerp = Mathf.Lerp(this.overrideAnimationSpeedLerp, 1f, Time.deltaTime * this.overrrideAnimationSpeedIn);
		}
		else
		{
			this.overrideAnimationSpeedLerp = Mathf.Lerp(this.overrideAnimationSpeedLerp, 0f, Time.deltaTime * this.overrrideAnimationSpeedOut);
		}
		this.playerAvatarVisuals.animationSpeedMultiplier = Mathf.Lerp(1f, this.overrrideAnimationSpeedTarget, this.overrideAnimationSpeedLerp);
		if (this.playerAvatarVisuals.animationSpeedMultiplier > 0.98f)
		{
			this.playerAvatarVisuals.animationSpeedMultiplier = 1f;
		}
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0007FAA0 File Offset: 0x0007DCA0
	private void OverridePupilSizeActivate(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		if (!this.isLocal)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("OverridePupilSizeActivateRPC", RpcTarget.All, new object[]
			{
				active,
				_multiplier,
				_prio,
				springSpeedIn,
				dampIn,
				springSpeedOut,
				dampOut,
				_time
			});
			return;
		}
		this.OverridePupilSizeActivateRPC(active, _multiplier, _prio, springSpeedIn, dampIn, springSpeedOut, dampOut, _time);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0007FB38 File Offset: 0x0007DD38
	[PunRPC]
	public void OverridePupilSizeActivateRPC(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		this.overridePupilSizeActive = active;
		this.overridePupilSizeMultiplier = _multiplier;
		this.overridePupilSizeMultiplierTarget = _multiplier;
		this.overridePupilSizePrio = _prio;
		this.overridePupilSpringSpeedIn = springSpeedIn;
		this.overridePupilSpringDampIn = dampIn;
		this.overridePupilSpringSpeedOut = springSpeedOut;
		this.overridePupilSpringDampOut = dampOut;
		this.overridePupilSizeTime = _time;
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0007FB8C File Offset: 0x0007DD8C
	public void OverridePupilSize(float _multiplier, int _prio, float springSpeedIn, float springDampIn, float springSpeedOut, float springDampOut, float _time = 0.1f)
	{
		if (this.overridePupilSizeTimer > 0f && _prio < this.overridePupilSizePrio)
		{
			return;
		}
		float num = this.overridePupilSizeMultiplierTarget;
		this.overridePupilSizeMultiplier = _multiplier;
		this.overridePupilSizeMultiplierTarget = _multiplier;
		this.overridePupilSizePrio = _prio;
		this.overridePupilSpringSpeedIn = springSpeedIn;
		this.overridePupilSpringDampIn = springDampIn;
		this.overridePupilSpringSpeedOut = springSpeedOut;
		this.overridePupilSpringDampOut = springDampOut;
		this.overridePupilSizeTime = _time;
		this.overridePupilSizeTimer = _time;
		if (SemiFunc.IsMultiplayer() && (!this.overridePupilSizeActive || num != _multiplier))
		{
			this.OverridePupilSizeActivate(true, _multiplier, _prio, springSpeedIn, springDampIn, springSpeedOut, springDampOut, _time);
		}
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0007FC20 File Offset: 0x0007DE20
	private void OverridePupilSizeTick()
	{
		if (this.overridePupilSizeTimer > 0f)
		{
			this.overridePupilSizeTimer -= Time.fixedDeltaTime;
			if (this.overridePupilSizeTimer <= 0f && SemiFunc.IsMultiplayer() && this.overridePupilSizeActive)
			{
				this.OverridePupilSizeActivate(false, this.overridePupilSizeMultiplierTarget, this.overridePupilSizePrio, this.overridePupilSpringSpeedIn, this.overridePupilSpringDampIn, this.overridePupilSpringSpeedOut, this.overridePupilSpringDampOut, this.overridePupilSizeTime);
			}
		}
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x0007FC9C File Offset: 0x0007DE9C
	private void OverridePupilSizeLogic()
	{
		if (!this.playerAvatarVisuals)
		{
			return;
		}
		if (!this.isLocal && this.overridePupilSizeActive)
		{
			this.OverridePupilSize(this.overridePupilSizeMultiplierTarget, this.overridePupilSizePrio, this.overridePupilSpringSpeedIn, this.overridePupilSpringDampIn, this.overridePupilSpringSpeedOut, this.overridePupilSpringDampOut, this.overridePupilSizeTime);
		}
		if (this.overridePupilSizeTimer > 0f)
		{
			this.overridePupilSizeSpring.speed = this.overridePupilSpringSpeedIn;
			this.overridePupilSizeSpring.damping = this.overridePupilSpringDampIn;
			this.playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(this.overridePupilSizeSpring, this.overridePupilSizeMultiplierTarget, -1f);
			return;
		}
		this.overridePupilSizeSpring.speed = this.overridePupilSpringSpeedOut;
		this.overridePupilSizeSpring.damping = this.overridePupilSpringDampOut;
		this.playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(this.overridePupilSizeSpring, 1f, -1f);
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0007FD94 File Offset: 0x0007DF94
	public void SetSpectate()
	{
		Object.Instantiate<GameObject>(this.spectateCamera).GetComponent<SpectateCamera>().SetDeath(this.spectatePoint);
		this.spectating = true;
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0007FDB8 File Offset: 0x0007DFB8
	public void SoundSetup(Sound _sound)
	{
		if (this.photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x0007FE0D File Offset: 0x0007E00D
	public void EnemyVisionFreezeTimerSet(float _time)
	{
		this.enemyVisionFreezeTimer = _time;
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0007FE16 File Offset: 0x0007E016
	public void FlashlightFlicker(float _multiplier)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FlashlightFlickerRPC", RpcTarget.All, new object[]
			{
				_multiplier
			});
			return;
		}
		this.FlashlightFlickerRPC(_multiplier);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0007FE47 File Offset: 0x0007E047
	[PunRPC]
	public void FlashlightFlickerRPC(float _multiplier)
	{
		this.flashlightController.FlickerSet(_multiplier);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0007FE58 File Offset: 0x0007E058
	public void Slide()
	{
		this.slideSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer())
		{
			Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 0f, true);
			return;
		}
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 0f, true);
		this.photonView.RPC("SlideRPC", RpcTarget.Others, Array.Empty<object>());
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0007FEF0 File Offset: 0x0007E0F0
	[PunRPC]
	private void SlideRPC()
	{
		this.slideSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 1f, false);
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0007FF49 File Offset: 0x0007E149
	public void Jump(bool _powerupEffect)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.JumpRPC(_powerupEffect);
			return;
		}
		this.photonView.RPC("JumpRPC", RpcTarget.All, new object[]
		{
			_powerupEffect
		});
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0007FF80 File Offset: 0x0007E180
	[PunRPC]
	private void JumpRPC(bool _powerupEffect)
	{
		this.playerAvatarVisuals.JumpImpulse();
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, true, this.MaterialTrigger, hostType);
		if (_powerupEffect)
		{
			this.extraJumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.playerAvatarVisuals.PowerupJumpEffect();
		}
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0008002B File Offset: 0x0007E22B
	public void Land()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.LandRPC();
			return;
		}
		this.photonView.RPC("LandRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00080058 File Offset: 0x0007E258
	[PunRPC]
	private void LandRPC()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 0.25f, SemiFunc.LayerMaskGetPhysGrabObject()))
		{
			PhysGrabObject component = raycastHit.transform.GetComponent<PhysGrabObject>();
			if (component)
			{
				component.mediumBreakImpulse = true;
				return;
			}
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 10f);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, true, this.MaterialTrigger, hostType);
		Vector3 position = this.PlayerVisionTarget.VisionTransform.position;
		if (this.isLocal)
		{
			position = this.localCameraPosition;
		}
		SemiFunc.PlayerEyesOverrideSoft(position, 2f, base.gameObject, 5f);
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0008017C File Offset: 0x0007E37C
	public void Footstep(Materials.SoundType soundType)
	{
		if (RecordingDirector.instance)
		{
			return;
		}
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!this.isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(base.transform.position, Vector3.down, soundType, true, this.MaterialTrigger, hostType);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (soundType == Materials.SoundType.Heavy)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 5f);
				return;
			}
			if (soundType == Materials.SoundType.Medium)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position + Vector3.up * 0.2f, 1f);
			}
		}
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x00080234 File Offset: 0x0007E434
	public void StandToCrouch()
	{
		if (this.isSprinting)
		{
			return;
		}
		this.standToCrouchSound.Play(base.transform.position, 1f, 1f, 1f, 1f).pitch *= this.playerAvatarVisuals.animationSpeedMultiplier;
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0008028B File Offset: 0x0007E48B
	private float GetPitchMulti()
	{
		return Mathf.Clamp(this.playerAvatarVisuals.animationSpeedMultiplier, 0.5f, 1.5f);
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x000802A8 File Offset: 0x0007E4A8
	public void CrouchToStand()
	{
		AudioSource audioSource = this.crouchToStandSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x000802F4 File Offset: 0x0007E4F4
	public void CrouchToCrawl()
	{
		if (this.isSliding || this.isSprinting)
		{
			return;
		}
		AudioSource audioSource = this.crouchToCrawlSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00080350 File Offset: 0x0007E550
	public void CrawlToCrouch()
	{
		if (this.isSliding || this.isSprinting)
		{
			return;
		}
		AudioSource audioSource = this.crawlToCrouchSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x000803AC File Offset: 0x0007E5AC
	public void TumbleStart()
	{
		AudioSource audioSource = this.tumbleStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x000803F8 File Offset: 0x0007E5F8
	public void TumbleStop()
	{
		AudioSource audioSource = this.tumbleStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		float pitchMulti = this.GetPitchMulti();
		audioSource.pitch *= pitchMulti;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00080444 File Offset: 0x0007E644
	public void TumbleBreakFree()
	{
		this.tumbleBreakFreeSound.Play(base.transform.position, 1f, 1f, 1f, 1f).pitch *= this.GetPitchMulti();
		this.playerAvatarVisuals.TumbleBreakFreeEffect();
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00080498 File Offset: 0x0007E698
	public void PlayerGlitchShort()
	{
		if (GameManager.instance.gameMode == 0)
		{
			CameraGlitch.Instance.PlayShort();
			return;
		}
		this.photonView.RPC("PlayerGlitchShortRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x000804C7 File Offset: 0x0007E6C7
	[PunRPC]
	private void PlayerGlitchShortRPC()
	{
		if (this.photonView.IsMine)
		{
			CameraGlitch.Instance.PlayShort();
		}
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x000804E0 File Offset: 0x0007E6E0
	public void Spawn(Vector3 position, Quaternion rotation)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.SpawnRPC(position, rotation);
			return;
		}
		this.photonView.RPC("SpawnRPC", RpcTarget.All, new object[]
		{
			position,
			rotation
		});
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00080520 File Offset: 0x0007E720
	[PunRPC]
	private void SpawnRPC(Vector3 position, Quaternion rotation)
	{
		if (!this.photonView)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
		if (!this.rb)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			PlayerController.instance.transform.position = position;
			PlayerController.instance.transform.rotation = rotation;
		}
		this.rb.position = position;
		this.rb.rotation = rotation;
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.clientPosition = position;
		this.clientPositionCurrent = position;
		this.clientRotation = rotation;
		this.clientRotationCurrent = rotation;
		this.spawnPosition = position;
		this.spawnRotation = rotation;
		this.playerAvatarVisuals.visualPosition = position;
		this.spawned = true;
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00080600 File Offset: 0x0007E800
	public void PlayerDeath(int enemyIndex)
	{
		if (this.deadSet)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.PlayerDeathRPC(enemyIndex);
			return;
		}
		this.photonView.RPC("PlayerDeathRPC", RpcTarget.All, new object[]
		{
			enemyIndex
		});
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00080640 File Offset: 0x0007E840
	[PunRPC]
	public void PlayerDeathRPC(int enemyIndex)
	{
		this.playerHealth.Death();
		this.deadSet = true;
		if (!this.isLocal)
		{
			this.deathBuildupSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		if (this.isLocal)
		{
			this.deadEnemyLookAtTransform = null;
			Enemy enemy = SemiFunc.EnemyGetFromIndex(enemyIndex);
			if (enemy)
			{
				if (enemy.KillLookAtTransform)
				{
					this.deadEnemyLookAtTransform = enemy.KillLookAtTransform;
				}
				else
				{
					Debug.LogError("Enemy has no kill look at transform..." + enemy.name);
				}
			}
			this.physGrabber.ReleaseObject(0.1f);
			if (this.playerTransform)
			{
				this.playerTransform.parent.gameObject.SetActive(false);
			}
			CameraGlitch.Instance.PlayLongHurt();
			GameDirector.instance.DeathStart();
		}
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x0008072C File Offset: 0x0007E92C
	private void PlayerDeathDone()
	{
		if (SemiFunc.RunIsTutorial())
		{
			TutorialDirector.instance.deadPlayer = true;
		}
		if (this.isDisabled)
		{
			return;
		}
		this.isDisabled = true;
		if (GameManager.Multiplayer())
		{
			if (!this.isLocal)
			{
				if (SpectateCamera.instance)
				{
					SpectateCamera.instance.UpdatePlayer(this);
				}
			}
			else
			{
				this.physGrabber.ReleaseObject(0.1f);
				if (SemiFunc.IsMultiplayer())
				{
					if (!SemiFunc.RunIsArena() && Inventory.instance.physGrabber.photonView.ViewID == this.physGrabber.photonView.ViewID)
					{
						Inventory.instance.ForceUnequip();
					}
				}
				else
				{
					Inventory.instance.ForceUnequip();
				}
			}
		}
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.playerDeathHead.Trigger();
		this.playerDeathEffects.Trigger();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0008082A File Offset: 0x0007EA2A
	public void OutroStart()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.OutroStartRPC();
			return;
		}
		this.photonView.RPC("OutroStartRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00080855 File Offset: 0x0007EA55
	[PunRPC]
	public void OutroStartRPC()
	{
		if (this.isLocal)
		{
			GameDirector.instance.OutroStart();
		}
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0008086C File Offset: 0x0007EA6C
	public void OutroDone()
	{
		if (this.quitApplication)
		{
			Application.Quit();
			return;
		}
		if (NetworkManager.instance.leavePhotonRoom)
		{
			NetworkManager.instance.LeavePhotonRoom();
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.OutroDoneRPC();
			return;
		}
		this.photonView.RPC("OutroDoneRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x000808C7 File Offset: 0x0007EAC7
	[PunRPC]
	public void OutroDoneRPC()
	{
		this.outroDone = true;
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x000808D0 File Offset: 0x0007EAD0
	public void ForceImpulse(Vector3 _force)
	{
		if (!GameManager.Multiplayer())
		{
			this.ForceImpulseRPC(_force);
			return;
		}
		this.photonView.RPC("ForceImpulseRPC", RpcTarget.All, new object[]
		{
			_force
		});
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00080901 File Offset: 0x0007EB01
	[PunRPC]
	private void ForceImpulseRPC(Vector3 _force)
	{
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			PlayerController.instance.ForceImpulse(_force);
		}
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00080922 File Offset: 0x0007EB22
	public void PlayerAvatarSetColor(int colorIndex)
	{
		if (!GameManager.Multiplayer())
		{
			this.SetColorRPC(colorIndex);
			return;
		}
		this.photonView.RPC("SetColorRPC", RpcTarget.AllBuffered, new object[]
		{
			colorIndex
		});
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x00080954 File Offset: 0x0007EB54
	[PunRPC]
	private void SetColorRPC(int colorIndex)
	{
		if (this.isLocal)
		{
			DataDirector.instance.ColorSetBody(colorIndex);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetPlayerColor(this.steamID, colorIndex);
		}
		this.playerAvatarVisuals.SetColor(colorIndex, default(Color));
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x000809A1 File Offset: 0x0007EBA1
	public void Revive(bool _revivedByTruck = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ReviveRPC(_revivedByTruck);
			return;
		}
		this.photonView.RPC("ReviveRPC", RpcTarget.All, new object[]
		{
			_revivedByTruck
		});
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x000809D8 File Offset: 0x0007EBD8
	[PunRPC]
	public void ReviveRPC(bool _revivedByTruck)
	{
		if (!this.playerDeathHead)
		{
			Debug.LogError("Tried to revive without death head...");
			return;
		}
		TutorialDirector.instance.playerRevived = true;
		if (_revivedByTruck)
		{
			TruckHealer.instance.Heal(this);
		}
		Vector3 position = this.playerDeathHead.physGrabObject.centerPoint - Vector3.up * 0.25f;
		Vector3 eulerAngles = this.playerDeathHead.physGrabObject.transform.eulerAngles;
		if (SemiFunc.RunIsTutorial())
		{
			position = Vector3.zero + Vector3.up * 2f - Vector3.right * 5f;
			this.playerDeathHead.transform.position = position;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.tumble.physGrabObject.Teleport(position, base.transform.rotation);
		}
		base.transform.position = position;
		this.clientPositionCurrent = base.transform.position;
		this.clientPosition = base.transform.position;
		this.clientPhysRiding = false;
		base.gameObject.SetActive(true);
		this.playerAvatarVisuals.gameObject.SetActive(true);
		this.playerAvatarVisuals.transform.position = base.transform.position;
		this.playerAvatarVisuals.visualPosition = base.transform.position;
		this.playerAvatarVisuals.Revive();
		this.isDisabled = false;
		this.playerDeathHead.Reset();
		this.playerDeathEffects.Reset();
		this.playerReviveEffects.Trigger();
		this.deadSet = false;
		this.deadTimer = this.deadTime;
		this.deadVoiceTimer = this.deadVoiceTime;
		if (this.voiceChat)
		{
			this.voiceChat.ToggleLobby(false);
		}
		this.playerAvatarCollision.SetCrouch();
		this.playerHealth.SetMaterialGreen();
		if (this.isLocal)
		{
			this.playerHealth.HealOther(1, true);
			this.playerTransform.position = base.transform.position;
			this.playerTransform.parent.gameObject.SetActive(true);
			CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
			GameDirector.instance.Revive();
			SpectateCamera.instance.StopSpectate();
			PlayerController.instance.Revive(eulerAngles);
			CameraGlitch.Instance.PlayLongHeal();
		}
		else if (!_revivedByTruck && SemiFunc.RunIsLevel())
		{
			PlayerAvatar playerAvatarScript = PlayerController.instance.playerAvatarScript;
			if (!playerAvatarScript.isDisabled && Vector3.Distance(playerAvatarScript.transform.position, base.transform.position) < 10f && playerAvatarScript.playerHealth.health >= 50 && !TutorialDirector.instance.playerHealed && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialHealing, 1))
			{
				TutorialDirector.instance.ActivateTip("Healing", 0.5f, false);
			}
		}
		this.RoomVolumeCheck.CheckSet();
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x00080CC4 File Offset: 0x0007EEC4
	private void FallDamageResetLogic()
	{
		if (this.fallDamageResetTimer > 0f)
		{
			this.fallDamageResetTimer -= Time.deltaTime;
			this.fallDamageResetState = true;
		}
		else
		{
			this.fallDamageResetState = false;
		}
		if (this.fallDamageResetState != this.fallDamageResetStatePrevious)
		{
			this.fallDamageResetStatePrevious = this.fallDamageResetState;
			if (!GameManager.Multiplayer())
			{
				this.FallDamageResetUpdateRPC(this.fallDamageResetState);
				return;
			}
			this.photonView.RPC("FallDamageResetUpdateRPC", RpcTarget.All, new object[]
			{
				this.fallDamageResetState
			});
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x00080D53 File Offset: 0x0007EF53
	public void FallDamageResetSet(float _time)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.fallDamageResetTimer = _time;
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00080D63 File Offset: 0x0007EF63
	[PunRPC]
	private void FallDamageResetUpdateRPC(bool _state)
	{
		this.fallDamageResetState = _state;
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x00080D6C File Offset: 0x0007EF6C
	private void ChatMessageSpeak(string _message, bool crouching)
	{
		if (!this.voiceChat)
		{
			return;
		}
		if (!this.voiceChat.ttsVoice)
		{
			return;
		}
		this.voiceChat.ttsVoice.TTSSpeakNow(_message, crouching);
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00080DA4 File Offset: 0x0007EFA4
	public void ChatMessageSend(string _message, bool _debugMessage)
	{
		if (!_debugMessage)
		{
			using (List<PlayerVoiceChat>.Enumerator enumerator = RunManager.instance.voiceChats.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.recordingEnabled)
					{
						return;
					}
				}
			}
		}
		bool flag = this.isCrouching;
		SemiFunc.Command(_message);
		if (!SemiFunc.IsMultiplayer())
		{
			this.ChatMessageSpeak(_message, flag);
			return;
		}
		if (this.isDisabled)
		{
			flag = true;
		}
		this.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, new object[]
		{
			_message,
			flag
		});
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00080E4C File Offset: 0x0007F04C
	[PunRPC]
	public void ChatMessageSendRPC(string _message, bool crouching)
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		this.ChatMessageSpeak(_message, crouching);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00080E64 File Offset: 0x0007F064
	public void LoadingLevelAnimationCompleted()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("LoadingLevelAnimationCompletedRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.LoadingLevelAnimationCompletedRPC();
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00080E8A File Offset: 0x0007F08A
	[PunRPC]
	public void LoadingLevelAnimationCompletedRPC()
	{
		this.levelAnimationCompleted = true;
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00080E93 File Offset: 0x0007F093
	public void HealedOther()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("HealedOtherRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00080EB2 File Offset: 0x0007F0B2
	[PunRPC]
	public void HealedOtherRPC()
	{
		if (this.isLocal)
		{
			TutorialDirector.instance.playerHealed = true;
		}
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00080EC8 File Offset: 0x0007F0C8
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.isCrouching);
			stream.SendNext(this.isSprinting);
			stream.SendNext(this.isCrawling);
			stream.SendNext(this.isSliding);
			stream.SendNext(this.isMoving);
			stream.SendNext(this.isGrounded);
			stream.SendNext(this.Interact);
			stream.SendNext(this.InputDirection);
			stream.SendNext(PlayerController.instance.VelocityRelative);
			stream.SendNext(this.rbVelocityRaw);
			stream.SendNext(PlayerController.instance.transform.position);
			stream.SendNext(PlayerController.instance.transform.rotation);
			stream.SendNext(this.localCameraPosition);
			stream.SendNext(this.localCameraRotation);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRiding);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingID);
			stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingPosition);
			stream.SendNext(this.flashlightLightAim.clientAimPoint);
			stream.SendNext(this.playerPing);
			return;
		}
		this.isCrouching = (bool)stream.ReceiveNext();
		this.isSprinting = (bool)stream.ReceiveNext();
		this.isCrawling = (bool)stream.ReceiveNext();
		this.isSliding = (bool)stream.ReceiveNext();
		this.isMoving = (bool)stream.ReceiveNext();
		this.isGrounded = (bool)stream.ReceiveNext();
		this.Interact = (bool)stream.ReceiveNext();
		this.InputDirection = (Vector3)stream.ReceiveNext();
		this.rbVelocity = (Vector3)stream.ReceiveNext();
		this.rbVelocityRaw = (Vector3)stream.ReceiveNext();
		this.clientPosition = (Vector3)stream.ReceiveNext();
		this.clientRotation = (Quaternion)stream.ReceiveNext();
		this.clientPositionDelta = Vector3.Distance(this.clientPositionCurrent, this.clientPosition);
		this.clientRotationDelta = Quaternion.Angle(this.clientRotationCurrent, this.clientRotation);
		this.localCameraPosition = (Vector3)stream.ReceiveNext();
		this.localCameraRotation = (Quaternion)stream.ReceiveNext();
		this.clientPhysRiding = (bool)stream.ReceiveNext();
		this.clientPhysRidingID = (int)stream.ReceiveNext();
		this.clientPhysRidingPosition = (Vector3)stream.ReceiveNext();
		if (this.clientPhysRiding)
		{
			PhotonView photonView = PhotonView.Find(this.clientPhysRidingID);
			if (photonView)
			{
				this.clientPhysRidingTransform = photonView.transform;
			}
			else
			{
				this.clientPhysRiding = false;
			}
		}
		this.playerAvatarVisuals.PhysRidingCheck();
		this.flashlightLightAim.clientAimPoint = (Vector3)stream.ReceiveNext();
		this.playerPing = (int)stream.ReceiveNext();
	}

	// Token: 0x040016FD RID: 5885
	public PhotonView photonView;

	// Token: 0x040016FE RID: 5886
	public Transform playerTransform;

	// Token: 0x040016FF RID: 5887
	public Transform lowPassRaycastPoint;

	// Token: 0x04001700 RID: 5888
	public GameObject spectateCamera;

	// Token: 0x04001701 RID: 5889
	public Transform spectatePoint;

	// Token: 0x04001702 RID: 5890
	public PhysGrabber physGrabber;

	// Token: 0x04001703 RID: 5891
	public PlayerPhysPusher playerPhysPusher;

	// Token: 0x04001704 RID: 5892
	public PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001705 RID: 5893
	public PlayerHealth playerHealth;

	// Token: 0x04001706 RID: 5894
	public FlashlightController flashlightController;

	// Token: 0x04001707 RID: 5895
	public FlashlightLightAim flashlightLightAim;

	// Token: 0x04001708 RID: 5896
	public MapToolController mapToolController;

	// Token: 0x04001709 RID: 5897
	public PlayerDeathEffects playerDeathEffects;

	// Token: 0x0400170A RID: 5898
	public PlayerReviveEffects playerReviveEffects;

	// Token: 0x0400170B RID: 5899
	public PlayerDeathHead playerDeathHead;

	// Token: 0x0400170C RID: 5900
	public PlayerHealthGrab healthGrab;

	// Token: 0x0400170D RID: 5901
	public PlayerTumble tumble;

	// Token: 0x0400170E RID: 5902
	public PlayerPhysObjectStander physObjectStander;

	// Token: 0x0400170F RID: 5903
	private Collider collider;

	// Token: 0x04001710 RID: 5904
	internal string playerName;

	// Token: 0x04001711 RID: 5905
	internal string steamID;

	// Token: 0x04001712 RID: 5906
	[Space]
	public Transform localCameraTransform;

	// Token: 0x04001713 RID: 5907
	private Camera localCamera;

	// Token: 0x04001714 RID: 5908
	internal Vector3 localCameraPosition = Vector3.zero;

	// Token: 0x04001715 RID: 5909
	internal Quaternion localCameraRotation = Quaternion.identity;

	// Token: 0x04001716 RID: 5910
	[Space]
	public PlayerVisionTarget PlayerVisionTarget;

	// Token: 0x04001717 RID: 5911
	public RoomVolumeCheck RoomVolumeCheck;

	// Token: 0x04001718 RID: 5912
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x04001719 RID: 5913
	[Space]
	internal bool isLocal;

	// Token: 0x0400171A RID: 5914
	internal bool isDisabled;

	// Token: 0x0400171B RID: 5915
	internal bool outroDone;

	// Token: 0x0400171C RID: 5916
	internal bool spawned;

	// Token: 0x0400171D RID: 5917
	private bool spawnImpulse = true;

	// Token: 0x0400171E RID: 5918
	private int spawnFrames = 3;

	// Token: 0x0400171F RID: 5919
	private bool spawnDoneImpulse = true;

	// Token: 0x04001720 RID: 5920
	private Vector3 spawnPosition;

	// Token: 0x04001721 RID: 5921
	internal Quaternion spawnRotation;

	// Token: 0x04001722 RID: 5922
	internal bool finalHeal;

	// Token: 0x04001723 RID: 5923
	internal bool isCrouching;

	// Token: 0x04001724 RID: 5924
	internal bool isSprinting;

	// Token: 0x04001725 RID: 5925
	internal bool isCrawling;

	// Token: 0x04001726 RID: 5926
	internal bool isSliding;

	// Token: 0x04001727 RID: 5927
	internal bool isMoving;

	// Token: 0x04001728 RID: 5928
	internal bool isGrounded;

	// Token: 0x04001729 RID: 5929
	internal bool isTumbling;

	// Token: 0x0400172A RID: 5930
	private bool Interact;

	// Token: 0x0400172B RID: 5931
	internal Vector3 InputDirection;

	// Token: 0x0400172C RID: 5932
	internal Vector3 LastNavmeshPosition;

	// Token: 0x0400172D RID: 5933
	internal float LastNavMeshPositionTimer;

	// Token: 0x0400172E RID: 5934
	internal PlayerVoiceChat voiceChat;

	// Token: 0x0400172F RID: 5935
	internal bool voiceChatFetched;

	// Token: 0x04001730 RID: 5936
	private Rigidbody rb;

	// Token: 0x04001731 RID: 5937
	internal Vector3 rbVelocity;

	// Token: 0x04001732 RID: 5938
	internal Vector3 rbVelocityRaw;

	// Token: 0x04001733 RID: 5939
	private float rbDiscreteTimer;

	// Token: 0x04001734 RID: 5940
	internal Vector3 clientPosition = Vector3.zero;

	// Token: 0x04001735 RID: 5941
	internal Vector3 clientPositionCurrent = Vector3.zero;

	// Token: 0x04001736 RID: 5942
	internal float clientPositionDelta;

	// Token: 0x04001737 RID: 5943
	internal Quaternion clientRotation = Quaternion.identity;

	// Token: 0x04001738 RID: 5944
	internal Quaternion clientRotationCurrent = Quaternion.identity;

	// Token: 0x04001739 RID: 5945
	private float clientRotationDelta;

	// Token: 0x0400173A RID: 5946
	public Sound jumpSound;

	// Token: 0x0400173B RID: 5947
	public Sound extraJumpSound;

	// Token: 0x0400173C RID: 5948
	public Sound landSound;

	// Token: 0x0400173D RID: 5949
	public Sound slideSound;

	// Token: 0x0400173E RID: 5950
	[Space]
	public Sound standToCrouchSound;

	// Token: 0x0400173F RID: 5951
	public Sound crouchToStandSound;

	// Token: 0x04001740 RID: 5952
	[Space]
	public Sound crouchToCrawlSound;

	// Token: 0x04001741 RID: 5953
	public Sound crawlToCrouchSound;

	// Token: 0x04001742 RID: 5954
	[Space]
	public Sound deathBuildupSound;

	// Token: 0x04001743 RID: 5955
	public Sound deathSound;

	// Token: 0x04001744 RID: 5956
	[Space]
	public Sound tumbleStartSound;

	// Token: 0x04001745 RID: 5957
	public Sound tumbleStopSound;

	// Token: 0x04001746 RID: 5958
	public Sound tumbleBreakFreeSound;

	// Token: 0x04001747 RID: 5959
	[Space]
	public Sound truckReturn;

	// Token: 0x04001748 RID: 5960
	public Sound truckReturnGlobal;

	// Token: 0x04001749 RID: 5961
	internal bool clientPhysRiding;

	// Token: 0x0400174A RID: 5962
	internal int clientPhysRidingID;

	// Token: 0x0400174B RID: 5963
	internal Vector3 clientPhysRidingPosition;

	// Token: 0x0400174C RID: 5964
	internal Transform clientPhysRidingTransform;

	// Token: 0x0400174D RID: 5965
	public static PlayerAvatar instance;

	// Token: 0x0400174E RID: 5966
	internal bool spectating;

	// Token: 0x0400174F RID: 5967
	internal bool deadSet;

	// Token: 0x04001750 RID: 5968
	private float deadTime = 0.5f;

	// Token: 0x04001751 RID: 5969
	private float deadTimer;

	// Token: 0x04001752 RID: 5970
	private float deadVoiceTime = 0.1f;

	// Token: 0x04001753 RID: 5971
	private float deadVoiceTimer;

	// Token: 0x04001754 RID: 5972
	internal float enemyVisionFreezeTimer;

	// Token: 0x04001755 RID: 5973
	private Transform deadEnemyLookAtTransform;

	// Token: 0x04001756 RID: 5974
	internal int steamIDshort;

	// Token: 0x04001757 RID: 5975
	internal PlayerAvatarCollision playerAvatarCollision;

	// Token: 0x04001758 RID: 5976
	internal bool fallDamageResetState;

	// Token: 0x04001759 RID: 5977
	private bool fallDamageResetStatePrevious;

	// Token: 0x0400175A RID: 5978
	private float fallDamageResetTimer;

	// Token: 0x0400175B RID: 5979
	internal int playerPing;

	// Token: 0x0400175C RID: 5980
	private float playerPingTimer;

	// Token: 0x0400175D RID: 5981
	internal bool quitApplication;

	// Token: 0x0400175E RID: 5982
	private float overrrideAnimationSpeedTimer;

	// Token: 0x0400175F RID: 5983
	private float overrrideAnimationSpeedTarget;

	// Token: 0x04001760 RID: 5984
	private float overrrideAnimationSpeedIn;

	// Token: 0x04001761 RID: 5985
	private float overrrideAnimationSpeedOut;

	// Token: 0x04001762 RID: 5986
	private float overrideAnimationSpeedLerp;

	// Token: 0x04001763 RID: 5987
	private bool overrideAnimationSpeedActive;

	// Token: 0x04001764 RID: 5988
	private float overrideAnimationSpeedTime;

	// Token: 0x04001765 RID: 5989
	private SpringFloat overridePupilSizeSpring = new SpringFloat();

	// Token: 0x04001766 RID: 5990
	private bool overridePupilSizeActive;

	// Token: 0x04001767 RID: 5991
	private float overridePupilSizeTimer;

	// Token: 0x04001768 RID: 5992
	private float overridePupilSizeTime;

	// Token: 0x04001769 RID: 5993
	private float overridePupilSizeMultiplier = 1f;

	// Token: 0x0400176A RID: 5994
	private float overridePupilSizeMultiplierTarget = 1f;

	// Token: 0x0400176B RID: 5995
	private float overridePupilSpringSpeedIn = 15f;

	// Token: 0x0400176C RID: 5996
	private float overridePupilSpringDampIn = 0.3f;

	// Token: 0x0400176D RID: 5997
	private float overridePupilSpringSpeedOut = 15f;

	// Token: 0x0400176E RID: 5998
	private float overridePupilSpringDampOut = 0.3f;

	// Token: 0x0400176F RID: 5999
	private int overridePupilSizePrio;

	// Token: 0x04001770 RID: 6000
	internal int upgradeMapPlayerCount;

	// Token: 0x04001771 RID: 6001
	internal bool levelAnimationCompleted;

	// Token: 0x04001772 RID: 6002
	internal WorldSpaceUIPlayerName worldSpaceUIPlayerName;
}
