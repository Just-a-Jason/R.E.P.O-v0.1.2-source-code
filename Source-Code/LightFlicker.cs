using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class LightFlicker : MonoBehaviour
{
	// Token: 0x06001199 RID: 4505 RVA: 0x0009C675 File Offset: 0x0009A875
	private void Start()
	{
		this.lightComp = base.GetComponent<Light>();
		this.intensityInit = this.lightComp.intensity;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0009C694 File Offset: 0x0009A894
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.intensityOld, this.intensityNew, this.intensityCurve.Evaluate(this.intensityLerp));
		this.intensityLerp += this.intensitySpeed * Time.deltaTime;
		if (this.intensityLerp >= 1f)
		{
			this.intensityOld = this.intensityNew;
			this.intensityNew = Random.Range(this.intensityAmountMin, this.intensityAmountMax);
			this.intensitySpeed = Random.Range(this.intensitySpeedMin, this.intensitySpeedMax);
			this.intensityLerp = 0f;
		}
		this.lightComp.intensity = this.intensityInit + num;
	}

	// Token: 0x04001D89 RID: 7561
	private Light lightComp;

	// Token: 0x04001D8A RID: 7562
	public AnimationCurve intensityCurve;

	// Token: 0x04001D8B RID: 7563
	public float intensityAmountMin;

	// Token: 0x04001D8C RID: 7564
	public float intensityAmountMax;

	// Token: 0x04001D8D RID: 7565
	public float intensitySpeedMin;

	// Token: 0x04001D8E RID: 7566
	public float intensitySpeedMax;

	// Token: 0x04001D8F RID: 7567
	private float intensityLerp = 1f;

	// Token: 0x04001D90 RID: 7568
	private float intensityNew;

	// Token: 0x04001D91 RID: 7569
	private float intensityOld;

	// Token: 0x04001D92 RID: 7570
	private float intensitySpeed;

	// Token: 0x04001D93 RID: 7571
	private float intensityInit;
}
