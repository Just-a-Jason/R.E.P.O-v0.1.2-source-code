using System;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class StartRoom : MonoBehaviour
{
	// Token: 0x0600080E RID: 2062 RVA: 0x0004E565 File Offset: 0x0004C765
	private void Start()
	{
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}
}
