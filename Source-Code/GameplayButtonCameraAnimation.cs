using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class GameplayButtonCameraAnimation : MonoBehaviour
{
	// Token: 0x06000925 RID: 2341 RVA: 0x00056E7C File Offset: 0x0005507C
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateCameraAnimation();
	}
}
