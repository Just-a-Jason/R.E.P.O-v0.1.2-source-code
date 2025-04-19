using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class EnemyHeadChaseOffset : MonoBehaviour
{
	// Token: 0x060002F1 RID: 753 RVA: 0x0001D324 File Offset: 0x0001B524
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			if (this.Lerp <= 0f || this.Lerp >= 1f)
			{
				this.Active = true;
			}
		}
		else if (this.Lerp <= 0f || this.Lerp >= 1f)
		{
			this.Active = false;
		}
		if (this.Active)
		{
			this.Lerp += Time.deltaTime * this.IntroSpeed;
		}
		else
		{
			this.Lerp -= Time.deltaTime * this.OutroSpeed;
		}
		this.Lerp = Mathf.Clamp01(this.Lerp);
		if (this.Active)
		{
			base.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(this.Offset), this.IntroCurve.Evaluate(this.Lerp));
			return;
		}
		base.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(this.Offset), this.OutroCurve.Evaluate(this.Lerp));
	}

	// Token: 0x04000508 RID: 1288
	public Enemy Enemy;

	// Token: 0x04000509 RID: 1289
	[Space]
	public Vector3 Offset;

	// Token: 0x0400050A RID: 1290
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x0400050B RID: 1291
	public float IntroSpeed;

	// Token: 0x0400050C RID: 1292
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x0400050D RID: 1293
	public float OutroSpeed;

	// Token: 0x0400050E RID: 1294
	private float Lerp;

	// Token: 0x0400050F RID: 1295
	private bool Active;
}
