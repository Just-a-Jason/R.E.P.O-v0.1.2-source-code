using System;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class ItemUpgradePlayerHealth : MonoBehaviour
{
	// Token: 0x06000C76 RID: 3190 RVA: 0x0006DB6D File Offset: 0x0006BD6D
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0006DB7B File Offset: 0x0006BD7B
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerHealth(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013C0 RID: 5056
	private ItemToggle itemToggle;
}
