using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class GizmoBall : MonoBehaviour
{
	// Token: 0x06001194 RID: 4500 RVA: 0x0009C47C File Offset: 0x0009A67C
	private void OnDrawGizmos()
	{
		this.color.a = 0.5f;
		Gizmos.color = this.color;
		Gizmos.DrawSphere(base.transform.position + this.offset, this.radius);
		this.color.a = 1f;
		Gizmos.color = this.color;
		Gizmos.DrawWireSphere(base.transform.position + this.offset, this.radius);
	}

	// Token: 0x04001D80 RID: 7552
	public Color color = Color.red;

	// Token: 0x04001D81 RID: 7553
	public float radius = 0.5f;

	// Token: 0x04001D82 RID: 7554
	public Vector3 offset;
}
