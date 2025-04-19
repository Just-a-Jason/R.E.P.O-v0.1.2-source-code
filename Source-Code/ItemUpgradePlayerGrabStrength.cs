using System;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class ItemUpgradePlayerGrabStrength : MonoBehaviour
{
	// Token: 0x06000C70 RID: 3184 RVA: 0x0006DAFD File Offset: 0x0006BCFD
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0006DB0B File Offset: 0x0006BD0B
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerGrabStrength(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013BE RID: 5054
	private ItemToggle itemToggle;
}
