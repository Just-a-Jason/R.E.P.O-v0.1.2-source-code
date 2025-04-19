using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000254 RID: 596
public class InventoryBattery : MonoBehaviour
{
	// Token: 0x06001289 RID: 4745 RVA: 0x000A22F8 File Offset: 0x000A04F8
	private void Start()
	{
		this.batteryState = 6;
		this.batteryImage = base.GetComponent<RawImage>();
		this.batteryImage.enabled = false;
		this.originalLocalScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000A2348 File Offset: 0x000A0548
	public void BatteryFetch()
	{
		if (!Inventory.instance)
		{
			return;
		}
		if (!this.batteryImage)
		{
			return;
		}
		int batteryStateFromInventorySpot = Inventory.instance.GetBatteryStateFromInventorySpot(this.inventorySpot);
		if (batteryStateFromInventorySpot != -1 && this.redBlinkTimer == 0f)
		{
			this.batteryState = batteryStateFromInventorySpot;
			this.batteryImage.color = new Color(1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000A23BD File Offset: 0x000A05BD
	public void BatteryShow()
	{
		this.batteryShowTimer = 0.2f;
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x000A23CC File Offset: 0x000A05CC
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.batteryShowTimer > 0f)
		{
			this.batteryShowTimer -= Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.originalLocalScale, Time.deltaTime * 30f);
			this.batteryImage.enabled = true;
		}
		else if (this.batteryImage.enabled)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			if (base.transform.localScale.x < 0.01f)
			{
				base.transform.localScale = Vector3.zero;
				this.batteryImage.enabled = false;
			}
		}
		if (this.batteryState == 0)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.921f, 0.4f, 0.2f);
		}
		if (this.batteryState == 1)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 2)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 3)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 4)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 5)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState == 6)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState > 6)
		{
			this.batteryState = 6;
		}
		if (this.batteryState < 0)
		{
			this.batteryState = 0;
		}
		if (this.batteryState <= 3 && SemiFunc.RunIsLobby())
		{
			this.redBlinkTimer += Time.deltaTime;
			if (this.redBlinkTimer > 0.5f)
			{
				this.batteryImage.color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				this.batteryImage.color = new Color(1f, 1f, 1f, 1f);
			}
			if (this.redBlinkTimer > 1f)
			{
				this.redBlinkTimer = 0f;
			}
		}
	}

	// Token: 0x04001F52 RID: 8018
	public int inventorySpot;

	// Token: 0x04001F53 RID: 8019
	private int batteryState;

	// Token: 0x04001F54 RID: 8020
	internal RawImage batteryImage;

	// Token: 0x04001F55 RID: 8021
	private float redBlinkTimer;

	// Token: 0x04001F56 RID: 8022
	private float batteryShowTimer;

	// Token: 0x04001F57 RID: 8023
	private Vector3 originalLocalScale;
}
