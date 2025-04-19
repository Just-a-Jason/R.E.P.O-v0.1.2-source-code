using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class PhysObjectParticles : MonoBehaviour
{
	// Token: 0x06000DCB RID: 3531 RVA: 0x0007D454 File Offset: 0x0007B654
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0007D464 File Offset: 0x0007B664
	public void DestroyParticles()
	{
		if (!SemiFunc.RunIsTutorial())
		{
			base.StartCoroutine(this.DestroyParticlesAfterTime(4f));
		}
		foreach (Transform transform in this.colliderTransforms)
		{
			Vector3 vector = transform.localScale;
			Vector3 eulerAngles = transform.rotation.eulerAngles;
			Transform colliderTransform = transform;
			int num = (int)(vector.x * 100f * (vector.y * 100f) * (vector.z * 100f) / 1000f);
			num = (int)((float)Mathf.Clamp(num, 10, 150) * this.multiplier);
			float num2 = vector.x * 100f * (vector.y * 100f) * (vector.z * 100f) / 30000f;
			if (transform.GetComponent<SphereCollider>())
			{
				num2 *= 0.55f;
				vector *= 0.4f;
			}
			num2 = Mathf.Clamp(num2, 0.3f, 2f) * this.multiplier;
			this.SpawnParticles(num, num2, vector, eulerAngles, colliderTransform);
		}
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0007D5A8 File Offset: 0x0007B7A8
	private IEnumerator DestroyParticlesAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x0007D5C0 File Offset: 0x0007B7C0
	public void ImpactSmoke(int amount, Vector3 position, float size)
	{
		size = Mathf.Clamp(size, 0.7f, 1.5f);
		Vector3 localPosition = base.transform.InverseTransformPoint(position);
		this.particleSystemSmoke.transform.localPosition = localPosition;
		ParticleSystem.MainModule main = this.particleSystemSmoke.main;
		ParticleSystem.ShapeModule shape = this.particleSystemSmoke.shape;
		float startSizeMultiplier = main.startSizeMultiplier;
		shape.scale = new Vector3(0.2f, 0.2f, 0.2f);
		float max = Mathf.Clamp(size / 4f, 0f, 2f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(0f, max);
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		this.particleSystemSmoke.Emit(amount);
		main.startSizeXMultiplier = startSizeMultiplier;
		main.startSizeYMultiplier = startSizeMultiplier;
		main.startSizeZMultiplier = startSizeMultiplier;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0007D6B4 File Offset: 0x0007B8B4
	private void SpawnParticles(int bitCount, float size, Vector3 colliderScale, Vector3 colliderRotation, Transform colliderTransform)
	{
		Vector3.left * 5f;
		this.particleSystemBits.transform.position = colliderTransform.position;
		this.particleSystemBitsSmall.transform.position = colliderTransform.position;
		this.particleSystemSmoke.transform.position = colliderTransform.position;
		ParticleSystem.ShapeModule shape = this.particleSystemBits.shape;
		ParticleSystem.MainModule main = this.particleSystemBits.main;
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		main = this.particleSystemBitsSmall.main;
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		shape = this.particleSystemBitsSmall.shape;
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		main = this.particleSystemSmoke.main;
		shape = this.particleSystemSmoke.shape;
		float startSizeMultiplier = main.startSizeMultiplier;
		main.startSizeXMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		main.startSizeYMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		main.startSizeZMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		float max = Mathf.Clamp(size, 0.5f, 2f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(0f, max);
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		this.particleSystemBits.Emit(bitCount);
		this.particleSystemBitsSmall.Emit(bitCount / 3);
		this.particleSystemSmoke.Emit(bitCount);
		this.particlesSpawned = true;
		main = this.particleSystemBits.main;
		main.startSizeXMultiplier /= size;
		main.startSizeYMultiplier /= size;
		main.startSizeZMultiplier /= size;
		main = this.particleSystemBitsSmall.main;
		main.startSizeXMultiplier /= size;
		main.startSizeYMultiplier /= size;
		main.startSizeZMultiplier /= size;
		main = this.particleSystemSmoke.main;
		main.startSizeXMultiplier = startSizeMultiplier;
		main.startSizeYMultiplier = startSizeMultiplier;
		main.startSizeZMultiplier = startSizeMultiplier;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0007D934 File Offset: 0x0007BB34
	private void LateUpdate()
	{
		if (!this.particlesSpawned)
		{
			return;
		}
		int maxParticles = this.particleSystemBits.main.maxParticles;
		if (this.particles == null || this.particles.Length < maxParticles)
		{
			this.particles = new ParticleSystem.Particle[maxParticles];
		}
		int num = this.particleSystemBits.GetParticles(this.particles);
		for (int i = 0; i < num; i++)
		{
			float time = (float)i / (float)num;
			Color c = this.gradient.Evaluate(time);
			this.particles[i].startColor = c;
		}
		this.particleSystemBits.SetParticles(this.particles, num);
		maxParticles = this.particleSystemBitsSmall.main.maxParticles;
		if (this.particles == null || this.particles.Length < maxParticles)
		{
			this.particles = new ParticleSystem.Particle[maxParticles];
		}
		num = this.particleSystemBitsSmall.GetParticles(this.particles);
		for (int j = 0; j < num; j++)
		{
			float time2 = (float)j / (float)num;
			Color c2 = this.gradient.Evaluate(time2);
			this.particles[j].startColor = c2;
		}
		this.particleSystemBitsSmall.SetParticles(this.particles, num);
		this.particlesSpawned = false;
	}

	// Token: 0x040016B2 RID: 5810
	public Gradient gradient;

	// Token: 0x040016B3 RID: 5811
	public ParticleSystem particleSystemBits;

	// Token: 0x040016B4 RID: 5812
	public ParticleSystem particleSystemBitsSmall;

	// Token: 0x040016B5 RID: 5813
	public ParticleSystem particleSystemSmoke;

	// Token: 0x040016B6 RID: 5814
	private ParticleSystem.Particle[] particles;

	// Token: 0x040016B7 RID: 5815
	private bool particlesSpawned;

	// Token: 0x040016B8 RID: 5816
	private PhysGrabObject physGrabObject;

	// Token: 0x040016B9 RID: 5817
	internal float multiplier = 1f;

	// Token: 0x040016BA RID: 5818
	public List<Transform> colliderTransforms = new List<Transform>();
}
