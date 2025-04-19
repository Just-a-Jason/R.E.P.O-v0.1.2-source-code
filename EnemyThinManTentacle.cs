using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class EnemyThinManTentacle : MonoBehaviour
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x00030678 File Offset: 0x0002E878
	private void Update()
	{
		if (this.rotationLerp >= 1f)
		{
			this.rotationStartPos = base.transform.localRotation;
			this.rotationEndPos = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
			this.rotationLerpSpeed = Random.Range(1f, 2f);
			this.rotationLerp = 0f;
		}
		else
		{
			this.rotationLerp += Time.deltaTime * this.rotationLerpSpeed;
			base.transform.localRotation = Quaternion.Lerp(this.rotationStartPos, this.rotationEndPos, this.wiggleCurve.Evaluate(this.rotationLerp));
		}
		if (this.scaleLerp >= 1f)
		{
			this.scaleStartPos = base.transform.localScale;
			this.scaleEndPos = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
			this.scaleLerpSpeed = Random.Range(2f, 4f);
			this.scaleLerp = 0f;
			return;
		}
		this.scaleLerp += Time.deltaTime * this.scaleLerpSpeed;
		base.transform.localScale = Vector3.Lerp(this.scaleStartPos, this.scaleEndPos, this.wiggleCurve.Evaluate(this.scaleLerp));
	}

	// Token: 0x040007E8 RID: 2024
	public AnimationCurve wiggleCurve;

	// Token: 0x040007E9 RID: 2025
	private float rotationLerp = 1f;

	// Token: 0x040007EA RID: 2026
	private float rotationLerpSpeed;

	// Token: 0x040007EB RID: 2027
	private Quaternion rotationStartPos;

	// Token: 0x040007EC RID: 2028
	private Quaternion rotationEndPos;

	// Token: 0x040007ED RID: 2029
	private float scaleLerp = 1f;

	// Token: 0x040007EE RID: 2030
	private float scaleLerpSpeed;

	// Token: 0x040007EF RID: 2031
	private Vector3 scaleStartPos;

	// Token: 0x040007F0 RID: 2032
	private Vector3 scaleEndPos;
}
