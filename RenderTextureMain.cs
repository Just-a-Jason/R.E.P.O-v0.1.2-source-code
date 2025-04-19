using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000264 RID: 612
public class RenderTextureMain : MonoBehaviour
{
	// Token: 0x06001310 RID: 4880 RVA: 0x000A6F8A File Offset: 0x000A518A
	private void Awake()
	{
		RenderTextureMain.instance = this;
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x000A6F94 File Offset: 0x000A5194
	private void Start()
	{
		this.textureWidthOriginal = this.textureWidthSmall;
		this.textureHeightOriginal = this.textureHeightSmall;
		this.originalSize = base.transform.localScale;
		foreach (Camera item in Camera.main.GetComponentsInChildren<Camera>())
		{
			this.cameras.Add(item);
		}
		this.ResetResolution();
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x000A6FFC File Offset: 0x000A51FC
	private void Update()
	{
		if (this.shakeActive)
		{
			if (this.shakeXLerp >= 1f)
			{
				this.shakeXLerp = 0f;
				this.shakeXOld = this.shakeXNew;
				this.shakeXNew = Random.Range(-5f, 5f);
			}
			else
			{
				this.shakeXLerp += Time.deltaTime * 100f;
				this.shakeX = Mathf.Lerp(this.shakeXOld, this.shakeXNew, this.shakeCurve.Evaluate(this.shakeXLerp));
			}
			if (this.shakeYLerp >= 1f)
			{
				this.shakeYLerp = 0f;
				this.shakeYOld = this.shakeYNew;
				this.shakeYNew = Random.Range(-5f, 5f);
			}
			else
			{
				this.shakeYLerp += Time.deltaTime * 100f;
				this.shakeY = Mathf.Lerp(this.shakeYOld, this.shakeYNew, this.shakeCurve.Evaluate(this.shakeYLerp));
			}
			base.transform.localPosition = new Vector3(this.shakeX, this.shakeY, 0f);
			this.shakeTimer -= Time.deltaTime;
			if (this.shakeTimer <= 0f)
			{
				base.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.shakeActive = false;
			}
		}
		if (this.sizeResetTimer > 0f)
		{
			this.sizeResetTimer -= Time.deltaTime;
		}
		else if (base.transform.localScale != this.originalSize)
		{
			base.transform.localScale = this.originalSize;
		}
		if (this.textureResetTimer > 0f)
		{
			this.textureResetTimer -= Time.deltaTime;
		}
		else if (this.renderTexture.width != (int)this.textureWidthOriginal || this.renderTexture.height != (int)this.textureHeightOriginal)
		{
			this.ResetResolution();
		}
		if (this.overlayDisableTimer > 0f)
		{
			this.overlayDisableTimer -= Time.deltaTime;
			if (this.overlayDisableTimer <= 0f)
			{
				this.overlayRawImage.enabled = true;
			}
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x000A7241 File Offset: 0x000A5441
	public void Shake(float _time)
	{
		this.shakeActive = true;
		this.shakeTimer = _time;
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x000A7251 File Offset: 0x000A5451
	public void ChangeSize(float _width, float _height, float _time)
	{
		base.transform.localScale = new Vector3(_width, _height, 1f);
		this.sizeResetTimer = _time;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x000A7271 File Offset: 0x000A5471
	public void ChangeResolution(float _width, float _height, float _time)
	{
		this.textureWidth = _width;
		this.textureHeight = _height;
		this.SetRenderTexture();
		this.textureResetTimer = _time;
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x000A728E File Offset: 0x000A548E
	public void ResetResolution()
	{
		this.textureWidth = this.textureWidthOriginal;
		this.textureHeight = this.textureHeightOriginal;
		this.SetRenderTexture();
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x000A72B0 File Offset: 0x000A54B0
	private void SetRenderTexture()
	{
		this.renderTexture.Release();
		this.renderTexture.width = (int)this.textureWidth;
		this.renderTexture.height = (int)this.textureHeight;
		this.renderTexture.Create();
		this.cameras[0].targetTexture = this.renderTexture;
		foreach (Camera camera in this.cameras)
		{
			camera.enabled = false;
			camera.enabled = true;
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000A735C File Offset: 0x000A555C
	private void OnApplicationQuit()
	{
		this.textureWidth = this.textureWidthSmall;
		this.textureHeight = this.textureHeightSmall;
		this.SetRenderTexture();
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x000A737C File Offset: 0x000A557C
	public void OverlayDisable()
	{
		this.overlayRawImage.enabled = false;
		this.overlayDisableTimer = 0.5f;
	}

	// Token: 0x04002043 RID: 8259
	public static RenderTextureMain instance;

	// Token: 0x04002044 RID: 8260
	private List<Camera> cameras = new List<Camera>();

	// Token: 0x04002045 RID: 8261
	public RenderTexture renderTexture;

	// Token: 0x04002046 RID: 8262
	[Space]
	public float textureWidthSmall;

	// Token: 0x04002047 RID: 8263
	public float textureHeightSmall;

	// Token: 0x04002048 RID: 8264
	[Space]
	public float textureWidthMedium;

	// Token: 0x04002049 RID: 8265
	public float textureHeightMedium;

	// Token: 0x0400204A RID: 8266
	[Space]
	public float textureWidthLarge;

	// Token: 0x0400204B RID: 8267
	public float textureHeightLarge;

	// Token: 0x0400204C RID: 8268
	internal float textureWidthOriginal;

	// Token: 0x0400204D RID: 8269
	internal float textureHeightOriginal;

	// Token: 0x0400204E RID: 8270
	internal float textureWidth;

	// Token: 0x0400204F RID: 8271
	internal float textureHeight;

	// Token: 0x04002050 RID: 8272
	internal float textureResetTimer;

	// Token: 0x04002051 RID: 8273
	internal float sizeResetTimer;

	// Token: 0x04002052 RID: 8274
	[Space]
	public AnimationCurve shakeCurve;

	// Token: 0x04002053 RID: 8275
	private float shakeTimer;

	// Token: 0x04002054 RID: 8276
	private bool shakeActive;

	// Token: 0x04002055 RID: 8277
	private float shakeX;

	// Token: 0x04002056 RID: 8278
	private float shakeXOld;

	// Token: 0x04002057 RID: 8279
	private float shakeXNew;

	// Token: 0x04002058 RID: 8280
	private float shakeXLerp = 1f;

	// Token: 0x04002059 RID: 8281
	private float shakeY;

	// Token: 0x0400205A RID: 8282
	private float shakeYOld;

	// Token: 0x0400205B RID: 8283
	private float shakeYNew;

	// Token: 0x0400205C RID: 8284
	private float shakeYLerp = 1f;

	// Token: 0x0400205D RID: 8285
	private Vector3 originalSize;

	// Token: 0x0400205E RID: 8286
	public RawImage overlayRawImage;

	// Token: 0x0400205F RID: 8287
	private float overlayDisableTimer;
}
