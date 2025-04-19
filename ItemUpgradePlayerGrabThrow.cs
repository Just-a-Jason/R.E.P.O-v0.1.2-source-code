using System;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class ItemUpgradePlayerGrabThrow : MonoBehaviour
{
	// Token: 0x06000C73 RID: 3187 RVA: 0x0006DB35 File Offset: 0x0006BD35
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0006DB43 File Offset: 0x0006BD43
	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerThrowStrength(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID)));
	}

	// Token: 0x040013BF RID: 5055
	private ItemToggle itemToggle;
}
