using System;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class ItemDroneTargetingCondition : MonoBehaviour, ITargetingCondition
{
	// Token: 0x06000A89 RID: 2697 RVA: 0x0005CE37 File Offset: 0x0005B037
	public bool CustomTargetingCondition(GameObject target)
	{
		target.GetComponent<PhysGrabObjectImpactDetector>();
		return target.CompareTag("Enemy");
	}
}
