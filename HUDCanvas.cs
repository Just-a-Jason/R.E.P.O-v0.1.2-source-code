using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class HUDCanvas : MonoBehaviour
{
	// Token: 0x0600122F RID: 4655 RVA: 0x000A065C File Offset: 0x0009E85C
	private void Awake()
	{
		HUDCanvas.instance = this;
		this.rect = base.GetComponent<RectTransform>();
	}

	// Token: 0x04001EDD RID: 7901
	public static HUDCanvas instance;

	// Token: 0x04001EDE RID: 7902
	internal RectTransform rect;
}
