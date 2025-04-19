using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class LobbyMenuOpen : MonoBehaviour
{
	// Token: 0x060010B5 RID: 4277 RVA: 0x0009695F File Offset: 0x00094B5F
	private void Awake()
	{
		LobbyMenuOpen.instance = this;
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x00096968 File Offset: 0x00094B68
	private void Update()
	{
		if (this.opened)
		{
			return;
		}
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			GameDirector.instance.CameraShake.Shake(0.25f, 0.05f);
			GameDirector.instance.CameraImpact.Shake(0.25f, 0.05f);
			MenuManager.instance.PageOpen(MenuPageIndex.Lobby, false);
			if (SemiFunc.IsMasterClient())
			{
				PhotonNetwork.CurrentRoom.IsOpen = true;
				SteamManager.instance.UnlockLobby();
			}
			this.opened = true;
		}
	}

	// Token: 0x04001BE6 RID: 7142
	public static LobbyMenuOpen instance;

	// Token: 0x04001BE7 RID: 7143
	public float timer = 2f;

	// Token: 0x04001BE8 RID: 7144
	private bool opened;
}
