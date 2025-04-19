using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class GraphicsButtonGrain : MonoBehaviour
{
	// Token: 0x06000942 RID: 2370 RVA: 0x00057140 File Offset: 0x00055340
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateGrain();
	}
}
