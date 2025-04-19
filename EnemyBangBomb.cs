using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class EnemyBangBomb : MonoBehaviour
{
	// Token: 0x06000137 RID: 311 RVA: 0x0000C045 File Offset: 0x0000A245
	private void Update()
	{
		this.source.rotation = SemiFunc.SpringQuaternionGet(this.spring, this.target.rotation, -1f);
	}

	// Token: 0x040002A6 RID: 678
	public SpringQuaternion spring;

	// Token: 0x040002A7 RID: 679
	public Transform source;

	// Token: 0x040002A8 RID: 680
	public Transform target;
}
