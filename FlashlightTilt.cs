using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class FlashlightTilt : MonoBehaviour
{
	// Token: 0x060006BA RID: 1722 RVA: 0x000409E0 File Offset: 0x0003EBE0
	private void Update()
	{
		Quaternion targetRotation = Quaternion.LookRotation(base.transform.parent.forward, Vector3.up);
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.spring, targetRotation, -1f);
	}

	// Token: 0x04000B69 RID: 2921
	public SpringQuaternion spring;
}
