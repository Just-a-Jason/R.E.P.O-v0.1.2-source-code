using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class EnemySlowMouthHiveAttack : MonoBehaviour
{
	// Token: 0x0600040E RID: 1038 RVA: 0x00028465 File Offset: 0x00026665
	private void Start()
	{
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00028468 File Offset: 0x00026668
	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = this.hitPositionTransform.position;
		if (this.curveProgress < 1f)
		{
			this.curveProgress += Time.deltaTime * 2f;
			Vector3 position3 = Vector3.Lerp(position, position2, this.curveProgress);
			this.blobTransform.position = position3;
			float d = this.flyUpCurve.Evaluate(this.curveProgress);
			this.blobTransform.position += Vector3.up * 2f * d;
			if (Vector3.Distance(this.prevCheckPosition, this.blobTransform.position) > 0.5f)
			{
				Collider[] array = Physics.OverlapSphere(this.blobTransform.position, this.blobMeshTransform.localScale.x / 2f, SemiFunc.LayerMaskGetShouldHits());
				for (int i = 0; i < array.Length; i++)
				{
					EnemyParent componentInParent = array[i].GetComponentInParent<EnemyParent>();
					if (!componentInParent || !(componentInParent == this.enemyParent))
					{
						this.Splat();
						break;
					}
				}
				this.prevCheckPosition = this.blobTransform.position;
				return;
			}
		}
		else
		{
			this.curveProgress = 0f;
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x000285BA File Offset: 0x000267BA
	private void Splat()
	{
		this.curveProgress = 0f;
	}

	// Token: 0x040006C2 RID: 1730
	public Transform hitPositionTransform;

	// Token: 0x040006C3 RID: 1731
	public Transform blobTransform;

	// Token: 0x040006C4 RID: 1732
	public Transform blobMeshTransform;

	// Token: 0x040006C5 RID: 1733
	public AnimationCurve flyUpCurve;

	// Token: 0x040006C6 RID: 1734
	private float curveProgress;

	// Token: 0x040006C7 RID: 1735
	private Vector3 prevCheckPosition;

	// Token: 0x040006C8 RID: 1736
	public EnemyParent enemyParent;
}
