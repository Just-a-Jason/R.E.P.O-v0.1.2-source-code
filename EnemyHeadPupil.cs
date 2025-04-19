using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class EnemyHeadPupil : MonoBehaviour
{
	// Token: 0x0600030F RID: 783 RVA: 0x0001E379 File Offset: 0x0001C579
	private void Update()
	{
		if (this.Active)
		{
			base.transform.localScale = new Vector3(this.EyeTarget.PupilCurrentSize, base.transform.localScale.y, this.EyeTarget.PupilCurrentSize);
		}
	}

	// Token: 0x04000557 RID: 1367
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x04000558 RID: 1368
	public bool Active = true;
}
