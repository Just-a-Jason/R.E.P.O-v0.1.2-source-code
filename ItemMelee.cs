using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000156 RID: 342
public class ItemMelee : MonoBehaviour, IPunObservable
{
	// Token: 0x06000B6E RID: 2926 RVA: 0x00064EA4 File Offset: 0x000630A4
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>().transform;
		this.hurtColliderRotation = base.transform.Find("Hurt Collider Rotation");
		this.physGrabObjectImpactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.hurtCollider.gameObject.SetActive(false);
		this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		this.swingPoint = base.transform.Find("Swing Point");
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.particleSystem = base.transform.Find("Particles").GetComponent<ParticleSystem>();
		this.particleSystemGroundHit = base.transform.Find("Particles Ground Hit").GetComponent<ParticleSystem>();
		this.photonView = base.GetComponent<PhotonView>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.forceGrabPoint = base.transform.Find("Force Grab Point");
		this.meshHealthy = base.transform.Find("Mesh Healthy");
		this.meshBroken = base.transform.Find("Mesh Broken");
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		if (SemiFunc.RunIsArena())
		{
			HurtCollider component = this.hurtCollider.GetComponent<HurtCollider>();
			component.playerDamage = component.enemyDamage;
		}
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00064FE8 File Offset: 0x000631E8
	private void DisableHurtBoxWhenEquipping()
	{
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			this.hurtCollider.gameObject.SetActive(false);
			this.swingTimer = 0f;
			this.trailRenderer.emitting = false;
		}
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00065044 File Offset: 0x00063244
	private void FixedUpdate()
	{
		this.DisableHurtBoxWhenEquipping();
		bool flag = false;
		using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isRotating)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Quaternion turnY = this.currentYRotation;
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			this.physGrabObject.TurnXYZ(turnX, turnY, Quaternion.identity);
		}
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			return;
		}
		if (this.isBroken)
		{
			return;
		}
		if (this.prevPosUpdateTimer > 0.1f)
		{
			this.prevPosition = this.swingPoint.position;
			this.prevPosUpdateTimer = 0f;
		}
		else
		{
			this.prevPosUpdateTimer += Time.fixedDeltaTime;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!flag)
		{
			if (this.torqueStrength != 1f)
			{
				this.physGrabObject.OverrideTorqueStrength(this.torqueStrength, 0.1f);
			}
			if (this.physGrabObject.grabbed)
			{
				if (this.itemBattery.batteryLife <= 0f)
				{
					this.physGrabObject.OverrideTorqueStrength(0.1f, 0.1f);
				}
				this.physGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSlippery(), 0.1f);
			}
		}
		if (flag)
		{
			this.physGrabObject.OverrideTorqueStrength(4f, 0.1f);
		}
		if (this.distanceCheckTimer > 0.1f)
		{
			this.prevPosDistance = Vector3.Distance(this.prevPosition, this.swingPoint.position) * 10f * this.rb.mass;
			this.distanceCheckTimer = 0f;
		}
		this.distanceCheckTimer += Time.fixedDeltaTime;
		this.TurnWeapon();
		Vector3 vector = this.prevPosition - this.swingPoint.position;
		float num = 1f;
		if (!this.physGrabObject.grabbed)
		{
			num = 0.5f;
		}
		if (vector.magnitude > num * this.swingDetectSpeedMultiplier && this.swingPoint.position - this.prevPosition != Vector3.zero)
		{
			this.swingTimer = 0.2f;
			if (!this.isSwinging)
			{
				this.newSwing = true;
			}
			this.swingDirection = Quaternion.LookRotation(this.swingPoint.position - this.prevPosition);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x000652D0 File Offset: 0x000634D0
	private void TurnWeapon()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.customRotation != Quaternion.identity && !this.turnWeapon)
		{
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			this.physGrabObject.TurnXYZ(turnX, this.customRotation, Quaternion.identity);
		}
		if (this.turnWeaponStrength != 1f)
		{
			this.physGrabObject.OverrideTorqueStrengthY(this.turnWeaponStrength, 0.1f);
		}
		if (!this.turnWeapon)
		{
			return;
		}
		this.physGrabObject.OverrideAngularDrag(0f, 0.1f);
		this.physGrabObject.OverrideDrag(0f, 0.1f);
		if (this.physGrabObject.grabbed && !this.playerAvatar)
		{
			this.playerAvatar = this.physGrabObject.playerGrabbing[0].GetComponent<PlayerAvatar>();
		}
		if (!this.physGrabObject.grabbed && this.playerAvatar)
		{
			this.playerAvatar = null;
		}
		if (!this.physGrabObject.grabbed)
		{
			return;
		}
		Vector3 forward = Vector3.forward;
		Vector3 up = Vector3.up;
		Transform transform = this.playerAvatar.transform;
		Vector3 direction = this.rb.velocity / Time.fixedDeltaTime;
		if (direction.magnitude > 200f)
		{
			Vector3 vector = this.playerAvatar.transform.InverseTransformDirection(direction);
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			Quaternion quaternion = Quaternion.identity;
			if (vector2 != Vector3.zero)
			{
				quaternion = Quaternion.LookRotation(vector2);
			}
			Quaternion quaternion2 = Quaternion.Euler(0f, Mathf.Round(Quaternion.Euler(0f, quaternion.eulerAngles.y + 90f, 0f).eulerAngles.y / 90f) * 90f, 0f);
			if (quaternion2.eulerAngles.y == 270f)
			{
				quaternion2 = Quaternion.Euler(0f, 90f, 0f);
			}
			if (quaternion2.eulerAngles.y == 180f)
			{
				quaternion2 = Quaternion.Euler(0f, 0f, 0f);
			}
			this.targetYRotation = quaternion2;
		}
		this.currentYRotation = Quaternion.Slerp(this.currentYRotation, this.targetYRotation, Time.deltaTime * 5f);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00065538 File Offset: 0x00063738
	private void Update()
	{
		if (this.grabbedTimer > 0f)
		{
			this.grabbedTimer -= Time.deltaTime;
		}
		if (this.physGrabObject.grabbed)
		{
			this.grabbedTimer = 1f;
		}
		if (this.hitFreezeDelay > 0f)
		{
			this.hitFreezeDelay -= Time.deltaTime;
			if (this.hitFreezeDelay <= 0f)
			{
				this.physGrabObject.FreezeForces(this.hitFreeze, Vector3.zero, Vector3.zero);
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.spawnTimer > 0f)
		{
			this.prevPosition = this.swingPoint.position;
			this.swingTimer = 0f;
			this.spawnTimer -= Time.deltaTime;
			return;
		}
		if (this.hitCooldown > 0f)
		{
			this.hitCooldown -= Time.deltaTime;
		}
		if (this.enemyOrPVPDurabilityLossCooldown > 0f)
		{
			this.enemyOrPVPDurabilityLossCooldown -= Time.deltaTime;
		}
		if (this.groundHitCooldown > 0f)
		{
			this.groundHitCooldown -= Time.deltaTime;
		}
		if (this.groundHitSoundTimer > 0f)
		{
			this.groundHitSoundTimer -= Time.deltaTime;
		}
		this.DisableHurtBoxWhenEquipping();
		if (this.itemEquippable.equipTimer > 0f || this.itemEquippable.unequipTimer > 0f)
		{
			return;
		}
		this.soundSwingLoop.PlayLoop(this.hurtCollider.gameObject.activeSelf, 10f, 10f, 3f);
		if (SemiFunc.IsMultiplayer() && !SemiFunc.IsMasterClient() && this.isSwinging)
		{
			this.swingTimer = 0.5f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.itemBattery.batteryLife <= 0f)
			{
				this.MeleeBreak();
			}
			else
			{
				this.MeleeFix();
			}
			if (this.durabilityLossCooldown > 0f)
			{
				this.durabilityLossCooldown -= Time.deltaTime;
			}
			if (!this.isBroken)
			{
				if (this.physGrabObject.grabbedLocal)
				{
					if (!this.itemBattery.batteryActive)
					{
					}
				}
				else
				{
					bool batteryActive = this.itemBattery.batteryActive;
				}
			}
		}
		if (this.isBroken)
		{
			return;
		}
		if (this.hitSoundDelayTimer > 0f)
		{
			this.hitSoundDelayTimer -= Time.deltaTime;
		}
		if (this.swingPitch != this.swingPitchTarget && this.swingPitchTargetProgress >= 1f)
		{
			this.swingPitch = this.swingPitchTarget;
		}
		Vector3 b = this.prevPosition - this.swingPoint.position;
		if (b.magnitude > 0.1f)
		{
			this.hurtColliderRotation.LookAt(this.hurtColliderRotation.position - b, Vector3.up);
			this.hurtColliderRotation.localEulerAngles = new Vector3(0f, this.hurtColliderRotation.localEulerAngles.y, 0f);
			this.hurtColliderRotation.localEulerAngles = new Vector3(0f, Mathf.Round(this.hurtColliderRotation.localEulerAngles.y / 90f) * 90f, 0f);
		}
		Vector3 vector = this.prevPosition - this.swingPoint.position;
		Vector3 normalized = this.swingStartDirection.normalized;
		Vector3 normalized2 = vector.normalized;
		double num = (double)Vector3.Dot(normalized, normalized2);
		double num2 = 0.85;
		if (!this.physGrabObject.grabbed)
		{
			num2 = 0.1;
		}
		if (num > num2)
		{
			this.swingTimer = 0f;
		}
		if (this.isSwinging)
		{
			this.ActivateHitbox();
		}
		if (this.hitTimer > 0f)
		{
			this.hitTimer -= Time.deltaTime;
		}
		if (this.swingTimer <= 0f)
		{
			if (this.hitBoxTimer <= 0f)
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
			else
			{
				this.hitBoxTimer -= Time.deltaTime;
			}
			this.trailRenderer.emitting = false;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.isSwinging = false;
				return;
			}
		}
		else
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.isSwinging = true;
			}
			if (this.hitTimer <= 0f)
			{
				this.hitBoxTimer = 0.2f;
			}
			this.swingTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00065990 File Offset: 0x00063B90
	public void SwingHit()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SwingHitRPC(true);
			return;
		}
		this.photonView.RPC("SwingHitRPC", RpcTarget.All, new object[]
		{
			true
		});
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000659C1 File Offset: 0x00063BC1
	public void EnemyOrPVPSwingHit()
	{
		if (this.enemyOrPVPDurabilityLossCooldown <= 0f)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				this.EnemyOrPVPSwingHitRPC();
				return;
			}
			this.photonView.RPC("EnemyOrPVPSwingHitRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x000659F4 File Offset: 0x00063BF4
	[PunRPC]
	public void EnemyOrPVPSwingHitRPC()
	{
		this.itemBattery.batteryLife -= this.durabilityDrainOnEnemiesAndPVP;
		this.enemyOrPVPDurabilityLossCooldown = 0.1f;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00065A1C File Offset: 0x00063C1C
	[PunRPC]
	public void SwingHitRPC(bool durabilityLoss)
	{
		bool flag = false;
		if (durabilityLoss)
		{
			if (this.hitCooldown > 0f)
			{
				return;
			}
			if (this.hitSoundDelayTimer > 0f)
			{
				return;
			}
			this.hitSoundDelayTimer = 0.1f;
			this.hitCooldown = 0.3f;
		}
		else
		{
			if (this.groundHitCooldown > 0f)
			{
				return;
			}
			if (this.groundHitSoundTimer > 0f)
			{
				return;
			}
			this.groundHitCooldown = 0.3f;
			this.groundHitSoundTimer = 0.1f;
			flag = true;
		}
		if (!flag)
		{
			this.soundHit.Pitch = 1f;
			this.soundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.particleSystem.Play();
		}
		else
		{
			this.soundHit.Pitch = 2f;
			this.soundHit.Play(base.transform.position, 0.5f, 1f, 1f, 1f);
			this.particleSystemGroundHit.Play();
		}
		if (this.physGrabObject.grabbed && !this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		if (this.hitBoxTimer > 0.05f)
		{
			this.hitBoxTimer = 0.05f;
		}
		this.hitTimer = 0.5f;
		if (SemiFunc.IsMasterClientOrSingleplayer() && durabilityLoss && this.durabilityLossCooldown <= 0f && SemiFunc.RunIsLevel())
		{
			this.itemBattery.batteryLife -= this.durabilityDrain;
			this.durabilityLossCooldown = 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.hitFreeze > 0f && !flag)
		{
			this.hitFreezeDelay = 0.06f;
		}
		if (this.onHit != null)
		{
			this.onHit.Invoke();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00065C28 File Offset: 0x00063E28
	public void GroundHit()
	{
		if (this.hitTimer > 0f)
		{
			return;
		}
		if (this.hitBoxTimer <= 0f)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SwingHitRPC(false);
			return;
		}
		this.photonView.RPC("SwingHitRPC", RpcTarget.All, new object[]
		{
			false
		});
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00065C80 File Offset: 0x00063E80
	private void MeleeBreak()
	{
		if (this.isBroken)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.MeleeBreakRPC();
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("MeleeBreakRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x00065CB8 File Offset: 0x00063EB8
	[PunRPC]
	public void MeleeBreakRPC()
	{
		if (this.physGrabObject.isMelee)
		{
			this.particleSystem.Play();
			this.physGrabObject.isMelee = false;
			this.physGrabObject.forceGrabPoint = null;
			this.itemBattery.BatteryToggle(false);
			this.isBroken = true;
			this.hurtCollider.gameObject.SetActive(false);
			this.trailRenderer.emitting = false;
			this.meshHealthy.gameObject.SetActive(false);
			this.meshBroken.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00065D47 File Offset: 0x00063F47
	private void MeleeFix()
	{
		if (!this.isBroken)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.MeleeFixRPC();
			return;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("MeleeFixRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00065D80 File Offset: 0x00063F80
	[PunRPC]
	public void MeleeFixRPC()
	{
		if (!this.physGrabObject.isMelee)
		{
			this.particleSystem.Play();
			this.physGrabObject.isMelee = true;
			this.physGrabObject.forceGrabPoint = this.forceGrabPoint;
			this.itemBattery.BatteryToggle(true);
			this.isBroken = false;
			this.meshHealthy.gameObject.SetActive(true);
			this.meshBroken.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00065DF8 File Offset: 0x00063FF8
	public void ActivateHitbox()
	{
		if (this.hitTimer > 0f)
		{
			return;
		}
		if (this.newSwing)
		{
			this.soundSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.swingPitchTarget = this.prevPosDistance;
			this.swingPitchTargetProgress = 0f;
			if (this.swingPoint)
			{
				this.swingStartDirection = this.swingPoint.position - this.prevPosition;
			}
			this.swingTimer = 0.4f;
			this.hitBoxTimer = 0.4f;
			if (this.grabbedTimer > 0f)
			{
				float num = 150f;
				if (!this.physGrabObject.grabbed)
				{
					num *= 0.5f;
				}
				this.rb.AddForceAtPosition(this.swingDirection * Vector3.forward * num * this.rb.mass, this.swingPoint.position);
			}
			this.newSwing = false;
		}
		if (this.hurtCollider)
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
		if (this.trailRenderer)
		{
			this.trailRenderer.emitting = true;
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00065F44 File Offset: 0x00064144
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext(this.isSwinging);
			return;
		}
		bool flag = this.isSwinging;
		this.isSwinging = (bool)stream.ReceiveNext();
		if (!flag && this.isSwinging)
		{
			this.newSwing = true;
			this.ActivateHitbox();
		}
	}

	// Token: 0x04001274 RID: 4724
	private float durabilityDrain = 2.5f;

	// Token: 0x04001275 RID: 4725
	public float durabilityDrainOnEnemiesAndPVP = 5f;

	// Token: 0x04001276 RID: 4726
	public float hitFreeze = 0.2f;

	// Token: 0x04001277 RID: 4727
	public float hitFreezeDelay;

	// Token: 0x04001278 RID: 4728
	public float swingDetectSpeedMultiplier = 1f;

	// Token: 0x04001279 RID: 4729
	public bool turnWeapon = true;

	// Token: 0x0400127A RID: 4730
	public float torqueStrength = 1f;

	// Token: 0x0400127B RID: 4731
	public float turnWeaponStrength = 40f;

	// Token: 0x0400127C RID: 4732
	public Quaternion customRotation = Quaternion.identity;

	// Token: 0x0400127D RID: 4733
	public UnityEvent onHit;

	// Token: 0x0400127E RID: 4734
	private Transform hurtCollider;

	// Token: 0x0400127F RID: 4735
	private Transform hurtColliderRotation;

	// Token: 0x04001280 RID: 4736
	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	// Token: 0x04001281 RID: 4737
	private PhysGrabObject physGrabObject;

	// Token: 0x04001282 RID: 4738
	private Rigidbody rb;

	// Token: 0x04001283 RID: 4739
	private float swingTimer = 0.1f;

	// Token: 0x04001284 RID: 4740
	private float hitBoxTimer = 0.1f;

	// Token: 0x04001285 RID: 4741
	private TrailRenderer trailRenderer;

	// Token: 0x04001286 RID: 4742
	public Sound soundSwingLoop;

	// Token: 0x04001287 RID: 4743
	public Sound soundSwing;

	// Token: 0x04001288 RID: 4744
	public Sound soundHit;

	// Token: 0x04001289 RID: 4745
	private Vector3 prevPosition;

	// Token: 0x0400128A RID: 4746
	private float prevPosDistance;

	// Token: 0x0400128B RID: 4747
	private float prevPosUpdateTimer;

	// Token: 0x0400128C RID: 4748
	private Transform swingPoint;

	// Token: 0x0400128D RID: 4749
	private Quaternion swingDirection;

	// Token: 0x0400128E RID: 4750
	private PlayerAvatar playerAvatar;

	// Token: 0x0400128F RID: 4751
	private float hitSoundDelayTimer;

	// Token: 0x04001290 RID: 4752
	private ParticleSystem particleSystem;

	// Token: 0x04001291 RID: 4753
	private ParticleSystem particleSystemGroundHit;

	// Token: 0x04001292 RID: 4754
	private PhotonView photonView;

	// Token: 0x04001293 RID: 4755
	private float swingPitch = 1f;

	// Token: 0x04001294 RID: 4756
	private float swingPitchTarget;

	// Token: 0x04001295 RID: 4757
	private float swingPitchTargetProgress;

	// Token: 0x04001296 RID: 4758
	private float distanceCheckTimer;

	// Token: 0x04001297 RID: 4759
	private ItemBattery itemBattery;

	// Token: 0x04001298 RID: 4760
	private Vector3 swingStartDirection = Vector3.zero;

	// Token: 0x04001299 RID: 4761
	private Transform forceGrabPoint;

	// Token: 0x0400129A RID: 4762
	private bool isBroken;

	// Token: 0x0400129B RID: 4763
	private Quaternion targetYRotation;

	// Token: 0x0400129C RID: 4764
	private Quaternion currentYRotation;

	// Token: 0x0400129D RID: 4765
	private float durabilityLossCooldown;

	// Token: 0x0400129E RID: 4766
	private Transform meshHealthy;

	// Token: 0x0400129F RID: 4767
	private Transform meshBroken;

	// Token: 0x040012A0 RID: 4768
	private bool isSwinging;

	// Token: 0x040012A1 RID: 4769
	private bool newSwing;

	// Token: 0x040012A2 RID: 4770
	private float hitTimer;

	// Token: 0x040012A3 RID: 4771
	private ItemEquippable itemEquippable;

	// Token: 0x040012A4 RID: 4772
	private float hitCooldown;

	// Token: 0x040012A5 RID: 4773
	private float groundHitCooldown;

	// Token: 0x040012A6 RID: 4774
	private float groundHitSoundTimer;

	// Token: 0x040012A7 RID: 4775
	private float spawnTimer = 3f;

	// Token: 0x040012A8 RID: 4776
	private float grabbedTimer;

	// Token: 0x040012A9 RID: 4777
	private float enemyOrPVPDurabilityLossCooldown;
}
