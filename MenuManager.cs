using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class MenuManager : MonoBehaviour
{
	// Token: 0x06001019 RID: 4121 RVA: 0x00091FAD File Offset: 0x000901AD
	private void Awake()
	{
		if (!MenuManager.instance)
		{
			MenuManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x00091FD8 File Offset: 0x000901D8
	private void Start()
	{
		this.StateSet(MenuManager.MenuState.Closed);
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x00091FE4 File Offset: 0x000901E4
	private void Update()
	{
		if (PlayerController.instance)
		{
			this.soundPosition = PlayerController.instance.transform.position;
		}
		else
		{
			this.soundPosition = base.transform.position;
		}
		int num = this.currentMenuState;
		if (num != 0)
		{
			if (num == 1)
			{
				this.StateClosed();
				this.stateStart = false;
			}
		}
		else
		{
			this.StateOpen();
			this.stateStart = false;
		}
		if (Input.GetMouseButton(0))
		{
			if (this.mouseHoldPosition == Vector2.zero)
			{
				this.mouseHoldPosition = SemiFunc.UIMousePosToUIPos();
				return;
			}
		}
		else
		{
			this.mouseHoldPosition = Vector2.zero;
		}
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x00092082 File Offset: 0x00090282
	private void FixedUpdate()
	{
		if (this.menuHover > 0f)
		{
			this.menuHover -= Time.fixedDeltaTime;
			return;
		}
		this.currentMenuID = "";
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x000920AF File Offset: 0x000902AF
	public void SetState(int state)
	{
		this.currentMenuState = state;
		this.stateStart = true;
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x000920C0 File Offset: 0x000902C0
	public void MenuEffectHover(float pitch = -1f, float volume = -1f)
	{
		if (pitch != -1f)
		{
			this.soundHover.Pitch = pitch;
		}
		if (volume != -1f)
		{
			this.soundHover.Volume = volume;
		}
		this.soundHover.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00092120 File Offset: 0x00090320
	public void MenuEffectClick(MenuManager.MenuClickEffectType effectType, MenuPage parentPage = null, float pitch = -1f, float volume = -1f, bool soundOnly = false)
	{
		switch (effectType)
		{
		case MenuManager.MenuClickEffectType.Action:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(AssetManager.instance.colorYellow);
			}
			if (pitch != -1f)
			{
				this.soundAction.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundAction.Volume = volume;
			}
			this.soundAction.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Confirm:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(Color.green);
			}
			if (pitch != -1f)
			{
				this.soundConfirm.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundConfirm.Volume = volume;
			}
			this.soundConfirm.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Deny:
			if (!soundOnly && this.activeSelectionBox)
			{
				this.activeSelectionBox.SetClick(Color.red);
			}
			if (pitch != -1f)
			{
				this.soundDeny.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundDeny.Volume = volume;
			}
			this.soundDeny.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Dud:
			if (!soundOnly)
			{
				this.activeSelectionBox.SetClick(Color.gray);
			}
			if (pitch != -1f)
			{
				this.soundDud.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundDud.Volume = volume;
			}
			this.soundDud.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		case MenuManager.MenuClickEffectType.Tick:
			if (!soundOnly)
			{
				Color click = new Color(0f, 0.5f, 1f, 1f);
				if (!parentPage)
				{
					if (MenuSelectionBox.instance)
					{
						MenuSelectionBox.instance.SetClick(click);
					}
				}
				else if (parentPage.selectionBox)
				{
					parentPage.selectionBox.SetClick(click);
				}
			}
			if (pitch != -1f)
			{
				this.soundTick.Pitch = pitch;
			}
			if (volume != -1f)
			{
				this.soundTick.Volume = volume;
			}
			this.soundTick.Play(this.soundPosition, 1f, 1f, 1f, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000923B1 File Offset: 0x000905B1
	public void MenuEffectPopUpOpen()
	{
		this.soundWindowPopUp.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x000923D9 File Offset: 0x000905D9
	public void MenuEffectPopUpClose()
	{
		this.soundWindowPopUpClose.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x00092401 File Offset: 0x00090601
	public void MenuEffectPageIntro()
	{
		this.soundPageIntro.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00092429 File Offset: 0x00090629
	public void MenuEffectPageOutro()
	{
		this.soundPageOutro.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x00092451 File Offset: 0x00090651
	public void MenuEffectMove()
	{
		this.soundMove.Play(this.soundPosition, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00092479 File Offset: 0x00090679
	private void StateOpen()
	{
		bool flag = this.stateStart;
		SemiFunc.CursorUnlock(0.1f);
		PlayerController.instance.InputDisableTimer = 0.1f;
		if (!this.currentMenuPage)
		{
			this.StateSet(MenuManager.MenuState.Closed);
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x000924AF File Offset: 0x000906AF
	private void StateClosed()
	{
		bool flag = this.stateStart;
		if (this.currentMenuPage)
		{
			this.StateSet(MenuManager.MenuState.Open);
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000924CC File Offset: 0x000906CC
	public void PageAdd(MenuPage menuPage)
	{
		if (this.allPages.Contains(menuPage))
		{
			return;
		}
		this.allPages.Add(menuPage);
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000924E9 File Offset: 0x000906E9
	public void PageRemove(MenuPage menuPage)
	{
		if (!this.allPages.Contains(menuPage))
		{
			return;
		}
		this.allPages.Remove(menuPage);
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00092508 File Offset: 0x00090708
	public MenuPage PageOpen(MenuPageIndex menuPageIndex, bool addedPageOnTop = false)
	{
		MenuManager.MenuPages menuPages = this.menuPages.Find((MenuManager.MenuPages x) => x.menuPageIndex == menuPageIndex);
		if (menuPages == null)
		{
			Debug.LogError("Page not found");
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(menuPages.menuPage);
		MenuPage component = gameObject.GetComponent<MenuPage>();
		gameObject.transform.SetParent(MenuHolder.instance.transform);
		gameObject.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		component.addedPageOnTop = addedPageOnTop;
		if (!addedPageOnTop)
		{
			MenuManager.instance.PageSetCurrent(menuPageIndex, component);
		}
		else
		{
			component.parentPage = this.currentMenuPage;
		}
		return component;
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x000925D8 File Offset: 0x000907D8
	public void PageClose(MenuPageIndex menuPageIndex)
	{
		if (this.menuPages.Find((MenuManager.MenuPages x) => x.menuPageIndex == menuPageIndex) == null)
		{
			return;
		}
		MenuPage menuPage = this.allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex);
		if (menuPage == null)
		{
			return;
		}
		menuPage.PageStateSet(MenuPage.PageState.Closing);
		this.allPages.Remove(menuPage);
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00092644 File Offset: 0x00090844
	public bool PageCheck(MenuPageIndex menuPageIndex)
	{
		return this.allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex) != null;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x0009267B File Offset: 0x0009087B
	public void PageSwap(MenuPageIndex menuPageIndex)
	{
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Closing);
		this.PageOpen(menuPageIndex, false);
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x00092694 File Offset: 0x00090894
	public MenuPage PageOpenOnTop(MenuPageIndex menuPageIndex)
	{
		MenuPage menuPage = this.currentMenuPage;
		this.PageInactiveAdd(menuPage);
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage2 = this.PageOpen(menuPageIndex, false);
		menuPage2.pageIsOnTopOfOtherPage = true;
		menuPage2.pageUnderThisPage = menuPage;
		return menuPage2;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x000926D4 File Offset: 0x000908D4
	public void PageAddOnTop(MenuPageIndex menuPageIndex)
	{
		if (this.addedPagesOnTop.Contains(this.currentMenuPage))
		{
			return;
		}
		MenuPage menuPage = this.currentMenuPage;
		MenuPage item = this.PageOpen(menuPageIndex, true);
		if (!this.addedPagesOnTop.Contains(this.currentMenuPage))
		{
			this.addedPagesOnTop.Add(item);
		}
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00092724 File Offset: 0x00090924
	public void PagePopUpTwoOptions(MenuButtonPopUp menuButtonPopUp, string popUpHeader, Color popUpHeaderColor, string popUpText, string option1Text, string option2Text)
	{
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUpTwoOptions;
		MenuPage pageUnderThisPage = this.currentMenuPage;
		this.currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage = this.PageOpen(menuPageIndex, false);
		menuPage.pageIsOnTopOfOtherPage = true;
		menuPage.pageUnderThisPage = pageUnderThisPage;
		MenuPageTwoOptions component = menuPage.GetComponent<MenuPageTwoOptions>();
		menuPage.menuHeaderName = popUpHeader;
		menuPage.menuHeader.text = popUpHeader;
		menuPage.menuHeader.color = popUpHeaderColor;
		component.option1Event = menuButtonPopUp.option1Event;
		component.option2Event = menuButtonPopUp.option2Event;
		component.bodyTextMesh.text = popUpText;
		component.option1Button.buttonTextString = option1Text;
		component.option2Button.buttonTextString = option2Text;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x000927C0 File Offset: 0x000909C0
	public void MenuHover()
	{
		this.menuHover = 0.1f;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x000927CD File Offset: 0x000909CD
	public void PageSetCurrent(MenuPageIndex menuPageIndex, MenuPage menuPage)
	{
		this.currentMenuPageIndex = menuPageIndex;
		this.currentMenuPage = menuPage;
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x000927DD File Offset: 0x000909DD
	public void StateSet(MenuManager.MenuState state)
	{
		this.currentMenuState = (int)state;
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000927E6 File Offset: 0x000909E6
	private void PageInactiveAdd(MenuPage menuPage)
	{
		if (this.inactivePages.Contains(menuPage))
		{
			return;
		}
		this.inactivePages.Add(menuPage);
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x00092803 File Offset: 0x00090A03
	private void PageInactiveRemove(MenuPage menuPage)
	{
		if (!this.inactivePages.Contains(menuPage))
		{
			return;
		}
		this.inactivePages.Remove(menuPage);
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00092824 File Offset: 0x00090A24
	public void PageReactivatePageUnderThisPage(MenuPage _menuPage)
	{
		if (this.currentMenuPage != _menuPage)
		{
			return;
		}
		if (this.currentMenuPage.pageUnderThisPage)
		{
			if (this.currentMenuPage.pageUnderThisPage.currentPageState == MenuPage.PageState.Inactive)
			{
				this.currentMenuPage.pageUnderThisPage.PageStateSet(MenuPage.PageState.Activating);
			}
			this.PageSetCurrent(this.currentMenuPage.pageUnderThisPage.menuPageIndex, this.currentMenuPage.pageUnderThisPage);
		}
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00092898 File Offset: 0x00090A98
	public void PageCloseAllExcept(MenuPageIndex menuPageIndex)
	{
		foreach (MenuPage menuPage in this.allPages)
		{
			if (menuPage.menuPageIndex != menuPageIndex)
			{
				menuPage.PageStateSet(MenuPage.PageState.Closing);
			}
		}
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000928F4 File Offset: 0x00090AF4
	public void PageCloseAll()
	{
		foreach (MenuPage menuPage in this.allPages)
		{
			menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x00092948 File Offset: 0x00090B48
	public void PageCloseAllAddedOnTop()
	{
		foreach (MenuPage menuPage in this.addedPagesOnTop)
		{
			menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0009299C File Offset: 0x00090B9C
	public void PlayerHeadAdd(MenuPlayerHead head)
	{
		if (!this.playerHeads.Contains(head))
		{
			this.playerHeads.Add(head);
		}
		for (int i = 0; i < this.playerHeads.Count; i++)
		{
			if (this.playerHeads[i] == null)
			{
				this.playerHeads.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000929F9 File Offset: 0x00090BF9
	public void SetActiveSelectionBox(MenuSelectionBox selectBox)
	{
		this.activeSelectionBox = selectBox;
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x00092A04 File Offset: 0x00090C04
	public void PlayerHeadRemove(MenuPlayerHead head)
	{
		if (this.playerHeads.Contains(head))
		{
			this.playerHeads.Remove(head);
		}
		for (int i = 0; i < this.playerHeads.Count; i++)
		{
			if (this.playerHeads[i] == null)
			{
				this.playerHeads.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00092A62 File Offset: 0x00090C62
	public void PagePopUpScheduled(string headerText, Color headerColor, string bodyText, string buttonText)
	{
		this.pagePopUpScheduled = true;
		this.pagePopUpScheduledHeaderText = headerText;
		this.pagePopUpScheduledHeaderColor = headerColor;
		this.pagePopUpScheduledBodyText = bodyText;
		this.pagePopUpScheduledButtonText = buttonText;
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00092A88 File Offset: 0x00090C88
	public void PagePopUpScheduledShow()
	{
		if (!this.pagePopUpScheduled)
		{
			return;
		}
		this.PagePopUp(this.pagePopUpScheduledHeaderText, this.pagePopUpScheduledHeaderColor, this.pagePopUpScheduledBodyText, this.pagePopUpScheduledButtonText);
		this.PagePopUpScheduledReset();
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00092AB7 File Offset: 0x00090CB7
	public void PagePopUpScheduledReset()
	{
		this.pagePopUpScheduled = false;
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x00092AC0 File Offset: 0x00090CC0
	public void SelectionBoxAdd(MenuSelectionBox selectBox)
	{
		if (!this.selectionBoxes.Contains(selectBox))
		{
			this.selectionBoxes.Add(selectBox);
		}
		for (int i = 0; i < this.selectionBoxes.Count; i++)
		{
			if (this.selectionBoxes[i] == null)
			{
				this.selectionBoxes.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00092B20 File Offset: 0x00090D20
	public void SelectionBoxRemove(MenuSelectionBox selectBox)
	{
		if (this.selectionBoxes.Contains(selectBox))
		{
			this.selectionBoxes.Remove(selectBox);
		}
		for (int i = 0; i < this.selectionBoxes.Count; i++)
		{
			if (this.selectionBoxes[i] == null)
			{
				this.selectionBoxes.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00092B80 File Offset: 0x00090D80
	public MenuSelectionBox SelectionBoxGetCorrect(MenuPage parentPage, MenuScrollBox menuScrollBox)
	{
		return this.selectionBoxes.Find((MenuSelectionBox x) => x.menuPage == parentPage && x.menuScrollBox == menuScrollBox);
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00092BB8 File Offset: 0x00090DB8
	public void PagePopUp(string headerText, Color headerColor, string bodyText, string buttonText)
	{
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUp;
		MenuPage pageUnderThisPage = this.currentMenuPage;
		if (this.currentMenuPage.menuPageIndex == MenuPageIndex.PopUpTwoOptions)
		{
			pageUnderThisPage = this.currentMenuPage.pageUnderThisPage;
			this.currentMenuPage = pageUnderThisPage;
		}
		pageUnderThisPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage = this.PageOpen(menuPageIndex, false);
		menuPage.pageIsOnTopOfOtherPage = true;
		menuPage.pageUnderThisPage = pageUnderThisPage;
		MenuPagePopUp component = menuPage.GetComponent<MenuPagePopUp>();
		menuPage.menuHeaderName = headerText;
		menuPage.menuHeader.text = headerText;
		menuPage.menuHeader.color = headerColor;
		component.bodyTextMesh.text = bodyText;
		component.okButton.buttonTextString = buttonText;
		this.currentMenuPage = menuPage;
		this.MenuEffectPopUpOpen();
	}

	// Token: 0x04001AFD RID: 6909
	public static MenuManager instance;

	// Token: 0x04001AFE RID: 6910
	internal MenuSelectionBox selectionBox;

	// Token: 0x04001AFF RID: 6911
	internal MenuSelectionBox activeSelectionBox;

	// Token: 0x04001B00 RID: 6912
	internal List<MenuPlayerHead> playerHeads = new List<MenuPlayerHead>();

	// Token: 0x04001B01 RID: 6913
	private List<MenuSelectionBox> selectionBoxes = new List<MenuSelectionBox>();

	// Token: 0x04001B02 RID: 6914
	internal string currentMenuID = "";

	// Token: 0x04001B03 RID: 6915
	public List<MenuManager.MenuPages> menuPages;

	// Token: 0x04001B04 RID: 6916
	internal MenuPageIndex currentMenuPageIndex;

	// Token: 0x04001B05 RID: 6917
	internal MenuPage currentMenuPage;

	// Token: 0x04001B06 RID: 6918
	internal int currentMenuState;

	// Token: 0x04001B07 RID: 6919
	private bool stateStart;

	// Token: 0x04001B08 RID: 6920
	internal MenuButton currentButton;

	// Token: 0x04001B09 RID: 6921
	internal int fetchSetting;

	// Token: 0x04001B0A RID: 6922
	private Vector3 soundPosition;

	// Token: 0x04001B0B RID: 6923
	private float menuHover;

	// Token: 0x04001B0C RID: 6924
	public Sound soundAction;

	// Token: 0x04001B0D RID: 6925
	public Sound soundConfirm;

	// Token: 0x04001B0E RID: 6926
	public Sound soundDeny;

	// Token: 0x04001B0F RID: 6927
	public Sound soundDud;

	// Token: 0x04001B10 RID: 6928
	public Sound soundTick;

	// Token: 0x04001B11 RID: 6929
	public Sound soundHover;

	// Token: 0x04001B12 RID: 6930
	public Sound soundPageIntro;

	// Token: 0x04001B13 RID: 6931
	public Sound soundPageOutro;

	// Token: 0x04001B14 RID: 6932
	public Sound soundWindowPopUp;

	// Token: 0x04001B15 RID: 6933
	public Sound soundWindowPopUpClose;

	// Token: 0x04001B16 RID: 6934
	public Sound soundMove;

	// Token: 0x04001B17 RID: 6935
	internal Vector2 mouseHoldPosition;

	// Token: 0x04001B18 RID: 6936
	internal int screenUIWidth = 720;

	// Token: 0x04001B19 RID: 6937
	internal int screenUIHeight = 405;

	// Token: 0x04001B1A RID: 6938
	internal List<MenuPage> allPages = new List<MenuPage>();

	// Token: 0x04001B1B RID: 6939
	internal List<MenuPage> inactivePages = new List<MenuPage>();

	// Token: 0x04001B1C RID: 6940
	internal List<MenuPage> addedPagesOnTop = new List<MenuPage>();

	// Token: 0x04001B1D RID: 6941
	private bool pagePopUpScheduled;

	// Token: 0x04001B1E RID: 6942
	private string pagePopUpScheduledHeaderText;

	// Token: 0x04001B1F RID: 6943
	private Color pagePopUpScheduledHeaderColor;

	// Token: 0x04001B20 RID: 6944
	private string pagePopUpScheduledBodyText;

	// Token: 0x04001B21 RID: 6945
	private string pagePopUpScheduledButtonText;

	// Token: 0x0200038F RID: 911
	[Serializable]
	public class MenuPages
	{
		// Token: 0x040027F5 RID: 10229
		public MenuPageIndex menuPageIndex;

		// Token: 0x040027F6 RID: 10230
		public GameObject menuPage;
	}

	// Token: 0x02000390 RID: 912
	public enum MenuState
	{
		// Token: 0x040027F8 RID: 10232
		Open,
		// Token: 0x040027F9 RID: 10233
		Closed
	}

	// Token: 0x02000391 RID: 913
	public enum MenuClickEffectType
	{
		// Token: 0x040027FB RID: 10235
		Action,
		// Token: 0x040027FC RID: 10236
		Confirm,
		// Token: 0x040027FD RID: 10237
		Deny,
		// Token: 0x040027FE RID: 10238
		Dud,
		// Token: 0x040027FF RID: 10239
		Tick
	}
}
