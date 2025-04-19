using System;
using TMPro;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class DirtFinderMapComplete : MonoBehaviour
{
	// Token: 0x06000CB8 RID: 3256 RVA: 0x00070012 File Offset: 0x0006E212
	private void Start()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.25f);
		GameDirector.instance.CameraShake.Shake(1f, 0.25f);
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00070048 File Offset: 0x0006E248
	private void Update()
	{
		if (this.FlashLerp < 1f)
		{
			this.FlashLerp += this.FlashSpeed * Time.deltaTime;
			this.FlashRenderer.color = Color.Lerp(this.FlashRenderer.color, new Color(255f, 255f, 255f, 0f), this.FlashCurve.Evaluate(this.FlashLerp));
			if (this.FlashLerp >= 1f)
			{
				this.FlashLerp = 1f;
				this.FlashRenderer.transform.gameObject.SetActive(false);
			}
		}
		this.TextDilate += 5f * this.TextDilateIncrease * Time.deltaTime;
		if (this.TextDilate >= 1f)
		{
			this.TextDilate = 1f;
			if (this.TextDilateWait > 0f)
			{
				this.TextDilateWait -= Time.deltaTime;
			}
			else
			{
				this.TextDilateWait = 0.5f;
				this.TextDilateIncrease = -1f;
			}
		}
		else if (this.TextDilate <= -1f)
		{
			this.TextDilate = -1f;
			if (this.TextDilateWait > 0f)
			{
				this.TextDilateWait -= Time.deltaTime;
			}
			else
			{
				this.TextDilateWait = 0.5f;
				this.TextDilateIncrease = 1f;
			}
		}
		this.TextTop.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, this.TextDilate);
		this.TextBot.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, this.TextDilate);
		this.CompleteTime -= Time.deltaTime;
		if (this.CompleteTime <= 0f)
		{
			this.CompleteTime = 0f;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0400144F RID: 5199
	public SpriteRenderer FlashRenderer;

	// Token: 0x04001450 RID: 5200
	public AnimationCurve FlashCurve;

	// Token: 0x04001451 RID: 5201
	public float FlashSpeed;

	// Token: 0x04001452 RID: 5202
	private float FlashLerp;

	// Token: 0x04001453 RID: 5203
	[Space]
	public TextMeshPro TextTop;

	// Token: 0x04001454 RID: 5204
	public TextMeshPro TextBot;

	// Token: 0x04001455 RID: 5205
	private float TextDilate;

	// Token: 0x04001456 RID: 5206
	private float TextDilateWait;

	// Token: 0x04001457 RID: 5207
	private float TextDilateIncrease = 1f;

	// Token: 0x04001458 RID: 5208
	[Space]
	public float CompleteTime;
}
