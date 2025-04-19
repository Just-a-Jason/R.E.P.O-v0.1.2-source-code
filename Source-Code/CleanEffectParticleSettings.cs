using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class CleanEffectParticleSettings : MonoBehaviour
{
	// Token: 0x060006C2 RID: 1730 RVA: 0x00040B24 File Offset: 0x0003ED24
	private void Start()
	{
		this.UpdateParticleProperties();
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00040B2C File Offset: 0x0003ED2C
	private void UpdateParticleProperties()
	{
		float num = this.cleanEffectParticleSize.transform.localScale.x * 10f / 4f;
		float x = this.cleanEffectParticleRadius.transform.localScale.x;
		float num2 = this.cleanEffectParticleAmount.transform.localScale.x * 100f / 4f;
		ParticleSystem.MainModule main = this.gleamParticles.main;
		float startSizeMultiplier = main.startSizeMultiplier;
		main.startSizeMultiplier = startSizeMultiplier * num;
		this.gleamParticles.shape.radius = x / 2f;
		ParticleSystem.EmissionModule emission = this.gleamParticles.emission;
		float rateOverTimeMultiplier = emission.rateOverTimeMultiplier;
		emission.rateOverTimeMultiplier = rateOverTimeMultiplier * num2;
		Object.Destroy(this.cleanEffectParticleSize);
		Object.Destroy(this.cleanEffectParticleRadius);
		Object.Destroy(this.cleanEffectParticleAmount);
	}

	// Token: 0x04000B72 RID: 2930
	public ParticleSystem gleamParticles;

	// Token: 0x04000B73 RID: 2931
	public GameObject cleanEffectParticleRadius;

	// Token: 0x04000B74 RID: 2932
	public GameObject cleanEffectParticleSize;

	// Token: 0x04000B75 RID: 2933
	public GameObject cleanEffectParticleAmount;
}
