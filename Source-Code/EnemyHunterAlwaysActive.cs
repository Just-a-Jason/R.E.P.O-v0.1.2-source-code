using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class EnemyHunterAlwaysActive : MonoBehaviour
{
	// Token: 0x0600038A RID: 906 RVA: 0x00023608 File Offset: 0x00021808
	private void Start()
	{
		this.shootLightIntensity = this.shootLight.lightComponent.intensity;
		this.hitLightIntensity = this.hitLight.lightComponent.intensity;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00023638 File Offset: 0x00021838
	private void Update()
	{
		if (this.shootEffectActive)
		{
			this.shootLight.lightComponent.intensity -= Time.deltaTime * 20f;
			this.shootLight.originalIntensity = this.shootLightIntensity;
			if (this.shootLight.lightComponent.intensity <= 0f)
			{
				this.shootLight.lightComponent.enabled = false;
				this.shootEffectActive = false;
			}
		}
		if (this.hitEffectActive)
		{
			this.hitLight.lightComponent.intensity -= Time.deltaTime * 20f;
			this.hitLight.originalIntensity = this.hitLightIntensity;
			if (this.hitLight.lightComponent.intensity <= 0f)
			{
				this.hitLight.lightComponent.enabled = false;
				this.hitEffectActive = false;
			}
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0002371C File Offset: 0x0002191C
	public void Trigger()
	{
		this.shootEffectActive = true;
		this.hitEffectActive = true;
		this.shootLight.lightComponent.enabled = true;
		this.shootLight.lightComponent.intensity = this.shootLightIntensity;
		this.shootLight.originalIntensity = this.shootLightIntensity;
		this.hitLight.lightComponent.enabled = true;
		this.hitLight.lightComponent.intensity = this.hitLightIntensity;
		this.hitLight.originalIntensity = this.hitLightIntensity;
	}

	// Token: 0x0400062A RID: 1578
	private bool shootEffectActive;

	// Token: 0x0400062B RID: 1579
	public PropLight shootLight;

	// Token: 0x0400062C RID: 1580
	private float shootLightIntensity;

	// Token: 0x0400062D RID: 1581
	private bool hitEffectActive;

	// Token: 0x0400062E RID: 1582
	public PropLight hitLight;

	// Token: 0x0400062F RID: 1583
	private float hitLightIntensity;
}
