using System;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class GraphicsButtonBloom : MonoBehaviour
{
	// Token: 0x0600093A RID: 2362 RVA: 0x000570F0 File Offset: 0x000552F0
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateBloom();
	}
}
