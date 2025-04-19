using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class TruckHealerLine : MonoBehaviour
{
	// Token: 0x060008A3 RID: 2211 RVA: 0x00052DB9 File Offset: 0x00050FB9
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x00052DD8 File Offset: 0x00050FD8
	private void Update()
	{
		if (!this.lineTarget)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.curveEval >= 1f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.curveEval += Time.deltaTime * 2.5f;
		this.lineRenderer.widthMultiplier = this.widthCurve.Evaluate(this.curveEval);
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.lineTarget.position;
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(position, position2, num) - Vector3.up * Mathf.Sin(num * 3.1415927f) * 0.5f;
				float d = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num2 = 1f;
				float d2 = this.wobbleCurve.Evaluate(num) * 2f;
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.02f * d * d2;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.02f * d * d2;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
			return;
		}
		this.outro = true;
	}

	// Token: 0x04000FAE RID: 4014
	public Transform lineTarget;

	// Token: 0x04000FAF RID: 4015
	private LineRenderer lineRenderer;

	// Token: 0x04000FB0 RID: 4016
	public AnimationCurve wobbleCurve;

	// Token: 0x04000FB1 RID: 4017
	public AnimationCurve widthCurve;

	// Token: 0x04000FB2 RID: 4018
	private float curveEval;

	// Token: 0x04000FB3 RID: 4019
	internal bool outro;
}
