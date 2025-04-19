using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class BarrelValuable : Trap
{
	// Token: 0x06001360 RID: 4960 RVA: 0x000AA25F File Offset: 0x000A845F
	protected override void Start()
	{
		base.Start();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x000AA274 File Offset: 0x000A8474
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 100, 1f, false, false, 1f);
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x000AA2AD File Offset: 0x000A84AD
	public void PotentialExplode()
	{
		if (this.isLocal)
		{
			if (this.HitCount >= this.MaxHitCount - 1)
			{
				this.Explode();
				return;
			}
			this.HitCount++;
		}
	}

	// Token: 0x04002127 RID: 8487
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002128 RID: 8488
	private int HitCount;

	// Token: 0x04002129 RID: 8489
	private int MaxHitCount = 3;

	// Token: 0x0400212A RID: 8490
	public Transform Center;
}
