using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class CameraShake : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x000063C8 File Offset: 0x000045C8
	public void Shake(float strengthAdd, float time)
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (strengthAdd > this.Strength)
		{
			strengthAdd = this.ShakeMultiplier(strengthAdd);
			this.Strength += strengthAdd;
			this.Strength = Mathf.Min(this.Strength, this.StrengthMax);
			this.StrengthLossDelay = Mathf.Max(time, this.StrengthLossDelay);
			this.SetInstant();
		}
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00006434 File Offset: 0x00004634
	public void ShakeDistance(float strength, float distanceMin, float distanceMax, Vector3 position, float time)
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		float value = Vector3.Distance(base.transform.position, position);
		float num = Mathf.InverseLerp(distanceMin, distanceMax, value);
		float num2 = strength * (1f - num);
		if (num2 > this.Strength)
		{
			num2 = this.ShakeMultiplier(num2);
			this.Strength += num2;
			this.Strength = Mathf.Min(this.Strength, this.StrengthMax);
			this.StrengthLossDelay = Mathf.Max(time, this.StrengthLossDelay);
			this.SetInstant();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x000064C4 File Offset: 0x000046C4
	private float ShakeMultiplier(float _strength)
	{
		_strength *= GameplayManager.instance.cameraShake;
		return _strength;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000064D8 File Offset: 0x000046D8
	public void SetInstant()
	{
		if (!this.InstantShake)
		{
			return;
		}
		this.RotXNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
		this.RotXLerp = 1f;
		this.RotYNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
		this.RotYLerp = 1f;
		this.RotZNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
		this.RotZLerp = 1f;
		this.PosXNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
		this.PosXLerp = 1f;
		this.PosYNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
		this.PosYLerp = 1f;
		this.PosZNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
		this.PosZLerp = 1f;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00006604 File Offset: 0x00004804
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.RotXOld, this.RotXNew, this.Curve.Evaluate(this.RotXLerp));
		this.RotXLerp += this.RotXSpeed * this.Speed * Time.deltaTime;
		if (this.RotXLerp >= 1f)
		{
			this.RotXOld = num;
			this.RotXNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
			this.RotXSpeed = Random.Range(0.8f, 1.2f);
			this.RotXLerp = 0f;
		}
		float num2 = Mathf.LerpUnclamped(this.RotYOld, this.RotYNew, this.Curve.Evaluate(this.RotYLerp));
		this.RotYLerp += this.RotYSpeed * this.Speed * Time.deltaTime;
		if (this.RotYLerp >= 1f)
		{
			this.RotYOld = num2;
			this.RotYNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
			this.RotYSpeed = Random.Range(0.8f, 1.2f);
			this.RotYLerp = 0f;
		}
		float num3 = Mathf.LerpUnclamped(this.RotZOld, this.RotZNew, this.Curve.Evaluate(this.RotZLerp));
		this.RotZLerp += this.RotZSpeed * this.Speed * Time.deltaTime;
		if (this.RotZLerp >= 1f)
		{
			this.RotZOld = num3;
			this.RotZNew = Random.Range(-1f, 1f) * this.RotationMultiplier * this.Strength;
			this.RotZSpeed = Random.Range(0.8f, 1.2f);
			this.RotZLerp = 0f;
		}
		float num4 = Mathf.LerpUnclamped(this.PosXOld, this.PosXNew, this.Curve.Evaluate(this.PosXLerp));
		this.PosXLerp += this.PosXSpeed * this.Speed * Time.deltaTime;
		if (this.PosXLerp >= 1f)
		{
			this.PosXOld = num4;
			this.PosXNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
			this.PosXSpeed = Random.Range(0.8f, 1.2f);
			this.PosXLerp = 0f;
		}
		float num5 = Mathf.LerpUnclamped(this.PosYOld, this.PosYNew, this.Curve.Evaluate(this.PosYLerp));
		this.PosYLerp += this.PosYSpeed * this.Speed * Time.deltaTime;
		if (this.PosYLerp >= 1f)
		{
			this.PosYOld = num5;
			this.PosYNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
			this.PosYSpeed = Random.Range(0.8f, 1.2f);
			this.PosYLerp = 0f;
		}
		float num6 = Mathf.LerpUnclamped(this.PosZOld, this.PosZNew, this.Curve.Evaluate(this.PosZLerp));
		this.PosZLerp += this.PosZSpeed * this.Speed * Time.deltaTime;
		if (this.PosZLerp >= 1f)
		{
			this.PosZOld = num6;
			this.PosZNew = Random.Range(-1f, 1f) * this.PositionMultiplier * this.Strength;
			this.PosZSpeed = Random.Range(0.8f, 1.2f);
			this.PosZLerp = 0f;
		}
		base.transform.localPosition = new Vector3(num4, num5, num6);
		base.transform.localRotation = Quaternion.Euler(num, num2, num3);
		if (this.StrengthLossDelay <= 0f)
		{
			if (this.Strength > 0f)
			{
				this.Strength -= this.StrengthLoss * Time.deltaTime;
				if (this.Strength <= 0.1f)
				{
					this.Strength = 0f;
					return;
				}
			}
		}
		else
		{
			this.StrengthLossDelay -= 1f * Time.deltaTime;
		}
	}

	// Token: 0x04000191 RID: 401
	public AnimationCurve Curve;

	// Token: 0x04000192 RID: 402
	[Space]
	public bool InstantShake;

	// Token: 0x04000193 RID: 403
	[Space]
	public float Strength;

	// Token: 0x04000194 RID: 404
	public float StrengthMax = 1f;

	// Token: 0x04000195 RID: 405
	public float StrengthLoss = 1f;

	// Token: 0x04000196 RID: 406
	public float StrengthLossDelay;

	// Token: 0x04000197 RID: 407
	[Space]
	public float Speed = 1f;

	// Token: 0x04000198 RID: 408
	[Space]
	public float RotationMultiplier = 1f;

	// Token: 0x04000199 RID: 409
	public float PositionMultiplier = 1f;

	// Token: 0x0400019A RID: 410
	private float RotXLerp = 1f;

	// Token: 0x0400019B RID: 411
	private float RotXNew;

	// Token: 0x0400019C RID: 412
	private float RotXOld;

	// Token: 0x0400019D RID: 413
	private float RotXSpeed;

	// Token: 0x0400019E RID: 414
	private float RotYLerp = 1f;

	// Token: 0x0400019F RID: 415
	private float RotYNew;

	// Token: 0x040001A0 RID: 416
	private float RotYOld;

	// Token: 0x040001A1 RID: 417
	private float RotYSpeed;

	// Token: 0x040001A2 RID: 418
	private float RotZLerp = 1f;

	// Token: 0x040001A3 RID: 419
	private float RotZNew;

	// Token: 0x040001A4 RID: 420
	private float RotZOld;

	// Token: 0x040001A5 RID: 421
	private float RotZSpeed;

	// Token: 0x040001A6 RID: 422
	private float PosXLerp = 1f;

	// Token: 0x040001A7 RID: 423
	private float PosXNew;

	// Token: 0x040001A8 RID: 424
	private float PosXOld;

	// Token: 0x040001A9 RID: 425
	private float PosXSpeed;

	// Token: 0x040001AA RID: 426
	private float PosYLerp = 1f;

	// Token: 0x040001AB RID: 427
	private float PosYNew;

	// Token: 0x040001AC RID: 428
	private float PosYOld;

	// Token: 0x040001AD RID: 429
	private float PosYSpeed;

	// Token: 0x040001AE RID: 430
	private float PosZLerp = 1f;

	// Token: 0x040001AF RID: 431
	private float PosZNew;

	// Token: 0x040001B0 RID: 432
	private float PosZOld;

	// Token: 0x040001B1 RID: 433
	private float PosZSpeed;
}
