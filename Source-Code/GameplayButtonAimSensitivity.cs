using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class GameplayButtonAimSensitivity : MonoBehaviour
{
	// Token: 0x06000923 RID: 2339 RVA: 0x00056E68 File Offset: 0x00055068
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateAimSensitivity();
	}
}
