using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class SteamManager : MonoBehaviour
{
	// Token: 0x060011AF RID: 4527 RVA: 0x0009D10C File Offset: 0x0009B30C
	private void Awake()
	{
		if (!SteamManager.instance)
		{
			SteamManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			try
			{
				SteamClient.Init(3241660U, true);
			}
			catch (Exception ex)
			{
				Debug.LogError("Steamworks failed to initialize. Error: " + ex.Message);
			}
			Debug.Log("STEAM ID: " + SteamClient.SteamId.ToString());
			if (Debug.isDebugBuild)
			{
				foreach (SteamManager.Developer developer in this.developerList)
				{
					if (SteamClient.SteamId.ToString() == developer.steamID)
					{
						Debug.Log("DEVELOPER MODE: " + developer.name.ToUpper());
						this.developerMode = true;
					}
				}
			}
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0009D220 File Offset: 0x0009B420
	private void OnEnable()
	{
		SteamMatchmaking.OnLobbyCreated += this.OnLobbyCreated;
		SteamMatchmaking.OnLobbyEntered += this.OnLobbyEntered;
		SteamFriends.OnGameLobbyJoinRequested += this.OnGameLobbyJoinRequested;
		SteamMatchmaking.OnLobbyMemberJoined += this.OnLobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave += this.OnLobbyMemberLeft;
		SteamMatchmaking.OnLobbyMemberDataChanged += this.OnLobbyMemberDataChanged;
		SteamFriends.OnGameOverlayActivated += this.OnGameOverlayActivated;
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x0009D2A4 File Offset: 0x0009B4A4
	private void Start()
	{
		this.GetSteamAuthTicket(out this.steamAuthTicket);
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length >= 2)
		{
			int i = 0;
			while (i < commandLineArgs.Length - 1)
			{
				if (commandLineArgs[i].ToLower() == "+connect_lobby")
				{
					ulong num;
					if (ulong.TryParse(commandLineArgs[i + 1], out num) && num > 0UL)
					{
						Debug.Log("Auto-Connecting to lobby: " + num.ToString());
						this.OnGameLobbyJoinRequested(new Lobby(num), SteamClient.SteamId);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x0009D32E File Offset: 0x0009B52E
	private void OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
	{
		Debug.Log("Steam: Lobby member joined: " + _friend.Name);
		MenuPageLobby.instance.JoiningPlayer(_friend.Name);
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x0009D357 File Offset: 0x0009B557
	private void OnLobbyMemberLeft(Lobby _lobby, Friend _friend)
	{
		Debug.Log("Steam: Lobby member left: " + _friend.Name);
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0009D370 File Offset: 0x0009B570
	private void OnLobbyMemberDataChanged(Lobby _lobby, Friend _friend)
	{
		Debug.Log(" ");
		Debug.Log("Steam: Lobby member data changed for: " + _friend.Name);
		Debug.Log("I am " + SteamClient.Name);
		Debug.Log("Current Owner: " + _lobby.Owner.Name);
		if (PhotonNetwork.IsMasterClient && RunManager.instance.masterSwitched && SteamClient.SteamId == _lobby.Owner.Id)
		{
			Debug.Log("I am the new owner and i am locking the lobby.");
			this.LockLobby();
		}
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0009D40F File Offset: 0x0009B60F
	private void OnDestroy()
	{
		if (SteamManager.instance == this)
		{
			this.CancelSteamAuthTicket();
			SteamClient.Shutdown();
		}
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x0009D42C File Offset: 0x0009B62C
	private void OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamID)
	{
		SteamManager.<OnGameLobbyJoinRequested>d__16 <OnGameLobbyJoinRequested>d__;
		<OnGameLobbyJoinRequested>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGameLobbyJoinRequested>d__.<>4__this = this;
		<OnGameLobbyJoinRequested>d__._lobby = _lobby;
		<OnGameLobbyJoinRequested>d__.<>1__state = -1;
		<OnGameLobbyJoinRequested>d__.<>t__builder.Start<SteamManager.<OnGameLobbyJoinRequested>d__16>(ref <OnGameLobbyJoinRequested>d__);
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0009D46C File Offset: 0x0009B66C
	private void OnLobbyEntered(Lobby _lobby)
	{
		this.currentLobby.Leave();
		this.currentLobby = _lobby;
		Debug.Log("Steam: Lobby entered with ID: " + _lobby.Id.ToString());
		Debug.Log("Steam: Region: " + _lobby.GetData("Region"));
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0009D4CC File Offset: 0x0009B6CC
	private void OnLobbyCreated(Result _result, Lobby _lobby)
	{
		if (_result == Result.OK)
		{
			Debug.Log("Steam: Lobby created with ID: " + _lobby.Id.ToString());
			return;
		}
		Debug.LogError("Steam: Failed to create lobby. Error: " + _result.ToString());
		NetworkManager.instance.LeavePhotonRoom();
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x0009D528 File Offset: 0x0009B728
	public void HostLobby()
	{
		SteamManager.<HostLobby>d__19 <HostLobby>d__;
		<HostLobby>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<HostLobby>d__.<>1__state = -1;
		<HostLobby>d__.<>t__builder.Start<SteamManager.<HostLobby>d__19>(ref <HostLobby>d__);
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0009D558 File Offset: 0x0009B758
	public void LeaveLobby()
	{
		if (this.currentLobby.IsOwnedBy(SteamClient.SteamId))
		{
			Debug.Log("Steam: Leaving lobby... and ruining it for others.");
			this.currentLobby.SetData("BuildName", "");
		}
		else
		{
			Debug.Log("Steam: Leaving lobby...");
		}
		this.CancelSteamAuthTicket();
		this.currentLobby.Leave();
		this.currentLobby = this.noLobby;
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0009D5C0 File Offset: 0x0009B7C0
	public void UnlockLobby()
	{
		Debug.Log("Steam: Unlocking lobby...");
		this.currentLobby.SetPrivate();
		this.currentLobby.SetFriendsOnly();
		this.currentLobby.SetJoinable(true);
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0009D5F1 File Offset: 0x0009B7F1
	public void LockLobby()
	{
		Debug.Log("Steam: Locking lobby...");
		this.currentLobby.SetPrivate();
		this.currentLobby.SetFriendsOnly();
		this.currentLobby.SetJoinable(false);
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x0009D624 File Offset: 0x0009B824
	public void SetLobbyData()
	{
		Debug.Log("Steam: Setting lobby data...");
		this.currentLobby.SetData("Region", PhotonNetwork.CloudRegion);
		this.currentLobby.SetData("BuildName", BuildManager.instance.version.title);
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0009D674 File Offset: 0x0009B874
	public void SendSteamAuthTicket()
	{
		Debug.Log("Sending Steam Auth Ticket...");
		string value = this.GetSteamAuthTicket(out this.steamAuthTicket);
		PhotonNetwork.AuthValues = new AuthenticationValues();
		PhotonNetwork.AuthValues.UserId = SteamClient.SteamId.ToString();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
		PhotonNetwork.AuthValues.AddAuthParameter("ticket", value);
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0009D6DC File Offset: 0x0009B8DC
	private string GetSteamAuthTicket(out AuthTicket ticket)
	{
		Debug.Log("Getting Steam Auth Ticket...");
		ticket = SteamUser.GetAuthSessionTicket();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ticket.Data.Length; i++)
		{
			stringBuilder.AppendFormat("{0:x2}", ticket.Data[i]);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x0009D734 File Offset: 0x0009B934
	public void CancelSteamAuthTicket()
	{
		Debug.Log("Cancelling Steam Auth Ticket...");
		if (this.steamAuthTicket == null)
		{
			return;
		}
		this.steamAuthTicket.Cancel();
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x0009D754 File Offset: 0x0009B954
	public void OpenSteamOverlayToLobby()
	{
		SteamFriends.OpenOverlay("friends");
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0009D760 File Offset: 0x0009B960
	private void OnGameOverlayActivated(bool obj)
	{
		InputManager.instance.ResetInput();
	}

	// Token: 0x04001DC6 RID: 7622
	public static SteamManager instance;

	// Token: 0x04001DC7 RID: 7623
	internal Lobby currentLobby;

	// Token: 0x04001DC8 RID: 7624
	internal Lobby noLobby;

	// Token: 0x04001DC9 RID: 7625
	internal bool joinLobby;

	// Token: 0x04001DCA RID: 7626
	public GameObject networkConnectPrefab;

	// Token: 0x04001DCB RID: 7627
	internal AuthTicket steamAuthTicket;

	// Token: 0x04001DCC RID: 7628
	[Space]
	public List<SteamManager.Developer> developerList;

	// Token: 0x04001DCD RID: 7629
	internal bool developerMode;

	// Token: 0x020003A9 RID: 937
	[Serializable]
	public class Developer
	{
		// Token: 0x0400287B RID: 10363
		public string name;

		// Token: 0x0400287C RID: 10364
		public string steamID;
	}
}
