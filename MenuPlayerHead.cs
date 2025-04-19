using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E7 RID: 487
public class MenuPlayerHead : MonoBehaviour
{
	// Token: 0x06001055 RID: 4181 RVA: 0x000932C8 File Offset: 0x000914C8
	private void Start()
	{
		this.playerListed = base.GetComponentInParent<MenuPlayerListed>();
		this.allRawImagesInChildren.AddRange(base.GetComponentsInChildren<RawImage>());
		this.playerListedTransform = this.playerListed.GetComponent<RectTransform>();
		MenuManager.instance.PlayerHeadAdd(this);
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startedTalkingAtTime = Time.time;
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x00093328 File Offset: 0x00091528
	public void SetColor(Color color)
	{
		foreach (RawImage rawImage in this.allRawImagesInChildren)
		{
			rawImage.color = color;
		}
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x0009337C File Offset: 0x0009157C
	private void HeadRight()
	{
		this.headRight.gameObject.SetActive(true);
		this.headLeft.gameObject.SetActive(false);
		this.eyesTransform = this.headRight.Find("Eyes").GetComponent<RectTransform>();
		this.left = false;
		this.right = true;
		this.headTransform = this.headRight.GetComponent<RectTransform>();
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000933E8 File Offset: 0x000915E8
	private void HeadLeft()
	{
		this.headRight.gameObject.SetActive(false);
		this.headLeft.gameObject.SetActive(true);
		this.eyesTransform = this.headLeft.Find("Eyes").GetComponent<RectTransform>();
		this.left = true;
		this.right = false;
		this.headTransform = this.headLeft.GetComponent<RectTransform>();
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x00093454 File Offset: 0x00091654
	private void Update()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (this.playerAvatar)
			{
				this.isTalkingPrev = this.isTalking;
				if (this.playerAvatar.voiceChatFetched)
				{
					this.isTalking = this.playerAvatar.voiceChat.isTalking;
				}
			}
			else
			{
				this.playerAvatar = this.playerListed.playerAvatar;
			}
		}
		if (MenuManager.instance.currentMenuPageIndex == MenuPageIndex.Lobby)
		{
			this.myFocusPoint.localPosition = MenuCursor.instance.transform.localPosition - this.rectTransform.parent.parent.localPosition;
			this.myFocusPoint.localPosition = new Vector3(this.myFocusPoint.localPosition.x + 18f, this.myFocusPoint.localPosition.y + 15f, 0f);
		}
		if (!this.isWinnerHead && this.headTransform)
		{
			this.focusPoint.localPosition = this.playerListedTransform.localPosition + this.rectTransform.localPosition + this.headTransform.localPosition * this.rectTransform.localScale.x;
			float length = 12.5f;
			if (this.left)
			{
				length = -12.5f;
			}
			this.focusPoint.localPosition += new Vector3(MenuPlayerHead.LengthDirX(length, this.headTransform.localEulerAngles.z), MenuPlayerHead.LengthDirY(length, this.headTransform.localEulerAngles.z), 0f);
			length = 6f;
			this.focusPoint.localPosition += new Vector3(MenuPlayerHead.LengthDirX(length, this.headTransform.localEulerAngles.z + 90f), MenuPlayerHead.LengthDirY(length, this.headTransform.localEulerAngles.z + 90f), 0f);
		}
		if (this.isTalking != this.isTalkingPrev)
		{
			if (this.isTalking)
			{
				this.startedTalkingAtTime = Time.time;
			}
			this.isTalkingPrev = this.isTalking;
		}
		if (!this.isWinnerHead)
		{
			this.listSpot = this.playerListed.listSpot;
			if (this.listSpot != this.listSpotPrev)
			{
				if (this.listSpot % 2 == 0)
				{
					this.HeadRight();
				}
				else
				{
					this.HeadLeft();
				}
				this.listSpotPrev = this.listSpot;
			}
		}
		if (!this.isWinnerHead)
		{
			MenuPlayerHead menuPlayerHead = null;
			float num = 0f;
			foreach (MenuPlayerHead menuPlayerHead2 in MenuManager.instance.playerHeads)
			{
				if (!(menuPlayerHead2 == this) && menuPlayerHead2.isTalking)
				{
					float num2 = menuPlayerHead2.startedTalkingAtTime;
					if (num2 > num)
					{
						num = num2;
						menuPlayerHead = menuPlayerHead2;
					}
				}
			}
			if (menuPlayerHead)
			{
				float d = 10f;
				Vector3 vector = menuPlayerHead.focusPoint.localPosition - this.focusPoint.localPosition;
				vector.z = 0f;
				Vector3 b = new Vector3(50f, 25f, 0f) + vector.normalized * d;
				if (this.left)
				{
					b = new Vector3(-50f, 25f, 0f) + vector.normalized * d;
				}
				this.eyesTransform.localPosition = Vector3.Lerp(this.eyesTransform.localPosition, b, Time.deltaTime * 10f);
			}
			else
			{
				float d2 = 10f;
				Vector3 vector2 = this.myFocusPoint.localPosition - this.focusPoint.localPosition;
				vector2.z = 0f;
				Vector3 b2 = new Vector3(50f, 25f, 0f) + vector2.normalized * d2;
				if (this.left)
				{
					b2 = new Vector3(-50f, 25f, 0f) + vector2.normalized * d2;
				}
				this.eyesTransform.localPosition = Vector3.Lerp(this.eyesTransform.localPosition, b2, Time.deltaTime * 10f);
			}
		}
		if (this.playerAvatar && this.playerAvatar.voiceChatFetched)
		{
			if (this.left)
			{
				float clipLoudness = this.playerAvatar.voiceChat.clipLoudness;
				this.headLeft.localEulerAngles = new Vector3(0f, 0f, -clipLoudness * 200f);
			}
			if (this.right)
			{
				float clipLoudness2 = this.playerAvatar.voiceChat.clipLoudness;
				this.headRight.localEulerAngles = new Vector3(0f, 0f, clipLoudness2 * 200f);
			}
		}
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x00093960 File Offset: 0x00091B60
	public void SetPlayer(PlayerAvatar player)
	{
		this.playerAvatar = player;
		if (this.allRawImagesInChildren.Count == 0)
		{
			this.allRawImagesInChildren.AddRange(base.GetComponentsInChildren<RawImage>());
		}
		foreach (RawImage rawImage in this.allRawImagesInChildren)
		{
			if (rawImage && this.playerAvatar && this.playerAvatar.playerAvatarVisuals)
			{
				rawImage.color = this.playerAvatar.playerAvatarVisuals.color;
			}
		}
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00093A10 File Offset: 0x00091C10
	private void OnDestroy()
	{
		MenuManager.instance.PlayerHeadRemove(this);
		Object.Destroy(this.focusPoint.gameObject);
		Object.Destroy(this.myFocusPoint.gameObject);
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00093A3D File Offset: 0x00091C3D
	public static float LengthDirX(float length, float direction)
	{
		return length * Mathf.Cos(direction * 0.017453292f);
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00093A4D File Offset: 0x00091C4D
	public static float LengthDirY(float length, float direction)
	{
		return length * Mathf.Sin(direction * 0.017453292f);
	}

	// Token: 0x04001B3D RID: 6973
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001B3E RID: 6974
	public Transform headRight;

	// Token: 0x04001B3F RID: 6975
	public Transform headLeft;

	// Token: 0x04001B40 RID: 6976
	private RectTransform eyesTransform;

	// Token: 0x04001B41 RID: 6977
	private MenuPlayerListed playerListed;

	// Token: 0x04001B42 RID: 6978
	private int listSpotPrev = -1;

	// Token: 0x04001B43 RID: 6979
	private int listSpot;

	// Token: 0x04001B44 RID: 6980
	private bool left;

	// Token: 0x04001B45 RID: 6981
	private bool right = true;

	// Token: 0x04001B46 RID: 6982
	private List<RawImage> allRawImagesInChildren = new List<RawImage>();

	// Token: 0x04001B47 RID: 6983
	private Vector3 eyesStartPosOriginal;

	// Token: 0x04001B48 RID: 6984
	private Vector3 eyesStartPos;

	// Token: 0x04001B49 RID: 6985
	private bool isTalkingPrev;

	// Token: 0x04001B4A RID: 6986
	internal bool isTalking;

	// Token: 0x04001B4B RID: 6987
	public RectTransform focusPoint;

	// Token: 0x04001B4C RID: 6988
	public RectTransform myFocusPoint;

	// Token: 0x04001B4D RID: 6989
	private RectTransform playerListedTransform;

	// Token: 0x04001B4E RID: 6990
	private RectTransform rectTransform;

	// Token: 0x04001B4F RID: 6991
	internal RectTransform headTransform;

	// Token: 0x04001B50 RID: 6992
	internal float startedTalkingAtTime;

	// Token: 0x04001B51 RID: 6993
	public bool isWinnerHead;
}
