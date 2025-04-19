using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class DirtFinderCompleteDance : MonoBehaviour
{
	// Token: 0x06000CA9 RID: 3241 RVA: 0x0006F9E0 File Offset: 0x0006DBE0
	private void Update()
	{
		if (this.Active)
		{
			this.IntroLerp += this.IntroSpeed * Time.deltaTime;
			this.IntroLerp = Mathf.Clamp01(this.IntroLerp);
			if (this.DanceLerpX >= 1f)
			{
				this.DanceLerpX = 0f;
			}
			this.DanceLerpX += this.DanceSpeedX * Time.deltaTime;
			float num = this.DanceCurveX.Evaluate(this.DanceLerpX) * this.IntroCurve.Evaluate(this.IntroLerp);
			if (this.DanceLerpY >= 1f)
			{
				this.DanceLerpY = 0f;
			}
			this.DanceLerpY += this.DanceSpeedY * Time.deltaTime;
			float num2 = this.DanceCurveY.Evaluate(this.DanceLerpY) * this.IntroCurve.Evaluate(this.IntroLerp);
			if (this.DanceLerpZ >= 1f)
			{
				this.DanceLerpZ = 0f;
			}
			this.DanceLerpZ += this.DanceSpeedZ * Time.deltaTime;
			float num3 = this.DanceCurveZ.Evaluate(this.DanceLerpZ) * this.IntroCurve.Evaluate(this.IntroLerp);
			base.transform.localRotation = Quaternion.Euler(this.DanceAmountX * num, this.DanceAmountY * num2, this.DanceAmountZ * num3);
		}
	}

	// Token: 0x0400141B RID: 5147
	public bool Active;

	// Token: 0x0400141C RID: 5148
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x0400141D RID: 5149
	public float IntroSpeed;

	// Token: 0x0400141E RID: 5150
	private float IntroLerp;

	// Token: 0x0400141F RID: 5151
	[Space]
	public AnimationCurve DanceCurveX;

	// Token: 0x04001420 RID: 5152
	public float DanceSpeedX;

	// Token: 0x04001421 RID: 5153
	public float DanceAmountX;

	// Token: 0x04001422 RID: 5154
	private float DanceLerpX;

	// Token: 0x04001423 RID: 5155
	[Space]
	public AnimationCurve DanceCurveY;

	// Token: 0x04001424 RID: 5156
	public float DanceSpeedY;

	// Token: 0x04001425 RID: 5157
	public float DanceAmountY;

	// Token: 0x04001426 RID: 5158
	private float DanceLerpY;

	// Token: 0x04001427 RID: 5159
	[Space]
	public AnimationCurve DanceCurveZ;

	// Token: 0x04001428 RID: 5160
	public float DanceSpeedZ;

	// Token: 0x04001429 RID: 5161
	public float DanceAmountZ;

	// Token: 0x0400142A RID: 5162
	private float DanceLerpZ;
}
