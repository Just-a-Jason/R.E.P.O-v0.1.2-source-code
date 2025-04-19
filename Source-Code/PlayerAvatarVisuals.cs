using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class PlayerAvatarVisuals : MonoBehaviour
{
	// Token: 0x06000E73 RID: 3699 RVA: 0x00081FB8 File Offset: 0x000801B8
	private void Start()
	{
		this.playerAvatarRightArm = base.GetComponentInChildren<PlayerAvatarRightArm>();
		this.playerAvatarTalkAnimation = base.GetComponentInChildren<PlayerAvatarTalkAnimation>();
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		if (!this.isMenuAvatar && (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine)))
		{
			this.animator.enabled = false;
			this.meshParent.SetActive(false);
		}
		if (SemiFunc.IsMultiplayer() && !SemiFunc.RunIsArena())
		{
			PlayerAvatar x = SessionManager.instance.CrownedPlayerGet();
			if (!this.isMenuAvatar)
			{
				if (x == this.playerAvatar)
				{
					this.arenaCrown.SetActive(true);
					return;
				}
			}
			else if (x == PlayerAvatar.instance)
			{
				this.arenaCrown.SetActive(true);
			}
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00082090 File Offset: 0x00080290
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (SemiFunc.FPSImpulse5() && !this.crownSetterWasHere && PlayerCrownSet.instance && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (this.playerAvatar && PlayerCrownSet.instance.crownOwnerSteamID == this.playerAvatar.steamID)
			{
				this.arenaCrown.SetActive(true);
			}
			this.crownSetterWasHere = true;
		}
		this.deltaTime = Time.deltaTime * this.animationSpeedMultiplier;
		this.deltaTime = Mathf.Max(this.deltaTime, 0f);
		if (this.isMenuAvatar)
		{
			this.MenuAvatarGetColorsFromRealAvatar();
		}
		if (!this.isMenuAvatar && this.playerAvatar.isDisabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.isMenuAvatar)
		{
			if (!GameManager.Multiplayer() || this.playerAvatar.photonView.IsMine)
			{
				if (this.playerAvatar)
				{
					base.transform.position = this.playerAvatar.transform.position;
					base.transform.rotation = this.playerAvatar.transform.rotation;
				}
			}
			else
			{
				if (this.playerAvatar.isTumbling && this.playerAvatar.tumble)
				{
					this.visualFollowLerp = 0f;
					this.visualPosition = this.playerAvatar.tumble.followPosition.position;
					this.bodySpringTarget = this.playerAvatar.tumble.followPosition.rotation;
					this.playerAvatar.clientPosition = this.visualPosition;
					this.playerAvatar.clientPositionCurrent = this.visualPosition;
				}
				else if (!this.playerAvatar.clientPhysRiding || !this.PhysRiderPointInstance)
				{
					float num = Mathf.Lerp(0f, 25f, this.visualFollowLerp);
					this.visualFollowLerp = Mathf.Clamp01(this.visualFollowLerp + 2f * this.deltaTime);
					this.visualPosition = Vector3.Lerp(this.visualPosition, this.playerAvatar.clientPositionCurrent, num * this.deltaTime);
				}
				else if (this.PhysRiderPointInstance)
				{
					float num2 = Mathf.Lerp(0f, 25f, this.visualFollowLerp);
					this.visualFollowLerp = Mathf.Clamp01(this.visualFollowLerp + 2f * this.deltaTime);
					this.visualPosition = Vector3.Lerp(this.visualPosition, this.PhysRiderPointInstance.transform.position, num2 * this.deltaTime);
					this.playerAvatar.clientPosition = this.visualPosition;
					this.playerAvatar.clientPositionCurrent = this.visualPosition;
				}
				if (!this.playerAvatar.isTumbling)
				{
					if (this.animSliding)
					{
						if (this.animSlidingImpulse && this.playerAvatar.rbVelocity.magnitude > 0.1f)
						{
							this.bodySpringTarget = Quaternion.LookRotation(base.transform.TransformDirection(this.playerAvatar.rbVelocity).normalized, Vector3.up);
						}
					}
					else
					{
						this.bodySpringTarget = this.playerAvatar.clientRotationCurrent;
					}
					base.transform.rotation = SemiFunc.SpringQuaternionGet(this.bodySpring, this.bodySpringTarget, this.deltaTime);
				}
				else if (this.playerAvatar.tumble.tumbleSetTimer <= 0f)
				{
					this.bodySpring.lastRotation = this.bodySpringTarget;
					base.transform.rotation = this.bodySpringTarget;
				}
				base.transform.position = this.visualPosition;
				if (this.playerAvatar.playerHealth.hurtFreeze)
				{
					this.animator.speed = 0f;
					return;
				}
				this.turnDifference = Quaternion.Angle(Quaternion.Euler(0f, this.turnPrevious, 0f), Quaternion.Euler(0f, this.bodySpringTarget.eulerAngles.y, 0f));
				float f = this.turnPrevious - this.bodySpringTarget.eulerAngles.y;
				if (Mathf.Abs(f) < 180f)
				{
					this.turnDirection = Mathf.Sign(f);
				}
				if (this.playerAvatar.isTumbling)
				{
					this.turnDifference = 0f;
				}
				this.turnPrevious = this.bodySpringTarget.eulerAngles.y;
			}
		}
		if (this.isMenuAvatar || (GameManager.Multiplayer() && !this.playerAvatar.photonView.IsMine))
		{
			if (this.playerEyes && this.playerEyes.lookAtActive && GameDirector.instance.currentState == GameDirector.gameState.Main && this.playerAvatar && this.playerAvatar.PlayerVisionTarget && this.playerAvatar.PlayerVisionTarget.VisionTransform)
			{
				Vector3 b = this.playerAvatar.PlayerVisionTarget.VisionTransform.position;
				Vector3 forward = this.playerAvatar.localCameraTransform.forward;
				if (this.playerAvatar.tumble && this.playerAvatar.tumble.isTumbling)
				{
					forward = this.playerAvatar.tumble.transform.forward;
				}
				if (this.isMenuAvatar)
				{
					b = base.transform.position + Vector3.up * 1.5f;
					forward = base.transform.forward;
				}
				Vector3 vector = this.playerEyes.lookAt.position - b;
				vector = SemiFunc.ClampDirection(vector, forward, 40f);
				this.headLookAtTransform.rotation = Quaternion.Slerp(this.headLookAtTransform.rotation, Quaternion.LookRotation(vector), this.deltaTime * 15f);
			}
			else
			{
				this.headLookAtTransform.localRotation = Quaternion.Slerp(this.headLookAtTransform.localRotation, Quaternion.identity, this.deltaTime * 15f);
			}
			float num3 = 0f;
			if (!this.playerAvatar.isTumbling && !this.isMenuAvatar)
			{
				num3 = this.playerAvatar.localCameraRotation.eulerAngles.x;
				if (num3 > 90f)
				{
					num3 -= 360f;
				}
				if (this.playerAvatar.isCrawling)
				{
					num3 *= 0.5f;
				}
			}
			float num4 = this.headLookAtTransform.localEulerAngles.x;
			if (num4 > 90f)
			{
				num4 -= 360f;
			}
			if (this.isMenuAvatar)
			{
				num4 *= 1.25f;
			}
			num3 += num4;
			float num5 = SemiFunc.SpringFloatGet(this.lookUpSpring, num3, this.deltaTime);
			this.headUpTransform.localRotation = Quaternion.Euler(num5 * 0.5f, 0f, 0f);
			this.bodyTopUpTransform.localRotation = Quaternion.Euler(num5 * 0.25f, 0f, 0f);
			this.upDifference = Quaternion.Angle(Quaternion.Euler(this.upPrevious, 0f, 0f), Quaternion.Euler(this.headUpTransform.eulerAngles.x, 0f, 0f));
			float f2 = this.upPrevious - this.headUpTransform.eulerAngles.x;
			if (Mathf.Abs(f2) < 180f)
			{
				this.upDirection = Mathf.Sign(f2);
			}
			this.upPrevious = this.headUpTransform.eulerAngles.x;
			float num6 = 0f;
			if (this.turnDifference > 0.5f && this.turnDirection != 0f)
			{
				num6 = this.turnDifference * -this.turnDirection * 25f;
			}
			Quaternion quaternion = Quaternion.Euler(0f, this.headLookAtTransform.localRotation.eulerAngles.y + num6, 0f);
			quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, 0.5f);
			Quaternion quaternion2 = SemiFunc.SpringQuaternionGet(this.lookSideSpring, quaternion, this.deltaTime);
			this.headSideTransform.localRotation = quaternion2;
			this.bodyTopSideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, quaternion2, 0.5f);
			Vector3 zero = Vector3.zero;
			if (this.isMenuAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb && Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				zero.z = PlayerAvatarMenu.instance.rb.angularVelocity.y * 0.01f;
			}
			else if (this.playerAvatar.rbVelocity.magnitude > 0.1f)
			{
				Vector3 vector2 = base.transform.TransformDirection(this.playerAvatar.rbVelocity);
				if (Vector3.Dot(vector2.normalized, base.transform.forward) < -0.5f)
				{
					zero.x = -3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.forward) > 0.5f)
				{
					zero.x = 3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.right) > 0.5f)
				{
					zero.z = -3f;
				}
				if (Vector3.Dot(vector2.normalized, base.transform.right) < -0.5f)
				{
					zero.z = 3f;
				}
			}
			if (this.tiltSprinting != this.animSprinting)
			{
				if (this.tiltSprinting)
				{
					this.tiltTimer = 0.25f;
					this.tiltTarget = this.leanSpringTargetPrevious * 2f;
				}
				else
				{
					this.tiltTimer = 0.25f;
					this.tiltTarget = zero * 3f;
				}
				this.tiltSprinting = this.animSprinting;
			}
			this.leanTransform.localRotation = SemiFunc.SpringQuaternionGet(this.leanSpring, Quaternion.Euler(zero), this.deltaTime);
			this.tiltTransform.localRotation = SemiFunc.SpringQuaternionGet(this.tiltSpring, Quaternion.Euler(this.tiltTarget), this.deltaTime);
			if (this.tiltTimer > 0f)
			{
				this.tiltTimer -= this.deltaTime;
				if (this.tiltTimer <= 0f)
				{
					this.tiltTarget = Vector3.zero;
				}
			}
			this.leanSpringTargetPrevious = zero;
			bool flag = false;
			float num7 = 15f;
			float num8 = 0.5f;
			Vector3 vector3 = Vector3.zero;
			if (this.isMenuAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb && Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				flag = true;
				num7 = 10f;
				num8 = 0.7f;
				Vector3 vector4 = Quaternion.Euler(0f, -PlayerAvatarMenu.instance.rb.angularVelocity.y * 0.1f, 0f) * Vector3.forward;
				vector4.y = 0f;
				vector3 = vector4;
			}
			else if (this.playerAvatar.isMoving && !this.animJumping && this.playerAvatar.rbVelocity.magnitude > 0.1f)
			{
				flag = true;
				num7 = 10f;
				num8 = 0.7f;
				Vector3 normalized = this.playerAvatar.rbVelocity.normalized;
				normalized.y = 0f;
				vector3 = normalized;
			}
			if (this.legTwistActive != flag)
			{
				this.legTwistActive = flag;
				this.legTwistSpring.speed = num7;
				this.legTwistSpring.damping = num8;
			}
			else
			{
				this.legTwistSpring.speed = Mathf.Lerp(this.legTwistSpring.speed, num7, this.deltaTime * 5f);
				this.legTwistSpring.damping = Mathf.Lerp(this.legTwistSpring.damping, num8, this.deltaTime * 5f);
			}
			Quaternion targetRotation = Quaternion.identity;
			if (vector3 != Vector3.zero)
			{
				targetRotation = Quaternion.LookRotation(vector3, Vector3.up);
			}
			this.legTwistTransform.localRotation = SemiFunc.SpringQuaternionGet(this.legTwistSpring, targetRotation, this.deltaTime);
			this.AnimationLogic();
		}
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00082D0C File Offset: 0x00080F0C
	private void MenuAvatarGetColorsFromRealAvatar()
	{
		if (this.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (this.playerAvatar && this.playerAvatar.playerAvatarVisuals.color != this.color)
		{
			this.SetColor(-1, this.playerAvatar.playerAvatarVisuals.color);
		}
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x00082D7A File Offset: 0x00080F7A
	private void OnDestroy()
	{
		Object.Destroy(this.PhysRiderPointInstance);
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x00082D88 File Offset: 0x00080F88
	private void AnimationLogic()
	{
		if (this.isMenuAvatar && PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.rb)
		{
			if (Mathf.Abs(PlayerAvatarMenu.instance.rb.angularVelocity.magnitude) > 1f)
			{
				this.animator.SetBool("Turning", true);
				return;
			}
			this.animator.SetBool("Turning", false);
			return;
		}
		else
		{
			bool flag = false;
			if (this.playerAvatar.isTumbling)
			{
				if (!this.animSprinting && !this.animTumbling)
				{
					this.animator.SetTrigger("TumblingImpulse");
					this.animTumbling = true;
				}
				if ((this.playerAvatar.tumble.physGrabObject.rbVelocity.magnitude > 1f && !this.playerAvatar.tumble.physGrabObject.impactDetector.inCart) || this.playerAvatar.tumble.physGrabObject.rbAngularVelocity.magnitude > 1f)
				{
					this.animator.SetBool("TumblingMove", true);
					flag = true;
				}
				else
				{
					this.animator.SetBool("TumblingMove", false);
				}
				this.animator.SetBool("Tumbling", true);
			}
			else
			{
				this.animator.SetBool("Tumbling", false);
				this.animator.SetBool("TumblingMove", false);
				this.animTumbling = false;
			}
			if (this.playerAvatar.isCrouching || this.playerAvatar.isTumbling)
			{
				this.animator.SetBool("Crouching", true);
			}
			else
			{
				this.animator.SetBool("Crouching", false);
			}
			if (this.playerAvatar.isCrawling || this.playerAvatar.isTumbling)
			{
				this.animator.SetBool("Crawling", true);
			}
			else
			{
				this.animator.SetBool("Crawling", false);
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch to Crawl") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl Move") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
			{
				this.animInCrawl = true;
			}
			else
			{
				this.animInCrawl = false;
			}
			if (this.playerAvatar.isMoving && !this.animJumping)
			{
				this.animator.SetBool("Moving", true);
			}
			else
			{
				this.animator.SetBool("Moving", false);
			}
			if (!this.playerAvatar.isMoving && !this.animJumping && Mathf.Abs(this.turnDifference) > 0.25f)
			{
				this.animator.SetBool("Turning", true);
			}
			else
			{
				this.animator.SetBool("Turning", false);
			}
			if (this.playerAvatar.isSprinting && !this.animJumping && !this.animTumbling)
			{
				if (!this.animSprinting && !this.animSliding)
				{
					this.animator.SetTrigger("SprintingImpulse");
					this.animSprinting = true;
				}
				this.animator.SetBool("Sprinting", true);
			}
			else
			{
				this.animator.SetBool("Sprinting", false);
				this.animSprinting = false;
			}
			this.animSlidingImpulse = false;
			if (this.playerAvatar.isSliding && !this.animJumping && !this.animTumbling)
			{
				if (!this.animSliding)
				{
					this.animSlidingImpulse = true;
					this.animator.SetTrigger("SlidingImpulse");
				}
				this.animator.SetBool("Sliding", true);
				this.animSliding = true;
			}
			else
			{
				this.animator.SetBool("Sliding", false);
				this.animSliding = false;
			}
			if (this.animJumping)
			{
				if (this.animJumpingImpulse)
				{
					this.animJumpTimer = 0.2f;
					this.animJumpingImpulse = false;
					this.animator.SetTrigger("JumpingImpulse");
					this.animator.SetBool("Jumping", true);
					this.animator.SetBool("Falling", false);
				}
				else if (this.playerAvatar.rbVelocityRaw.y < -0.5f && this.animJumpTimer <= 0f)
				{
					this.animator.SetBool("Falling", true);
				}
				if (this.playerAvatar.isGrounded && this.animJumpTimer <= 0f)
				{
					this.animJumpedTimer = 0.5f;
					this.animJumping = false;
				}
				this.animJumpTimer -= this.deltaTime;
			}
			else
			{
				this.animator.SetBool("Jumping", false);
				this.animator.SetBool("Falling", false);
			}
			if (this.animJumpedTimer > 0f)
			{
				this.animJumpedTimer -= this.deltaTime;
			}
			if (!this.playerAvatar.isGrounded)
			{
				this.animFallingTimer += this.deltaTime;
			}
			else
			{
				this.animFallingTimer = 0f;
			}
			if (!this.playerAvatar.isCrawling && !this.animJumping && !this.animSliding && !this.animTumbling && this.animFallingTimer > 0.25f && this.animJumpedTimer <= 0f)
			{
				this.animJumpTimer = 0.2f;
				this.animJumping = true;
				this.animJumpingImpulse = false;
				this.animator.SetTrigger("FallingImpulse");
				this.animator.SetBool("Jumping", true);
				this.animator.SetBool("Falling", true);
			}
			if (flag)
			{
				float num = Mathf.Max(this.playerAvatar.tumble.physGrabObject.rbVelocity.magnitude, this.playerAvatar.tumble.physGrabObject.rbAngularVelocity.magnitude) * 0.5f;
				num = Mathf.Clamp(num, 0.5f, 1.25f);
				this.animator.speed = num * this.animationSpeedMultiplier;
				this.playerAvatar.tumble.TumbleMoveSoundSet(flag, num);
				return;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
			{
				float num2 = 1f + (float)StatsManager.instance.playerUpgradeSpeed[this.playerAvatar.steamID] * 0.1f;
				this.animator.speed = num2 * this.animationSpeedMultiplier;
				return;
			}
			if (this.playerAvatar.isMoving && this.playerAvatar.mapToolController.Active)
			{
				this.animator.speed = 0.5f * this.animationSpeedMultiplier;
				return;
			}
			this.animator.speed = 1f * this.animationSpeedMultiplier;
			return;
		}
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x00083443 File Offset: 0x00081643
	public void JumpImpulse()
	{
		if (this.playerAvatar.isCrawling || this.animTumbling)
		{
			return;
		}
		this.animJumpingImpulse = true;
		this.animJumping = true;
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0008346C File Offset: 0x0008166C
	public void PhysRidingCheck()
	{
		bool flag = this.PhysRiderPointInstance != null;
		if (flag && this.PhysRiderPointInstance.transform.parent != this.playerAvatar.clientPhysRidingTransform)
		{
			Object.Destroy(this.PhysRiderPointInstance);
			flag = false;
		}
		if (!flag)
		{
			this.PhysRiderPointInstance = Object.Instantiate<GameObject>(this.PhysRiderPoint, Vector3.zero, Quaternion.identity, this.playerAvatar.clientPhysRidingTransform);
		}
		this.PhysRiderPointInstance.transform.localPosition = this.playerAvatar.clientPhysRidingPosition;
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x000834FC File Offset: 0x000816FC
	public void SetColor(int _colorIndex, Color _setColor = default(Color))
	{
		bool flag = false;
		Color value;
		if (_colorIndex != -1)
		{
			value = AssetManager.instance.playerColors[_colorIndex];
		}
		else
		{
			value = _setColor;
			flag = true;
		}
		int nameID = Shader.PropertyToID("_AlbedoColor");
		this.color = value;
		if (!flag)
		{
			this.playerAvatar.playerHealth.bodyMaterial.SetColor(nameID, value);
		}
		else
		{
			PlayerHealth componentInParent = base.GetComponentInParent<PlayerHealth>();
			if (componentInParent)
			{
				componentInParent.bodyMaterial.SetColor(nameID, value);
			}
		}
		if (SemiFunc.RunIsLobbyMenu() && MenuPageLobby.instance)
		{
			foreach (MenuPlayerListed menuPlayerListed in MenuPageLobby.instance.menuPlayerListedList)
			{
				if (menuPlayerListed.playerAvatar == this.playerAvatar)
				{
					menuPlayerListed.playerHead.SetColor(value);
					break;
				}
			}
		}
		this.colorSet = true;
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x000835F4 File Offset: 0x000817F4
	public void Revive()
	{
		this.bodySpringTarget = this.playerAvatar.clientRotationCurrent;
		this.bodySpring.lastRotation = this.bodySpringTarget;
		this.turnPrevious = this.bodySpringTarget.eulerAngles.y;
		this.playerAvatar.isCrawling = true;
		this.playerAvatar.isCrouching = true;
		this.playerAvatar.isTumbling = false;
		this.playerAvatar.isMoving = false;
		this.playerAvatar.isSprinting = false;
		this.visualFollowLerp = 1f;
		this.animator.Play("Crawl");
		this.animInCrawl = true;
		this.animator.SetBool("Crouching", true);
		this.animator.SetBool("Crawling", true);
		this.animator.SetBool("Moving", false);
		this.animator.SetBool("Sprinting", false);
		this.animator.SetBool("Sliding", false);
		this.animator.SetBool("Jumping", false);
		this.animator.SetBool("Falling", false);
		this.animator.SetBool("Turning", false);
		this.animator.SetBool("Tumbling", false);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x00083730 File Offset: 0x00081930
	public void PowerupJumpEffect()
	{
		ParticleSystem[] array = this.powerupJumpEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0008375C File Offset: 0x0008195C
	public void TumbleBreakFreeEffect()
	{
		ParticleSystem[] array = this.tumbleBreakFreeEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x00083786 File Offset: 0x00081986
	public void FootstepLight()
	{
		this.playerAvatar.Footstep(Materials.SoundType.Light);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x00083794 File Offset: 0x00081994
	public void FootstepMedium()
	{
		if (this.isMenuAvatar)
		{
			return;
		}
		this.playerAvatar.Footstep(Materials.SoundType.Medium);
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x000837AB File Offset: 0x000819AB
	public void FootstepHeavy()
	{
		this.playerAvatar.Footstep(Materials.SoundType.Heavy);
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x000837B9 File Offset: 0x000819B9
	public void StandToCrouch()
	{
		this.playerAvatar.StandToCrouch();
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x000837C6 File Offset: 0x000819C6
	public void CrouchToStand()
	{
		this.playerAvatar.CrouchToStand();
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x000837D3 File Offset: 0x000819D3
	public void CrouchToCrawl()
	{
		this.playerAvatar.CrouchToCrawl();
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x000837E0 File Offset: 0x000819E0
	public void CrawlToCrouch()
	{
		this.playerAvatar.CrawlToCrouch();
	}

	// Token: 0x040017BE RID: 6078
	public bool isMenuAvatar;

	// Token: 0x040017BF RID: 6079
	[Space]
	public PlayerAvatar playerAvatar;

	// Token: 0x040017C0 RID: 6080
	public GameObject meshParent;

	// Token: 0x040017C1 RID: 6081
	private Animator animator;

	// Token: 0x040017C2 RID: 6082
	private bool animSprinting;

	// Token: 0x040017C3 RID: 6083
	private bool animSliding;

	// Token: 0x040017C4 RID: 6084
	private bool animSlidingImpulse;

	// Token: 0x040017C5 RID: 6085
	private bool animJumping;

	// Token: 0x040017C6 RID: 6086
	private bool animJumpingImpulse;

	// Token: 0x040017C7 RID: 6087
	private float animJumpTimer;

	// Token: 0x040017C8 RID: 6088
	private float animJumpedTimer;

	// Token: 0x040017C9 RID: 6089
	private float animFallingTimer;

	// Token: 0x040017CA RID: 6090
	internal bool animInCrawl;

	// Token: 0x040017CB RID: 6091
	internal bool animTumbling;

	// Token: 0x040017CC RID: 6092
	internal PlayerAvatarTalkAnimation playerAvatarTalkAnimation;

	// Token: 0x040017CD RID: 6093
	internal PlayerAvatarRightArm playerAvatarRightArm;

	// Token: 0x040017CE RID: 6094
	[Space]
	public Transform headUpTransform;

	// Token: 0x040017CF RID: 6095
	public Transform headSideTransform;

	// Token: 0x040017D0 RID: 6096
	public Transform TTSTransform;

	// Token: 0x040017D1 RID: 6097
	[Space]
	public Transform bodyTopUpTransform;

	// Token: 0x040017D2 RID: 6098
	public Transform bodyTopSideTransform;

	// Token: 0x040017D3 RID: 6099
	[Space]
	public GameObject PhysRiderPoint;

	// Token: 0x040017D4 RID: 6100
	public PlayerEyes playerEyes;

	// Token: 0x040017D5 RID: 6101
	private GameObject PhysRiderPointInstance;

	// Token: 0x040017D6 RID: 6102
	[Space]
	public ParticleSystem[] powerupJumpEffect;

	// Token: 0x040017D7 RID: 6103
	public ParticleSystem[] tumbleBreakFreeEffect;

	// Token: 0x040017D8 RID: 6104
	public Transform effectGetIntoTruck;

	// Token: 0x040017D9 RID: 6105
	private float effectGetIntoTruckTimer;

	// Token: 0x040017DA RID: 6106
	[Space]
	public GameObject arenaCrown;

	// Token: 0x040017DB RID: 6107
	public Transform leanTransform;

	// Token: 0x040017DC RID: 6108
	public SpringQuaternion leanSpring;

	// Token: 0x040017DD RID: 6109
	private Vector3 leanSpringTargetPrevious;

	// Token: 0x040017DE RID: 6110
	[Space]
	public Transform tiltTransform;

	// Token: 0x040017DF RID: 6111
	public SpringQuaternion tiltSpring;

	// Token: 0x040017E0 RID: 6112
	private bool tiltSprinting;

	// Token: 0x040017E1 RID: 6113
	private float tiltTimer;

	// Token: 0x040017E2 RID: 6114
	private Vector3 tiltTarget;

	// Token: 0x040017E3 RID: 6115
	[Space]
	public SpringQuaternion bodySpring;

	// Token: 0x040017E4 RID: 6116
	[HideInInspector]
	public Quaternion bodySpringTarget;

	// Token: 0x040017E5 RID: 6117
	public Transform legTwistTransform;

	// Token: 0x040017E6 RID: 6118
	public SpringQuaternion legTwistSpring;

	// Token: 0x040017E7 RID: 6119
	private bool legTwistActive;

	// Token: 0x040017E8 RID: 6120
	public Transform headLookAtTransform;

	// Token: 0x040017E9 RID: 6121
	public SpringFloat lookUpSpring;

	// Token: 0x040017EA RID: 6122
	public SpringQuaternion lookSideSpring;

	// Token: 0x040017EB RID: 6123
	public Transform attachPointJawTop;

	// Token: 0x040017EC RID: 6124
	public Transform attachPointJawBottom;

	// Token: 0x040017ED RID: 6125
	public Transform attachPointTopHeadMiddle;

	// Token: 0x040017EE RID: 6126
	private Vector3 positionLast;

	// Token: 0x040017EF RID: 6127
	internal Vector3 visualPosition = Vector3.zero;

	// Token: 0x040017F0 RID: 6128
	private float visualFollowLerp;

	// Token: 0x040017F1 RID: 6129
	internal float turnDifference;

	// Token: 0x040017F2 RID: 6130
	internal float turnDirection;

	// Token: 0x040017F3 RID: 6131
	private float turnPrevious;

	// Token: 0x040017F4 RID: 6132
	internal float upDifference;

	// Token: 0x040017F5 RID: 6133
	internal float upDirection;

	// Token: 0x040017F6 RID: 6134
	private float upPrevious;

	// Token: 0x040017F7 RID: 6135
	internal float animationSpeedMultiplier = 1f;

	// Token: 0x040017F8 RID: 6136
	internal float deltaTime;

	// Token: 0x040017F9 RID: 6137
	internal Color color;

	// Token: 0x040017FA RID: 6138
	internal bool colorSet;

	// Token: 0x040017FB RID: 6139
	private bool crownSetterWasHere;
}
