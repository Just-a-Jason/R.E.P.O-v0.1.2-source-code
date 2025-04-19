using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class LightInteractableFadeRemove : MonoBehaviour
{
	// Token: 0x060008B4 RID: 2228 RVA: 0x00053973 File Offset: 0x00051B73
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x00053984 File Offset: 0x00051B84
	private void Update()
	{
		if (this.isFading)
		{
			this.currentTime += Time.deltaTime;
			float time = this.currentTime / this.fadeDuration;
			this.lightComponent.intensity = this.fadeCurve.Evaluate(time) * this.lightComponent.intensity;
			if (this.currentTime >= this.fadeDuration)
			{
				Object.Destroy(this.lightComponent);
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x000539FB File Offset: 0x00051BFB
	public void StartFading()
	{
		this.isFading = true;
	}

	// Token: 0x04000FD2 RID: 4050
	public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04000FD3 RID: 4051
	public float fadeDuration = 2f;

	// Token: 0x04000FD4 RID: 4052
	private Light lightComponent;

	// Token: 0x04000FD5 RID: 4053
	private float currentTime;

	// Token: 0x04000FD6 RID: 4054
	private bool isFading;
}
