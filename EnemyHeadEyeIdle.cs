using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class EnemyHeadEyeIdle : MonoBehaviour
{
	// Token: 0x060002FE RID: 766 RVA: 0x0001DC90 File Offset: 0x0001BE90
	private void Update()
	{
		if (this.EyeTarget.Idle)
		{
			if (this.Timer <= 0f)
			{
				this.Timer = Random.Range(this.TimeMin, this.TimeMax);
				this.CurrentX = Random.Range(this.MinX, this.MaxX);
				this.CurrentY = Random.Range(this.MinY, this.MaxY);
			}
			else
			{
				this.Timer -= Time.deltaTime;
			}
		}
		else
		{
			this.CurrentX = 0f;
			this.CurrentY = 0f;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(this.CurrentX, this.CurrentY, base.transform.localPosition.z), this.Speed * Time.deltaTime);
	}

	// Token: 0x0400051F RID: 1311
	public EnemyHeadEyeTarget EyeTarget;

	// Token: 0x04000520 RID: 1312
	public float Speed;

	// Token: 0x04000521 RID: 1313
	[Space]
	public float TimeMin;

	// Token: 0x04000522 RID: 1314
	public float TimeMax;

	// Token: 0x04000523 RID: 1315
	private float Timer;

	// Token: 0x04000524 RID: 1316
	[Space]
	public float MinX;

	// Token: 0x04000525 RID: 1317
	public float MaxX;

	// Token: 0x04000526 RID: 1318
	private float CurrentX;

	// Token: 0x04000527 RID: 1319
	[Space]
	public float MinY;

	// Token: 0x04000528 RID: 1320
	public float MaxY;

	// Token: 0x04000529 RID: 1321
	private float CurrentY;
}
