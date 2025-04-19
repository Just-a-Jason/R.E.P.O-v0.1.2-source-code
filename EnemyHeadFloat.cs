using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class EnemyHeadFloat : MonoBehaviour
{
	// Token: 0x06000308 RID: 776 RVA: 0x0001E01E File Offset: 0x0001C21E
	public void Disable(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0001E028 File Offset: 0x0001C228
	private void Update()
	{
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= Time.deltaTime;
			return;
		}
		if (!this.ReversePos)
		{
			this.LerpPos += this.SpeedPos * Time.deltaTime;
			if (this.LerpPos >= 1f)
			{
				this.ReversePos = true;
				this.LerpPos = 1f;
			}
		}
		else
		{
			this.LerpPos -= this.SpeedPos * Time.deltaTime;
			if (this.LerpPos <= 0f)
			{
				this.ReversePos = false;
				this.LerpPos = 0f;
			}
		}
		base.transform.localPosition = new Vector3(0f, this.CurvePos.Evaluate(this.LerpPos) * this.AmountPos, 0f);
		this.LerpRot += this.SpeedRot * Time.deltaTime;
		if (this.LerpRot >= 1f)
		{
			this.LerpRot = 0f;
		}
		base.transform.localRotation = Quaternion.Euler(this.CurveRot.Evaluate(this.LerpRot) * this.AmountRot, 0f, 0f);
	}

	// Token: 0x04000543 RID: 1347
	public AnimationCurve CurvePos;

	// Token: 0x04000544 RID: 1348
	public float SpeedPos;

	// Token: 0x04000545 RID: 1349
	public float AmountPos;

	// Token: 0x04000546 RID: 1350
	private float LerpPos;

	// Token: 0x04000547 RID: 1351
	private bool ReversePos;

	// Token: 0x04000548 RID: 1352
	[Space]
	public AnimationCurve CurveRot;

	// Token: 0x04000549 RID: 1353
	public float SpeedRot;

	// Token: 0x0400054A RID: 1354
	public float AmountRot;

	// Token: 0x0400054B RID: 1355
	private float LerpRot;

	// Token: 0x0400054C RID: 1356
	private float DisableTimer;
}
