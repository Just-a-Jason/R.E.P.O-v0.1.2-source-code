using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class GraphicsButtonMotionBlur : MonoBehaviour
{
	// Token: 0x0600094A RID: 2378 RVA: 0x00057190 File Offset: 0x00055390
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateMotionBlur();
	}
}
