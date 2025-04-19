using System;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class PhysGrabObjectMeshCollider : MonoBehaviour
{
	// Token: 0x06001418 RID: 5144 RVA: 0x000B09E0 File Offset: 0x000AEBE0
	private void OnDrawGizmos()
	{
		if (!this.showGizmo)
		{
			return;
		}
		Mesh sharedMesh = base.GetComponent<MeshCollider>().sharedMesh;
		if (sharedMesh != null)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f * this.gizmoAlpha);
			Gizmos.DrawMesh(sharedMesh, base.transform.position, base.transform.rotation, base.transform.localScale);
			Gizmos.color = new Color(0f, 1f, 0f, 0.4f * this.gizmoAlpha);
			Gizmos.DrawWireMesh(sharedMesh, base.transform.position, base.transform.rotation, base.transform.localScale);
		}
	}

	// Token: 0x0400223C RID: 8764
	public bool showGizmo = true;

	// Token: 0x0400223D RID: 8765
	[Range(0.2f, 1f)]
	public float gizmoAlpha = 1f;
}
