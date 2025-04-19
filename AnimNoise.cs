using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class AnimNoise : MonoBehaviour
{
	// Token: 0x06001135 RID: 4405 RVA: 0x00099614 File Offset: 0x00097814
	public void NoiseOverride(float time, float speed, float strength, float introSpeed, float outroSpeed)
	{
		this.noiseOverrideTimer = time;
		this.noiseOverrideSpeed = Mathf.Max(speed, speed * this.noiseOverrideMultSpeed);
		this.noiseOverrideStrength = Mathf.Max(strength, strength * this.noiseOverrideMultStrength);
		this.noiseOverrideIntroSpeed = introSpeed;
		this.noiseOverrideOutroSpeed = outroSpeed;
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x00099660 File Offset: 0x00097860
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.noiseRotXOld, this.noiseRotXNew, this.noiseCurve.Evaluate(this.noiseRotXLerp));
		this.noiseRotXLerp += this.noiseRotXSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotXLerp >= 1f)
		{
			this.noiseRotXOld = this.noiseRotXNew;
			this.noiseRotXNew = Random.Range(this.noiseRotXAmountMin, this.noiseRotXAmountMax) * this.noiseStrength;
			this.noiseRotXSpeed = Random.Range(this.noiseRotXSpeedMin, this.noiseRotXSpeedMax);
			this.noiseRotXLerp = 0f;
		}
		float num2 = Mathf.LerpUnclamped(this.noiseRotYOld, this.noiseRotYNew, this.noiseCurve.Evaluate(this.noiseRotYLerp));
		this.noiseRotYLerp += this.noiseRotYSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotYLerp >= 1f)
		{
			this.noiseRotYOld = this.noiseRotYNew;
			this.noiseRotYNew = Random.Range(this.noiseRotYAmountMin, this.noiseRotYAmountMax) * this.noiseStrength;
			this.noiseRotYSpeed = Random.Range(this.noiseRotYSpeedMin, this.noiseRotYSpeedMax);
			this.noiseRotYLerp = 0f;
		}
		float num3 = Mathf.LerpUnclamped(this.noiseRotZOld, this.noiseRotZNew, this.noiseCurve.Evaluate(this.noiseRotZLerp));
		this.noiseRotZLerp += this.noiseRotZSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotZLerp >= 1f)
		{
			this.noiseRotZOld = this.noiseRotZNew;
			this.noiseRotZNew = Random.Range(this.noiseRotZAmountMin, this.noiseRotZAmountMax) * this.noiseStrength;
			this.noiseRotZSpeed = Random.Range(this.noiseRotZSpeedMin, this.noiseRotZSpeedMax);
			this.noiseRotZLerp = 0f;
		}
		float num4 = Mathf.LerpUnclamped(this.noisePosXOld, this.noisePosXNew, this.noiseCurve.Evaluate(this.noisePosXLerp));
		this.noisePosXLerp += this.noisePosXSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosXLerp >= 1f)
		{
			this.noisePosXOld = this.noisePosXNew;
			this.noisePosXNew = Random.Range(this.noisePosXAmountMin, this.noisePosXAmountMax) * this.noiseStrength;
			this.noisePosXSpeed = Random.Range(this.noisePosXSpeedMin, this.noisePosXSpeedMax);
			this.noisePosXLerp = 0f;
		}
		float num5 = Mathf.LerpUnclamped(this.noisePosYOld, this.noisePosYNew, this.noiseCurve.Evaluate(this.noisePosYLerp));
		this.noisePosYLerp += this.noisePosYSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosYLerp >= 1f)
		{
			this.noisePosYOld = this.noisePosYNew;
			this.noisePosYNew = Random.Range(this.noisePosYAmountMin, this.noisePosYAmountMax) * this.noiseStrength;
			this.noisePosYSpeed = Random.Range(this.noisePosYSpeedMin, this.noisePosYSpeedMax);
			this.noisePosYLerp = 0f;
		}
		float num6 = Mathf.LerpUnclamped(this.noisePosZOld, this.noisePosZNew, this.noiseCurve.Evaluate(this.noisePosZLerp));
		this.noisePosZLerp += this.noisePosZSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosZLerp >= 1f)
		{
			this.noisePosZOld = this.noisePosZNew;
			this.noisePosZNew = Random.Range(this.noisePosZAmountMin, this.noisePosZAmountMax) * this.noiseStrength;
			this.noisePosZSpeed = Random.Range(this.noisePosZSpeedMin, this.noisePosZSpeedMax);
			this.noisePosZLerp = 0f;
		}
		if (this.noiseOverrideTimer > 0f)
		{
			this.noiseOverrideLerp = Mathf.Clamp01(this.noiseOverrideLerp + this.noiseOverrideIntroSpeed * Time.deltaTime);
			this.noiseOverrideTimer -= Time.deltaTime;
		}
		else
		{
			this.noiseOverrideLerp = Mathf.Clamp01(this.noiseOverrideLerp - this.noiseOverrideOutroSpeed * Time.deltaTime);
		}
		this.noiseStrength = Mathf.Lerp(this.noiseStrengthDefault, this.noiseOverrideStrength, this.noiseOverrideCurve.Evaluate(this.noiseOverrideLerp));
		this.noiseSpeed = Mathf.Lerp(this.noiseSpeedDefault, this.noiseOverrideSpeed, this.noiseOverrideCurve.Evaluate(this.noiseOverrideLerp));
		base.transform.localPosition = new Vector3(num4 * this.MasterAmount, num5 * this.MasterAmount, num6 * this.MasterAmount);
		base.transform.localRotation = Quaternion.Euler(num * this.MasterAmount, num2 * this.MasterAmount, num3 * this.MasterAmount);
	}

	// Token: 0x04001C7A RID: 7290
	public AnimationCurve noiseCurve;

	// Token: 0x04001C7B RID: 7291
	public AnimationCurve noiseOverrideCurve;

	// Token: 0x04001C7C RID: 7292
	public float noiseStrengthDefault = 1f;

	// Token: 0x04001C7D RID: 7293
	public float noiseSpeedDefault = 1f;

	// Token: 0x04001C7E RID: 7294
	private float noiseStrength = 1f;

	// Token: 0x04001C7F RID: 7295
	private float noiseSpeed = 1f;

	// Token: 0x04001C80 RID: 7296
	[HideInInspector]
	public float MasterAmount = 1f;

	// Token: 0x04001C81 RID: 7297
	[Header("Override Multipliers")]
	public float noiseOverrideMultStrength = 1f;

	// Token: 0x04001C82 RID: 7298
	public float noiseOverrideMultSpeed = 1f;

	// Token: 0x04001C83 RID: 7299
	private float noiseOverrideLerp;

	// Token: 0x04001C84 RID: 7300
	private float noiseOverrideTimer;

	// Token: 0x04001C85 RID: 7301
	private float noiseOverrideStrength;

	// Token: 0x04001C86 RID: 7302
	private float noiseOverrideSpeed;

	// Token: 0x04001C87 RID: 7303
	private float noiseOverrideIntroSpeed;

	// Token: 0x04001C88 RID: 7304
	private float noiseOverrideOutroSpeed;

	// Token: 0x04001C89 RID: 7305
	[Header("Rotation X")]
	public float noiseRotXAmountMin;

	// Token: 0x04001C8A RID: 7306
	public float noiseRotXAmountMax;

	// Token: 0x04001C8B RID: 7307
	public float noiseRotXSpeedMin;

	// Token: 0x04001C8C RID: 7308
	public float noiseRotXSpeedMax;

	// Token: 0x04001C8D RID: 7309
	private float noiseRotXLerp = 1f;

	// Token: 0x04001C8E RID: 7310
	private float noiseRotXNew;

	// Token: 0x04001C8F RID: 7311
	private float noiseRotXOld;

	// Token: 0x04001C90 RID: 7312
	private float noiseRotXSpeed;

	// Token: 0x04001C91 RID: 7313
	[Header("Rotation Y")]
	public float noiseRotYAmountMin;

	// Token: 0x04001C92 RID: 7314
	public float noiseRotYAmountMax;

	// Token: 0x04001C93 RID: 7315
	public float noiseRotYSpeedMin;

	// Token: 0x04001C94 RID: 7316
	public float noiseRotYSpeedMax;

	// Token: 0x04001C95 RID: 7317
	private float noiseRotYLerp = 1f;

	// Token: 0x04001C96 RID: 7318
	private float noiseRotYNew;

	// Token: 0x04001C97 RID: 7319
	private float noiseRotYOld;

	// Token: 0x04001C98 RID: 7320
	private float noiseRotYSpeed;

	// Token: 0x04001C99 RID: 7321
	[Header("Rotation Z")]
	public float noiseRotZAmountMin;

	// Token: 0x04001C9A RID: 7322
	public float noiseRotZAmountMax;

	// Token: 0x04001C9B RID: 7323
	public float noiseRotZSpeedMin;

	// Token: 0x04001C9C RID: 7324
	public float noiseRotZSpeedMax;

	// Token: 0x04001C9D RID: 7325
	private float noiseRotZLerp = 1f;

	// Token: 0x04001C9E RID: 7326
	private float noiseRotZNew;

	// Token: 0x04001C9F RID: 7327
	private float noiseRotZOld;

	// Token: 0x04001CA0 RID: 7328
	private float noiseRotZSpeed;

	// Token: 0x04001CA1 RID: 7329
	[Header("Position X")]
	public float noisePosXAmountMin;

	// Token: 0x04001CA2 RID: 7330
	public float noisePosXAmountMax;

	// Token: 0x04001CA3 RID: 7331
	public float noisePosXSpeedMin;

	// Token: 0x04001CA4 RID: 7332
	public float noisePosXSpeedMax;

	// Token: 0x04001CA5 RID: 7333
	private float noisePosXLerp = 1f;

	// Token: 0x04001CA6 RID: 7334
	private float noisePosXNew;

	// Token: 0x04001CA7 RID: 7335
	private float noisePosXOld;

	// Token: 0x04001CA8 RID: 7336
	private float noisePosXSpeed;

	// Token: 0x04001CA9 RID: 7337
	[Header("Position Y")]
	public float noisePosYAmountMin;

	// Token: 0x04001CAA RID: 7338
	public float noisePosYAmountMax;

	// Token: 0x04001CAB RID: 7339
	public float noisePosYSpeedMin;

	// Token: 0x04001CAC RID: 7340
	public float noisePosYSpeedMax;

	// Token: 0x04001CAD RID: 7341
	private float noisePosYLerp = 1f;

	// Token: 0x04001CAE RID: 7342
	private float noisePosYNew;

	// Token: 0x04001CAF RID: 7343
	private float noisePosYOld;

	// Token: 0x04001CB0 RID: 7344
	private float noisePosYSpeed;

	// Token: 0x04001CB1 RID: 7345
	[Header("Position Z")]
	public float noisePosZAmountMin;

	// Token: 0x04001CB2 RID: 7346
	public float noisePosZAmountMax;

	// Token: 0x04001CB3 RID: 7347
	public float noisePosZSpeedMin;

	// Token: 0x04001CB4 RID: 7348
	public float noisePosZSpeedMax;

	// Token: 0x04001CB5 RID: 7349
	private float noisePosZLerp = 1f;

	// Token: 0x04001CB6 RID: 7350
	private float noisePosZNew;

	// Token: 0x04001CB7 RID: 7351
	private float noisePosZOld;

	// Token: 0x04001CB8 RID: 7352
	private float noisePosZSpeed;
}
