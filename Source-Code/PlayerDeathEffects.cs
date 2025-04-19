using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class PlayerDeathEffects : MonoBehaviour
{
	// Token: 0x06000EC1 RID: 3777 RVA: 0x000859D1 File Offset: 0x00083BD1
	private void Start()
	{
		this.deathLightIntensityDefault = this.deathLight.intensity;
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x000859E4 File Offset: 0x00083BE4
	private void Update()
	{
		base.transform.position = this.followTransform.position;
		base.transform.rotation = this.followTransform.rotation;
		if (this.triggered)
		{
			this.deathLight.intensity = Mathf.Lerp(this.deathLight.intensity, 0f, Time.deltaTime * 1f);
			if (this.smokeParticles.isStopped && this.bitWeakParticles.isStopped && this.bitStrongParticles.isStopped && this.deathLight.intensity < 0.01f)
			{
				base.gameObject.SetActive(false);
			}
			if (this.hurtColliderTimer > 0f)
			{
				this.hurtColliderTimer -= Time.deltaTime;
				if (this.hurtColliderTimer <= 0f)
				{
					this.hurtCollider.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00085AD8 File Offset: 0x00083CD8
	public void Trigger()
	{
		this.bitWeakParticles.main.startColor = this.playerAvatarVisuals.color;
		this.bitStrongParticles.main.startColor = this.playerAvatarVisuals.color;
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.triggered = true;
		this.enableTransform.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.hurtColliderTimer = this.hurtColliderTime;
		this.deathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.smokeParticles.gameObject.SetActive(true);
		this.smokeParticles.Play();
		this.fireParticles.gameObject.SetActive(true);
		this.fireParticles.Play();
		this.bitWeakParticles.gameObject.SetActive(true);
		this.bitWeakParticles.Play();
		this.bitStrongParticles.gameObject.SetActive(true);
		this.bitStrongParticles.Play();
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x00085C57 File Offset: 0x00083E57
	public void Reset()
	{
		base.gameObject.SetActive(true);
		this.triggered = false;
		this.deathLight.intensity = this.deathLightIntensityDefault;
		this.enableTransform.gameObject.SetActive(false);
	}

	// Token: 0x0400189E RID: 6302
	private bool triggered;

	// Token: 0x0400189F RID: 6303
	public PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x040018A0 RID: 6304
	[Space]
	public Transform followTransform;

	// Token: 0x040018A1 RID: 6305
	public Transform enableTransform;

	// Token: 0x040018A2 RID: 6306
	[Space]
	public Light deathLight;

	// Token: 0x040018A3 RID: 6307
	private float deathLightIntensityDefault;

	// Token: 0x040018A4 RID: 6308
	public ParticleSystem smokeParticles;

	// Token: 0x040018A5 RID: 6309
	public ParticleSystem fireParticles;

	// Token: 0x040018A6 RID: 6310
	public ParticleSystem bitWeakParticles;

	// Token: 0x040018A7 RID: 6311
	public ParticleSystem bitStrongParticles;

	// Token: 0x040018A8 RID: 6312
	public HurtCollider hurtCollider;

	// Token: 0x040018A9 RID: 6313
	private float hurtColliderTime = 0.5f;

	// Token: 0x040018AA RID: 6314
	private float hurtColliderTimer;

	// Token: 0x040018AB RID: 6315
	[Space]
	public Sound deathSound;
}
