using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020000DB RID: 219
public class HurtCollider : MonoBehaviour
{
	// Token: 0x060007CD RID: 1997 RVA: 0x0004BE78 File Offset: 0x0004A078
	private void Awake()
	{
		this.BoxCollider = base.GetComponent<BoxCollider>();
		if (!this.BoxCollider)
		{
			this.SphereCollider = base.GetComponent<SphereCollider>();
			this.Collider = this.SphereCollider;
			this.ColliderIsBox = false;
		}
		else
		{
			this.Collider = this.BoxCollider;
		}
		this.Collider.isTrigger = true;
		this.LayerMask = SemiFunc.LayerMaskGetPhysGrabObject() + LayerMask.GetMask(new string[]
		{
			"Player"
		}) + LayerMask.GetMask(new string[]
		{
			"Default"
		}) + LayerMask.GetMask(new string[]
		{
			"Enemy"
		});
		this.RayMask = LayerMask.GetMask(new string[]
		{
			"Default",
			"PhysGrabObjectHinge"
		});
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0004BF4E File Offset: 0x0004A14E
	private void OnEnable()
	{
		if (!this.colliderCheckRunning)
		{
			this.colliderCheckRunning = true;
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0004BF6C File Offset: 0x0004A16C
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.colliderCheckRunning = false;
		this.cooldownLogicRunning = false;
		this.hits.Clear();
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0004BF8D File Offset: 0x0004A18D
	private IEnumerator CooldownLogic()
	{
		while (this.hits.Count > 0)
		{
			for (int i = 0; i < this.hits.Count; i++)
			{
				HurtCollider.Hit hit = this.hits[i];
				hit.cooldown -= Time.deltaTime;
				if (hit.cooldown <= 0f)
				{
					this.hits.RemoveAt(i);
					i--;
				}
			}
			yield return null;
		}
		this.cooldownLogicRunning = false;
		yield break;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0004BF9C File Offset: 0x0004A19C
	private bool CanHit(GameObject hitObject, float cooldown, bool raycast, Vector3 hitPosition, HurtCollider.HitType hitType)
	{
		using (List<HurtCollider.Hit>.Enumerator enumerator = this.hits.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.hitObject == hitObject)
				{
					return false;
				}
			}
		}
		HurtCollider.Hit hit = new HurtCollider.Hit();
		hit.hitObject = hitObject;
		hit.cooldown = cooldown;
		hit.hitType = hitType;
		this.hits.Add(hit);
		if (!this.cooldownLogicRunning)
		{
			base.StartCoroutine(this.CooldownLogic());
			this.cooldownLogicRunning = true;
		}
		if (raycast)
		{
			Vector3 normalized = (hitPosition - this.Collider.bounds.center).normalized;
			float maxDistance = Vector3.Distance(hitPosition, this.Collider.bounds.center);
			foreach (RaycastHit raycastHit in Physics.RaycastAll(this.Collider.bounds.center, normalized, maxDistance, this.RayMask, QueryTriggerInteraction.Collide))
			{
				if (raycastHit.collider.gameObject.CompareTag("Wall"))
				{
					PhysGrabObject componentInParent = hitObject.GetComponentInParent<PhysGrabObject>();
					PhysGrabObject componentInParent2 = raycastHit.collider.gameObject.GetComponentInParent<PhysGrabObject>();
					if (!componentInParent || componentInParent != componentInParent2)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0004C11C File Offset: 0x0004A31C
	private IEnumerator ColliderCheck()
	{
		yield return null;
		while (!LevelGenerator.Instance || !LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		for (;;)
		{
			Vector3 center = this.Collider.bounds.center;
			Collider[] array;
			if (this.ColliderIsBox)
			{
				Vector3 halfExtents = this.BoxCollider.size * 0.5f;
				halfExtents.x *= Mathf.Abs(base.transform.lossyScale.x);
				halfExtents.y *= Mathf.Abs(base.transform.lossyScale.y);
				halfExtents.z *= Mathf.Abs(base.transform.lossyScale.z);
				array = Physics.OverlapBox(center, halfExtents, base.transform.rotation, this.LayerMask, QueryTriggerInteraction.Collide);
			}
			else
			{
				float radius = base.transform.lossyScale.x * this.SphereCollider.radius;
				array = Physics.OverlapSphere(center, radius, this.LayerMask, QueryTriggerInteraction.Collide);
			}
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					if (this.playerLogic && this.playerDamageCooldown > 0f && collider.gameObject.CompareTag("Player"))
					{
						PlayerAvatar playerAvatar = collider.gameObject.GetComponentInParent<PlayerAvatar>();
						if (!playerAvatar)
						{
							PlayerController componentInParent = collider.gameObject.GetComponentInParent<PlayerController>();
							if (componentInParent)
							{
								playerAvatar = componentInParent.playerAvatarScript;
							}
						}
						if (playerAvatar)
						{
							this.PlayerHurt(playerAvatar);
						}
					}
					if (this.enemyDamageCooldown > 0f || this.physDamageCooldown > 0f || this.playerDamageCooldown > 0f)
					{
						if (collider.gameObject.CompareTag("Phys Grab Object"))
						{
							PhysGrabObject componentInParent2 = collider.gameObject.GetComponentInParent<PhysGrabObject>();
							if (!this.ignoreObjects.Contains(componentInParent2) && componentInParent2)
							{
								bool flag = false;
								PlayerTumble componentInParent3 = collider.gameObject.GetComponentInParent<PlayerTumble>();
								if (componentInParent3)
								{
									flag = true;
								}
								if (this.playerLogic && this.playerDamageCooldown > 0f && flag)
								{
									this.PlayerHurt(componentInParent3.playerAvatar);
								}
								if (SemiFunc.IsMasterClientOrSingleplayer())
								{
									EnemyRigidbody enemyRigidbody = null;
									if (this.enemyLogic && !flag)
									{
										enemyRigidbody = collider.gameObject.GetComponentInParent<EnemyRigidbody>();
										this.EnemyHurtRigidbody(enemyRigidbody, componentInParent2);
									}
									if (this.physLogic && !enemyRigidbody && !flag && this.physDamageCooldown > 0f && this.CanHit(componentInParent2.gameObject, this.physDamageCooldown, this.physRayCast, componentInParent2.centerPoint, HurtCollider.HitType.PhysObject))
									{
										bool flag2 = false;
										PhysGrabObjectImpactDetector componentInParent4 = collider.gameObject.GetComponentInParent<PhysGrabObjectImpactDetector>();
										if (componentInParent4)
										{
											if (this.physHingeDestroy)
											{
												PhysGrabHinge component = componentInParent2.GetComponent<PhysGrabHinge>();
												if (component)
												{
													component.DestroyHinge();
													flag2 = true;
												}
											}
											else if (this.physHingeBreak)
											{
												PhysGrabHinge component2 = componentInParent2.GetComponent<PhysGrabHinge>();
												if (component2 && component2.joint)
												{
													component2.joint.breakForce = 0f;
													component2.joint.breakTorque = 0f;
													flag2 = true;
												}
											}
											if (!flag2)
											{
												if (this.physDestroy)
												{
													if (!componentInParent4.destroyDisable)
													{
														PhysGrabHinge component3 = componentInParent2.GetComponent<PhysGrabHinge>();
														if (component3)
														{
															component3.DestroyHinge();
														}
														else
														{
															componentInParent4.DestroyObject(true);
														}
													}
													else
													{
														this.PhysObjectHurt(componentInParent2, HurtCollider.BreakImpact.Heavy, 50f, 30f, true, true);
													}
													flag2 = true;
												}
												else if (componentInParent2 && this.PhysObjectHurt(componentInParent2, this.physImpact, this.physHitForce, this.physHitTorque, true, false))
												{
													flag2 = true;
												}
											}
										}
										if (flag2)
										{
											this.onImpactAny.Invoke();
											this.onImpactPhysObject.Invoke();
										}
									}
								}
							}
						}
						else if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemyLogic)
						{
							Enemy componentInParent5 = collider.gameObject.GetComponentInParent<Enemy>();
							if (componentInParent5 && !componentInParent5.HasRigidbody && this.CanHit(componentInParent5.gameObject, this.enemyDamageCooldown, this.enemyRayCast, componentInParent5.transform.position, HurtCollider.HitType.Enemy) && this.EnemyHurt(componentInParent5))
							{
								this.onImpactAny.Invoke();
								this.onImpactEnemyEnemy = componentInParent5;
								this.onImpactEnemy.Invoke();
							}
							if (this.enemyHitTriggers)
							{
								EnemyParent componentInParent6 = collider.gameObject.GetComponentInParent<EnemyParent>();
								if (componentInParent6)
								{
									EnemyRigidbody componentInChildren = componentInParent6.GetComponentInChildren<EnemyRigidbody>();
									if (componentInChildren)
									{
										this.EnemyHurtRigidbody(componentInChildren, componentInChildren.physGrabObject);
									}
								}
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		yield break;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0004C12C File Offset: 0x0004A32C
	private void EnemyHurtRigidbody(EnemyRigidbody _enemyRigidbody, PhysGrabObject _physGrabObject)
	{
		if (this.enemyDamageCooldown > 0f && _enemyRigidbody && this.CanHit(_physGrabObject.gameObject, this.enemyDamageCooldown, this.enemyRayCast, _physGrabObject.centerPoint, HurtCollider.HitType.Enemy) && this.EnemyHurt(_enemyRigidbody.enemy))
		{
			this.onImpactAny.Invoke();
			this.onImpactEnemyEnemy = _enemyRigidbody.enemy;
			this.onImpactEnemy.Invoke();
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0004C1A0 File Offset: 0x0004A3A0
	private bool EnemyHurt(Enemy _enemy)
	{
		if (_enemy == this.enemyHost)
		{
			return false;
		}
		if (!this.enemyLogic)
		{
			return false;
		}
		bool flag = false;
		if (this.enemyKill)
		{
			if (_enemy.HasHealth)
			{
				_enemy.Health.Hurt(_enemy.Health.healthCurrent, base.transform.forward);
			}
			else if (_enemy.HasStateDespawn)
			{
				_enemy.EnemyParent.SpawnedTimerSet(0f);
				_enemy.CurrentState = EnemyState.Despawn;
				flag = true;
			}
		}
		if (!flag)
		{
			if (this.enemyStun && _enemy.HasStateStunned && _enemy.Type <= this.enemyStunType)
			{
				_enemy.StateStunned.Set(this.enemyStunTime);
			}
			if (this.enemyFreezeTime > 0f)
			{
				_enemy.Freeze(this.enemyFreezeTime);
			}
			if (_enemy.HasRigidbody)
			{
				this.PhysObjectHurt(_enemy.Rigidbody.physGrabObject, this.enemyImpact, this.enemyHitForce, this.enemyHitTorque, true, false);
				if (this.enemyFreezeTime > 0f)
				{
					_enemy.Rigidbody.FreezeForces(this.applyForce, this.applyTorque);
				}
			}
			if (this.enemyDamage > 0 && _enemy.HasHealth)
			{
				_enemy.Health.Hurt(this.enemyDamage, this.applyForce.normalized);
			}
		}
		return true;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0004C2EC File Offset: 0x0004A4EC
	private void PlayerHurt(PlayerAvatar _player)
	{
		if (GameManager.Multiplayer() && !_player.photonView.IsMine)
		{
			return;
		}
		int enemyIndex = SemiFunc.EnemyGetIndex(this.enemyHost);
		if (this.playerKill)
		{
			this.onImpactAny.Invoke();
			this.onImpactPlayer.Invoke();
			_player.playerHealth.Hurt(_player.playerHealth.health, true, enemyIndex);
			return;
		}
		if (this.CanHit(_player.gameObject, this.playerDamageCooldown, this.playerRayCast, _player.PlayerVisionTarget.VisionTransform.position, HurtCollider.HitType.Player))
		{
			_player.playerHealth.Hurt(this.playerDamage, true, enemyIndex);
			bool flag = false;
			Vector3 center = this.Collider.bounds.center;
			Vector3 vector = (_player.PlayerVisionTarget.VisionTransform.position - center).normalized;
			vector = SemiFunc.ClampDirection(vector, base.transform.forward, this.hitSpread);
			bool flag2 = _player.tumble.isTumbling;
			if (this.playerTumbleTime > 0f && _player.playerHealth.health > 0)
			{
				_player.tumble.TumbleRequest(true, false);
				_player.tumble.TumbleOverrideTime(this.playerTumbleTime);
				if (this.playerTumbleImpactHurtTime > 0f)
				{
					_player.tumble.ImpactHurtSet(this.playerTumbleImpactHurtTime, this.playerTumbleImpactHurtDamage);
				}
				flag2 = true;
				flag = true;
			}
			if (flag2 && (this.playerTumbleForce > 0f || this.playerTumbleTorque > 0f))
			{
				flag = true;
				if (this.playerTumbleForce > 0f)
				{
					_player.tumble.TumbleForce(vector * this.playerTumbleForce);
				}
				if (this.playerTumbleTorque > 0f)
				{
					Vector3 rhs = Vector3.zero;
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.up)
					{
						rhs = _player.transform.up;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.down)
					{
						rhs = -_player.transform.up;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.right)
					{
						rhs = _player.transform.right;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.left)
					{
						rhs = -_player.transform.right;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.forward)
					{
						rhs = _player.transform.forward;
					}
					if (this.playerTumbleTorqueAxis == HurtCollider.TorqueAxis.back)
					{
						rhs = -_player.transform.forward;
					}
					Vector3 torque = Vector3.Cross((_player.localCameraPosition - center).normalized, rhs) * this.playerTumbleTorque;
					_player.tumble.TumbleTorque(torque);
				}
			}
			if (!flag2 && this.playerHitForce > 0f)
			{
				PlayerController.instance.ForceImpulse(vector * this.playerHitForce);
			}
			if (this.playerHitForce > 0f || this.playerDamage > 0 || flag)
			{
				this.onImpactPlayerAvatar = _player;
				this.onImpactAny.Invoke();
				this.onImpactPlayer.Invoke();
			}
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0004C5DC File Offset: 0x0004A7DC
	private bool PhysObjectHurt(PhysGrabObject physGrabObject, HurtCollider.BreakImpact impact, float hitForce, float hitTorque, bool apply, bool destroyLaunch)
	{
		bool result = false;
		if (impact == HurtCollider.BreakImpact.Light)
		{
			physGrabObject.lightBreakImpulse = true;
			result = true;
		}
		else if (impact == HurtCollider.BreakImpact.Medium)
		{
			physGrabObject.mediumBreakImpulse = true;
			result = true;
		}
		else if (impact == HurtCollider.BreakImpact.Heavy)
		{
			physGrabObject.heavyBreakImpulse = true;
			result = true;
		}
		if (this.enemyHost && impact != HurtCollider.BreakImpact.None && physGrabObject.playerGrabbing.Count <= 0 && !physGrabObject.impactDetector.isEnemy)
		{
			physGrabObject.impactDetector.enemyInteractionTimer = 2f;
		}
		if (hitForce > 0f)
		{
			if (hitForce >= 5f && physGrabObject.playerGrabbing.Count > 0)
			{
				foreach (PhysGrabber physGrabber in physGrabObject.playerGrabbing.ToList<PhysGrabber>())
				{
					if (!SemiFunc.IsMultiplayer())
					{
						physGrabber.ReleaseObjectRPC(true, 2f);
					}
					else
					{
						physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
						{
							false,
							1f
						});
					}
				}
			}
			Vector3 center = this.Collider.bounds.center;
			Vector3 vector = (physGrabObject.centerPoint - center).normalized;
			vector = SemiFunc.ClampDirection(vector, base.transform.forward, this.hitSpread);
			this.applyForce = vector * hitForce;
			Vector3 normalized = (physGrabObject.centerPoint - center).normalized;
			Vector3 rhs = -physGrabObject.transform.up;
			this.applyTorque = Vector3.Cross(normalized, rhs) * hitTorque;
			if (apply)
			{
				if (destroyLaunch && !physGrabObject.rb.isKinematic)
				{
					physGrabObject.rb.velocity = Vector3.zero;
					physGrabObject.rb.angularVelocity = Vector3.zero;
					physGrabObject.impactDetector.destroyDisableLaunches++;
					physGrabObject.impactDetector.destroyDisableLaunchesTimer = 10f;
					Vector3 vector2 = Random.insideUnitSphere.normalized * 4f;
					if (physGrabObject.impactDetector.destroyDisableLaunches >= 3)
					{
						vector2 *= 20f;
						physGrabObject.impactDetector.destroyDisableLaunches = 0;
					}
					vector2.y = 0f;
					this.applyForce = (Vector3.up * 20f + vector2) * physGrabObject.rb.mass;
					this.applyTorque = Random.insideUnitSphere.normalized * 0.25f * physGrabObject.rb.mass;
				}
				physGrabObject.rb.AddForce(this.applyForce, ForceMode.Impulse);
				physGrabObject.rb.AddTorque(this.applyTorque, ForceMode.Impulse);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0004C8BC File Offset: 0x0004AABC
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		SphereCollider component2 = base.GetComponent<SphereCollider>();
		if (component2 && (base.transform.localScale.z != base.transform.localScale.x || base.transform.localScale.z != base.transform.localScale.y))
		{
			Debug.LogError("Sphere Collider must be uniform scale");
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 6f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		if (component)
		{
			Gizmos.DrawWireCube(component.center, component.size);
		}
		if (component2)
		{
			Gizmos.DrawWireSphere(component2.center, component2.radius);
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 0.2f);
		if (component)
		{
			Gizmos.DrawCube(component.center, component.size);
		}
		if (component2)
		{
			Gizmos.DrawSphere(component2.center, component2.radius);
		}
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 vector = Vector3.zero;
		if (component)
		{
			vector = component.bounds.center;
		}
		if (component2)
		{
			vector = component2.bounds.center;
		}
		Vector3 vector2 = vector + base.transform.forward * 0.5f;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, -base.transform.right, 0.5f) * 0.25f);
		Gizmos.DrawLine(vector2, vector2 + Vector3.LerpUnclamped(-base.transform.forward, base.transform.right, 0.5f) * 0.25f);
		if (this.hitSpread < 180f)
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
			Vector3 vector3 = (Quaternion.AngleAxis(this.hitSpread, base.transform.right) * base.transform.forward).normalized * 1.5f;
			Vector3 vector4 = (Quaternion.AngleAxis(-this.hitSpread, base.transform.right) * base.transform.forward).normalized * 1.5f;
			Vector3 vector5 = (Quaternion.AngleAxis(this.hitSpread, base.transform.up) * base.transform.forward).normalized * 1.5f;
			Vector3 vector6 = (Quaternion.AngleAxis(-this.hitSpread, base.transform.up) * base.transform.forward).normalized * 1.5f;
			Gizmos.DrawRay(vector, vector3);
			Gizmos.DrawRay(vector, vector4);
			Gizmos.DrawRay(vector, vector5);
			Gizmos.DrawRay(vector, vector6);
			Gizmos.DrawLineStrip(new Vector3[]
			{
				vector + vector3,
				vector + vector5,
				vector + vector4,
				vector + vector6
			}, true);
			return;
		}
		if (this.hitSpread > 180f)
		{
			Debug.LogError("Hit Spread cannot be greater than 180 degrees");
		}
	}

	// Token: 0x04000DF8 RID: 3576
	public bool playerLogic = true;

	// Token: 0x04000DF9 RID: 3577
	[Space]
	public bool playerKill = true;

	// Token: 0x04000DFA RID: 3578
	public int playerDamage = 10;

	// Token: 0x04000DFB RID: 3579
	public float playerDamageCooldown = 0.25f;

	// Token: 0x04000DFC RID: 3580
	public float playerHitForce;

	// Token: 0x04000DFD RID: 3581
	public bool playerRayCast;

	// Token: 0x04000DFE RID: 3582
	public float playerTumbleForce;

	// Token: 0x04000DFF RID: 3583
	public float playerTumbleTorque;

	// Token: 0x04000E00 RID: 3584
	public HurtCollider.TorqueAxis playerTumbleTorqueAxis = HurtCollider.TorqueAxis.down;

	// Token: 0x04000E01 RID: 3585
	public float playerTumbleTime;

	// Token: 0x04000E02 RID: 3586
	public float playerTumbleImpactHurtTime;

	// Token: 0x04000E03 RID: 3587
	public int playerTumbleImpactHurtDamage;

	// Token: 0x04000E04 RID: 3588
	public bool physLogic = true;

	// Token: 0x04000E05 RID: 3589
	[Space]
	public bool physDestroy = true;

	// Token: 0x04000E06 RID: 3590
	public bool physHingeDestroy = true;

	// Token: 0x04000E07 RID: 3591
	public bool physHingeBreak;

	// Token: 0x04000E08 RID: 3592
	public HurtCollider.BreakImpact physImpact = HurtCollider.BreakImpact.Medium;

	// Token: 0x04000E09 RID: 3593
	public float physDamageCooldown = 0.25f;

	// Token: 0x04000E0A RID: 3594
	public float physHitForce;

	// Token: 0x04000E0B RID: 3595
	public float physHitTorque;

	// Token: 0x04000E0C RID: 3596
	public bool physRayCast;

	// Token: 0x04000E0D RID: 3597
	public bool enemyLogic = true;

	// Token: 0x04000E0E RID: 3598
	public Enemy enemyHost;

	// Token: 0x04000E0F RID: 3599
	[Space]
	[FormerlySerializedAs("enemyDespawn")]
	public bool enemyKill = true;

	// Token: 0x04000E10 RID: 3600
	public bool enemyStun = true;

	// Token: 0x04000E11 RID: 3601
	public float enemyStunTime = 2f;

	// Token: 0x04000E12 RID: 3602
	public EnemyType enemyStunType = EnemyType.Medium;

	// Token: 0x04000E13 RID: 3603
	public float enemyFreezeTime = 0.1f;

	// Token: 0x04000E14 RID: 3604
	[Space]
	public HurtCollider.BreakImpact enemyImpact = HurtCollider.BreakImpact.Medium;

	// Token: 0x04000E15 RID: 3605
	public int enemyDamage;

	// Token: 0x04000E16 RID: 3606
	public float enemyDamageCooldown = 0.25f;

	// Token: 0x04000E17 RID: 3607
	public float enemyHitForce;

	// Token: 0x04000E18 RID: 3608
	public float enemyHitTorque;

	// Token: 0x04000E19 RID: 3609
	public bool enemyRayCast;

	// Token: 0x04000E1A RID: 3610
	public bool enemyHitTriggers = true;

	// Token: 0x04000E1B RID: 3611
	[Range(0f, 180f)]
	public float hitSpread = 180f;

	// Token: 0x04000E1C RID: 3612
	public List<PhysGrabObject> ignoreObjects = new List<PhysGrabObject>();

	// Token: 0x04000E1D RID: 3613
	public UnityEvent onImpactAny;

	// Token: 0x04000E1E RID: 3614
	public UnityEvent onImpactPlayer;

	// Token: 0x04000E1F RID: 3615
	internal PlayerAvatar onImpactPlayerAvatar;

	// Token: 0x04000E20 RID: 3616
	public UnityEvent onImpactPhysObject;

	// Token: 0x04000E21 RID: 3617
	public UnityEvent onImpactEnemy;

	// Token: 0x04000E22 RID: 3618
	internal Enemy onImpactEnemyEnemy;

	// Token: 0x04000E23 RID: 3619
	private Collider Collider;

	// Token: 0x04000E24 RID: 3620
	private BoxCollider BoxCollider;

	// Token: 0x04000E25 RID: 3621
	private SphereCollider SphereCollider;

	// Token: 0x04000E26 RID: 3622
	private bool ColliderIsBox = true;

	// Token: 0x04000E27 RID: 3623
	private LayerMask LayerMask;

	// Token: 0x04000E28 RID: 3624
	private LayerMask RayMask;

	// Token: 0x04000E29 RID: 3625
	internal List<HurtCollider.Hit> hits = new List<HurtCollider.Hit>();

	// Token: 0x04000E2A RID: 3626
	private bool colliderCheckRunning;

	// Token: 0x04000E2B RID: 3627
	private bool cooldownLogicRunning;

	// Token: 0x04000E2C RID: 3628
	private Vector3 applyForce;

	// Token: 0x04000E2D RID: 3629
	private Vector3 applyTorque;

	// Token: 0x02000301 RID: 769
	public enum BreakImpact
	{
		// Token: 0x04002576 RID: 9590
		None,
		// Token: 0x04002577 RID: 9591
		Light,
		// Token: 0x04002578 RID: 9592
		Medium,
		// Token: 0x04002579 RID: 9593
		Heavy
	}

	// Token: 0x02000302 RID: 770
	public enum TorqueAxis
	{
		// Token: 0x0400257B RID: 9595
		up,
		// Token: 0x0400257C RID: 9596
		down,
		// Token: 0x0400257D RID: 9597
		left,
		// Token: 0x0400257E RID: 9598
		right,
		// Token: 0x0400257F RID: 9599
		forward,
		// Token: 0x04002580 RID: 9600
		back
	}

	// Token: 0x02000303 RID: 771
	public enum HitType
	{
		// Token: 0x04002582 RID: 9602
		Player,
		// Token: 0x04002583 RID: 9603
		PhysObject,
		// Token: 0x04002584 RID: 9604
		Enemy
	}

	// Token: 0x02000304 RID: 772
	public class Hit
	{
		// Token: 0x04002585 RID: 9605
		public HurtCollider.HitType hitType;

		// Token: 0x04002586 RID: 9606
		public GameObject hitObject;

		// Token: 0x04002587 RID: 9607
		public float cooldown;
	}
}
