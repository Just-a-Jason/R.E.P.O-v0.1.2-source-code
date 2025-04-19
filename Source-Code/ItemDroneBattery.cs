using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class ItemDroneBattery : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000A8F RID: 2703 RVA: 0x0005D357 File Offset: 0x0005B557
	private void Start()
	{
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0005D389 File Offset: 0x0005B589
	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0005D398 File Offset: 0x0005B598
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			this.myPhysGrabObject.OverrideZeroGravity(0.1f);
			this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
			this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			if (this.itemDrone.magnetActive && this.itemDrone.magnetTargetPhysGrabObject)
			{
				ItemBattery component = this.itemDrone.magnetTargetPhysGrabObject.GetComponent<ItemBattery>();
				if (component)
				{
					component.ChargeBattery(base.gameObject, 5f);
					this.itemBattery.Drain(5f);
				}
				if (component.batteryLife >= 99f && !component.batteryActive && component.autoDrain)
				{
					this.itemDrone.MagnetActiveToggle(false);
				}
			}
		}
	}

	// Token: 0x04001122 RID: 4386
	private ItemDrone itemDrone;

	// Token: 0x04001123 RID: 4387
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001124 RID: 4388
	private ItemEquippable itemEquippable;

	// Token: 0x04001125 RID: 4389
	private ItemBattery itemBattery;
}
