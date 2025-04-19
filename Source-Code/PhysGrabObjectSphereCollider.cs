using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class PhysGrabObjectSphereCollider : MonoBehaviour
{
	// Token: 0x0600141A RID: 5146 RVA: 0x000B0AC0 File Offset: 0x000AECC0
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f * this.gizmoTransparency);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
		Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f * this.gizmoTransparency);
		Gizmos.DrawSphere(Vector3.zero, 0.5f);
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x0400223E RID: 8766
	public bool drawGizmos = true;

	// Token: 0x0400223F RID: 8767
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;
}
