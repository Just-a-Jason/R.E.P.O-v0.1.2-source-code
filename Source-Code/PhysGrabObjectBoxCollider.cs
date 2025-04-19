using System;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class PhysGrabObjectBoxCollider : MonoBehaviour
{
	// Token: 0x06000DC0 RID: 3520 RVA: 0x0007CD54 File Offset: 0x0007AF54
	private void Start()
	{
		if (this.unEquipCollider)
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			if (component)
			{
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0007CD80 File Offset: 0x0007AF80
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (component == null)
		{
			return;
		}
		Color color = new Color(0f, 1f, 0f, 1f * this.gizmoTransparency);
		Color color2 = new Color(0f, 1f, 0f, 0.2f * this.gizmoTransparency);
		if (this.unEquipCollider)
		{
			color = (color2 = new Color(0f, 0.5f, 0f, 1f * this.gizmoTransparency));
			color2.a = 0.2f * this.gizmoTransparency;
		}
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = color2;
		Gizmos.DrawCube(component.center, component.size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x040016AA RID: 5802
	public bool drawGizmos = true;

	// Token: 0x040016AB RID: 5803
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;

	// Token: 0x040016AC RID: 5804
	public bool unEquipCollider;
}
