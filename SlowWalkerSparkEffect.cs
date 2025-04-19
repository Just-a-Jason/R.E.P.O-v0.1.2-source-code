using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class SlowWalkerSparkEffect : MonoBehaviour
{
	// Token: 0x060004B6 RID: 1206 RVA: 0x0002EE79 File Offset: 0x0002D079
	private void Start()
	{
		this.sparkEffects = new List<ParticleSystem>();
		this.sparkEffects.AddRange(base.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0002EE98 File Offset: 0x0002D098
	private void Update()
	{
		if (this.playSparkEffectTimer <= 0f && this.isPlayingSparkEffect)
		{
			this.ToggleSparkEffect(false);
			this.isPlayingSparkEffect = false;
		}
		if (this.playSparkEffectTimer > 0f)
		{
			if (!this.isPlayingSparkEffect)
			{
				this.ToggleSparkEffect(true);
				this.isPlayingSparkEffect = true;
			}
			this.playSparkEffectTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0002EF00 File Offset: 0x0002D100
	private void ToggleSparkEffect(bool toggle)
	{
		foreach (ParticleSystem particleSystem in this.sparkEffects)
		{
			if (toggle)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0002EF60 File Offset: 0x0002D160
	public void PlaySparkEffect()
	{
		this.playSparkEffectTimer = 0.2f;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0002EF6D File Offset: 0x0002D16D
	public void StopSparkEffect()
	{
		this.playSparkEffectTimer = 0f;
	}

	// Token: 0x040007A7 RID: 1959
	private float playSparkEffectTimer;

	// Token: 0x040007A8 RID: 1960
	private bool isPlayingSparkEffect;

	// Token: 0x040007A9 RID: 1961
	private List<ParticleSystem> sparkEffects;
}
