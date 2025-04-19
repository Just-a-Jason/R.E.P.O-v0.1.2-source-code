using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class ItemDroneHeal : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000A97 RID: 2711 RVA: 0x0005D68A File Offset: 0x0005B88A
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemDrone = base.GetComponent<ItemDrone>();
		this.myPhysGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0005D6B0 File Offset: 0x0005B8B0
	public bool CustomTargetingCondition(GameObject target)
	{
		PlayerAvatar component = target.GetComponent<PlayerAvatar>();
		return component.playerHealth.health < component.playerHealth.maxHealth;
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0005D6DC File Offset: 0x0005B8DC
	private void Update()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		if (this.itemDrone.itemActivated)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.myPhysGrabObject.OverrideZeroGravity(0.1f);
				this.myPhysGrabObject.OverrideDrag(1f, 0.1f);
				this.myPhysGrabObject.OverrideAngularDrag(10f, 0.1f);
			}
			if (this.itemDrone.magnetActive && this.itemDrone.playerAvatarTarget)
			{
				this.healTimer += Time.deltaTime;
				if (this.healTimer > this.healRate)
				{
					this.itemDrone.playerAvatarTarget.playerHealth.Heal(this.healAmount, true);
					if (this.itemDrone.playerAvatarTarget.playerHealth.health >= this.itemDrone.playerAvatarTarget.playerHealth.maxHealth)
					{
						this.itemDrone.MagnetActiveToggle(false);
					}
					this.healTimer = 0f;
				}
			}
		}
	}

	// Token: 0x0400112A RID: 4394
	private ItemDrone itemDrone;

	// Token: 0x0400112B RID: 4395
	private PhysGrabObject myPhysGrabObject;

	// Token: 0x0400112C RID: 4396
	private float healRate = 2f;

	// Token: 0x0400112D RID: 4397
	private float healTimer;

	// Token: 0x0400112E RID: 4398
	private int healAmount = 10;

	// Token: 0x0400112F RID: 4399
	private ItemEquippable itemEquippable;
}
