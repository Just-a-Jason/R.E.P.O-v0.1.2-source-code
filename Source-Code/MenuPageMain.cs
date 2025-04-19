using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class MenuPageMain : MonoBehaviour
{
	// Token: 0x060010F2 RID: 4338 RVA: 0x00097ECF File Offset: 0x000960CF
	private void Awake()
	{
		MenuPageMain.instance = this;
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00097ED8 File Offset: 0x000960D8
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuPage = base.GetComponent<MenuPage>();
		if (MainMenuOpen.instance.firstOpen)
		{
			MainMenuOpen.instance.firstOpen = false;
			this.menuPage.disableOutroAnimation = false;
		}
		else
		{
			this.menuPage.disableIntroAnimation = false;
			this.menuPage.disableOutroAnimation = false;
			this.doIntroAnimation = false;
		}
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.TutorialPlayed) <= 0)
		{
			this.tutorialButtonBlinkActive = true;
			this.tutorialButton.customColors = true;
			this.tutorialButton.colorNormal = new Color(1f, 0.55f, 0f);
			this.tutorialButton.colorHover = Color.white;
			this.tutorialButton.colorClick = new Color(1f, 0.55f, 0f);
		}
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00097FB4 File Offset: 0x000961B4
	private void Update()
	{
		if (this.tutorialButtonBlinkActive)
		{
			if (this.tutorialButtonTimer <= 0f)
			{
				this.tutorialButtonTimer = 0.5f;
				this.tutorialButtonBlink = !this.tutorialButtonBlink;
				if (this.tutorialButtonBlink)
				{
					this.tutorialButton.colorNormal = Color.white;
				}
				else
				{
					this.tutorialButton.colorNormal = new Color(1f, 0.55f, 0f);
				}
			}
			else
			{
				this.tutorialButtonTimer -= Time.deltaTime;
			}
		}
		if (this.menuPage.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		if (RunManager.instance.localMultiplayerTest)
		{
			GameManager.instance.localTest = true;
			RunManager.instance.localMultiplayerTest = false;
			RunManager.instance.ResetProgress();
			RunManager.instance.waitToChangeScene = true;
			RunManager.instance.lobbyJoin = true;
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
			SteamManager.instance.joinLobby = false;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.popUpTimer > 0f)
		{
			this.popUpTimer -= Time.deltaTime;
			if (this.popUpTimer <= 0f)
			{
				MenuManager.instance.PagePopUpScheduledShow();
			}
		}
		if (this.doIntroAnimation)
		{
			this.waitTimer += Time.deltaTime;
			if (this.waitTimer > 3f)
			{
				this.animateIn = true;
			}
			else
			{
				this.rectTransform.localPosition = new Vector3(-600f, 0f, 0f);
			}
			if (this.animateIn)
			{
				this.rectTransform.localPosition = new Vector3(Mathf.Lerp(this.rectTransform.localPosition.x, 0f, Time.deltaTime * 2f), 0f, 0f);
				if (Mathf.Abs(this.rectTransform.localPosition.x) < 50f && !this.introDone)
				{
					this.menuPage.PageStateSet(MenuPage.PageState.Active);
					this.introDone = true;
				}
			}
		}
		else if (!this.introDone)
		{
			this.menuPage.PageStateSet(MenuPage.PageState.Active);
			this.introDone = true;
		}
		if (SteamManager.instance.joinLobby)
		{
			if (this.joinLobbyTimer > 0f)
			{
				this.joinLobbyTimer -= Time.deltaTime;
				return;
			}
			GameManager.instance.localTest = false;
			RunManager.instance.ResetProgress();
			RunManager.instance.waitToChangeScene = true;
			RunManager.instance.lobbyJoin = true;
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
			SteamManager.instance.joinLobby = false;
		}
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00098248 File Offset: 0x00096448
	public void ButtonEventSinglePlayer()
	{
		SemiFunc.MainMenuSetSingleplayer();
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Saves, false);
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00098267 File Offset: 0x00096467
	public void ButtonEventTutorial()
	{
		DataDirector.instance.TutorialPlayed();
		TutorialDirector.instance.Reset();
		RunManager.instance.ResetProgress();
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Tutorial);
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00098294 File Offset: 0x00096494
	public void ButtonEventHostGame()
	{
		SemiFunc.MainMenuSetMultiplayer();
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Saves, false);
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000982B3 File Offset: 0x000964B3
	public void ButtonEventJoinGame()
	{
		SteamManager.instance.OpenSteamOverlayToLobby();
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x000982C0 File Offset: 0x000964C0
	public void ButtonEventQuit()
	{
		RunManager.instance.skipLoadingUI = true;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.quitApplication = true;
			playerAvatar.OutroStartRPC();
		}
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00098328 File Offset: 0x00096528
	public void ButtonEventSettings()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Settings);
	}

	// Token: 0x04001C31 RID: 7217
	public static MenuPageMain instance;

	// Token: 0x04001C32 RID: 7218
	private RectTransform rectTransform;

	// Token: 0x04001C33 RID: 7219
	private float waitTimer;

	// Token: 0x04001C34 RID: 7220
	private bool animateIn;

	// Token: 0x04001C35 RID: 7221
	internal MenuPage menuPage;

	// Token: 0x04001C36 RID: 7222
	private bool introDone;

	// Token: 0x04001C37 RID: 7223
	public GameObject networkConnectPrefab;

	// Token: 0x04001C38 RID: 7224
	private float joinLobbyTimer = 0.1f;

	// Token: 0x04001C39 RID: 7225
	private float popUpTimer = 1.5f;

	// Token: 0x04001C3A RID: 7226
	private bool doIntroAnimation = true;

	// Token: 0x04001C3B RID: 7227
	public MenuButton tutorialButton;

	// Token: 0x04001C3C RID: 7228
	private bool tutorialButtonBlinkActive;

	// Token: 0x04001C3D RID: 7229
	private bool tutorialButtonBlink;

	// Token: 0x04001C3E RID: 7230
	private float tutorialButtonTimer;
}
