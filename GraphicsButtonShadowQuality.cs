using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class GraphicsButtonShadowQuality : MonoBehaviour
{
	// Token: 0x06000950 RID: 2384 RVA: 0x000571CC File Offset: 0x000553CC
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateShadowQuality();
	}
}
