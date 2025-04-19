using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class PlayerController : MonoBehaviour
{
	// Token: 0x06000EA2 RID: 3746 RVA: 0x00084007 File Offset: 0x00082207
	private void Awake()
	{
		PlayerController.instance = this;
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0008400F File Offset: 0x0008220F
	public void PlayerSetName(string _playerName, string _steamID)
	{
		this.playerName = _playerName;
		this.playerSteamID = _steamID;
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0008401F File Offset: 0x0008221F
	public void MoveForce(Vector3 direction, float amount, float time)
	{
		this.MoveForceDirection = direction.normalized;
		this.MoveForceAmount = amount;
		this.MoveForceTimer = time;
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0008403C File Offset: 0x0008223C
	public void InputDisable(float time)
	{
		this.InputDisableTimer = time;
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00084045 File Offset: 0x00082245
	public void MoveMult(float multiplier, float time)
	{
		this.MoveMultiplier = multiplier;
		this.MoveMultiplierTimer = time;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00084055 File Offset: 0x00082255
	public void CrouchDisable(float time)
	{
		this.CrouchInactiveTimer = Mathf.Max(time, this.CrouchInactiveTimer);
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0008406C File Offset: 0x0008226C
	private void OnCollisionEnter(Collision other)
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (other.gameObject.CompareTag("Phys Grab Object"))
		{
			this.playerAvatarScript.photonView.RPC("ResetPhysPusher", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000840BC File Offset: 0x000822BC
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rbOriginalMass = this.rb.mass;
		this.rbOriginalDrag = this.rb.drag;
		this.AudioSource = base.GetComponent<AudioSource>();
		this.positionPrevious = base.transform.position;
		Inventory component = base.GetComponent<Inventory>();
		if (SemiFunc.RunIsArena())
		{
			component.enabled = false;
		}
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(this.playerAvatarPrefab, base.transform.position, Quaternion.identity);
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0008415E File Offset: 0x0008235E
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		string key = SemiFunc.PlayerGetSteamID(this.playerAvatarScript);
		if (StatsManager.instance.playerUpgradeStamina.ContainsKey(key))
		{
			this.EnergyStart += (float)StatsManager.instance.playerUpgradeStamina[key] * 10f;
			this.SprintSpeed += (float)StatsManager.instance.playerUpgradeSpeed[key];
			this.SprintSpeedUpgrades += (float)StatsManager.instance.playerUpgradeSpeed[key];
			this.JumpExtra = StatsManager.instance.playerUpgradeExtraJump[key];
		}
		this.EnergyCurrent = this.EnergyStart;
		this.playerOriginalMoveSpeed = this.MoveSpeed;
		this.playerOriginalSprintSpeed = this.SprintSpeed;
		this.playerOriginalCrouchSpeed = this.CrouchSpeed;
		this.playerOriginalCustomGravity = this.CustomGravity;
		if (SemiFunc.MenuLevel())
		{
			this.rb.isKinematic = true;
			base.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0008416D File Offset: 0x0008236D
	public void ChangeState()
	{
		this.playerAvatarScript.UpdateState(this.Crouching, this.sprinting, this.Crawling, this.Sliding, this.moving);
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x00084198 File Offset: 0x00082398
	public void ForceImpulse(Vector3 force)
	{
		this.VelocityImpulse += base.transform.InverseTransformDirection(force);
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x000841B7 File Offset: 0x000823B7
	public void AntiGravity(float _timer)
	{
		this.antiGravityTimer = _timer;
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x000841C0 File Offset: 0x000823C0
	public void Feather(float _timer)
	{
		this.featherTimer = _timer;
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x000841C9 File Offset: 0x000823C9
	public void Kinematic(float _timer)
	{
		this.kinematicTimer = _timer;
		this.rb.isKinematic = true;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x000841EC File Offset: 0x000823EC
	public void SetCrawl()
	{
		this.Crouching = true;
		this.Crawling = true;
		this.CrouchActiveTimer = this.CrouchTimeMin;
		this.sprinting = false;
		this.Sliding = false;
		this.moving = false;
		PlayerCollisionStand.instance.SetBlocked();
		CameraCrouchPosition.instance.Lerp = 1f;
		CameraCrouchPosition.instance.Active = true;
		CameraCrouchPosition.instance.ActivePrev = true;
		this.ChangeState();
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0008425D File Offset: 0x0008245D
	public void OverrideSpeed(float _speedMulti, float _time = 0.1f)
	{
		this.overrideSpeedTimer = _time;
		this.overrideSpeedMultiplier = _speedMulti;
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00084270 File Offset: 0x00082470
	private void OverrideSpeedTick()
	{
		if (this.overrideSpeedTimer > 0f)
		{
			this.overrideSpeedTimer -= Time.fixedDeltaTime;
			if (this.overrideSpeedTimer <= 0f)
			{
				this.overrideSpeedMultiplier = 1f;
				this.MoveSpeed = this.playerOriginalMoveSpeed;
				this.SprintSpeed = this.playerOriginalSprintSpeed;
				this.CrouchSpeed = this.playerOriginalCrouchSpeed;
			}
		}
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x000842D8 File Offset: 0x000824D8
	private void OverrideSpeedLogic()
	{
		if (this.overrideSpeedTimer <= 0f)
		{
			return;
		}
		this.MoveSpeed = this.playerOriginalMoveSpeed * this.overrideSpeedMultiplier;
		this.SprintSpeed = this.playerOriginalSprintSpeed * this.overrideSpeedMultiplier;
		this.CrouchSpeed = this.playerOriginalCrouchSpeed * this.overrideSpeedMultiplier;
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0008432C File Offset: 0x0008252C
	public void OverrideAnimationSpeed(float _animSpeedMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		this.playerAvatarScript.OverrideAnimationSpeed(_animSpeedMulti, _timeIn, _timeOut, _time);
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x0008433E File Offset: 0x0008253E
	public void OverrideTimeScale(float _timeScaleMulti, float _time = 0.1f)
	{
		this.overrideTimeScaleTimer = _time;
		this.overrideTimeScaleMultiplier = _timeScaleMulti;
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00084350 File Offset: 0x00082550
	private void OverrideTimeScaleTick()
	{
		if (this.overrideTimeScaleTimer > 0f)
		{
			this.overrideTimeScaleTimer -= Time.fixedDeltaTime;
			if (this.overrideTimeScaleTimer <= 0f)
			{
				this.overrideTimeScaleMultiplier = 1f;
				this.rb.mass = this.rbOriginalMass;
				this.rb.drag = this.rbOriginalDrag;
				this.CustomGravity = this.playerOriginalCustomGravity;
				this.MoveSpeed = this.playerOriginalMoveSpeed;
				this.SprintSpeed = this.playerOriginalSprintSpeed;
				this.CrouchSpeed = this.playerOriginalCrouchSpeed;
				this.rb.useGravity = true;
			}
		}
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x000843F8 File Offset: 0x000825F8
	private void OverrideTimeScaleLogic()
	{
		if (this.overrideTimeScaleTimer <= 0f)
		{
			return;
		}
		float t = this.overrideSpeedMultiplier;
		float y = this.rb.velocity.y;
		this.rb.velocity = Vector3.Lerp(Vector3.zero, this.rb.velocity, t);
		this.rb.velocity = new Vector3(this.rb.velocity.x, y, this.rb.velocity.z);
		this.rb.angularVelocity = Vector3.Lerp(Vector3.zero, this.rb.angularVelocity, t);
		this.rb.mass = Mathf.Lerp(0.01f, this.rbOriginalMass, t);
		this.rb.drag = Mathf.Lerp((1f + this.overrideSpeedMultiplier) * 10f, this.rbOriginalDrag, t);
		this.CustomGravity = Mathf.Lerp(0.1f, this.playerOriginalCustomGravity, t);
		this.MoveSpeed = Mathf.Lerp(0.1f, this.playerOriginalMoveSpeed, t);
		this.SprintSpeed = Mathf.Lerp(0.1f, this.playerOriginalSprintSpeed, t);
		this.CrouchSpeed = Mathf.Lerp(0.1f, this.playerOriginalCrouchSpeed, t);
		this.rb.useGravity = false;
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0008454B File Offset: 0x0008274B
	public void OverrideLookSpeed(float _lookSpeedTarget, float timeIn, float timeOut, float _time = 0.1f)
	{
		this.overrideLookSpeedTimer = _time;
		this.overrideLookSpeedTarget = _lookSpeedTarget;
		this.overrideLookSpeedTimeIn = timeIn;
		this.overrideLookSpeedTimeOut = timeOut;
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x0008456A File Offset: 0x0008276A
	private void OverrideLookSpeedTick()
	{
		if (this.overrideLookSpeedTimer > 0f)
		{
			this.overrideLookSpeedTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0008458C File Offset: 0x0008278C
	private void OverrideLookSpeedLogic()
	{
		if (this.overrideLookSpeedTimer <= 0f && this.overrideLookSpeedProgress <= 0f)
		{
			return;
		}
		float smooth;
		if (this.overrideLookSpeedTimer > 0f)
		{
			this.overrideLookSpeedProgress += Time.fixedDeltaTime / this.overrideLookSpeedTimeIn;
			this.overrideLookSpeedProgress = Mathf.Clamp01(this.overrideLookSpeedProgress);
			this.overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, this.overrideLookSpeedProgress);
			smooth = Mathf.Lerp(this.cameraAim.aimSmoothOriginal, this.overrideLookSpeedTarget, this.overrideLookSpeedLerp);
		}
		else
		{
			this.overrideLookSpeedProgress -= Time.fixedDeltaTime / this.overrideLookSpeedTimeOut;
			this.overrideLookSpeedProgress = Mathf.Clamp01(this.overrideLookSpeedProgress);
			this.overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, this.overrideLookSpeedProgress);
			smooth = Mathf.Lerp(this.cameraAim.aimSmoothOriginal, this.overrideLookSpeedTarget, this.overrideLookSpeedLerp);
			if (this.overrideLookSpeedProgress <= 0f)
			{
				smooth = this.cameraAim.aimSmoothOriginal;
			}
		}
		this.cameraAim.OverrideAimSmooth(smooth, 0.1f);
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x000846B1 File Offset: 0x000828B1
	public void OverrideVoicePitch(float _voicePitchMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		if (this.playerAvatarScript.voiceChat)
		{
			this.playerAvatarScript.voiceChat.OverridePitch(_voicePitchMulti, _timeIn, _timeOut, _time, true);
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x000846DB File Offset: 0x000828DB
	private void OverrideVoicePitchTick()
	{
		if (this.overrideVoicePitchTimer > 0f)
		{
			this.overrideVoicePitchTimer -= Time.fixedDeltaTime;
			if (this.overrideVoicePitchTimer <= 0f)
			{
				this.overrideVoicePitchMultiplier = 1f;
			}
		}
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00084714 File Offset: 0x00082914
	private void FixedUpdate()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			this.OverrideSpeedTick();
			this.OverrideTimeScaleTick();
			this.OverrideLookSpeedTick();
			this.OverrideVoicePitchTick();
			if (this.kinematicTimer > 0f)
			{
				this.VelocityImpulse = Vector3.zero;
				this.rb.isKinematic = true;
				this.kinematicTimer -= Time.fixedDeltaTime;
				if (this.kinematicTimer <= 0f)
				{
					this.rb.isKinematic = false;
				}
				return;
			}
			if (this.playerAvatarScript.isTumbling)
			{
				base.transform.position = this.playerAvatarScript.tumble.transform.position + Vector3.down * 0.3f;
			}
			if (this.Crawling != this.previousCrawlingState)
			{
				this.ChangeState();
				this.previousCrawlingState = this.Crawling;
			}
			if (this.Crouching != this.previousCrouchingState)
			{
				this.ChangeState();
				this.previousCrouchingState = this.Crouching;
			}
			if (this.sprinting != this.previousSprintingState)
			{
				this.ChangeState();
				this.previousSprintingState = this.sprinting;
			}
			if (this.Sliding != this.previousSlidingState)
			{
				this.ChangeState();
				this.previousSlidingState = this.Sliding;
			}
			if (this.moving != this.previousMovingState)
			{
				this.ChangeState();
				this.previousMovingState = this.moving;
			}
			base.transform.rotation = Quaternion.Euler(0f, this.cameraGameObject.transform.localRotation.eulerAngles.y, 0f);
			if ((SemiFunc.InputHold(InputKey.Sprint) || this.toggleSprint) && !this.playerAvatarScript.isTumbling && !this.Crouching && this.EnergyCurrent >= 1f)
			{
				if (this.rb.velocity.magnitude > 0.01f)
				{
					this.CanSlide = true;
					TutorialDirector.instance.playerSprinted = true;
					this.sprinting = true;
					this.SprintedTimer = 0.5f;
					this.SprintDrainTimer = 0.2f;
				}
			}
			else
			{
				if (this.SprintedTimer > 0f)
				{
					this.SprintedTimer -= Time.fixedDeltaTime;
					if (this.SprintedTimer <= 0f)
					{
						this.CanSlide = false;
						this.SprintedTimer = 0f;
					}
				}
				this.SprintSpeedLerp = 0f;
				this.sprinting = false;
			}
			if (this.SprintDrainTimer > 0f && !this.DebugEnergy)
			{
				float num = this.EnergySprintDrain;
				num += this.SprintSpeedUpgrades;
				this.EnergyCurrent -= num * Time.fixedDeltaTime;
				this.EnergyCurrent = Mathf.Max(0f, this.EnergyCurrent);
				if (this.EnergyCurrent <= 0f)
				{
					this.toggleSprint = false;
				}
				this.SprintDrainTimer -= Time.fixedDeltaTime;
			}
			if ((this.Crouching && PlayerCollisionStand.instance.CheckBlocked()) || this.playerAvatarScript.isTumbling)
			{
				TutorialDirector.instance.playerCrawled = true;
				this.Crawling = true;
			}
			else
			{
				this.Crawling = false;
			}
			if (this.playerAvatarScript.isTumbling || (this.CollisionController.Grounded && (SemiFunc.InputHold(InputKey.Crouch) || this.toggleCrouch)))
			{
				if (this.CrouchInactiveTimer <= 0f)
				{
					if (!this.Crouching)
					{
						this.CrouchActiveTimer = this.CrouchTimeMin;
					}
					TutorialDirector.instance.playerCrouched = true;
					this.Crouching = true;
					this.sprinting = false;
				}
			}
			else if (this.Crouching && this.CrouchActiveTimer <= 0f && !this.Crawling)
			{
				this.Crawling = false;
				this.Crouching = false;
				this.CrouchInactiveTimer = this.CrouchTimeMin;
			}
			if (this.CrouchActiveTimer > 0f)
			{
				this.CrouchActiveTimer -= Time.fixedDeltaTime;
			}
			if (this.CrouchInactiveTimer > 0f)
			{
				this.CrouchInactiveTimer -= Time.fixedDeltaTime;
			}
			if (this.sprinting || this.Crouching)
			{
				this.CanInteract = false;
			}
			else
			{
				this.CanInteract = true;
			}
			Vector3 vector = Vector3.zero;
			if (this.MoveForceTimer > 0f)
			{
				this.InputDirection = this.MoveForceDirection;
				this.MoveForceTimer -= Time.fixedDeltaTime;
				this.rb.velocity = this.MoveForceDirection * this.MoveForceAmount;
			}
			else if (this.InputDisableTimer <= 0f)
			{
				this.InputDirection = new Vector3(SemiFunc.InputMovementX(), 0f, SemiFunc.InputMovementY()).normalized;
				if (GameDirector.instance.DisableInput || this.playerAvatarScript.isTumbling)
				{
					this.InputDirection = Vector3.zero;
				}
				if (this.InputDirection.magnitude <= 0.1f)
				{
					this.SprintSpeedLerp = 0f;
				}
				if (this.MoveMultiplierTimer > 0f)
				{
					this.InputDirection *= this.MoveMultiplier;
				}
				if (this.sprinting)
				{
					this.SprintSpeedCurrent = Mathf.Lerp(this.MoveSpeed, this.SprintSpeed, this.SprintSpeedLerp);
					this.SprintSpeedLerp += this.SprintAcceleration * Time.fixedDeltaTime;
					this.SprintSpeedLerp = Mathf.Clamp01(this.SprintSpeedLerp);
					vector += this.InputDirection * this.SprintSpeedCurrent;
					this.SlideDirection = this.InputDirection * this.SprintSpeedCurrent;
					this.SlideDirectionCurrent = this.SlideDirection;
					this.Sliding = false;
				}
				else if (this.Crouching)
				{
					if (this.CanSlide)
					{
						this.playerAvatarScript.Slide();
						if (!this.DebugEnergy)
						{
							this.EnergyCurrent -= 5f;
						}
						this.EnergyCurrent = Mathf.Max(0f, this.EnergyCurrent);
						this.CanSlide = false;
						this.Sliding = true;
						this.SlideTimer = this.SlideTime;
					}
					if (this.SlideTimer > 0f)
					{
						vector += this.SlideDirectionCurrent;
						this.SlideDirectionCurrent -= this.SlideDirection * this.SlideDecay * Time.fixedDeltaTime;
						this.SlideTimer -= Time.fixedDeltaTime;
						if (this.SlideTimer <= 0f)
						{
							this.Sliding = false;
						}
					}
					if (this.debugSlow)
					{
						this.InputDirection *= 0.2f;
					}
					vector += this.InputDirection * this.CrouchSpeed;
				}
				else
				{
					if (this.debugSlow)
					{
						this.InputDirection *= 0.1f;
					}
					vector += this.InputDirection * this.MoveSpeed;
					this.Sliding = false;
				}
			}
			else
			{
				this.InputDirection = Vector3.zero;
			}
			if (this.InputDisableTimer > 0f)
			{
				this.InputDisableTimer -= Time.fixedDeltaTime;
			}
			if (this.MoveMultiplierTimer > 0f)
			{
				this.MoveMultiplierTimer -= Time.fixedDeltaTime;
			}
			if (this.antiGravityTimer > 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.drag = 2f;
					this.rb.useGravity = false;
				}
				this.antiGravityTimer -= Time.fixedDeltaTime;
			}
			else if (!this.rb.useGravity)
			{
				this.rb.drag = 0f;
				this.rb.useGravity = true;
			}
			vector += this.VelocityImpulse;
			this.VelocityRelativeNew += this.VelocityImpulse;
			this.VelocityImpulse = Vector3.zero;
			if (this.VelocityIdle)
			{
				this.VelocityRelativeNew = vector;
			}
			else
			{
				this.VelocityRelativeNew = Vector3.Lerp(this.VelocityRelativeNew, vector, this.MoveFriction * Time.fixedDeltaTime);
			}
			Vector3 vector2 = base.transform.InverseTransformDirection(this.rb.velocity);
			if (this.VelocityRelativeNew.magnitude > 0.1f)
			{
				this.VelocityIdle = false;
				this.col.material = this.PhysicMaterialMove;
				this.VelocityRelative = Vector3.Lerp(this.VelocityRelative, this.VelocityRelativeNew, this.MoveFriction * Time.fixedDeltaTime);
				this.VelocityRelative.y = vector2.y;
				this.rb.AddRelativeForce(this.VelocityRelative - vector2, ForceMode.Impulse);
				this.Velocity = base.transform.InverseTransformDirection(this.VelocityRelative - vector2);
			}
			else
			{
				this.VelocityIdle = true;
				this.col.material = this.PhysicMaterialIdle;
				this.VelocityRelative = Vector3.zero;
				this.Velocity = this.rb.velocity;
			}
			if (!this.CollisionController.Grounded && !this.JumpImpulse && this.featherTimer <= 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.AddForce(new Vector3(0f, -this.CustomGravity * Time.fixedDeltaTime, 0f), ForceMode.Impulse);
				}
				else
				{
					this.rb.AddForce(new Vector3(0f, -(this.CustomGravity * 0.1f) * Time.fixedDeltaTime, 0f), ForceMode.Impulse);
				}
			}
			if (this.JumpImpulse)
			{
				bool flag = false;
				foreach (PhysGrabObject physGrabObject in this.JumpGroundedObjects)
				{
					foreach (PhysGrabber physGrabber in physGrabObject.playerGrabbing)
					{
						if (physGrabber.playerAvatar == this.playerAvatarScript)
						{
							flag = true;
							physGrabber.ReleaseObject(0.1f);
							physGrabber.grabDisableTimer = 1f;
							break;
						}
					}
				}
				Vector3 b = new Vector3(0f, this.rb.velocity.y, 0f);
				float d = this.JumpForce;
				if (flag)
				{
					d = this.JumpForce * 0.5f;
				}
				this.rb.AddForce(Vector3.up * d - b, ForceMode.Impulse);
				this.JumpCooldown = 0.1f;
				this.JumpImpulse = false;
				this.CollisionGrounded.Grounded = false;
				this.JumpGroundedBuffer = 0f;
				this.CollisionController.GroundedDisableTimer = 0.1f;
				this.CollisionController.fallDistance = 0f;
			}
			if (this.VelocityRelativeNew.magnitude > 0.1f)
			{
				this.movingResetTimer = 0.1f;
				this.moving = true;
			}
			else if (this.movingResetTimer > 0f)
			{
				this.movingResetTimer -= Time.fixedDeltaTime;
				if (this.movingResetTimer <= 0f)
				{
					this.sprinting = false;
					this.moving = false;
				}
			}
			if (this.featherTimer > 0f)
			{
				if (this.rb.useGravity)
				{
					this.rb.useGravity = false;
				}
				if (this.antiGravityTimer <= 0f)
				{
					this.rb.AddForce(new Vector3(0f, -15f, 0f), ForceMode.Force);
				}
				this.featherTimer -= Time.fixedDeltaTime;
				if (this.featherTimer <= 0f)
				{
					this.rb.useGravity = true;
				}
			}
			this.OverrideTimeScaleLogic();
			this.OverrideSpeedLogic();
			this.OverrideLookSpeedLogic();
			this.positionPrevious = base.transform.position;
		}
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x000852E4 File Offset: 0x000834E4
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.deathSeenTimer > 0f)
		{
			this.deathSeenTimer -= Time.deltaTime;
		}
		if (this.CollisionController.Grounded)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Crouch))
			{
				if (SemiFunc.InputDown(InputKey.Crouch))
				{
					this.toggleCrouch = !this.toggleCrouch;
					if (this.toggleCrouch)
					{
						this.toggleSprint = false;
					}
				}
			}
			else
			{
				this.toggleCrouch = false;
			}
		}
		if (!this.playerAvatarScript.isTumbling)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Sprint))
			{
				if (SemiFunc.InputDown(InputKey.Sprint))
				{
					this.toggleSprint = !this.toggleSprint;
					if (this.toggleSprint)
					{
						this.toggleCrouch = false;
					}
				}
			}
			else
			{
				this.toggleSprint = false;
			}
		}
		if (this.sprinting)
		{
			this.sprintRechargeTimer = this.sprintRechargeTime;
			if (SemiFunc.RunIsArena())
			{
				this.sprintRechargeTimer *= 0.5f;
			}
		}
		else if (this.sprintRechargeTimer > 0f)
		{
			this.sprintRechargeTimer -= Time.deltaTime;
		}
		else if (this.EnergyCurrent < this.EnergyStart)
		{
			float num = this.sprintRechargeAmount;
			if (SemiFunc.RunIsArena())
			{
				num *= 5f;
			}
			this.EnergyCurrent += num * Time.deltaTime;
			if (this.EnergyCurrent > this.EnergyStart)
			{
				this.EnergyCurrent = this.EnergyStart;
			}
		}
		if (!this.JumpImpulse)
		{
			if (SemiFunc.InputDown(InputKey.Jump) && !this.playerAvatarScript.isTumbling && this.InputDisableTimer <= 0f)
			{
				this.JumpInputBuffer = 0.25f;
			}
			if (this.CollisionGrounded.Grounded)
			{
				this.JumpFirst = true;
				this.JumpExtraCurrent = this.JumpExtra;
				this.JumpGroundedBuffer = 0.25f;
			}
			else if (this.JumpGroundedBuffer > 0f)
			{
				this.JumpGroundedBuffer -= Time.deltaTime;
				if (this.JumpGroundedBuffer <= 0f)
				{
					this.JumpFirst = false;
				}
			}
			if (this.JumpInputBuffer > 0f)
			{
				this.JumpInputBuffer -= Time.deltaTime;
			}
			if (this.JumpCooldown > 0f)
			{
				this.JumpCooldown -= Time.deltaTime;
			}
			if (this.JumpInputBuffer > 0f && (this.JumpGroundedBuffer > 0f || (!this.JumpFirst && this.JumpExtraCurrent > 0)) && this.JumpCooldown <= 0f)
			{
				if (this.JumpFirst)
				{
					this.JumpFirst = false;
					this.playerAvatarScript.Jump(false);
				}
				else
				{
					this.JumpExtraCurrent--;
					this.playerAvatarScript.Jump(true);
				}
				CameraJump.instance.Jump();
				TutorialDirector.instance.playerJumped = true;
				this.JumpImpulse = true;
				this.JumpInputBuffer = 0f;
			}
			if (this.JumpGroundedBuffer <= 0f && this.JumpGroundedObjects.Count > 0)
			{
				this.JumpGroundedObjects.Clear();
			}
		}
		if (this.landCooldown > 0f)
		{
			this.landCooldown -= Time.deltaTime;
		}
		if (this.rb.velocity.y < -4f || (this.playerAvatarScript.tumble && this.playerAvatarScript.tumble.physGrabObject.rbVelocity.y < -4f))
		{
			this.CanLand = true;
		}
		if (this.GroundedPrevious != this.CollisionController.Grounded)
		{
			if (this.CollisionController.Grounded && this.CanLand)
			{
				if (!SemiFunc.MenuLevel() && this.landCooldown <= 0f)
				{
					this.landCooldown = 1f;
					CameraJump.instance.Land();
					this.playerAvatarScript.Land();
				}
				this.CanLand = false;
			}
			this.GroundedPrevious = this.CollisionController.Grounded;
		}
		if (this.tumbleInputDisableTimer > 0f)
		{
			this.tumbleInputDisableTimer -= Time.deltaTime;
		}
		if (this.playerAvatarScript.isTumbling)
		{
			this.col.enabled = false;
			this.rb.isKinematic = true;
			bool flag = false;
			if (this.playerAvatarScript.tumble.notMovingTimer > 0.5f && (Mathf.Abs(SemiFunc.InputMovementX()) > 0f || Mathf.Abs(SemiFunc.InputMovementY()) > 0f))
			{
				flag = true;
			}
			if ((SemiFunc.InputDown(InputKey.Jump) || SemiFunc.InputDown(InputKey.Tumble) || flag) && this.tumbleInputDisableTimer <= 0f && !this.playerAvatarScript.tumble.tumbleOverride && this.InputDisableTimer <= 0f)
			{
				this.playerAvatarScript.tumble.TumbleRequest(false, true);
				return;
			}
		}
		else
		{
			this.col.enabled = true;
			this.rb.isKinematic = false;
			if (SemiFunc.InputDown(InputKey.Tumble) && this.tumbleInputDisableTimer <= 0f && this.InputDisableTimer <= 0f)
			{
				TutorialDirector.instance.playerTumbled = true;
				this.playerAvatarScript.tumble.TumbleRequest(true, true);
			}
		}
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x00085810 File Offset: 0x00083A10
	public void Revive(Vector3 _rotation)
	{
		base.transform.rotation = Quaternion.Euler(0f, _rotation.y, 0f);
		this.InputDisable(0.5f);
		this.Kinematic(0.2f);
		this.SetCrawl();
		this.CollisionController.ResetFalling();
		this.VelocityIdle = true;
		this.col.material = this.PhysicMaterialIdle;
		this.VelocityRelative = Vector3.zero;
		this.Velocity = Vector3.zero;
		this.EnergyCurrent = this.EnergyStart;
	}

	// Token: 0x0400182A RID: 6186
	public static PlayerController instance;

	// Token: 0x0400182B RID: 6187
	private bool previousCrouchingState;

	// Token: 0x0400182C RID: 6188
	private bool previousCrawlingState;

	// Token: 0x0400182D RID: 6189
	private bool previousSprintingState;

	// Token: 0x0400182E RID: 6190
	private bool previousSlidingState;

	// Token: 0x0400182F RID: 6191
	private bool previousMovingState;

	// Token: 0x04001830 RID: 6192
	public GameObject playerAvatar;

	// Token: 0x04001831 RID: 6193
	public GameObject playerAvatarPrefab;

	// Token: 0x04001832 RID: 6194
	public PlayerCollision PlayerCollision;

	// Token: 0x04001833 RID: 6195
	[HideInInspector]
	public GameObject physGrabObject;

	// Token: 0x04001834 RID: 6196
	[HideInInspector]
	public bool physGrabActive;

	// Token: 0x04001835 RID: 6197
	[HideInInspector]
	public Transform physGrabPoint;

	// Token: 0x04001836 RID: 6198
	[Space]
	public PlayerCollisionController CollisionController;

	// Token: 0x04001837 RID: 6199
	public PlayerCollisionGrounded CollisionGrounded;

	// Token: 0x04001838 RID: 6200
	private bool GroundedPrevious;

	// Token: 0x04001839 RID: 6201
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x0400183A RID: 6202
	[HideInInspector]
	public Rigidbody rb;

	// Token: 0x0400183B RID: 6203
	private bool CanLand;

	// Token: 0x0400183C RID: 6204
	private float landCooldown;

	// Token: 0x0400183D RID: 6205
	[Space]
	public float MoveSpeed = 0.5f;

	// Token: 0x0400183E RID: 6206
	public float MoveFriction = 5f;

	// Token: 0x0400183F RID: 6207
	[HideInInspector]
	public Vector3 Velocity;

	// Token: 0x04001840 RID: 6208
	[HideInInspector]
	public Vector3 VelocityRelative;

	// Token: 0x04001841 RID: 6209
	private Vector3 VelocityRelativeNew;

	// Token: 0x04001842 RID: 6210
	private bool VelocityIdle;

	// Token: 0x04001843 RID: 6211
	private Vector3 VelocityImpulse = Vector3.zero;

	// Token: 0x04001844 RID: 6212
	[Space]
	public float SprintSpeed = 1f;

	// Token: 0x04001845 RID: 6213
	public float SprintSpeedUpgrades;

	// Token: 0x04001846 RID: 6214
	private float SprintSpeedCurrent = 1f;

	// Token: 0x04001847 RID: 6215
	[HideInInspector]
	public float SprintSpeedLerp;

	// Token: 0x04001848 RID: 6216
	public float SprintAcceleration = 1f;

	// Token: 0x04001849 RID: 6217
	private float SprintedTimer;

	// Token: 0x0400184A RID: 6218
	private float SprintDrainTimer;

	// Token: 0x0400184B RID: 6219
	[Space]
	public float CrouchSpeed = 1f;

	// Token: 0x0400184C RID: 6220
	public float CrouchTimeMin = 0.2f;

	// Token: 0x0400184D RID: 6221
	private float CrouchActiveTimer;

	// Token: 0x0400184E RID: 6222
	private float CrouchInactiveTimer;

	// Token: 0x0400184F RID: 6223
	[Space]
	public float SlideTime = 1f;

	// Token: 0x04001850 RID: 6224
	public float SlideDecay = 0.1f;

	// Token: 0x04001851 RID: 6225
	private float SlideTimer;

	// Token: 0x04001852 RID: 6226
	private Vector3 SlideDirection;

	// Token: 0x04001853 RID: 6227
	private Vector3 SlideDirectionCurrent;

	// Token: 0x04001854 RID: 6228
	internal bool CanSlide;

	// Token: 0x04001855 RID: 6229
	[HideInInspector]
	public bool Sliding;

	// Token: 0x04001856 RID: 6230
	[Space]
	public float JumpForce = 20f;

	// Token: 0x04001857 RID: 6231
	internal int JumpExtra;

	// Token: 0x04001858 RID: 6232
	private int JumpExtraCurrent;

	// Token: 0x04001859 RID: 6233
	private bool JumpFirst;

	// Token: 0x0400185A RID: 6234
	public float CustomGravity = 20f;

	// Token: 0x0400185B RID: 6235
	internal bool JumpImpulse;

	// Token: 0x0400185C RID: 6236
	private float JumpCooldown;

	// Token: 0x0400185D RID: 6237
	private float JumpGroundedBuffer;

	// Token: 0x0400185E RID: 6238
	internal List<PhysGrabObject> JumpGroundedObjects = new List<PhysGrabObject>();

	// Token: 0x0400185F RID: 6239
	private float JumpInputBuffer;

	// Token: 0x04001860 RID: 6240
	public float StepUpForce = 2f;

	// Token: 0x04001861 RID: 6241
	public bool DebugNoTumble;

	// Token: 0x04001862 RID: 6242
	[Space]
	public bool DebugEnergy;

	// Token: 0x04001863 RID: 6243
	public float EnergyStart = 100f;

	// Token: 0x04001864 RID: 6244
	[HideInInspector]
	public float EnergyCurrent;

	// Token: 0x04001865 RID: 6245
	public float EnergySprintDrain = 1f;

	// Token: 0x04001866 RID: 6246
	private float sprintRechargeTimer;

	// Token: 0x04001867 RID: 6247
	private float sprintRechargeTime = 1f;

	// Token: 0x04001868 RID: 6248
	private float sprintRechargeAmount = 2f;

	// Token: 0x04001869 RID: 6249
	[Space(15f)]
	public CameraAim cameraAim;

	// Token: 0x0400186A RID: 6250
	public GameObject cameraGameObject;

	// Token: 0x0400186B RID: 6251
	public GameObject cameraGameObjectLocal;

	// Token: 0x0400186C RID: 6252
	public Transform VisionTarget;

	// Token: 0x0400186D RID: 6253
	[HideInInspector]
	public bool CanInteract;

	// Token: 0x0400186E RID: 6254
	[HideInInspector]
	public bool moving;

	// Token: 0x0400186F RID: 6255
	private float movingResetTimer;

	// Token: 0x04001870 RID: 6256
	[HideInInspector]
	public bool sprinting;

	// Token: 0x04001871 RID: 6257
	[HideInInspector]
	public bool Crouching;

	// Token: 0x04001872 RID: 6258
	[HideInInspector]
	public bool Crawling;

	// Token: 0x04001873 RID: 6259
	[HideInInspector]
	public Vector3 InputDirection;

	// Token: 0x04001874 RID: 6260
	[HideInInspector]
	public AudioSource AudioSource;

	// Token: 0x04001875 RID: 6261
	private Vector3 positionPrevious;

	// Token: 0x04001876 RID: 6262
	private Vector3 MoveForceDirection = Vector3.zero;

	// Token: 0x04001877 RID: 6263
	private float MoveForceAmount;

	// Token: 0x04001878 RID: 6264
	private float MoveForceTimer;

	// Token: 0x04001879 RID: 6265
	internal float InputDisableTimer;

	// Token: 0x0400187A RID: 6266
	private float MoveMultiplier = 1f;

	// Token: 0x0400187B RID: 6267
	private float MoveMultiplierTimer;

	// Token: 0x0400187C RID: 6268
	internal string playerName;

	// Token: 0x0400187D RID: 6269
	internal string playerSteamID;

	// Token: 0x0400187E RID: 6270
	private float overrideSpeedTimer;

	// Token: 0x0400187F RID: 6271
	internal float overrideSpeedMultiplier = 1f;

	// Token: 0x04001880 RID: 6272
	private float overrideLookSpeedTimer;

	// Token: 0x04001881 RID: 6273
	internal float overrideLookSpeedTarget = 1f;

	// Token: 0x04001882 RID: 6274
	private float overrideLookSpeedTimeIn = 15f;

	// Token: 0x04001883 RID: 6275
	private float overrideLookSpeedTimeOut = 0.3f;

	// Token: 0x04001884 RID: 6276
	private float overrideLookSpeedLerp;

	// Token: 0x04001885 RID: 6277
	private float overrideLookSpeedProgress;

	// Token: 0x04001886 RID: 6278
	private float overrideVoicePitchTimer;

	// Token: 0x04001887 RID: 6279
	internal float overrideVoicePitchMultiplier = 1f;

	// Token: 0x04001888 RID: 6280
	private float overrideTimeScaleTimer;

	// Token: 0x04001889 RID: 6281
	internal float overrideTimeScaleMultiplier = 1f;

	// Token: 0x0400188A RID: 6282
	private Vector3 originalVelocity;

	// Token: 0x0400188B RID: 6283
	private Vector3 originalAngularVelocity;

	// Token: 0x0400188C RID: 6284
	public PlayerAvatar playerAvatarScript;

	// Token: 0x0400188D RID: 6285
	[Space]
	public Collider col;

	// Token: 0x0400188E RID: 6286
	public PhysicMaterial PhysicMaterialMove;

	// Token: 0x0400188F RID: 6287
	public PhysicMaterial PhysicMaterialIdle;

	// Token: 0x04001890 RID: 6288
	internal float antiGravityTimer;

	// Token: 0x04001891 RID: 6289
	internal float featherTimer;

	// Token: 0x04001892 RID: 6290
	internal float deathSeenTimer;

	// Token: 0x04001893 RID: 6291
	internal float tumbleInputDisableTimer;

	// Token: 0x04001894 RID: 6292
	private float kinematicTimer;

	// Token: 0x04001895 RID: 6293
	private bool toggleSprint;

	// Token: 0x04001896 RID: 6294
	private bool toggleCrouch;

	// Token: 0x04001897 RID: 6295
	private float rbOriginalMass;

	// Token: 0x04001898 RID: 6296
	private float rbOriginalDrag;

	// Token: 0x04001899 RID: 6297
	private float playerOriginalMoveSpeed;

	// Token: 0x0400189A RID: 6298
	private float playerOriginalCustomGravity;

	// Token: 0x0400189B RID: 6299
	private float playerOriginalSprintSpeed;

	// Token: 0x0400189C RID: 6300
	private float playerOriginalCrouchSpeed;

	// Token: 0x0400189D RID: 6301
	internal bool debugSlow;
}
