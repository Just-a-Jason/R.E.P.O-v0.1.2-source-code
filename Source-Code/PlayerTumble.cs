using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class PlayerTumble : MonoBehaviour
{
	// Token: 0x06000F09 RID: 3849 RVA: 0x000890AD File Offset: 0x000872AD
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x000890D3 File Offset: 0x000872D3
	private void Start()
	{
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x000890EA File Offset: 0x000872EA
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("SetupRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.playerAvatar.playerName
				});
			}
			this.SetupDone();
		}
		yield break;
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x000890FC File Offset: 0x000872FC
	private void SetupDone()
	{
		Collider[] array = this.colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		this.playerAvatar.SoundSetup(this.tumbleLaunchSound);
		base.transform.parent = this.playerAvatar.transform.parent;
		this.setup = true;
		string key = SemiFunc.PlayerGetSteamID(this.playerAvatar);
		if (StatsManager.instance.playerUpgradeLaunch.ContainsKey(key))
		{
			this.tumbleLaunch = StatsManager.instance.playerUpgradeLaunch[SemiFunc.PlayerGetSteamID(this.playerAvatar)];
		}
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00089198 File Offset: 0x00087398
	[PunRPC]
	public void SetupRPC(string _playerName)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.playerName == _playerName)
			{
				this.playerAvatar = playerAvatar;
				this.playerAvatar.tumble = this;
				break;
			}
		}
		this.SetupDone();
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x00089214 File Offset: 0x00087414
	private void Update()
	{
		if (SemiFunc.RunIsLobbyMenu() || !this.physGrabObject.spawned)
		{
			return;
		}
		if (this.isTumbling)
		{
			this.rb.isKinematic = false;
		}
		else
		{
			this.rb.isKinematic = true;
		}
		if (!this.isTumbling && this.playerAvatar)
		{
			Vector3 position = this.playerAvatar.transform.position + Vector3.up * 0.3f;
			Quaternion rotation = this.playerAvatar.transform.rotation;
			this.rb.MovePosition(position);
			this.rb.MoveRotation(rotation);
		}
		if (this.tumbleSetTimer > 0f)
		{
			this.tumbleSetTimer -= Time.deltaTime;
		}
		if (this.tumbleMoveSoundTimer > 0f)
		{
			this.tumbleMoveSoundTimer -= Time.deltaTime;
			this.tumbleMoveSound.PlayLoop(true, 1f, 1f, this.tumbleMoveSoundSpeed);
		}
		else
		{
			this.tumbleMoveSound.PlayLoop(false, 1f, 1f, this.tumbleMoveSoundSpeed);
		}
		if (this.isTumbling && this.playerAvatar.isLocal)
		{
			CameraZoom.Instance.OverrideZoomSet(55f, 0.1f, 1f, 1f, base.gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.6f, 0.2f, 2f, 2f, 0.1f, base.gameObject);
		}
		bool flag = false;
		if (this.isTumbling)
		{
			Vector3 rbVelocity = this.physGrabObject.rbVelocity;
			if (rbVelocity.magnitude > 4f && !this.physGrabObject.impactDetector.inCart)
			{
				flag = true;
				this.hurtCollider.transform.LookAt(this.hurtCollider.transform.position + rbVelocity);
				if (this.physGrabObject.playerGrabbing.Count == 0 && this.overrideEnemyHurtTimer <= 0f)
				{
					this.hurtCollider.enemyLogic = true;
				}
				else
				{
					this.hurtCollider.enemyLogic = false;
				}
				if (this.playerAvatar.isLocal)
				{
					this.hurtCollider.playerLogic = false;
				}
			}
		}
		if (this.hurtColliderPauseTimer > 0f)
		{
			flag = false;
			this.hurtColliderPauseTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (!this.hurtCollider.gameObject.activeSelf)
			{
				this.hurtCollider.gameObject.SetActive(true);
			}
		}
		else if (this.hurtCollider.gameObject.activeSelf)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
		if (this.overrideEnemyHurtTimer > 0f)
		{
			this.overrideEnemyHurtTimer -= Time.deltaTime;
		}
		if (this.isTumbling)
		{
			if ((Vector3.Distance(this.notMovingPositionLast, base.transform.position) <= 0.5f || this.physGrabObject.impactDetector.inCart) && this.physGrabObject.playerGrabbing.Count <= 0)
			{
				this.notMovingTimer += Time.deltaTime;
			}
			else
			{
				this.notMovingTimer = 0f;
				this.notMovingPositionLast = base.transform.position;
			}
		}
		else
		{
			this.notMovingTimer = 0f;
			this.notMovingPositionLast = base.transform.position;
		}
		if (this.breakFreeCooldown <= 0f)
		{
			if (this.physGrabObject.playerGrabbing.Count > 0 && this.playerAvatar.isLocal && SemiFunc.InputDown(InputKey.Jump))
			{
				this.breakFreeCooldown = 0.5f;
				this.TumbleForce(this.playerAvatar.localCameraTransform.forward * 15f);
				this.TumbleTorque(base.transform.right * 10f);
				this.BreakFree(this.playerAvatar.localCameraTransform.forward);
			}
		}
		else
		{
			this.breakFreeCooldown -= Time.deltaTime;
		}
		if (this.impactHurtTimer > 0f)
		{
			this.impactHurtTimer -= Time.deltaTime;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.TumbleOverrideTime(1f);
		}
		if (this.tumbleOverrideTimer > 0f)
		{
			this.tumbleOverrideTimer -= Time.deltaTime;
			this.tumbleOverride = true;
		}
		else
		{
			this.tumbleOverride = false;
		}
		if (this.tumbleOverride != this.tumbleOverridePrevious)
		{
			if (this.tumbleOverride)
			{
				this.TumbleOverride(true);
			}
			else
			{
				this.TumbleOverride(false);
			}
			this.tumbleOverridePrevious = this.tumbleOverride;
		}
		if (this.isTumbling && this.playerAvatar.isDisabled)
		{
			this.TumbleRequest(false, false);
		}
		if (this.isTumbling != this.isTumblingPrevious)
		{
			if (this.isTumbling)
			{
				this.SetPosition();
				Vector3 rbVelocityRaw = this.playerAvatar.rbVelocityRaw;
				this.rb.AddForce(rbVelocityRaw, ForceMode.VelocityChange);
				Vector3 a = Vector3.Cross(Vector3.up, rbVelocityRaw);
				if (a.magnitude <= 0f)
				{
					a = Random.insideUnitSphere.normalized * 1f;
				}
				this.rb.AddTorque(a * 2f, ForceMode.VelocityChange);
			}
			this.isTumblingPrevious = this.isTumbling;
		}
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x00089784 File Offset: 0x00087984
	private void FixedUpdate()
	{
		if (!this.isTumbling || (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		if (this.isTumbling && this.playerAvatar.playerHealth.hurtFreeze && this.playerAvatar.deadSet)
		{
			this.physGrabObject.FreezeForces(0.1f, Vector3.zero, Vector3.zero);
			return;
		}
		if (this.customGravityOverrideTimer > 0f)
		{
			this.customGravityOverrideTimer -= Time.fixedDeltaTime;
		}
		if (this.rb.useGravity && this.physGrabObject.playerGrabbing.Count <= 0 && this.customGravityOverrideTimer <= 0f)
		{
			this.rb.AddForce(-Vector3.up * this.customGravity, ForceMode.Force);
		}
		if (this.tumbleForceTimer > 0f)
		{
			this.tumbleForceTimer -= Time.fixedDeltaTime;
		}
		if (this.tumbleForceTimer <= 0f && !this.playerAvatar.playerHealth.hurtFreeze)
		{
			if (this.tumbleForce.magnitude > 0f)
			{
				this.rb.AddForce(this.tumbleForce, ForceMode.Impulse);
				this.tumbleForce = Vector3.zero;
			}
			if (this.tumbleTorque.magnitude > 0f)
			{
				this.rb.AddTorque(this.tumbleTorque, ForceMode.Impulse);
				this.tumbleTorque = Vector3.zero;
			}
		}
		if (this.notMovingTimer > 2f)
		{
			this.lookAtLerp += 0.5f * Time.fixedDeltaTime;
			this.lookAtLerp = Mathf.Clamp01(this.lookAtLerp);
			Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.playerAvatar.localCameraRotation, this.rb, 5f);
			vector = Vector3.Lerp(Vector3.zero, vector, 3f * Time.fixedDeltaTime);
			vector = Vector3.Lerp(Vector3.zero, vector, this.lookAtLerp);
			this.rb.AddTorque(vector, ForceMode.Impulse);
			return;
		}
		this.lookAtLerp = 0f;
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00089990 File Offset: 0x00087B90
	public void DisableCustomGravity(float _time)
	{
		this.customGravityOverrideTimer = _time;
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x00089999 File Offset: 0x00087B99
	private void SetPosition()
	{
		this.rb.isKinematic = false;
		this.tumbleForceTimer = 0.1f;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x000899D2 File Offset: 0x00087BD2
	public void OverrideEnemyHurt(float _time)
	{
		this.overrideEnemyHurtTimer = _time;
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x000899DC File Offset: 0x00087BDC
	public void HitEnemy()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.playerAvatar.isLocal)
			{
				this.playerAvatar.playerHealth.Hurt(5, true, -1);
				return;
			}
			this.playerAvatar.playerHealth.HurtOther(5, base.transform.position, true, -1);
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x00089A30 File Offset: 0x00087C30
	public void TumbleImpact()
	{
		if (this.playerAvatar.isLocal)
		{
			PlayerController.instance.CollisionController.StopFallLoop();
		}
		if (this.hurtColliderPauseTimer > 0f)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer() && !this.hurtCollider.onImpactPlayerAvatar.photonView.IsMine)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("TumbleImpactRPC", RpcTarget.All, new object[]
			{
				this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID
			});
			return;
		}
		this.TumbleImpactRPC(this.hurtCollider.onImpactPlayerAvatar.photonView.ViewID);
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x00089AE0 File Offset: 0x00087CE0
	[PunRPC]
	public void TumbleImpactRPC(int _playerID)
	{
		float time = 0.15f;
		this.hurtColliderPauseTimer = 0.5f;
		Vector3 vector = Vector3.zero;
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == _playerID)
			{
				playerAvatar.playerHealth.HurtFreezeOverride(time);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					vector = (playerAvatar.transform.position - base.transform.position).normalized;
					playerAvatar.tumble.physGrabObject.FreezeForces(time, vector * 5f, Vector3.zero);
					break;
				}
				break;
			}
		}
		this.impactParticle.gameObject.SetActive(true);
		this.impactParticle.transform.position = Vector3.Lerp(base.transform.position, base.transform.position + vector, 0.5f);
		this.impactSound.Play(this.impactParticle.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 5f, 15f, base.transform.position, 0.5f);
		this.playerAvatar.playerHealth.HurtFreezeOverride(time);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.FreezeForces(time, vector * -5f, Vector3.zero);
		}
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x00089CB4 File Offset: 0x00087EB4
	public void TumbleOverride(bool _active)
	{
		if (!GameManager.Multiplayer())
		{
			this.TumbleOverrideRPC(_active);
			return;
		}
		this.photonView.RPC("TumbleOverrideRPC", RpcTarget.All, new object[]
		{
			_active
		});
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x00089CE5 File Offset: 0x00087EE5
	[PunRPC]
	public void TumbleOverrideRPC(bool _active)
	{
		this.tumbleOverride = _active;
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x00089CEE File Offset: 0x00087EEE
	public void TumbleOverrideTime(float _time)
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.TumbleOverrideTimeRPC(_time);
			return;
		}
		this.photonView.RPC("TumbleOverrideTimeRPC", RpcTarget.MasterClient, new object[]
		{
			_time
		});
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x00089D26 File Offset: 0x00087F26
	[PunRPC]
	public void TumbleOverrideTimeRPC(float _time)
	{
		this.tumbleOverrideTimer = _time;
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x00089D2F File Offset: 0x00087F2F
	public void TumbleForce(Vector3 _force)
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.TumbleForceRPC(_force);
			return;
		}
		this.photonView.RPC("TumbleForceRPC", RpcTarget.MasterClient, new object[]
		{
			_force
		});
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x00089D67 File Offset: 0x00087F67
	[PunRPC]
	public void TumbleForceRPC(Vector3 _force)
	{
		this.tumbleForce += _force;
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x00089D7B File Offset: 0x00087F7B
	public void TumbleTorque(Vector3 _torque)
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.TumbleTorqueRPC(_torque);
			return;
		}
		this.photonView.RPC("TumbleTorqueRPC", RpcTarget.MasterClient, new object[]
		{
			_torque
		});
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x00089DB3 File Offset: 0x00087FB3
	[PunRPC]
	public void TumbleTorqueRPC(Vector3 _torque)
	{
		this.tumbleTorque += _torque;
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x00089DC8 File Offset: 0x00087FC8
	public void TumbleRequest(bool _isTumbling, bool _playerInput)
	{
		if (PlayerController.instance.DebugNoTumble && !_playerInput)
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.isTumbling == _isTumbling)
		{
			return;
		}
		if (!GameManager.Multiplayer())
		{
			this.TumbleRequestRPC(_isTumbling, _playerInput);
			return;
		}
		this.photonView.RPC("TumbleRequestRPC", RpcTarget.MasterClient, new object[]
		{
			_isTumbling,
			_playerInput
		});
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x00089E30 File Offset: 0x00088030
	[PunRPC]
	public void TumbleRequestRPC(bool _isTumbling, bool _playerInput)
	{
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.isTumbling == _isTumbling)
		{
			return;
		}
		this.TumbleSet(_isTumbling, _playerInput);
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00089E4C File Offset: 0x0008804C
	public void TumbleSet(bool _isTumbling, bool _playerInput)
	{
		this.isTumbling = _isTumbling;
		this.SetPosition();
		if (this.isTumbling)
		{
			this.rb.isKinematic = false;
			if (this.tumbleLaunch > 0 && _playerInput)
			{
				Vector3 b = this.playerAvatar.localCameraTransform.forward * (3f * (float)this.tumbleLaunch);
				this.tumbleForce += b;
			}
		}
		else
		{
			this.rb.isKinematic = true;
			this.tumbleForce = Vector3.zero;
		}
		if (!GameManager.Multiplayer())
		{
			this.TumbleSetRPC(this.isTumbling, _playerInput);
			return;
		}
		this.photonView.RPC("TumbleSetRPC", RpcTarget.All, new object[]
		{
			this.isTumbling,
			_playerInput
		});
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00089F18 File Offset: 0x00088118
	[PunRPC]
	public void TumbleSetRPC(bool _isTumbling, bool _playerInput)
	{
		if (this.playerAvatar.isLocal && _isTumbling && !_playerInput)
		{
			ChatManager.instance.TumbleInterruption();
		}
		this.isTumbling = _isTumbling;
		this.playerAvatar.isTumbling = this.isTumbling;
		this.playerAvatar.EnemyVisionFreezeTimerSet(0.5f);
		Vector3 position = this.playerAvatar.transform.position + Vector3.up * 0.3f;
		Quaternion rotation = this.playerAvatar.transform.rotation;
		if (!this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.physGrabObject.photonTransformView.Teleport(position, rotation);
		}
		else
		{
			this.physGrabObject.rb.position = position;
			this.physGrabObject.rb.rotation = rotation;
		}
		if (this.playerAvatar.isLocal)
		{
			this.playerAvatar.physGrabber.ReleaseObject(0.1f);
			PlayerController.instance.tumbleInputDisableTimer = 1f;
			GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
			GameDirector.instance.CameraShake.Shake(2f, 0.5f);
			CameraPosition.instance.TumbleSet();
		}
		Collider[] array;
		if (this.isTumbling)
		{
			if (this.tumbleLaunch > 0 && _playerInput)
			{
				this.tumbleLaunchSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.playerAvatar.playerAvatarVisuals.PowerupJumpEffect();
			}
			this.playerAvatar.TumbleStart();
			this.tumbleSetTimer = 0.1f;
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				this.playerAvatar.playerAvatarCollision.Collider.enabled = false;
			}
			array = this.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		this.playerAvatar.TumbleStop();
		if (this.playerAvatar.isLocal)
		{
			PlayerController.instance.col.enabled = true;
		}
		else
		{
			this.playerAvatar.playerAvatarCollision.Collider.enabled = true;
		}
		array = this.colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0008A194 File Offset: 0x00088394
	public void BreakImpact()
	{
		if ((!SemiFunc.IsMultiplayer() || (this.playerAvatar && this.playerAvatar.isLocal)) && this.impactHurtTimer > 0f)
		{
			PlayerController.instance.CollisionController.ResetFalling();
			this.playerAvatar.playerHealth.Hurt(this.impactHurtDamage, true, -1);
			this.impactHurtTimer = 0f;
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0008A201 File Offset: 0x00088401
	public void ImpactHurtSet(float _time, int _damage)
	{
		if (!GameManager.Multiplayer())
		{
			this.ImpactHurtSetRPC(_time, _damage);
			return;
		}
		this.photonView.RPC("ImpactHurtSetRPC", RpcTarget.All, new object[]
		{
			_time,
			_damage
		});
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0008A23C File Offset: 0x0008843C
	[PunRPC]
	public void ImpactHurtSetRPC(float _time, int _damage)
	{
		if (this.impactHurtTimer <= 0f || (this.impactHurtTimer <= _time && _damage == this.impactHurtDamage) || _damage > this.impactHurtDamage)
		{
			this.impactHurtTimer = _time;
			this.impactHurtDamage = _damage;
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0008A274 File Offset: 0x00088474
	private void BreakFree(Vector3 _direction)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("BreakFreeRPC", RpcTarget.All, new object[]
			{
				_direction
			});
		}
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0008A2A0 File Offset: 0x000884A0
	[PunRPC]
	private void BreakFreeRPC(Vector3 _direction)
	{
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, base.transform.position, 0.25f);
		this.playerAvatar.TumbleBreakFree();
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			if (physGrabber.playerAvatar.isLocal && Vector3.Dot((physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform.position - base.transform.position).normalized, _direction) > 0.5f)
			{
				physGrabber.OverridePullDistanceIncrement(-1f);
			}
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0008A3AC File Offset: 0x000885AC
	public void TumbleMoveSoundSet(bool _active, float _speed)
	{
		_speed = 1f - _speed;
		_speed = 1f + _speed * 0.25f;
		this.tumbleMoveSoundSpeed = _speed;
		this.tumbleMoveSoundTimer = 0.1f;
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0008A3D8 File Offset: 0x000885D8
	private void OnDrawGizmos()
	{
		if (!this.isTumbling)
		{
			return;
		}
		float d = 0.1f;
		Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.8f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * d);
		Gizmos.color = new Color(0.28f, 1f, 0f, 0.5f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one * d);
	}

	// Token: 0x04001948 RID: 6472
	internal bool setup;

	// Token: 0x04001949 RID: 6473
	public PlayerAvatar playerAvatar;

	// Token: 0x0400194A RID: 6474
	public Transform followPosition;

	// Token: 0x0400194B RID: 6475
	public ParticleSystem impactParticle;

	// Token: 0x0400194C RID: 6476
	public Sound impactSound;

	// Token: 0x0400194D RID: 6477
	[Space]
	public Collider[] colliders;

	// Token: 0x0400194E RID: 6478
	public HurtCollider hurtCollider;

	// Token: 0x0400194F RID: 6479
	[Space]
	public float customGravity = 10f;

	// Token: 0x04001950 RID: 6480
	private float customGravityOverrideTimer;

	// Token: 0x04001951 RID: 6481
	internal Rigidbody rb;

	// Token: 0x04001952 RID: 6482
	internal PhysGrabObject physGrabObject;

	// Token: 0x04001953 RID: 6483
	internal PhotonView photonView;

	// Token: 0x04001954 RID: 6484
	internal bool isTumbling;

	// Token: 0x04001955 RID: 6485
	private bool isTumblingPrevious = true;

	// Token: 0x04001956 RID: 6486
	internal float tumbleSetTimer;

	// Token: 0x04001957 RID: 6487
	internal float notMovingTimer;

	// Token: 0x04001958 RID: 6488
	private Vector3 notMovingPositionLast;

	// Token: 0x04001959 RID: 6489
	private Vector3 tumbleForce;

	// Token: 0x0400195A RID: 6490
	private Vector3 tumbleTorque;

	// Token: 0x0400195B RID: 6491
	private float tumbleForceTimer;

	// Token: 0x0400195C RID: 6492
	private float tumbleOverrideTimer;

	// Token: 0x0400195D RID: 6493
	internal bool tumbleOverride;

	// Token: 0x0400195E RID: 6494
	private bool tumbleOverridePrevious;

	// Token: 0x0400195F RID: 6495
	private float lookAtLerp;

	// Token: 0x04001960 RID: 6496
	[Space]
	public Sound tumbleMoveSound;

	// Token: 0x04001961 RID: 6497
	public Sound tumbleLaunchSound;

	// Token: 0x04001962 RID: 6498
	private float tumbleMoveSoundTimer;

	// Token: 0x04001963 RID: 6499
	private float tumbleMoveSoundSpeed;

	// Token: 0x04001964 RID: 6500
	internal int tumbleLaunch;

	// Token: 0x04001965 RID: 6501
	private float overrideEnemyHurtTimer;

	// Token: 0x04001966 RID: 6502
	private float impactHurtTimer;

	// Token: 0x04001967 RID: 6503
	private int impactHurtDamage;

	// Token: 0x04001968 RID: 6504
	private float hurtColliderPauseTimer;

	// Token: 0x04001969 RID: 6505
	private float breakFreeCooldown;
}
