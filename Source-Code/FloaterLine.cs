using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class FloaterLine : MonoBehaviour
{
	// Token: 0x06000276 RID: 630 RVA: 0x00019471 File Offset: 0x00017671
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
		this.physGrabObject = this.lineTarget.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000277 RID: 631 RVA: 0x000194A0 File Offset: 0x000176A0
	private void Update()
	{
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 b = this.lineTarget.position;
			if (this.physGrabObject)
			{
				b = this.physGrabObject.midPoint;
			}
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 20f;
				array[i] = Vector3.Lerp(position, b, num) + Vector3.up * Mathf.Sin(num * 3.1415927f) * 1f;
				float num2 = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num3 = 1f;
				if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
				{
					num2 *= 3f;
					num3 = 2f;
				}
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
		}
		else
		{
			this.outro = true;
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop && !this.redMaterialSet)
		{
			this.lineRenderer.material = this.redMaterial;
			this.redMaterialSet = true;
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
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x0400047D RID: 1149
	public Transform lineTarget;

	// Token: 0x0400047E RID: 1150
	private LineRenderer lineRenderer;

	// Token: 0x0400047F RID: 1151
	private PhysGrabObject physGrabObject;

	// Token: 0x04000480 RID: 1152
	internal FloaterAttackLogic floaterAttack;

	// Token: 0x04000481 RID: 1153
	internal bool outro;

	// Token: 0x04000482 RID: 1154
	public Material redMaterial;

	// Token: 0x04000483 RID: 1155
	internal bool redMaterialSet;
}
