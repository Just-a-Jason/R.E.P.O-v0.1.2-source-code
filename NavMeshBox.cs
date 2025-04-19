using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class NavMeshBox : MonoBehaviour
{
	// Token: 0x06000810 RID: 2064 RVA: 0x0004E58C File Offset: 0x0004C78C
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		Gizmos.color = new Color(0.4f, 0.19f, 1f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = new Color(0.9f, 0.22f, 1f, 0.2f);
		Gizmos.DrawCube(component.center, component.size);
	}
}
