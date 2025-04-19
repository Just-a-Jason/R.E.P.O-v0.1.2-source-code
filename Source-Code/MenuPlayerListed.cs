using System;
using TMPro;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class MenuPlayerListed : MonoBehaviour
{
	// Token: 0x0600105F RID: 4191 RVA: 0x00093A80 File Offset: 0x00091C80
	private void Start()
	{
		this.parentTransform = base.transform.parent.GetComponent<RectTransform>();
		this.playerHead.focusPoint.SetParent(this.parentTransform);
		this.playerHead.myFocusPoint.SetParent(this.parentTransform);
		this.midScreenFocus = new Vector3((float)(MenuManager.instance.screenUIWidth / 2), (float)(MenuManager.instance.screenUIHeight / 2), 0f) - this.parentTransform.localPosition - this.parentTransform.parent.GetComponent<RectTransform>().localPosition;
		if (this.forceCrown)
		{
			this.leftCrown.SetActive(true);
			this.rightCrown.SetActive(true);
			this.ForcePlayer(Arena.instance.winnerPlayer);
			TextMeshProUGUI componentInChildren = base.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren && this.playerAvatar)
			{
				componentInChildren.text = this.playerAvatar.playerName;
			}
		}
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00093B80 File Offset: 0x00091D80
	public void ForcePlayer(PlayerAvatar _playerAvatar)
	{
		this.playerHead.SetPlayer(_playerAvatar);
		this.playerAvatar = _playerAvatar;
		this.localFetch = false;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00093B9C File Offset: 0x00091D9C
	private void Update()
	{
		if (SemiFunc.FPSImpulse5() && !this.crownSetterWasHere && PlayerCrownSet.instance && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (this.playerAvatar && PlayerCrownSet.instance.crownOwnerSteamID == this.playerAvatar.steamID)
			{
				this.leftCrown.SetActive(true);
				this.rightCrown.SetActive(true);
			}
			this.crownSetterWasHere = true;
		}
		if (!this.localFetch)
		{
			if (this.playerAvatar)
			{
				this.isLocal = this.playerAvatar.isLocal;
			}
			this.localFetch = true;
		}
		if (!this.forceCrown && this.playerHead.myFocusPoint.localPosition != this.midScreenFocus)
		{
			this.playerHead.myFocusPoint.localPosition = this.midScreenFocus;
		}
		if (this.playerAvatar)
		{
			if (!this.fetchCrown)
			{
				if (SessionManager.instance.CrownedPlayerGet() == this.playerAvatar)
				{
					this.leftCrown.SetActive(true);
					this.rightCrown.SetActive(true);
				}
				this.fetchCrown = true;
			}
			if (this.isSpectate && this.playerName.text != this.playerAvatar.playerName)
			{
				this.playerName.text = this.playerAvatar.playerName;
			}
			if (this.playerAvatar.voiceChatFetched && this.playerAvatar.voiceChat.isTalking)
			{
				Color b = new Color(0.6f, 0.6f, 0.4f);
				this.playerName.color = Color.Lerp(this.playerName.color, b, Time.deltaTime * 10f);
			}
			else
			{
				Color b2 = new Color(0.2f, 0.2f, 0.2f);
				this.playerName.color = Color.Lerp(this.playerName.color, b2, Time.deltaTime * 10f);
			}
		}
		if (!this.forceCrown)
		{
			if (RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu)
			{
				base.transform.localPosition = new Vector3(-23f, (float)(-(float)this.listSpot * 22), 0f);
				return;
			}
			base.transform.localPosition = new Vector3(0f, (float)(-(float)this.listSpot * 32), 0f);
		}
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00093E12 File Offset: 0x00092012
	public void MenuPlayerListedOutro()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001B52 RID: 6994
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001B53 RID: 6995
	internal int listSpot;

	// Token: 0x04001B54 RID: 6996
	public TextMeshProUGUI playerName;

	// Token: 0x04001B55 RID: 6997
	public MenuPlayerHead playerHead;

	// Token: 0x04001B56 RID: 6998
	private RectTransform parentTransform;

	// Token: 0x04001B57 RID: 6999
	private Vector3 midScreenFocus;

	// Token: 0x04001B58 RID: 7000
	public TextMeshProUGUI pingText;

	// Token: 0x04001B59 RID: 7001
	private bool localFetch;

	// Token: 0x04001B5A RID: 7002
	internal bool isLocal;

	// Token: 0x04001B5B RID: 7003
	public bool isSpectate = true;

	// Token: 0x04001B5C RID: 7004
	public GameObject leftCrown;

	// Token: 0x04001B5D RID: 7005
	public GameObject rightCrown;

	// Token: 0x04001B5E RID: 7006
	private bool fetchCrown;

	// Token: 0x04001B5F RID: 7007
	public bool forceCrown;

	// Token: 0x04001B60 RID: 7008
	private bool crownSetterWasHere;
}
