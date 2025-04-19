using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class HUD : MonoBehaviour
{
	// Token: 0x0600122B RID: 4651 RVA: 0x000A0622 File Offset: 0x0009E822
	private void Awake()
	{
		HUD.instance = this;
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x000A062A File Offset: 0x0009E82A
	public void Hide()
	{
		this.hideParent.SetActive(false);
		this.hidden = true;
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x000A063F File Offset: 0x0009E83F
	public void Show()
	{
		this.hideParent.SetActive(true);
		this.hidden = false;
	}

	// Token: 0x04001EDA RID: 7898
	public static HUD instance;

	// Token: 0x04001EDB RID: 7899
	public bool hidden;

	// Token: 0x04001EDC RID: 7900
	public GameObject hideParent;
}
