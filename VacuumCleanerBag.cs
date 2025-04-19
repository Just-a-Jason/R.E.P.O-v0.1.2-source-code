using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class VacuumCleanerBag : MonoBehaviour
{
	// Token: 0x06000746 RID: 1862 RVA: 0x00045358 File Offset: 0x00043558
	private void Update()
	{
		if (this.ActiveX)
		{
			this.LerpX += this.SpeedX * Time.deltaTime;
			if (this.LerpX >= 1f)
			{
				if (!this.Active)
				{
					this.ActiveX = false;
				}
				this.LerpX = 0f;
			}
		}
		else if (this.Active)
		{
			this.ActiveX = true;
		}
		if (this.ActiveZ)
		{
			this.LerpZ += this.SpeedZ * Time.deltaTime;
			if (this.LerpZ >= 1f)
			{
				if (!this.Active)
				{
					this.ActiveZ = false;
				}
				this.LerpZ = 0f;
			}
		}
		else if (this.Active)
		{
			this.ActiveZ = true;
		}
		if (this.ActiveX || this.ActiveZ)
		{
			float num = Mathf.Lerp(0f, this.AmountX, this.AnimationCurve.Evaluate(this.LerpX));
			float num2 = Mathf.Lerp(0f, this.AmountZ, this.AnimationCurve.Evaluate(this.LerpZ));
			base.transform.localScale = new Vector3(1f + num, 1f, 1f + num2);
		}
	}

	// Token: 0x04000CC3 RID: 3267
	[HideInInspector]
	public bool Active;

	// Token: 0x04000CC4 RID: 3268
	[Space]
	public float AmountX;

	// Token: 0x04000CC5 RID: 3269
	public float SpeedX;

	// Token: 0x04000CC6 RID: 3270
	private float LerpX;

	// Token: 0x04000CC7 RID: 3271
	private bool ActiveX;

	// Token: 0x04000CC8 RID: 3272
	[Space]
	public float AmountZ;

	// Token: 0x04000CC9 RID: 3273
	public float SpeedZ;

	// Token: 0x04000CCA RID: 3274
	private float LerpZ;

	// Token: 0x04000CCB RID: 3275
	private bool ActiveZ;

	// Token: 0x04000CCC RID: 3276
	[Space]
	public AnimationCurve AnimationCurve;
}
