using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000252 RID: 594
public class InventorySpot : SemiUI
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600126B RID: 4715 RVA: 0x000A1BAA File Offset: 0x0009FDAA
	// (set) Token: 0x0600126C RID: 4716 RVA: 0x000A1BB2 File Offset: 0x0009FDB2
	public ItemEquippable CurrentItem { get; private set; }

	// Token: 0x0600126D RID: 4717 RVA: 0x000A1BBC File Offset: 0x0009FDBC
	protected override void Start()
	{
		this.inventoryIcon = base.GetComponentInChildren<Image>();
		this.photonView = base.GetComponent<PhotonView>();
		this.UpdateUI();
		this.currentState = InventorySpot.SpotState.Empty;
		this.battery = base.GetComponentInChildren<InventoryBattery>();
		base.Start();
		this.uiText = null;
		this.SetEmoji(null);
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x000A1C1B File Offset: 0x0009FE1B
	private IEnumerator LateStart()
	{
		yield return null;
		if (!SemiFunc.MenuLevel() && !SemiFunc.RunIsLobbyMenu() && !SemiFunc.RunIsArena())
		{
			Inventory.instance.InventorySpotAddAtIndex(this, this.inventorySpotIndex);
		}
		yield break;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x000A1C2C File Offset: 0x0009FE2C
	public void SetEmoji(Sprite emoji)
	{
		if (!emoji)
		{
			this.inventoryIcon.enabled = false;
			this.noItem.enabled = true;
			return;
		}
		this.noItem.enabled = false;
		this.inventoryIcon.enabled = true;
		this.inventoryIcon.sprite = emoji;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x000A1C7E File Offset: 0x0009FE7E
	public bool IsOccupied()
	{
		return this.currentState == InventorySpot.SpotState.Occupied;
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x000A1C89 File Offset: 0x0009FE89
	public void EquipItem(ItemEquippable item)
	{
		if (this.currentState != InventorySpot.SpotState.Empty)
		{
			return;
		}
		this.CurrentItem = item;
		this.currentState = InventorySpot.SpotState.Occupied;
		this.UpdateUI();
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x000A1CA8 File Offset: 0x0009FEA8
	public void UnequipItem()
	{
		if (this.currentState != InventorySpot.SpotState.Occupied)
		{
			return;
		}
		this.CurrentItem = null;
		this.currentState = InventorySpot.SpotState.Empty;
		this.UpdateUI();
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x000A1CC8 File Offset: 0x0009FEC8
	public void UpdateUI()
	{
		if (this.currentState == InventorySpot.SpotState.Occupied && this.CurrentItem != null)
		{
			base.SemiUISpringScale(0.5f, 2f, 0.2f);
			this.SetEmoji(this.CurrentItem.GetComponent<ItemAttributes>().icon);
			return;
		}
		this.SetEmoji(null);
		base.SemiUISpringScale(0.5f, 2f, 0.2f);
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000A1D34 File Offset: 0x0009FF34
	protected override void Update()
	{
		if (SemiFunc.InputDown(InputKey.Inventory1) && this.inventorySpotIndex == 0)
		{
			this.HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory2) && this.inventorySpotIndex == 1)
		{
			this.HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory3) && this.inventorySpotIndex == 2)
		{
			this.HandleInput();
		}
		InventorySpot.SpotState spotState = this.currentState;
		if (spotState != InventorySpot.SpotState.Empty)
		{
			if (spotState == InventorySpot.SpotState.Occupied)
			{
				this.StateOccupied();
			}
		}
		else
		{
			this.StateEmpty();
		}
		base.Update();
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x000A1DB0 File Offset: 0x0009FFB0
	private void HandleInput()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (PlayerController.instance.InputDisableTimer > 0f)
		{
			return;
		}
		if (!this.handleInput && Time.time - this.lastEquipTime < this.equipCooldown)
		{
			return;
		}
		PhysGrabber.instance.OverrideGrabRelease();
		this.lastEquipTime = Time.time;
		this.handleInput = false;
		if (this.IsOccupied())
		{
			this.CurrentItem.RequestUnequip();
			return;
		}
		this.AttemptEquipItem();
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x000A1E2C File Offset: 0x000A002C
	private void AttemptEquipItem()
	{
		ItemEquippable itemPlayerIsHolding = this.GetItemPlayerIsHolding();
		if (itemPlayerIsHolding != null)
		{
			itemPlayerIsHolding.RequestEquip(this.inventorySpotIndex, PhysGrabber.instance.photonView.ViewID);
		}
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x000A1E64 File Offset: 0x000A0064
	private ItemEquippable GetItemPlayerIsHolding()
	{
		if (!PhysGrabber.instance.grabbed)
		{
			return null;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (grabbedPhysGrabObject == null)
		{
			return null;
		}
		return grabbedPhysGrabObject.GetComponent<ItemEquippable>();
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x000A1E8C File Offset: 0x000A008C
	private void StateOccupied()
	{
		if (this.currentState != InventorySpot.SpotState.Occupied || !this.CurrentItem)
		{
			return;
		}
		if (this.CurrentItem.GetComponent<ItemBattery>())
		{
			this.battery.BatteryFetch();
			this.battery.BatteryShow();
		}
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000A1ED8 File Offset: 0x000A00D8
	private void StateEmpty()
	{
		if (this.currentState != InventorySpot.SpotState.Empty)
		{
			return;
		}
		base.SemiUIScoot(new Vector2(0f, -20f));
	}

	// Token: 0x04001F42 RID: 8002
	[FormerlySerializedAs("SpotIndex")]
	public int inventorySpotIndex;

	// Token: 0x04001F44 RID: 8004
	private PhotonView photonView;

	// Token: 0x04001F45 RID: 8005
	private float equipCooldown = 0.2f;

	// Token: 0x04001F46 RID: 8006
	private float lastEquipTime;

	// Token: 0x04001F47 RID: 8007
	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private InventorySpot.SpotState currentState;

	// Token: 0x04001F48 RID: 8008
	private InventoryBattery battery;

	// Token: 0x04001F49 RID: 8009
	internal Image inventoryIcon;

	// Token: 0x04001F4A RID: 8010
	private bool handleInput;

	// Token: 0x04001F4B RID: 8011
	public TextMeshProUGUI noItem;

	// Token: 0x020003B3 RID: 947
	public enum SpotState
	{
		// Token: 0x040028A0 RID: 10400
		Empty,
		// Token: 0x040028A1 RID: 10401
		Occupied
	}
}
