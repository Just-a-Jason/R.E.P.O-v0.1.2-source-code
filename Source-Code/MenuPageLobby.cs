using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class MenuPageLobby : MonoBehaviour
{
	// Token: 0x060010E4 RID: 4324 RVA: 0x000976DE File Offset: 0x000958DE
	private void Awake()
	{
		MenuPageLobby.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
		this.roomNameText.text = PhotonNetwork.CloudRegion + " " + PhotonNetwork.CurrentRoom.Name;
		this.UpdateChatPrompt();
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x0009771C File Offset: 0x0009591C
	private void Start()
	{
		if (!SemiFunc.IsMasterClient())
		{
			this.inviteButton.transform.localPosition = new Vector3(this.startButton.transform.localPosition.x + 40f, this.startButton.transform.localPosition.y, this.startButton.transform.localPosition.z);
			this.inviteButton.buttonText.alignment = TextAlignmentOptions.Right;
			this.startButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x000977B4 File Offset: 0x000959B4
	private void Update()
	{
		if (this.joiningPlayersTimer > 0f)
		{
			this.joiningPlayersTimer -= Time.deltaTime;
		}
		else if (this.joiningPlayers.Count > 0)
		{
			this.joiningPlayers.Clear();
		}
		if (this.joiningPlayers.Count > 0 || this.joiningPlayersEndTimer > 0f)
		{
			this.joiningPlayer = true;
			this.joiningPlayersCanvasGroup.alpha = Mathf.Lerp(this.joiningPlayersCanvasGroup.alpha, 1f, Time.deltaTime * 10f);
			this.startButton.disabled = true;
		}
		else
		{
			this.joiningPlayersCanvasGroup.alpha = Mathf.Lerp(this.joiningPlayersCanvasGroup.alpha, 0f, Time.deltaTime * 10f);
			this.joiningPlayer = false;
			this.startButton.disabled = false;
		}
		if (this.joiningPlayersEndTimer > 0f)
		{
			this.joiningPlayersEndTimer -= Time.deltaTime;
		}
		this.listCheckTimer -= Time.deltaTime;
		if (this.listCheckTimer <= 0f)
		{
			this.listCheckTimer = 1f;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in list)
			{
				if (!this.lobbyPlayers.Contains(playerAvatar) && playerAvatar.playerAvatarVisuals.colorSet)
				{
					this.PlayerAdd(playerAvatar);
					flag = true;
				}
			}
			foreach (PlayerAvatar playerAvatar2 in this.lobbyPlayers.ToList<PlayerAvatar>())
			{
				if (!list.Contains(playerAvatar2))
				{
					this.PlayerRemove(playerAvatar2);
					flag = true;
				}
			}
			if (flag)
			{
				this.listObjects.Sort((GameObject a, GameObject b) => a.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID.CompareTo(b.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID));
				for (int i = 0; i < this.listObjects.Count; i++)
				{
					this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
					this.listObjects[i].transform.SetSiblingIndex(i);
				}
			}
			foreach (GameObject gameObject in this.listObjects)
			{
				PlayerAvatar playerAvatar3 = gameObject.GetComponent<MenuPlayerListed>().playerAvatar;
				if (playerAvatar3)
				{
					if (playerAvatar3.photonView.Owner == PhotonNetwork.MasterClient)
					{
						gameObject.GetComponent<MenuPlayerListed>().playerName.text = playerAvatar3.playerName + " <color=#331100>[HOST]</color>";
					}
					else
					{
						gameObject.GetComponent<MenuPlayerListed>().playerName.text = playerAvatar3.playerName;
					}
					this.SetPingText(gameObject.GetComponent<MenuPlayerListed>().pingText, playerAvatar3.playerPing);
				}
			}
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x00097AD8 File Offset: 0x00095CD8
	private void PlayerAdd(PlayerAvatar player)
	{
		this.lobbyPlayers.Add(player);
		GameObject gameObject = Object.Instantiate<GameObject>(this.menuPlayerListedPrefab, base.transform);
		MenuPlayerListed component = gameObject.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		component.GetComponent<RectTransform>().SetParent(this.playerListTransform);
		MenuSliderPlayerMicGain componentInChildren = component.GetComponentInChildren<MenuSliderPlayerMicGain>();
		componentInChildren.playerAvatar = player;
		if (player.isLocal)
		{
			Object.Destroy(componentInChildren.gameObject);
		}
		component.transform.localPosition = Vector3.zero;
		this.listObjects.Add(gameObject);
		this.menuPlayerListedList.Add(component);
		component.listSpot = Mathf.Max(this.listObjects.Count - 1, 0);
		foreach (string text in this.joiningPlayers)
		{
			if (player.playerName == text)
			{
				this.joiningPlayers.Remove(text);
				this.joiningPlayersEndTimer = 1f;
				break;
			}
		}
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00097BFC File Offset: 0x00095DFC
	private void PlayerRemove(PlayerAvatar player)
	{
		this.lobbyPlayers.Remove(player);
		foreach (GameObject gameObject in this.listObjects)
		{
			if (gameObject.GetComponent<MenuPlayerListed>().playerAvatar == player)
			{
				gameObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				this.listObjects.Remove(gameObject);
				this.menuPlayerListedList.Remove(gameObject.GetComponent<MenuPlayerListed>());
				break;
			}
		}
		for (int i = 0; i < this.listObjects.Count; i++)
		{
			this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x00097CC4 File Offset: 0x00095EC4
	private void SetPingText(TextMeshProUGUI text, int ping)
	{
		if (ping < 50)
		{
			text.color = new Color(0.2f, 0.8f, 0.2f);
		}
		else if (ping < 100)
		{
			text.color = new Color(0.8f, 0.8f, 0.2f);
		}
		else if (ping < 200)
		{
			text.color = new Color(0.8f, 0.4f, 0.2f);
		}
		else
		{
			text.color = new Color(0.8f, 0.2f, 0.2f);
		}
		text.text = ping.ToString() + " ms";
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00097D68 File Offset: 0x00095F68
	public void JoiningPlayer(string playerName)
	{
		this.joiningPlayers.Add(playerName);
		this.joiningPlayersTimer = 10f;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00097D81 File Offset: 0x00095F81
	public void ChangeColorButton()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Color);
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x00097D90 File Offset: 0x00095F90
	public void UpdateChatPrompt()
	{
		this.chatPromptText.text = InputManager.instance.InputDisplayReplaceTags("Press [chat] to chat");
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x00097DAC File Offset: 0x00095FAC
	public void ButtonLeave()
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.OutroStartRPC();
		}
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x00097E0C File Offset: 0x0009600C
	public void ButtonSettings()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Settings);
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x00097E1C File Offset: 0x0009601C
	public void ButtonStart()
	{
		if (this.joiningPlayer)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, -1f, -1f, false);
			return;
		}
		PhotonNetwork.CurrentRoom.IsOpen = false;
		SteamManager.instance.LockLobby();
		DataDirector.instance.RunsPlayedAdd();
		if (RunManager.instance.loadLevel == 0)
		{
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
			return;
		}
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00097E8F File Offset: 0x0009608F
	public void ButtonInvite()
	{
		SteamManager.instance.OpenSteamOverlayToLobby();
	}

	// Token: 0x04001C20 RID: 7200
	public static MenuPageLobby instance;

	// Token: 0x04001C21 RID: 7201
	internal MenuPage menuPage;

	// Token: 0x04001C22 RID: 7202
	private float listCheckTimer;

	// Token: 0x04001C23 RID: 7203
	internal List<PlayerAvatar> lobbyPlayers = new List<PlayerAvatar>();

	// Token: 0x04001C24 RID: 7204
	internal List<GameObject> listObjects = new List<GameObject>();

	// Token: 0x04001C25 RID: 7205
	internal List<MenuPlayerListed> menuPlayerListedList = new List<MenuPlayerListed>();

	// Token: 0x04001C26 RID: 7206
	public GameObject menuPlayerListedPrefab;

	// Token: 0x04001C27 RID: 7207
	public RectTransform playerListTransform;

	// Token: 0x04001C28 RID: 7208
	public TextMeshProUGUI roomNameText;

	// Token: 0x04001C29 RID: 7209
	public TextMeshProUGUI chatPromptText;

	// Token: 0x04001C2A RID: 7210
	public MenuButton startButton;

	// Token: 0x04001C2B RID: 7211
	public MenuButton inviteButton;

	// Token: 0x04001C2C RID: 7212
	public CanvasGroup joiningPlayersCanvasGroup;

	// Token: 0x04001C2D RID: 7213
	private List<string> joiningPlayers = new List<string>();

	// Token: 0x04001C2E RID: 7214
	private float joiningPlayersTimer;

	// Token: 0x04001C2F RID: 7215
	private float joiningPlayersEndTimer;

	// Token: 0x04001C30 RID: 7216
	private bool joiningPlayer;
}
