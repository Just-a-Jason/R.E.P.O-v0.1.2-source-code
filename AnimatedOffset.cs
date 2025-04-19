using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class AnimatedOffset : MonoBehaviour
{
	// Token: 0x06000909 RID: 2313 RVA: 0x000562A6 File Offset: 0x000544A6
	public void Active(float time)
	{
		this.ActiveTimer = time;
		if (!this.Animating)
		{
			this.Animating = true;
			base.StartCoroutine(this.Animate());
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000562CB File Offset: 0x000544CB
	private IEnumerator Animate()
	{
		while (this.Animating)
		{
			if (this.ActiveTimer > 0f)
			{
				if (this.IntroLerp == 0f)
				{
					this.IntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.IntroLerp = Mathf.Clamp01(this.IntroLerp + this.IntroSpeed * Time.deltaTime);
				base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.PositionOffset, this.IntroCurve.Evaluate(this.IntroLerp));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(this.RotationOffset), this.IntroCurve.Evaluate(this.IntroLerp));
				if (this.IntroLerp >= 1f)
				{
					this.ActiveTimer -= Time.deltaTime;
				}
				this.OutroLerp = 0f;
			}
			else
			{
				if (this.OutroLerp == 0f)
				{
					this.OutroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.OutroLerp = Mathf.Clamp01(this.OutroLerp + this.OutroSpeed * Time.deltaTime);
				base.transform.localPosition = Vector3.Lerp(this.PositionOffset, Vector3.zero, this.OutroCurve.Evaluate(this.OutroLerp));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.RotationOffset), Quaternion.identity, this.OutroCurve.Evaluate(this.OutroLerp));
				if (this.OutroLerp >= 1f)
				{
					this.Animating = false;
				}
				this.IntroLerp = 0f;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400106F RID: 4207
	internal bool Animating;

	// Token: 0x04001070 RID: 4208
	private float ActiveTimer;

	// Token: 0x04001071 RID: 4209
	public Vector3 PositionOffset;

	// Token: 0x04001072 RID: 4210
	public Vector3 RotationOffset;

	// Token: 0x04001073 RID: 4211
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04001074 RID: 4212
	public float IntroSpeed;

	// Token: 0x04001075 RID: 4213
	public Sound IntroSound;

	// Token: 0x04001076 RID: 4214
	private float IntroLerp;

	// Token: 0x04001077 RID: 4215
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04001078 RID: 4216
	public float OutroSpeed;

	// Token: 0x04001079 RID: 4217
	public Sound OutroSound;

	// Token: 0x0400107A RID: 4218
	private float OutroLerp;
}
