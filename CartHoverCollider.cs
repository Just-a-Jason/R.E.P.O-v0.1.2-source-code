using System;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class CartHoverCollider : MonoBehaviour
{
	// Token: 0x06000CFA RID: 3322 RVA: 0x00071C5C File Offset: 0x0006FE5C
	private void OnTriggerStay(Collider other)
	{
		this.cartHover = true;
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00071C65 File Offset: 0x0006FE65
	private void OnTriggerExit(Collider other)
	{
		this.cartHover = false;
	}

	// Token: 0x040014D9 RID: 5337
	[HideInInspector]
	public bool cartHover;
}
