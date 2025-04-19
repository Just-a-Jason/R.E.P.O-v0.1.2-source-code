using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class ItemUpgradePlayerSprintSpeed : MonoBehaviour
{
	// Token: 0x06000C79 RID: 3193 RVA: 0x0006DBA5 File Offset: 0x0006BDA5
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0006DBB3 File Offset: 0x0006BDB3
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerSprintSpeed(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013C1 RID: 5057
	private ItemToggle itemToggle;
}
