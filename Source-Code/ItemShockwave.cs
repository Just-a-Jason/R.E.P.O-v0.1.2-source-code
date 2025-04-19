using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class ItemShockwave : MonoBehaviour
{
	// Token: 0x06000B15 RID: 2837 RVA: 0x00062BD8 File Offset: 0x00060DD8
	private void Start()
	{
		this.startScale = base.transform.localScale.x;
		this.lightShockwave = base.GetComponentInChildren<Light>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.meshRenderer.material.color = Color.white;
		base.transform.localScale = Vector3.zero;
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleSystemSparks.Play();
		this.particleSystemLightning.Play();
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00062D00 File Offset: 0x00060F00
	private void Update()
	{
		base.transform.Rotate(Vector3.up, 100f * Time.deltaTime);
		if (base.transform.localScale.x < this.startScale)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * 20f;
			this.lightShockwave.intensity = Mathf.Lerp(4f, 35f, Mathf.InverseLerp(0f, this.startScale, base.transform.localScale.x));
			this.lightShockwave.range = base.transform.localScale.x * 3f;
			return;
		}
		if (!this.finalScale)
		{
			base.transform.localScale = Vector3.one * this.startScale;
			this.hurtCollider.gameObject.SetActive(false);
			this.finalScale = true;
			return;
		}
		float num = Mathf.Lerp(base.transform.localScale.x, this.startScale * 1.2f, Time.deltaTime * 2f);
		base.transform.localScale = Vector3.one * num;
		float num2 = Mathf.InverseLerp(this.startScale, this.startScale * 1.2f, num);
		Color color = this.meshRenderer.material.color;
		color.a = Mathf.Lerp(1f, 0f, num2);
		this.meshRenderer.material.color = color;
		this.lightShockwave.intensity = Mathf.Lerp(35f, 0f, num2);
		if (num2 > 0.998f)
		{
			if (this.particleSystemSparks)
			{
				this.particleSystemSparks.transform.parent = null;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040011FD RID: 4605
	public MeshRenderer meshRenderer;

	// Token: 0x040011FE RID: 4606
	private float startScale = 1f;

	// Token: 0x040011FF RID: 4607
	private bool finalScale;

	// Token: 0x04001200 RID: 4608
	private Light lightShockwave;

	// Token: 0x04001201 RID: 4609
	public ParticleSystem particleSystemWave;

	// Token: 0x04001202 RID: 4610
	public ParticleSystem particleSystemSparks;

	// Token: 0x04001203 RID: 4611
	public ParticleSystem particleSystemLightning;

	// Token: 0x04001204 RID: 4612
	private HurtCollider hurtCollider;

	// Token: 0x04001205 RID: 4613
	public Sound soundExplosion;

	// Token: 0x04001206 RID: 4614
	public Sound soundExplosionGlobal;
}
