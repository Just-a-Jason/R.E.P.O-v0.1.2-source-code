using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class GraphicsButtonLightDistance : MonoBehaviour
{
	// Token: 0x06000946 RID: 2374 RVA: 0x00057168 File Offset: 0x00055368
	public void ButtonPress()
	{
		GraphicsManager.instance.UpdateLightDistance();
	}
}
