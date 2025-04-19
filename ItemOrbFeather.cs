using System;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class ItemOrbFeather : MonoBehaviour
{
	// Token: 0x06000BA9 RID: 2985 RVA: 0x00067FA8 File Offset: 0x000661A8
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x00067FC4 File Offset: 0x000661C4
	private void Update()
	{
		if (!this.itemOrb.itemActive)
		{
			return;
		}
		if (this.itemOrb.localPlayerAffected)
		{
			PlayerController.instance.Feather(0.1f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject physGrabObject in this.itemOrb.objectAffected)
		{
			if (physGrabObject && this.physGrabObject != physGrabObject)
			{
				PlayerTumble component = physGrabObject.GetComponent<PlayerTumble>();
				if (!component)
				{
					physGrabObject.OverrideMass(1f, 0.1f);
					physGrabObject.OverrideDrag(1f, 0.1f);
					physGrabObject.OverrideAngularDrag(5f, 0.1f);
				}
				else
				{
					component.DisableCustomGravity(0.1f);
					physGrabObject.OverrideMass(0.05f, 0.1f);
					if (component.playerAvatar.isLocal)
					{
						PlayerController.instance.Feather(0.1f);
					}
				}
			}
		}
	}

	// Token: 0x04001302 RID: 4866
	private ItemOrb itemOrb;

	// Token: 0x04001303 RID: 4867
	private PhysGrabObject physGrabObject;
}
