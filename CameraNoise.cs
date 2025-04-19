using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class CameraNoise : MonoBehaviour
{
	// Token: 0x06000099 RID: 153 RVA: 0x00006143 File Offset: 0x00004343
	private void Awake()
	{
		CameraNoise.Instance = this;
		this.Strength = this.StrengthDefault;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00006158 File Offset: 0x00004358
	private void Update()
	{
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			if (this.OverrideTimer <= 0f)
			{
				this.OverrideTimer = 0f;
			}
			else
			{
				this.Strength = Mathf.Lerp(this.Strength, this.OverrideStrength * GameplayManager.instance.cameraNoise, 5f * Time.deltaTime);
			}
			this.AnimNoise.noiseStrengthDefault = this.Strength;
		}
		else if (Mathf.Abs(this.Strength - this.StrengthDefault * GameplayManager.instance.cameraNoise) > 0.001f)
		{
			this.Strength = Mathf.Lerp(this.Strength, this.StrengthDefault * GameplayManager.instance.cameraNoise, 5f * Time.deltaTime);
		}
		this.AnimNoise.noiseStrengthDefault = this.Strength;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00006240 File Offset: 0x00004440
	public void Override(float strength, float time)
	{
		this.OverrideStrength = strength;
		this.OverrideTimer = time;
	}

	// Token: 0x04000183 RID: 387
	public static CameraNoise Instance;

	// Token: 0x04000184 RID: 388
	public float StrengthDefault = 0.2f;

	// Token: 0x04000185 RID: 389
	public AnimNoise AnimNoise;

	// Token: 0x04000186 RID: 390
	private float Strength = 1f;

	// Token: 0x04000187 RID: 391
	private float OverrideStrength = 1f;

	// Token: 0x04000188 RID: 392
	private float OverrideTimer;
}
