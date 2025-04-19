using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class ItemLight : MonoBehaviour
{
	// Token: 0x06000ACE RID: 2766 RVA: 0x0006054C File Offset: 0x0005E74C
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.lightIntensityOriginal = this.itemLight.intensity;
		this.lightRangeOriginal = this.itemLight.range;
		this.itemLight.intensity = 0f;
		this.itemLight.range = 0f;
		this.itemLight.enabled = false;
		if (this.meshRenderers.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				if (meshRenderer && meshRenderer.gameObject.activeSelf && meshRenderer && meshRenderer.gameObject.activeSelf)
				{
					Material material = meshRenderer.material;
					this.fresnelScaleOriginal = material.GetFloat("_FresnelScale");
					break;
				}
			}
		}
		if (this.alwaysActive)
		{
			this.itemLight.enabled = true;
			this.showLight = true;
			this.itemLight.intensity = this.lightIntensityOriginal;
			this.itemLight.range = this.lightRangeOriginal;
		}
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00060684 File Offset: 0x0005E884
	private void SetAllFresnel(float _value)
	{
		if (this.meshRenderers.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				if (meshRenderer && meshRenderer.gameObject.activeSelf)
				{
					meshRenderer.material.SetFloat("_FresnelScale", _value);
				}
			}
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00060704 File Offset: 0x0005E904
	private void Update()
	{
		if (this.showLight)
		{
			if (!this.itemLight.enabled)
			{
				this.itemLight.intensity = 0f;
				this.itemLight.range = 0f;
				this.animationCurveEval = 0f;
				this.itemLight.enabled = true;
			}
			if (this.itemLight.intensity < this.lightIntensityOriginal - 0.01f)
			{
				this.animationCurveEval += Time.deltaTime * 0.05f;
				float t = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
				if (this.meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer in this.meshRenderers)
					{
						if (meshRenderer && meshRenderer.gameObject.activeSelf)
						{
							Material material = meshRenderer.material;
							float @float = material.GetFloat("_FresnelScale");
							material.SetFloat("_FresnelScale", Mathf.Lerp(@float, this.fresnelScaleOriginal, t));
						}
					}
				}
				this.itemLight.intensity = Mathf.Lerp(this.itemLight.intensity, this.lightIntensityOriginal, t);
				this.itemLight.range = Mathf.Lerp(this.itemLight.range, this.lightRangeOriginal, t);
			}
		}
		else if (this.itemLight.enabled)
		{
			this.animationCurveEval += Time.deltaTime * 1f;
			float t2 = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
			this.itemLight.intensity = Mathf.Lerp(this.itemLight.intensity, 0f, t2);
			this.itemLight.range = Mathf.Lerp(this.itemLight.range, 0f, t2);
			if (this.meshRenderers.Count > 0)
			{
				foreach (MeshRenderer meshRenderer2 in this.meshRenderers)
				{
					if (meshRenderer2 && meshRenderer2.gameObject.activeSelf)
					{
						Material material2 = meshRenderer2.material;
						float float2 = material2.GetFloat("_FresnelScale");
						material2.SetFloat("_FresnelScale", Mathf.Lerp(float2, 0f, t2));
					}
				}
			}
			if (this.itemLight.intensity < 0.01f)
			{
				this.animationCurveEval = 0f;
				this.itemLight.intensity = 0f;
				this.itemLight.range = 0f;
				if (this.meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer3 in this.meshRenderers)
					{
						if (meshRenderer3 && meshRenderer3.gameObject.activeSelf)
						{
							meshRenderer3.material.SetFloat("_FresnelScale", 0f);
						}
					}
				}
				this.itemLight.enabled = false;
			}
		}
		if (SemiFunc.FPSImpulse1())
		{
			if (SemiFunc.PlayerGetNearestTransformWithinRange(16f, base.transform.position, false, default(LayerMask)))
			{
				this.culledLight = false;
			}
			else
			{
				this.culledLight = true;
			}
			if (!this.alwaysActive)
			{
				if (this.culledLight)
				{
					this.showLight = false;
					return;
				}
				if (this.physGrabObject.grabbed)
				{
					this.showLight = false;
					return;
				}
				if (!this.showLight)
				{
					this.itemLight.enabled = true;
					this.showLight = true;
					return;
				}
			}
			else
			{
				if (this.culledLight)
				{
					this.showLight = false;
					return;
				}
				this.showLight = true;
			}
		}
	}

	// Token: 0x04001185 RID: 4485
	public bool alwaysActive;

	// Token: 0x04001186 RID: 4486
	public Light itemLight;

	// Token: 0x04001187 RID: 4487
	private float lightIntensityOriginal;

	// Token: 0x04001188 RID: 4488
	private float lightRangeOriginal;

	// Token: 0x04001189 RID: 4489
	private bool showLight = true;

	// Token: 0x0400118A RID: 4490
	private PhysGrabObject physGrabObject;

	// Token: 0x0400118B RID: 4491
	private bool culledLight;

	// Token: 0x0400118C RID: 4492
	public AnimationCurve lightIntensityCurve;

	// Token: 0x0400118D RID: 4493
	private float animationCurveEval;

	// Token: 0x0400118E RID: 4494
	public List<MeshRenderer> meshRenderers;

	// Token: 0x0400118F RID: 4495
	private float fresnelScaleOriginal;
}
