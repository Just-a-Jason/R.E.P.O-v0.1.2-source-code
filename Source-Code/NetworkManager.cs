using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060008FA RID: 2298 RVA: 0x00055EDC File Offset: 0x000540DC
	private void Start()
	{
		NetworkManager.instance = this;
		if (PhotonNetwork.IsMasterClient)
		{
			this.lastSyncTime = 0f;
		}
		if (GameManager.instance.gameMode == 1)
		{
			PhotonNetwork.Instantiate(this.playerAvatarPrefab.name, Vector3.zero, Quaternion.identity, 0, null);
			PhotonNetwork.SerializationRate = 25;
			PhotonNetwork.SendRate = 25;
			bool flag = true;
			PhotonVoiceView[] array = Object.FindObjectsByType<PhotonVoiceView>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				PhotonNetwork.Instantiate("Voice", Vector3.zero, Quaternion.identity, 0, null);
			}
			base.photonView.RPC("PlayerSpawnedRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00055F9B File Offset: 0x0005419B
	[PunRPC]
	public void PlayerSpawnedRPC()
	{
		this.instantiatedPlayerAvatars++;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00055FAB File Offset: 0x000541AB
	[PunRPC]
	public void AllPlayerSpawnedRPC()
	{
		LevelGenerator.Instance.AllPlayersReady = true;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00055FB8 File Offset: 0x000541B8
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.lastSyncTime);
			stream.SendNext(this.instantiatedPlayerAvatars);
			return;
		}
		this.gameTime = (float)stream.ReceiveNext();
		this.instantiatedPlayerAvatars = (int)stream.ReceiveNext();
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00056014 File Offset: 0x00054214
	private void Update()
	{
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				if (!this.LoadingDone && this.instantiatedPlayerAvatars == PhotonNetwork.CurrentRoom.PlayerCount)
				{
					base.photonView.RPC("AllPlayerSpawnedRPC", RpcTarget.AllBuffered, Array.Empty<object>());
					this.LoadingDone = true;
				}
				this.gameTime += Time.deltaTime;
				if (Time.time - this.lastSyncTime > this.syncInterval)
				{
					this.lastSyncTime = this.gameTime;
					return;
				}
			}
			else
			{
				this.gameTime += Time.deltaTime;
			}
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000560B4 File Offset: 0x000542B4
	public void LeavePhotonRoom()
	{
		Debug.Log("Leave Photon");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		this.leavePhotonRoom = false;
		if (RunManager.instance.levelCurrent == RunManager.instance.levelTutorial)
		{
			TutorialDirector.instance.EndTutorial();
		}
		base.StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00056122 File Offset: 0x00054322
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Player left room: " + otherPlayer.NickName);
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0005613C File Offset: 0x0005433C
	public override void OnMasterClientSwitched(Player _newMasterClient)
	{
		Debug.Log("Master client left...");
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.OutroStartRPC();
		}
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: Host disconnected", "Ok Dang");
		this.leavePhotonRoom = true;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x000561C0 File Offset: 0x000543C0
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log(string.Format("Disconnected from server for reason {0}", cause));
		if (cause != DisconnectCause.DisconnectByClientLogic && cause != DisconnectCause.DisconnectByServerLogic)
		{
			MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + cause.ToString(), "Ok Dang");
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				playerAvatar.OutroStartRPC();
			}
			this.leavePhotonRoom = true;
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x0005626C File Offset: 0x0005446C
	public void DestroyAll()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		Debug.Log("Destroyed all network objects.");
		PhotonNetwork.DestroyAll();
	}

	// Token: 0x0400105E RID: 4190
	public static NetworkManager instance;

	// Token: 0x0400105F RID: 4191
	public float gameTime;

	// Token: 0x04001060 RID: 4192
	private float syncInterval = 0.5f;

	// Token: 0x04001061 RID: 4193
	private float lastSyncTime;

	// Token: 0x04001062 RID: 4194
	public GameObject playerAvatarPrefab;

	// Token: 0x04001063 RID: 4195
	private int instantiatedPlayerAvatars;

	// Token: 0x04001064 RID: 4196
	private bool LoadingDone;

	// Token: 0x04001065 RID: 4197
	internal bool leavePhotonRoom;
}
