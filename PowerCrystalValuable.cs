using System;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class PowerCrystalValuable : MonoBehaviour
{
	// Token: 0x06001402 RID: 5122 RVA: 0x000B002B File Offset: 0x000AE22B
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x000B003C File Offset: 0x000AE23C
	private void Update()
	{
		if (this.GlowActive)
		{
			this.GlowLerp += 8f * Time.deltaTime;
		}
		if (this.GlowLerp >= 1f)
		{
			this.GlowLerp = 0f;
			this.GlowActive = false;
		}
		this.CrystalLight.lightComponent.intensity = 3f + this.GlowCurve.Evaluate(this.GlowLerp) * this.GlowIntensity;
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x000B00B8 File Offset: 0x000AE2B8
	public void Explode()
	{
		SemiFunc.LightRemove(this.CrystalLight);
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 50, 1f, false, false, 1f);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x000B00FC File Offset: 0x000AE2FC
	public void GlowDim()
	{
		this.GlowIntensity = 3f;
		this.GlowActive = true;
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x000B0110 File Offset: 0x000AE310
	public void GlowMed()
	{
		this.GlowIntensity = 5f;
		this.GlowActive = true;
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x000B0124 File Offset: 0x000AE324
	public void GlowStrong()
	{
		this.GlowIntensity = 10f;
		this.GlowActive = true;
	}

	// Token: 0x04002223 RID: 8739
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002224 RID: 8740
	public Transform Center;

	// Token: 0x04002225 RID: 8741
	public GameObject Crystal;

	// Token: 0x04002226 RID: 8742
	public AnimationCurve GlowCurve;

	// Token: 0x04002227 RID: 8743
	public PropLight CrystalLight;

	// Token: 0x04002228 RID: 8744
	private bool GlowActive;

	// Token: 0x04002229 RID: 8745
	private float GlowLerp;

	// Token: 0x0400222A RID: 8746
	private float GlowIntensity;
}
