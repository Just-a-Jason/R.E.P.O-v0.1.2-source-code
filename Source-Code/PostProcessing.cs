using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000223 RID: 547
public class PostProcessing : MonoBehaviour
{
	// Token: 0x0600119E RID: 4510 RVA: 0x0009C7C7 File Offset: 0x0009A9C7
	private void Awake()
	{
		PostProcessing.Instance = this;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0009C7D0 File Offset: 0x0009A9D0
	private void Start()
	{
		this.volume.profile.TryGetSettings<Grain>(out this.grain);
		this.grainSizeDefault = this.grain.size.value;
		this.grainIntensityDefault = this.grain.intensity.value;
		this.grain.intensity.value = 1f;
		this.volume.profile.TryGetSettings<MotionBlur>(out this.motionBlur);
		this.motionBlurDefault = this.motionBlur.shutterAngle.value;
		this.volume.profile.TryGetSettings<LensDistortion>(out this.lensDistortion);
		this.volume.profile.TryGetSettings<Bloom>(out this.bloom);
		this.volume.profile.TryGetSettings<ColorGrading>(out this.colorGrading);
		this.volume.profile.TryGetSettings<Vignette>(out this.vignette);
		this.volume.profile.TryGetSettings<ChromaticAberration>(out this.chromaticAberration);
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0009C8D8 File Offset: 0x0009AAD8
	private void Update()
	{
		if (!this.setupDone)
		{
			return;
		}
		Color color = this.vignetteColor;
		float num = this.vignetteIntensity;
		float num2 = this.vignetteSmoothness;
		if (this.vignetteOverrideActive)
		{
			if (this.vignetteOverrideTimer > 0f)
			{
				color = Color.Lerp(color, this.vignetteOverrideColor, this.vignetteOverrideLerp);
				num = Mathf.Lerp(num, this.vignetteOverrideIntensity, this.vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, this.vignetteOverrideSmoothness, this.vignetteOverrideLerp);
				this.vignetteOverrideLerp += this.vignetteOverrideSpeedIn * Time.deltaTime;
				this.vignetteOverrideLerp = Mathf.Clamp01(this.vignetteOverrideLerp);
				this.vignetteOverrideTimer -= Time.deltaTime;
			}
			else
			{
				color = Color.Lerp(color, this.vignetteOverrideColor, this.vignetteOverrideLerp);
				num = Mathf.Lerp(num, this.vignetteOverrideIntensity, this.vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, this.vignetteOverrideSmoothness, this.vignetteOverrideLerp);
				this.vignetteOverrideLerp -= this.vignetteOverrideSpeedOut * Time.deltaTime;
				if (this.vignetteOverrideLerp <= 0f)
				{
					this.vignetteOverrideActive = false;
					this.vignetteOverrideLerp = 0f;
				}
			}
		}
		this.vignette.color.value = color;
		this.vignette.intensity.value = num;
		this.vignette.smoothness.value = num2;
		if (this.saturationOverrideActive)
		{
			if (this.saturationOverrideTimer > 0f)
			{
				this.colorGrading.saturation.value = Mathf.Lerp(this.colorGradingSaturation, this.saturationOverrideAmount, this.saturationOverrideLerp);
				this.saturationOverrideLerp += this.saturationOverrideSpeedIn * Time.deltaTime;
				this.saturationOverrideLerp = Mathf.Clamp01(this.saturationOverrideLerp);
				this.saturationOverrideTimer -= Time.deltaTime;
			}
			else
			{
				this.colorGrading.saturation.value = Mathf.Lerp(this.colorGradingSaturation, this.saturationOverrideAmount, this.saturationOverrideLerp);
				this.saturationOverrideLerp -= this.saturationOverrideSpeedOut * Time.deltaTime;
				if (this.saturationOverrideLerp <= 0f)
				{
					this.colorGrading.saturation.value = this.colorGradingSaturation;
					this.saturationOverrideActive = false;
					this.saturationOverrideLerp = 0f;
				}
			}
		}
		if (this.contrastOverrideActive)
		{
			if (this.contrastOverrideTimer > 0f)
			{
				this.colorGrading.contrast.value = Mathf.Lerp(this.colorGradingContrast, this.contrastOverrideAmount, this.contrastOverrideLerp);
				this.contrastOverrideLerp += this.contrastOverrideSpeedIn * Time.deltaTime;
				this.contrastOverrideLerp = Mathf.Clamp01(this.contrastOverrideLerp);
				this.contrastOverrideTimer -= Time.deltaTime;
			}
			else
			{
				this.colorGrading.contrast.value = Mathf.Lerp(this.colorGradingContrast, this.contrastOverrideAmount, this.contrastOverrideLerp);
				this.contrastOverrideLerp -= this.contrastOverrideSpeedOut * Time.deltaTime;
				if (this.contrastOverrideLerp <= 0f)
				{
					this.colorGrading.contrast.value = this.colorGradingContrast;
					this.contrastOverrideActive = false;
					this.contrastOverrideLerp = 0f;
				}
			}
		}
		if (this.bloomDisableTimer > 0f)
		{
			this.bloomDisableTimer -= Time.deltaTime;
			if (this.bloomDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
			{
				this.bloom.active = true;
			}
		}
		if (this.grainDisableTimer > 0f)
		{
			this.grainDisableTimer -= Time.deltaTime;
			if (this.grainDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
			{
				this.grain.active = true;
			}
		}
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0009CCA8 File Offset: 0x0009AEA8
	public void SpectateSet()
	{
		this.motionBlur.shutterAngle.value = 1f;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0009CCBF File Offset: 0x0009AEBF
	public void SpectateReset()
	{
		this.motionBlur.shutterAngle.value = this.motionBlurDefault;
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0009CCD8 File Offset: 0x0009AED8
	public void Setup()
	{
		this.colorGrading.temperature.value = LevelGenerator.Instance.Level.ColorTemperature;
		this.colorGrading.colorFilter.value = LevelGenerator.Instance.Level.ColorFilter;
		this.colorGradingSaturation = this.colorGrading.saturation.value;
		this.colorGradingContrast = this.colorGrading.contrast.value;
		this.bloom.intensity.value = LevelGenerator.Instance.Level.BloomIntensity;
		this.bloom.threshold.value = LevelGenerator.Instance.Level.BloomThreshold;
		this.vignette.color.value = LevelGenerator.Instance.Level.VignetteColor;
		this.vignetteColor = this.vignette.color.value;
		this.vignette.intensity.value = LevelGenerator.Instance.Level.VignetteIntensity;
		this.vignetteIntensity = this.vignette.intensity.value;
		this.vignette.smoothness.value = LevelGenerator.Instance.Level.VignetteSmoothness;
		this.vignetteSmoothness = this.vignette.smoothness.value;
		base.StartCoroutine(this.Intro());
		this.setupDone = true;
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0009CE40 File Offset: 0x0009B040
	private IEnumerator Intro()
	{
		while (GameDirector.instance.currentState < GameDirector.gameState.Main)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (this.introLerp < 1f)
		{
			this.grain.intensity.value = Mathf.Lerp(0.8f, this.grainIntensityDefault, this.introCurve.Evaluate(this.introLerp));
			this.grain.size.value = Mathf.Lerp(1.5f, this.grainSizeDefault, this.introCurve.Evaluate(this.introLerp));
			this.introLerp += this.introSpeed * Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0009CE50 File Offset: 0x0009B050
	public void VignetteOverride(Color _color, float _intensity, float _smoothness, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.vignetteOverrideActive && _obj != this.vignetteOverrideObject)
		{
			return;
		}
		_smoothness = Mathf.Clamp01(_smoothness);
		this.vignetteOverrideActive = true;
		this.vignetteOverrideObject = _obj;
		this.vignetteOverrideTimer = _time;
		this.vignetteOverrideSpeedIn = _speedIn;
		this.vignetteOverrideSpeedOut = _speedOut;
		this.vignetteOverrideColor = _color;
		this.vignetteOverrideIntensity = _intensity;
		this.vignetteOverrideSmoothness = _smoothness;
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0009CEBC File Offset: 0x0009B0BC
	public void SaturationOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.saturationOverrideActive && _obj != this.saturationOverrideObject)
		{
			return;
		}
		this.saturationOverrideActive = true;
		this.saturationOverrideObject = _obj;
		this.saturationOverrideTimer = _time;
		this.saturationOverrideSpeedIn = _speedIn;
		this.saturationOverrideSpeedOut = _speedOut;
		this.saturationOverrideAmount = _amount;
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0009CF10 File Offset: 0x0009B110
	public void ContrastOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.contrastOverrideActive && _obj != this.contrastOverrideObject)
		{
			return;
		}
		this.contrastOverrideActive = true;
		this.contrastOverrideObject = _obj;
		this.contrastOverrideTimer = _time;
		this.contrastOverrideSpeedIn = _speedIn;
		this.contrastOverrideSpeedOut = _speedOut;
		this.contrastOverrideAmount = _amount;
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0009CF61 File Offset: 0x0009B161
	public void BloomDisable(float _time)
	{
		this.bloomDisableTimer = _time;
		this.bloom.active = false;
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0009CF76 File Offset: 0x0009B176
	public void GrainDisable(float _time)
	{
		this.grainDisableTimer = _time;
		this.grain.active = false;
	}

	// Token: 0x04001D94 RID: 7572
	public static PostProcessing Instance;

	// Token: 0x04001D95 RID: 7573
	private bool setupDone;

	// Token: 0x04001D96 RID: 7574
	public PostProcessVolume volume;

	// Token: 0x04001D97 RID: 7575
	internal Grain grain;

	// Token: 0x04001D98 RID: 7576
	private float grainDisableTimer;

	// Token: 0x04001D99 RID: 7577
	internal Bloom bloom;

	// Token: 0x04001D9A RID: 7578
	private float bloomDisableTimer;

	// Token: 0x04001D9B RID: 7579
	internal ColorGrading colorGrading;

	// Token: 0x04001D9C RID: 7580
	private float colorGradingSaturation;

	// Token: 0x04001D9D RID: 7581
	private float colorGradingContrast;

	// Token: 0x04001D9E RID: 7582
	internal Vignette vignette;

	// Token: 0x04001D9F RID: 7583
	private Color vignetteColor;

	// Token: 0x04001DA0 RID: 7584
	private float vignetteIntensity;

	// Token: 0x04001DA1 RID: 7585
	private float vignetteSmoothness;

	// Token: 0x04001DA2 RID: 7586
	internal MotionBlur motionBlur;

	// Token: 0x04001DA3 RID: 7587
	internal LensDistortion lensDistortion;

	// Token: 0x04001DA4 RID: 7588
	internal ChromaticAberration chromaticAberration;

	// Token: 0x04001DA5 RID: 7589
	public AnimationCurve introCurve;

	// Token: 0x04001DA6 RID: 7590
	public float introSpeed;

	// Token: 0x04001DA7 RID: 7591
	private float introLerp;

	// Token: 0x04001DA8 RID: 7592
	private float motionBlurDefault;

	// Token: 0x04001DA9 RID: 7593
	private float bloomDefault;

	// Token: 0x04001DAA RID: 7594
	private float grainIntensityDefault;

	// Token: 0x04001DAB RID: 7595
	private float grainSizeDefault;

	// Token: 0x04001DAC RID: 7596
	[Space]
	private bool vignetteOverrideActive;

	// Token: 0x04001DAD RID: 7597
	private float vignetteOverrideLerp;

	// Token: 0x04001DAE RID: 7598
	private float vignetteOverrideTimer;

	// Token: 0x04001DAF RID: 7599
	private float vignetteOverrideSpeedIn;

	// Token: 0x04001DB0 RID: 7600
	private float vignetteOverrideSpeedOut;

	// Token: 0x04001DB1 RID: 7601
	private Color vignetteOverrideColor;

	// Token: 0x04001DB2 RID: 7602
	private float vignetteOverrideIntensity;

	// Token: 0x04001DB3 RID: 7603
	private float vignetteOverrideSmoothness;

	// Token: 0x04001DB4 RID: 7604
	private GameObject vignetteOverrideObject;

	// Token: 0x04001DB5 RID: 7605
	private bool saturationOverrideActive;

	// Token: 0x04001DB6 RID: 7606
	private float saturationOverrideLerp;

	// Token: 0x04001DB7 RID: 7607
	private float saturationOverrideTimer;

	// Token: 0x04001DB8 RID: 7608
	private float saturationOverrideSpeedIn;

	// Token: 0x04001DB9 RID: 7609
	private float saturationOverrideSpeedOut;

	// Token: 0x04001DBA RID: 7610
	private float saturationOverrideAmount;

	// Token: 0x04001DBB RID: 7611
	private GameObject saturationOverrideObject;

	// Token: 0x04001DBC RID: 7612
	private bool contrastOverrideActive;

	// Token: 0x04001DBD RID: 7613
	private float contrastOverrideLerp;

	// Token: 0x04001DBE RID: 7614
	private float contrastOverrideTimer;

	// Token: 0x04001DBF RID: 7615
	private float contrastOverrideSpeedIn;

	// Token: 0x04001DC0 RID: 7616
	private float contrastOverrideSpeedOut;

	// Token: 0x04001DC1 RID: 7617
	private float contrastOverrideAmount;

	// Token: 0x04001DC2 RID: 7618
	private GameObject contrastOverrideObject;
}
