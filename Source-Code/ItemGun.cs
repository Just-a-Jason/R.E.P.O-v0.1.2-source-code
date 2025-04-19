using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class ItemGun : MonoBehaviour
{
	// Token: 0x06000C0C RID: 3084 RVA: 0x0006AD78 File Offset: 0x00068F78
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.photonView = base.GetComponent<PhotonView>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.triggerAnimationCurve = AssetManager.instance.animationCurveClickInOut;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0006ADD4 File Offset: 0x00068FD4
	private void Update()
	{
		if (this.physGrabObject.grabbed && this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(this.distanceKeep);
		}
		if (this.triggerAnimationActive)
		{
			float num = 45f;
			this.triggerAnimationEval += Time.deltaTime * 4f;
			this.gunTrigger.localRotation = Quaternion.Euler(num * this.triggerAnimationCurve.Evaluate(this.triggerAnimationEval), 0f, 0f);
			if (this.triggerAnimationEval >= 1f)
			{
				this.gunTrigger.localRotation = Quaternion.Euler(0f, 0f, 0f);
				this.triggerAnimationActive = false;
				this.triggerAnimationEval = 1f;
			}
		}
		this.UpdateMaster();
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0006AEA8 File Offset: 0x000690A8
	private void UpdateMaster()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			Quaternion turnX = Quaternion.Euler(this.aimVerticalOffset, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
			{
				if (flag3)
				{
					if (physGrabber.playerAvatar.isCrouching || physGrabber.playerAvatar.isCrawling)
					{
						flag2 = true;
					}
					flag3 = false;
				}
				if (physGrabber.isRotating)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
			float num = this.grabVerticalOffset;
			if (flag2)
			{
				num += 0.5f;
			}
			this.physGrabObject.OverrideGrabVerticalPosition(num);
			if (!flag)
			{
				if (this.grabStrengthMultiplier != 1f)
				{
					this.physGrabObject.OverrideGrabStrength(this.grabStrengthMultiplier, 0.1f);
				}
				if (this.torqueMultiplier != 1f)
				{
					this.physGrabObject.OverrideTorqueStrength(this.torqueMultiplier, 0.1f);
				}
				if (this.itemBattery.batteryLife <= 0f)
				{
					this.physGrabObject.OverrideTorqueStrength(0.1f, 0.1f);
				}
			}
			if (flag)
			{
				this.physGrabObject.OverrideAngularDrag(40f, 0.1f);
				this.physGrabObject.OverrideTorqueStrength(6f, 0.1f);
			}
		}
		if (this.itemToggle.toggleState != this.prevToggleState)
		{
			if (this.shootCooldownTimer <= 0f)
			{
				this.Shoot();
				this.shootCooldownTimer = this.shootCooldown;
			}
			if (this.itemBattery.batteryLife <= 0f)
			{
				this.soundNoAmmoClick.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.StartTriggerAnimation();
				SemiFunc.CameraShakeImpact(1f, 0.1f);
				this.physGrabObject.rb.AddForceAtPosition(-this.gunMuzzle.forward * 1f, this.gunMuzzle.position, ForceMode.Impulse);
			}
			this.prevToggleState = this.itemToggle.toggleState;
		}
		if (this.shootCooldownTimer > 0f)
		{
			this.shootCooldownTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0006B13C File Offset: 0x0006933C
	public void Misfire()
	{
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		if (this.physGrabObject.hasNeverBeenGrabbed)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if ((float)Random.Range(0, 100) < this.misfirePercentageChange)
		{
			this.Shoot();
		}
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0006B17C File Offset: 0x0006937C
	public void Shoot()
	{
		bool flag = false;
		if (this.itemBattery.batteryLife <= 0f)
		{
			flag = true;
		}
		if (Random.Range(0, 10000) == 0)
		{
			flag = false;
		}
		if (flag)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ShootRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ShootRPC();
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0006B1D6 File Offset: 0x000693D6
	private void MuzzleFlash()
	{
		Object.Instantiate<GameObject>(this.muzzleFlashPrefab, this.gunMuzzle.position, this.gunMuzzle.rotation, this.gunMuzzle).GetComponent<ItemGunMuzzleFlash>().ActivateAllEffects();
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x0006B209 File Offset: 0x00069409
	private void StartTriggerAnimation()
	{
		this.triggerAnimationActive = true;
		this.triggerAnimationEval = 0f;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0006B220 File Offset: 0x00069420
	[PunRPC]
	public void ShootRPC()
	{
		float distanceMin = 3f * this.cameraShakeMultiplier;
		float distanceMax = 16f * this.cameraShakeMultiplier;
		SemiFunc.CameraShakeImpactDistance(this.gunMuzzle.position, 5f * this.cameraShakeMultiplier, 0.1f, distanceMin, distanceMax);
		SemiFunc.CameraShakeDistance(this.gunMuzzle.position, 0.1f * this.cameraShakeMultiplier, 0.1f * this.cameraShakeMultiplier, distanceMin, distanceMax);
		this.soundShoot.Play(this.gunMuzzle.position, 1f, 1f, 1f, 1f);
		this.soundShootGlobal.Play(this.gunMuzzle.position, 1f, 1f, 1f, 1f);
		this.MuzzleFlash();
		this.StartTriggerAnimation();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.investigateRadius > 0f)
			{
				EnemyDirector.instance.SetInvestigate(base.transform.position, this.investigateRadius);
			}
			this.physGrabObject.rb.AddForceAtPosition(-this.gunMuzzle.forward * this.gunRecoilForce, this.gunMuzzle.position, ForceMode.Impulse);
			if (!this.batteryDrainFullBar)
			{
				this.itemBattery.batteryLife -= this.batteryDrain;
			}
			else
			{
				this.itemBattery.RemoveFullBar(this.batteryDrainFullBars);
			}
			for (int i = 0; i < this.numberOfBullets; i++)
			{
				Vector3 endPosition = this.gunMuzzle.position;
				bool hit = false;
				bool flag = false;
				Vector3 vector = this.gunMuzzle.forward;
				if (this.gunRandomSpread > 0f)
				{
					float angle = Random.Range(0f, this.gunRandomSpread / 2f);
					float angle2 = Random.Range(0f, 360f);
					Vector3 normalized = Vector3.Cross(vector, Random.onUnitSphere).normalized;
					Quaternion rhs = Quaternion.AngleAxis(angle, normalized);
					vector = (Quaternion.AngleAxis(angle2, vector) * rhs * vector).normalized;
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(this.gunMuzzle.position, vector, out raycastHit, this.gunRange, SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
				{
					"Enemy"
				})))
				{
					endPosition = raycastHit.point;
					hit = true;
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					endPosition = this.gunMuzzle.position + this.gunMuzzle.forward * this.gunRange;
					hit = false;
				}
				this.ShootBullet(endPosition, hit);
			}
		}
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0006B4C0 File Offset: 0x000696C0
	private void ShootBullet(Vector3 _endPosition, bool _hit)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ShootBulletRPC", RpcTarget.All, new object[]
			{
				_endPosition,
				_hit
			});
			return;
		}
		this.ShootBulletRPC(_endPosition, _hit);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0006B510 File Offset: 0x00069710
	[PunRPC]
	public void ShootBulletRPC(Vector3 _endPosition, bool _hit)
	{
		if (this.physGrabObject.playerGrabbing.Count > 1)
		{
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
			{
				physGrabber.OverrideGrabRelease();
			}
		}
		ItemGunBullet component = Object.Instantiate<GameObject>(this.bulletPrefab, this.gunMuzzle.position, this.gunMuzzle.rotation).GetComponent<ItemGunBullet>();
		component.hitPosition = _endPosition;
		component.bulletHit = _hit;
		this.soundHit.Play(_endPosition, 1f, 1f, 1f, 1f);
		component.shootLineWidthCurve = this.shootLineWidthCurve;
		component.ActivateAll();
	}

	// Token: 0x04001356 RID: 4950
	private PhysGrabObject physGrabObject;

	// Token: 0x04001357 RID: 4951
	private ItemToggle itemToggle;

	// Token: 0x04001358 RID: 4952
	public int numberOfBullets = 1;

	// Token: 0x04001359 RID: 4953
	[Range(0f, 65f)]
	public float gunRandomSpread;

	// Token: 0x0400135A RID: 4954
	public float gunRange = 50f;

	// Token: 0x0400135B RID: 4955
	public float distanceKeep = 0.8f;

	// Token: 0x0400135C RID: 4956
	public float gunRecoilForce = 1f;

	// Token: 0x0400135D RID: 4957
	public float cameraShakeMultiplier = 1f;

	// Token: 0x0400135E RID: 4958
	public float torqueMultiplier = 1f;

	// Token: 0x0400135F RID: 4959
	public float grabStrengthMultiplier = 1f;

	// Token: 0x04001360 RID: 4960
	public float shootCooldown = 1f;

	// Token: 0x04001361 RID: 4961
	public float batteryDrain = 0.1f;

	// Token: 0x04001362 RID: 4962
	public bool batteryDrainFullBar;

	// Token: 0x04001363 RID: 4963
	public int batteryDrainFullBars = 1;

	// Token: 0x04001364 RID: 4964
	[Range(0f, 100f)]
	public float misfirePercentageChange = 50f;

	// Token: 0x04001365 RID: 4965
	public AnimationCurve shootLineWidthCurve;

	// Token: 0x04001366 RID: 4966
	public float grabVerticalOffset = -0.2f;

	// Token: 0x04001367 RID: 4967
	public float aimVerticalOffset = -10f;

	// Token: 0x04001368 RID: 4968
	public float investigateRadius = 20f;

	// Token: 0x04001369 RID: 4969
	public Transform gunMuzzle;

	// Token: 0x0400136A RID: 4970
	public GameObject bulletPrefab;

	// Token: 0x0400136B RID: 4971
	public GameObject muzzleFlashPrefab;

	// Token: 0x0400136C RID: 4972
	public Transform gunTrigger;

	// Token: 0x0400136D RID: 4973
	public Sound soundShoot;

	// Token: 0x0400136E RID: 4974
	public Sound soundShootGlobal;

	// Token: 0x0400136F RID: 4975
	public Sound soundNoAmmoClick;

	// Token: 0x04001370 RID: 4976
	public Sound soundHit;

	// Token: 0x04001371 RID: 4977
	private float shootCooldownTimer;

	// Token: 0x04001372 RID: 4978
	private ItemBattery itemBattery;

	// Token: 0x04001373 RID: 4979
	private PhotonView photonView;

	// Token: 0x04001374 RID: 4980
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001375 RID: 4981
	private bool prevToggleState;

	// Token: 0x04001376 RID: 4982
	private AnimationCurve triggerAnimationCurve;

	// Token: 0x04001377 RID: 4983
	private float triggerAnimationEval;

	// Token: 0x04001378 RID: 4984
	private bool triggerAnimationActive;
}
