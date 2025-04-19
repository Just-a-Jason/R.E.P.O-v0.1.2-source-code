using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class ItemOrbBattery : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000BA5 RID: 2981 RVA: 0x00067EE3 File Offset: 0x000660E3
	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x00067EF0 File Offset: 0x000660F0
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00067F0C File Offset: 0x0006610C
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				physGrabObject.GetComponent<ItemBattery>().ChargeBattery(base.gameObject, SemiFunc.BatteryGetChargeRate(3));
			}
		}
	}

	// Token: 0x04001300 RID: 4864
	private ItemOrb itemOrb;

	// Token: 0x04001301 RID: 4865
	private PhysGrabObject physGrabObject;
}
