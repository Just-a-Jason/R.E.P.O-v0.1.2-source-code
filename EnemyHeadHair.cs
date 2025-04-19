using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class EnemyHeadHair : MonoBehaviour
{
	// Token: 0x0600031A RID: 794 RVA: 0x0001E7E1 File Offset: 0x0001C9E1
	private void Start()
	{
		this.Scale = base.transform.localScale;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0001E7F4 File Offset: 0x0001C9F4
	private void Update()
	{
		if (this.PositionSpeed == 0f)
		{
			base.transform.position = this.Target.position;
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.Target.position, this.PositionSpeed * Time.deltaTime);
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.Target.rotation, this.RotationSpeed * Time.deltaTime);
		base.transform.localScale = new Vector3(this.Scale.x * this.Target.lossyScale.x, this.Scale.y * this.Target.lossyScale.y, this.Scale.z * this.Target.lossyScale.z);
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0001E8F0 File Offset: 0x0001CAF0
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = this.DebugShow;
		}
	}

	// Token: 0x0400056B RID: 1387
	public Transform Target;

	// Token: 0x0400056C RID: 1388
	public bool DebugShow;

	// Token: 0x0400056D RID: 1389
	[Space]
	public float PositionSpeed;

	// Token: 0x0400056E RID: 1390
	public float RotationSpeed;

	// Token: 0x0400056F RID: 1391
	private Vector3 Scale;
}
