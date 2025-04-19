using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class CeilingEyeLine : MonoBehaviour
{
	// Token: 0x060001E8 RID: 488 RVA: 0x00013340 File Offset: 0x00011540
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00013360 File Offset: 0x00011560
	private void Update()
	{
		base.transform.position = this.followTransform.position;
		base.transform.rotation = this.followTransform.rotation;
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
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.02f * d;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.02f * d;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
		}
		else
		{
			this.outro = true;
		}
		if (!this.outro)
		{
			if (this.lineRenderer.widthMultiplier < 0.195f)
			{
				this.lineRenderer.widthMultiplier = Mathf.Lerp(this.lineRenderer.widthMultiplier, 0.2f, Time.deltaTime * 2f);
				return;
			}
			this.lineRenderer.widthMultiplier = 0.2f;
			return;
		}
		else
		{
			if (this.lineRenderer.widthMultiplier > 0.005f)
			{
				this.lineRenderer.widthMultiplier = Mathf.Lerp(this.lineRenderer.widthMultiplier, 0f, Time.deltaTime * 5f);
				return;
			}
			this.lineRenderer.widthMultiplier = 0f;
			return;
		}
	}

	// Token: 0x040003CC RID: 972
	public Transform followTransform;

	// Token: 0x040003CD RID: 973
	public Transform lineTarget;

	// Token: 0x040003CE RID: 974
	private LineRenderer lineRenderer;

	// Token: 0x040003CF RID: 975
	internal bool outro;
}
