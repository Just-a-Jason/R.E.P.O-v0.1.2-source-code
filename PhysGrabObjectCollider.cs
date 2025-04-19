using System;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class PhysGrabObjectCollider : MonoBehaviour
{
	// Token: 0x06001415 RID: 5141 RVA: 0x000B09A3 File Offset: 0x000AEBA3
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x000B09B1 File Offset: 0x000AEBB1
	private void OnDestroy()
	{
		if (this.physGrabObject)
		{
			this.physGrabObject.colliders.Remove(base.transform);
		}
	}

	// Token: 0x0400223A RID: 8762
	[HideInInspector]
	public int colliderID;

	// Token: 0x0400223B RID: 8763
	private PhysGrabObject physGrabObject;
}
