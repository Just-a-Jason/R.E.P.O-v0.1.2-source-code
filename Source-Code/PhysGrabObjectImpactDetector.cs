using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000190 RID: 400
public class PhysGrabObjectImpactDetector : MonoBehaviour, IPunObservable
{
	// Token: 0x06000D1A RID: 3354 RVA: 0x0007317C File Offset: 0x0007137C
	private void Start()
	{
		this.inCartVolumeMultiplier = 0.6f;
		if (base.GetComponent<PhysGrabHinge>())
		{
			this.isHinge = true;
		}
		this.cart = base.GetComponent<PhysGrabCart>();
		if (this.cart)
		{
			this.isCart = true;
		}
		this.enemyRigidbody = base.GetComponent<EnemyRigidbody>();
		if (this.enemyRigidbody)
		{
			this.isEnemy = true;
		}
		this.previousSlidingPosition = base.transform.position;
		this.valuableObject = base.GetComponent<ValuableObject>();
		if (this.valuableObject)
		{
			this.isValuable = true;
			this.breakLogic = true;
			this.fragility = this.valuableObject.durabilityPreset.fragility;
			this.durability = this.valuableObject.durabilityPreset.durability;
			this.impactAudio = this.valuableObject.audioPreset;
			this.impactAudioPitch = this.valuableObject.audioPresetPitch;
		}
		else
		{
			this.notValuableObject = base.GetComponent<NotValuableObject>();
			this.isNotValuable = true;
			if (this.notValuableObject)
			{
				if (this.notValuableObject.durabilityPreset)
				{
					this.breakLogic = true;
					this.fragility = this.notValuableObject.durabilityPreset.fragility;
					this.durability = this.notValuableObject.durabilityPreset.durability;
				}
				this.impactAudio = this.notValuableObject.audioPreset;
				this.impactAudioPitch = this.notValuableObject.audioPresetPitch;
			}
		}
		if (this.impactAudio)
		{
			this.audioActive = true;
		}
		else
		{
			this.audioActive = false;
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.mainCamera = Camera.main;
		this.ColliderGet(base.transform);
		this.colliderVolume /= 200000f;
		GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Phys Object Particles"), new Vector3(0f, 0f, 0f), Quaternion.identity);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.particles = gameObject.GetComponent<PhysObjectParticles>();
		this.particles.multiplier = this.particleMultiplier;
		if (this.isValuable)
		{
			this.particles.gradient = this.valuableObject.particleColors;
		}
		if (this.notValuableObject)
		{
			this.particles.gradient = this.notValuableObject.particleColors;
		}
		this.particles.colliderTransforms = this.colliderTransforms;
		this.originalPosition = this.rb.position;
		this.originalRotation = this.rb.rotation;
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00073450 File Offset: 0x00071650
	private void ColliderGet(Transform transform)
	{
		if (transform.CompareTag("Phys Grab Object") && transform.GetComponent<Collider>())
		{
			this.colliderTransforms.Add(transform);
			Bounds bounds = transform.transform.GetComponent<Collider>().bounds;
			float num = bounds.size.x * 100f * (bounds.size.y * 100f) * (bounds.size.z * 100f);
			if (transform.GetComponent<SphereCollider>())
			{
				num *= 0.55f;
			}
			this.colliderVolume += num;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			this.ColliderGet(transform2);
		}
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00073540 File Offset: 0x00071740
	[PunRPC]
	private void InCartRPC(bool inCartState)
	{
		this.inCart = inCartState;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00073549 File Offset: 0x00071749
	private void IndestructibleSpawnTimer()
	{
		if (this.indestructibleSpawnTimer > 0f)
		{
			this.physGrabObject.OverrideIndestructible(0.1f);
			this.indestructibleSpawnTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x0007357C File Offset: 0x0007177C
	private void Update()
	{
		this.IndestructibleSpawnTimer();
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.inCartPrevious != this.inCart)
		{
			this.inCartPrevious = this.inCart;
			if (GameManager.instance.gameMode == 1)
			{
				this.photonView.RPC("InCartRPC", RpcTarget.Others, new object[]
				{
					this.inCart
				});
			}
		}
		if (this.timerInCart > 0f)
		{
			this.inCart = true;
			this.timerInCart -= Time.deltaTime;
		}
		else
		{
			this.inCart = false;
			this.currentCart = null;
		}
		if (this.isValuable && !this.valuableObject.dollarValueSet)
		{
			return;
		}
		if (this.isCollidingTimer > 0f)
		{
			this.isColliding = true;
			this.isCollidingTimer -= Time.deltaTime;
		}
		else
		{
			this.isColliding = false;
		}
		if (this.isSliding)
		{
			Vector3 b = this.previousSlidingPosition;
			b.y = 0f;
			Vector3 position = base.transform.position;
			position.y = 0f;
			float num = (position - b).magnitude / Time.deltaTime;
			if (num >= this.slidingSpeedThreshold)
			{
				this.slidingAudioSpeed = Mathf.Lerp(this.slidingAudioSpeed, 1f + num * 0.01f, 10f * Time.deltaTime);
				Materials.Instance.SlideLoop(this.rb.worldCenterOfMass, this.materialTrigger, 1f, 1f + this.slidingAudioSpeed);
			}
			if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
			{
				this.slidingTimer -= Time.deltaTime;
				if (this.slidingTimer < 0f)
				{
					this.isSliding = false;
				}
			}
		}
		this.previousSlidingPosition = base.transform.position;
		if (this.playerHurtMultiplierTimer > 0f)
		{
			this.playerHurtMultiplierTimer -= Time.deltaTime;
			if (this.playerHurtMultiplierTimer <= 0f)
			{
				this.playerHurtMultiplier = 1f;
			}
		}
		if (this.physGrabObject.grabbed)
		{
			this.collisionsActiveTimer = 0.5f;
		}
		if (this.rb.velocity.magnitude > 0.01f || this.rb.angularVelocity.magnitude > 0.1f)
		{
			this.collisionsActiveTimer = 0.5f;
		}
		if (this.collisionsActiveTimer > 0f)
		{
			if (!this.collisionsActive)
			{
				this.collisionActivatedBuffer = 0.1f;
			}
			this.collisionsActive = true;
			this.collisionsActiveTimer -= Time.deltaTime;
		}
		else
		{
			this.collisionsActive = false;
		}
		if (this.collisionActivatedBuffer > 0f)
		{
			this.collisionActivatedBuffer -= Time.deltaTime;
		}
		if (this.breakLevel1Cooldown > 0f)
		{
			this.breakLevel1Cooldown -= Time.deltaTime;
		}
		if (this.breakLevel2Cooldown > 0f)
		{
			this.breakLevel2Cooldown -= Time.deltaTime;
		}
		if (this.breakLevel3Cooldown > 0f)
		{
			this.breakLevel3Cooldown -= Time.deltaTime;
		}
		if (this.impactLightCooldown > 0f)
		{
			this.impactLightCooldown -= Time.deltaTime;
		}
		if (this.impactMediumCooldown > 0f)
		{
			this.impactMediumCooldown -= Time.deltaTime;
		}
		if (this.impactHeavyCooldown > 0f)
		{
			this.impactHeavyCooldown -= Time.deltaTime;
		}
		if (this.impactCooldown > 0f)
		{
			this.impactCooldown -= Time.deltaTime;
		}
		if (this.impulseTimerDeactivateImpacts > 0f)
		{
			this.impulseTimerDeactivateImpacts -= Time.deltaTime;
		}
		if (this.resetPrevPositionTimer > 0f)
		{
			this.resetPrevPositionTimer -= Time.deltaTime;
			this.previousPosition = Vector3.zero;
		}
		if (this.enemyInteractionTimer > 0f)
		{
			this.enemyInteractionTimer -= Time.deltaTime;
		}
		if (this.destroyDisableLaunchesTimer > 0f)
		{
			this.destroyDisableLaunchesTimer -= Time.deltaTime;
			if (this.destroyDisableLaunchesTimer <= 0f)
			{
				this.destroyDisableLaunches = 0;
			}
		}
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x000739C4 File Offset: 0x00071BC4
	private void FixedUpdate()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.inCart && !this.isEnemy && this.physGrabObject.playerGrabbing.Count == 0 && this.currentCart && !this.rb.isKinematic && !base.GetComponent<PlayerTumble>())
		{
			PhysGrabCart component = this.currentCart.GetComponent<PhysGrabCart>();
			if (component.actualVelocity.magnitude > 1f)
			{
				Vector3 velocity = this.rb.velocity;
				this.rb.velocity = Vector3.Lerp(this.rb.velocity, component.actualVelocity, 30f * Time.fixedDeltaTime);
				if (this.rb.velocity.y > velocity.y)
				{
					this.rb.velocity = new Vector3(this.rb.velocity.x, velocity.y, this.rb.velocity.z);
				}
			}
		}
		this.impactHappened = false;
		this.breakForce = 0f;
		this.impactForce = 0f;
		if (this.impactDisabledTimer <= 0f)
		{
			Vector3 vector = this.rb.velocity / Time.fixedDeltaTime;
			Vector3 vector2 = this.rb.angularVelocity / Time.fixedDeltaTime;
			float magnitude = this.previousVelocity.magnitude;
			float num = Mathf.Abs(magnitude - vector.magnitude);
			float magnitude2 = this.previousAngularVelocity.magnitude;
			float num2 = Mathf.Abs(magnitude2 - vector2.magnitude);
			Vector3 normalized = vector.normalized;
			Vector3 normalized2 = this.previousVelocity.normalized;
			float num3 = Vector3.Angle(normalized, normalized2);
			Vector3 normalized3 = vector2.normalized;
			Vector3 normalized4 = this.previousAngularVelocity.normalized;
			float num4 = Vector3.Angle(normalized3, normalized4);
			num *= 1f;
			num2 *= 0.4f * this.rb.mass;
			num3 *= 0.2f;
			num4 *= 0.02f * this.rb.mass;
			if ((num > 1f && magnitude > 1f) || (num2 > 1f && magnitude2 > 1f) || (num3 > 1f && magnitude > 1f) || (num4 > 1f && magnitude2 > 1f))
			{
				this.impactHappened = true;
				float num5 = num * 2f;
				float num6 = Mathf.Max(this.rb.mass, 1f);
				this.breakForce += num5 * num6;
			}
			this.breakForce *= 8f;
			this.impactForce = this.breakForce / 8f * this.impactFragilityMultiplier;
			this.breakForce = this.breakForce * (this.fragility / 100f) * this.fragilityMultiplier;
			if (this.impactHappened)
			{
				if (this.inCart)
				{
					this.breakForce = 0f;
				}
				if (this.inCart || this.isCart)
				{
					this.impactForce *= 0.3f;
				}
			}
		}
		else
		{
			this.impactDisabledTimer -= Time.fixedDeltaTime;
		}
		this.previousPreviousVelocityRaw = this.previousVelocityRaw;
		this.previousVelocityRaw = this.rb.velocity;
		this.previousVelocity = this.rb.velocity / Time.fixedDeltaTime;
		this.previousAngularVelocity = this.rb.angularVelocity / Time.fixedDeltaTime;
		if (Vector3.Distance(this.prevPos, base.transform.position) > 0.01f || Quaternion.Angle(this.prevRot, base.transform.rotation) > 0.1f)
		{
			this.isMoving = true;
		}
		this.prevPos = base.transform.position;
		this.prevRot = base.transform.rotation;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x00073DCB File Offset: 0x00071FCB
	public void ImpactDisable(float time)
	{
		this.impactDisabledTimer = time;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00073DD4 File Offset: 0x00071FD4
	private void EnemyInvestigate(float radius)
	{
		if (this.physGrabObject.enemyInteractTimer > 0f || this.inCart || this.isCart)
		{
			return;
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position, radius);
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00073E10 File Offset: 0x00072010
	public void DestroyObject(bool effects = true)
	{
		if (this.destroyDisable)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		EnemyRigidbody component = base.GetComponent<EnemyRigidbody>();
		if (component)
		{
			component.enemy.EnemyParent.Despawn();
			return;
		}
		if (!this.physGrabObject.dead)
		{
			this.physGrabObject.dead = true;
			this.EnemyInvestigate(15f);
			if (!SemiFunc.IsMultiplayer())
			{
				this.DestroyObjectRPC(effects);
				return;
			}
			this.photonView.RPC("DestroyObjectRPC", RpcTarget.All, new object[]
			{
				effects
			});
		}
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00073EA4 File Offset: 0x000720A4
	[PunRPC]
	public void DestroyObjectRPC(bool effects)
	{
		this.physGrabObject.dead = true;
		if (effects)
		{
			GameDirector.instance.CameraImpact.ShakeDistance(10f, 1f, 6f, base.transform.position, 0.1f);
		}
		if (this.particles)
		{
			this.particles.transform.parent = null;
			this.particles.DestroyParticles();
		}
		if (this.audioActive && effects)
		{
			AudioSource audioSource = this.impactAudio.destroy.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		this.onDestroy.Invoke();
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00073F78 File Offset: 0x00072178
	public void BreakHeavy(Vector3 contactPoint)
	{
		float num = 0.1f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelHeavy);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Heavy);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(10f);
		}
		this.breakLevel3Cooldown = 0.6f;
		this.breakLevel2Cooldown = 0.4f;
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00074074 File Offset: 0x00072274
	public void BreakMedium(Vector3 contactPoint)
	{
		float num = 0.05f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelMedium);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Medium);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(5f);
		}
		this.breakLevel2Cooldown = 0.4f;
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00074164 File Offset: 0x00072364
	public void BreakLight(Vector3 contactPoint)
	{
		float num = 0.01f * (1f + 9f * (100f - this.durability) / 100f);
		bool flag = false;
		if (this.isValuable || this.breakLogic)
		{
			float num2 = 0f;
			if (this.isValuable)
			{
				num2 = Mathf.Round(this.valuableObject.dollarValueOriginal * num);
				num2 += Mathf.Round(Random.Range(-num2 * 0.1f, num2 * 0.1f));
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueCurrent);
			}
			this.Break(num2, contactPoint, this.breakLevelLight);
			flag = true;
		}
		if (this.isNotValuable && this.notValuableObject.hasHealth)
		{
			this.notValuableObject.Impact(PhysGrabObjectImpactDetector.ImpactState.Light);
			flag = true;
		}
		if (flag)
		{
			this.EnemyInvestigate(3f);
		}
		this.breakLevel1Cooldown = 0.3f;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00074248 File Offset: 0x00072448
	internal void Break(float valueLost, Vector3 _contactPoint, int breakLevel)
	{
		bool flag = false;
		if (this.isValuable && !this.isIndestructible && !this.destroyDisable)
		{
			flag = true;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.BreakRPC(valueLost, _contactPoint, breakLevel, flag);
			return;
		}
		this.photonView.RPC("BreakRPC", RpcTarget.All, new object[]
		{
			valueLost,
			_contactPoint,
			breakLevel,
			flag
		});
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x000742C4 File Offset: 0x000724C4
	private void HealLogic(float healAmount, Vector3 healingPoint)
	{
		this.valuableObject.dollarValueCurrent += Mathf.Floor(healAmount);
		this.valuableObject.dollarValueCurrent = Mathf.Clamp(this.valuableObject.dollarValueCurrent, 0f, this.valuableObject.dollarValueOriginal);
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x00074314 File Offset: 0x00072514
	public float Heal(float healPercent, Vector3 healingPoint)
	{
		float result = 0f;
		if (this.isValuable)
		{
			if (GameManager.Multiplayer())
			{
				if (PhotonNetwork.IsMasterClient)
				{
					float num = this.valuableObject.dollarValueOriginal * healPercent;
					num = Mathf.Clamp(num, 0f, this.valuableObject.dollarValueOriginal - this.valuableObject.dollarValueCurrent);
					if (num > 0f)
					{
						this.photonView.RPC("HealRPC", RpcTarget.All, new object[]
						{
							this.valuableObject.dollarValueOriginal * healPercent
						});
					}
					result = num;
				}
			}
			else
			{
				float num2 = this.valuableObject.dollarValueOriginal * healPercent;
				num2 = Mathf.Clamp(num2, 0f, this.valuableObject.dollarValueOriginal - this.valuableObject.dollarValueCurrent);
				if (num2 > 0f)
				{
					this.HealLogic(num2, healingPoint);
				}
				result = num2;
			}
		}
		return result;
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x000743F0 File Offset: 0x000725F0
	[PunRPC]
	private void HealRPC(float healAmount, Vector3 healingPoint)
	{
		this.HealLogic(healAmount, healingPoint);
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x000743FC File Offset: 0x000725FC
	private void ResetObject()
	{
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing.ToList<PhysGrabber>())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
		}
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
		this.valuableObject.dollarValueCurrent = this.valuableObject.dollarValueOriginal;
		this.rb.position = this.originalPosition;
		this.rb.rotation = this.originalRotation;
		base.transform.position = this.originalPosition;
		AssetManager.instance.soundUnequip.Play(this.originalPosition, 1f, 1f, 1f, 1f);
		this.BreakEffect(this.breakLevelLight, this.originalPosition);
		Vector3 position = this.physGrabObject.transform.TransformPoint(this.physGrabObject.midPointOffset);
		Object.Instantiate<GameObject>(AssetManager.instance.prefabTeleportEffect, position, Quaternion.identity).transform.localScale = Vector3.one * 2f;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00074584 File Offset: 0x00072784
	[PunRPC]
	private void BreakRPC(float valueLost, Vector3 _contactPoint, int breakLevel, bool _loseValue)
	{
		if (_loseValue)
		{
			if (this.valuableObject)
			{
				float dollarValueCurrent = this.valuableObject.dollarValueCurrent;
				this.valuableObject.dollarValueCurrent -= valueLost;
				bool flag = false;
				if (this.valuableObject.dollarValueCurrent < this.valuableObject.dollarValueOriginal * 0.15f)
				{
					if (!SemiFunc.RunIsTutorial())
					{
						this.DestroyObject(true);
					}
					else
					{
						if (this.particles)
						{
							this.particles.DestroyParticles();
						}
						this.ResetObject();
						this.ImpactHeavy(1000f, _contactPoint);
					}
					flag = true;
				}
				if (flag)
				{
					valueLost = dollarValueCurrent;
				}
			}
			WorldSpaceUIParent.instance.ValueLostCreate(_contactPoint, (int)valueLost);
		}
		this.onAllBreaks.Invoke();
		if (breakLevel == this.breakLevelHeavy)
		{
			this.onBreakHeavy.Invoke();
			if (this.physGrabObject)
			{
				this.physGrabObject.heavyBreakImpulse = false;
			}
		}
		if (breakLevel == this.breakLevelMedium)
		{
			this.onBreakMedium.Invoke();
			if (this.physGrabObject)
			{
				this.physGrabObject.mediumBreakImpulse = false;
			}
		}
		if (breakLevel == this.breakLevelLight)
		{
			this.onBreakLight.Invoke();
			if (this.physGrabObject)
			{
				this.physGrabObject.lightBreakImpulse = false;
			}
		}
		this.BreakEffect(breakLevel, _contactPoint);
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000746D0 File Offset: 0x000728D0
	public void BreakEffect(int breakLevel, Vector3 contactPoint)
	{
		if (!this.particleDisable && this.particles)
		{
			this.particles.ImpactSmoke(5, contactPoint, this.colliderVolume);
		}
		if (breakLevel == this.breakLevelHeavy)
		{
			if (this.audioActive && this.impactAudio)
			{
				this.impactAudio.breakHeavy.Play(contactPoint, 1f, 1f, 1f, 1f);
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 10f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel == this.breakLevelMedium)
		{
			if (this.audioActive && this.impactAudio)
			{
				AudioSource audioSource = this.impactAudio.breakMedium.Play(contactPoint, 1f, 1f, 1f, 1f);
				if (audioSource)
				{
					audioSource.pitch *= this.impactAudioPitch;
				}
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 5f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(3f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel == this.breakLevelLight)
		{
			if (this.audioActive && this.impactAudio)
			{
				AudioSource audioSource2 = this.impactAudio.breakLight.Play(contactPoint, 1f, 1f, 1f, 1f);
				if (audioSource2)
				{
					audioSource2.pitch *= this.impactAudioPitch;
				}
			}
			if (this.physGrabObject)
			{
				SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 3f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(1f, 1f, 6f, contactPoint, 0.1f);
		}
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00074904 File Offset: 0x00072B04
	public void ImpactHeavy(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactHeavyRPC(force, contactPoint);
		}
		else
		{
			this.photonView.RPC("ImpactHeavyRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactHeavyTimer = 0.1f;
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00074970 File Offset: 0x00072B70
	[PunRPC]
	private void ImpactHeavyRPC(float force, Vector3 contactPoint)
	{
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 8f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float volumeMultiplier = this.ImpactSoundGetVolume(force, this.impactAudio.impactHeavy.Volume);
			AudioSource audioSource = this.impactAudio.impactHeavy.Play(contactPoint, volumeMultiplier, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		if (!this.particleDisable && !this.inCart && this.particles)
		{
			this.particles.ImpactSmoke(5, contactPoint, this.colliderVolume);
		}
		this.onAllImpacts.Invoke();
		this.onImpactHeavy.Invoke();
		this.EnemyInvestigate(1f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactHeavyTimer = 0.1f;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00074A90 File Offset: 0x00072C90
	public void ImpactMedium(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactMediumRPC(force, contactPoint);
		}
		else
		{
			this.photonView.RPC("ImpactMediumRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactMediumTimer = 0.1f;
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00074AFC File Offset: 0x00072CFC
	[PunRPC]
	private void ImpactMediumRPC(float force, Vector3 contactPoint)
	{
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 5f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float volumeMultiplier = this.ImpactSoundGetVolume(force, this.impactAudio.impactMedium.Volume);
			AudioSource audioSource = this.impactAudio.impactMedium.Play(contactPoint, volumeMultiplier, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		this.onImpactMedium.Invoke();
		this.onAllImpacts.Invoke();
		if (!this.rb.isKinematic)
		{
			this.rb.angularVelocity *= 0.55f;
		}
		this.EnemyInvestigate(0.5f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactMediumTimer = 0.1f;
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00074C14 File Offset: 0x00072E14
	private float ImpactSoundGetVolume(float force, float volume)
	{
		float num = Mathf.Clamp01(force * 0.01f);
		if (this.inCart)
		{
			num *= this.inCartVolumeMultiplier;
		}
		return Mathf.Clamp(num, 0.1f, 1f);
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00074C50 File Offset: 0x00072E50
	public void ImpactLight(float force, Vector3 contactPoint)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ImpactLightRPC(force, contactPoint);
		}
		else
		{
			this.photonView.RPC("ImpactLightRPC", RpcTarget.All, new object[]
			{
				force,
				contactPoint
			});
		}
		this.EnemyInvestigate(0.2f);
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactLightTimer = 0.1f;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00074CC7 File Offset: 0x00072EC7
	public void changeInCart()
	{
		this.timerInCart = 0.1f;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00074CD4 File Offset: 0x00072ED4
	[PunRPC]
	private void ImpactLightRPC(float force, Vector3 contactPoint)
	{
		if (!this.physGrabObject)
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(this.physGrabObject.centerPoint, 1f, base.gameObject, 3f);
		if (this.audioActive && !this.isHinge && this.impactAudio)
		{
			float num = this.ImpactSoundGetVolume(force, this.impactAudio.impactLight.Volume);
			if (this.inCart)
			{
				num *= this.inCartVolumeMultiplier;
			}
			AudioSource audioSource = this.impactAudio.impactLight.Play(contactPoint, num, 1f, 1f, 1f);
			if (audioSource)
			{
				audioSource.pitch *= this.impactAudioPitch;
			}
		}
		if (!this.rb.isKinematic)
		{
			this.rb.angularVelocity *= 0.6f;
		}
		this.onAllImpacts.Invoke();
		this.onImpactLight.Invoke();
		this.physGrabObject.impactHappenedTimer = 0.1f;
		this.physGrabObject.impactLightTimer = 0.1f;
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00074DF0 File Offset: 0x00072FF0
	private void OnTriggerStay(Collider other)
	{
		if ((GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient) && other.CompareTag("Cart"))
		{
			this.currentCart = other.GetComponentInParent<PhysGrabCart>();
			if (this.currentCart)
			{
				this.currentCart.physGrabInCart.Add(this.physGrabObject);
			}
			this.changeInCart();
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00074E54 File Offset: 0x00073054
	private void OnCollisionStay(Collision collision)
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.collisionsActive)
		{
			return;
		}
		if (!this.isMoving)
		{
			return;
		}
		this.isCollidingTimer = 0.1f;
		if (!this.isHinge && !this.slidingDisable && !this.isCart && !this.inCart && !this.isCart && this.isValuable && this.valuableObject.volumeType >= ValuableVolume.Type.Medium && collision.gameObject.GetComponent<MaterialSurface>())
		{
			if (this.rb.velocity.magnitude > this.slidingSpeedThreshold && (Mathf.Abs(this.rb.velocity.x) > Mathf.Abs(this.rb.velocity.y) || Mathf.Abs(this.rb.velocity.z) > Mathf.Abs(this.rb.velocity.y)))
			{
				this.isSliding = true;
				this.slidingTimer = 0.1f;
			}
			PhysGrabObject component = collision.gameObject.GetComponent<PhysGrabObject>();
			if (component && component.rb.velocity.magnitude > this.rb.velocity.magnitude * 0.8f)
			{
				this.isSliding = false;
			}
		}
		Vector3 a = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			a += contactPoint.point;
		}
		if (collision.contacts.Length != 0)
		{
			this.contactPoint = a / (float)collision.contacts.Length;
		}
		else
		{
			this.contactPoint = Vector3.zero;
		}
		PhysGrabObjectImpactDetector component2 = collision.gameObject.GetComponent<PhysGrabObjectImpactDetector>();
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		bool flag3 = this.isCart && this.contactPoint.y < base.transform.position.y;
		if (this.impactHappened && (!this.isCart || !component2 || !component2.inCart) && !flag3)
		{
			if (this.impulseTimerDeactivateImpacts <= 0f)
			{
				if (this.impactForce > 150f && this.impactHeavyCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactHeavy(this.impactForce, this.contactPoint);
					this.impactHeavyCooldown = 0.5f;
					this.impactMediumCooldown = 0.5f;
					this.impactLightCooldown = 0.5f;
				}
				if (this.impactForce > 80f && this.impactMediumCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactMedium(this.impactForce, this.contactPoint);
					this.impactMediumCooldown = 0.5f;
					this.impactLightCooldown = 0.5f;
				}
				if (this.impactForce > 20f && this.impactLightCooldown <= 0f)
				{
					flag2 = true;
					this.ImpactLight(this.impactForce, this.contactPoint);
					this.impactLightCooldown = 0.5f;
				}
			}
			if (this.indestructibleSpawnTimer <= 0f)
			{
				float num2 = Mathf.Max(this.rb.mass, 1f);
				if (this.breakForce > this.impactLevel3 * num2 && this.breakLevel3Cooldown <= 0f && !this.inCart)
				{
					flag = true;
					num = 3;
				}
				if (this.breakForce > this.impactLevel2 * num2 && this.breakLevel2Cooldown <= 0f && !flag && !this.inCart)
				{
					flag = true;
					num = 2;
				}
				if (this.breakForce > this.impactLevel1 * num2 && this.breakLevel1Cooldown <= 0f && !flag && !this.inCart)
				{
					flag = true;
					num = 1;
				}
			}
		}
		bool flag4 = false;
		bool flag5 = false;
		if (flag && (!this.isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag2 && (!this.isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag && this.isBrokenHinge)
		{
			flag5 = true;
		}
		if (!this.canHurtLogic)
		{
			flag4 = false;
		}
		bool flag6 = false;
		if (flag4 && (flag || (flag2 && this.isCart)))
		{
			bool flag7 = false;
			PlayerTumble playerTumble = null;
			if (collision.transform.CompareTag("Player"))
			{
				flag7 = true;
			}
			else
			{
				playerTumble = collision.transform.GetComponent<PlayerTumble>();
				if (playerTumble)
				{
					flag7 = true;
				}
			}
			if (flag7 && this.isCart)
			{
				if (this.physGrabObject.playerGrabbing.Count <= 0)
				{
					flag7 = false;
				}
				else if (this.cart.inCart.GetComponent<BoxCollider>().bounds.Contains(collision.transform.position))
				{
					flag7 = false;
				}
			}
			if (flag7)
			{
				PlayerController componentInParent = collision.transform.GetComponentInParent<PlayerController>();
				PlayerAvatar playerAvatar;
				if (playerTumble)
				{
					playerAvatar = playerTumble.playerAvatar;
				}
				else if (componentInParent)
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
				else
				{
					playerAvatar = collision.transform.GetComponentInParent<PlayerAvatar>();
					if (!playerAvatar)
					{
						playerAvatar = collision.transform.GetComponent<PlayerAvatar>();
					}
					if (!playerAvatar)
					{
						playerAvatar = collision.transform.GetComponentInChildren<PlayerAvatar>();
					}
					if (!playerAvatar)
					{
						PlayerPhysPusher component3 = collision.transform.GetComponent<PlayerPhysPusher>();
						if (component3)
						{
							playerAvatar = component3.Player;
						}
					}
				}
				bool flag8 = false;
				using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.playerAvatar == playerAvatar)
						{
							flag8 = true;
							break;
						}
					}
				}
				if (playerAvatar && !flag8)
				{
					Vector3 vector = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position - this.contactPoint;
					float magnitude = this.previousPreviousVelocityRaw.magnitude;
					Vector3 direction = Vector3.Lerp(this.previousPreviousVelocityRaw.normalized, vector.normalized, 0f);
					if (magnitude >= 3f)
					{
						PlayerAvatar playerAvatar2 = null;
						foreach (RaycastHit raycastHit in this.rb.SweepTestAll(direction, 1f, QueryTriggerInteraction.Collide))
						{
							playerAvatar2 = this.ImpactGetPlayer(raycastHit.collider, componentInParent, playerTumble);
							if (playerAvatar2 == playerAvatar)
							{
								break;
							}
						}
						if (!playerAvatar2)
						{
							foreach (Collider hit in Physics.OverlapSphere(this.contactPoint, 0.2f, LayerMask.GetMask(new string[]
							{
								"Player"
							})))
							{
								playerAvatar2 = this.ImpactGetPlayer(hit, componentInParent, playerTumble);
								if (playerAvatar2 == playerAvatar)
								{
									break;
								}
							}
						}
						if (playerAvatar2 == playerAvatar)
						{
							bool flag9 = false;
							float time = 0.1f;
							if (!this.playerHurtDisable && !this.isIndestructible && !this.destroyDisable && this.isValuable)
							{
								time = 0.15f;
								int damage = Mathf.RoundToInt((float)(5 * num) * (this.rb.mass * 0.5f) * this.playerHurtMultiplier);
								playerAvatar.playerHealth.HurtOther(damage, this.contactPoint, true, -1);
								flag9 = true;
							}
							bool flag10 = false;
							if (this.isHinge)
							{
								if (magnitude >= 3f)
								{
									flag10 = true;
								}
							}
							else if (this.isCart)
							{
								if (magnitude >= 3f)
								{
									flag10 = true;
								}
							}
							else if (magnitude >= 6f)
							{
								flag10 = true;
							}
							if (flag9 || flag10)
							{
								if (!playerTumble)
								{
									playerTumble = playerAvatar.tumble;
								}
								playerTumble.TumbleRequest(true, false);
								playerTumble.TumbleOverrideTime(2f);
								Vector3 force = vector.normalized * 4f * (float)num;
								Vector3 torque = Vector3.Cross((playerAvatar.localCameraPosition - this.contactPoint).normalized, playerAvatar.transform.forward) * 5f * force.magnitude;
								playerTumble.physGrabObject.FreezeForces(time, force, torque);
								playerAvatar.playerHealth.HurtFreezeOverride(time);
								this.physGrabObject.FreezeForces(time, Vector3.zero, Vector3.zero);
								flag6 = true;
							}
						}
					}
				}
			}
		}
		if ((flag4 || flag5) && !this.playerHurtDisable && this.enemyInteractionTimer <= 0f && !this.isIndestructible && !this.destroyDisable && (this.isValuable || this.isEnemy || flag5) && collision.transform.CompareTag("Enemy"))
		{
			EnemyRigidbody component4 = collision.transform.GetComponent<EnemyRigidbody>();
			if (component4 && component4.enemy.HasHealth && component4.enemy.Health.objectHurt && component4.enemy.Health.objectHurtDisableTimer <= 0f)
			{
				Vector3 vector2 = component4.physGrabObject.centerPoint - this.contactPoint;
				float magnitude2 = this.previousPreviousVelocityRaw.magnitude;
				Vector3 direction2 = Vector3.Lerp(this.previousPreviousVelocityRaw.normalized, vector2.normalized, 0.5f);
				if (magnitude2 > 2f)
				{
					EnemyRigidbody enemyRigidbody = null;
					foreach (RaycastHit raycastHit2 in this.rb.SweepTestAll(direction2, 1f, QueryTriggerInteraction.Collide))
					{
						enemyRigidbody = raycastHit2.transform.GetComponent<EnemyRigidbody>();
					}
					if (!enemyRigidbody)
					{
						Collider[] array2 = Physics.OverlapSphere(this.contactPoint, 0.2f, SemiFunc.LayerMaskGetPhysGrabObject());
						for (int i = 0; i < array2.Length; i++)
						{
							enemyRigidbody = array2[i].GetComponentInParent<EnemyRigidbody>();
						}
					}
					if (enemyRigidbody == component4)
					{
						flag6 = true;
						int num3 = Mathf.RoundToInt((float)(10 * num) * (this.rb.mass * 0.5f));
						num3 = Mathf.RoundToInt((float)num3 * component4.enemy.Health.objectHurtMultiplier);
						component4.enemy.Health.Hurt(num3, -vector2.normalized);
						if (component4.enemy.Health.onObjectHurt != null)
						{
							if (this.physGrabObject.grabbedTimer > 0f)
							{
								component4.enemy.Health.onObjectHurtPlayer = this.physGrabObject.lastPlayerGrabbing;
							}
							else
							{
								component4.enemy.Health.onObjectHurtPlayer = null;
							}
							component4.enemy.Health.onObjectHurt.Invoke();
						}
						Vector3 force2 = vector2.normalized * (2f * (float)num);
						component4.rb.AddForce(force2, ForceMode.Impulse);
						Vector3 normalized = vector2.normalized;
						Vector3 rhs = -component4.rb.transform.up;
						Vector3 torque2 = Vector3.Cross(normalized, rhs) * (2f * (float)num);
						component4.rb.AddTorque(torque2, ForceMode.Impulse);
						EnemyType type = component4.enemy.Type;
						if (this.isValuable)
						{
							if (component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								float mass = this.valuableObject.physAttributePreset.mass;
								bool flag11 = false;
								if (mass >= 2f)
								{
									flag11 = true;
								}
								else if (type <= EnemyType.Medium)
								{
									flag11 = true;
								}
								if (flag11)
								{
									component4.enemy.StateStunned.Set(2f);
								}
							}
						}
						else if (this.isBrokenHinge)
						{
							if (type <= EnemyType.Medium && component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								component4.enemy.StateStunned.Set(2f);
							}
							this.DestroyObject(true);
						}
					}
				}
			}
		}
		if (flag6)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				this.ImpactEffectRPC(this.contactPoint);
			}
			else
			{
				this.photonView.RPC("ImpactEffectRPC", RpcTarget.All, new object[]
				{
					this.contactPoint
				});
			}
		}
		if (flag && this.physGrabObject.overrideDisableBreakEffectsTimer <= 0f)
		{
			if ((this.destroyDisable || this.isIndestructible || !this.isValuable) && !this.indestructibleBreakEffects)
			{
				if (!flag2)
				{
					if (num == 1)
					{
						this.ImpactLight(this.impactForce, this.contactPoint);
					}
					if (num == 2)
					{
						this.ImpactMedium(this.impactForce, this.contactPoint);
					}
					if (num == 3)
					{
						this.ImpactHeavy(this.impactForce, this.contactPoint);
						return;
					}
				}
			}
			else
			{
				if (num == 1)
				{
					this.BreakLight(this.contactPoint);
				}
				if (num == 2)
				{
					this.BreakMedium(this.contactPoint);
				}
				if (num == 3)
				{
					this.BreakHeavy(this.contactPoint);
				}
			}
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00075B64 File Offset: 0x00073D64
	[PunRPC]
	private void ImpactEffectRPC(Vector3 _position)
	{
		AssetManager.instance.PhysImpactEffect(_position);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00075B74 File Offset: 0x00073D74
	private PlayerAvatar ImpactGetPlayer(Collider _hit, PlayerController _playerController, PlayerTumble _playerTumble)
	{
		if (_playerTumble)
		{
			PlayerTumble playerTumble = _hit.transform.GetComponent<PlayerTumble>();
			if (!playerTumble)
			{
				playerTumble = _hit.transform.GetComponentInParent<PlayerTumble>();
			}
			if (playerTumble)
			{
				return playerTumble.playerAvatar;
			}
		}
		PlayerAvatar playerAvatar = _hit.transform.GetComponentInParent<PlayerAvatar>();
		if (!playerAvatar)
		{
			playerAvatar = _hit.transform.GetComponent<PlayerAvatar>();
		}
		if (!playerAvatar)
		{
			playerAvatar = _hit.transform.GetComponentInChildren<PlayerAvatar>();
		}
		if (!playerAvatar)
		{
			PlayerPhysPusher component = _hit.transform.GetComponent<PlayerPhysPusher>();
			if (component)
			{
				playerAvatar = component.Player;
			}
		}
		if (!playerAvatar && _playerController)
		{
			PlayerController playerController = _hit.transform.GetComponentInParent<PlayerController>();
			if (!playerController && _hit.transform.GetComponentInParent<PlayerCollisionController>())
			{
				playerController = PlayerController.instance;
			}
			if (playerController)
			{
				playerAvatar = playerController.playerAvatarScript;
			}
		}
		return playerAvatar;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00075C5D File Offset: 0x00073E5D
	public void PlayerHurtMultiplier(float _multiplier, float _time)
	{
		this.playerHurtMultiplier = _multiplier;
		this.playerHurtMultiplierTimer = _time;
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00075C6D File Offset: 0x00073E6D
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x04001526 RID: 5414
	public bool particleDisable;

	// Token: 0x04001527 RID: 5415
	[Range(0f, 4f)]
	public float particleMultiplier = 1f;

	// Token: 0x04001528 RID: 5416
	[Space]
	public bool playerHurtDisable;

	// Token: 0x04001529 RID: 5417
	public bool slidingDisable;

	// Token: 0x0400152A RID: 5418
	public bool destroyDisable;

	// Token: 0x0400152B RID: 5419
	internal int destroyDisableLaunches;

	// Token: 0x0400152C RID: 5420
	internal float destroyDisableLaunchesTimer;

	// Token: 0x0400152D RID: 5421
	internal bool destroyDisableTeleport = true;

	// Token: 0x0400152E RID: 5422
	public bool indestructibleBreakEffects = true;

	// Token: 0x0400152F RID: 5423
	public bool canHurtLogic = true;

	// Token: 0x04001530 RID: 5424
	[HideInInspector]
	public PhysObjectParticles particles;

	// Token: 0x04001531 RID: 5425
	private List<Transform> colliderTransforms = new List<Transform>();

	// Token: 0x04001532 RID: 5426
	private EnemyRigidbody enemyRigidbody;

	// Token: 0x04001533 RID: 5427
	[HideInInspector]
	public bool isEnemy;

	// Token: 0x04001534 RID: 5428
	internal float enemyInteractionTimer;

	// Token: 0x04001535 RID: 5429
	private Rigidbody rb;

	// Token: 0x04001536 RID: 5430
	private Materials.MaterialTrigger materialTrigger = new Materials.MaterialTrigger();

	// Token: 0x04001537 RID: 5431
	[HideInInspector]
	public float fragility = 50f;

	// Token: 0x04001538 RID: 5432
	[HideInInspector]
	public float durability = 100f;

	// Token: 0x04001539 RID: 5433
	private float impactLevel1 = 300f;

	// Token: 0x0400153A RID: 5434
	private float impactLevel2 = 400f;

	// Token: 0x0400153B RID: 5435
	private float impactLevel3 = 500f;

	// Token: 0x0400153C RID: 5436
	private float breakLevel1Cooldown;

	// Token: 0x0400153D RID: 5437
	private float breakLevel2Cooldown;

	// Token: 0x0400153E RID: 5438
	private float breakLevel3Cooldown;

	// Token: 0x0400153F RID: 5439
	private float impactLightCooldown;

	// Token: 0x04001540 RID: 5440
	private float impactMediumCooldown;

	// Token: 0x04001541 RID: 5441
	private float impactHeavyCooldown;

	// Token: 0x04001542 RID: 5442
	private Vector3 previousPosition;

	// Token: 0x04001543 RID: 5443
	private Vector3 previousRotation;

	// Token: 0x04001544 RID: 5444
	private Camera mainCamera;

	// Token: 0x04001545 RID: 5445
	private float impactCooldown;

	// Token: 0x04001546 RID: 5446
	internal bool isIndestructible;

	// Token: 0x04001547 RID: 5447
	internal float impulseTimerDeactivateImpacts = 5f;

	// Token: 0x04001548 RID: 5448
	internal float highestVelocity;

	// Token: 0x04001549 RID: 5449
	internal float impactForce;

	// Token: 0x0400154A RID: 5450
	internal float resetPrevPositionTimer;

	// Token: 0x0400154B RID: 5451
	private PhysGrabObject physGrabObject;

	// Token: 0x0400154C RID: 5452
	private PhotonView photonView;

	// Token: 0x0400154D RID: 5453
	internal bool isHinge;

	// Token: 0x0400154E RID: 5454
	internal bool isBrokenHinge;

	// Token: 0x0400154F RID: 5455
	private ValuableObject valuableObject;

	// Token: 0x04001550 RID: 5456
	private NotValuableObject notValuableObject;

	// Token: 0x04001551 RID: 5457
	private bool isNotValuable;

	// Token: 0x04001552 RID: 5458
	private bool breakLogic;

	// Token: 0x04001553 RID: 5459
	[HideInInspector]
	public bool isValuable;

	// Token: 0x04001554 RID: 5460
	private bool collisionsActive;

	// Token: 0x04001555 RID: 5461
	private float collisionsActiveTimer;

	// Token: 0x04001556 RID: 5462
	private float collisionActivatedBuffer;

	// Token: 0x04001557 RID: 5463
	[HideInInspector]
	public bool isSliding;

	// Token: 0x04001558 RID: 5464
	private float slidingTimer;

	// Token: 0x04001559 RID: 5465
	private float slidingGain;

	// Token: 0x0400155A RID: 5466
	private float slidingSpeedThreshold = 0.1f;

	// Token: 0x0400155B RID: 5467
	private float slidingAudioSpeed;

	// Token: 0x0400155C RID: 5468
	private Vector3 previousSlidingPosition;

	// Token: 0x0400155D RID: 5469
	internal Vector3 previousVelocity;

	// Token: 0x0400155E RID: 5470
	internal Vector3 previousAngularVelocity;

	// Token: 0x0400155F RID: 5471
	internal Vector3 previousVelocityRaw;

	// Token: 0x04001560 RID: 5472
	internal Vector3 previousPreviousVelocityRaw;

	// Token: 0x04001561 RID: 5473
	private bool impactHappened;

	// Token: 0x04001562 RID: 5474
	internal float impactDisabledTimer;

	// Token: 0x04001563 RID: 5475
	private Vector3 contactPoint;

	// Token: 0x04001564 RID: 5476
	private PhysAudio impactAudio;

	// Token: 0x04001565 RID: 5477
	private float impactAudioPitch = 1f;

	// Token: 0x04001566 RID: 5478
	private bool audioActive;

	// Token: 0x04001567 RID: 5479
	private float colliderVolume;

	// Token: 0x04001568 RID: 5480
	private float timerInCart;

	// Token: 0x04001569 RID: 5481
	internal int breakLevelHeavy;

	// Token: 0x0400156A RID: 5482
	internal int breakLevelMedium = 1;

	// Token: 0x0400156B RID: 5483
	internal int breakLevelLight = 2;

	// Token: 0x0400156C RID: 5484
	private Vector3 prevPos;

	// Token: 0x0400156D RID: 5485
	private Quaternion prevRot;

	// Token: 0x0400156E RID: 5486
	private bool isMoving;

	// Token: 0x0400156F RID: 5487
	private float breakForce;

	// Token: 0x04001570 RID: 5488
	private Vector3 originalPosition;

	// Token: 0x04001571 RID: 5489
	private Quaternion originalRotation;

	// Token: 0x04001572 RID: 5490
	public UnityEvent onAllImpacts;

	// Token: 0x04001573 RID: 5491
	public UnityEvent onImpactLight;

	// Token: 0x04001574 RID: 5492
	public UnityEvent onImpactMedium;

	// Token: 0x04001575 RID: 5493
	public UnityEvent onImpactHeavy;

	// Token: 0x04001576 RID: 5494
	[Space(15f)]
	public UnityEvent onAllBreaks;

	// Token: 0x04001577 RID: 5495
	public UnityEvent onBreakLight;

	// Token: 0x04001578 RID: 5496
	public UnityEvent onBreakMedium;

	// Token: 0x04001579 RID: 5497
	public UnityEvent onBreakHeavy;

	// Token: 0x0400157A RID: 5498
	[Space(15f)]
	public UnityEvent onDestroy;

	// Token: 0x0400157B RID: 5499
	[HideInInspector]
	public bool inCart;

	// Token: 0x0400157C RID: 5500
	private bool inCartPrevious;

	// Token: 0x0400157D RID: 5501
	[HideInInspector]
	public bool isCart;

	// Token: 0x0400157E RID: 5502
	private PhysGrabCart cart;

	// Token: 0x0400157F RID: 5503
	private float inCartVolumeMultiplier;

	// Token: 0x04001580 RID: 5504
	private float impactCheckTimer;

	// Token: 0x04001581 RID: 5505
	private PhysGrabCart currentCart;

	// Token: 0x04001582 RID: 5506
	internal float indestructibleSpawnTimer = 5f;

	// Token: 0x04001583 RID: 5507
	internal bool isColliding;

	// Token: 0x04001584 RID: 5508
	private float isCollidingTimer;

	// Token: 0x04001585 RID: 5509
	[HideInInspector]
	public float fragilityMultiplier = 1f;

	// Token: 0x04001586 RID: 5510
	[HideInInspector]
	public float impactFragilityMultiplier = 1f;

	// Token: 0x04001587 RID: 5511
	private float playerHurtMultiplier = 1f;

	// Token: 0x04001588 RID: 5512
	private float playerHurtMultiplierTimer;

	// Token: 0x02000365 RID: 869
	public enum ImpactState
	{
		// Token: 0x04002763 RID: 10083
		None,
		// Token: 0x04002764 RID: 10084
		Light,
		// Token: 0x04002765 RID: 10085
		Medium,
		// Token: 0x04002766 RID: 10086
		Heavy
	}
}
