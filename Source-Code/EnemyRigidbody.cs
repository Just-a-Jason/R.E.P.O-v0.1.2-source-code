using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A2 RID: 162
public class EnemyRigidbody : MonoBehaviour
{
	// Token: 0x06000630 RID: 1584 RVA: 0x0003B320 File Offset: 0x00039520
	private void Awake()
	{
		this.enemyParent = base.GetComponentInParent<EnemyParent>();
		this.yOffset = base.transform.position.y - this.followTarget.position.y;
		this.enemy.Rigidbody = this;
		this.enemy.HasRigidbody = true;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.impactDetector.impactFragilityMultiplier = this.impactFragility;
		if (this.playerCollision)
		{
			this.hasPlayerCollision = true;
			this.playerCollisionActive = true;
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.playerCollision.enabled = false;
			}
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0003B3E7 File Offset: 0x000395E7
	public void IdleSet(float time)
	{
		this.idleTimer = time;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0003B3F0 File Offset: 0x000395F0
	private void Update()
	{
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			if (this.physGrabObject.grabbedLocal)
			{
				ItemInfoUI.instance.ItemInfoText(null, this.enemyParent.enemyName, true);
			}
			this.onGrabbedPlayerAvatar = this.physGrabObject.playerGrabbing[0].playerAvatar;
			this.onGrabbedPosition = this.physGrabObject.playerGrabbing[0].physGrabPoint.position;
			this.onGrabbed.Invoke();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.enemyInteractTimer = 10f;
			if (this.touchingCartTimer > 0f)
			{
				this.touchingCartTimer -= Time.deltaTime;
			}
			this.positionSpeed = this.positionSpeedChase;
			this.rotationSpeed = this.rotationSpeedChase;
			this.distanceWarp = this.distanceWarpChase;
			this.positionSpeedLerpCurrent = this.positionSpeedLerpChase;
			if (this.idleTimer > 0f)
			{
				this.positionSpeed = this.positionSpeedIdle;
				this.rotationSpeed = this.rotationSpeedIdle;
				this.distanceWarp = this.distanceWarpIdle;
				this.positionSpeedLerpCurrent = this.positionSpeedLerpIdle;
				this.idleTimer -= Time.deltaTime;
			}
			if (this.overrideFollowPositionTimer > 0f)
			{
				this.positionSpeed = this.overrideFollowPositionSpeed;
				if (this.overrideFollowPositionLerp != -1f)
				{
					this.positionSpeedLerpCurrent = this.overrideFollowPositionLerp;
				}
				this.overrideFollowPositionTimer -= Time.deltaTime;
			}
			if (this.overrideFollowRotationTimer > 0f)
			{
				this.rotationSpeed = this.overrideFollowRotationSpeed;
				this.overrideFollowRotationTimer -= Time.deltaTime;
			}
			if (this.disableNoGravityTimer > 0f)
			{
				this.disableNoGravityTimer -= Time.deltaTime;
			}
			else if (!this.gravity)
			{
				this.physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
		if (!this.enemy.IsStunned())
		{
			this.impactDetector.ImpactDisable(0.25f);
		}
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x0003B5FC File Offset: 0x000397FC
	private void FixedUpdate()
	{
		if (!this.frozen)
		{
			this.velocity = this.physGrabObject.rbVelocity;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.spawned)
		{
			if (this.teleportedTimer > 0f)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				this.teleportedTimer -= Time.fixedDeltaTime;
				return;
			}
			if (this.hasPlayerCollision)
			{
				if (this.enemy.IsStunned())
				{
					if (this.playerCollisionActive)
					{
						this.playerCollisionActive = false;
						this.playerCollision.enabled = false;
					}
				}
				else if (!this.playerCollisionActive)
				{
					this.playerCollisionActive = true;
					this.playerCollision.enabled = true;
				}
			}
			if (this.enemy.FreezeTimer > 0f)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
				return;
			}
			if (this.frozen)
			{
				this.rb.AddForce(this.freezeVelocity, ForceMode.VelocityChange);
				this.rb.AddTorque(this.freezeAngularVelocity, ForceMode.VelocityChange);
				this.rb.AddForce(this.freezeForce, ForceMode.Impulse);
				this.rb.AddTorque(this.freezeTorque, ForceMode.Impulse);
				this.freezeForce = Vector3.zero;
				this.freezeTorque = Vector3.zero;
				this.frozen = false;
				return;
			}
			bool flag = false;
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.enemy.SetChaseTarget(this.physGrabObject.playerGrabbing[0].playerAvatar);
				if (this.physGrabObject.grabDisplacementCurrent.magnitude >= this.grabForceNeeded.amount || EnemyDirector.instance.debugEasyGrab)
				{
					this.grabShakeReleaseTimer = 0f;
					this.grabForceTimer += Time.fixedDeltaTime;
					if (this.grabForceTimer >= this.grabTimeNeeded)
					{
						flag = true;
						if (this.grabOverride)
						{
							this.grabStrengthTimer = this.grabStrengthTime;
						}
					}
				}
				else
				{
					this.grabShakeReleaseTimer += Time.fixedDeltaTime;
				}
				if (this.grabShakeReleaseTimer > 3f && this.enemy.StateStunned.stunTimer <= 0.25f && !this.grabbed)
				{
					this.GrabReleaseShake();
				}
				this.grabTimeCurrent += Time.fixedDeltaTime;
				if (!EnemyDirector.instance.debugNoGrabMaxTime && this.enemy.StateStunned.stunTimer <= 0.25f && this.grabTimeCurrent >= this.grabTimeMaxRandom * (float)this.physGrabObject.playerGrabbing.Count)
				{
					this.GrabReleaseShake();
				}
			}
			else
			{
				this.grabTimeCurrent = 0f;
				this.grabTimeMaxRandom = this.grabTimeMax * Random.Range(0.9f, 1.1f);
				this.grabForceTimer = 0f;
				this.grabShakeReleaseTimer = 0f;
			}
			if (this.grabStrengthTimer > 0f)
			{
				flag = true;
				if (this.grabStun && this.enemy.HasStateStunned)
				{
					this.enemy.StateStunned.Set(0.1f);
				}
				if (this.rb.velocity.magnitude < 2f)
				{
					this.grabStrengthTimer -= Time.fixedDeltaTime;
					if (this.grabStrengthTimer <= 0f)
					{
						this.GrabReleaseShake();
					}
				}
			}
			if (flag)
			{
				this.enemy.StuckCount = 0;
				if (this.enemy.HasJump)
				{
					this.enemy.Jump.jumpCooldown = 1f;
				}
			}
			if (this.grabbedPrevious != flag)
			{
				this.GrabbedSet(flag);
			}
			if (this.customGravity > 0f && this.gravity && this.disableNoGravityTimer <= 0f && this.rb.useGravity && this.physGrabObject.playerGrabbing.Count <= 0)
			{
				this.rb.AddForce(-Vector3.up * this.customGravity, ForceMode.Force);
			}
			if (this.grabbed)
			{
				if (this.materialState != 0)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialGrabbed;
					}
					this.materialState = 0;
				}
			}
			else if (this.enemy.IsStunned() || this.colliderMaterialStunnedOverrideTimer > 0f)
			{
				if (this.materialState != 1)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialStunned;
					}
					this.materialState = 1;
				}
			}
			else if (this.disableFollowPositionTimer > 0f || this.disableFollowRotationTimer > 0f)
			{
				if (this.materialState != 2)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialDisabled;
					}
					this.materialState = 2;
				}
			}
			else if (this.enemy.HasJump && this.enemy.Jump.jumping)
			{
				if (this.materialState != 3)
				{
					Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material = this.ColliderMaterialJumping;
					}
					this.materialState = 3;
				}
			}
			else if (this.materialState != 4)
			{
				Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = this.ColliderMaterialDefault;
				}
				this.materialState = 4;
			}
			if (this.colliderMaterialStunnedOverrideTimer > 0f)
			{
				this.colliderMaterialStunnedOverrideTimer -= Time.fixedDeltaTime;
			}
			if (this.disableFollowRotationTimer <= 0f && !this.enemy.IsStunned())
			{
				this.rotationSpeedLerp += this.disableFollowRotationResetSpeed * Time.fixedDeltaTime;
				this.rotationSpeedLerp = Mathf.Clamp01(this.rotationSpeedLerp);
				this.rotationSpeedCurrent = Mathf.Lerp(0f, this.rotationSpeed, this.speedResetCurve.Evaluate(this.rotationSpeedLerp));
				Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.followTarget.rotation, this.rb, this.rotationSpeedCurrent);
				if (this.grabStrengthTimer > 0f)
				{
					vector = Vector3.Lerp(Vector3.zero, vector, this.grabRotationStrength);
				}
				this.rb.AddTorque(vector, ForceMode.Impulse);
			}
			else
			{
				this.rotationSpeedLerp = 0f;
				this.disableFollowRotationTimer -= Time.fixedDeltaTime;
			}
			if (this.disableFollowPositionTimer <= 0f && !this.enemy.IsStunned())
			{
				this.timeSinceStun += Time.fixedDeltaTime;
				this.positionSpeedLerp += this.disableFollowPositionResetSpeed * Time.fixedDeltaTime;
				this.positionSpeedLerp = Mathf.Clamp01(this.positionSpeedLerp);
				this.positionSpeedCurrent = Mathf.Lerp(0f, this.positionSpeed, this.speedResetCurve.Evaluate(this.positionSpeedLerp));
				Vector3 vector2 = SemiFunc.PhysFollowPosition(this.rb.transform.position, this.followTarget.position, this.rb.velocity, this.positionSpeedCurrent);
				if (this.grabStrengthTimer > 0f)
				{
					vector2 = Vector3.Lerp(Vector3.zero, vector2, this.grabPositionStrength);
				}
				if ((this.gravity || this.disableNoGravityTimer > 0f) && this.physGrabObject.playerGrabbing.Count <= 0)
				{
					vector2.y = 0f;
				}
				vector2 = Vector3.Lerp(this.positionForce, vector2, this.positionSpeedLerpCurrent * Time.fixedDeltaTime);
				this.rb.AddForce(vector2, ForceMode.Impulse);
			}
			else
			{
				this.timeSinceStun = 0f;
				this.positionSpeedLerp = 0f;
				this.disableFollowPositionTimer -= Time.fixedDeltaTime;
			}
			if (!this.grabbed && Vector3.Distance(this.lastMovingPosition, base.transform.position) < this.notMovingDistance)
			{
				this.notMovingTimer += Time.fixedDeltaTime;
			}
			else
			{
				this.lastMovingPosition = base.transform.position;
				this.notMovingTimer = 0f;
			}
			if (this.enemy.HasNavMeshAgent && !this.grabbed)
			{
				float num = Vector3.Distance(new Vector3(this.followTarget.position.x, 0f, this.followTarget.position.z), new Vector3(this.rb.position.x, 0f, this.rb.position.z));
				bool flag2 = false;
				if (this.enemy.HasJump && this.enemy.Jump.jumping)
				{
					flag2 = true;
				}
				if (this.warpDisableTimer <= 0f && num >= this.distanceWarp && !flag2)
				{
					if (this.enemy.NavMeshAgent.IsDisabled() || this.enemy.NavMeshAgent.IsStopped())
					{
						this.enemy.transform.position = this.rb.position;
						this.timeSinceLastWarp = 0f;
						if (LevelGenerator.Instance.Generated && (!this.enemy.HasAttackPhysObject || !this.enemy.AttackStuckPhysObject.Active) && this.notMovingTimer >= 1f)
						{
							this.enemy.StuckCount++;
						}
					}
					else if (this.enemy.NavMeshAgent.Agent.velocity.magnitude > 0.1f || num >= this.distanceWarp * 2f)
					{
						RaycastHit raycastHit;
						if (Physics.Raycast(this.rb.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, 10f, LayerMask.GetMask(new string[]
						{
							"Default",
							"NavmeshOnly",
							"PlayerOnlyCollision"
						})))
						{
							this.enemy.NavMeshAgent.AgentMove(raycastHit.point);
						}
						else
						{
							this.enemy.NavMeshAgent.AgentMove(this.rb.position);
						}
						this.timeSinceLastWarp = 0f;
						if (LevelGenerator.Instance.Generated && (!this.enemy.HasAttackPhysObject || !this.enemy.AttackStuckPhysObject.Active) && this.notMovingTimer >= 1f)
						{
							this.enemy.StuckCount++;
						}
					}
				}
				else if (!this.enemy.NavMeshAgent.IsDisabled() && !this.enemy.NavMeshAgent.IsStopped())
				{
					this.timeSinceLastWarp += Time.fixedDeltaTime;
					if (this.timeSinceLastWarp >= 3f)
					{
						this.enemy.StuckCount = 0;
					}
				}
			}
			if (this.warpDisableTimer > 0f)
			{
				this.warpDisableTimer -= Time.fixedDeltaTime;
			}
			if (this.stunFromFall && (!this.enemy.HasJump || !this.enemy.Jump.jumping) && !this.grabbed && this.gravity && this.disableNoGravityTimer <= 0f && this.rb.useGravity && (!this.enemy.HasGrounded || !this.enemy.Grounded.grounded))
			{
				if (this.rb.velocity.y < -2f)
				{
					if (this.stunFromFallTimer >= this.stunFromFallTime && this.enemy.HasStateStunned)
					{
						if (!this.enemy.IsStunned())
						{
							this.rb.AddTorque(-base.transform.right * (this.rb.mass * 0.5f), ForceMode.Impulse);
						}
						this.enemy.StateStunned.Set(3f);
					}
					this.stunFromFallTimer += Time.fixedDeltaTime;
					return;
				}
				this.stunFromFallTimer = 0f;
				return;
			}
			else
			{
				this.stunFromFallTimer = 0f;
			}
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0003C258 File Offset: 0x0003A458
	private void OnCollisionStay(Collision other)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				return;
			}
			if (other.gameObject.CompareTag("Phys Grab Object"))
			{
				PhysGrabObject physGrabObject = other.gameObject.GetComponent<PhysGrabObject>();
				if (!physGrabObject)
				{
					physGrabObject = other.gameObject.GetComponentInParent<PhysGrabObject>();
				}
				if (physGrabObject)
				{
					this.onTouchPhysObjectPhysObject = physGrabObject;
					this.onTouchPhysObjectPosition = other.GetContact(0).point;
					this.onTouchPhysObject.Invoke();
					physGrabObject.EnemyInteractTimeSet();
					PhysGrabCart component = physGrabObject.GetComponent<PhysGrabCart>();
					if (component)
					{
						this.touchingCartTimer = 0.25f;
						foreach (PhysGrabInCart.CartObject cartObject in component.physGrabInCart.inCartObjects.ToList<PhysGrabInCart.CartObject>())
						{
							cartObject.physGrabObject.EnemyInteractTimeSet();
						}
					}
					if (this.enemy.CheckChase())
					{
						if (this.enemy.FreezeTimer <= 0f)
						{
							Vector3 normalized = (physGrabObject.centerPoint - this.physGrabObject.centerPoint).normalized;
							if (Vector3.Dot((this.followTarget.position - this.physGrabObject.centerPoint).normalized, normalized) > 0f)
							{
								Vector3 force = normalized * 10f;
								force.y = 5f;
								physGrabObject.rb.AddForce(force, ForceMode.Impulse);
								physGrabObject.rb.AddTorque(Random.insideUnitSphere * force.magnitude, ForceMode.Impulse);
								physGrabObject.lightBreakImpulse = true;
								PhysGrabHinge component2 = physGrabObject.GetComponent<PhysGrabHinge>();
								if (component2 && component2.brokenTimer >= 1.5f)
								{
									component2.DestroyHinge();
								}
								GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
								GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 15f, base.transform.position, 0.1f);
								this.rb.AddForce(-normalized * 2f, ForceMode.Impulse);
								this.DisableFollowPosition(0.1f, 5f);
								return;
							}
						}
					}
					else
					{
						PlayerTumble component3 = physGrabObject.GetComponent<PlayerTumble>();
						if (component3)
						{
							this.onTouchPlayerAvatar = component3.playerAvatar;
							this.onTouchPlayer.Invoke();
							this.enemy.SetChaseTarget(component3.playerAvatar);
							return;
						}
						if (physGrabObject.playerGrabbing.Count > 0)
						{
							PlayerAvatar playerAvatar = physGrabObject.playerGrabbing[0].playerAvatar;
							this.onTouchPlayerGrabbedObjectAvatar = playerAvatar;
							this.onTouchPlayerGrabbedObjectPhysObject = physGrabObject;
							this.onTouchPlayerGrabbedObjectPosition = other.GetContact(0).point;
							this.onTouchPlayerGrabbedObject.Invoke();
							this.enemy.SetChaseTarget(playerAvatar);
							return;
						}
					}
				}
			}
			else if (other.gameObject.CompareTag("Player"))
			{
				PlayerController componentInParent = other.gameObject.GetComponentInParent<PlayerController>();
				if (componentInParent)
				{
					this.onTouchPlayerAvatar = componentInParent.playerAvatarScript;
					this.onTouchPlayer.Invoke();
					this.enemy.SetChaseTarget(componentInParent.playerAvatarScript);
					return;
				}
				PlayerAvatar componentInParent2 = other.gameObject.GetComponentInParent<PlayerAvatar>();
				if (componentInParent2)
				{
					this.onTouchPlayerAvatar = componentInParent2;
					this.onTouchPlayer.Invoke();
					this.enemy.SetChaseTarget(componentInParent2);
				}
			}
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0003C5F8 File Offset: 0x0003A7F8
	public void DisableFollowPosition(float time, float resetSpeed)
	{
		this.disableFollowPositionTimer = time;
		this.disableFollowPositionResetSpeed = resetSpeed;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x0003C608 File Offset: 0x0003A808
	public void DisableFollowRotation(float time, float resetSpeed)
	{
		this.disableFollowRotationTimer = time;
		this.disableFollowRotationResetSpeed = resetSpeed;
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x0003C618 File Offset: 0x0003A818
	public void DisableNoGravity(float time)
	{
		this.disableNoGravityTimer = time;
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x0003C621 File Offset: 0x0003A821
	public void OverrideFollowPosition(float time, float speed, float lerp = -1f)
	{
		this.overrideFollowPositionTimer = time;
		this.overrideFollowPositionSpeed = speed;
		this.overrideFollowPositionLerp = lerp;
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x0003C638 File Offset: 0x0003A838
	public void OverrideFollowRotation(float time, float speed)
	{
		this.overrideFollowRotationTimer = time;
		this.overrideFollowRotationSpeed = speed;
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0003C648 File Offset: 0x0003A848
	public void Teleport()
	{
		this.physGrabObject.Teleport(this.followTarget.position + new Vector3(0f, this.yOffset, 0f), this.followTarget.rotation);
		if (!this.rb.isKinematic)
		{
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		this.freezeForce = Vector3.zero;
		this.freezeTorque = Vector3.zero;
		this.frozen = false;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0003C6DC File Offset: 0x0003A8DC
	public void FreezeForces(Vector3 force, Vector3 torque)
	{
		if (!this.frozen)
		{
			this.freezeVelocity = this.rb.velocity;
			this.freezeAngularVelocity = this.rb.angularVelocity;
			this.frozen = true;
		}
		this.freezeForce += force;
		this.freezeTorque += torque;
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0003C760 File Offset: 0x0003A960
	public void JumpImpulse()
	{
		if (this.materialState != 3)
		{
			Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = this.ColliderMaterialJumping;
			}
			this.materialState = 3;
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0003C7A0 File Offset: 0x0003A9A0
	public void StuckReset()
	{
		this.notMovingTimer = 0f;
		this.enemy.StuckCount = 0;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0003C7B9 File Offset: 0x0003A9B9
	public void WarpDisable(float time)
	{
		this.warpDisableTimer = time;
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x0003C7C2 File Offset: 0x0003A9C2
	public void OverrideColliderMaterialStunned(float _time)
	{
		this.colliderMaterialStunnedOverrideTimer = _time;
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0003C7CC File Offset: 0x0003A9CC
	public void LightImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.LightImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeLight, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeLight, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactLight.Invoke();
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0003C860 File Offset: 0x0003AA60
	public void MediumImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.MediumImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeMedium, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeMedium, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactMedium.Invoke();
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x0003C8F4 File Offset: 0x0003AAF4
	public void HeavyImpact()
	{
		if (this.enemy.HasHealth)
		{
			this.enemy.Health.HeavyImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(this.impactShakeHeavy, 5f, 15f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(this.impactShakeHeavy, 5f, 15f, base.transform.position, 0.1f);
		this.onImpactHeavy.Invoke();
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0003C988 File Offset: 0x0003AB88
	public void GrabRelease()
	{
		bool flag = false;
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
			flag = true;
		}
		if (flag)
		{
			if (GameManager.instance.gameMode == 0)
			{
				this.GrabReleaseRPC();
				return;
			}
			this.photonView.RPC("GrabReleaseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0003CA50 File Offset: 0x0003AC50
	[PunRPC]
	private void GrabReleaseRPC()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.physGrabObject.grabDisableTimer = 1f;
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0003CACC File Offset: 0x0003ACCC
	private void GrabReleaseShake()
	{
		this.grabStrengthTimer = 0f;
		this.GrabbedSet(false);
		float d = 1f * this.rb.mass;
		this.rb.AddRelativeTorque(Vector3.up * d, ForceMode.Impulse);
		this.GrabRelease();
		this.DisableFollowRotation(0.5f, 50f);
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0003CB2C File Offset: 0x0003AD2C
	private void GrabbedSet(bool _grabbed)
	{
		this.grabbed = _grabbed;
		this.grabbedPrevious = _grabbed;
		if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GrabbedSetRPC", RpcTarget.All, new object[]
			{
				this.grabbed
			});
		}
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0003CB7A File Offset: 0x0003AD7A
	[PunRPC]
	private void GrabbedSetRPC(bool _grabbed)
	{
		this.grabbed = _grabbed;
	}

	// Token: 0x04000A23 RID: 2595
	public Enemy enemy;

	// Token: 0x04000A24 RID: 2596
	public Transform followTarget;

	// Token: 0x04000A25 RID: 2597
	internal PhysGrabObject physGrabObject;

	// Token: 0x04000A26 RID: 2598
	internal PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04000A27 RID: 2599
	internal PhotonView photonView;

	// Token: 0x04000A28 RID: 2600
	internal Rigidbody rb;

	// Token: 0x04000A29 RID: 2601
	internal Vector3 velocity;

	// Token: 0x04000A2A RID: 2602
	[Space]
	public bool gravity = true;

	// Token: 0x04000A2B RID: 2603
	public float customGravity;

	// Token: 0x04000A2C RID: 2604
	[Space]
	public GrabForce grabForceNeeded;

	// Token: 0x04000A2D RID: 2605
	public float grabTimeNeeded = 0.5f;

	// Token: 0x04000A2E RID: 2606
	private float grabForceTimer;

	// Token: 0x04000A2F RID: 2607
	private float grabShakeReleaseTimer;

	// Token: 0x04000A30 RID: 2608
	public bool grabStun;

	// Token: 0x04000A31 RID: 2609
	public bool grabOverride;

	// Token: 0x04000A32 RID: 2610
	public float grabPositionStrength;

	// Token: 0x04000A33 RID: 2611
	public float grabRotationStrength;

	// Token: 0x04000A34 RID: 2612
	public float grabStrengthTime = 1f;

	// Token: 0x04000A35 RID: 2613
	internal float grabStrengthTimer;

	// Token: 0x04000A36 RID: 2614
	public float grabTimeMax = 3f;

	// Token: 0x04000A37 RID: 2615
	private float grabTimeMaxRandom;

	// Token: 0x04000A38 RID: 2616
	private float grabTimeCurrent;

	// Token: 0x04000A39 RID: 2617
	internal bool grabbed;

	// Token: 0x04000A3A RID: 2618
	private bool grabbedPrevious;

	// Token: 0x04000A3B RID: 2619
	[Space]
	public float positionSpeedIdle = 1f;

	// Token: 0x04000A3C RID: 2620
	public float positionSpeedLerpIdle = 10f;

	// Token: 0x04000A3D RID: 2621
	public float positionSpeedChase = 2f;

	// Token: 0x04000A3E RID: 2622
	public float positionSpeedLerpChase = 50f;

	// Token: 0x04000A3F RID: 2623
	private float positionSpeed;

	// Token: 0x04000A40 RID: 2624
	private float positionSpeedCurrent;

	// Token: 0x04000A41 RID: 2625
	internal float positionSpeedLerp = 1f;

	// Token: 0x04000A42 RID: 2626
	private float positionSpeedLerpCurrent = 1f;

	// Token: 0x04000A43 RID: 2627
	private Vector3 positionForce;

	// Token: 0x04000A44 RID: 2628
	[Space]
	public float rotationSpeedIdle = 1f;

	// Token: 0x04000A45 RID: 2629
	public float rotationSpeedChase = 2f;

	// Token: 0x04000A46 RID: 2630
	private float rotationSpeed;

	// Token: 0x04000A47 RID: 2631
	private float rotationSpeedCurrent;

	// Token: 0x04000A48 RID: 2632
	private float rotationSpeedLerp = 1f;

	// Token: 0x04000A49 RID: 2633
	[Space]
	public float distanceWarpIdle = 1f;

	// Token: 0x04000A4A RID: 2634
	public float distanceWarpChase = 2f;

	// Token: 0x04000A4B RID: 2635
	private float distanceWarp;

	// Token: 0x04000A4C RID: 2636
	private float timeSinceLastWarp;

	// Token: 0x04000A4D RID: 2637
	[Space]
	public float notMovingDistance = 1f;

	// Token: 0x04000A4E RID: 2638
	internal float notMovingTimer;

	// Token: 0x04000A4F RID: 2639
	private Vector3 lastMovingPosition;

	// Token: 0x04000A50 RID: 2640
	[Space]
	public bool stunFromFall = true;

	// Token: 0x04000A51 RID: 2641
	private float stunFromFallTime = 1f;

	// Token: 0x04000A52 RID: 2642
	private float stunFromFallTimer;

	// Token: 0x04000A53 RID: 2643
	[Space]
	public AnimationCurve speedResetCurve;

	// Token: 0x04000A54 RID: 2644
	public float stunResetSpeed = 10f;

	// Token: 0x04000A55 RID: 2645
	internal float disableFollowPositionTimer;

	// Token: 0x04000A56 RID: 2646
	internal float disableFollowPositionResetSpeed;

	// Token: 0x04000A57 RID: 2647
	internal float disableFollowRotationTimer;

	// Token: 0x04000A58 RID: 2648
	internal float disableFollowRotationResetSpeed;

	// Token: 0x04000A59 RID: 2649
	internal float disableNoGravityTimer;

	// Token: 0x04000A5A RID: 2650
	internal float overrideFollowPositionTimer;

	// Token: 0x04000A5B RID: 2651
	internal float overrideFollowPositionSpeed;

	// Token: 0x04000A5C RID: 2652
	internal float overrideFollowPositionLerp;

	// Token: 0x04000A5D RID: 2653
	internal float overrideFollowRotationTimer;

	// Token: 0x04000A5E RID: 2654
	internal float overrideFollowRotationSpeed;

	// Token: 0x04000A5F RID: 2655
	private float idleTimer;

	// Token: 0x04000A60 RID: 2656
	internal float timeSinceStun;

	// Token: 0x04000A61 RID: 2657
	[Space]
	public PhysicMaterial ColliderMaterialDefault;

	// Token: 0x04000A62 RID: 2658
	public PhysicMaterial ColliderMaterialDisabled;

	// Token: 0x04000A63 RID: 2659
	public PhysicMaterial ColliderMaterialStunned;

	// Token: 0x04000A64 RID: 2660
	public PhysicMaterial ColliderMaterialGrabbed;

	// Token: 0x04000A65 RID: 2661
	public PhysicMaterial ColliderMaterialJumping;

	// Token: 0x04000A66 RID: 2662
	private float colliderMaterialStunnedOverrideTimer;

	// Token: 0x04000A67 RID: 2663
	[Space]
	public Collider playerCollision;

	// Token: 0x04000A68 RID: 2664
	private bool hasPlayerCollision;

	// Token: 0x04000A69 RID: 2665
	private bool playerCollisionActive;

	// Token: 0x04000A6A RID: 2666
	private int materialState = -1;

	// Token: 0x04000A6B RID: 2667
	internal float teleportedTimer;

	// Token: 0x04000A6C RID: 2668
	internal float touchingCartTimer;

	// Token: 0x04000A6D RID: 2669
	internal bool frozen;

	// Token: 0x04000A6E RID: 2670
	private Vector3 freezeVelocity;

	// Token: 0x04000A6F RID: 2671
	private Vector3 freezeAngularVelocity;

	// Token: 0x04000A70 RID: 2672
	private Vector3 freezeForce;

	// Token: 0x04000A71 RID: 2673
	private Vector3 freezeTorque;

	// Token: 0x04000A72 RID: 2674
	internal float yOffset;

	// Token: 0x04000A73 RID: 2675
	[Space]
	public float impactShakeLight = 1f;

	// Token: 0x04000A74 RID: 2676
	public float impactShakeMedium = 2f;

	// Token: 0x04000A75 RID: 2677
	public float impactShakeHeavy = 4f;

	// Token: 0x04000A76 RID: 2678
	public float impactFragility = 1f;

	// Token: 0x04000A77 RID: 2679
	public UnityEvent onImpactLight;

	// Token: 0x04000A78 RID: 2680
	public UnityEvent onImpactMedium;

	// Token: 0x04000A79 RID: 2681
	public UnityEvent onImpactHeavy;

	// Token: 0x04000A7A RID: 2682
	public UnityEvent onTouchPlayer;

	// Token: 0x04000A7B RID: 2683
	internal PlayerAvatar onTouchPlayerAvatar;

	// Token: 0x04000A7C RID: 2684
	public UnityEvent onTouchPlayerGrabbedObject;

	// Token: 0x04000A7D RID: 2685
	internal PlayerAvatar onTouchPlayerGrabbedObjectAvatar;

	// Token: 0x04000A7E RID: 2686
	internal PhysGrabObject onTouchPlayerGrabbedObjectPhysObject;

	// Token: 0x04000A7F RID: 2687
	internal Vector3 onTouchPlayerGrabbedObjectPosition;

	// Token: 0x04000A80 RID: 2688
	public UnityEvent onTouchPhysObject;

	// Token: 0x04000A81 RID: 2689
	internal PhysGrabObject onTouchPhysObjectPhysObject;

	// Token: 0x04000A82 RID: 2690
	internal Vector3 onTouchPhysObjectPosition;

	// Token: 0x04000A83 RID: 2691
	public UnityEvent onGrabbed;

	// Token: 0x04000A84 RID: 2692
	internal PlayerAvatar onGrabbedPlayerAvatar;

	// Token: 0x04000A85 RID: 2693
	internal Vector3 onGrabbedPosition;

	// Token: 0x04000A86 RID: 2694
	private float warpDisableTimer;

	// Token: 0x04000A87 RID: 2695
	private EnemyParent enemyParent;
}
