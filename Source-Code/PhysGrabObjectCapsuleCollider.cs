using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class PhysGrabObjectCapsuleCollider : MonoBehaviour
{
	// Token: 0x06001412 RID: 5138 RVA: 0x000B0694 File Offset: 0x000AE894
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		if (this.capsuleMesh == null)
		{
			this.capsuleMesh = this.CreateCapsuleMesh();
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.4f * this.gizmoTransparency);
		Vector3 localScale = base.transform.localScale;
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f * this.gizmoTransparency);
		Gizmos.DrawMesh(this.capsuleMesh, position, rotation, localScale);
		float num = Mathf.Min(localScale.x, localScale.z) * 0.5f;
		float num2 = localScale.y - num * 2f;
		Gizmos.color = new Color(0f, 1f, 0f, 0.4f * this.gizmoTransparency);
		Gizmos.DrawWireSphere(position + rotation * Vector3.up * (num2 + num), num);
		Gizmos.DrawWireSphere(position + rotation * Vector3.down * (num2 + num), num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.right * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.right * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.left * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.left * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.forward * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.forward * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.back * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.back * num);
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x000B0964 File Offset: 0x000AEB64
	private Mesh CreateCapsuleMesh()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		Object.DestroyImmediate(gameObject);
		return sharedMesh;
	}

	// Token: 0x04002237 RID: 8759
	private Mesh capsuleMesh;

	// Token: 0x04002238 RID: 8760
	public bool drawGizmos = true;

	// Token: 0x04002239 RID: 8761
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;
}
