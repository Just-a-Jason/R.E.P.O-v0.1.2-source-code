using System;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class MoneyValuable : MonoBehaviour
{
	// Token: 0x060013BE RID: 5054 RVA: 0x000AC9CD File Offset: 0x000AABCD
	public void MoneyBurst()
	{
		this.moneyBurst.Play();
	}

	// Token: 0x040021CD RID: 8653
	public ParticleSystem moneyBurst;
}
