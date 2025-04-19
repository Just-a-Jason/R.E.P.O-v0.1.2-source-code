using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class ItemDrone : MonoBehaviour
{
	// Token: 0x06000AA7 RID: 2727 RVA: 0x0005E218 File Offset: 0x0005C418
	private void Start()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name == "Particles")
			{
				this.teleportParticles = transform.gameObject;
				break;
			}
		}
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.droneCollider = base.GetComponentInChildren<Collider>();
		this.cameraMain = Camera.main;
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lineBetweenTwoPoints = base.GetComponent<LineBetweenTwoPoints>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		if (!this.itemBattery)
		{
			this.hasBattery = false;
		}
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.emojiIcon = this.itemAttributes.emojiIcon;
		this.colorPreset = this.itemAttributes.colorPreset;
		this.droneColor = this.colorPreset.GetColorMain();
		this.batteryColor = this.colorPreset.GetColorLight();
		this.beamColor = this.colorPreset.GetColorDark();
		this.batteryDrainRate = this.batteryDrainPreset.GetBatteryDrainRate();
		this.itemBattery.batteryDrainRate = this.batteryDrainRate;
		this.itemBattery.batteryColor = this.batteryColor;
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num];
			this.physicMaterialOriginal = collider.material;
		}
		Sound.CopySound(this.itemDroneSounds.DroneLoop, this.soundDroneLoop);
		Sound.CopySound(this.itemDroneSounds.DroneBeamLoop, this.soundDroneBeamLoop);
		ItemLight componentInChildren = base.GetComponentInChildren<ItemLight>();
		if (componentInChildren)
		{
			componentInChildren.itemLight.color = this.droneColor;
		}
		AudioSource component = base.GetComponent<AudioSource>();
		this.soundDroneLoop.Source = component;
		this.soundDroneBeamLoop.Source = component;
		foreach (object obj2 in base.transform)
		{
			Transform transform2 = (Transform)obj2;
			if (transform2.name == "Drone Icon")
			{
				this.onSwitchTransform = transform2;
				this.onSwitchTransform.GetComponent<Renderer>().material.SetTexture("_EmissionMap", this.droneIcon);
				this.onSwitchTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
			}
			if (transform2.name == "Drone")
			{
				this.droneTransform = transform2;
				foreach (object obj3 in transform2)
				{
					Transform transform3 = (Transform)obj3;
					if (transform3.name.Contains("Drone Triangle"))
					{
						foreach (object obj4 in transform3)
						{
							Transform item = (Transform)obj4;
							this.droneTriangleTransforms.Add(item);
						}
					}
					if (transform3.name.Contains("Drone Pyramid"))
					{
						foreach (object obj5 in transform3)
						{
							Transform transform4 = (Transform)obj5;
							this.dronePyramidTransforms.Add(transform4);
							transform4.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
						}
					}
				}
			}
		}
		this.droneTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0005E6BC File Offset: 0x0005C8BC
	private void AnimateDrone()
	{
		if (!this.itemActivated)
		{
			return;
		}
		this.lerpAnimationProgress += Time.deltaTime * 10f;
		if (this.lerpAnimationProgress > 1f)
		{
			this.lerpAnimationProgress = 1f;
			this.animationOpen = true;
		}
		float num = 15f;
		if (this.magnetActive)
		{
			num = 60f;
		}
		foreach (Transform transform in this.dronePyramidTransforms)
		{
			float num2 = -33f;
			if (this.lerpAnimationProgress != 1f)
			{
				transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(0f, num2, this.lerpAnimationProgress), 0f);
			}
			else
			{
				float num3 = Mathf.Sin(Time.time * num) * 5f;
				transform.localRotation = Quaternion.Euler(0f, num2 + num3, 0f);
			}
		}
		foreach (Transform transform2 in this.droneTriangleTransforms)
		{
			float num4 = 45f;
			if (this.lerpAnimationProgress != 1f)
			{
				transform2.localRotation = Quaternion.Euler(Mathf.Lerp(0f, num4, this.lerpAnimationProgress), 0f, 0f);
			}
			else
			{
				float num5 = Mathf.Sin(Time.time * num / 3f) * 10f;
				transform2.localRotation = Quaternion.Euler(num4 + num5, 0f, 0f);
			}
		}
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0005E878 File Offset: 0x0005CA78
	private bool TargetFindPlayer()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		this.playerAvatarTarget = null;
		this.playerTumbleTarget = null;
		float num = 10000f;
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 1f, LayerMask.GetMask(new string[]
		{
			"Player"
		})))
		{
			PlayerAvatar playerAvatar = collider.GetComponentInParent<PlayerAvatar>();
			if (!playerAvatar)
			{
				PlayerController componentInParent = collider.GetComponentInParent<PlayerController>();
				if (componentInParent)
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
			}
			if (playerAvatar && (this.customTargetingCondition == null || this.customTargetingCondition.CustomTargetingCondition(playerAvatar.gameObject)))
			{
				float num2 = Vector3.Distance(base.transform.position, playerAvatar.PlayerVisionTarget.VisionTransform.position);
				if (num2 < num)
				{
					num = num2;
					this.playerAvatarTarget = playerAvatar;
					this.targetIsPlayer = true;
					if (this.playerAvatarTarget.isLocal)
					{
						this.targetIsLocalPlayer = true;
					}
				}
			}
		}
		if (this.playerAvatarTarget)
		{
			Transform transform = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			Vector3 newAttachPoint = transform.position;
			if (this.playerAvatarTarget.isTumbling && this.playerAvatarTarget.transform)
			{
				this.playerTumbleTarget = this.playerAvatarTarget.tumble;
				transform = this.playerTumbleTarget.transform;
				newAttachPoint = this.playerTumbleTarget.physGrabObject.centerPoint;
			}
			this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
			this.targetIsPlayer = true;
			this.NewRayHitPoint(newAttachPoint, this.playerAvatarTarget.GetComponent<PhotonView>().ViewID, -1, transform);
			this.attachPoint = this.rayHitPosition;
			this.ActivateMagnet();
			return true;
		}
		return false;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0005EA50 File Offset: 0x0005CC50
	private void GetPlayerTumbleTarget()
	{
		if (!this.magnetTarget)
		{
			return;
		}
		if (this.playerTumbleTarget && !this.playerTumbleTarget.playerAvatar.isTumbling)
		{
			this.playerAvatarTarget = this.playerTumbleTarget.playerAvatar;
			this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
			this.targetIsPlayer = true;
			this.ActivateMagnet();
			this.playerTumbleTarget = null;
			Transform visionTransform = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			Vector3 position = visionTransform.position;
			this.NewRayHitPoint(position, this.playerAvatarTarget.GetComponent<PhotonView>().ViewID, -1, visionTransform);
		}
		if (this.playerAvatarTarget && this.playerAvatarTarget.isTumbling)
		{
			Vector3 position2 = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform.position;
			this.playerTumbleTarget = this.playerAvatarTarget.tumble;
			Transform transform = this.playerTumbleTarget.transform;
			Vector3 centerPoint = this.playerTumbleTarget.physGrabObject.centerPoint;
			this.targetIsLocalPlayer = false;
			this.targetIsPlayer = false;
			this.magnetTarget = this.playerTumbleTarget.transform;
			this.magnetTargetPhysGrabObject = this.playerTumbleTarget.physGrabObject;
			this.playerAvatarTarget = null;
			this.attachPoint = this.rayHitPosition;
			this.ActivateMagnet();
		}
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0005EBA0 File Offset: 0x0005CDA0
	private void FullReset()
	{
		this.hadTarget = false;
		this.magnetTarget = null;
		this.magnetTargetPhysGrabObject = null;
		this.magnetTargetRigidbody = null;
		this.DeactivateMagnet();
		this.playerTumbleTarget = null;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		this.attachPoint = Vector3.zero;
		this.attachPointFound = false;
		this.rayHitPosition = Vector3.zero;
		this.animatedRayHitPosition = Vector3.zero;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0005EC25 File Offset: 0x0005CE25
	private void ToggleOnFullInit()
	{
		if (!this.fullInit && this.itemActivated && !this.togglePrevious)
		{
			this.fullReset = false;
			this.fullInit = true;
			this.togglePrevious = true;
		}
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0005EC54 File Offset: 0x0005CE54
	private void ToggleOffFullReset()
	{
		if (!this.fullReset && !this.itemActivated && this.togglePrevious)
		{
			this.fullReset = true;
			this.fullInit = false;
			this.togglePrevious = false;
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0005EC83 File Offset: 0x0005CE83
	private void ToggleOffIfLostTarget()
	{
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0005EC88 File Offset: 0x0005CE88
	private void ToggleOffIfEnemyTargetIsDead()
	{
		if (this.magnetTarget && this.targetIsEnemy)
		{
			if (this.enemyTarget)
			{
				if (!this.enemyTarget.Spawned && this.itemToggle.toggleState)
				{
					this.ForceTurnOff();
					this.enemyTarget = null;
					return;
				}
			}
			else if (this.itemToggle.toggleState)
			{
				this.ForceTurnOff();
				this.enemyTarget = null;
			}
		}
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0005ECFC File Offset: 0x0005CEFC
	private void ToggleOffIfPlayerTargetIsDead()
	{
		if (SemiFunc.FPSImpulse5() && this.targetIsPlayer)
		{
			if (this.playerAvatarTarget && this.playerAvatarTarget.isDisabled && this.itemToggle.toggleState)
			{
				this.ButtonToggleSet(false);
				this.playerAvatarTarget = null;
			}
			if (this.playerTumbleTarget && this.playerTumbleTarget.playerAvatar.isDisabled && this.itemToggle.toggleState)
			{
				this.ButtonToggleSet(false);
				this.playerTumbleTarget = null;
			}
		}
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0005ED87 File Offset: 0x0005CF87
	private void ForceTurnOff()
	{
		this.itemBattery.BatteryToggle(false);
		this.ButtonToggleSet(false);
		this.itemToggle.ToggleItem(false, -1);
		this.hadTarget = false;
		this.itemActivated = false;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0005EDB8 File Offset: 0x0005CFB8
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.magnetActivePrev != this.magnetActive)
		{
			this.BatteryToggle(this.magnetActive);
			this.magnetActivePrev = this.magnetActive;
		}
		if (this.hadTarget && !this.magnetActive)
		{
			this.ForceTurnOff();
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		this.soundDroneLoop.PlayLoop(this.itemActivated, 2f, 2f, 1f);
		this.AnimateDrone();
		if (!this.itemActivated)
		{
			this.onNoBatteryTimer = 0f;
		}
		if (this.itemActivated)
		{
			this.physGrabObject.impactDetector.canHurtLogic = false;
		}
		else
		{
			this.physGrabObject.impactDetector.canHurtLogic = true;
		}
		if (this.itemActivated && this.magnetActive && this.magnetTarget && !this.itemEquippable.isEquipped)
		{
			if (this.rayHitPosition != Vector3.zero && !this.targetIsPlayer)
			{
				bool flag = false;
				if (this.playerTumbleTarget && this.playerTumbleTarget.playerAvatar.isLocal)
				{
					flag = true;
				}
				if (!flag)
				{
					this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.TransformPoint(this.animatedRayHitPosition));
					this.connectionPoint = this.magnetTarget.TransformPoint(this.animatedRayHitPosition);
				}
				else
				{
					Vector3 b = new Vector3(0f, -0.5f, 0f);
					Vector3 point = this.cameraMain.transform.position + b;
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point);
					this.connectionPoint = point;
				}
			}
			else
			{
				this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
				if (!this.targetIsPlayer)
				{
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTargetPhysGrabObject.midPoint);
					this.connectionPoint = this.magnetTargetPhysGrabObject.midPoint;
				}
				if (this.targetIsPlayer)
				{
					Vector3 zero = new Vector3(0f, -0.5f, 0f);
					if (this.playerAvatarTarget.isTumbling)
					{
						zero = Vector3.zero;
					}
					if (!this.targetIsLocalPlayer)
					{
						this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.position + zero);
						this.connectionPoint = this.magnetTarget.position + zero;
					}
					else
					{
						Vector3 point2 = this.cameraMain.transform.position + zero;
						this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point2);
						this.connectionPoint = point2;
					}
				}
			}
		}
		if (!this.itemActivated && !this.magnetActive && this.animationOpen)
		{
			this.lerpAnimationProgress += Time.deltaTime * 10f;
			if (this.lerpAnimationProgress > 1f)
			{
				this.lerpAnimationProgress = 1f;
				this.animationOpen = false;
			}
			foreach (Transform transform in this.dronePyramidTransforms)
			{
				float b2 = 0f;
				transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(-33f, b2, this.lerpAnimationProgress), 0f);
			}
			foreach (Transform transform2 in this.droneTriangleTransforms)
			{
				float b3 = 0f;
				transform2.localRotation = Quaternion.Euler(Mathf.Lerp(45f, b3, this.lerpAnimationProgress), 0f, 0f);
			}
		}
		this.GetPlayerTumbleTarget();
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.itemToggle.toggleState != this.itemActivated)
		{
			this.ButtonToggle();
		}
		if (this.physGrabObject.playerGrabbing.Count == 1)
		{
			this.lastPlayerToTouch = this.physGrabObject.playerGrabbing[0].transform;
		}
		if (!this.itemActivated)
		{
			return;
		}
		this.springConstant = 40f;
		this.dampingCoefficient = 10f;
		if (!this.magnetActive)
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 0.5f)
			{
				bool flag2 = false;
				if (this.targetPlayers && !flag2)
				{
					flag2 = this.TargetFindPlayer();
				}
				if (!flag2 && (this.targetValuables || this.targetNonValuables || this.targetEnemies))
				{
					flag2 = this.SphereCheck();
				}
				if (flag2)
				{
					this.hadTarget = true;
					this.ActivateMagnet();
				}
				this.checkTimer = 0f;
			}
		}
		else if (!this.attachPointFound)
		{
			if (!this.targetIsPlayer)
			{
				if (this.rayTimer <= 0f)
				{
					this.FindBeamAttachPosition();
					this.rayTimer = 0.5f;
				}
				else
				{
					this.rayTimer -= Time.deltaTime;
				}
			}
		}
		else if (this.rb.velocity.magnitude > 0.2f)
		{
			this.newAttachPointTimer += Time.deltaTime;
			if (this.newAttachPointTimer > 0.5f)
			{
				this.attachPointFound = false;
				this.newAttachPointTimer = 0f;
				this.rayTimer = 0f;
			}
		}
		if (this.itemActivated && this.hasBattery && this.itemBattery.batteryLife <= 0f)
		{
			if (!this.itemBattery.batteryActive)
			{
				this.itemBattery.BatteryToggle(true);
			}
			this.onNoBatteryTimer += Time.deltaTime;
			if (this.onNoBatteryTimer >= 1.5f)
			{
				this.ForceTurnOff();
				this.onNoBatteryTimer = 0f;
			}
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0005F418 File Offset: 0x0005D618
	public void ButtonToggleSet(bool toggle)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.ButtonToggleRPC(toggle);
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("ButtonToggleRPC", RpcTarget.All, new object[]
			{
				toggle
			});
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0005F450 File Offset: 0x0005D650
	private void FixedUpdate()
	{
		if (!this.itemActivated)
		{
			return;
		}
		if (this.magnetTarget)
		{
			ItemEquippable componentInParent = this.magnetTarget.GetComponentInParent<ItemEquippable>();
			if (componentInParent && componentInParent.isEquipped && this.magnetActive)
			{
				this.ForceTurnOff();
				this.DeactivateMagnet();
			}
		}
		if (this.itemEquippable.isEquipped)
		{
			if (this.magnetActive)
			{
				this.ForceTurnOff();
				this.DeactivateMagnet();
			}
			return;
		}
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.itemActivated)
		{
			return;
		}
		if (this.magnetActive)
		{
			if (!this.magnetTarget)
			{
				this.DeactivateMagnet();
				return;
			}
			if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 4f)
			{
				this.FindTeleportSpot();
			}
			Collider collider = null;
			if (this.magnetTarget)
			{
				collider = this.magnetTarget.GetComponent<Collider>();
			}
			if (!this.playerTumbleTarget && (!this.magnetTarget || !this.magnetTarget.gameObject.activeSelf || !this.magnetTarget.gameObject.activeInHierarchy || (collider && !collider.enabled)))
			{
				this.DeactivateMagnet();
				return;
			}
			this.physGrabObject.OverrideMaterial(this.physicMaterialSlippery, 0.1f);
			if (this.randomNudgeTimer <= 0f)
			{
				if (Vector3.Distance(base.transform.position, this.magnetTarget.transform.position) > 1.5f)
				{
					Vector3 lhs = base.transform.position - this.magnetTarget.transform.position;
					Vector3[] array = new Vector3[]
					{
						Vector3.up,
						Vector3.down,
						Vector3.left,
						Vector3.right
					};
					Vector3 rhs = array[Random.Range(0, array.Length)];
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					if (normalized != Vector3.zero)
					{
						this.rb.AddForce(normalized * 1f, ForceMode.Impulse);
						this.rb.AddTorque(normalized * 10f, ForceMode.Impulse);
					}
				}
				this.randomNudgeTimer = 0.5f;
			}
			else
			{
				this.randomNudgeTimer -= Time.fixedDeltaTime;
			}
			if (this.attachPointFound)
			{
				Vector3 a = this.magnetTarget.TransformPoint(this.attachPoint) - base.transform.position;
				Vector3 a2 = this.springConstant * a;
				Vector3 velocity = this.rb.velocity;
				Vector3 b = -this.dampingCoefficient * velocity;
				Vector3 vector = a2 + b;
				vector = Vector3.ClampMagnitude(vector, 20f);
				this.rb.AddForce(vector);
				if (!this.magnetTarget.gameObject.activeSelf)
				{
					this.DeactivateMagnet();
				}
				SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.magnetTarget.TransformPoint(this.rayHitPosition), 1f);
				return;
			}
			Vector3 vector2 = this.magnetTarget.position - base.transform.position;
			if (vector2.magnitude > 0.8f)
			{
				this.rb.AddForce(vector2.normalized * 3f);
			}
			else
			{
				Vector3 force = -this.rb.velocity * 0.9f;
				this.rb.AddForce(force);
			}
			if (!this.magnetTarget.gameObject.activeSelf)
			{
				this.DeactivateMagnet();
			}
			SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.magnetTarget.position, 1f);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0005F850 File Offset: 0x0005DA50
	[PunRPC]
	public void TeleportEffectRPC(Vector3 startPosition, Vector3 endPosition)
	{
		this.itemDroneSounds.DroneRetract.Pitch = 3f;
		this.itemDroneSounds.DroneRetract.Play(startPosition, 1f, 1f, 1f, 1f);
		this.itemDroneSounds.DroneRetract.Pitch = 4f;
		this.itemDroneSounds.DroneRetract.Play(endPosition, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.teleportParticles, startPosition, Quaternion.identity);
		Object.Instantiate<GameObject>(this.teleportParticles, endPosition, Quaternion.identity);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0005F8F8 File Offset: 0x0005DAF8
	private void TeleportEffect(Vector3 startPosition, Vector3 endPosition)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
				{
					startPosition,
					endPosition
				});
				return;
			}
		}
		else
		{
			this.TeleportEffectRPC(startPosition, endPosition);
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0005F948 File Offset: 0x0005DB48
	private void FindTeleportSpot()
	{
		if (!this.magnetActive)
		{
			return;
		}
		if (!this.magnetTarget)
		{
			return;
		}
		if (this.teleportSpotTimer > 0f)
		{
			this.teleportSpotTimer -= Time.deltaTime;
			return;
		}
		ItemEquippable componentInParent = this.magnetTarget.GetComponentInParent<ItemEquippable>();
		if (componentInParent && componentInParent.isEquipped)
		{
			return;
		}
		Vector3 vector = this.magnetTarget.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
		int i = 0;
		while (i < 10)
		{
			vector = this.magnetTarget.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			float num = Vector3.Distance(vector, this.magnetTarget.position);
			float maxDistance = Mathf.Max(0f, num - 0.2f);
			RaycastHit[] array = Physics.RaycastAll(vector, this.magnetTarget.position - vector, maxDistance, SemiFunc.LayerMaskGetVisionObstruct());
			bool flag = false;
			foreach (RaycastHit raycastHit in array)
			{
				if (raycastHit.transform.GetComponentInParent<Rigidbody>() != this.magnetTarget.GetComponentInParent<Rigidbody>() && raycastHit.transform != base.transform)
				{
					flag = true;
					break;
				}
			}
			if (!flag && Physics.OverlapBox(vector, new Vector3(0.2f, 0.2f, 0.2f), base.transform.rotation, SemiFunc.LayerMaskGetVisionObstruct()).Length == 0)
			{
				this.TeleportEffect(base.transform.position, vector);
				if (SemiFunc.IsMultiplayer())
				{
					this.physGrabObject.photonTransformView.Teleport(vector, base.transform.rotation);
					break;
				}
				base.transform.position = vector;
				break;
			}
			else
			{
				i++;
			}
		}
		this.teleportSpotTimer = 0.2f;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0005FB64 File Offset: 0x0005DD64
	private void DeactivateMagnet()
	{
		if (!this.magnetActive)
		{
			return;
		}
		this.attachPointFound = false;
		this.playerAvatarTarget = null;
		this.targetIsPlayer = false;
		this.targetIsLocalPlayer = false;
		this.targetIsEnemy = false;
		this.playerTumbleTarget = null;
		this.magnetTargetPhysGrabObject = null;
		this.magnetTargetRigidbody = null;
		this.MagnetActiveToggle(false);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0005FBB9 File Offset: 0x0005DDB9
	private void ActivateMagnet()
	{
		if (this.magnetActive)
		{
			return;
		}
		this.MagnetActiveToggle(true);
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0005FBCB File Offset: 0x0005DDCB
	private void BatteryToggle(bool activated)
	{
		if (this.hasBattery)
		{
			this.itemBattery.batteryActive = activated;
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0005FBE4 File Offset: 0x0005DDE4
	private void ButtonToggleLogic(bool activated)
	{
		this.FullReset();
		this.MagnetActiveToggle(activated);
		this.droneOwner = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		this.lerpAnimationProgress = 0f;
		if (activated)
		{
			this.onSwitchTransform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.droneColor);
			this.itemDroneSounds.DroneStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			if (this.magnetActive)
			{
				this.DeactivateMagnet();
			}
			this.itemDroneSounds.DroneEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.itemActivated = activated;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0005FCBC File Offset: 0x0005DEBC
	public void ButtonToggle()
	{
		this.itemActivated = !this.itemActivated;
		if (GameManager.instance.gameMode == 0)
		{
			this.ButtonToggleLogic(this.itemActivated);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ButtonToggleRPC", RpcTarget.All, new object[]
			{
				this.itemActivated
			});
		}
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0005FD1D File Offset: 0x0005DF1D
	[PunRPC]
	private void ButtonToggleRPC(bool activated)
	{
		this.ButtonToggleLogic(activated);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0005FD28 File Offset: 0x0005DF28
	private Transform GetHighestParentWithRigidbody(Transform child)
	{
		if (base.GetComponent<Rigidbody>() != null && child.GetComponent<PhotonView>() != null)
		{
			return child;
		}
		Transform transform = child;
		while (transform.parent != null)
		{
			if (transform.parent.GetComponent<Rigidbody>() != null && transform.parent.GetComponent<PhotonView>() != null)
			{
				return transform.parent;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0005FD9C File Offset: 0x0005DF9C
	private void MagnetActiveToggleLogic(bool activated)
	{
		this.magnetActive = activated;
		this.lerpAnimationProgress = 0f;
		if (!activated)
		{
			this.itemDroneSounds.DroneRetract.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.rayHitPosition = Vector3.zero;
			return;
		}
		this.itemDroneSounds.DroneDeploy.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0005FE2A File Offset: 0x0005E02A
	public void MagnetActiveToggle(bool toggleBool)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.MagnetActiveToggleLogic(toggleBool);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("MagnetActiveToggleRPC", RpcTarget.All, new object[]
			{
				toggleBool
			});
		}
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0005FE67 File Offset: 0x0005E067
	[PunRPC]
	private void MagnetActiveToggleRPC(bool activated)
	{
		this.MagnetActiveToggleLogic(activated);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0005FE70 File Offset: 0x0005E070
	private void NewRayHitPointLogic(Vector3 newRayHitPosition, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (newMagnetTarget)
		{
			this.magnetTargetPhysGrabObject = newMagnetTarget.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = newMagnetTarget.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
				this.targetIsPlayer = false;
				this.targetIsLocalPlayer = false;
			}
			else
			{
				this.magnetTarget = newMagnetTarget;
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
			PlayerTumble component = this.magnetTargetRigidbody.GetComponent<PlayerTumble>();
			if (component)
			{
				if (component.isTumbling)
				{
					this.playerTumbleTarget = component;
					return;
				}
				this.DeactivateMagnet();
				return;
			}
		}
		else
		{
			this.magnetTargetPhysGrabObject = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
				this.targetIsPlayer = false;
				this.targetIsLocalPlayer = false;
			}
			else
			{
				this.targetIsPlayer = true;
				this.playerAvatarTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>();
				this.magnetTarget = this.playerAvatarTarget.PlayerVisionTarget.VisionTransform;
				this.targetIsLocalPlayer = this.playerAvatarTarget.isLocal;
				if (PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().isLocal)
				{
					this.targetIsLocalPlayer = true;
				}
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
		}
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0005FFFC File Offset: 0x0005E1FC
	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (!GameManager.Multiplayer())
		{
			this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, newMagnetTarget);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("NewRayHitPointRPC", RpcTarget.All, new object[]
			{
				newAttachPoint,
				photonViewId,
				colliderID
			});
		}
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00060055 File Offset: 0x0005E255
	[PunRPC]
	private void NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID)
	{
		this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, null);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00060064 File Offset: 0x0005E264
	private bool SphereCheck()
	{
		this.playerTumbleTarget = null;
		this.playerAvatarTarget = null;
		this.targetIsPlayer = false;
		this.targetIsEnemy = false;
		this.targetIsLocalPlayer = false;
		bool result = false;
		if (this.itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 1f);
		float num = 10000f;
		foreach (Collider collider in array)
		{
			Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(collider.transform);
			PhysGrabObjectCollider component = collider.GetComponent<PhysGrabObjectCollider>();
			PhysGrabObject physGrabObject = null;
			bool flag = false;
			bool flag2 = false;
			this.targetIsEnemy = false;
			if (highestParentWithRigidbody)
			{
				PhysGrabObjectImpactDetector component2 = highestParentWithRigidbody.GetComponent<PhysGrabObjectImpactDetector>();
				physGrabObject = highestParentWithRigidbody.GetComponent<PhysGrabObject>();
				if (component2)
				{
					if (component2.isValuable)
					{
						flag2 = true;
					}
					if (component2.isEnemy)
					{
						flag = true;
						this.targetIsEnemy = true;
						this.enemyTarget = component2.GetComponentInParent<EnemyParent>();
					}
				}
			}
			bool flag3 = true;
			if (this.customTargetingCondition != null && highestParentWithRigidbody)
			{
				flag3 = this.customTargetingCondition.CustomTargetingCondition(highestParentWithRigidbody.gameObject);
			}
			bool flag4 = this.targetValuables && flag2;
			if (!flag4)
			{
				flag4 = (this.targetEnemies && flag);
			}
			if (!flag4)
			{
				flag4 = (this.targetNonValuables && !flag2);
			}
			if (component && highestParentWithRigidbody != base.transform && highestParentWithRigidbody && flag4 && flag3 && highestParentWithRigidbody.gameObject.activeSelf)
			{
				float num2 = Vector3.Distance(base.transform.position, physGrabObject.centerPoint);
				if (num2 < num)
				{
					bool flag5 = false;
					RaycastHit raycastHit;
					if (Physics.Raycast(base.transform.position, physGrabObject.centerPoint - base.transform.position, out raycastHit, (physGrabObject.centerPoint - base.transform.position).magnitude, LayerMask.GetMask(new string[]
					{
						"Default"
					})) && raycastHit.collider.transform != collider.transform && raycastHit.collider.transform != base.transform)
					{
						flag5 = true;
					}
					if (!flag5)
					{
						num = num2;
						this.magnetTarget = collider.transform;
						this.magnetTargetPhysGrabObject = physGrabObject;
						this.magnetTargetRigidbody = highestParentWithRigidbody.GetComponent<Rigidbody>();
						Vector3 position = collider.transform.position;
						this.NewRayHitPoint(position, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
						this.attachPoint = this.rayHitPosition;
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00060308 File Offset: 0x0005E508
	private void FindBeamAttachPosition()
	{
		if (!this.magnetTarget)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			float num = 0.5f;
			Vector3 b = new Vector3(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num));
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, this.magnetTarget.position - base.transform.position + b, out raycastHit, 1f, SemiFunc.LayerMaskGetPhysGrabObject()))
			{
				Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(raycastHit.collider.transform);
				PhysGrabObjectCollider component = raycastHit.collider.transform.GetComponent<PhysGrabObjectCollider>();
				if (component && highestParentWithRigidbody == this.magnetTargetPhysGrabObject.transform)
				{
					Vector3 normalized = (base.transform.position - raycastHit.point).normalized;
					this.NewRayHitPoint(raycastHit.point, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					Vector3 position = raycastHit.point + normalized * 0.5f;
					this.attachPoint = this.magnetTarget.InverseTransformPoint(position);
					this.attachPointFound = true;
				}
			}
		}
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00060460 File Offset: 0x0005E660
	public void SetTumbleTarget(PlayerTumble tumble)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetTumbleTargetRPC(tumble.photonView.ViewID);
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("SetTumbleTargetRPC", RpcTarget.All, new object[]
			{
				tumble.photonView.ViewID
			});
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000604B8 File Offset: 0x0005E6B8
	[PunRPC]
	public void SetTumbleTargetRPC(int _photonViewID)
	{
		PhotonView photonView = PhotonView.Find(_photonViewID);
		if (photonView)
		{
			PlayerTumble component = photonView.GetComponent<PlayerTumble>();
			if (component)
			{
				this.playerTumbleTarget = component;
			}
		}
	}

	// Token: 0x04001140 RID: 4416
	public GameObject teleportParticles;

	// Token: 0x04001141 RID: 4417
	private ItemAttributes itemAttributes;

	// Token: 0x04001142 RID: 4418
	[HideInInspector]
	public SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001143 RID: 4419
	public Texture droneIcon;

	// Token: 0x04001144 RID: 4420
	[HideInInspector]
	public ColorPresets colorPreset;

	// Token: 0x04001145 RID: 4421
	public BatteryDrainPresets batteryDrainPreset;

	// Token: 0x04001146 RID: 4422
	[HideInInspector]
	public float batteryDrainRate = 0.1f;

	// Token: 0x04001147 RID: 4423
	[HideInInspector]
	public Color droneColor;

	// Token: 0x04001148 RID: 4424
	[HideInInspector]
	public Color batteryColor;

	// Token: 0x04001149 RID: 4425
	[HideInInspector]
	public Color beamColor;

	// Token: 0x0400114A RID: 4426
	private float checkTimer;

	// Token: 0x0400114B RID: 4427
	private Transform magnetTarget;

	// Token: 0x0400114C RID: 4428
	[HideInInspector]
	public PhysGrabObject magnetTargetPhysGrabObject;

	// Token: 0x0400114D RID: 4429
	[HideInInspector]
	public Rigidbody magnetTargetRigidbody;

	// Token: 0x0400114E RID: 4430
	[HideInInspector]
	public bool magnetActive;

	// Token: 0x0400114F RID: 4431
	public PlayerTumble playerTumbleTarget;

	// Token: 0x04001150 RID: 4432
	private Rigidbody rb;

	// Token: 0x04001151 RID: 4433
	private bool attachPointFound;

	// Token: 0x04001152 RID: 4434
	private Vector3 attachPoint;

	// Token: 0x04001153 RID: 4435
	private float springConstant = 50f;

	// Token: 0x04001154 RID: 4436
	private float dampingCoefficient = 5f;

	// Token: 0x04001155 RID: 4437
	private float newAttachPointTimer;

	// Token: 0x04001156 RID: 4438
	[HideInInspector]
	public bool itemActivated;

	// Token: 0x04001157 RID: 4439
	private PhotonView photonView;

	// Token: 0x04001158 RID: 4440
	private Vector3 rayHitPosition;

	// Token: 0x04001159 RID: 4441
	private Vector3 animatedRayHitPosition;

	// Token: 0x0400115A RID: 4442
	private LineBetweenTwoPoints lineBetweenTwoPoints;

	// Token: 0x0400115B RID: 4443
	public Transform lineStartPoint;

	// Token: 0x0400115C RID: 4444
	private float rayTimer;

	// Token: 0x0400115D RID: 4445
	private Transform prevMagnetTarget;

	// Token: 0x0400115E RID: 4446
	private Transform droneTransform;

	// Token: 0x0400115F RID: 4447
	private List<Transform> dronePyramidTransforms = new List<Transform>();

	// Token: 0x04001160 RID: 4448
	private List<Transform> droneTriangleTransforms = new List<Transform>();

	// Token: 0x04001161 RID: 4449
	private float lerpAnimationProgress;

	// Token: 0x04001162 RID: 4450
	private bool hasBattery = true;

	// Token: 0x04001163 RID: 4451
	private ItemBattery itemBattery;

	// Token: 0x04001164 RID: 4452
	private float onNoBatteryTimer;

	// Token: 0x04001165 RID: 4453
	private bool animationOpen;

	// Token: 0x04001166 RID: 4454
	private Transform onSwitchTransform;

	// Token: 0x04001167 RID: 4455
	public ItemDroneSounds itemDroneSounds;

	// Token: 0x04001168 RID: 4456
	[HideInInspector]
	public Sound soundDroneLoop;

	// Token: 0x04001169 RID: 4457
	[HideInInspector]
	public Sound soundDroneBeamLoop;

	// Token: 0x0400116A RID: 4458
	public PhysicMaterial physicMaterialSlippery;

	// Token: 0x0400116B RID: 4459
	public bool targetValuables;

	// Token: 0x0400116C RID: 4460
	public bool targetPlayers;

	// Token: 0x0400116D RID: 4461
	public bool targetEnemies;

	// Token: 0x0400116E RID: 4462
	public bool targetNonValuables;

	// Token: 0x0400116F RID: 4463
	[HideInInspector]
	public Vector3 connectionPoint;

	// Token: 0x04001170 RID: 4464
	[HideInInspector]
	public Transform lastPlayerToTouch;

	// Token: 0x04001171 RID: 4465
	private PhysGrabObject physGrabObject;

	// Token: 0x04001172 RID: 4466
	private float randomNudgeTimer;

	// Token: 0x04001173 RID: 4467
	private Collider droneCollider;

	// Token: 0x04001174 RID: 4468
	private PhysicMaterial physicMaterialOriginal;

	// Token: 0x04001175 RID: 4469
	private ItemToggle itemToggle;

	// Token: 0x04001176 RID: 4470
	internal PlayerAvatar playerAvatarTarget;

	// Token: 0x04001177 RID: 4471
	private bool targetIsPlayer;

	// Token: 0x04001178 RID: 4472
	internal bool targetIsLocalPlayer;

	// Token: 0x04001179 RID: 4473
	private ItemEquippable itemEquippable;

	// Token: 0x0400117A RID: 4474
	private Camera cameraMain;

	// Token: 0x0400117B RID: 4475
	internal PlayerAvatar droneOwner;

	// Token: 0x0400117C RID: 4476
	private float teleportSpotTimer;

	// Token: 0x0400117D RID: 4477
	private bool hadTarget;

	// Token: 0x0400117E RID: 4478
	private bool targetIsEnemy;

	// Token: 0x0400117F RID: 4479
	private bool togglePrevious;

	// Token: 0x04001180 RID: 4480
	private bool fullReset;

	// Token: 0x04001181 RID: 4481
	private bool fullInit;

	// Token: 0x04001182 RID: 4482
	private EnemyParent enemyTarget;

	// Token: 0x04001183 RID: 4483
	private bool magnetActivePrev;

	// Token: 0x04001184 RID: 4484
	private ITargetingCondition customTargetingCondition;
}
