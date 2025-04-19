using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class ItemManager : MonoBehaviour
{
	// Token: 0x06000B58 RID: 2904 RVA: 0x0006483A File Offset: 0x00062A3A
	private void Awake()
	{
		if (!ItemManager.instance)
		{
			ItemManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00064865 File Offset: 0x00062A65
	private void Start()
	{
		base.StartCoroutine(this.TurnOffIconLights());
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00064874 File Offset: 0x00062A74
	public void TurnOffIconLightsAgain()
	{
		base.StartCoroutine(this.TurnOffIconLights());
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00064883 File Offset: 0x00062A83
	private IEnumerator TurnOffIconLights()
	{
		if (SemiFunc.RunIsShop() || SemiFunc.MenuLevel())
		{
			this.itemIconLights.SetActive(false);
			yield break;
		}
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			this.itemIconLights.SetActive(false);
			yield break;
		}
		for (;;)
		{
			if (!this.spawnedItems.Exists((ItemAttributes x) => !x.hasIcon))
			{
				break;
			}
			yield return new WaitForSeconds(0.2f);
		}
		this.itemIconLights.SetActive(false);
		yield break;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00064892 File Offset: 0x00062A92
	public void ResetAllItems()
	{
		this.purchasedItems.Clear();
		this.powerCrystals.Clear();
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x000648AA File Offset: 0x00062AAA
	public void ItemsInitialize()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsLobby() || SemiFunc.RunIsTutorial())
		{
			this.GetAllItemVolumesInScene();
			this.GetPurchasedItems();
			SemiFunc.TruckPopulateItemVolumes();
		}
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x000648DC File Offset: 0x00062ADC
	public int IsInLocalPlayersInventory(string itemName)
	{
		for (int i = 0; i < this.localPlayerInventory.Count; i++)
		{
			if (this.localPlayerInventory[i] == itemName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x00064918 File Offset: 0x00062B18
	public void FetchLocalPlayersInventory()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.localPlayerInventory.Clear();
		Inventory inventory = Inventory.instance;
		if (inventory != null)
		{
			foreach (InventorySpot inventorySpot in inventory.GetAllSpots())
			{
				ItemEquippable itemEquippable = (inventorySpot != null) ? inventorySpot.CurrentItem : null;
				if (itemEquippable != null)
				{
					ItemAttributes component = itemEquippable.GetComponent<ItemAttributes>();
					if (component != null)
					{
						this.localPlayerInventory.Add(component.item.itemName);
					}
				}
			}
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x000649C4 File Offset: 0x00062BC4
	public void GetAllItemVolumesInScene()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		this.itemVolumes.Clear();
		foreach (ItemVolume item in Object.FindObjectsOfType<ItemVolume>())
		{
			this.itemVolumes.Add(item);
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00064A08 File Offset: 0x00062C08
	public void AddSpawnedItem(ItemAttributes item)
	{
		this.spawnedItems.Add(item);
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00064A18 File Offset: 0x00062C18
	private void GetPurchasedItems()
	{
		this.purchasedItems.Clear();
		foreach (KeyValuePair<string, int> keyValuePair in StatsManager.instance.itemsPurchased)
		{
			string key = keyValuePair.Key;
			int value = keyValuePair.Value;
			if (StatsManager.instance.itemDictionary.ContainsKey(key))
			{
				Item item = StatsManager.instance.itemDictionary[key];
				bool flag = item.itemType == SemiFunc.itemType.power_crystal && !SemiFunc.RunIsLobby();
				bool flag2 = item.itemType == SemiFunc.itemType.cart && SemiFunc.RunIsLobby();
				if (!flag && !flag2 && !item.disabled)
				{
					int num = Mathf.Clamp(value, 0, StatsManager.instance.itemDictionary[key].maxAmount);
					for (int i = 0; i < num; i++)
					{
						this.purchasedItems.Add(item);
					}
				}
			}
			else
			{
				Debug.LogWarning("Item '" + key + "' not found in the itemDictionary.");
			}
		}
	}

	// Token: 0x04001254 RID: 4692
	public static ItemManager instance;

	// Token: 0x04001255 RID: 4693
	public List<ItemVolume> itemVolumes;

	// Token: 0x04001256 RID: 4694
	public List<Item> purchasedItems = new List<Item>();

	// Token: 0x04001257 RID: 4695
	public List<PhysGrabObject> powerCrystals = new List<PhysGrabObject>();

	// Token: 0x04001258 RID: 4696
	public List<ItemAttributes> spawnedItems = new List<ItemAttributes>();

	// Token: 0x04001259 RID: 4697
	public List<string> localPlayerInventory = new List<string>();

	// Token: 0x0400125A RID: 4698
	internal bool firstIcon = true;

	// Token: 0x0400125B RID: 4699
	public GameObject itemIconLights;
}
