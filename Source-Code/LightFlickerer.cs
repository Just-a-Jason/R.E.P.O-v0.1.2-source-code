using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class LightFlickerer : MonoBehaviour
{
	// Token: 0x06000832 RID: 2098 RVA: 0x0004F6D8 File Offset: 0x0004D8D8
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.intensity = this.lightComponent.intensity;
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0004F6F8 File Offset: 0x0004D8F8
	private void Update()
	{
		if (this.timer <= 0f)
		{
			this.timer = Random.Range(0.05f, 0.2f);
			this.intensityTarget = Random.Range(0.75f, 1f);
		}
		else
		{
			this.timer -= Time.deltaTime;
		}
		this.lightComponent.intensity = Mathf.Lerp(this.lightComponent.intensity, this.intensity * this.intensityTarget, Time.deltaTime * 30f);
	}

	// Token: 0x04000F14 RID: 3860
	private Light lightComponent;

	// Token: 0x04000F15 RID: 3861
	private float intensityTarget;

	// Token: 0x04000F16 RID: 3862
	private float timer;

	// Token: 0x04000F17 RID: 3863
	private float intensity;
}
