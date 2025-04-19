using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class Fireplace : MonoBehaviour
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x00054F18 File Offset: 0x00053118
	private void Awake()
	{
		if (this.isLit)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.fire, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			if (this.isCornerFireplace)
			{
				this.fireOffset = new Vector3(0.824f, 0.028f, 0.824f);
				gameObject.transform.localRotation *= Quaternion.Euler(0f, 45f, 0f);
			}
			gameObject.transform.localPosition = this.fireOffset;
		}
	}

	// Token: 0x04001031 RID: 4145
	public bool isLit;

	// Token: 0x04001032 RID: 4146
	public bool isCornerFireplace;

	// Token: 0x04001033 RID: 4147
	public GameObject fire;

	// Token: 0x04001034 RID: 4148
	private Vector3 fireOffset = new Vector3(0f, 0.028f, 0.249f);
}
