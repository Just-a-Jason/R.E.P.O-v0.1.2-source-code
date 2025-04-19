using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class SledgehammerTransition : MonoBehaviour
{
	// Token: 0x0600071D RID: 1821 RVA: 0x0004384C File Offset: 0x00041A4C
	public void IntroSet()
	{
		this.Intro = true;
		this.LerpAmount = 0f;
		this.PositionStart = this.SwingTarget.position;
		this.RotationStart = this.SwingTarget.rotation;
		this.ScaleStart = this.SwingTarget.localScale;
		base.transform.position = this.PositionStart;
		base.transform.rotation = this.RotationStart;
		base.transform.localScale = this.ScaleStart;
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x000438D4 File Offset: 0x00041AD4
	public void OutroSet()
	{
		this.Intro = false;
		this.LerpAmount = 0f;
		this.PositionStart = this.HitTarget.position;
		this.RotationStart = this.HitTarget.rotation;
		this.ScaleStart = this.HitTarget.localScale;
		base.transform.position = this.PositionStart;
		base.transform.rotation = this.RotationStart;
		base.transform.localScale = this.ScaleStart;
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0004395C File Offset: 0x00041B5C
	public void Update()
	{
		if (this.LerpAmount < 1f)
		{
			if (this.Intro)
			{
				this.LerpAmount += this.IntroSpeed * Time.deltaTime;
				base.transform.position = Vector3.Lerp(this.PositionStart, this.HitTarget.position, this.IntroCurve.Evaluate(this.LerpAmount));
				base.transform.rotation = Quaternion.Lerp(this.RotationStart, this.HitTarget.rotation, this.IntroCurve.Evaluate(this.LerpAmount));
				base.transform.localScale = Vector3.Lerp(this.ScaleStart, this.HitTarget.localScale, this.IntroCurve.Evaluate(this.LerpAmount));
			}
			else
			{
				this.LerpAmount += this.OutroSpeed * Time.deltaTime;
				base.transform.position = Vector3.Lerp(this.PositionStart, this.SwingTarget.position, this.OutroCurve.Evaluate(this.LerpAmount));
				base.transform.rotation = Quaternion.Lerp(this.RotationStart, this.SwingTarget.rotation, this.OutroCurve.Evaluate(this.LerpAmount));
				base.transform.localScale = Vector3.Lerp(this.ScaleStart, this.SwingTarget.localScale, this.OutroCurve.Evaluate(this.LerpAmount));
			}
			if (this.LerpAmount >= 1f)
			{
				if (this.Intro)
				{
					this.Controller.IntroDone();
				}
				else
				{
					this.Controller.OutroDone();
				}
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04000C4E RID: 3150
	public SledgehammerController Controller;

	// Token: 0x04000C4F RID: 3151
	private Vector3 PositionStart;

	// Token: 0x04000C50 RID: 3152
	private Quaternion RotationStart;

	// Token: 0x04000C51 RID: 3153
	private Vector3 ScaleStart;

	// Token: 0x04000C52 RID: 3154
	[Space]
	public Transform SwingTarget;

	// Token: 0x04000C53 RID: 3155
	public Transform HitTarget;

	// Token: 0x04000C54 RID: 3156
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04000C55 RID: 3157
	public float IntroSpeed = 1f;

	// Token: 0x04000C56 RID: 3158
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C57 RID: 3159
	public float OutroSpeed = 1f;

	// Token: 0x04000C58 RID: 3160
	private float LerpAmount;

	// Token: 0x04000C59 RID: 3161
	private bool Intro;
}
