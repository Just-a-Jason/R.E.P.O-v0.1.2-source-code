using System;
using TMPro;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class WorldSpaceUIValueLost : WorldSpaceUIChild
{
	// Token: 0x06001354 RID: 4948 RVA: 0x000A98AC File Offset: 0x000A7AAC
	protected override void Start()
	{
		base.Start();
		this.shakeXAmount = 0.005f;
		this.shakeYAmount = 0.005f;
		this.timer = 3f;
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.textColor = this.text.color;
		this.text.color = Color.white;
		this.text.text = "-$" + SemiFunc.DollarGetString(this.value);
		this.scale = base.transform.localScale;
		if (this.value < 1000)
		{
			this.scale *= 0.75f;
			base.transform.localScale = this.scale;
		}
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x000A9974 File Offset: 0x000A7B74
	protected override void Update()
	{
		base.Update();
		if (this.text.color != this.textColor)
		{
			this.flashTimer -= Time.deltaTime;
			if (this.flashTimer <= 0f && this.text.color != this.textColor)
			{
				this.text.color = Color.Lerp(this.text.color, this.textColor, 20f * Time.deltaTime);
				this.shakeX = Mathf.Lerp(this.shakeX, 0f, 20f * Time.deltaTime);
				this.shakeY = Mathf.Lerp(this.shakeY, 0f, 20f * Time.deltaTime);
			}
			else
			{
				if (this.shakeTimerX <= 0f)
				{
					this.shakeXTarget = Random.Range(-this.shakeXAmount, this.shakeXAmount);
					this.shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					this.shakeTimerX -= Time.deltaTime;
					this.shakeX = Mathf.Lerp(this.shakeX, this.shakeXTarget, 50f * Time.deltaTime);
				}
				if (this.shakeTimerX <= 0f)
				{
					this.shakeYTarget = Random.Range(-this.shakeYAmount, this.shakeYAmount);
					this.shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					this.shakeTimerX -= Time.deltaTime;
					this.shakeY = Mathf.Lerp(this.shakeY, this.shakeYTarget, 50f * Time.deltaTime);
				}
			}
		}
		this.floatY += 0.02f * Time.deltaTime;
		this.positionOffset = new Vector3(this.shakeX, this.shakeY + this.floatY, 0f);
		this.timer -= Time.deltaTime;
		if (this.timer > 0f)
		{
			this.curveLerp += 10f * Time.deltaTime;
			this.curveLerp = Mathf.Clamp01(this.curveLerp);
			base.transform.localScale = this.scale * this.curveIntro.Evaluate(this.curveLerp);
			return;
		}
		this.curveLerp -= 5f * Time.deltaTime;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		base.transform.localScale = this.scale * this.curveOutro.Evaluate(this.curveLerp);
		if (this.curveLerp <= 0f)
		{
			WorldSpaceUIParent.instance.valueLostList.Remove(this);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040020FB RID: 8443
	internal float timer;

	// Token: 0x040020FC RID: 8444
	private float flashTimer = 0.2f;

	// Token: 0x040020FD RID: 8445
	private Vector3 scale;

	// Token: 0x040020FE RID: 8446
	private TextMeshProUGUI text;

	// Token: 0x040020FF RID: 8447
	private Color textColor;

	// Token: 0x04002100 RID: 8448
	private float shakeXAmount;

	// Token: 0x04002101 RID: 8449
	private float shakeYAmount;

	// Token: 0x04002102 RID: 8450
	private float floatY;

	// Token: 0x04002103 RID: 8451
	private float shakeTimerX;

	// Token: 0x04002104 RID: 8452
	private float shakeXTarget;

	// Token: 0x04002105 RID: 8453
	private float shakeX;

	// Token: 0x04002106 RID: 8454
	private float shakeTimerY;

	// Token: 0x04002107 RID: 8455
	private float shakeYTarget;

	// Token: 0x04002108 RID: 8456
	private float shakeY;

	// Token: 0x04002109 RID: 8457
	public AnimationCurve curveIntro;

	// Token: 0x0400210A RID: 8458
	public AnimationCurve curveOutro;

	// Token: 0x0400210B RID: 8459
	private float curveLerp;

	// Token: 0x0400210C RID: 8460
	internal int value;
}
