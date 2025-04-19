using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class DrawGizmoCube : MonoBehaviour
{
	// Token: 0x06001143 RID: 4419 RVA: 0x0009A16B File Offset: 0x0009836B
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.2f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0009A1AA File Offset: 0x000983AA
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
}
