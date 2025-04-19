using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class InvisibleWall : MonoBehaviour
{
	// Token: 0x060007D9 RID: 2009 RVA: 0x0004CD40 File Offset: 0x0004AF40
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		Gizmos.color = new Color(0.1f, 1f, 0.4f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = new Color(0.1f, 1f, 0.4f, 0.5f);
		Gizmos.DrawCube(component.center, component.size);
	}
}
