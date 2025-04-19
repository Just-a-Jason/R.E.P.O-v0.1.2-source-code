using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class EnemyValuable : MonoBehaviour
{
	// Token: 0x06000656 RID: 1622 RVA: 0x0003D1E0 File Offset: 0x0003B3E0
	private void Start()
	{
		this.impactDetector = base.GetComponentInChildren<PhysGrabObjectImpactDetector>();
		this.impactDetector.indestructibleSpawnTimer = 0.1f;
		this.outerMaterial = this.outerMeshRenderer.material;
		this.fresnelPowerIndex = Shader.PropertyToID("_FresnelPower");
		this.fresnelColorIndex = Shader.PropertyToID("_FresnelColor");
		this.fresnelPowerDefault = this.outerMaterial.GetFloat(this.fresnelPowerIndex);
		this.outerMaterial.SetFloat(this.fresnelPowerIndex, this.fresnelPowerIndestructible);
		this.fresnelColorDefault = this.outerMaterial.GetColor(this.fresnelColorIndex);
		this.outerMaterial.SetColor(this.fresnelColorIndex, this.fresnelColorIndestructible);
		EnemyDirector.instance.AddEnemyValuable(this);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0003D2A4 File Offset: 0x0003B4A4
	private void Update()
	{
		if (this.indestructibleTimer > 0f)
		{
			this.indestructibleTimer -= Time.deltaTime;
			if (this.indestructibleTimer <= 0f)
			{
				this.impactDetector.destroyDisable = false;
			}
		}
		else if (this.indestructibleLerp < 1f)
		{
			float value = Mathf.Lerp(this.fresnelPowerIndestructible, this.fresnelPowerDefault, this.indestructibleCurve.Evaluate(this.indestructibleLerp));
			this.outerMaterial.SetFloat(this.fresnelPowerIndex, value);
			Color value2 = Color.Lerp(this.fresnelColorIndestructible, this.fresnelColorDefault, this.indestructibleCurve.Evaluate(this.indestructibleLerp));
			this.outerMaterial.SetColor(this.fresnelColorIndex, value2);
			this.indestructibleLerp += 2f * Time.deltaTime;
		}
		this.innerTransform.Rotate(base.transform.up * 60f * Time.deltaTime);
		this.speckSmallTransform.Rotate(base.transform.up * 100f * Time.deltaTime);
		this.speckBigTransform.Rotate(-base.transform.up * 20f * Time.deltaTime);
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0003D406 File Offset: 0x0003B606
	public void Destroy()
	{
		this.impactDetector.DestroyObject(true);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0003D414 File Offset: 0x0003B614
	public void DestroyImpulse()
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.gameObject.SetActive(true);
			particleSystem.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
		}
	}

	// Token: 0x04000AAB RID: 2731
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04000AAC RID: 2732
	private float indestructibleTimer = 5f;

	// Token: 0x04000AAD RID: 2733
	public MeshRenderer outerMeshRenderer;

	// Token: 0x04000AAE RID: 2734
	private Material outerMaterial;

	// Token: 0x04000AAF RID: 2735
	private int fresnelPowerIndex;

	// Token: 0x04000AB0 RID: 2736
	private int fresnelColorIndex;

	// Token: 0x04000AB1 RID: 2737
	[Space]
	private float fresnelPowerDefault;

	// Token: 0x04000AB2 RID: 2738
	public float fresnelPowerIndestructible;

	// Token: 0x04000AB3 RID: 2739
	[Space]
	private Color fresnelColorDefault;

	// Token: 0x04000AB4 RID: 2740
	public Color fresnelColorIndestructible;

	// Token: 0x04000AB5 RID: 2741
	[Space]
	public AnimationCurve indestructibleCurve;

	// Token: 0x04000AB6 RID: 2742
	private float indestructibleLerp;

	// Token: 0x04000AB7 RID: 2743
	[Space]
	public Transform innerTransform;

	// Token: 0x04000AB8 RID: 2744
	public Transform speckSmallTransform;

	// Token: 0x04000AB9 RID: 2745
	public Transform speckBigTransform;

	// Token: 0x04000ABA RID: 2746
	[Space]
	public List<ParticleSystem> particleSystems;
}
