using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class ShopManager : MonoBehaviour
{
	// Token: 0x06000C17 RID: 3095 RVA: 0x0006B685 File Offset: 0x00069885
	private void Awake()
	{
		if (!ShopManager.instance)
		{
			ShopManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0006B6B0 File Offset: 0x000698B0
	private void Update()
	{
		if (SemiFunc.RunIsShop() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.shopTutorial)
			{
				if (TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialShop, 1))
				{
					TutorialDirector.instance.ActivateTip("Shop", 2f, false);
				}
				this.shopTutorial = false;
				return;
			}
		}
		else
		{
			this.shopTutorial = true;
		}
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0006B70C File Offset: 0x0006990C
	public void ShopCheck()
	{
		this.totalCost = 0;
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes itemAttributes in this.shoppingList)
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
					this.totalCost += itemAttributes.value;
				}
			}
			else
			{
				list.Add(itemAttributes);
			}
		}
		foreach (ItemAttributes item in list)
		{
			this.shoppingList.Remove(item);
		}
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0006B7F0 File Offset: 0x000699F0
	public void ShoppingListItemAdd(ItemAttributes item)
	{
		this.shoppingList.Add(item);
		SemiFunc.ShopUpdateCost();
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0006B803 File Offset: 0x00069A03
	public void ShoppingListItemRemove(ItemAttributes item)
	{
		this.shoppingList.Remove(item);
		SemiFunc.ShopUpdateCost();
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0006B817 File Offset: 0x00069A17
	public void ShopInitialize()
	{
		if (SemiFunc.RunIsShop())
		{
			this.totalCurrency = SemiFunc.StatGetRunCurrency();
			this.totalCost = 0;
			this.shoppingList.Clear();
			this.GetAllItemsFromStatsManager();
			this.GetAllItemVolumesInScene();
			SemiFunc.ShopPopulateItemVolumes();
		}
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0006B850 File Offset: 0x00069A50
	private void GetAllItemVolumesInScene()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.itemVolumes.Clear();
		foreach (ItemVolume itemVolume in Object.FindObjectsOfType<ItemVolume>())
		{
			if (itemVolume.itemSecretShopType == SemiFunc.itemSecretShopType.none)
			{
				this.itemVolumes.Add(itemVolume);
			}
			else
			{
				if (!this.secretItemVolumes.ContainsKey(itemVolume.itemSecretShopType))
				{
					this.secretItemVolumes.Add(itemVolume.itemSecretShopType, new List<ItemVolume>());
				}
				this.secretItemVolumes[itemVolume.itemSecretShopType].Add(itemVolume);
			}
		}
		foreach (List<ItemVolume> list in this.secretItemVolumes.Values)
		{
			list.Shuffle<ItemVolume>();
		}
		this.itemVolumes.Shuffle<ItemVolume>();
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0006B934 File Offset: 0x00069B34
	private void GetAllItemsFromStatsManager()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.potentialItems.Clear();
		this.potentialItemConsumables.Clear();
		this.potentialItemUpgrades.Clear();
		this.potentialItemHealthPacks.Clear();
		this.potentialSecretItems.Clear();
		foreach (Item item in StatsManager.instance.itemDictionary.Values)
		{
			int num = SemiFunc.StatGetItemsPurchased(item.itemAssetName);
			float num2 = item.value.valueMax / 1000f * this.itemValueMultiplier;
			if (item.itemType == SemiFunc.itemType.item_upgrade)
			{
				num2 -= num2 * 0.05f * (float)(GameDirector.instance.PlayerList.Count - 1);
				int itemsUpgradesPurchased = StatsManager.instance.GetItemsUpgradesPurchased(item.itemAssetName);
				num2 += num2 * this.upgradeValueIncrease * (float)itemsUpgradesPurchased;
				num2 = Mathf.Ceil(num2);
			}
			if (item.itemType == SemiFunc.itemType.healthPack)
			{
				num2 += num2 * this.healthPackValueIncrease * (float)RunManager.instance.levelsCompleted;
				num2 = Mathf.Ceil(num2);
			}
			if (item.itemType == SemiFunc.itemType.power_crystal)
			{
				num2 += num2 * this.crystalValueIncrease * (float)RunManager.instance.levelsCompleted;
				num2 = Mathf.Ceil(num2);
			}
			float num3 = Mathf.Clamp(num2, 1f, num2);
			bool flag = item.itemType == SemiFunc.itemType.power_crystal;
			bool flag2 = item.itemType == SemiFunc.itemType.item_upgrade;
			bool flag3 = item.itemType == SemiFunc.itemType.healthPack;
			int maxAmountInShop = item.maxAmountInShop;
			if (num < maxAmountInShop && (!item.maxPurchase || StatsManager.instance.GetItemsUpgradesPurchasedTotal(item.itemAssetName) < item.maxPurchaseAmount) && (num3 <= (float)this.totalCurrency || Random.Range(0, 100) < 25))
			{
				for (int i = 0; i < maxAmountInShop - num; i++)
				{
					if (flag2)
					{
						this.potentialItemUpgrades.Add(item);
					}
					else if (flag3)
					{
						this.potentialItemHealthPacks.Add(item);
					}
					else if (flag)
					{
						this.potentialItemConsumables.Add(item);
					}
					else if (item.itemSecretShopType == SemiFunc.itemSecretShopType.none)
					{
						this.potentialItems.Add(item);
					}
					else
					{
						if (!this.potentialSecretItems.ContainsKey(item.itemSecretShopType))
						{
							this.potentialSecretItems.Add(item.itemSecretShopType, new List<Item>());
						}
						this.potentialSecretItems[item.itemSecretShopType].Add(item);
					}
				}
			}
		}
		this.potentialItems.Shuffle<Item>();
		this.potentialItemConsumables.Shuffle<Item>();
		this.potentialItemUpgrades.Shuffle<Item>();
		this.potentialItemHealthPacks.Shuffle<Item>();
		foreach (List<Item> list in this.potentialSecretItems.Values)
		{
			list.Shuffle<Item>();
		}
	}

	// Token: 0x04001379 RID: 4985
	public static ShopManager instance;

	// Token: 0x0400137A RID: 4986
	public Transform itemRotateHelper;

	// Token: 0x0400137B RID: 4987
	public List<ItemVolume> itemVolumes;

	// Token: 0x0400137C RID: 4988
	public List<Item> potentialItems = new List<Item>();

	// Token: 0x0400137D RID: 4989
	public List<Item> potentialItemConsumables = new List<Item>();

	// Token: 0x0400137E RID: 4990
	public List<Item> potentialItemUpgrades = new List<Item>();

	// Token: 0x0400137F RID: 4991
	public List<Item> potentialItemHealthPacks = new List<Item>();

	// Token: 0x04001380 RID: 4992
	public Dictionary<SemiFunc.itemSecretShopType, List<ItemVolume>> secretItemVolumes = new Dictionary<SemiFunc.itemSecretShopType, List<ItemVolume>>();

	// Token: 0x04001381 RID: 4993
	public Dictionary<SemiFunc.itemSecretShopType, List<Item>> potentialSecretItems = new Dictionary<SemiFunc.itemSecretShopType, List<Item>>();

	// Token: 0x04001382 RID: 4994
	public int itemSpawnTargetAmount = 8;

	// Token: 0x04001383 RID: 4995
	public int itemConsumablesAmount = 6;

	// Token: 0x04001384 RID: 4996
	public int itemUpgradesAmount = 3;

	// Token: 0x04001385 RID: 4997
	public int itemHealthPacksAmount = 3;

	// Token: 0x04001386 RID: 4998
	internal List<ItemAttributes> shoppingList = new List<ItemAttributes>();

	// Token: 0x04001387 RID: 4999
	[HideInInspector]
	public int totalCost;

	// Token: 0x04001388 RID: 5000
	[HideInInspector]
	public int totalCurrency;

	// Token: 0x04001389 RID: 5001
	[HideInInspector]
	public bool isThief;

	// Token: 0x0400138A RID: 5002
	[HideInInspector]
	public Transform extractionPoint;

	// Token: 0x0400138B RID: 5003
	internal float itemValueMultiplier = 4f;

	// Token: 0x0400138C RID: 5004
	internal float upgradeValueIncrease = 0.5f;

	// Token: 0x0400138D RID: 5005
	internal float healthPackValueIncrease = 0.05f;

	// Token: 0x0400138E RID: 5006
	internal float crystalValueIncrease = 0.2f;

	// Token: 0x0400138F RID: 5007
	private bool shopTutorial;
}
