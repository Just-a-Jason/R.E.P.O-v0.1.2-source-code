using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class VaseExplosionTest : MonoBehaviour
{
	// Token: 0x060013D8 RID: 5080 RVA: 0x000AD190 File Offset: 0x000AB390
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x000AD1A0 File Offset: 0x000AB3A0
	public void Explosion()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 10, 10, 1f, false, false, 1f);
	}

	// Token: 0x040021EA RID: 8682
	public Transform Center;

	// Token: 0x040021EB RID: 8683
	private ParticleScriptExplosion particleScriptExplosion;
}
