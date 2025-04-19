using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001F7 RID: 503
public class NetworkConnect : MonoBehaviourPunCallbacks
{
	// Token: 0x060010A9 RID: 4265 RVA: 0x00096618 File Offset: 0x00094818
	private void Awake()
	{
		NetworkConnect.instance = this;
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00096620 File Offset: 0x00094820
	private void Start()
	{
		PhotonNetwork.NickName = SteamClient.Name;
		PhotonNetwork.AutomaticallySyncScene = false;
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
		Object.Instantiate<GameObject>(this.punVoiceClient, Vector3.zero, Quaternion.identity);
		PhotonNetwork.Disconnect();
		base.StartCoroutine(this.CreateLobby());
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x00096679 File Offset: 0x00094879
	private IEnumerator CreateLobby()
	{
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		if (!GameManager.instance.localTest)
		{
			if (SteamManager.instance.currentLobby.Id.IsValid)
			{
				this.RoomName = SteamManager.instance.currentLobby.Id.ToString();
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = SteamManager.instance.currentLobby.GetData("Region");
				string data = SteamManager.instance.currentLobby.GetData("BuildName");
				if (data != BuildManager.instance.version.title)
				{
					if (data != "")
					{
						Debug.Log("Build name mismatch. Leaving lobby. Build name is ''" + data + "''");
						string bodyText = "Game lobby is using version\n<color=#FDFF00><b>" + data + "</b>";
						MenuManager.instance.PagePopUpScheduled("Wrong Game Version", Color.red, bodyText, "Ok Dang");
					}
					else
					{
						Debug.Log("Lobby closed. Leaving lobby.");
						MenuManager.instance.PagePopUpScheduled("Lobby Closed", Color.red, "The lobby has closed.", "Ok Dang");
					}
					PhotonNetwork.Disconnect();
					SteamManager.instance.LeaveLobby();
					GameManager.instance.SetGameMode(0);
					RunManager.instance.levelCurrent = RunManager.instance.levelMainMenu;
					SceneManager.LoadSceneAsync("Reload");
					yield break;
				}
				Debug.Log("Already in lobby on Network Connect. Connecting to region: " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
			}
			else
			{
				Debug.Log("Created lobby on Network Connect.");
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
				SteamManager.instance.HostLobby();
				while (!SteamManager.instance.currentLobby.Id.IsValid)
				{
					yield return null;
				}
				this.RoomName = SteamManager.instance.currentLobby.Id.ToString();
			}
			SteamManager.instance.SendSteamAuthTicket();
		}
		else
		{
			Debug.Log("Local test mode.");
			RunManager.instance.ResetProgress();
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
			this.RoomName = SteamClient.Name;
		}
		PhotonNetwork.ConnectUsingSettings();
		yield break;
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x00096688 File Offset: 0x00094888
	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master Server");
		if (!GameManager.instance.localTest && SteamManager.instance.currentLobby.Id.IsValid && SteamManager.instance.currentLobby.IsOwnedBy(SteamClient.SteamId))
		{
			Debug.Log("I am the owner.");
			SteamManager.instance.SetLobbyData();
			this.TryJoiningRoom();
			return;
		}
		Debug.Log("I am not the owner.");
		this.TryJoiningRoom();
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00096705 File Offset: 0x00094905
	private void TryJoiningRoom()
	{
		Debug.Log("Trying to join room: " + this.RoomName);
		PhotonNetwork.JoinOrCreateRoom(this.RoomName, new RoomOptions
		{
			MaxPlayers = 6,
			IsVisible = false
		}, TypedLobby.Default, null);
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00096741 File Offset: 0x00094941
	public override void OnCreatedRoom()
	{
		Debug.Log("Created room successfully: " + PhotonNetwork.CurrentRoom.Name);
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0009675C File Offset: 0x0009495C
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CloudRegion);
		this.joinedRoom = true;
		PhotonNetwork.AutomaticallySyncScene = true;
		RunManager.instance.waitToChangeScene = false;
		if (GameManager.instance.localTest && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Reload");
		}
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x000967C4 File Offset: 0x000949C4
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogError("Failed to create room: " + message);
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + message, "Ok Dang");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00096830 File Offset: 0x00094A30
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.LogError("Failed to join room: " + message);
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + message, "Ok Dang");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x0009689C File Offset: 0x00094A9C
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log(string.Format("Disconnected from server for reason {0}", cause));
		if (cause != DisconnectCause.DisconnectByClientLogic && cause != DisconnectCause.DisconnectByServerLogic)
		{
			MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + cause.ToString(), "Ok Dang");
			PhotonNetwork.Disconnect();
			SteamManager.instance.LeaveLobby();
			GameManager.instance.SetGameMode(0);
			base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
		}
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x00096923 File Offset: 0x00094B23
	private void OnDestroy()
	{
		if (this.joinedRoom)
		{
			Debug.Log("Game Mode: Multiplayer");
			GameManager.instance.SetGameMode(1);
		}
		Debug.Log("NetworkConnect destroyed.");
		RunManager.instance.waitToChangeScene = false;
	}

	// Token: 0x04001BE1 RID: 7137
	public static NetworkConnect instance;

	// Token: 0x04001BE2 RID: 7138
	private bool joinedRoom;

	// Token: 0x04001BE3 RID: 7139
	private string RoomName;

	// Token: 0x04001BE4 RID: 7140
	private bool ConnectedToMasterServer;

	// Token: 0x04001BE5 RID: 7141
	public GameObject punVoiceClient;
}
