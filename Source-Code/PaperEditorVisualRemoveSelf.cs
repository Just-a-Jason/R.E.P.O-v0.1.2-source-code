using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class PaperEditorVisualRemoveSelf : MonoBehaviour
{
	// Token: 0x060006DF RID: 1759 RVA: 0x00041235 File Offset: 0x0003F435
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
