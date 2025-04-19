using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class EnemyHeadEyeTremble : MonoBehaviour
{
	// Token: 0x06000306 RID: 774 RVA: 0x0001DF04 File Offset: 0x0001C104
	private void Update()
	{
		if (this.TimerX <= 0f)
		{
			this.TimerX = Random.Range(this.TimeMin, this.TimeMax);
			this.TargetX = Random.Range(this.Min, this.Max);
		}
		else
		{
			this.TimerX -= Time.deltaTime;
		}
		this.CurrentX = Mathf.Lerp(this.CurrentX, this.TargetX, this.Speed * Time.deltaTime);
		if (this.TimerY <= 0f)
		{
			this.TimerY = Random.Range(this.TimeMin, this.TimeMax);
			this.TargetY = Random.Range(this.Min, this.Max);
		}
		else
		{
			this.TimerY -= Time.deltaTime;
		}
		this.CurrentY = Mathf.Lerp(this.CurrentY, this.TargetY, this.Speed * Time.deltaTime);
		base.transform.localRotation = Quaternion.Euler(this.CurrentX, this.CurrentY, 0f);
	}

	// Token: 0x04000538 RID: 1336
	public float Speed;

	// Token: 0x04000539 RID: 1337
	[Space]
	public float TimeMin;

	// Token: 0x0400053A RID: 1338
	public float TimeMax;

	// Token: 0x0400053B RID: 1339
	private float TimerX;

	// Token: 0x0400053C RID: 1340
	private float TimerY;

	// Token: 0x0400053D RID: 1341
	[Space]
	public float Min;

	// Token: 0x0400053E RID: 1342
	public float Max;

	// Token: 0x0400053F RID: 1343
	private float TargetX;

	// Token: 0x04000540 RID: 1344
	private float CurrentX;

	// Token: 0x04000541 RID: 1345
	private float TargetY;

	// Token: 0x04000542 RID: 1346
	private float CurrentY;
}
