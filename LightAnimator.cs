using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class LightAnimator : MonoBehaviour
{
	// Token: 0x06001196 RID: 4502 RVA: 0x0009C520 File Offset: 0x0009A720
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.lightIntensityInit = this.lightComponent.intensity;
		if (this.lightActive)
		{
			this.lightIntensity = this.lightIntensityInit;
			this.lightComponent.intensity = this.lightIntensity;
			this.lightComponent.enabled = true;
			return;
		}
		this.lightIntensity = 0f;
		this.lightComponent.intensity = this.lightIntensity;
		this.lightComponent.enabled = false;
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0009C5A4 File Offset: 0x0009A7A4
	private void Update()
	{
		if (this.lightActive)
		{
			if (!this.lightComponent.enabled)
			{
				this.lightComponent.enabled = true;
			}
			this.lightIntensity = Mathf.Clamp(this.lightIntensity + this.introSpeed * Time.deltaTime, 0f, this.lightIntensityInit);
			this.lightComponent.intensity = this.lightIntensity;
			return;
		}
		if (this.lightComponent.enabled)
		{
			this.lightIntensity = Mathf.Clamp(this.lightIntensity - this.outroSpeed * Time.deltaTime, 0f, this.lightIntensityInit);
			this.lightComponent.intensity = this.lightIntensity;
			if (this.lightIntensity <= 0f)
			{
				this.lightComponent.enabled = false;
			}
		}
	}

	// Token: 0x04001D83 RID: 7555
	private Light lightComponent;

	// Token: 0x04001D84 RID: 7556
	private float lightIntensityInit;

	// Token: 0x04001D85 RID: 7557
	private float lightIntensity;

	// Token: 0x04001D86 RID: 7558
	public bool lightActive;

	// Token: 0x04001D87 RID: 7559
	public float introSpeed;

	// Token: 0x04001D88 RID: 7560
	public float outroSpeed;
}
