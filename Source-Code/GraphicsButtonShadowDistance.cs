using System;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class GraphicsButtonShadowDistance : MonoBehaviour
{
	// Token: 0x0600094E RID: 2382 RVA: 0x000571B8 File Offset: 0x000553B8
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateShadowDistance();
	}
}
