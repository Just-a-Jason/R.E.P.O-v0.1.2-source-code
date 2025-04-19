using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class ItemDroneZeroGravity : MonoBehaviour
{
	// Token: 0x06000AA3 RID: 2723 RVA: 0x0005DFBA File Offset: 0x0005C1BA
	private void Start()
	{
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0005DFEC File Offset: 0x0005C1EC
	private void FixedUpdate()
	{
		if (this.itemDrone.magnetActive && this.itemDrone.magnetTargetPhysGrabObject)
		{
			if (this.itemDrone.playerTumbleTarget)
			{
				this.itemBattery.batteryLife -= 2f * Time.fixedDeltaTime;
				this.itemDrone.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
			}
			EnemyParent componentInParent = this.itemDrone.magnetTargetPhysGrabObject.GetComponentInParent<EnemyParent>();
			if (componentInParent)
			{
				SemiFunc.ItemAffectEnemyBatteryDrain(componentInParent, this.itemBattery, this.tumbleEnemyTimer, Time.fixedDeltaTime, 1f);
				this.tumbleEnemyTimer += Time.fixedDeltaTime;
			}
		}
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0005E0B0 File Offset: 0x0005C2B0
	private void Update()
	{
		if (!this.itemDrone.itemActivated)
		{
			this.tumbleEnemyTimer = 0f;
		}
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated && this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget && this.itemDrone.targetIsLocalPlayer)
		{
			this.itemBattery.batteryLife -= 2f * Time.deltaTime;
			PlayerController.instance.AntiGravity(0.1f);
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
				this.itemDrone.magnetTargetPhysGrabObject.OverrideDrag(0.1f, 0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideAngularDrag(0.1f, 0.1f);
				this.itemDrone.magnetTargetPhysGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x0400113B RID: 4411
	private ItemDrone itemDrone;

	// Token: 0x0400113C RID: 4412
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x0400113D RID: 4413
	private ItemEquippable itemEquippable;

	// Token: 0x0400113E RID: 4414
	private float tumbleEnemyTimer;

	// Token: 0x0400113F RID: 4415
	private ItemBattery itemBattery;
}
