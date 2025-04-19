using System;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class GraphicsButtonChromaticAberration : MonoBehaviour
{
	// Token: 0x0600093C RID: 2364 RVA: 0x00057104 File Offset: 0x00055304
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateChromaticAberration();
	}
}
