using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class PowerCrystal : MonoBehaviour
{
	// Token: 0x0600084B RID: 2123 RVA: 0x0005002E File Offset: 0x0004E22E
	private void Start()
	{
		ItemManager.instance.powerCrystals.Add(base.GetComponent<PhysGrabObject>());
	}
}
