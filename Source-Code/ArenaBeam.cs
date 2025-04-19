using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class ArenaBeam : MonoBehaviour
{
	// Token: 0x06000778 RID: 1912 RVA: 0x00046EEB File Offset: 0x000450EB
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00046F0C File Offset: 0x0004510C
	private void Update()
	{
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.lineTarget.position;
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(position, position2, num);
				float d = 1f - Mathf.Abs(num - 0.3f) * 2f;
				float num2 = 1f;
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.05f * d;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.05f * d;
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
				this.lineRenderer.widthMultiplier = Mathf.Lerp(this.lineRenderer.widthMultiplier, 0f, Time.deltaTime * 2f);
				return;
			}
			this.lineRenderer.widthMultiplier = 0f;
			base.transform.parent.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x04000D31 RID: 3377
	public Transform lineTarget;

	// Token: 0x04000D32 RID: 3378
	private LineRenderer lineRenderer;

	// Token: 0x04000D33 RID: 3379
	private PhysGrabObject physGrabObject;

	// Token: 0x04000D34 RID: 3380
	internal bool outro;
}
