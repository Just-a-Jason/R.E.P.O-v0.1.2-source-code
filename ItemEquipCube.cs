using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class ItemEquipCube : MonoBehaviour
{
	// Token: 0x06000B32 RID: 2866 RVA: 0x00063C99 File Offset: 0x00061E99
	private void OnTriggerStay(Collider other)
	{
		this.isObstructed = true;
		this.obstructedTimer = 0.1f;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x00063CAD File Offset: 0x00061EAD
	private void Update()
	{
		if (this.obstructedTimer > 0f)
		{
			this.obstructedTimer -= Time.deltaTime;
			return;
		}
		this.isObstructed = false;
	}

	// Token: 0x0400123A RID: 4666
	[HideInInspector]
	public bool isObstructed;

	// Token: 0x0400123B RID: 4667
	private float obstructedTimer;
}
