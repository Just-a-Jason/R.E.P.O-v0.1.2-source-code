using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class ItemUpgradePlayerGrabRange : MonoBehaviour
{
	// Token: 0x06000C6D RID: 3181 RVA: 0x0006DAC5 File Offset: 0x0006BCC5
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0006DAD3 File Offset: 0x0006BCD3
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerGrabRange(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013BD RID: 5053
	private ItemToggle itemToggle;
}
