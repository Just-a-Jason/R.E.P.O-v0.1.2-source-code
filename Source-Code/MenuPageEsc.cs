using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class MenuPageEsc : MonoBehaviour
{
	// Token: 0x060010DA RID: 4314 RVA: 0x0009739F File Offset: 0x0009559F
	private void Start()
	{
		MenuPageEsc.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.PlayerGainSlidersUpdate();
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x000973B9 File Offset: 0x000955B9
	private void Update()
	{
		if (SemiFunc.MenuLevel())
		{
			this.menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x000973CE File Offset: 0x000955CE
	public void ButtonEventContinue()
	{
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x000973DC File Offset: 0x000955DC
	public void PlayerGainSlidersUpdate()
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar playerAvatar in this.playerMicGainSliders.Keys)
		{
			if (!playerAvatar || !this.playerMicGainSliders[playerAvatar])
			{
				list.Add(playerAvatar);
			}
		}
		foreach (PlayerAvatar key in list)
		{
			this.playerMicGainSliders.Remove(key);
		}
		foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
		{
			if (!this.playerMicGainSliders.ContainsKey(playerAvatar2) && !playerAvatar2.isLocal)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.playerMicrophoneVolumeSliderPrefab, base.transform);
				gameObject.transform.localPosition = new Vector3(340f, 30f, 0f);
				gameObject.transform.localPosition += new Vector3(0f, 42f * (float)this.playerMicGainSliders.Count, 0f);
				MenuSliderPlayerMicGain component = gameObject.GetComponent<MenuSliderPlayerMicGain>();
				component.playerAvatar = playerAvatar2;
				component.SliderNameSet(playerAvatar2.playerName + " VOICE");
				this.playerMicGainSliders.Add(playerAvatar2, component);
			}
		}
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x0009759C File Offset: 0x0009579C
	public void ButtonEventSelfDestruct()
	{
		if (SemiFunc.IsMultiplayer())
		{
			ChatManager.instance.PossessSelfDestruction();
		}
		else
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		}
		MenuManager.instance.PageCloseAll();
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x000975E8 File Offset: 0x000957E8
	public void ButtonEventQuit()
	{
		RunManager.instance.skipLoadingUI = true;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.quitApplication = true;
			playerAvatar.OutroStartRPC();
		}
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00097650 File Offset: 0x00095850
	public void ButtonEventQuitToMenu()
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.OutroStartRPC();
		}
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x000976B0 File Offset: 0x000958B0
	public void ButtonEventChangeColor()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Color);
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x000976BE File Offset: 0x000958BE
	public void ButtonEventSettings()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Settings);
	}

	// Token: 0x04001C1C RID: 7196
	public static MenuPageEsc instance;

	// Token: 0x04001C1D RID: 7197
	internal MenuPage menuPage;

	// Token: 0x04001C1E RID: 7198
	public GameObject playerMicrophoneVolumeSliderPrefab;

	// Token: 0x04001C1F RID: 7199
	internal Dictionary<PlayerAvatar, MenuSliderPlayerMicGain> playerMicGainSliders = new Dictionary<PlayerAvatar, MenuSliderPlayerMicGain>();
}
