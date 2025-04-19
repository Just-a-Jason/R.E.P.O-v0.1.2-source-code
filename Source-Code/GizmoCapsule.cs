using System;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class GizmoCapsule : MonoBehaviour
{
	// Token: 0x06001191 RID: 4497 RVA: 0x0009C17C File Offset: 0x0009A37C
	private void OnDrawGizmos()
	{
		if (this.capsuleMesh == null)
		{
			this.capsuleMesh = this.CreateCapsuleMesh();
		}
		this.gizmoColor.a = 0.4f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Vector3 localScale = base.transform.localScale;
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		this.gizmoColor.a = 0.2f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawMesh(this.capsuleMesh, position, rotation, localScale);
		float num = Mathf.Min(localScale.x, localScale.z) * 0.5f;
		float num2 = localScale.y - num * 2f;
		this.gizmoColor.a = 0.4f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawWireSphere(position + rotation * Vector3.up * (num2 + num), num);
		Gizmos.DrawWireSphere(position + rotation * Vector3.down * (num2 + num), num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.right * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.right * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.left * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.left * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.forward * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.forward * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.back * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.back * num);
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0009C438 File Offset: 0x0009A638
	private Mesh CreateCapsuleMesh()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		Object.DestroyImmediate(gameObject);
		return sharedMesh;
	}

	// Token: 0x04001D7D RID: 7549
	public Color gizmoColor = Color.yellow;

	// Token: 0x04001D7E RID: 7550
	private Mesh capsuleMesh;

	// Token: 0x04001D7F RID: 7551
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;
}
