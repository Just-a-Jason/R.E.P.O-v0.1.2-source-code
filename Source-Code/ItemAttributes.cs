using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class ItemAttributes : MonoBehaviour
{
	// Token: 0x06000B23 RID: 2851 RVA: 0x000634BF File Offset: 0x000616BF
	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			bool enabled = base.enabled;
		}
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000634D0 File Offset: 0x000616D0
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.item)
		{
			this.colorPreset = this.item.colorPreset;
		}
		else
		{
			this.colorPreset = null;
		}
		if (this.item)
		{
			this.emojiIcon = this.item.emojiIcon;
			return;
		}
		this.emojiIcon = SemiFunc.emojiIcon.drone_heal;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00063538 File Offset: 0x00061738
	private void Start()
	{
		this.itemName = this.item.itemName;
		this.instanceName = null;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemType = this.item.itemType;
		this.itemAssetName = this.item.itemAssetName;
		this.itemValueMin = this.item.value.valueMin;
		this.itemValueMax = this.item.value.valueMax;
		if (SemiFunc.RunIsShop())
		{
			this.shopItem = true;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			ItemVolume componentInChildren = base.GetComponentInChildren<ItemVolume>();
			if (componentInChildren)
			{
				this.itemVolume = componentInChildren.transform;
			}
			if (this.itemVolume)
			{
				Vector3 b = this.itemVolume.position - base.transform.position;
				base.transform.position -= b;
				Rigidbody component = base.GetComponent<Rigidbody>();
				component.position -= b;
				if (SemiFunc.IsMultiplayer())
				{
					base.GetComponent<PhotonTransformView>().Teleport(component.position, base.transform.rotation);
				}
				Object.Destroy(this.itemVolume.gameObject);
			}
		}
		if (!this.shopItem)
		{
			ItemManager.instance.AddSpawnedItem(this);
		}
		base.transform.parent = LevelGenerator.Instance.ItemParent.transform;
		this.GetValue();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		if (this.itemEquippable)
		{
			this.itemEquippable.itemEmojiIcon = this.emojiIcon;
			this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		}
		base.StartCoroutine(this.GenerateIcon());
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00063721 File Offset: 0x00061921
	private IEnumerator GenerateIcon()
	{
		yield return null;
		if (this.itemEquippable && !this.icon)
		{
			SemiIconMaker componentInChildren = base.GetComponentInChildren<SemiIconMaker>();
			if (componentInChildren)
			{
				this.icon = componentInChildren.CreateIconFromRenderTexture();
			}
			else
			{
				Debug.LogWarning("No IconMaker found in " + base.gameObject.name + ", add SemiIconMaker prefab and align the camera to make a proper icon... or make a custom icon and assign it in the Item Attributes!");
			}
		}
		this.hasIcon = true;
		yield break;
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00063730 File Offset: 0x00061930
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.photonView = base.GetComponent<PhotonView>();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.ItemFetchName(this.itemAssetName, this, this.photonView.ViewID);
		}
		yield break;
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00063740 File Offset: 0x00061940
	public void GetValue()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		float num = Random.Range(this.itemValueMin, this.itemValueMax) * ShopManager.instance.itemValueMultiplier;
		if (num < 1000f)
		{
			num = 1000f;
		}
		if (num >= 1000f)
		{
			num = Mathf.Ceil(num / 1000f);
		}
		if (this.itemType == SemiFunc.itemType.item_upgrade)
		{
			num += num * ShopManager.instance.upgradeValueIncrease * (float)StatsManager.instance.GetItemsUpgradesPurchased(this.itemAssetName);
		}
		else if (this.itemType == SemiFunc.itemType.healthPack)
		{
			num += num * ShopManager.instance.healthPackValueIncrease * (float)RunManager.instance.levelsCompleted;
		}
		else if (this.itemType == SemiFunc.itemType.power_crystal)
		{
			num += num * ShopManager.instance.crystalValueIncrease * (float)RunManager.instance.levelsCompleted;
		}
		this.value = (int)num;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("GetValueRPC", RpcTarget.Others, new object[]
			{
				this.value
			});
		}
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x00063846 File Offset: 0x00061A46
	[PunRPC]
	public void GetValueRPC(int _value)
	{
		this.value = _value;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0006384F File Offset: 0x00061A4F
	public void DisableUI(bool _disable)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("DisableUIRPC", RpcTarget.All, new object[]
			{
				_disable
			});
			return;
		}
		this.DisableUIRPC(_disable);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00063880 File Offset: 0x00061A80
	[PunRPC]
	public void DisableUIRPC(bool _disable)
	{
		this.disableUI = _disable;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0006388C File Offset: 0x00061A8C
	private void ShopInTruckLogic()
	{
		if (!SemiFunc.RunIsShop())
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.inStartRoomCheckTimer > 0f)
			{
				this.inStartRoomCheckTimer -= Time.deltaTime;
				return;
			}
			bool flag = false;
			using (List<RoomVolume>.Enumerator enumerator = this.roomVolumeCheck.CurrentRooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Extraction)
					{
						flag = true;
					}
				}
			}
			if (!flag && this.inStartRoom)
			{
				ShopManager.instance.ShoppingListItemRemove(this);
				this.inStartRoom = false;
			}
			this.inStartRoomCheckTimer = 0.5f;
			if (flag && !this.inStartRoom)
			{
				ShopManager.instance.ShoppingListItemAdd(this);
				this.inStartRoom = true;
			}
		}
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00063960 File Offset: 0x00061B60
	private void Update()
	{
		if (this.showInfoTimer > 0f && this.physGrabObject.grabbedLocal)
		{
			this.itemTag = "";
			this.promptName = "";
			this.showInfoTimer = 0f;
		}
		if (this.showInfoTimer > 0f)
		{
			if (!PhysGrabber.instance.grabbed)
			{
				this.ShowingInfo();
				this.showInfoTimer -= Time.fixedDeltaTime;
			}
			else
			{
				this.showInfoTimer = 0f;
			}
		}
		this.ShopInTruckLogic();
		if (this.physGrabObject.playerGrabbing.Count > 0 && !this.disableUI && PhysGrabber.instance.grabbedPhysGrabObject == this.physGrabObject && PhysGrabber.instance.grabbed)
		{
			this.ShowingInfo();
		}
		if (this.isHeldTimer > 0f)
		{
			this.isHeldTimer -= Time.deltaTime;
		}
		if (this.isHeldTimer <= 0f && this.physGrabObject.grabbedLocal)
		{
			this.isHeldTimer = 0.2f;
		}
		if (this.isHeldTimer > 0f)
		{
			if (SemiFunc.RunIsShop() && !PhysGrabber.instance.grabbed && PhysGrabber.instance.currentlyLookingAtItemAttributes == this)
			{
				WorldSpaceUIValue.instance.Show(this.physGrabObject, this.value, true, this.costOffset);
			}
			SemiFunc.UIItemInfoText(this, this.promptName);
			return;
		}
		if (this.itemTag != "")
		{
			this.itemTag = "";
			this.promptName = "";
		}
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00063AF8 File Offset: 0x00061CF8
	public void ShowingInfo()
	{
		if (this.isHeldTimer < 0f)
		{
			return;
		}
		bool grabbedLocal = this.physGrabObject.grabbedLocal;
		if (grabbedLocal || SemiFunc.RunIsShop())
		{
			this.isHeldTimer = 0.2f;
			bool flag = SemiFunc.RunIsShop() && (this.itemType == SemiFunc.itemType.item_upgrade || this.itemType == SemiFunc.itemType.healthPack);
			ItemToggle itemToggle = this.itemToggle;
			if (itemToggle && !itemToggle.disabled && !flag && this.itemTag == "")
			{
				this.itemTag = InputManager.instance.InputDisplayReplaceTags("[interact]");
				if (grabbedLocal)
				{
					this.promptName = this.itemName + " <color=#FFFFFF>[" + this.itemTag + "]</color>";
					return;
				}
				this.promptName = this.itemName;
				return;
			}
			else
			{
				if (!flag && this.showInfoTimer <= 0f && itemToggle && !itemToggle.disabled && this.itemTag != "")
				{
					this.promptName = this.itemName + " <color=#FFFFFF>[" + this.itemTag + "]</color>";
					return;
				}
				this.promptName = this.itemName;
			}
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00063C2A File Offset: 0x00061E2A
	public void ShowInfo()
	{
		if (SemiFunc.RunIsShop() && !this.physGrabObject.grabbedLocal)
		{
			this.isHeldTimer = 0.1f;
			this.showInfoTimer = 0.1f;
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00063C56 File Offset: 0x00061E56
	private void OnDestroy()
	{
		if (this.icon)
		{
			Object.Destroy(this.icon);
		}
	}

	// Token: 0x0400121F RID: 4639
	private PhotonView photonView;

	// Token: 0x04001220 RID: 4640
	public Item item;

	// Token: 0x04001221 RID: 4641
	public Vector3 costOffset;

	// Token: 0x04001222 RID: 4642
	internal SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001223 RID: 4643
	internal ColorPresets colorPreset;

	// Token: 0x04001224 RID: 4644
	internal int value;

	// Token: 0x04001225 RID: 4645
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x04001226 RID: 4646
	private float inStartRoomCheckTimer;

	// Token: 0x04001227 RID: 4647
	private bool inStartRoom;

	// Token: 0x04001228 RID: 4648
	private ItemEquippable itemEquippable;

	// Token: 0x04001229 RID: 4649
	private Transform itemVolume;

	// Token: 0x0400122A RID: 4650
	internal bool shopItem;

	// Token: 0x0400122B RID: 4651
	private PhysGrabObject physGrabObject;

	// Token: 0x0400122C RID: 4652
	internal bool disableUI;

	// Token: 0x0400122D RID: 4653
	internal string itemName;

	// Token: 0x0400122E RID: 4654
	internal string instanceName;

	// Token: 0x0400122F RID: 4655
	internal float showInfoTimer;

	// Token: 0x04001230 RID: 4656
	internal bool hasIcon;

	// Token: 0x04001231 RID: 4657
	public Sprite icon;

	// Token: 0x04001232 RID: 4658
	private SemiFunc.itemType itemType;

	// Token: 0x04001233 RID: 4659
	private ItemToggle itemToggle;

	// Token: 0x04001234 RID: 4660
	private float isHeldTimer;

	// Token: 0x04001235 RID: 4661
	private string itemTag = "";

	// Token: 0x04001236 RID: 4662
	private string promptName = "";

	// Token: 0x04001237 RID: 4663
	private string itemAssetName = "";

	// Token: 0x04001238 RID: 4664
	private float itemValueMin;

	// Token: 0x04001239 RID: 4665
	private float itemValueMax;
}
