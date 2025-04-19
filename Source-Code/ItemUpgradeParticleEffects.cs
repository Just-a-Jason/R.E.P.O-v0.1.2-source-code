using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class ItemUpgradeParticleEffects : MonoBehaviour
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x0006DA21 File Offset: 0x0006BC21
	private void Update()
	{
		this.destroyTimer += Time.deltaTime;
		if (this.destroyTimer > 5f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040013BA RID: 5050
	private float destroyTimer;
}
