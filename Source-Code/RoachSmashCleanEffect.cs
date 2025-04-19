using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class RoachSmashCleanEffect : MonoBehaviour
{
	// Token: 0x060006F7 RID: 1783 RVA: 0x00041CA1 File Offset: 0x0003FEA1
	private void Start()
	{
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00041CA3 File Offset: 0x0003FEA3
	private void Update()
	{
		if (!this.CleanEffectDone)
		{
			if (this.CleanEffectTimer > this.CleanEffectDelay)
			{
				this.cleanEffect.Clean();
				this.CleanEffectDone = true;
				return;
			}
			this.CleanEffectTimer += Time.deltaTime;
		}
	}

	// Token: 0x04000BCD RID: 3021
	public CleanEffect cleanEffect;

	// Token: 0x04000BCE RID: 3022
	public float CleanEffectDelay;

	// Token: 0x04000BCF RID: 3023
	private float CleanEffectTimer;

	// Token: 0x04000BD0 RID: 3024
	private bool CleanEffectDone;
}
