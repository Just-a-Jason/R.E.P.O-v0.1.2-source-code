using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class CameraCrouchNoise : MonoBehaviour
{
	// Token: 0x060000BF RID: 191 RVA: 0x0000758B File Offset: 0x0000578B
	private void Start()
	{
		this.Player = PlayerController.instance;
		this.AnimNoise.MasterAmount = 0f;
		this.AnimNoise.enabled = false;
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000075B4 File Offset: 0x000057B4
	private void Update()
	{
		if (this.Player.Crouching && !RecordingDirector.instance)
		{
			this.AnimNoise.enabled = true;
			this.AnimNoise.MasterAmount = Mathf.Lerp(this.AnimNoise.MasterAmount, this.Strength * GameplayManager.instance.cameraNoise, Time.deltaTime * this.LerpSpeed);
			return;
		}
		if (this.AnimNoise.enabled)
		{
			this.AnimNoise.MasterAmount = Mathf.Lerp(this.AnimNoise.MasterAmount, 0f, Time.deltaTime * this.LerpSpeed);
			if (this.AnimNoise.MasterAmount < 0.001f)
			{
				this.AnimNoise.enabled = false;
			}
		}
	}

	// Token: 0x040001F7 RID: 503
	private PlayerController Player;

	// Token: 0x040001F8 RID: 504
	public AnimNoise AnimNoise;

	// Token: 0x040001F9 RID: 505
	public float Strength = 1f;

	// Token: 0x040001FA RID: 506
	public float LerpSpeed = 2f;
}
