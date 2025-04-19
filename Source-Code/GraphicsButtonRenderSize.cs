using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class GraphicsButtonRenderSize : MonoBehaviour
{
	// Token: 0x0600094C RID: 2380 RVA: 0x000571A4 File Offset: 0x000553A4
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateRenderSize();
	}
}
