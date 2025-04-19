using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class ItemMineExplosive : MonoBehaviour
{
	// Token: 0x06000B7F RID: 2943 RVA: 0x00066036 File Offset: 0x00064236
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00066044 File Offset: 0x00064244
	public void OnTriggered()
	{
		this.particleScriptExplosion.Spawn(base.transform.position, 1.2f, 75, 200, 4f, false, false, 1f);
	}

	// Token: 0x040012AA RID: 4778
	private ParticleScriptExplosion particleScriptExplosion;
}
