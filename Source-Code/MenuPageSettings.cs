using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class MenuPageSettings : MonoBehaviour
{
	// Token: 0x06001119 RID: 4377 RVA: 0x00098E0A File Offset: 0x0009700A
	private void Start()
	{
		MenuPageSettings.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00098E1E File Offset: 0x0009701E
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.ButtonEventBack();
		}
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00098E2F File Offset: 0x0009702F
	public void ButtonEventGameplay()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsGameplay);
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00098E46 File Offset: 0x00097046
	public void ButtonEventAudio()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsAudio);
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00098E60 File Offset: 0x00097060
	public void ButtonEventBack()
	{
		if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu)
		{
			MenuManager.instance.PageCloseAllExcept(MenuPageIndex.Main);
			MenuManager.instance.PageSetCurrent(MenuPageIndex.Main, MenuPageMain.instance.menuPage);
			return;
		}
		if (RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu)
		{
			MenuManager.instance.PageCloseAllExcept(MenuPageIndex.Lobby);
			MenuManager.instance.PageSetCurrent(MenuPageIndex.Lobby, MenuPageLobby.instance.menuPage);
			return;
		}
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Escape, false);
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x00098EFC File Offset: 0x000970FC
	public void ButtonEventControls()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsControls);
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x00098F13 File Offset: 0x00097113
	public void ButtonEventGraphics()
	{
		MenuManager.instance.PageCloseAllAddedOnTop();
		MenuManager.instance.PageAddOnTop(MenuPageIndex.SettingsGraphics);
	}

	// Token: 0x04001C60 RID: 7264
	public static MenuPageSettings instance;

	// Token: 0x04001C61 RID: 7265
	internal MenuPage menuPage;
}
