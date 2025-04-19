using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class CameraTarget : MonoBehaviour
{
	// Token: 0x060000AA RID: 170 RVA: 0x00006ACC File Offset: 0x00004CCC
	private void Update()
	{
		if (this.targetActiveImpulse)
		{
			if (this.targetActive)
			{
				if (this.targetActive)
				{
					this.targetToggleAudio.Play(1f);
					this.camNoise.NoiseOverride(1.5f, 1f, 2f, 0.5f, 0.5f);
				}
				this.targetActive = false;
			}
			else
			{
				if (!this.targetActive)
				{
					this.targetToggleAudio.Play(1f);
					this.camNoise.NoiseOverride(1.5f, 1f, 2f, 0.5f, 0.5f);
				}
				this.targetActive = true;
			}
			this.targetActiveImpulse = false;
		}
		if (this.targetActive)
		{
			this.targetLerpAmount = Mathf.Clamp(this.targetLerpAmount + this.targetLerpSpeed * Time.deltaTime, 0f, 1f);
			return;
		}
		this.targetLerpAmount = Mathf.Clamp(this.targetLerpAmount - this.targetLerpSpeed * Time.deltaTime, 0f, 1f);
	}

	// Token: 0x040001B2 RID: 434
	[HideInInspector]
	public bool targetActiveImpulse;

	// Token: 0x040001B3 RID: 435
	[HideInInspector]
	public bool targetActive;

	// Token: 0x040001B4 RID: 436
	[HideInInspector]
	public float targetLerpAmount;

	// Token: 0x040001B5 RID: 437
	public float targetLerpSpeed = 0.01f;

	// Token: 0x040001B6 RID: 438
	public AnimationCurve targetLerpCurve;

	// Token: 0x040001B7 RID: 439
	public AnimNoise camNoise;

	// Token: 0x040001B8 RID: 440
	public AudioPlay targetToggleAudio;
}
