using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class EnemyHeadHairTarget : MonoBehaviour
{
	// Token: 0x06000320 RID: 800 RVA: 0x0001EA95 File Offset: 0x0001CC95
	private void Start()
	{
		base.transform.parent = this.Parent;
	}

	// Token: 0x0400057F RID: 1407
	public Transform Parent;
}
