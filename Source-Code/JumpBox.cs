using System;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class JumpBox : MonoBehaviour
{
	// Token: 0x060007DB RID: 2011 RVA: 0x0004CDC8 File Offset: 0x0004AFC8
	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (!component)
		{
			return;
		}
		Gizmos.color = new Color(0f, 0.66f, 1f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = new Color(0f, 0.44f, 1f, 0.2f);
		Gizmos.DrawCube(component.center, component.size);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 zero = Vector3.zero;
		Vector3 center = component.bounds.center;
		Vector3 vector = center + base.transform.forward * 0.5f;
		Gizmos.DrawLine(center, vector);
		Gizmos.DrawLine(vector, vector + Vector3.LerpUnclamped(-base.transform.forward, -base.transform.right, 0.5f) * 0.25f);
		Gizmos.DrawLine(vector, vector + Vector3.LerpUnclamped(-base.transform.forward, base.transform.right, 0.5f) * 0.25f);
	}
}
