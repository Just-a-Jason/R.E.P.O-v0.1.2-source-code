using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class EnemyHeadEyeTilt : MonoBehaviour
{
	// Token: 0x06000304 RID: 772 RVA: 0x0001DEE4 File Offset: 0x0001C0E4
	private void Update()
	{
		base.transform.localRotation = this.Follow.localRotation;
	}

	// Token: 0x04000537 RID: 1335
	public Transform Follow;
}
