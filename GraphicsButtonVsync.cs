using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class GraphicsButtonVsync : MonoBehaviour
{
	// Token: 0x06000952 RID: 2386 RVA: 0x000571E0 File Offset: 0x000553E0
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateVsync();
	}
}
