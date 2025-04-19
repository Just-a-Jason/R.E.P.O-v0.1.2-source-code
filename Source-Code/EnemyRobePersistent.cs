using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class EnemyRobePersistent : MonoBehaviour
{
	// Token: 0x060003D5 RID: 981 RVA: 0x00025CD8 File Offset: 0x00023ED8
	private void Update()
	{
		if (this.enemyRobe.isActiveAndEnabled && this.enemyRobe.currentState != EnemyRobe.State.Spawn)
		{
			if (!this.particleConstant.isPlaying)
			{
				this.particleConstant.Play();
				return;
			}
		}
		else if (this.particleConstant.isPlaying)
		{
			this.particleConstant.Stop();
		}
	}

	// Token: 0x0400067A RID: 1658
	public EnemyRobe enemyRobe;

	// Token: 0x0400067B RID: 1659
	public ParticleSystem particleConstant;
}
