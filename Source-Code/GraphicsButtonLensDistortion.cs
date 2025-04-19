using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class GraphicsButtonLensDistortion : MonoBehaviour
{
	// Token: 0x06000944 RID: 2372 RVA: 0x00057154 File Offset: 0x00055354
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateLensDistortion();
	}
}
