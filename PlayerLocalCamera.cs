using System;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class PlayerLocalCamera : MonoBehaviour
{
	// Token: 0x06000EF7 RID: 3831 RVA: 0x00088A78 File Offset: 0x00086C78
	private void OnDrawGizmos()
	{
		if (this.debug)
		{
			Gizmos.color = new Color(1f, 0f, 0.79f, 0.5f);
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawSphere(Vector3.zero, 0.1f);
			Gizmos.DrawCube(new Vector3(0f, 0f, 0.15f), new Vector3(0.1f, 0.1f, 0.3f));
		}
	}

	// Token: 0x04001932 RID: 6450
	public bool debug;
}
