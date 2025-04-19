using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class MenuPageSettingsControls : MonoBehaviour
{
	// Token: 0x06001110 RID: 4368 RVA: 0x00098D02 File Offset: 0x00096F02
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00098D10 File Offset: 0x00096F10
	public void ResetControls()
	{
		InputManager.instance.LoadKeyBindings("DefaultKeyBindings.es3");
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsControls);
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00098D36 File Offset: 0x00096F36
	public void SaveControls()
	{
		InputManager.instance.SaveCurrentKeyBindings();
	}

	// Token: 0x04001C5C RID: 7260
	private MenuPage menuPage;
}
