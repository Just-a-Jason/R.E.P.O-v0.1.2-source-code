using System;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class WiggleRotation : MonoBehaviour
{
	// Token: 0x060006FB RID: 1787 RVA: 0x00041CF0 File Offset: 0x0003FEF0
	private void LateUpdate()
	{
		float num = this.wiggleOffsetPercentage / 100f * 2f * 3.1415927f;
		Quaternion localRotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * this.wiggleFrequency + num) * this.maxRotation * this.wiggleMultiplier, this.wiggleAxis);
		base.transform.localRotation = localRotation;
	}

	// Token: 0x04000BD2 RID: 3026
	public Vector3 wiggleAxis = Vector3.up;

	// Token: 0x04000BD3 RID: 3027
	public float maxRotation = 10f;

	// Token: 0x04000BD4 RID: 3028
	public float wiggleFrequency = 1f;

	// Token: 0x04000BD5 RID: 3029
	[Range(0f, 100f)]
	public float wiggleOffsetPercentage;

	// Token: 0x04000BD6 RID: 3030
	public float wiggleMultiplier = 1f;
}
