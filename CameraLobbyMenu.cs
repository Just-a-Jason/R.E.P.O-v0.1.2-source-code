using System;

// Token: 0x02000026 RID: 38
public class CameraLobbyMenu : CameraNoPlayerTarget
{
	// Token: 0x06000094 RID: 148 RVA: 0x00006051 File Offset: 0x00004251
	protected override void Awake()
	{
		base.Awake();
		CameraNoise.Instance.AnimNoise.noiseStrengthDefault = 0.3f;
		CameraNoise.Instance.AnimNoise.noiseSpeedDefault = 4f;
	}
}
