using System;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class ItemUpgradePlayerEnergy : MonoBehaviour
{
	// Token: 0x06000C67 RID: 3175 RVA: 0x0006DA55 File Offset: 0x0006BC55
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0006DA63 File Offset: 0x0006BC63
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerEnergy(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013BB RID: 5051
	private ItemToggle itemToggle;
}
