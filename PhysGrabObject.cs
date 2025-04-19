using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class PhysGrabObject : MonoBehaviour, IPunObservable
{
	// Token: 0x06000D40 RID: 3392 RVA: 0x00075DFC File Offset: 0x00073FFC
	private void Awake()
	{
		this.photonTransformView = base.GetComponent<PhotonTransformView>();
		if (!this.photonTransformView)
		{
			Debug.LogError("No Photon Transform View found on " + base.gameObject.name);
		}
		this.physGrabCart = base.GetComponent<PhysGrabCart>();
		this.isCart = this.physGrabCart;
		this.forceGrabPoint = base.transform.Find("Force Grab Point");
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.isKinematic = true;
		Transform transform = base.transform.Find("Center of Mass");
		if (transform)
		{
			this.rb.centerOfMass = transform.localPosition;
		}
		this.rb.interpolation = RigidbodyInterpolation.Interpolate;
		this.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		this.angularDragOriginal = this.rb.angularDrag;
		this.dragOriginal = this.rb.drag;
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		if (this.impactDetector)
		{
			this.hasImpactDetector = true;
		}
		if (base.GetComponent<ValuableObject>())
		{
			this.isValuable = true;
		}
		this.mapCustom = base.GetComponent<MapCustom>();
		if (this.mapCustom)
		{
			this.hasMapCustom = true;
		}
		foreach (Transform transform2 in base.GetComponentsInParent<Transform>())
		{
			if (transform2.name.Contains("debug") || transform2.name.Contains("Debug"))
			{
				this.spawned = true;
			}
		}
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00075F84 File Offset: 0x00074184
	public void TurnXYZ(Quaternion turnX, Quaternion turnY, Quaternion turnZ)
	{
		Vector3 point = turnY * Vector3.forward;
		Vector3 point2 = turnY * Vector3.up;
		point = turnZ * point;
		point2 = turnZ * point2;
		foreach (PhysGrabber physGrabber in this.playerGrabbing)
		{
			physGrabber.cameraRelativeGrabbedForward = turnX * point;
			physGrabber.cameraRelativeGrabbedUp = turnX * point2;
		}
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00076010 File Offset: 0x00074210
	public void TorqueToTarget(PhysGrabber player, Quaternion target, float strength, float dampen)
	{
		if (this.rb.isKinematic)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Vector3 forward = base.transform.forward;
		Vector3 up = base.transform.up;
		Vector3 vector2 = target * Vector3.forward;
		Vector3 vector3 = target * Vector3.up;
		player.cameraRelativeGrabbedUp = vector3;
		player.cameraRelativeGrabbedForward = vector2;
		Vector3 vector4 = Vector3.Cross(forward, vector2);
		if (vector4.sqrMagnitude > 1E-08f)
		{
			float value = Vector3.Angle(forward, vector2);
			vector += vector4.normalized * Mathf.Clamp(value, 0f, 60f);
		}
		Vector3 vector5 = Vector3.Cross(up, vector3);
		if (vector5.sqrMagnitude > 1E-08f)
		{
			float value2 = Vector3.Angle(up, vector3);
			vector += vector5.normalized * Mathf.Clamp(value2, 0f, 60f);
		}
		vector *= this.rb.mass;
		vector = Vector3.ClampMagnitude(vector, 60f).normalized;
		if (this.rb.mass < 1f)
		{
			vector *= 0.75f;
		}
		float num = Vector3.Angle(base.transform.forward, vector2) / 180f;
		float num2 = Vector3.Angle(base.transform.up, vector3) / 180f;
		float num3 = Mathf.Clamp01(this.rb.mass);
		float d = (num + num2) * dampen * num3;
		vector *= strength;
		vector *= d;
		Vector3 torque = vector * num3;
		this.rb.AddTorque(torque, ForceMode.Impulse);
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x000761B8 File Offset: 0x000743B8
	private void Start()
	{
		if (!SemiFunc.IsMultiplayer() && this.photonTransformView)
		{
			this.photonTransformView.enabled = false;
			this.photonTransformView = null;
		}
		this.mainCamera = Camera.main;
		this.isEnemy = base.GetComponent<EnemyRigidbody>();
		this.isMelee = base.GetComponent<ItemMelee>();
		if (!this.isEnemy)
		{
			this.isNonValuable = base.GetComponent<NotValuableObject>();
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		bool flag = false;
		foreach (Collider collider in componentsInChildren)
		{
			if (!collider.isTrigger)
			{
				if (flag)
				{
					bounds.Encapsulate(collider.bounds);
				}
				else
				{
					bounds = collider.bounds;
					flag = true;
				}
			}
		}
		base.transform.rotation = rotation;
		if (flag)
		{
			this.boundingBox = bounds.size;
			this.midPointOffset = base.transform.InverseTransformPoint(bounds.center);
		}
		else
		{
			this.boundingBox = Vector3.one;
			Debug.LogWarning("No colliders found on the object or its children!");
		}
		int num = 0;
		foreach (PhysGrabObjectCollider physGrabObjectCollider in base.GetComponentsInChildren<PhysGrabObjectCollider>())
		{
			this.colliders.Add(physGrabObjectCollider.transform);
			physGrabObjectCollider.colliderID = num;
			num++;
		}
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.photonView = base.GetComponent<PhotonView>();
		this.hinge = base.GetComponent<PhysGrabHinge>();
		if (this.hinge)
		{
			this.hasHinge = true;
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				this.prevTargetPos = base.transform.position;
				this.prevTargetRot = base.transform.rotation;
				this.targetPos = base.transform.position;
				this.targetRot = base.transform.rotation;
				this.isMaster = true;
			}
			if (PhotonNetwork.IsMasterClient && this.spawned)
			{
				this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!base.GetComponent<EnemyRigidbody>())
			{
				base.StartCoroutine(this.EnableRigidbody());
			}
		}
		else
		{
			this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
		if (this.overrideTagsAndLayers)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (!transform.CompareTag("Cart") && !transform.CompareTag("Grab Area"))
				{
					if (transform.gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision") && transform.gameObject.layer != LayerMask.NameToLayer("Triggers"))
					{
						transform.gameObject.tag = "Phys Grab Object";
					}
					if (transform.gameObject.layer != LayerMask.NameToLayer("IgnorePhysGrab") && transform.gameObject.layer != LayerMask.NameToLayer("CollisionCheck") && transform.gameObject.layer != LayerMask.NameToLayer("CartWheels") && transform.gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectHinge") && transform.gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectCart") && transform.gameObject.layer != LayerMask.NameToLayer("Triggers") && transform.gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision"))
					{
						transform.gameObject.layer = LayerMask.NameToLayer("PhysGrabObject");
					}
				}
			}
		}
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x000765B4 File Offset: 0x000747B4
	public void OverrideFragility(float multiplier)
	{
		if (!this.impactDetector)
		{
			return;
		}
		if (!this.isValuable)
		{
			return;
		}
		this.overrideFragilityTimer = 0.1f;
		this.impactDetector.fragilityMultiplier = multiplier;
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x000765E4 File Offset: 0x000747E4
	private void OverrideTimersTick()
	{
		if (this.timerAlterDeactivate > 0f)
		{
			if (this.isActive)
			{
				base.transform.position = new Vector3(0f, 3000f, 0f);
			}
			this.isActive = false;
			this.rb.detectCollisions = false;
			this.rb.isKinematic = true;
			if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel() && this.photonTransformView.enabled)
			{
				this.photonTransformView.enabled = false;
			}
			this.timerAlterDeactivate -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterDeactivate != -123f)
		{
			this.OverrideDeactivateReset();
		}
		if (this.mapCustom && this.hasMapCustom && !this.isActive)
		{
			this.mapCustom.Hide();
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.OverrideStrengthTick();
		if (this.overrideMassGoDownTimer > 0f)
		{
			this.overrideMassGoDownTimer -= Time.fixedDeltaTime;
		}
		if (this.timerAlterMass > 0f)
		{
			this.rb.mass = this.alterMassValue;
			this.timerAlterMass -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterMass != -123f)
		{
			if (this.massOriginal == 0f)
			{
				this.massOriginal = this.rb.mass;
			}
			this.ResetMass();
		}
		if (this.impactDetector)
		{
			if (this.overrideFragilityTimer > 0f)
			{
				this.overrideFragilityTimer -= Time.fixedDeltaTime;
			}
			else if (this.overrideFragilityTimer != -123f)
			{
				if (this.impactDetector.fragilityMultiplier != 1f)
				{
					this.impactDetector.fragilityMultiplier = 1f;
				}
				this.overrideFragilityTimer = -123f;
			}
		}
		if (this.overrideAngularDragGoDownTimer > 0f)
		{
			this.overrideAngularDragGoDownTimer -= Time.fixedDeltaTime;
		}
		if (this.timerAlterAngularDrag > 0f)
		{
			this.rb.angularDrag = this.alterAngularDragValue;
			this.timerAlterAngularDrag -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterAngularDrag != -123f)
		{
			this.rb.angularDrag = this.angularDragOriginal;
			this.timerAlterAngularDrag = -123f;
			this.alterAngularDragValue = 0f;
		}
		if (this.overrideDragGoDownTimer > 0f)
		{
			this.overrideDragGoDownTimer -= Time.fixedDeltaTime;
		}
		if (this.timerAlterDrag > 0f)
		{
			this.rb.drag = this.alterDragValue;
			this.timerAlterDrag -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterDrag != -123f)
		{
			this.rb.drag = this.dragOriginal;
			this.timerAlterDrag = -123f;
			this.alterDragValue = 0f;
		}
		if (this.timerAlterIndestructible > 0f)
		{
			if (this.impactDetector)
			{
				this.impactDetector.isIndestructible = true;
			}
			this.timerAlterIndestructible -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterIndestructible != -123f)
		{
			this.ResetIndestructible();
		}
		if (this.timerAlterMaterial > 0f)
		{
			this.timerAlterMaterial -= Time.fixedDeltaTime;
		}
		else if (this.timerAlterMaterial != -123f)
		{
			foreach (Transform transform in this.colliders)
			{
				if (transform)
				{
					transform.GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
				}
				else
				{
					this.colliders.Remove(transform);
				}
			}
			this.timerAlterMaterial = -123f;
			this.alterMaterialCurrent = null;
		}
		if (this.timerZeroGravity > 0f)
		{
			this.rb.useGravity = false;
			this.timerZeroGravity -= Time.fixedDeltaTime;
		}
		else if (this.timerZeroGravity != -123f)
		{
			this.rb.useGravity = true;
			this.timerZeroGravity = -123f;
		}
		if ((!this.hasHinge || this.hinge.dead || this.hinge.broken) && this.rb.useGravity)
		{
			if (this.grabbed)
			{
				if (this.timerAlterAngularDrag <= 0f)
				{
					this.rb.angularDrag = 0.5f;
				}
				if (this.timerAlterDrag <= 0f)
				{
					this.rb.drag = 0.5f;
					return;
				}
			}
			else
			{
				if (this.timerAlterAngularDrag <= 0f)
				{
					this.rb.angularDrag = this.angularDragOriginal;
				}
				if (this.timerAlterDrag <= 0f)
				{
					this.rb.drag = this.dragOriginal;
				}
			}
		}
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00076AB4 File Offset: 0x00074CB4
	private IEnumerator EnableRigidbody()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.1f);
		this.spawned = true;
		this.rb.isKinematic = false;
		if (this.spawnTorque != Vector3.zero)
		{
			this.rb.AddTorque(this.spawnTorque, ForceMode.Impulse);
		}
		yield break;
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00076AC4 File Offset: 0x00074CC4
	private void TickImpactTimers()
	{
		if (this.impactHappenedTimer > 0f)
		{
			this.impactHappenedTimer -= Time.fixedDeltaTime;
		}
		if (this.impactLightTimer > 0f)
		{
			this.impactLightTimer -= Time.fixedDeltaTime;
		}
		if (this.impactMediumTimer > 0f)
		{
			this.impactMediumTimer -= Time.fixedDeltaTime;
		}
		if (this.impactHeavyTimer > 0f)
		{
			this.impactHeavyTimer -= Time.fixedDeltaTime;
		}
		if (this.breakLightTimer > 0f)
		{
			this.breakLightTimer -= Time.fixedDeltaTime;
		}
		if (this.breakMediumTimer > 0f)
		{
			this.breakMediumTimer -= Time.fixedDeltaTime;
		}
		if (this.breakHeavyTimer > 0f)
		{
			this.breakHeavyTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00076BAA File Offset: 0x00074DAA
	public void OverrideGrabStrength(float value, float time = 0.1f)
	{
		this.overrideGrabStrengthTimer = time;
		this.overrideGrabStrength = value;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00076BBA File Offset: 0x00074DBA
	public void OverrideTorqueStrengthX(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthXTimer = time;
		this.overrideTorqueStrengthX = value;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00076BCA File Offset: 0x00074DCA
	public void OverrideTorqueStrengthY(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthYTimer = time;
		this.overrideTorqueStrengthY = value;
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00076BDA File Offset: 0x00074DDA
	public void OverrideTorqueStrengthZ(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthZTimer = time;
		this.overrideTorqueStrengthZ = value;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00076BEA File Offset: 0x00074DEA
	public void OverrideTorqueStrength(float value, float time = 0.1f)
	{
		this.overrideTorqueStrengthTimer = time;
		this.overrideTorqueStrength = value;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00076BFC File Offset: 0x00074DFC
	public void OverrideStrengthTick()
	{
		if (this.overrideTorqueStrengthXTimer > 0f)
		{
			this.overrideTorqueStrengthXTimer -= Time.fixedDeltaTime;
			if (this.overrideTorqueStrengthXTimer <= 0f)
			{
				this.overrideTorqueStrengthX = 1f;
			}
		}
		if (this.overrideTorqueStrengthYTimer > 0f)
		{
			this.overrideTorqueStrengthYTimer -= Time.fixedDeltaTime;
			if (this.overrideTorqueStrengthYTimer <= 0f)
			{
				this.overrideTorqueStrengthY = 1f;
			}
		}
		if (this.overrideTorqueStrengthZTimer > 0f)
		{
			this.overrideTorqueStrengthZTimer -= Time.fixedDeltaTime;
			if (this.overrideTorqueStrengthZTimer <= 0f)
			{
				this.overrideTorqueStrengthZ = 1f;
			}
		}
		if (this.overrideTorqueStrengthTimer > 0f)
		{
			this.overrideTorqueStrengthTimer -= Time.fixedDeltaTime;
			if (this.overrideTorqueStrengthTimer <= 0f)
			{
				this.overrideTorqueStrength = 1f;
			}
		}
		if (this.overrideGrabStrengthTimer > 0f)
		{
			this.overrideGrabStrengthTimer -= Time.fixedDeltaTime;
			if (this.overrideGrabStrengthTimer <= 0f)
			{
				this.overrideGrabStrength = 1f;
			}
		}
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00076D1C File Offset: 0x00074F1C
	private void FixedUpdate()
	{
		this.TickImpactTimers();
		this.OverrideTimersTick();
		this.OverrideGrabRelativePositionTick();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rbVelocity = this.rb.velocity;
			this.rbAngularVelocity = this.rb.angularVelocity;
			this.isKinematic = this.rb.isKinematic;
			if (!this.isKinematic)
			{
				float maxLength = 40f;
				float maxLength2 = 30f;
				this.rb.velocity = Vector3.ClampMagnitude(this.rb.velocity, maxLength);
				this.rb.angularVelocity = Vector3.ClampMagnitude(this.rb.angularVelocity, maxLength2);
			}
		}
		if (this.frozenTimer > 0f)
		{
			this.frozenTimer -= Time.fixedDeltaTime;
			this.rb.MovePosition(this.frozenPosition);
			this.rb.MoveRotation(this.frozenRotation);
			if (!this.rb.isKinematic)
			{
				this.rb.velocity = Vector3.zero;
				this.rbVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				this.rbAngularVelocity = Vector3.zero;
			}
			return;
		}
		if (this.frozen)
		{
			this.rb.AddForce(this.frozenVelocity, ForceMode.VelocityChange);
			this.rb.AddTorque(this.frozenAngularVelocity, ForceMode.VelocityChange);
			this.rb.AddForce(this.frozenForce, ForceMode.Impulse);
			this.rb.AddTorque(this.frozenTorque, ForceMode.Impulse);
			this.frozenForce = Vector3.zero;
			this.frozenTorque = Vector3.zero;
			this.frozen = false;
			return;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.rbVelocity = this.rb.velocity;
			this.rbAngularVelocity = this.rb.angularVelocity;
		}
		if (this.playerGrabbing.Count > 0)
		{
			if (this.hasNeverBeenGrabbed)
			{
				this.OverrideIndestructible(0.5f);
				this.hasNeverBeenGrabbed = false;
			}
			this.grabbed = true;
			this.heldByLocalPlayer = false;
			if (GameManager.Multiplayer())
			{
				using (List<PhysGrabber>.Enumerator enumerator = this.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.photonView.IsMine)
						{
							this.heldByLocalPlayer = true;
						}
					}
					goto IL_255;
				}
			}
			this.heldByLocalPlayer = true;
		}
		else
		{
			this.heldByLocalPlayer = false;
			this.grabbed = false;
		}
		IL_255:
		if ((!GameManager.Multiplayer() || this.isMaster) && !this.rb.isKinematic)
		{
			Vector3 vector = Vector3.zero;
			this.grabDisplacementCurrent = Vector3.zero;
			foreach (PhysGrabber physGrabber in this.playerGrabbing)
			{
				float num = physGrabber.forceMax;
				if (this.isCart && this.physGrabCart.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber.transform.position))
				{
					num *= 0.25f;
				}
				physGrabber.grabbedPhysGrabObject = this;
				if (!physGrabber.physGrabForcesDisabled)
				{
					Vector3 a = physGrabber.physGrabPointPullerPosition;
					if (this.overrideGrabRelativeVerticalPositionTimer != 0f)
					{
						Vector3 up = physGrabber.playerAvatar.localCameraTransform.up;
						a += up * this.overrideGrabRelativeVerticalPosition;
					}
					if (this.overrideGrabRelativeHorizontalPositionTimer != 0f)
					{
						Vector3 right = physGrabber.playerAvatar.localCameraTransform.right;
						a += right * this.overrideGrabRelativeHorizontalPosition;
					}
					Vector3 vector2 = Vector3.ClampMagnitude(a - physGrabber.physGrabPoint.position, num) * 10f;
					vector2 = Vector3.ClampMagnitude(vector2, num);
					Vector3 pointVelocity = this.rb.GetPointVelocity(physGrabber.physGrabPoint.position);
					Vector3 vector3 = vector2 * physGrabber.springConstant - pointVelocity * physGrabber.dampingConstant;
					vector3 = Vector3.ClampMagnitude(vector3, num) * 2f;
					if (this.isMelee)
					{
						vector3 = Vector3.ClampMagnitude(vector3, num) * 4f;
					}
					vector3 *= physGrabber.grabStrength;
					vector3 *= this.overrideGrabStrength;
					Vector3 vector4 = vector3 * physGrabber.forceConstant;
					if (this.hasHinge && !this.hinge.dead && !this.hinge.broken)
					{
						vector4 *= 2f;
					}
					using (List<PhysGrabObject>.Enumerator enumerator2 = physGrabber.playerAvatar.physObjectStander.physGrabObjects.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current == this && vector4.y > 0f)
							{
								vector4.y = 0f;
							}
						}
					}
					if (this.isCart && this.physGrabCart.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber.playerAvatar.transform.position) && vector4.y > 0f)
					{
						vector4.y = 0f;
					}
					this.rb.AddForceAtPosition(vector4, physGrabber.physGrabPoint.position);
					this.grabDisplacementCurrent += vector2 * physGrabber.grabStrength;
					if (!this.hasHinge || this.hinge.dead || this.hinge.broken)
					{
						Transform localCameraTransform = physGrabber.playerAvatar.localCameraTransform;
						Vector3 vector5 = localCameraTransform.TransformDirection(physGrabber.physRotation * physGrabber.cameraRelativeGrabbedForward);
						Vector3 vector6 = localCameraTransform.TransformDirection(physGrabber.physRotation * physGrabber.cameraRelativeGrabbedUp);
						Vector3 forward = base.transform.forward;
						Vector3 up2 = base.transform.up;
						Vector3 vector7 = Vector3.zero;
						Vector3 vector8 = Vector3.Cross(forward, vector5);
						if (vector8.sqrMagnitude > 1E-08f)
						{
							vector7 += vector8.normalized * Mathf.Clamp(Vector3.Angle(forward, vector5), 0f, 60f);
						}
						Vector3 vector9 = Vector3.Cross(up2, vector6);
						if (vector9.sqrMagnitude > 1E-08f)
						{
							vector7 += vector9.normalized * Mathf.Clamp(Vector3.Angle(up2, vector6), 0f, 60f);
						}
						vector7 *= this.rb.mass;
						vector7 = Vector3.ClampMagnitude(vector7, 60f).normalized;
						if (this.rb.mass < 1f)
						{
							vector7 *= 0.75f;
						}
						if (this.isMelee && physGrabber.grabStrength > 0f)
						{
							this.OverrideMass(this.massOriginal + physGrabber.grabStrength * 0.2f, 0.1f);
						}
						if (this.isMelee)
						{
							Vector3 vector10 = Vector3.zero;
							Vector3 vector11 = Vector3.Cross(base.transform.forward, vector5);
							if (vector11.sqrMagnitude > 1E-08f)
							{
								vector10 = vector11.normalized * Vector3.Angle(base.transform.forward, vector5);
							}
							Vector3 a2 = Vector3.zero;
							Vector3 vector12 = Vector3.Cross(base.transform.up, vector6);
							if (vector12.sqrMagnitude > 1E-08f)
							{
								a2 = vector12.normalized * Vector3.Angle(base.transform.up, vector6);
							}
							vector7 = vector10.normalized + a2 * 3f;
							float d = 0.8f;
							vector7 *= d;
						}
						float num2 = Mathf.Clamp01(this.rb.mass);
						num2 = Mathf.Max(num2, 1f);
						if (this.massOriginal > 2f)
						{
							num2 *= physGrabber.grabStrength;
						}
						if (physGrabber.mouseTurningVelocity.magnitude > 0.1f && this.massOriginal > 1f)
						{
							float num3 = Mathf.Max(this.rb.mass, 0.1f);
							float num4 = 2f / num3;
							float num5 = 1f + this.boundingBox.magnitude;
							num4 += num5;
							if (num4 < 1f)
							{
								num4 = 1f;
							}
							if (num4 > 10f)
							{
								num4 = 10f;
							}
							vector7 *= num4;
						}
						float num6 = Mathf.Clamp01(this.rb.mass);
						if (this.rb.mass > 1f)
						{
							num6 *= 0.9f;
						}
						if (!this.isMelee)
						{
							vector7 *= 5f;
						}
						else
						{
							vector7 *= 0.05f;
						}
						vector7 *= this.overrideTorqueStrength;
						float num7 = Vector3.Angle(base.transform.forward, vector5) / 180f;
						float num8 = Vector3.Angle(base.transform.up, vector6) / 180f;
						float num9 = num7 + num8;
						vector7 *= num9 * 15f * num6 * Time.fixedDeltaTime;
						Vector3 direction = base.transform.InverseTransformDirection(vector7);
						float num10 = this.overrideTorqueStrengthX;
						float num11 = this.overrideTorqueStrengthY;
						float num12 = this.overrideTorqueStrengthZ;
						if (num10 > 1f)
						{
							num10 *= num9;
						}
						if (num11 > 1f)
						{
							num11 *= num9;
						}
						if (num12 > 1f)
						{
							num12 *= num9;
						}
						direction.x *= num10;
						direction.y *= num11;
						direction.z *= num12;
						vector7 = base.transform.TransformDirection(direction);
						Vector3 b = vector7 * num6;
						vector += b;
						this.rb.angularVelocity *= 0.9f;
					}
				}
			}
			if (vector.magnitude > 0f)
			{
				this.rb.AddTorque(vector, ForceMode.Impulse);
			}
		}
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x00077798 File Offset: 0x00075998
	public void OverrideGrabVerticalPosition(float pos)
	{
		this.overrideGrabRelativeVerticalPosition = pos;
		this.overrideGrabRelativeVerticalPositionTimer = 0.1f;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x000777AC File Offset: 0x000759AC
	public void OverrideGrabHorizontalPosition(float pos)
	{
		this.overrideGrabRelativeHorizontalPosition = pos;
		this.overrideGrabRelativeHorizontalPositionTimer = 0.1f;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x000777C0 File Offset: 0x000759C0
	private void OverrideGrabRelativePositionTick()
	{
		if (this.overrideGrabRelativeHorizontalPositionTimer > 0f)
		{
			this.overrideGrabRelativeHorizontalPositionTimer -= Time.deltaTime;
			if (this.overrideGrabRelativeHorizontalPositionTimer <= 0f)
			{
				this.overrideGrabRelativeHorizontalPosition = 0f;
			}
		}
		if (this.overrideGrabRelativeVerticalPositionTimer > 0f)
		{
			this.overrideGrabRelativeVerticalPositionTimer -= Time.deltaTime;
			if (this.overrideGrabRelativeVerticalPositionTimer <= 0f)
			{
				this.overrideGrabRelativeVerticalPosition = 0f;
			}
		}
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0007783B File Offset: 0x00075A3B
	public void OverrideZeroGravity(float time = 0.1f)
	{
		this.timerZeroGravity = time;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00077844 File Offset: 0x00075A44
	public void OverrideDrag(float value, float time = 0.1f)
	{
		this.timerAlterDrag = time;
		if (this.alterDragValue <= value)
		{
			this.alterDragValue = value;
			this.overrideDragGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideDragGoDownTimer <= 0f)
		{
			this.alterDragValue = value;
		}
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0007787D File Offset: 0x00075A7D
	public void OverrideAngularDrag(float value, float time = 0.1f)
	{
		this.timerAlterAngularDrag = time;
		if (this.alterAngularDragValue <= value)
		{
			this.alterAngularDragValue = value;
			this.overrideAngularDragGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideAngularDragGoDownTimer <= 0f)
		{
			this.timerAlterAngularDrag = value;
		}
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x000778B6 File Offset: 0x00075AB6
	public void OverrideIndestructible(float time = 0.1f)
	{
		this.timerAlterIndestructible = time;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x000778BF File Offset: 0x00075ABF
	public void OverrideDeactivate(float time = 0.1f)
	{
		this.timerAlterDeactivate = time;
		this.rb.isKinematic = true;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x000778D4 File Offset: 0x00075AD4
	public void OverrideDeactivateReset()
	{
		this.isActive = true;
		this.rb.detectCollisions = true;
		if (this.spawned)
		{
			this.rb.isKinematic = false;
		}
		if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel())
		{
			this.photonTransformView.enabled = true;
		}
		this.timerAlterDeactivate = -123f;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0007792D File Offset: 0x00075B2D
	public void OverrideBreakEffects(float _time)
	{
		this.overrideDisableBreakEffectsTimer = _time;
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00077938 File Offset: 0x00075B38
	public void OverrideMaterial(PhysicMaterial material, float time = 0.1f)
	{
		if (this.alterMaterialCurrent != this.alterMaterialPrevious || this.alterMaterialCurrent == null)
		{
			this.alterMaterialPrevious = this.alterMaterialCurrent;
			foreach (Transform transform in this.colliders)
			{
				if (transform)
				{
					Collider component = transform.GetComponent<Collider>();
					if (component)
					{
						component.material = material;
					}
				}
				else
				{
					this.colliders.Remove(transform);
				}
			}
		}
		this.alterMaterialCurrent = material;
		this.timerAlterMaterial = time;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x000779EC File Offset: 0x00075BEC
	public void ResetIndestructible()
	{
		if (this.impactDetector)
		{
			this.impactDetector.isIndestructible = false;
		}
		this.timerAlterIndestructible = -123f;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00077A12 File Offset: 0x00075C12
	public void SetPositionLogic(Vector3 _position, Quaternion _rotation)
	{
		this.photonTransformView.Teleport(_position, _rotation);
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00077A24 File Offset: 0x00075C24
	[PunRPC]
	private void SetPositionRPC(Vector3 position, Quaternion rotation)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.SetPositionLogic(position, rotation);
			return;
		}
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.rb.position = position;
		this.rb.rotation = rotation;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00077A74 File Offset: 0x00075C74
	public void Teleport(Vector3 position, Quaternion rotation)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.rb.position = position;
			this.rb.rotation = rotation;
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonTransformView.Teleport(position, rotation);
			return;
		}
		this.photonView.RPC("SetPositionRPC", RpcTarget.MasterClient, new object[]
		{
			position,
			rotation
		});
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00077AF7 File Offset: 0x00075CF7
	public void OverrideMass(float value, float time = 0.1f)
	{
		this.timerAlterMass = time;
		if (this.alterMassValue <= value)
		{
			this.alterMassValue = value;
			this.overrideMassGoDownTimer = 0.1f;
			return;
		}
		if (this.overrideMassGoDownTimer <= 0f)
		{
			this.alterMassValue = value;
		}
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00077B30 File Offset: 0x00075D30
	public void ResetMass()
	{
		this.rb.mass = this.massOriginal;
		this.timerAlterMass = -123f;
		this.alterMassValue = 0f;
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00077B5C File Offset: 0x00075D5C
	private void Update()
	{
		if (this.grabbed)
		{
			for (int i = 0; i < this.playerGrabbing.Count; i++)
			{
				if (!this.playerGrabbing[i])
				{
					this.playerGrabbing.RemoveAt(i);
				}
			}
		}
		this.midPoint = base.transform.TransformPoint(this.midPointOffset);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.playerGrabbing.Count > 0)
			{
				this.lastPlayerGrabbing = this.playerGrabbing[this.playerGrabbing.Count - 1].playerAvatar;
				this.grabbedTimer = 0.5f;
			}
			else if (this.lastPlayerGrabbing)
			{
				this.grabbedTimer -= Time.deltaTime;
				if (this.grabbedTimer <= 0f)
				{
					this.lastPlayerGrabbing = null;
				}
			}
			if (this.enemyInteractTimer > 0f)
			{
				this.enemyInteractTimer -= Time.deltaTime;
				if (this.playerGrabbing.Count > 0)
				{
					this.enemyInteractTimer = 0f;
				}
			}
			if (this.hasImpactDetector && !this.impactDetector.isIndestructible)
			{
				if (this.heavyImpactImpulse)
				{
					this.impactDetector.ImpactHeavy(150f, this.centerPoint);
					this.heavyImpactImpulse = false;
				}
				if (this.mediumImpactImpulse)
				{
					this.impactDetector.ImpactMedium(80f, this.centerPoint);
					this.mediumImpactImpulse = false;
				}
				if (this.lightImpactImpulse)
				{
					this.impactDetector.ImpactLight(20f, this.centerPoint);
					this.lightImpactImpulse = false;
				}
				if (this.heavyBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakHeavy(this.centerPoint);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelHeavy);
					}
					this.heavyBreakImpulse = false;
				}
				if (this.mediumBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakMedium(this.centerPoint);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelMedium);
					}
					this.mediumBreakImpulse = false;
				}
				if (this.lightBreakImpulse)
				{
					if (this.isValuable)
					{
						this.impactDetector.BreakLight(this.centerPoint);
					}
					else
					{
						this.impactDetector.Break(0f, this.centerPoint, this.impactDetector.breakLevelLight);
					}
					this.lightBreakImpulse = false;
				}
			}
			if (this.overrideDisableBreakEffectsTimer > 0f)
			{
				this.overrideDisableBreakEffectsTimer -= Time.deltaTime;
			}
			if (this.dead && this.playerGrabbing.Count == 0)
			{
				this.DestroyPhysGrabObject();
			}
		}
		if (this.grabDisableTimer > 0f)
		{
			this.grabDisableTimer -= Time.deltaTime;
		}
		this.centerPoint = this.midPoint;
		if (SemiFunc.IsMasterClientOrSingleplayer() && base.transform.position.y < -50f)
		{
			if (this.impactDetector.destroyDisable)
			{
				if (this.impactDetector.destroyDisableTeleport)
				{
					this.Teleport(TruckSafetySpawnPoint.instance.transform.position, TruckSafetySpawnPoint.instance.transform.rotation);
					return;
				}
			}
			else
			{
				this.impactDetector.DestroyObject(true);
			}
		}
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00077EA9 File Offset: 0x000760A9
	public void EnemyInteractTimeSet()
	{
		this.enemyInteractTimer = 10f;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00077EB8 File Offset: 0x000760B8
	public void FreezeForces(float _time, Vector3 _force, Vector3 _torque)
	{
		if (this.rb.isKinematic)
		{
			return;
		}
		this.frozenTimer = _time;
		if (!this.frozen)
		{
			this.frozenPosition = base.transform.position;
			this.frozenRotation = base.transform.rotation;
			this.frozenVelocity = this.rb.velocity;
			this.frozenAngularVelocity = this.rb.angularVelocity;
			this.frozenForce = Vector3.zero;
			this.frozenTorque = Vector3.zero;
			this.frozen = true;
		}
		this.frozenForce += _force;
		this.frozenTorque += _torque;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00077F87 File Offset: 0x00076187
	private void OnDestroy()
	{
		if (RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00077FB6 File Offset: 0x000761B6
	public void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyPhysGrabObjectRPC();
			return;
		}
		this.photonView.RPC("DestroyPhysGrabObjectRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00077FE1 File Offset: 0x000761E1
	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x00077FEE File Offset: 0x000761EE
	private void OnDisable()
	{
		RoundDirector.instance.PhysGrabObjectRemove(this);
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00077FFB File Offset: 0x000761FB
	private void OnEnable()
	{
		RoundDirector.instance.PhysGrabObjectAdd(this);
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00078008 File Offset: 0x00076208
	public void GrabStarted(PhysGrabber player)
	{
		if (!this.grabbedLocal)
		{
			this.grabbedLocal = true;
			if (GameManager.instance.gameMode == 0)
			{
				if (!this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Add(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabStartedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00078078 File Offset: 0x00076278
	public void GrabEnded(PhysGrabber player)
	{
		if (this.grabbedLocal)
		{
			this.grabbedLocal = false;
			if (GameManager.instance.gameMode == 0)
			{
				this.Throw(player);
				if (this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Remove(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabEndedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x000780F0 File Offset: 0x000762F0
	public void GrabLink(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		this.photonView.RPC("GrabLinkRPC", RpcTarget.All, new object[]
		{
			playerPhotonID,
			colliderID,
			point,
			cameraRelativeGrabbedForward,
			cameraRelativeGrabbedUp
		});
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00078144 File Offset: 0x00076344
	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = base.transform.InverseTransformPoint(point);
		component.grabbedObjectTransform = base.transform;
		component.grabbedPhysGrabObjectColliderID = colliderID;
		component.grabbedPhysGrabObjectCollider = this.FindColliderFromID(colliderID).GetComponent<Collider>();
		component.grabbed = true;
		Transform localCameraTransform = component.playerAvatar.localCameraTransform;
		if (this.playerGrabbing.Count != 0)
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(base.transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(base.transform.up);
		}
		else
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(base.transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(base.transform.up);
			this.camRelForward = base.transform.InverseTransformDirection(base.transform.forward);
			this.camRelUp = base.transform.InverseTransformDirection(base.transform.up);
		}
		component.cameraRelativeGrabbedForward = component.cameraRelativeGrabbedForward.normalized;
		component.cameraRelativeGrabbedUp = component.cameraRelativeGrabbedUp.normalized;
		if (component.photonView.IsMine)
		{
			Vector3 localGrabPosition = component.localGrabPosition;
			this.photonView.RPC("GrabPointSyncRPC", RpcTarget.All, new object[]
			{
				playerPhotonID,
				localGrabPosition
			});
		}
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000782B5 File Offset: 0x000764B5
	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>().localGrabPosition = localPointInBox;
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000782C8 File Offset: 0x000764C8
	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		if (!this.playerGrabbing.Contains(component))
		{
			this.photonView.RPC("GrabPlayerAddRPC", RpcTarget.All, new object[]
			{
				playerPhotonID
			});
		}
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00078310 File Offset: 0x00076510
	[PunRPC]
	private void GrabPlayerAddRPC(int photonViewID)
	{
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		this.playerGrabbing.Add(component);
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00078338 File Offset: 0x00076538
	[PunRPC]
	private void GrabPlayerRemoveRPC(int photonViewID)
	{
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		this.playerGrabbing.Remove(component);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00078360 File Offset: 0x00076560
	private void Throw(PhysGrabber player)
	{
		float d = Mathf.Max(this.rb.mass * 1.5f, 1f);
		Vector3 vector = Vector3.ClampMagnitude(player.physGrabPointPullerPosition - player.physGrabPoint.position, player.forceMax) * d;
		vector *= 0.5f + player.throwStrength;
		this.rb.AddForce(vector, ForceMode.Impulse);
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x000783D4 File Offset: 0x000765D4
	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		this.Throw(component);
		component.grabbed = false;
		if (this.playerGrabbing.Contains(component))
		{
			this.photonView.RPC("GrabPlayerRemoveRPC", RpcTarget.All, new object[]
			{
				playerPhotonID
			});
		}
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x0007842C File Offset: 0x0007662C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (!this.impactDetector)
			{
				this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
			}
			if (!this.rb)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			stream.SendNext(this.rbVelocity);
			stream.SendNext(this.rbAngularVelocity);
			stream.SendNext(this.impactDetector.isSliding);
			stream.SendNext(this.isKinematic);
			return;
		}
		if (!this.impactDetector)
		{
			this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		}
		this.rbVelocity = (Vector3)stream.ReceiveNext();
		this.rbAngularVelocity = (Vector3)stream.ReceiveNext();
		this.impactDetector.isSliding = (bool)stream.ReceiveNext();
		this.isKinematic = (bool)stream.ReceiveNext();
		this.lastUpdateTime = Time.time;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0007852C File Offset: 0x0007672C
	public Transform FindColliderFromID(int colliderID)
	{
		foreach (Transform transform in this.colliders)
		{
			if (transform.GetComponent<PhysGrabObjectCollider>().colliderID == colliderID)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x0400158E RID: 5518
	public bool clientNonKinematic;

	// Token: 0x0400158F RID: 5519
	public bool overrideTagsAndLayers = true;

	// Token: 0x04001590 RID: 5520
	internal PhotonView photonView;

	// Token: 0x04001591 RID: 5521
	internal PhotonTransformView photonTransformView;

	// Token: 0x04001592 RID: 5522
	[HideInInspector]
	public Rigidbody rb;

	// Token: 0x04001593 RID: 5523
	private bool isMaster;

	// Token: 0x04001594 RID: 5524
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x04001595 RID: 5525
	internal PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001596 RID: 5526
	private bool hasImpactDetector;

	// Token: 0x04001597 RID: 5527
	internal Vector3 targetPos;

	// Token: 0x04001598 RID: 5528
	private float distance;

	// Token: 0x04001599 RID: 5529
	internal Quaternion targetRot;

	// Token: 0x0400159A RID: 5530
	private float angle;

	// Token: 0x0400159B RID: 5531
	internal Vector3 grabDisplacementCurrent;

	// Token: 0x0400159C RID: 5532
	[HideInInspector]
	public bool dead;

	// Token: 0x0400159D RID: 5533
	[HideInInspector]
	public bool grabbed;

	// Token: 0x0400159E RID: 5534
	[HideInInspector]
	public bool grabbedLocal;

	// Token: 0x0400159F RID: 5535
	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	// Token: 0x040015A0 RID: 5536
	[HideInInspector]
	public bool spawned;

	// Token: 0x040015A1 RID: 5537
	internal PlayerAvatar lastPlayerGrabbing;

	// Token: 0x040015A2 RID: 5538
	internal float grabbedTimer;

	// Token: 0x040015A3 RID: 5539
	[HideInInspector]
	public bool lightBreakImpulse;

	// Token: 0x040015A4 RID: 5540
	[HideInInspector]
	public bool mediumBreakImpulse;

	// Token: 0x040015A5 RID: 5541
	[HideInInspector]
	public bool heavyBreakImpulse;

	// Token: 0x040015A6 RID: 5542
	[HideInInspector]
	public bool lightImpactImpulse;

	// Token: 0x040015A7 RID: 5543
	[HideInInspector]
	public bool mediumImpactImpulse;

	// Token: 0x040015A8 RID: 5544
	[HideInInspector]
	public bool heavyImpactImpulse;

	// Token: 0x040015A9 RID: 5545
	[HideInInspector]
	public float enemyInteractTimer;

	// Token: 0x040015AA RID: 5546
	internal float angularDragOriginal;

	// Token: 0x040015AB RID: 5547
	internal float dragOriginal;

	// Token: 0x040015AC RID: 5548
	internal bool isValuable;

	// Token: 0x040015AD RID: 5549
	internal bool isEnemy;

	// Token: 0x040015AE RID: 5550
	internal bool isPlayer;

	// Token: 0x040015AF RID: 5551
	internal bool isMelee;

	// Token: 0x040015B0 RID: 5552
	internal bool isNonValuable;

	// Token: 0x040015B1 RID: 5553
	internal bool isKinematic;

	// Token: 0x040015B2 RID: 5554
	[HideInInspector]
	public float massOriginal;

	// Token: 0x040015B3 RID: 5555
	private float lastUpdateTime;

	// Token: 0x040015B4 RID: 5556
	[TupleElementNames(new string[]
	{
		"position",
		"timestamp"
	})]
	private List<ValueTuple<Vector3, double>> positionBuffer = new List<ValueTuple<Vector3, double>>();

	// Token: 0x040015B5 RID: 5557
	[TupleElementNames(new string[]
	{
		"rotation",
		"timestamp"
	})]
	private List<ValueTuple<Quaternion, double>> rotationBuffer = new List<ValueTuple<Quaternion, double>>();

	// Token: 0x040015B6 RID: 5558
	private float gradualLerp;

	// Token: 0x040015B7 RID: 5559
	private Vector3 prevTargetPos;

	// Token: 0x040015B8 RID: 5560
	private Quaternion prevTargetRot;

	// Token: 0x040015B9 RID: 5561
	internal Vector3 rbVelocity = Vector3.zero;

	// Token: 0x040015BA RID: 5562
	internal Vector3 rbAngularVelocity = Vector3.zero;

	// Token: 0x040015BB RID: 5563
	internal Vector3 currentPosition;

	// Token: 0x040015BC RID: 5564
	internal Quaternion currentRotation;

	// Token: 0x040015BD RID: 5565
	private bool hasHinge;

	// Token: 0x040015BE RID: 5566
	private PhysGrabHinge hinge;

	// Token: 0x040015BF RID: 5567
	private float timerZeroGravity;

	// Token: 0x040015C0 RID: 5568
	private float timerAlterDrag;

	// Token: 0x040015C1 RID: 5569
	private float alterDragValue;

	// Token: 0x040015C2 RID: 5570
	private float timerAlterAngularDrag;

	// Token: 0x040015C3 RID: 5571
	private float alterAngularDragValue;

	// Token: 0x040015C4 RID: 5572
	private float timerAlterMass;

	// Token: 0x040015C5 RID: 5573
	private float alterMassValue;

	// Token: 0x040015C6 RID: 5574
	private float timerAlterMaterial;

	// Token: 0x040015C7 RID: 5575
	private float timerAlterDeactivate = -123f;

	// Token: 0x040015C8 RID: 5576
	private float overrideFragilityTimer;

	// Token: 0x040015C9 RID: 5577
	internal float overrideDisableBreakEffectsTimer;

	// Token: 0x040015CA RID: 5578
	private bool isActive = true;

	// Token: 0x040015CB RID: 5579
	private PhysicMaterial alterMaterialPrevious;

	// Token: 0x040015CC RID: 5580
	private PhysicMaterial alterMaterialCurrent;

	// Token: 0x040015CD RID: 5581
	[HideInInspector]
	public Vector3 midPoint;

	// Token: 0x040015CE RID: 5582
	[HideInInspector]
	public Vector3 midPointOffset;

	// Token: 0x040015CF RID: 5583
	private Vector3 grabRotation;

	// Token: 0x040015D0 RID: 5584
	private bool isHidden;

	// Token: 0x040015D1 RID: 5585
	internal float grabDisableTimer;

	// Token: 0x040015D2 RID: 5586
	internal bool heldByLocalPlayer;

	// Token: 0x040015D3 RID: 5587
	private CollisionDetectionMode previousCollisionDetectionMode;

	// Token: 0x040015D4 RID: 5588
	private Camera mainCamera;

	// Token: 0x040015D5 RID: 5589
	private float timerAlterIndestructible;

	// Token: 0x040015D6 RID: 5590
	internal Transform forceGrabPoint;

	// Token: 0x040015D7 RID: 5591
	private MapCustom mapCustom;

	// Token: 0x040015D8 RID: 5592
	private bool hasMapCustom;

	// Token: 0x040015D9 RID: 5593
	private bool isCart;

	// Token: 0x040015DA RID: 5594
	private PhysGrabCart physGrabCart;

	// Token: 0x040015DB RID: 5595
	[HideInInspector]
	public List<Transform> colliders = new List<Transform>();

	// Token: 0x040015DC RID: 5596
	[HideInInspector]
	public Vector3 centerPoint;

	// Token: 0x040015DD RID: 5597
	public Vector3 camRelForward;

	// Token: 0x040015DE RID: 5598
	public Vector3 camRelUp;

	// Token: 0x040015DF RID: 5599
	internal bool frozen;

	// Token: 0x040015E0 RID: 5600
	private float frozenTimer;

	// Token: 0x040015E1 RID: 5601
	private Vector3 frozenPosition;

	// Token: 0x040015E2 RID: 5602
	private Quaternion frozenRotation;

	// Token: 0x040015E3 RID: 5603
	private Vector3 frozenVelocity;

	// Token: 0x040015E4 RID: 5604
	private Vector3 frozenAngularVelocity;

	// Token: 0x040015E5 RID: 5605
	private Vector3 frozenForce;

	// Token: 0x040015E6 RID: 5606
	private Vector3 frozenTorque;

	// Token: 0x040015E7 RID: 5607
	private float overrideDragGoDownTimer;

	// Token: 0x040015E8 RID: 5608
	private float overrideAngularDragGoDownTimer;

	// Token: 0x040015E9 RID: 5609
	private float overrideMassGoDownTimer;

	// Token: 0x040015EA RID: 5610
	internal float impactHappenedTimer;

	// Token: 0x040015EB RID: 5611
	internal float impactLightTimer;

	// Token: 0x040015EC RID: 5612
	internal float impactMediumTimer;

	// Token: 0x040015ED RID: 5613
	internal float impactHeavyTimer;

	// Token: 0x040015EE RID: 5614
	internal float breakLightTimer;

	// Token: 0x040015EF RID: 5615
	internal float breakMediumTimer;

	// Token: 0x040015F0 RID: 5616
	internal float breakHeavyTimer;

	// Token: 0x040015F1 RID: 5617
	internal bool hasNeverBeenGrabbed = true;

	// Token: 0x040015F2 RID: 5618
	[HideInInspector]
	public Vector3 boundingBox;

	// Token: 0x040015F3 RID: 5619
	internal Vector3 spawnTorque = Vector3.zero;

	// Token: 0x040015F4 RID: 5620
	private float smoothRotationDelta;

	// Token: 0x040015F5 RID: 5621
	private bool rbIsSleepingPrevious;

	// Token: 0x040015F6 RID: 5622
	private float overrideTorqueStrengthX = 1f;

	// Token: 0x040015F7 RID: 5623
	private float overrideTorqueStrengthXTimer;

	// Token: 0x040015F8 RID: 5624
	private float overrideTorqueStrengthY = 1f;

	// Token: 0x040015F9 RID: 5625
	private float overrideTorqueStrengthYTimer;

	// Token: 0x040015FA RID: 5626
	private float overrideTorqueStrengthZ = 1f;

	// Token: 0x040015FB RID: 5627
	private float overrideTorqueStrengthZTimer;

	// Token: 0x040015FC RID: 5628
	private float overrideTorqueStrength = 1f;

	// Token: 0x040015FD RID: 5629
	private float overrideTorqueStrengthTimer;

	// Token: 0x040015FE RID: 5630
	private float overrideGrabStrength = 1f;

	// Token: 0x040015FF RID: 5631
	private float overrideGrabStrengthTimer;

	// Token: 0x04001600 RID: 5632
	private float overrideGrabRelativeVerticalPosition;

	// Token: 0x04001601 RID: 5633
	private float overrideGrabRelativeVerticalPositionTimer;

	// Token: 0x04001602 RID: 5634
	private float overrideGrabRelativeHorizontalPosition;

	// Token: 0x04001603 RID: 5635
	private float overrideGrabRelativeHorizontalPositionTimer;
}
