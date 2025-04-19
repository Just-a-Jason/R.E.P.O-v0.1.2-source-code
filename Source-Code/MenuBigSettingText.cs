using System;
using TMPro;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class MenuBigSettingText : MonoBehaviour
{
	// Token: 0x06000FE6 RID: 4070 RVA: 0x000912E7 File Offset: 0x0008F4E7
	private void Start()
	{
		this.textMeshPro = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x04001ABD RID: 6845
	internal TextMeshProUGUI textMeshPro;
}
