using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class MenuSpectateList : SemiUI
{
	// Token: 0x06001097 RID: 4247 RVA: 0x00095DD0 File Offset: 0x00093FD0
	protected override void Update()
	{
		base.Update();
		if (!SemiFunc.IsMultiplayer())
		{
			base.Hide();
			return;
		}
		if (!SpectateCamera.instance)
		{
			base.Hide();
			return;
		}
		base.SemiUIScoot(new Vector2(0f, (float)this.listObjects.Count * 22f));
		this.listCheckTimer -= Time.deltaTime;
		if (this.listCheckTimer <= 0f)
		{
			this.listCheckTimer = 1f;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in list)
			{
				if (playerAvatar.isDisabled)
				{
					if (!this.spectatingPlayers.Contains(playerAvatar))
					{
						this.PlayerAdd(playerAvatar);
						flag = true;
					}
				}
				else if (this.spectatingPlayers.Contains(playerAvatar))
				{
					this.PlayerRemove(playerAvatar);
					flag = true;
				}
			}
			foreach (PlayerAvatar playerAvatar2 in this.spectatingPlayers.ToList<PlayerAvatar>())
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
		}
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x00095F9C File Offset: 0x0009419C
	private void PlayerAdd(PlayerAvatar player)
	{
		this.spectatingPlayers.Add(player);
		GameObject gameObject = Object.Instantiate<GameObject>(this.menuPlayerListedPrefab, base.transform);
		MenuPlayerListed component = gameObject.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		this.listObjects.Add(gameObject);
		component.listSpot = Mathf.Max(this.listObjects.Count - 1, 0);
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x00096004 File Offset: 0x00094204
	private void PlayerRemove(PlayerAvatar player)
	{
		this.spectatingPlayers.Remove(player);
		foreach (GameObject gameObject in this.listObjects)
		{
			if (gameObject.GetComponent<MenuPlayerListed>().playerAvatar == player)
			{
				gameObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				this.listObjects.Remove(gameObject);
				break;
			}
		}
		for (int i = 0; i < this.listObjects.Count; i++)
		{
			this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}

	// Token: 0x04001BC7 RID: 7111
	public GameObject menuPlayerListedPrefab;

	// Token: 0x04001BC8 RID: 7112
	internal List<PlayerAvatar> spectatingPlayers = new List<PlayerAvatar>();

	// Token: 0x04001BC9 RID: 7113
	internal List<GameObject> listObjects = new List<GameObject>();

	// Token: 0x04001BCA RID: 7114
	private float listCheckTimer;
}
