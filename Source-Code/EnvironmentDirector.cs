using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class EnvironmentDirector : MonoBehaviour
{
	// Token: 0x0600078D RID: 1933 RVA: 0x0004767D File Offset: 0x0004587D
	private void Awake()
	{
		EnvironmentDirector.Instance = this;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00047685 File Offset: 0x00045885
	private void Start()
	{
		this.MainCamera = Camera.main;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00047694 File Offset: 0x00045894
	public void Setup()
	{
		RenderSettings.fogColor = LevelGenerator.Instance.Level.FogColor;
		RenderSettings.fogStartDistance = LevelGenerator.Instance.Level.FogStartDistance;
		RenderSettings.fogEndDistance = LevelGenerator.Instance.Level.FogEndDistance;
		this.MainCamera.backgroundColor = RenderSettings.fogColor;
		this.MainCamera.farClipPlane = RenderSettings.fogEndDistance + 1f;
		this.DarkAdaptationLerp = 0.1f;
		if (LevelGenerator.Instance.Level.AmbiencePresets.Count > 0)
		{
			AmbienceLoop.instance.Setup();
			AmbienceBreakers.instance.Setup();
		}
		else
		{
			Debug.LogError("Level is missing ambience preset!");
		}
		this.SetupDone = true;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0004774C File Offset: 0x0004594C
	private void Update()
	{
		if (!this.SetupDone)
		{
			return;
		}
		Color ambientColor = LevelGenerator.Instance.Level.AmbientColor;
		Color ambientColorAdaptation = LevelGenerator.Instance.Level.AmbientColorAdaptation;
		if (!FlashlightController.Instance.LightActive)
		{
			if (this.DarkAdaptationLerp < 1f)
			{
				this.DarkAdaptationLerp += Time.deltaTime * this.DarkAdaptationSpeedIn;
				this.DarkAdaptationLerp = Mathf.Clamp01(this.DarkAdaptationLerp);
				RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, this.DarkAdaptationCurve.Evaluate(this.DarkAdaptationLerp));
				return;
			}
		}
		else if (this.DarkAdaptationLerp > 0f)
		{
			this.DarkAdaptationLerp -= Time.deltaTime * this.DarkAdaptationSpeedOut;
			this.DarkAdaptationLerp = Mathf.Clamp01(this.DarkAdaptationLerp);
			RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, this.DarkAdaptationCurve.Evaluate(this.DarkAdaptationLerp));
		}
	}

	// Token: 0x04000D44 RID: 3396
	public static EnvironmentDirector Instance;

	// Token: 0x04000D45 RID: 3397
	private bool SetupDone;

	// Token: 0x04000D46 RID: 3398
	private Camera MainCamera;

	// Token: 0x04000D47 RID: 3399
	[Space]
	public float DarkAdaptationSpeedIn = 5f;

	// Token: 0x04000D48 RID: 3400
	public float DarkAdaptationSpeedOut = 5f;

	// Token: 0x04000D49 RID: 3401
	public AnimationCurve DarkAdaptationCurve;

	// Token: 0x04000D4A RID: 3402
	private float DarkAdaptationLerp;
}
