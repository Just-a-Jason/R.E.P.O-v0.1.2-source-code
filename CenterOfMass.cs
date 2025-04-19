using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class CenterOfMass : MonoBehaviour
{
	// Token: 0x06001410 RID: 5136 RVA: 0x000B0669 File Offset: 0x000AE869
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.1f);
	}
}
