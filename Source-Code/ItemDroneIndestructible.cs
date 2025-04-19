using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class ItemDroneIndestructible : MonoBehaviour
{
	// Token: 0x06000A9B RID: 2715 RVA: 0x0005D804 File Offset: 0x0005BA04
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0005D82C File Offset: 0x0005BA2C
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			this.myPhysGrabObject.OverrideZeroGravity(0.1f);
			this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
			this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			if (this.itemDrone.magnetActive && this.itemDrone.magnetTargetPhysGrabObject)
			{
				this.itemDrone.magnetTargetPhysGrabObject.OverrideIndestructible(0.1f);
			}
		}
	}

	// Token: 0x04001130 RID: 4400
	private ItemDrone itemDrone;

	// Token: 0x04001131 RID: 4401
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001132 RID: 4402
	private ItemEquippable itemEquippable;
}
