using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class CollisionFree : MonoBehaviour
{
	// Token: 0x060011D1 RID: 4561 RVA: 0x0009DED8 File Offset: 0x0009C0D8
	private void OnTriggerExit(Collider other)
	{
		this.colliding = false;
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0009DEE1 File Offset: 0x0009C0E1
	private void OnTriggerStay(Collider other)
	{
		this.colliding = true;
	}

	// Token: 0x04001DFB RID: 7675
	[HideInInspector]
	public bool colliding;
}
