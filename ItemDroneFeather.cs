using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class ItemDroneFeather : MonoBehaviour
{
	// Token: 0x06000A93 RID: 2707 RVA: 0x0005D48F File Offset: 0x0005B68F
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0005D4C1 File Offset: 0x0005B6C1
	private void BatteryDrain(float amount)
	{
		this.itemBattery.batteryLife -= amount * Time.fixedDeltaTime;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0005D4DC File Offset: 0x0005B6DC
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated && this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget && this.itemDrone.targetIsLocalPlayer)
		{
			PlayerController.instance.Feather(0.1f);
			this.BatteryDrain(2f);
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
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
				PlayerTumble component = this.itemDrone.magnetTargetPhysGrabObject.GetComponent<PlayerTumble>();
				if (!component)
				{
					this.itemDrone.magnetTargetPhysGrabObject.OverrideMass(1f, 0.1f);
					this.itemDrone.magnetTargetPhysGrabObject.OverrideDrag(1f, 0.1f);
					this.itemDrone.magnetTargetPhysGrabObject.OverrideAngularDrag(5f, 0.1f);
					return;
				}
				component.DisableCustomGravity(0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideMass(0.5f, 0.1f);
				this.BatteryDrain(2f);
				if (component.playerAvatar.isLocal)
				{
					PlayerController.instance.Feather(0.1f);
				}
			}
		}
	}

	// Token: 0x04001126 RID: 4390
	private ItemDrone itemDrone;

	// Token: 0x04001127 RID: 4391
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x04001128 RID: 4392
	private ItemEquippable itemEquippable;

	// Token: 0x04001129 RID: 4393
	private ItemBattery itemBattery;
}
