using System;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class MapLayer : MonoBehaviour
{
	// Token: 0x06000CEA RID: 3306 RVA: 0x000713A2 File Offset: 0x0006F5A2
	private void Start()
	{
		this.positionStart = base.transform.position;
	}

	// Token: 0x040014A9 RID: 5289
	public int layer;

	// Token: 0x040014AA RID: 5290
	internal Vector3 positionStart;
}
