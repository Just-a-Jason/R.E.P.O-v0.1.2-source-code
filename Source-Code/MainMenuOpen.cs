using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class MainMenuOpen : MonoBehaviour
{
	// Token: 0x060010B8 RID: 4280 RVA: 0x00096A12 File Offset: 0x00094C12
	private void Awake()
	{
		MainMenuOpen.instance = this;
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x00096A1A File Offset: 0x00094C1A
	public void NetworkConnect()
	{
		Object.Instantiate<GameObject>(this.networkConnectPrefab);
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x00096A28 File Offset: 0x00094C28
	private void Start()
	{
		MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x00096A37 File Offset: 0x00094C37
	public void MainMenuSetState(int state)
	{
		this.mainMenuGameModeState = (MainMenuOpen.MainMenuGameModeState)state;
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x00096A40 File Offset: 0x00094C40
	public MainMenuOpen.MainMenuGameModeState MainMenuGetState()
	{
		return this.mainMenuGameModeState;
	}

	// Token: 0x04001BE9 RID: 7145
	public static MainMenuOpen instance;

	// Token: 0x04001BEA RID: 7146
	public GameObject networkConnectPrefab;

	// Token: 0x04001BEB RID: 7147
	internal bool firstOpen = true;

	// Token: 0x04001BEC RID: 7148
	public MainMenuOpen.MainMenuGameModeState mainMenuGameModeState;

	// Token: 0x0200039B RID: 923
	public enum MainMenuGameModeState
	{
		// Token: 0x04002818 RID: 10264
		SinglePlayer,
		// Token: 0x04002819 RID: 10265
		MultiPlayer
	}
}
