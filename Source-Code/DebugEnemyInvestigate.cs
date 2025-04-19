using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class DebugEnemyInvestigate : MonoBehaviour
{
	// Token: 0x060000D6 RID: 214 RVA: 0x00007EEC File Offset: 0x000060EC
	private void Update()
	{
		this.lerp += Time.deltaTime;
		this.radiusCurrent = Mathf.Lerp(0f, this.radius, this.animationCurve.Evaluate(this.lerp));
		if (this.lerp >= 1f)
		{
			this.alpha -= Time.deltaTime;
			if (this.alpha <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00007F6C File Offset: 0x0000616C
	private void OnDrawGizmos()
	{
		base.transform.eulerAngles = Vector3.zero;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 1f, this.alpha);
		Gizmos.DrawWireSphere(Vector3.zero, 0.1f);
		Gizmos.color = new Color(1f, 0.62f, 0f, 0.23f * this.alpha);
		Gizmos.DrawSphere(Vector3.zero, this.radiusCurrent);
		Gizmos.color = new Color(1f, 0.62f, 0f, this.alpha);
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(0f, 45f, 0f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
	}

	// Token: 0x0400021D RID: 541
	public AnimationCurve animationCurve;

	// Token: 0x0400021E RID: 542
	private float lerp;

	// Token: 0x0400021F RID: 543
	private float alpha = 1f;

	// Token: 0x04000220 RID: 544
	internal float radius = 1f;

	// Token: 0x04000221 RID: 545
	private float radiusCurrent = 2f;
}
