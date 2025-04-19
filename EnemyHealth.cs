using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009B RID: 155
public class EnemyHealth : MonoBehaviour
{
	// Token: 0x060005E5 RID: 1509 RVA: 0x00039B00 File Offset: 0x00037D00
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
		this.healthCurrent = this.health;
		this.hurtCurve = AssetManager.instance.animationCurveImpact;
		this.renderers = new List<MeshRenderer>();
		if (this.meshParent)
		{
			this.renderers.AddRange(this.meshParent.GetComponentsInChildren<MeshRenderer>(true));
		}
		foreach (MeshRenderer meshRenderer in this.renderers)
		{
			Material material = null;
			foreach (Material material2 in this.sharedMaterials)
			{
				if (meshRenderer.sharedMaterial.name == material2.name)
				{
					material = material2;
					meshRenderer.sharedMaterial = this.instancedMaterials[this.sharedMaterials.IndexOf(material2)];
				}
			}
			if (!material)
			{
				material = meshRenderer.sharedMaterial;
				this.sharedMaterials.Add(material);
				this.instancedMaterials.Add(meshRenderer.material);
			}
		}
		this.materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		this.materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		foreach (Material material3 in this.instancedMaterials)
		{
			material3.SetColor(this.materialHurtColor, Color.red);
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00039CC4 File Offset: 0x00037EC4
	private void Update()
	{
		if (this.hurtEffect)
		{
			this.hurtLerp += 2.5f * Time.deltaTime;
			this.hurtLerp = Mathf.Clamp01(this.hurtLerp);
			foreach (Material material in this.instancedMaterials)
			{
				material.SetFloat(this.materialHurtAmount, this.hurtCurve.Evaluate(this.hurtLerp));
			}
			if (this.hurtLerp > 1f)
			{
				this.hurtEffect = false;
				foreach (Material material2 in this.instancedMaterials)
				{
					material2.SetFloat(this.materialHurtAmount, 0f);
				}
			}
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && this.deadImpulse)
		{
			this.deadImpulseTimer -= Time.deltaTime;
			if (this.deadImpulseTimer <= 0f)
			{
				if (!GameManager.Multiplayer())
				{
					this.DeathImpulseRPC();
				}
				else
				{
					this.photonView.RPC("DeathImpulseRPC", RpcTarget.All, Array.Empty<object>());
				}
			}
		}
		if (this.objectHurtDisableTimer > 0f)
		{
			this.objectHurtDisableTimer -= Time.deltaTime;
		}
		if (this.onHurtImpulse)
		{
			this.onHurt.Invoke();
			this.onHurtImpulse = false;
		}
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00039E50 File Offset: 0x00038050
	public void OnSpawn()
	{
		if (this.hurtEffect)
		{
			this.hurtLerp = 1f;
			this.hurtEffect = false;
			foreach (Material material in this.instancedMaterials)
			{
				material.SetFloat(this.materialHurtAmount, 0f);
			}
		}
		this.healthCurrent = this.health;
		this.dead = false;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00039ED8 File Offset: 0x000380D8
	public void LightImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactLightDamage > 0)
			{
				this.Hurt(this.impactLightDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00039F30 File Offset: 0x00038130
	public void MediumImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactMediumDamage > 0)
			{
				this.Hurt(this.impactMediumDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00039F88 File Offset: 0x00038188
	public void HeavyImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactHeavyDamage > 0)
			{
				this.Hurt(this.impactHeavyDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00039FE0 File Offset: 0x000381E0
	public void Hurt(int _damage, Vector3 _hurtDirection)
	{
		if (this.dead)
		{
			return;
		}
		this.healthCurrent -= _damage;
		if (this.healthCurrent <= 0)
		{
			this.healthCurrent = 0;
			this.Death(_hurtDirection);
			return;
		}
		if (!GameManager.Multiplayer())
		{
			this.HurtRPC(_damage, _hurtDirection);
			return;
		}
		this.photonView.RPC("HurtRPC", RpcTarget.All, new object[]
		{
			_damage,
			_hurtDirection
		});
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0003A055 File Offset: 0x00038255
	[PunRPC]
	public void HurtRPC(int _damage, Vector3 _hurtDirection)
	{
		this.hurtDirection = _hurtDirection;
		this.hurtEffect = true;
		this.hurtLerp = 0f;
		if (this.hurtDirection == Vector3.zero)
		{
			this.hurtDirection = Random.insideUnitSphere;
		}
		this.onHurtImpulse = true;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0003A094 File Offset: 0x00038294
	private void Death(Vector3 _deathDirection)
	{
		if (!GameManager.Multiplayer())
		{
			this.DeathRPC(_deathDirection);
			return;
		}
		this.photonView.RPC("DeathRPC", RpcTarget.All, new object[]
		{
			_deathDirection
		});
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0003A0C8 File Offset: 0x000382C8
	[PunRPC]
	public void DeathRPC(Vector3 _deathDirection)
	{
		this.hurtDirection = _deathDirection;
		this.hurtEffect = true;
		this.hurtLerp = 0f;
		this.deadImpulseTimer = this.deathFreezeTime;
		this.enemy.Freeze(this.deathFreezeTime);
		this.onDeathStart.Invoke();
		this.deadImpulse = true;
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0003A11D File Offset: 0x0003831D
	[PunRPC]
	public void DeathImpulseRPC()
	{
		this.deadImpulse = false;
		this.dead = true;
		if (this.hurtDirection == Vector3.zero)
		{
			this.hurtDirection = Random.insideUnitSphere;
		}
		this.onDeath.Invoke();
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0003A155 File Offset: 0x00038355
	public void ObjectHurtDisable(float _time)
	{
		this.objectHurtDisableTimer = _time;
	}

	// Token: 0x040009BA RID: 2490
	private PhotonView photonView;

	// Token: 0x040009BB RID: 2491
	private Enemy enemy;

	// Token: 0x040009BC RID: 2492
	public int health = 100;

	// Token: 0x040009BD RID: 2493
	internal int healthCurrent;

	// Token: 0x040009BE RID: 2494
	private bool deadImpulse;

	// Token: 0x040009BF RID: 2495
	internal bool dead;

	// Token: 0x040009C0 RID: 2496
	private float deadImpulseTimer;

	// Token: 0x040009C1 RID: 2497
	public float deathFreezeTime = 0.1f;

	// Token: 0x040009C2 RID: 2498
	public bool impactHurt;

	// Token: 0x040009C3 RID: 2499
	public int impactLightDamage;

	// Token: 0x040009C4 RID: 2500
	public int impactMediumDamage;

	// Token: 0x040009C5 RID: 2501
	public int impactHeavyDamage;

	// Token: 0x040009C6 RID: 2502
	public bool objectHurt;

	// Token: 0x040009C7 RID: 2503
	public float objectHurtMultiplier = 1f;

	// Token: 0x040009C8 RID: 2504
	public bool objectHurtStun = true;

	// Token: 0x040009C9 RID: 2505
	internal float objectHurtStunTime = 2f;

	// Token: 0x040009CA RID: 2506
	public Transform meshParent;

	// Token: 0x040009CB RID: 2507
	private List<MeshRenderer> renderers;

	// Token: 0x040009CC RID: 2508
	private List<Material> sharedMaterials = new List<Material>();

	// Token: 0x040009CD RID: 2509
	internal List<Material> instancedMaterials = new List<Material>();

	// Token: 0x040009CE RID: 2510
	public bool spawnValuable = true;

	// Token: 0x040009CF RID: 2511
	public int spawnValuableMax = 3;

	// Token: 0x040009D0 RID: 2512
	internal int spawnValuableCurrent;

	// Token: 0x040009D1 RID: 2513
	internal Vector3 hurtDirection;

	// Token: 0x040009D2 RID: 2514
	private bool hurtEffect;

	// Token: 0x040009D3 RID: 2515
	private AnimationCurve hurtCurve;

	// Token: 0x040009D4 RID: 2516
	private float hurtLerp;

	// Token: 0x040009D5 RID: 2517
	public UnityEvent onHurt;

	// Token: 0x040009D6 RID: 2518
	private bool onHurtImpulse;

	// Token: 0x040009D7 RID: 2519
	public UnityEvent onDeathStart;

	// Token: 0x040009D8 RID: 2520
	public UnityEvent onDeath;

	// Token: 0x040009D9 RID: 2521
	public UnityEvent onObjectHurt;

	// Token: 0x040009DA RID: 2522
	internal PlayerAvatar onObjectHurtPlayer;

	// Token: 0x040009DB RID: 2523
	private int materialHurtColor;

	// Token: 0x040009DC RID: 2524
	private int materialHurtAmount;

	// Token: 0x040009DD RID: 2525
	internal float objectHurtDisableTimer;
}
