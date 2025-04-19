using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class ItemOrbMagnet : MonoBehaviour
{
	// Token: 0x06000BB2 RID: 2994 RVA: 0x00068258 File Offset: 0x00066458
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00068274 File Offset: 0x00066474
	private void FixedUpdate()
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
				Vector3 normalized = (this.physGrabObject.transform.position - physGrabObject.transform.position).normalized;
				if ((this.physGrabObject.transform.position - physGrabObject.transform.position).magnitude > 0.45f)
				{
					physGrabObject.rb.AddForce(normalized * Mathf.Clamp(physGrabObject.rb.mass * 10f, 0.2f, 5f));
				}
				physGrabObject.rb.velocity = this.physGrabObject.rb.velocity;
				physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x0400130C RID: 4876
	private ItemOrb itemOrb;

	// Token: 0x0400130D RID: 4877
	private PhysGrabObject physGrabObject;
}
