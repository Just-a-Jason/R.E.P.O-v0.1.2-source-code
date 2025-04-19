using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class EnemyHeadTilt : MonoBehaviour
{
	// Token: 0x06000311 RID: 785 RVA: 0x0001E3C8 File Offset: 0x0001C5C8
	private void Update()
	{
		float z = Mathf.Clamp(Vector3.Cross(this.ForwardPrev, base.transform.forward).y * this.Amount, -this.MaxAmount, this.MaxAmount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, z), this.Speed * Time.deltaTime);
		this.ForwardPrev = base.transform.forward;
	}

	// Token: 0x04000559 RID: 1369
	public float Amount = -500f;

	// Token: 0x0400055A RID: 1370
	public float MaxAmount = 20f;

	// Token: 0x0400055B RID: 1371
	public float Speed = 10f;

	// Token: 0x0400055C RID: 1372
	private Vector3 ForwardPrev;
}
