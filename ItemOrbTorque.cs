using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ItemOrbTorque : MonoBehaviour
{
	// Token: 0x06000BB5 RID: 2997 RVA: 0x000683B4 File Offset: 0x000665B4
	private void Start()
	{
		this.itemOrb = base.GetComponent<ItemOrb>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x000683D0 File Offset: 0x000665D0
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
				float num = Vector3.Distance(new Vector3(physGrabObject.rb.position.x, 0f, physGrabObject.rb.position.z), new Vector3(base.transform.position.x, 0f, base.transform.position.z));
				Rigidbody rb = physGrabObject.rb;
				Vector3 normalized = (base.transform.position - rb.position).normalized;
				float num2 = 0.5f;
				if (num < num2)
				{
					float num3 = Mathf.Clamp(num - 0.2f, 0f, num2) / num2;
				}
				float num4 = 0.5f;
				num2 = 1f;
				if (num < num2)
				{
					float num5 = Mathf.Clamp(num - 0.5f, 0f, num2) / num2;
					num4 *= num5;
				}
				if (physGrabObject.isEnemy)
				{
					num4 *= 5f;
				}
				physGrabObject.OverrideFragility(0.1f);
				physGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
			}
		}
	}

	// Token: 0x0400130E RID: 4878
	private ItemOrb itemOrb;

	// Token: 0x0400130F RID: 4879
	private PhysGrabObject physGrabObject;
}
