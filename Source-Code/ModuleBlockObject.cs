using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class ModuleBlockObject : MonoBehaviour
{
	// Token: 0x06000805 RID: 2053 RVA: 0x0004E339 File Offset: 0x0004C539
	private void Start()
	{
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}
}
