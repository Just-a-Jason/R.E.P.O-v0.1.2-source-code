using System;
using TMPro;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class WorldSpaceUITTS : WorldSpaceUIChild
{
	// Token: 0x0600134D RID: 4941 RVA: 0x000A92B4 File Offset: 0x000A74B4
	private void Awake()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.text.color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, 0f);
		this.cameraMain = Camera.main;
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x000A9310 File Offset: 0x000A7510
	protected override void Update()
	{
		base.Update();
		if (this.alphaCheckTimer <= 0f)
		{
			this.textAlphaTarget = 1f;
			this.alphaCheckTimer = 0.1f;
			if (!SpectateCamera.instance || SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
			{
				float num = 5f;
				float num2 = 20f;
				float num3 = Vector3.Distance(this.cameraMain.transform.position, this.worldPosition);
				if (num3 > num)
				{
					num3 = Mathf.Clamp(num3, num, num2);
					this.textAlphaTarget = 1f - (num3 - num) / (num2 - num);
				}
				if (this.ttsVoice && this.ttsVoice.playerAvatar.voiceChat.lowPassLogicTTS.LowPass)
				{
					this.textAlphaTarget *= 0.5f;
				}
			}
		}
		else
		{
			this.alphaCheckTimer -= Time.deltaTime;
		}
		if (!this.followTransform || !this.ttsVoice || !this.ttsVoice.isSpeaking || !this.playerAvatar || this.playerAvatar.isDisabled)
		{
			this.textAlphaTarget = 0f;
			if (this.textAlpha < 0.01f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		this.textAlpha = Mathf.Lerp(this.textAlpha, this.textAlphaTarget, 30f * Time.deltaTime);
		if (!this.flashDone)
		{
			this.flashTimer -= Time.deltaTime;
			if (this.flashTimer <= 0f)
			{
				if (this.textColor != this.textColorTarget)
				{
					this.textColor = Color.Lerp(this.textColor, this.textColorTarget, 20f * Time.deltaTime);
				}
				else
				{
					this.flashDone = true;
				}
			}
		}
		this.text.color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, this.textAlpha);
		if (this.followTransform)
		{
			this.followPosition = Vector3.Lerp(this.followPosition, this.followTransform.position, 10f * Time.deltaTime);
		}
		this.worldPosition = this.followPosition + this.curveIntro.Evaluate(this.curveLerp) * Vector3.up * 0.025f;
		this.curveLerp += Time.deltaTime * 4f;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		if (this.ttsVoice && this.ttsVoice.currentWordTime != this.wordTime)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040020DC RID: 8412
	internal TextMeshProUGUI text;

	// Token: 0x040020DD RID: 8413
	internal float wordTime;

	// Token: 0x040020DE RID: 8414
	internal TTSVoice ttsVoice;

	// Token: 0x040020DF RID: 8415
	internal Transform followTransform;

	// Token: 0x040020E0 RID: 8416
	internal PlayerAvatar playerAvatar;

	// Token: 0x040020E1 RID: 8417
	private float flashTimer = 0.1f;

	// Token: 0x040020E2 RID: 8418
	private Color textColor = Color.yellow;

	// Token: 0x040020E3 RID: 8419
	private Color textColorTarget = Color.white;

	// Token: 0x040020E4 RID: 8420
	private bool flashDone;

	// Token: 0x040020E5 RID: 8421
	public AnimationCurve curveIntro;

	// Token: 0x040020E6 RID: 8422
	private float curveLerp;

	// Token: 0x040020E7 RID: 8423
	internal Vector3 followPosition;

	// Token: 0x040020E8 RID: 8424
	private float alphaCheckTimer;

	// Token: 0x040020E9 RID: 8425
	private float textAlphaTarget;

	// Token: 0x040020EA RID: 8426
	private float textAlpha;

	// Token: 0x040020EB RID: 8427
	private Camera cameraMain;
}
