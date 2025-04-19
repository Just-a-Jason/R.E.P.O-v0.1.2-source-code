using System;
using UnityEngine;
using UnityEngine.Animations;

// Token: 0x02000130 RID: 304
public class SetPositionConstraint : MonoBehaviour
{
	// Token: 0x06000A7B RID: 2683 RVA: 0x0005CCE0 File Offset: 0x0005AEE0
	private void Start()
	{
		base.GetComponent<PositionConstraint>().constraintActive = true;
	}
}
