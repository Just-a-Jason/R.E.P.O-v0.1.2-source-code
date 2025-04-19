using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026B RID: 619
public class VideoOverlay : MonoBehaviour
{
	// Token: 0x0600133A RID: 4922 RVA: 0x000A8A12 File Offset: 0x000A6C12
	private void Awake()
	{
		VideoOverlay.Instance = this;
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x000A8A1A File Offset: 0x000A6C1A
	public void Override(float time, float amount, float speed)
	{
		this.OverrideTimer = time;
		this.OverrideAmount = amount;
		this.OverrideSpeed = speed;
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x000A8A34 File Offset: 0x000A6C34
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.RawImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 5);
			return;
		}
		if ((GameDirector.instance.currentState == GameDirector.gameState.Start && LoadingUI.instance.levelAnimationCompleted) || GameDirector.instance.currentState == GameDirector.gameState.Outro)
		{
			this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 50), 20f * Time.deltaTime);
			return;
		}
		if (this.IntroLerp < 1f)
		{
			this.IntroLerp += Time.deltaTime * 0.5f;
			float num = this.IntroCurve.Evaluate(this.IntroLerp);
			this.RawImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(5f * num));
			return;
		}
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255f * this.OverrideAmount)), Time.deltaTime * this.OverrideSpeed);
			return;
		}
		float num2 = 0f;
		if (this.IdleTimer > 0f)
		{
			this.IdleTimer -= Time.deltaTime;
			num2 = this.IdleAlpha;
			if (!GraphicsManager.instance.glitchLoop || VideoGreenScreen.instance)
			{
				num2 = 0f;
			}
			if (this.IdleTimer <= 0f)
			{
				this.IdleCooldown = Random.Range(this.IdleCooldownMin, this.IdleCooldownMax);
			}
		}
		else if (this.IdleCooldown > 0f)
		{
			this.IdleCooldown -= Time.deltaTime;
		}
		else
		{
			this.IdleTimer = Random.Range(this.IdleTimeMin, this.IdleTimeMax);
		}
		this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(100f * num2)), Time.deltaTime * 1f);
	}

	// Token: 0x040020BB RID: 8379
	public static VideoOverlay Instance;

	// Token: 0x040020BC RID: 8380
	public RawImage RawImage;

	// Token: 0x040020BD RID: 8381
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x040020BE RID: 8382
	public float IntroSpeed;

	// Token: 0x040020BF RID: 8383
	private float IntroLerp;

	// Token: 0x040020C0 RID: 8384
	[Space]
	public float IdleAlpha;

	// Token: 0x040020C1 RID: 8385
	public float IdleCooldownMin;

	// Token: 0x040020C2 RID: 8386
	public float IdleCooldownMax;

	// Token: 0x040020C3 RID: 8387
	private float IdleCooldown;

	// Token: 0x040020C4 RID: 8388
	public float IdleTimeMin;

	// Token: 0x040020C5 RID: 8389
	public float IdleTimeMax;

	// Token: 0x040020C6 RID: 8390
	private float IdleTimer = 0.1f;

	// Token: 0x040020C7 RID: 8391
	private float OverrideTimer;

	// Token: 0x040020C8 RID: 8392
	private float OverrideAmount;

	// Token: 0x040020C9 RID: 8393
	private float OverrideSpeed;
}
