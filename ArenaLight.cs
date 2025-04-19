using System;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class ArenaLight : MonoBehaviour
{
	// Token: 0x0600077B RID: 1915 RVA: 0x00047132 File Offset: 0x00045332
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.arenaLight = base.GetComponentInChildren<Light>();
		this.lightIntensity = this.arenaLight.intensity;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00047160 File Offset: 0x00045360
	private void Update()
	{
		if (this.arenaLight.enabled)
		{
			if (this.arenaLight.intensity > this.lightIntensity)
			{
				this.arenaLight.intensity = Mathf.Lerp(this.arenaLight.intensity, this.lightIntensity, Time.deltaTime * 2f);
				Color b = new Color(0.3f, 0f, 0f);
				this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), b, Time.deltaTime * 2f));
				return;
			}
			this.arenaLight.intensity = this.lightIntensity;
		}
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0004721F File Offset: 0x0004541F
	public void TurnOnArenaWarningLight()
	{
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
		this.arenaLight.enabled = true;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00047247 File Offset: 0x00045447
	public void PulsateLight()
	{
		this.arenaLight.intensity = this.lightIntensity * 2f;
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
	}

	// Token: 0x04000D35 RID: 3381
	internal MeshRenderer meshRenderer;

	// Token: 0x04000D36 RID: 3382
	internal Light arenaLight;

	// Token: 0x04000D37 RID: 3383
	private float lightIntensity = 0.5f;
}
