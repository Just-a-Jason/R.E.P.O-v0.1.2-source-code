using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class GraphicsButtonMaxFPS : MonoBehaviour
{
	// Token: 0x06000948 RID: 2376 RVA: 0x0005717C File Offset: 0x0005537C
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateMaxFPS();
	}
}
