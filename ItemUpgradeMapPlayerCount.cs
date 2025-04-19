using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class ItemUpgradeMapPlayerCount : MonoBehaviour
{
	// Token: 0x06000C62 RID: 3170 RVA: 0x0006D9E9 File Offset: 0x0006BBE9
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0006D9F7 File Offset: 0x0006BBF7
	public void Upgrade()
	{
		PunManager.instance.UpgradeMapPlayerCount(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013B9 RID: 5049
	private ItemToggle itemToggle;
}
