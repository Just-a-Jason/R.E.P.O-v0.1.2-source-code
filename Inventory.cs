using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class Inventory : MonoBehaviour
{
	// Token: 0x0600127B RID: 4731 RVA: 0x000A1F0B File Offset: 0x000A010B
	private void Awake()
	{
		if (Inventory.instance != null && Inventory.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Inventory.instance = this;
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x000A1F3C File Offset: 0x000A013C
	public void InventorySpotAddAtIndex(InventorySpot spot, int index)
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		this.inventorySpots[index] = spot;
		using (List<InventorySpot>.Enumerator enumerator = this.inventorySpots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == null)
				{
					return;
				}
			}
		}
		this.spotsFeched = true;
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x000A1FB0 File Offset: 0x000A01B0
	private void Start()
	{
		if (SemiFunc.RunIsArena())
		{
			base.enabled = false;
		}
		this.playerController = base.GetComponent<PlayerController>();
		for (int i = 0; i < 3; i++)
		{
			this.inventorySpots.Add(null);
		}
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x000A1FFC File Offset: 0x000A01FC
	private IEnumerator LateStart()
	{
		yield return null;
		this.physGrabber = this.playerController.playerAvatarScript.physGrabber;
		this.playerAvatar = this.playerController.playerAvatarScript;
		yield break;
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000A200B File Offset: 0x000A020B
	public InventorySpot GetSpotByIndex(int index)
	{
		return this.inventorySpots[index];
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x000A201C File Offset: 0x000A021C
	public bool IsItemEquipped(ItemEquippable item)
	{
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot && inventorySpot.CurrentItem == item)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x000A2088 File Offset: 0x000A0288
	public bool IsSpotOccupied(int index)
	{
		InventorySpot spotByIndex = this.GetSpotByIndex(index);
		return spotByIndex != null && spotByIndex.IsOccupied();
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x000A20AE File Offset: 0x000A02AE
	public List<InventorySpot> GetAllSpots()
	{
		return this.inventorySpots;
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x000A20B8 File Offset: 0x000A02B8
	public int GetFirstFreeInventorySpotIndex()
	{
		List<InventorySpot> allSpots = Inventory.instance.GetAllSpots();
		for (int i = 0; i < allSpots.Count; i++)
		{
			if (!allSpots[i].IsOccupied())
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x000A20F4 File Offset: 0x000A02F4
	public int InventorySpotsOccupied()
	{
		int num = 0;
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot && inventorySpot.IsOccupied())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x000A2158 File Offset: 0x000A0358
	public void InventoryDropAll(Vector3 dropPosition, int playerViewID)
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot.IsOccupied())
			{
				ItemEquippable currentItem = inventorySpot.CurrentItem;
				if (currentItem != null)
				{
					currentItem.ForceUnequip(dropPosition, playerViewID);
				}
			}
		}
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x000A21CC File Offset: 0x000A03CC
	public int GetBatteryStateFromInventorySpot(int index)
	{
		InventorySpot spotByIndex = this.GetSpotByIndex(index);
		if (spotByIndex != null && spotByIndex.IsOccupied())
		{
			ItemEquippable currentItem = spotByIndex.CurrentItem;
			if (currentItem != null)
			{
				ItemBattery component = currentItem.GetComponent<ItemBattery>();
				if (component != null)
				{
					return component.batteryLifeInt;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x000A221C File Offset: 0x000A041C
	public void ForceUnequip()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in this.inventorySpots)
		{
			if (inventorySpot.IsOccupied())
			{
				ItemEquippable currentItem = inventorySpot.CurrentItem;
				if (currentItem)
				{
					if (SemiFunc.IsMultiplayer())
					{
						currentItem.GetComponent<ItemEquippable>().ForceUnequip(this.playerAvatar.PlayerVisionTarget.VisionTransform.position, this.physGrabber.photonView.ViewID);
					}
					else
					{
						currentItem.GetComponent<ItemEquippable>().ForceUnequip(this.playerAvatar.PlayerVisionTarget.VisionTransform.position, -1);
					}
				}
			}
		}
	}

	// Token: 0x04001F4C RID: 8012
	public static Inventory instance;

	// Token: 0x04001F4D RID: 8013
	internal readonly List<InventorySpot> inventorySpots = new List<InventorySpot>();

	// Token: 0x04001F4E RID: 8014
	internal PhysGrabber physGrabber;

	// Token: 0x04001F4F RID: 8015
	private PlayerController playerController;

	// Token: 0x04001F50 RID: 8016
	private PlayerAvatar playerAvatar;

	// Token: 0x04001F51 RID: 8017
	internal bool spotsFeched;
}
