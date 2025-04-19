using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class ItemUpgradePlayerExtraJump : MonoBehaviour
{
	// Token: 0x06000C6A RID: 3178 RVA: 0x0006DA8D File Offset: 0x0006BC8D
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0006DA9B File Offset: 0x0006BC9B
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerExtraJump(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013BC RID: 5052
	private ItemToggle itemToggle;
}
