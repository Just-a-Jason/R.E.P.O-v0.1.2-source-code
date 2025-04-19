using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001E6 RID: 486
public class MenuPage : MonoBehaviour
{
	// Token: 0x06001044 RID: 4164 RVA: 0x00092CC4 File Offset: 0x00090EC4
	private void Start()
	{
		this.selectionBox = base.GetComponentInChildren<MenuSelectionBox>();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.originalPosition = this.rectTransform.localPosition;
		this.animateAwayPosition = new Vector2(this.originalPosition.x, this.originalPosition.y - this.rectTransform.rect.height);
		this.rectTransform.localPosition = new Vector2(this.originalPosition.x, this.originalPosition.y + this.rectTransform.rect.height);
		MenuManager.instance.PageAdd(this);
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00092D8B File Offset: 0x00090F8B
	private IEnumerator LateStart()
	{
		yield return null;
		if (!this.parentPage)
		{
			this.parentPage = this;
		}
		yield break;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00092D9A File Offset: 0x00090F9A
	private void FixedUpdate()
	{
		if (this.pageActiveTimer <= 0f)
		{
			this.pageActive = false;
		}
		if (this.pageActiveTimer > 0f)
		{
			this.pageActive = true;
			this.pageActiveTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00092DD8 File Offset: 0x00090FD8
	private void Update()
	{
		switch (this.currentPageState)
		{
		case MenuPage.PageState.Opening:
			this.StateOpening();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Active:
			this.StateActive();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Closing:
			this.StateClosing();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Inactive:
			this.StateInactive();
			this.stateStart = false;
			break;
		case MenuPage.PageState.Activating:
			this.StateActivating();
			this.stateStart = false;
			break;
		}
		if (this.activeSettingElementTimer > 0f)
		{
			this.activeSettingElementTimer -= Time.deltaTime;
			if (this.activeSettingElementTimer <= 0f)
			{
				this.currentActiveSettingElement = -1;
			}
		}
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00092E84 File Offset: 0x00091084
	private void OnEnable()
	{
		this.ResetPage();
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x00092E8C File Offset: 0x0009108C
	public void ResetPage()
	{
		if (!this.rectTransform)
		{
			return;
		}
		this.rectTransform.localPosition = new Vector2(this.originalPosition.x, this.originalPosition.y + this.rectTransform.rect.height);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00092EE6 File Offset: 0x000910E6
	public void PageStateSet(MenuPage.PageState pageState)
	{
		this.stateTimer = 0f;
		this.currentPageState = pageState;
		this.stateStart = true;
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00092F04 File Offset: 0x00091104
	private void StateOpening()
	{
		if (this.stateStart)
		{
			if (!this.popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageIntro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpOpen();
			}
			this.LockAndHide();
		}
		if (!this.addedPageOnTop)
		{
			MenuSelectionBox.instance.firstSelection = true;
		}
		if (Vector2.Distance(this.rectTransform.localPosition, this.originalPosition) < 0.8f)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
		if (!this.disableIntroAnimation)
		{
			float deltaTime = Time.deltaTime;
			this.rectTransform.localPosition = Vector2.Lerp(this.rectTransform.localPosition, this.originalPosition, 40f * deltaTime);
		}
		this.LockAndHide();
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00092FC0 File Offset: 0x000911C0
	private void StateActive()
	{
		if (this.stateStart && !this.disableIntroAnimation)
		{
			this.rectTransform.localPosition = this.originalPosition;
		}
		if (!this.disableIntroAnimation)
		{
			this.rectTransform.localPosition = this.originalPosition;
		}
		this.PageAddedOnTopLogic();
		MenuSelectionBox instance = MenuSelectionBox.instance;
		if (!instance || instance != this.selectionBox)
		{
			this.selectionBox.Reinstate();
		}
		this.LockAndHide();
		if (MenuManager.instance.currentMenuPageIndex != this.menuPageIndex)
		{
			this.PageStateSet(MenuPage.PageState.Inactive);
		}
		this.pageActive = true;
		this.pageActiveTimer = 0.1f;
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x0009306F File Offset: 0x0009126F
	public bool SettingElementActiveCheckFree(int index)
	{
		return this.currentActiveSettingElement == -1 || this.currentActiveSettingElement == index;
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00093088 File Offset: 0x00091288
	private void StateClosing()
	{
		this.LockAndHide();
		if (this.stateStart)
		{
			if (!this.popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageOutro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpClose();
			}
			if (MenuManager.instance.currentMenuPage == this)
			{
				MenuManager.instance.currentMenuPage = null;
				MenuManager.instance.PageRemove(this);
			}
		}
		if (Vector2.Distance(this.rectTransform.localPosition, this.animateAwayPosition) < 0.8f)
		{
			this.onPageEnd.Invoke();
			MenuManager.instance.PageRemove(this);
			Object.Destroy(base.gameObject);
		}
		float deltaTime = Time.deltaTime;
		this.rectTransform.localPosition = Vector2.Lerp(this.rectTransform.localPosition, this.animateAwayPosition, 40f * deltaTime);
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x00093164 File Offset: 0x00091364
	private void StateInactive()
	{
		bool flag = this.stateStart;
		if (MenuManager.instance.currentMenuPageIndex == this.menuPageIndex)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x00093188 File Offset: 0x00091388
	private void PageAddedOnTopLogic()
	{
		if (this.currentPageState == MenuPage.PageState.Opening || this.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		if (this.addedPageOnTop)
		{
			if (this.parentPage)
			{
				if (this.currentPageState != this.parentPage.currentPageState)
				{
					this.PageStateSet(this.parentPage.currentPageState);
					return;
				}
			}
			else if (this.currentPageState != MenuPage.PageState.Closing)
			{
				this.PageStateSet(MenuPage.PageState.Closing);
			}
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x000931F1 File Offset: 0x000913F1
	private void StateActivating()
	{
		bool flag = this.stateStart;
		if (this.stateTimer > 0.1f)
		{
			this.PageStateSet(MenuPage.PageState.Active);
		}
		this.stateTimer += Time.deltaTime;
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x00093220 File Offset: 0x00091420
	public void SettingElementActiveSet(int index)
	{
		if (this.currentActiveSettingElement == -1)
		{
			this.currentActiveSettingElement = index;
		}
		this.activeSettingElementTimer = 0.1f;
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x0009323D File Offset: 0x0009143D
	private void LockAndHide()
	{
		SemiFunc.UIHideAim();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideGoal();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideShopCost();
		SemiFunc.UIHideTumble();
		SemiFunc.UIHideWorldSpace();
		SemiFunc.UIHideValuableDiscover();
		SemiFunc.CameraOverrideStopAim();
	}

	// Token: 0x04001B22 RID: 6946
	public string menuHeaderName;

	// Token: 0x04001B23 RID: 6947
	public MenuPageIndex menuPageIndex;

	// Token: 0x04001B24 RID: 6948
	private Vector2 originalPosition;

	// Token: 0x04001B25 RID: 6949
	internal RectTransform rectTransform;

	// Token: 0x04001B26 RID: 6950
	private Vector2 animateAwayPosition = new Vector2(0f, 0f);

	// Token: 0x04001B27 RID: 6951
	private Vector2 targetPosition;

	// Token: 0x04001B28 RID: 6952
	internal float bottomElementYPos;

	// Token: 0x04001B29 RID: 6953
	internal List<MenuSelectableElement> selectableElements = new List<MenuSelectableElement>();

	// Token: 0x04001B2A RID: 6954
	public TextMeshProUGUI menuHeader;

	// Token: 0x04001B2B RID: 6955
	internal bool pageIsOnTopOfOtherPage;

	// Token: 0x04001B2C RID: 6956
	internal MenuPage pageUnderThisPage;

	// Token: 0x04001B2D RID: 6957
	internal bool pageActive;

	// Token: 0x04001B2E RID: 6958
	private float pageActiveTimer;

	// Token: 0x04001B2F RID: 6959
	internal bool popUpAnimation;

	// Token: 0x04001B30 RID: 6960
	internal MenuSelectionBox selectionBox;

	// Token: 0x04001B31 RID: 6961
	private float stateTimer;

	// Token: 0x04001B32 RID: 6962
	internal bool addedPageOnTop;

	// Token: 0x04001B33 RID: 6963
	internal MenuPage parentPage;

	// Token: 0x04001B34 RID: 6964
	public bool disableIntroAnimation;

	// Token: 0x04001B35 RID: 6965
	public bool disableOutroAnimation;

	// Token: 0x04001B36 RID: 6966
	internal List<MenuSettingElement> settingElements = new List<MenuSettingElement>();

	// Token: 0x04001B37 RID: 6967
	internal int currentActiveSettingElement = -1;

	// Token: 0x04001B38 RID: 6968
	private float activeSettingElementTimer;

	// Token: 0x04001B39 RID: 6969
	public UnityEvent onPageEnd;

	// Token: 0x04001B3A RID: 6970
	internal int scrollBoxes;

	// Token: 0x04001B3B RID: 6971
	private bool stateStart = true;

	// Token: 0x04001B3C RID: 6972
	internal MenuPage.PageState currentPageState;

	// Token: 0x02000396 RID: 918
	public enum PageState
	{
		// Token: 0x04002806 RID: 10246
		Opening,
		// Token: 0x04002807 RID: 10247
		Active,
		// Token: 0x04002808 RID: 10248
		Closing,
		// Token: 0x04002809 RID: 10249
		Inactive,
		// Token: 0x0400280A RID: 10250
		Activating,
		// Token: 0x0400280B RID: 10251
		Closed
	}
}
