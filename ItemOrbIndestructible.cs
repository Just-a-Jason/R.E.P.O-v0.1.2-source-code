using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class ItemOrbIndestructible : MonoBehaviour
{
	// Token: 0x06000BAF RID: 2991 RVA: 0x000681AB File Offset: 0x000663AB
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x000681C8 File Offset: 0x000663C8
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				physGrabObject.OverrideIndestructible(0.1f);
			}
		}
	}

	// Token: 0x0400130A RID: 4874
	private ItemOrb itemOrb;

	// Token: 0x0400130B RID: 4875
	private PhysGrabObject physGrabObject;
}
