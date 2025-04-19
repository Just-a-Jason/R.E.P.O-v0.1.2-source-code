using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class PlayerCrownSet : MonoBehaviour
{
	// Token: 0x06001125 RID: 4389 RVA: 0x000992D3 File Offset: 0x000974D3
	private void Start()
	{
		if (!PlayerCrownSet.instance)
		{
			PlayerCrownSet.instance = this;
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001C6D RID: 7277
	public static PlayerCrownSet instance;

	// Token: 0x04001C6E RID: 7278
	internal bool crownOwnerFetched;

	// Token: 0x04001C6F RID: 7279
	internal string crownOwnerSteamID;
}
