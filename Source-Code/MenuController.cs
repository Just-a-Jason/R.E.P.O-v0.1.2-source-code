using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001DF RID: 479
public class MenuController : MonoBehaviour
{
	// Token: 0x06001001 RID: 4097 RVA: 0x00091B81 File Offset: 0x0008FD81
	private void Awake()
	{
		MenuController.instance = this;
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x00091B8C File Offset: 0x0008FD8C
	private void Update()
	{
		SemiFunc.CursorUnlock(0.1f);
		if (this.picked)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button0))
		{
			this.OnSinglePlayerPicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Joystick1Button1))
		{
			this.OnMultiplayerPicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || SteamManager.instance.joinLobby)
		{
			this.OnMultiplayerOnlinePicked();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			this.OnTutorialPicked();
		}
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00091C0F File Offset: 0x0008FE0F
	public void OnSinglePlayerPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00091C4C File Offset: 0x0008FE4C
	public void OnMultiplayerPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(1);
		GameManager.instance.localTest = true;
		Object.Instantiate<GameObject>(this.networkConnectPrefab);
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x00091CA0 File Offset: 0x0008FEA0
	public void OnMultiplayerOnlinePicked()
	{
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x00091CA4 File Offset: 0x0008FEA4
	public void OnTutorialPicked()
	{
		this.picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Tutorial);
	}

	// Token: 0x04001AE1 RID: 6881
	public static MenuController instance;

	// Token: 0x04001AE2 RID: 6882
	public GameObject networkConnectPrefab;

	// Token: 0x04001AE3 RID: 6883
	private bool picked;
}
