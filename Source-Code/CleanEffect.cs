using System;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class CleanEffect : MonoBehaviour
{
	// Token: 0x060006BF RID: 1727 RVA: 0x00040ADE File Offset: 0x0003ECDE
	public void Update()
	{
		if (!this.destroyNow)
		{
			return;
		}
		this.destroyTimer += Time.deltaTime;
		if (this.destroyTimer > 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00040B13 File Offset: 0x0003ED13
	public void Clean()
	{
		this.destroyNow = true;
	}

	// Token: 0x04000B6E RID: 2926
	[Space]
	[Header("Sounds")]
	public Sound CleanSound;

	// Token: 0x04000B6F RID: 2927
	public ParticleSystem GleamParticles;

	// Token: 0x04000B70 RID: 2928
	private float destroyTimer;

	// Token: 0x04000B71 RID: 2929
	public bool destroyNow;
}
