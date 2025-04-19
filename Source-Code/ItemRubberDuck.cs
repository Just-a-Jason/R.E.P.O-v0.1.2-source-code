using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class ItemRubberDuck : MonoBehaviour
{
	// Token: 0x06000B0A RID: 2826 RVA: 0x00062494 File Offset: 0x00060694
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.hurtCollider.gameObject.SetActive(false);
		this.photonView = base.GetComponent<PhotonView>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		foreach (TrailRenderer item in base.GetComponentsInChildren<TrailRenderer>())
		{
			this.trails.Add(item);
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0006252C File Offset: 0x0006072C
	private void Update()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if ((SemiFunc.RunIsLevel() || SemiFunc.RunIsArena()) && this.rb.velocity.magnitude < 0.1f)
		{
			if (this.lilQuacksTimer > 0f)
			{
				this.lilQuacksTimer -= Time.deltaTime;
				return;
			}
			this.lilQuacksTimer = Random.Range(1f, 3f);
			this.LilQuackJump();
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x000625A4 File Offset: 0x000607A4
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped || this.itemEquippable.wasEquippedTimer > 0f)
		{
			this.prevPosition = this.rb.position;
			return;
		}
		if (this.itemBattery.batteryLifeInt == 0)
		{
			if (!this.brokenObject.activeSelf)
			{
				this.brokenObject.SetActive(true);
				this.notBrokenObject.SetActive(false);
			}
		}
		else if (this.brokenObject.activeSelf)
		{
			this.brokenObject.SetActive(false);
			this.notBrokenObject.SetActive(true);
		}
		Vector3 vector = (this.rb.position - this.prevPosition) / Time.fixedDeltaTime;
		Vector3 normalized = (this.rb.position - this.prevPosition).normalized;
		this.prevPosition = this.rb.position;
		if (!this.physGrabObject.grabbed && this.itemBattery.batteryLife > 0f)
		{
			if (vector.magnitude > 5f)
			{
				this.playDuckLoop = true;
				this.trailTimer = 0.2f;
			}
			else
			{
				this.playDuckLoop = false;
			}
		}
		else
		{
			this.playDuckLoop = false;
		}
		if (this.trailTimer > 0f)
		{
			this.playDuckLoop = true;
			this.trailTimer -= Time.fixedDeltaTime;
			using (List<TrailRenderer>.Enumerator enumerator = this.trails.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TrailRenderer trailRenderer = enumerator.Current;
					trailRenderer.emitting = true;
				}
				goto IL_1C1;
			}
		}
		this.playDuckLoop = false;
		foreach (TrailRenderer trailRenderer2 in this.trails)
		{
			trailRenderer2.emitting = false;
		}
		IL_1C1:
		this.soundDuckLoop.PlayLoop(this.playDuckLoop, 2f, 1f, 1f);
		if (this.hurtColliderTime > 0f)
		{
			this.hurtTransform.forward = normalized;
			if (!this.hurtCollider.gameObject.activeSelf)
			{
				this.hurtCollider.gameObject.SetActive(true);
				float num = vector.magnitude * 2f;
				if (num > 50f)
				{
					num = 50f;
				}
				this.hurtCollider.physHitForce = num;
				this.hurtCollider.physHitTorque = num;
				this.hurtCollider.enemyHitForce = num;
				this.hurtCollider.enemyHitTorque = num;
				this.hurtCollider.playerTumbleForce = num;
				this.hurtCollider.playerTumbleTorque = num;
			}
			this.hurtColliderTime -= Time.fixedDeltaTime;
			return;
		}
		if (this.hurtCollider.gameObject.activeSelf)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00062890 File Offset: 0x00060A90
	private void LilQuackJump()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		this.rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
		this.rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
		this.rb.AddForce(Random.insideUnitCircle * 0.2f, ForceMode.Impulse);
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("LilQuackJumpRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.LilQuackJumpRPC();
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00062938 File Offset: 0x00060B38
	[PunRPC]
	public void LilQuackJumpRPC()
	{
		this.soundQuack.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00062965 File Offset: 0x00060B65
	public void Squeak()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		this.soundSqueak.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x000629A8 File Offset: 0x00060BA8
	public void Quack()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		this.soundQuack.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.hurtColliderTime = 0.2f;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.itemBattery.batteryLife -= 2.5f;
			if (Random.Range(0, 10) == 0)
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("QuackRPC", RpcTarget.All, Array.Empty<object>());
				}
				else
				{
					this.QuackRPC();
				}
			}
			if (this.rb.velocity.magnitude < 20f)
			{
				this.rb.velocity = this.rb.velocity * 5f;
				this.rb.AddTorque(Random.insideUnitSphere * 40f);
			}
		}
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00062AB4 File Offset: 0x00060CB4
	[PunRPC]
	public void QuackRPC()
	{
		this.soundDuckExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundDuckExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		ParticlePrefabExplosion particlePrefabExplosion = this.particleScriptExplosion.Spawn(base.transform.position, 0.85f, 0, 250, 1f, false, true, 1f);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}

	// Token: 0x040011E7 RID: 4583
	public Sound soundQuack;

	// Token: 0x040011E8 RID: 4584
	public Sound soundSqueak;

	// Token: 0x040011E9 RID: 4585
	public Sound soundDuckLoop;

	// Token: 0x040011EA RID: 4586
	public Sound soundDuckExplosion;

	// Token: 0x040011EB RID: 4587
	public Sound soundDuckExplosionGlobal;

	// Token: 0x040011EC RID: 4588
	private Rigidbody rb;

	// Token: 0x040011ED RID: 4589
	private PhotonView photonView;

	// Token: 0x040011EE RID: 4590
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040011EF RID: 4591
	private PhysGrabObject physGrabObject;

	// Token: 0x040011F0 RID: 4592
	public HurtCollider hurtCollider;

	// Token: 0x040011F1 RID: 4593
	public Transform hurtTransform;

	// Token: 0x040011F2 RID: 4594
	private float hurtColliderTime;

	// Token: 0x040011F3 RID: 4595
	private Vector3 prevPosition;

	// Token: 0x040011F4 RID: 4596
	private bool playDuckLoop;

	// Token: 0x040011F5 RID: 4597
	private List<TrailRenderer> trails = new List<TrailRenderer>();

	// Token: 0x040011F6 RID: 4598
	private ItemBattery itemBattery;

	// Token: 0x040011F7 RID: 4599
	private float trailTimer;

	// Token: 0x040011F8 RID: 4600
	public GameObject brokenObject;

	// Token: 0x040011F9 RID: 4601
	public GameObject notBrokenObject;

	// Token: 0x040011FA RID: 4602
	private ItemEquippable itemEquippable;

	// Token: 0x040011FB RID: 4603
	private float lilQuacksTimer;
}
