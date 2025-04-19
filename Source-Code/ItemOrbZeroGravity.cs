using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class ItemOrbZeroGravity : MonoBehaviour
{
	// Token: 0x06000BB8 RID: 3000 RVA: 0x00068578 File Offset: 0x00066778
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0006859E File Offset: 0x0006679E
	private void BatteryDrain(float amount)
	{
		this.itemBattery.batteryLife -= amount * Time.deltaTime;
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x000685BC File Offset: 0x000667BC
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			PlayerController.instance.AntiGravity(0.1f);
			this.BatteryDrain(0.5f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				physGrabObject.OverrideDrag(0.5f, 0.1f);
				physGrabObject.OverrideAngularDrag(0.5f, 0.1f);
				physGrabObject.OverrideZeroGravity(0.1f);
				if (!physGrabObject.GetComponent<PlayerTumble>())
				{
					this.BatteryDrain(0.5f);
				}
			}
		}
	}

	// Token: 0x04001310 RID: 4880
	private ItemOrb itemOrb;

	// Token: 0x04001311 RID: 4881
	private PhysGrabObject physGrabObject;

	// Token: 0x04001312 RID: 4882
	private ItemBattery itemBattery;
}
