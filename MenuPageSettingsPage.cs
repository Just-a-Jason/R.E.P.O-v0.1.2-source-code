using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class MenuPageSettingsPage : MonoBehaviour
{
	// Token: 0x06001114 RID: 4372 RVA: 0x00098D4A File Offset: 0x00096F4A
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00098D58 File Offset: 0x00096F58
	private void Update()
	{
		if (this.menuPage.currentPageState == MenuPage.PageState.Closing && !this.saveSettings)
		{
			this.SaveSettings();
			this.saveSettings = true;
		}
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00098D80 File Offset: 0x00096F80
	public void ResetSettings()
	{
		DataDirector.instance.ResetSettingTypeToDefault(this.settingType);
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(this.menuPage.menuPageIndex);
		if (this.settingType == DataDirector.SettingType.Graphics)
		{
			GraphicsManager.instance.UpdateAll();
			return;
		}
		if (this.settingType == DataDirector.SettingType.Gameplay)
		{
			GameplayManager.instance.UpdateAll();
			return;
		}
		if (this.settingType == DataDirector.SettingType.Audio)
		{
			AudioManager.instance.UpdateAll();
		}
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00098DF6 File Offset: 0x00096FF6
	public void SaveSettings()
	{
		DataDirector.instance.SaveSettings();
	}

	// Token: 0x04001C5D RID: 7261
	private MenuPage menuPage;

	// Token: 0x04001C5E RID: 7262
	public DataDirector.SettingType settingType;

	// Token: 0x04001C5F RID: 7263
	private bool saveSettings;
}
