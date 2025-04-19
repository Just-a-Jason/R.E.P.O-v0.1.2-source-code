using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class DusterDusting : MonoBehaviour
{
	// Token: 0x06000700 RID: 1792 RVA: 0x0004205C File Offset: 0x0004025C
	private void Update()
	{
		if (this.Active)
		{
			GameDirector.instance.CameraShake.Shake(1f, 0.25f);
			if (this.ActivePrev != this.Active)
			{
				this.Start.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveAmount += 2f * Time.deltaTime;
		}
		else
		{
			if (this.ActivePrev != this.Active)
			{
				this.Stop.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveAmount -= 1.5f * Time.deltaTime;
		}
		this.ActiveAmount = Mathf.Clamp01(this.ActiveAmount);
		this.ActivePrev = this.Active;
		this.Loop.PlayLoop(this.Active, 2f, 2f, 1f);
		if (this.ActiveAmount > 0f)
		{
			if (!this.ReverseZ)
			{
				this.LerpZ += this.SpeedZ * Time.deltaTime;
				if (this.LerpZ >= 1f)
				{
					this.ReverseZ = true;
				}
			}
			else
			{
				this.LerpZ -= this.SpeedZ * Time.deltaTime;
				if (this.LerpZ <= 0f)
				{
					this.ReverseZ = false;
				}
			}
		}
		if (this.ActiveAmount > 0f)
		{
			if (!this.ReverseX)
			{
				this.LerpX += this.SpeedX * Time.deltaTime;
				if (this.LerpX >= 1f)
				{
					this.ReverseX = true;
				}
			}
			else
			{
				this.LerpX -= this.SpeedX * Time.deltaTime;
				if (this.LerpX <= 0f)
				{
					this.ReverseX = false;
				}
			}
		}
		if (this.ActiveAmount > 0f)
		{
			base.transform.localRotation = Quaternion.Euler((this.Curve.Evaluate(this.LerpX) * this.AmountX - this.AmountX * 0.5f) * this.Curve.Evaluate(this.ActiveAmount), 0f, (this.Curve.Evaluate(this.LerpZ) * this.AmountZ - this.AmountX * 0.5f) * this.Curve.Evaluate(this.ActiveAmount));
			return;
		}
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	// Token: 0x04000BE0 RID: 3040
	public bool Active;

	// Token: 0x04000BE1 RID: 3041
	private bool ActivePrev;

	// Token: 0x04000BE2 RID: 3042
	public float AmountZ;

	// Token: 0x04000BE3 RID: 3043
	public float SpeedZ;

	// Token: 0x04000BE4 RID: 3044
	private float LerpZ;

	// Token: 0x04000BE5 RID: 3045
	private bool ReverseZ;

	// Token: 0x04000BE6 RID: 3046
	[Space]
	public float AmountX;

	// Token: 0x04000BE7 RID: 3047
	public float SpeedX;

	// Token: 0x04000BE8 RID: 3048
	private float LerpX;

	// Token: 0x04000BE9 RID: 3049
	private bool ReverseX;

	// Token: 0x04000BEA RID: 3050
	[Space]
	public AnimationCurve Curve;

	// Token: 0x04000BEB RID: 3051
	public float ActiveAmount;

	// Token: 0x04000BEC RID: 3052
	[Header("Sounds")]
	[Space]
	public Sound Start;

	// Token: 0x04000BED RID: 3053
	public Sound Loop;

	// Token: 0x04000BEE RID: 3054
	public Sound Stop;
}
