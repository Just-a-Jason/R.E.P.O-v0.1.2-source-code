using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class GameplayButtonPlayerNames : MonoBehaviour
{
	// Token: 0x06000929 RID: 2345 RVA: 0x00056EA4 File Offset: 0x000550A4
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdatePlayerNames();
	}
}
