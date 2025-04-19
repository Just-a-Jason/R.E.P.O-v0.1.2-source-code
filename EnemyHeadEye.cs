using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class EnemyHeadEye : MonoBehaviour
{
	// Token: 0x060002FC RID: 764 RVA: 0x0001DC04 File Offset: 0x0001BE04
	private void Update()
	{
		Quaternion b = Quaternion.LookRotation(this.Target.position - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, this.EyeTarget.Speed * Time.deltaTime);
		base.transform.localRotation = SemiFunc.ClampRotation(base.transform.localRotation, this.EyeTarget.Limit);
	}

	// Token: 0x0400051B RID: 1307
	public Transform Target;

	// Token: 0x0400051C RID: 1308
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x0400051D RID: 1309
	private float CurrentX;

	// Token: 0x0400051E RID: 1310
	private float CurrentY;
}
