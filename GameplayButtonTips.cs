using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class GameplayButtonTips : MonoBehaviour
{
	// Token: 0x0600092B RID: 2347 RVA: 0x00056EB8 File Offset: 0x000550B8
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateTips();
	}
}
