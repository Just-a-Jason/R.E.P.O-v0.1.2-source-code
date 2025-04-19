using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class SlowWalkerJumpEffect : MonoBehaviour
{
	// Token: 0x060004B1 RID: 1201 RVA: 0x0002ECE5 File Offset: 0x0002CEE5
	private void Start()
	{
		this.particles.AddRange(base.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0002ECF8 File Offset: 0x0002CEF8
	private void PlayParticles()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0002ED48 File Offset: 0x0002CF48
	public void JumpEffect()
	{
		this.rotationTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
		this.PlayParticles();
		GameDirector.instance.CameraImpact.ShakeDistance(4f, 6f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(4f, 6f, 15f, base.transform.position, 0.1f);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0002EDD8 File Offset: 0x0002CFD8
	public void LandEffect()
	{
		this.rotationTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 6f, 15f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(6f, 6f, 15f, base.transform.position, 0.1f);
		this.PlayParticles();
	}

	// Token: 0x040007A5 RID: 1957
	public Transform rotationTransform;

	// Token: 0x040007A6 RID: 1958
	private List<ParticleSystem> particles = new List<ParticleSystem>();
}
