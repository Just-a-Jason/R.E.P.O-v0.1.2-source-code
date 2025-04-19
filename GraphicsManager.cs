using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class GraphicsManager : MonoBehaviour
{
	// Token: 0x06000958 RID: 2392 RVA: 0x0005722A File Offset: 0x0005542A
	private void Awake()
	{
		GraphicsManager.instance = this;
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00057234 File Offset: 0x00055434
	private void Update()
	{
		if (this.firstSetupTimer > 0f)
		{
			this.firstSetupTimer -= Time.deltaTime;
			if (this.firstSetupTimer <= 0f)
			{
				this.UpdateAll();
			}
			return;
		}
		if (this.fullscreenCheckTimer <= 0f)
		{
			this.fullscreenCheckTimer = 1f;
			if (Screen.fullScreenMode != this.windowMode || Screen.fullScreen != this.windowFullscreen)
			{
				if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 0);
				}
				else if (Screen.fullScreenMode == FullScreenMode.Windowed)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 1);
				}
				this.UpdateWindowMode(false);
				if (GraphicsButtonWindowMode.instance)
				{
					GraphicsButtonWindowMode.instance.UpdateSlider();
					return;
				}
			}
		}
		else
		{
			this.fullscreenCheckTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00057304 File Offset: 0x00055504
	public void UpdateAll()
	{
		this.UpdateVsync();
		this.UpdateMaxFPS();
		this.UpdateLightDistance();
		this.UpdateShadowQuality();
		this.UpdateShadowDistance();
		this.UpdateMotionBlur();
		this.UpdateLensDistortion();
		this.UpdateBloom();
		this.UpdateChromaticAberration();
		this.UpdateGrain();
		this.UpdateWindowMode(false);
		this.UpdateRenderSize();
		this.UpdateGlitchLoop();
		this.UpdateGamma();
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00057366 File Offset: 0x00055566
	public void UpdateVsync()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Vsync) == 1)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00057384 File Offset: 0x00055584
	public void UpdateMaxFPS()
	{
		Application.targetFrameRate = DataDirector.instance.SettingValueFetch(DataDirector.Setting.MaxFPS);
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00057398 File Offset: 0x00055598
	public void UpdateLightDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LightDistance))
		{
		case 0:
			this.lightDistance = 10f;
			break;
		case 1:
			this.lightDistance = 15f;
			break;
		case 2:
			this.lightDistance = 20f;
			break;
		case 3:
			this.lightDistance = 25f;
			break;
		case 4:
			this.lightDistance = 30f;
			break;
		}
		LightManager.instance.UpdateInstant();
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x00057418 File Offset: 0x00055618
	public void UpdateShadowQuality()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowQuality))
		{
		case 0:
			QualitySettings.shadowResolution = ShadowResolution.Low;
			return;
		case 1:
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 2:
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 3:
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00057464 File Offset: 0x00055664
	public void UpdateShadowDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowDistance))
		{
		case 0:
			this.shadowDistance = 5f;
			break;
		case 1:
			this.shadowDistance = 10f;
			break;
		case 2:
			this.shadowDistance = 15f;
			break;
		case 3:
			this.shadowDistance = 20f;
			break;
		case 4:
			this.shadowDistance = 25f;
			break;
		}
		QualitySettings.shadowDistance = this.shadowDistance;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x000574E4 File Offset: 0x000556E4
	public void UpdateMotionBlur()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.MotionBlur) == 1)
		{
			PostProcessing.Instance.motionBlur.active = true;
			return;
		}
		PostProcessing.Instance.motionBlur.active = false;
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00057516 File Offset: 0x00055716
	public void UpdateLensDistortion()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LensEffect) == 1)
		{
			PostProcessing.Instance.lensDistortion.active = true;
			return;
		}
		PostProcessing.Instance.lensDistortion.active = false;
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00057548 File Offset: 0x00055748
	public void UpdateBloom()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
		{
			PostProcessing.Instance.bloom.active = true;
			return;
		}
		PostProcessing.Instance.bloom.active = false;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0005757A File Offset: 0x0005577A
	public void UpdateChromaticAberration()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ChromaticAberration) == 1)
		{
			PostProcessing.Instance.chromaticAberration.active = true;
			return;
		}
		PostProcessing.Instance.chromaticAberration.active = false;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x000575AC File Offset: 0x000557AC
	public void UpdateGrain()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
		{
			PostProcessing.Instance.grain.active = true;
			return;
		}
		PostProcessing.Instance.grain.active = false;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x000575E0 File Offset: 0x000557E0
	public void UpdateWindowMode(bool _setResolution)
	{
		int num = DataDirector.instance.SettingValueFetch(DataDirector.Setting.WindowMode);
		if (num != 0)
		{
			if (num == 1)
			{
				this.windowMode = FullScreenMode.Windowed;
				this.windowFullscreen = false;
				if (_setResolution)
				{
					List<Resolution> list = new List<Resolution>();
					foreach (Resolution item in Screen.resolutions)
					{
						if ((float)item.width / (float)item.height == 1.7777778f)
						{
							list.Add(item);
						}
					}
					Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
					if (list.Count > 0)
					{
						resolution = list[list.Count / 2];
					}
					Screen.SetResolution(resolution.width, resolution.height, this.windowFullscreen);
				}
			}
		}
		else
		{
			this.windowMode = FullScreenMode.FullScreenWindow;
			this.windowFullscreen = true;
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, this.windowFullscreen);
		}
		this.fullscreenCheckTimer = 1f;
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000576F0 File Offset: 0x000558F0
	public void UpdateRenderSize()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.RenderSize))
		{
		case 0:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthLarge;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightLarge;
			break;
		case 1:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthMedium;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightMedium;
			break;
		case 2:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthSmall;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightSmall;
			break;
		}
		RenderTextureMain.instance.ResetResolution();
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x000577A4 File Offset: 0x000559A4
	public void UpdateGlitchLoop()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.GlitchLoop) == 1)
		{
			this.glitchLoop = true;
			return;
		}
		this.glitchLoop = false;
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x000577C4 File Offset: 0x000559C4
	public void UpdateGamma()
	{
		this.gamma = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.Gamma);
		PostProcessing.Instance.colorGrading.gamma.value = new Vector4(0f, 0f, 0f, this.gammaCurve.Evaluate(this.gamma / 100f));
	}

	// Token: 0x040010B1 RID: 4273
	public static GraphicsManager instance;

	// Token: 0x040010B2 RID: 4274
	internal float lightDistance;

	// Token: 0x040010B3 RID: 4275
	internal float shadowDistance;

	// Token: 0x040010B4 RID: 4276
	internal float gamma;

	// Token: 0x040010B5 RID: 4277
	public AnimationCurve gammaCurve;

	// Token: 0x040010B6 RID: 4278
	internal bool glitchLoop;

	// Token: 0x040010B7 RID: 4279
	private float fullscreenCheckTimer;

	// Token: 0x040010B8 RID: 4280
	private FullScreenMode windowMode;

	// Token: 0x040010B9 RID: 4281
	private bool windowFullscreen;

	// Token: 0x040010BA RID: 4282
	private float firstSetupTimer = 1f;
}
