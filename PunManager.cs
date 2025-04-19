using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class PunManager : MonoBehaviour
{
	// Token: 0x06000BCB RID: 3019 RVA: 0x00069178 File Offset: 0x00067378
	private void Awake()
	{
		PunManager.instance = this;
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00069180 File Offset: 0x00067380
	private void Start()
	{
		this.statsManager = StatsManager.instance;
		this.shopManager = ShopManager.instance;
		this.itemManager = ItemManager.instance;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x000691B0 File Offset: 0x000673B0
	public void SetItemName(string name, ItemAttributes itemAttributes, int photonViewID)
	{
		if (photonViewID == -1)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetItemNameRPC", RpcTarget.All, new object[]
			{
				name,
				photonViewID
			});
			return;
		}
		this.SetItemNameLOGIC(name, photonViewID, itemAttributes);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00069200 File Offset: 0x00067400
	private void SetItemNameLOGIC(string name, int photonViewID, ItemAttributes _itemAttributes = null)
	{
		if (photonViewID == -1 && SemiFunc.IsMultiplayer())
		{
			return;
		}
		ItemAttributes itemAttributes = _itemAttributes;
		if (SemiFunc.IsMultiplayer())
		{
			itemAttributes = PhotonView.Find(photonViewID).GetComponent<ItemAttributes>();
		}
		if (_itemAttributes == null && !SemiFunc.IsMultiplayer())
		{
			return;
		}
		itemAttributes.instanceName = name;
		ItemBattery component = itemAttributes.GetComponent<ItemBattery>();
		if (component)
		{
			component.SetBatteryLife(this.statsManager.itemStatBattery[name]);
		}
		ItemEquippable component2 = itemAttributes.GetComponent<ItemEquippable>();
		if (component2)
		{
			int spot = 0;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			int hashCode = name.GetHashCode();
			bool flag = false;
			PlayerAvatar playerAvatar = null;
			foreach (PlayerAvatar playerAvatar2 in list)
			{
				string steamID = playerAvatar2.steamID;
				if (StatsManager.instance.playerInventorySpot1[steamID] == hashCode && StatsManager.instance.playerInventorySpot1Taken[steamID] == 0)
				{
					spot = 0;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot1Taken[steamID] = 1;
					break;
				}
				if (StatsManager.instance.playerInventorySpot2[steamID] == hashCode && StatsManager.instance.playerInventorySpot2Taken[steamID] == 0)
				{
					spot = 1;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot2Taken[steamID] = 1;
					break;
				}
				if (StatsManager.instance.playerInventorySpot3[steamID] == hashCode && StatsManager.instance.playerInventorySpot3Taken[steamID] == 0)
				{
					spot = 2;
					flag = true;
					playerAvatar = playerAvatar2;
					StatsManager.instance.playerInventorySpot3Taken[steamID] = 1;
					break;
				}
			}
			if (flag)
			{
				int requestingPlayerId = -1;
				if (SemiFunc.IsMultiplayer())
				{
					requestingPlayerId = playerAvatar.photonView.ViewID;
				}
				component2.RequestEquip(spot, requestingPlayerId);
			}
		}
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x000693D4 File Offset: 0x000675D4
	public void CrownPlayerSync(string _steamID)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, new object[]
			{
				_steamID
			});
		}
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x000693FF File Offset: 0x000675FF
	[PunRPC]
	public void CrownPlayerRPC(string _steamID)
	{
		SessionManager.instance.crownedPlayerSteamID = _steamID;
		PlayerCrownSet component = Object.Instantiate<GameObject>(SessionManager.instance.crownPrefab).GetComponent<PlayerCrownSet>();
		component.crownOwnerFetched = true;
		component.crownOwnerSteamID = _steamID;
		StatsManager.instance.UpdateCrown(_steamID);
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00069438 File Offset: 0x00067638
	[PunRPC]
	public void SetItemNameRPC(string name, int photonViewID)
	{
		this.SetItemNameLOGIC(name, photonViewID, null);
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00069444 File Offset: 0x00067644
	public void ShopUpdateCost()
	{
		int num = 0;
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes itemAttributes in ShopManager.instance.shoppingList)
		{
			if (itemAttributes)
			{
				itemAttributes.roomVolumeCheck.CheckSet();
				if (!itemAttributes.roomVolumeCheck.inExtractionPoint)
				{
					list.Add(itemAttributes);
				}
				else
				{
					num += itemAttributes.value;
				}
			}
			else
			{
				list.Add(itemAttributes);
			}
		}
		foreach (ItemAttributes item in list)
		{
			ShopManager.instance.shoppingList.Remove(item);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("UpdateShoppingCostRPC", RpcTarget.All, new object[]
				{
					num
				});
				return;
			}
			this.UpdateShoppingCostRPC(num);
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00069558 File Offset: 0x00067758
	private void Update()
	{
		if (SemiFunc.FPSImpulse5() && SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient() && this.totalHaul != RoundDirector.instance.totalHaul)
		{
			this.totalHaul = RoundDirector.instance.totalHaul;
			this.photonView.RPC("SyncHaul", RpcTarget.Others, new object[]
			{
				this.totalHaul
			});
		}
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x000695C1 File Offset: 0x000677C1
	[PunRPC]
	public void SyncHaul(int value)
	{
		RoundDirector.instance.totalHaul = value;
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x000695CE File Offset: 0x000677CE
	[PunRPC]
	public void UpdateShoppingCostRPC(int value)
	{
		ShopManager.instance.totalCost = value;
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x000695DC File Offset: 0x000677DC
	public void ShopPopulateItemVolumes()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (KeyValuePair<SemiFunc.itemSecretShopType, List<ItemVolume>> keyValuePair in ShopManager.instance.secretItemVolumes)
		{
			List<ItemVolume> value = keyValuePair.Value;
			foreach (ItemVolume itemVolume in value)
			{
				if (ShopManager.instance.potentialSecretItems.ContainsKey(keyValuePair.Key))
				{
					List<Item> list = ShopManager.instance.potentialSecretItems[keyValuePair.Key];
					if (Random.Range(0, 3) == 0 && itemVolume)
					{
						this.SpawnShopItem(itemVolume, ShopManager.instance.potentialSecretItems[keyValuePair.Key], ref num, true);
					}
				}
			}
			foreach (ItemVolume itemVolume2 in value)
			{
				if (itemVolume2)
				{
					Object.Destroy(itemVolume2.gameObject);
				}
			}
		}
		foreach (ItemVolume itemVolume3 in this.shopManager.itemVolumes)
		{
			if (this.shopManager.potentialItems.Count == 0 && this.shopManager.potentialItemConsumables.Count == 0)
			{
				break;
			}
			if ((num >= this.shopManager.itemSpawnTargetAmount || !this.SpawnShopItem(itemVolume3, this.shopManager.potentialItems, ref num, false)) && (num2 >= this.shopManager.itemConsumablesAmount || !this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemConsumables, ref num2, false)))
			{
				if (num3 < this.shopManager.itemUpgradesAmount)
				{
					this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemUpgrades, ref num3, false);
				}
				if (num4 < this.shopManager.itemHealthPacksAmount)
				{
					this.SpawnShopItem(itemVolume3, this.shopManager.potentialItemHealthPacks, ref num4, false);
				}
			}
		}
		foreach (ItemVolume itemVolume4 in this.shopManager.itemVolumes)
		{
			Object.Destroy(itemVolume4.gameObject);
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x0006988C File Offset: 0x00067A8C
	private bool SpawnShopItem(ItemVolume itemVolume, List<Item> itemList, ref int spawnCount, bool isSecret = false)
	{
		for (int i = itemList.Count - 1; i >= 0; i--)
		{
			Item item = itemList[i];
			if (item.itemVolume == itemVolume.itemVolume)
			{
				ShopManager.instance.itemRotateHelper.transform.parent = itemVolume.transform;
				ShopManager.instance.itemRotateHelper.transform.localRotation = item.spawnRotationOffset;
				Quaternion rotation = ShopManager.instance.itemRotateHelper.transform.rotation;
				ShopManager.instance.itemRotateHelper.transform.parent = ShopManager.instance.transform;
				string prefabName = "Items/" + item.prefab.name;
				if (SemiFunc.IsMultiplayer())
				{
					PhotonNetwork.InstantiateRoomObject(prefabName, itemVolume.transform.position, rotation, 0, null);
				}
				else
				{
					Object.Instantiate<GameObject>(item.prefab, itemVolume.transform.position, rotation);
				}
				itemList.RemoveAt(i);
				if (!isSecret)
				{
					spawnCount++;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00069994 File Offset: 0x00067B94
	public void TruckPopulateItemVolumes()
	{
		ItemManager.instance.spawnedItems.Clear();
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		List<ItemVolume> list = new List<ItemVolume>(this.itemManager.itemVolumes);
		List<Item> list2 = new List<Item>(this.itemManager.purchasedItems);
		while (list.Count > 0 && list2.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				Item item = list2[i];
				ItemVolume itemVolume = list.Find((ItemVolume v) => v.itemVolume == item.itemVolume);
				if (itemVolume)
				{
					this.SpawnItem(item, itemVolume);
					list.Remove(itemVolume);
					list2.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		foreach (ItemVolume itemVolume2 in this.itemManager.itemVolumes)
		{
			Object.Destroy(itemVolume2.gameObject);
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00069AA8 File Offset: 0x00067CA8
	private void SpawnItem(Item item, ItemVolume volume)
	{
		ShopManager.instance.itemRotateHelper.transform.parent = volume.transform;
		ShopManager.instance.itemRotateHelper.transform.localRotation = item.spawnRotationOffset;
		Quaternion rotation = ShopManager.instance.itemRotateHelper.transform.rotation;
		ShopManager.instance.itemRotateHelper.transform.parent = ShopManager.instance.transform;
		if (SemiFunc.IsMasterClient())
		{
			PhotonNetwork.InstantiateRoomObject("Items/" + item.prefab.name, volume.transform.position, rotation, 0, null);
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(item.prefab, volume.transform.position, rotation);
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00069B6C File Offset: 0x00067D6C
	public void AddingItem(string itemName, int index, int photonViewID, ItemAttributes itemAttributes)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("AddingItemRPC", RpcTarget.All, new object[]
			{
				itemName,
				index,
				photonViewID
			});
			return;
		}
		this.AddingItemLOGIC(itemName, index, photonViewID, itemAttributes);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00069BBC File Offset: 0x00067DBC
	private void AddingItemLOGIC(string itemName, int index, int photonViewID, ItemAttributes itemAttributes = null)
	{
		if (!StatsManager.instance.item.ContainsKey(itemName))
		{
			StatsManager.instance.item.Add(itemName, index);
			StatsManager.instance.itemStatBattery.Add(itemName, 100);
			StatsManager.instance.takenItemNames.Add(itemName);
		}
		else
		{
			Debug.LogWarning("Item " + itemName + " already exists in the dictionary");
		}
		this.SetItemNameLOGIC(itemName, photonViewID, itemAttributes);
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x00069C2F File Offset: 0x00067E2F
	[PunRPC]
	public void AddingItemRPC(string itemName, int index, int photonViewID)
	{
		this.AddingItemLOGIC(itemName, index, photonViewID, null);
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x00069C3B File Offset: 0x00067E3B
	public void UpdateStat(string dictionaryName, string key, int value)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("UpdateStatRPC", RpcTarget.All, new object[]
			{
				dictionaryName,
				key,
				value
			});
			return;
		}
		this.UpdateStatRPC(dictionaryName, key, value);
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00069C76 File Offset: 0x00067E76
	[PunRPC]
	public void UpdateStatRPC(string dictionaryName, string key, int value)
	{
		StatsManager.instance.DictionaryUpdateValue(dictionaryName, key, value);
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00069C88 File Offset: 0x00067E88
	public int SetRunStatSet(string statName, int value)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.statsManager.runStats[statName] = value;
				this.photonView.RPC("SetRunStatRPC", RpcTarget.Others, new object[]
				{
					statName,
					value
				});
			}
			else
			{
				this.statsManager.runStats[statName] = value;
			}
		}
		return this.statsManager.runStats[statName];
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00069CFE File Offset: 0x00067EFE
	[PunRPC]
	public void SetRunStatRPC(string statName, int value)
	{
		this.statsManager.runStats[statName] = value;
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x00069D14 File Offset: 0x00067F14
	public int UpgradeItemBattery(string itemName)
	{
		Dictionary<string, int> itemBatteryUpgrades = this.statsManager.itemBatteryUpgrades;
		itemBatteryUpgrades[itemName]++;
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradeItemBatteryRPC", RpcTarget.Others, new object[]
			{
				itemName,
				this.statsManager.itemBatteryUpgrades[itemName]
			});
		}
		return this.statsManager.itemBatteryUpgrades[itemName];
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00069D8B File Offset: 0x00067F8B
	[PunRPC]
	public void UpgradeItemBatteryRPC(string itemName, int value)
	{
		this.statsManager.itemBatteryUpgrades[itemName] = value;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00069DA0 File Offset: 0x00067FA0
	public int UpgradePlayerHealth(string playerName)
	{
		Dictionary<string, int> playerUpgradeHealth = this.statsManager.playerUpgradeHealth;
		playerUpgradeHealth[playerName]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateHealthRightAway(playerName);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerHealthRPC", RpcTarget.Others, new object[]
			{
				playerName,
				this.statsManager.playerUpgradeHealth[playerName]
			});
		}
		return this.statsManager.playerUpgradeHealth[playerName];
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x00069E25 File Offset: 0x00068025
	[PunRPC]
	public void UpgradePlayerHealthRPC(string playerName, int value)
	{
		this.statsManager.playerUpgradeHealth[playerName] = value;
		this.UpdateHealthRightAway(playerName);
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00069E40 File Offset: 0x00068040
	private void UpdateHealthRightAway(string playerName)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(playerName);
		if (playerAvatar == SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.playerHealth.maxHealth += 20;
			playerAvatar.playerHealth.Heal(20, false);
		}
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x00069E84 File Offset: 0x00068084
	public int UpgradePlayerEnergy(string _steamID)
	{
		Dictionary<string, int> playerUpgradeStamina = this.statsManager.playerUpgradeStamina;
		playerUpgradeStamina[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateEnergyRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerEnergyRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeStamina[_steamID]
			});
		}
		return this.statsManager.playerUpgradeStamina[_steamID];
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00069F09 File Offset: 0x00068109
	[PunRPC]
	public void UpgradePlayerEnergyRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeStamina[_steamID] = value;
		this.UpdateEnergyRightAway(_steamID);
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00069F24 File Offset: 0x00068124
	private void UpdateEnergyRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.EnergyStart += 10f;
			PlayerController.instance.EnergyCurrent = PlayerController.instance.EnergyStart;
		}
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00069F64 File Offset: 0x00068164
	public int UpgradePlayerExtraJump(string _steamID)
	{
		Dictionary<string, int> playerUpgradeExtraJump = this.statsManager.playerUpgradeExtraJump;
		playerUpgradeExtraJump[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateExtraJumpRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeExtraJump[_steamID]
			});
		}
		return this.statsManager.playerUpgradeExtraJump[_steamID];
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00069FE9 File Offset: 0x000681E9
	[PunRPC]
	public void UpgradePlayerExtraJumpRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeExtraJump[_steamID] = value;
		this.UpdateExtraJumpRightAway(_steamID);
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0006A004 File Offset: 0x00068204
	private void UpdateExtraJumpRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.JumpExtra++;
		}
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x0006A02C File Offset: 0x0006822C
	public int UpgradeMapPlayerCount(string _steamID)
	{
		Dictionary<string, int> playerUpgradeMapPlayerCount = this.statsManager.playerUpgradeMapPlayerCount;
		playerUpgradeMapPlayerCount[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateMapPlayerCountRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradeMapPlayerCountRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeMapPlayerCount[_steamID]
			});
		}
		return this.statsManager.playerUpgradeMapPlayerCount[_steamID];
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0006A0B1 File Offset: 0x000682B1
	[PunRPC]
	public void UpgradeMapPlayerCountRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeMapPlayerCount[_steamID] = value;
		this.UpdateMapPlayerCountRightAway(_steamID);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0006A0CC File Offset: 0x000682CC
	private void UpdateMapPlayerCountRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar == SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.upgradeMapPlayerCount++;
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0006A0FC File Offset: 0x000682FC
	public int UpgradePlayerTumbleLaunch(string _steamID)
	{
		Dictionary<string, int> playerUpgradeLaunch = this.statsManager.playerUpgradeLaunch;
		playerUpgradeLaunch[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateTumbleLaunchRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeLaunch[_steamID]
			});
		}
		return this.statsManager.playerUpgradeLaunch[_steamID];
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0006A181 File Offset: 0x00068381
	[PunRPC]
	public void UpgradePlayerTumbleLaunchRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeLaunch[_steamID] = value;
		this.UpdateTumbleLaunchRightAway(_steamID);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0006A19C File Offset: 0x0006839C
	private void UpdateTumbleLaunchRightAway(string _steamID)
	{
		SemiFunc.PlayerAvatarGetFromSteamID(_steamID).tumble.tumbleLaunch++;
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0006A1B8 File Offset: 0x000683B8
	public int UpgradePlayerSprintSpeed(string _steamID)
	{
		Dictionary<string, int> playerUpgradeSpeed = this.statsManager.playerUpgradeSpeed;
		playerUpgradeSpeed[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateSprintSpeedRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeSpeed[_steamID]
			});
		}
		return this.statsManager.playerUpgradeSpeed[_steamID];
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0006A23D File Offset: 0x0006843D
	[PunRPC]
	public void UpgradePlayerSprintSpeedRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeSpeed[_steamID] = value;
		this.UpdateSprintSpeedRightAway(_steamID);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0006A258 File Offset: 0x00068458
	private void UpdateSprintSpeedRightAway(string _steamID)
	{
		if (SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.SprintSpeed += 1f;
			PlayerController.instance.SprintSpeedUpgrades += 1f;
		}
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x0006A298 File Offset: 0x00068498
	public int UpgradePlayerGrabStrength(string _steamID)
	{
		Dictionary<string, int> playerUpgradeStrength = this.statsManager.playerUpgradeStrength;
		playerUpgradeStrength[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateGrabStrengthRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeStrength[_steamID]
			});
		}
		return this.statsManager.playerUpgradeStrength[_steamID];
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0006A31D File Offset: 0x0006851D
	[PunRPC]
	public void UpgradePlayerGrabStrengthRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeStrength[_steamID] = value;
		this.UpdateGrabStrengthRightAway(_steamID);
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0006A338 File Offset: 0x00068538
	private void UpdateGrabStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.grabStrength += 0.2f;
		}
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0006A36C File Offset: 0x0006856C
	public int UpgradePlayerThrowStrength(string _steamID)
	{
		Dictionary<string, int> playerUpgradeThrow = this.statsManager.playerUpgradeThrow;
		playerUpgradeThrow[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateThrowStrengthRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeThrow[_steamID]
			});
		}
		return this.statsManager.playerUpgradeThrow[_steamID];
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0006A3F1 File Offset: 0x000685F1
	[PunRPC]
	public void UpgradePlayerThrowStrengthRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeThrow[_steamID] = value;
		this.UpdateGrabStrengthRightAway(_steamID);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0006A40C File Offset: 0x0006860C
	private void UpdateThrowStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.throwStrength += 0.3f;
		}
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x0006A440 File Offset: 0x00068640
	public int UpgradePlayerGrabRange(string _steamID)
	{
		Dictionary<string, int> playerUpgradeRange = this.statsManager.playerUpgradeRange;
		playerUpgradeRange[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.UpdateGrabRangeRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.Others, new object[]
			{
				_steamID,
				this.statsManager.playerUpgradeRange[_steamID]
			});
		}
		return this.statsManager.playerUpgradeRange[_steamID];
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x0006A4C5 File Offset: 0x000686C5
	[PunRPC]
	public void UpgradePlayerGrabRangeRPC(string _steamID, int value)
	{
		this.statsManager.playerUpgradeRange[_steamID] = value;
		this.UpdateGrabRangeRightAway(_steamID);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0006A4E0 File Offset: 0x000686E0
	private void UpdateGrabRangeRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			playerAvatar.physGrabber.grabRange += 1f;
		}
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x0006A514 File Offset: 0x00068714
	public void SyncAllDictionaries()
	{
		StatsManager.instance.statsSynced = true;
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncData.Clear();
			Hashtable hashtable = new Hashtable();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.statsManager.dictionaryOfDictionaries)
			{
				string key = keyValuePair.Key;
				hashtable.Add(key, this.ConvertToHashtable(keyValuePair.Value));
				num++;
				num2++;
				num3++;
				list.Add(key);
				if (num > 3 || num2 == this.statsManager.dictionaryOfDictionaries.Count)
				{
					this.syncData.Add(hashtable);
					list.Clear();
					num = 0;
				}
			}
			for (int i = 0; i < this.syncData.Count; i++)
			{
				bool flag = i == this.syncData.Count - 1;
				Hashtable hashtable2 = this.syncData[i];
				this.photonView.RPC("ReceiveSyncData", RpcTarget.Others, new object[]
				{
					hashtable2,
					flag
				});
			}
			this.syncData.Clear();
		}
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x0006A670 File Offset: 0x00068870
	private Hashtable ConvertToHashtable(Dictionary<string, int> dictionary)
	{
		Hashtable hashtable = new Hashtable();
		foreach (KeyValuePair<string, int> keyValuePair in dictionary)
		{
			hashtable.Add(keyValuePair.Key, keyValuePair.Value);
		}
		return hashtable;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0006A6D8 File Offset: 0x000688D8
	private Dictionary<K, V> ConvertToDictionary<K, V>(Hashtable hashtable)
	{
		Dictionary<K, V> dictionary = new Dictionary<K, V>();
		foreach (DictionaryEntry dictionaryEntry in hashtable)
		{
			dictionary.Add((K)((object)dictionaryEntry.Key), (V)((object)dictionaryEntry.Value));
		}
		return dictionary;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0006A744 File Offset: 0x00068944
	[PunRPC]
	public void ReceiveSyncData(Hashtable data, bool finalChunk)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.statsManager.dictionaryOfDictionaries)
		{
			string key = keyValuePair.Key;
			if (data.ContainsKey(key))
			{
				list.Add(key);
			}
		}
		foreach (string key2 in list)
		{
			Dictionary<string, int> dictionary = this.statsManager.dictionaryOfDictionaries[key2];
			foreach (DictionaryEntry dictionaryEntry in ((Hashtable)data[key2]))
			{
				string key3 = (string)dictionaryEntry.Key;
				int value = (int)dictionaryEntry.Value;
				dictionary[key3] = value;
			}
		}
		if (finalChunk)
		{
			StatsManager.instance.statsSynced = true;
		}
	}

	// Token: 0x0400133D RID: 4925
	internal PhotonView photonView;

	// Token: 0x0400133E RID: 4926
	internal StatsManager statsManager;

	// Token: 0x0400133F RID: 4927
	private ShopManager shopManager;

	// Token: 0x04001340 RID: 4928
	private ItemManager itemManager;

	// Token: 0x04001341 RID: 4929
	public static PunManager instance;

	// Token: 0x04001342 RID: 4930
	private List<Hashtable> syncData = new List<Hashtable>();

	// Token: 0x04001343 RID: 4931
	public PhotonLagSimulationGui lagSimulationGui;

	// Token: 0x04001344 RID: 4932
	private int totalHaul;
}
