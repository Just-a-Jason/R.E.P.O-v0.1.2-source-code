using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class GraphicsButtonGamma : MonoBehaviour
{
	// Token: 0x0600093E RID: 2366 RVA: 0x00057118 File Offset: 0x00055318
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateGamma();
	}
}
