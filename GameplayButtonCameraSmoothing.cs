using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class GameplayButtonCameraSmoothing : MonoBehaviour
{
	// Token: 0x06000927 RID: 2343 RVA: 0x00056E90 File Offset: 0x00055090
	public void ButtonPressed()
	{
		GameplayManager.instance.UpdateCameraSmoothing();
	}
}
