using System;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class GraphicsButtonGlitchLoop : MonoBehaviour
{
	// Token: 0x06000940 RID: 2368 RVA: 0x0005712C File Offset: 0x0005532C
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateGlitchLoop();
	}
}
