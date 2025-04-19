using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class ItemOrbHeal : MonoBehaviour
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x000680E8 File Offset: 0x000662E8
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemBattery = base.GetComponent<ItemBattery>();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00068110 File Offset: 0x00066310
	private void Update()
	{
		if (!this.itemOrb.itemActive || this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			if (this.healTimer > this.healRate)
			{
				PlayerController.instance.playerAvatarScript.playerHealth.Heal(this.healAmount, true);
				this.healTimer = 0f;
			}
			this.healTimer += Time.deltaTime;
		}
	}

	// Token: 0x04001304 RID: 4868
	private ItemOrb itemOrb;

	// Token: 0x04001305 RID: 4869
	private ItemBattery itemBattery;

	// Token: 0x04001306 RID: 4870
	private PhysGrabObject physGrabObject;

	// Token: 0x04001307 RID: 4871
	private float healRate = 2f;

	// Token: 0x04001308 RID: 4872
	private float healTimer;

	// Token: 0x04001309 RID: 4873
	private int healAmount = 10;
}
