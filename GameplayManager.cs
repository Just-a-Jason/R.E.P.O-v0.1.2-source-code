using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class GameplayManager : MonoBehaviour
{
	// Token: 0x0600092D RID: 2349 RVA: 0x00056ECC File Offset: 0x000550CC
	private void Awake()
	{
		GameplayManager.instance = this;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00056ED4 File Offset: 0x000550D4
	private void Start()
	{
		this.UpdateAll();
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00056EDC File Offset: 0x000550DC
	private void Update()
	{
		if (this.cameraAnimationOverrideTimer > 0f)
		{
			this.cameraAnimationOverrideTimer -= Time.deltaTime;
			if (this.cameraAnimationOverrideTimer <= 0f)
			{
				this.UpdateCameraAnimation();
			}
		}
		if (this.cameraNoiseOverrideTimer > 0f)
		{
			this.cameraNoiseOverrideTimer -= Time.deltaTime;
		}
		else
		{
			this.cameraNoise = DataDirector.instance.SettingValueFetchFloat(DataDirector.Setting.CameraNoise);
		}
		if (this.cameraShakeOverrideTimer > 0f)
		{
			this.cameraShakeOverrideTimer -= Time.deltaTime;
			return;
		}
		this.cameraShake = DataDirector.instance.SettingValueFetchFloat(DataDirector.Setting.CameraShake);
		if (SpectateCamera.instance)
		{
			if (SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				this.cameraShake *= 0.1f;
				return;
			}
			this.cameraShake *= 0.5f;
		}
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00056FBE File Offset: 0x000551BE
	public void UpdateAll()
	{
		this.UpdateTips();
		this.UpdateCameraSmoothing();
		this.UpdateAimSensitivity();
		this.UpdateCameraAnimation();
		this.UpdatePlayerNames();
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00056FDE File Offset: 0x000551DE
	public void UpdateTips()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Tips) == 1)
		{
			this.tips = true;
			return;
		}
		this.tips = false;
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00056FFE File Offset: 0x000551FE
	public void UpdateCameraSmoothing()
	{
		this.cameraSmoothing = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.CameraSmoothing);
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00057013 File Offset: 0x00055213
	public void UpdateAimSensitivity()
	{
		this.aimSensitivity = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.AimSensitivity);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00057028 File Offset: 0x00055228
	public void UpdateCameraAnimation()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.CameraAnimation))
		{
		case 0:
			this.cameraAnimation = 0f;
			return;
		case 1:
			this.cameraAnimation = 0.25f;
			return;
		case 2:
			this.cameraAnimation = 0.5f;
			return;
		case 3:
			this.cameraAnimation = 0.75f;
			return;
		case 4:
			this.cameraAnimation = 1f;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00057098 File Offset: 0x00055298
	public void UpdatePlayerNames()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.PlayerNames) == 1)
		{
			this.playerNames = true;
			return;
		}
		this.playerNames = false;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x000570B8 File Offset: 0x000552B8
	public void OverrideCameraAnimation(float _value, float _time)
	{
		this.cameraAnimation = _value;
		this.cameraAnimationOverrideTimer = _time;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x000570C8 File Offset: 0x000552C8
	public void OverrideCameraNoise(float _value, float _time)
	{
		this.cameraNoise = _value;
		this.cameraNoiseOverrideTimer = _time;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x000570D8 File Offset: 0x000552D8
	public void OverrideCameraShake(float _value, float _time)
	{
		this.cameraShake = _value;
		this.cameraShakeOverrideTimer = _time;
	}

	// Token: 0x040010A4 RID: 4260
	public static GameplayManager instance;

	// Token: 0x040010A5 RID: 4261
	internal bool tips;

	// Token: 0x040010A6 RID: 4262
	internal bool playerNames;

	// Token: 0x040010A7 RID: 4263
	internal float cameraSmoothing;

	// Token: 0x040010A8 RID: 4264
	internal float aimSensitivity;

	// Token: 0x040010A9 RID: 4265
	internal float cameraAnimation;

	// Token: 0x040010AA RID: 4266
	internal float cameraNoise;

	// Token: 0x040010AB RID: 4267
	internal float cameraShake;

	// Token: 0x040010AC RID: 4268
	private float cameraAnimationOverrideTimer;

	// Token: 0x040010AD RID: 4269
	private float cameraNoiseOverrideTimer;

	// Token: 0x040010AE RID: 4270
	private float cameraShakeOverrideTimer;
}
