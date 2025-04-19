using System;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class ShatterEffect : MonoBehaviour
{
	// Token: 0x06000F7E RID: 3966 RVA: 0x0008E0AC File Offset: 0x0008C2AC
	private void Start()
	{
		this.mainModule = this.partSystem.main;
		this.emissionModule = this.partSystem.emission;
		this.shapeModule = this.partSystem.shape;
		this.mainModuleSmoke = this.particleSystemSmoke.main;
		this.shapeModuleSmoke = this.particleSystemSmoke.shape;
		this.SetupParticleSystem();
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0008E114 File Offset: 0x0008C314
	private void SetupParticleSystem()
	{
		this.emissionModule.rateOverTimeMultiplier = this.partSystem.emission.rateOverTimeMultiplier * (this.particleAmountMultiplier / 100f);
		this.shapeModule.scale = this.ParticleEmissionBox.localScale;
		this.shapeModuleSmoke.scale = this.ParticleEmissionBox.localScale;
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0008E178 File Offset: 0x0008C378
	public void SpawnParticles(Vector3 direction)
	{
		this.partSystem.transform.rotation = Quaternion.LookRotation(direction);
		this.particleSystemSmoke.transform.rotation = Quaternion.LookRotation(direction);
		this.partSystem.transform.position = this.ParticleEmissionBox.position;
		this.shapeModule.scale = this.ParticleEmissionBox.localScale;
		this.mainModule.startColor = this.particleColors;
		this.partSystem.Play();
		this.partSystem.transform.SetParent(null);
		this.particleSystemSmoke.Play();
		this.particleSystemSmoke.transform.SetParent(null);
		this.ShatterSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001A57 RID: 6743
	public ParticleSystem partSystem;

	// Token: 0x04001A58 RID: 6744
	public ParticleSystem particleSystemSmoke;

	// Token: 0x04001A59 RID: 6745
	[Range(0f, 100f)]
	public float particleAmountMultiplier = 50f;

	// Token: 0x04001A5A RID: 6746
	public Gradient particleColors;

	// Token: 0x04001A5B RID: 6747
	public Transform ParticleEmissionBox;

	// Token: 0x04001A5C RID: 6748
	[Space]
	public Sound ShatterSound;

	// Token: 0x04001A5D RID: 6749
	private ParticleSystem.MainModule mainModule;

	// Token: 0x04001A5E RID: 6750
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04001A5F RID: 6751
	private ParticleSystem.ShapeModule shapeModule;

	// Token: 0x04001A60 RID: 6752
	private ParticleSystem.ShapeModule shapeModuleSmoke;

	// Token: 0x04001A61 RID: 6753
	private ParticleSystem.MainModule mainModuleSmoke;
}
