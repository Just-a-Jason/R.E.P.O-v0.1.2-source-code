using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class CameraMainMenu : CameraNoPlayerTarget
{
	// Token: 0x06000096 RID: 150 RVA: 0x00006089 File Offset: 0x00004289
	protected override void Awake()
	{
		base.Awake();
		CameraNoise.Instance.AnimNoise.noiseStrengthDefault = 0.3f;
		CameraNoise.Instance.AnimNoise.noiseSpeedDefault = 4f;
	}

	// Token: 0x06000097 RID: 151 RVA: 0x000060BC File Offset: 0x000042BC
	protected override void Update()
	{
		base.Update();
		if (GameDirector.instance.currentState == GameDirector.gameState.Main && this.introLerp < 1f)
		{
			this.introLerp += 0.25f * Time.deltaTime;
			base.transform.localEulerAngles = new Vector3(Mathf.LerpUnclamped(0f, -45f, this.introCurve.Evaluate(this.introLerp)), 0f, 0f);
		}
	}

	// Token: 0x04000181 RID: 385
	public AnimationCurve introCurve;

	// Token: 0x04000182 RID: 386
	private float introLerp;
}
