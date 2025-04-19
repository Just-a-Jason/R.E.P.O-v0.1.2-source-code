using System;
using UnityEngine;

// Token: 0x020001EE RID: 494
public class MenuSettingElement : MonoBehaviour
{
	// Token: 0x06001076 RID: 4214 RVA: 0x00094AAD File Offset: 0x00092CAD
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.parentPage.settingElements.Add(this);
		this.settingElementID = this.parentPage.settingElements.Count;
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00094AE2 File Offset: 0x00092CE2
	private void OnDestroy()
	{
		if (this.parentPage)
		{
			this.parentPage.settingElements.Remove(this);
		}
	}

	// Token: 0x04001B8C RID: 7052
	private MenuPage parentPage;

	// Token: 0x04001B8D RID: 7053
	internal int settingElementID;
}
