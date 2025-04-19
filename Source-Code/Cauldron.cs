using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class Cauldron : MonoBehaviour
{
	// Token: 0x060008F1 RID: 2289 RVA: 0x000558C3 File Offset: 0x00053AC3
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.liquid)
		{
			this.liquidRenderer = this.liquid.GetComponent<Renderer>();
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x000558F0 File Offset: 0x00053AF0
	private void Update()
	{
		if (!this.liquid)
		{
			return;
		}
		float loopPitch = 1f;
		if (this.lightGreen.gameObject.activeSelf)
		{
			loopPitch = 1f + this.lightGreen.intensity / 8f * 5f;
		}
		this.soundLoop.LoopPitch = loopPitch;
		this.soundLoop.PlayLoop(this.cauldronActive, 1f, 1f, 1f);
		if (this.cauldronActive && this.lightGreen.gameObject.activeSelf)
		{
			float num = 1f + this.lightGreen.intensity * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num / 30f, 0f, 3f, this.liquid.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && this.explosionTimer <= 0f) || (SemiFunc.IsMasterClient() && this.explosionTimer <= 0f))
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 1f)
			{
				bool flag = this.cauldronActive;
				this.cauldronActive = false;
				foreach (Collider collider in Physics.OverlapSphere(this.sphereChecker.position, this.sphereChecker.localScale.x * 0.5f, SemiFunc.LayerMaskGetPlayersAndPhysObjects(), QueryTriggerInteraction.Ignore))
				{
					if (collider)
					{
						if (collider.GetComponentInParent<PhysGrabObject>())
						{
							this.cauldronActive = true;
							break;
						}
						if (collider.GetComponentInParent<PlayerAvatar>())
						{
							this.cauldronActive = true;
							break;
						}
						if (collider.GetComponentInParent<PlayerController>())
						{
							this.cauldronActive = true;
							break;
						}
					}
				}
				if (this.cauldronActive && !flag)
				{
					this.CookStart();
				}
				if (!this.cauldronActive && flag)
				{
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("EndCookRPC", RpcTarget.All, Array.Empty<object>());
					}
					else
					{
						this.EndCookRPC();
					}
				}
				this.checkTimer = 0f;
			}
		}
		if (this.explosionTimer > 0f)
		{
			this.explosionTimer -= Time.deltaTime;
			if (this.explosionTimer <= 0f && this.explosion)
			{
				this.explosion.gameObject.SetActive(false);
			}
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				if (this.hurtCollider)
				{
					this.hurtCollider.SetActive(false);
				}
			}
			else if (this.hurtCollider)
			{
				this.hurtCollider.SetActive(true);
			}
		}
		if (this.cauldronActive)
		{
			if (!this.sparkParticles.isPlaying)
			{
				this.sparkParticles.Play();
			}
			if (!this.windParticles.isPlaying)
			{
				this.windParticles.Play();
			}
			if (!this.lightGreen.gameObject.activeSelf)
			{
				this.lightGreen.gameObject.SetActive(true);
				this.lightGreen.intensity = 0f;
				return;
			}
			this.lightGreen.intensity += Time.deltaTime * 2f;
			this.lightGreen.intensity = Mathf.Clamp(this.lightGreen.intensity, 0f, 8f);
			float num2 = Mathf.Abs(this.lightGreen.intensity / 8f);
			this.lightGreen.range = 4f + 1f * Mathf.Sin(Time.time * 10f) * num2;
			if (this.lightGreen.intensity > 7.5f)
			{
				this.safetyTimer += Time.deltaTime;
				if (this.safetyTimer > 3f)
				{
					this.EndCook();
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.Explosion();
					return;
				}
			}
		}
		else
		{
			if (this.lightGreen.gameObject.activeSelf)
			{
				this.lightGreen.intensity -= Time.deltaTime * 20f;
				this.lightGreen.intensity = Mathf.Clamp(this.lightGreen.intensity, 0f, 8f);
				if (this.lightGreen.intensity < 0.1f)
				{
					this.lightGreen.gameObject.SetActive(false);
				}
			}
			this.sparkParticles.Stop();
			this.windParticles.Stop();
		}
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00055D8E File Offset: 0x00053F8E
	private void Explosion()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ExplosionRPC();
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00055DB4 File Offset: 0x00053FB4
	[PunRPC]
	public void EndCookRPC()
	{
		this.EndCook();
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00055DBC File Offset: 0x00053FBC
	private void EndCook()
	{
		this.cauldronActive = false;
		this.safetyTimer = 0f;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00055DD0 File Offset: 0x00053FD0
	[PunRPC]
	public void ExplosionRPC()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.liquid.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, this.liquid.position, 0.1f);
		this.soundExplosion.Play(this.liquid.position, 1f, 1f, 1f, 1f);
		this.explosion.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.explosionTimer = 3f;
		this.hurtColliderTimer = 0.5f;
		this.EndCook();
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00055EA2 File Offset: 0x000540A2
	private void CookStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CookStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.CookStartRPC();
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00055EC8 File Offset: 0x000540C8
	[PunRPC]
	public void CookStartRPC()
	{
		this.cauldronActive = true;
	}

	// Token: 0x0400104E RID: 4174
	public Transform sphereChecker;

	// Token: 0x0400104F RID: 4175
	public ParticleSystem sparkParticles;

	// Token: 0x04001050 RID: 4176
	public ParticleSystem windParticles;

	// Token: 0x04001051 RID: 4177
	public Light lightGreen;

	// Token: 0x04001052 RID: 4178
	public GameObject explosion;

	// Token: 0x04001053 RID: 4179
	public GameObject hurtCollider;

	// Token: 0x04001054 RID: 4180
	private float checkTimer;

	// Token: 0x04001055 RID: 4181
	private bool cauldronActive;

	// Token: 0x04001056 RID: 4182
	private float explosionTimer;

	// Token: 0x04001057 RID: 4183
	private float hurtColliderTimer;

	// Token: 0x04001058 RID: 4184
	private PhotonView photonView;

	// Token: 0x04001059 RID: 4185
	public Transform liquid;

	// Token: 0x0400105A RID: 4186
	private Renderer liquidRenderer;

	// Token: 0x0400105B RID: 4187
	public Sound soundExplosion;

	// Token: 0x0400105C RID: 4188
	public Sound soundLoop;

	// Token: 0x0400105D RID: 4189
	private float safetyTimer;
}
