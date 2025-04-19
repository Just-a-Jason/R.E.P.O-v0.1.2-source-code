using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class PlayerPhysObjectStander : MonoBehaviour
{
	// Token: 0x06000EFB RID: 3835 RVA: 0x00088C67 File Offset: 0x00086E67
	private void Awake()
	{
		this.Collider = base.GetComponent<SphereCollider>();
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x00088C78 File Offset: 0x00086E78
	private void Update()
	{
		if (this.checkTimer <= 0f)
		{
			this.physGrabObjects.Clear();
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.Collider.radius, this.layerMask);
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					PhysGrabObject physGrabObject = collider.gameObject.GetComponent<PhysGrabObject>();
					if (!physGrabObject)
					{
						physGrabObject = collider.gameObject.GetComponentInParent<PhysGrabObject>();
					}
					if (physGrabObject)
					{
						this.physGrabObjects.Add(physGrabObject);
					}
				}
			}
			this.checkTimer = 0.1f;
			return;
		}
		this.checkTimer -= 1f * Time.deltaTime;
	}

	// Token: 0x04001934 RID: 6452
	public LayerMask layerMask;

	// Token: 0x04001935 RID: 6453
	private SphereCollider Collider;

	// Token: 0x04001936 RID: 6454
	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04001937 RID: 6455
	private float checkTimer;
}
