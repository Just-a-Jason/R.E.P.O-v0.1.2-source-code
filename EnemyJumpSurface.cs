using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class EnemyJumpSurface : MonoBehaviour
{
	// Token: 0x06000606 RID: 1542 RVA: 0x0003AB10 File Offset: 0x00038D10
	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = position + base.transform.TransformDirection(this.jumpDirection.normalized * 0.3f);
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawWireSphere(vector, 0.03f);
	}

	// Token: 0x04000A00 RID: 2560
	public Vector3 jumpDirection;
}
