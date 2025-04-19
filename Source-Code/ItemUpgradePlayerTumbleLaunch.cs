using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class ItemUpgradePlayerTumbleLaunch : MonoBehaviour
{
	// Token: 0x06000C7C RID: 3196 RVA: 0x0006DBDD File Offset: 0x0006BDDD
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0006DBEB File Offset: 0x0006BDEB
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerTumbleLaunch(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013C2 RID: 5058
	private ItemToggle itemToggle;
}
