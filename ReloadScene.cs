using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200020D RID: 525
public class ReloadScene : MonoBehaviour, IPunObservable
{
	// Token: 0x0600112F RID: 4399 RVA: 0x000994FF File Offset: 0x000976FF
	[PunRPC]
	private void PlayerReady()
	{
		this.PlayersReady++;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0009950F File Offset: 0x0009770F
	private void Awake()
	{
		this.photonview = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0009951D File Offset: 0x0009771D
	private void Start()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonview.RPC("PlayerReady", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0009953C File Offset: 0x0009773C
	private void Update()
	{
		if (this.minTime > 0f)
		{
			this.minTime -= Time.deltaTime;
			return;
		}
		if (!this.Restarting)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SceneManager.LoadSceneAsync("Main");
				this.Restarting = true;
				return;
			}
			if (PhotonNetwork.IsMasterClient && this.PlayersReady == PhotonNetwork.CurrentRoom.PlayerCount && (PhotonNetwork.LevelLoadingProgress == 0f || PhotonNetwork.LevelLoadingProgress == 1f))
			{
				PhotonNetwork.AutomaticallySyncScene = true;
				PhotonNetwork.LoadLevel("Main");
				this.Restarting = true;
			}
		}
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x000995D3 File Offset: 0x000977D3
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.PlayersReady);
			return;
		}
		this.PlayersReady = (int)stream.ReceiveNext();
	}

	// Token: 0x04001C76 RID: 7286
	private PhotonView photonview;

	// Token: 0x04001C77 RID: 7287
	public int PlayersReady;

	// Token: 0x04001C78 RID: 7288
	private bool Restarting;

	// Token: 0x04001C79 RID: 7289
	private float minTime = 0.1f;
}
