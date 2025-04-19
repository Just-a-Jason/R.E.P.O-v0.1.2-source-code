using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class EnemyHeadEyeTarget : MonoBehaviour
{
	// Token: 0x06000300 RID: 768 RVA: 0x0001DD78 File Offset: 0x0001BF78
	private void Start()
	{
		this.Camera = Camera.main;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0001DD88 File Offset: 0x0001BF88
	private void Update()
	{
		if (!this.Enemy.CheckChase())
		{
			this.Idle = true;
		}
		else
		{
			this.Idle = false;
		}
		if (this.Idle || !this.Enemy.TargetPlayerAvatar)
		{
			base.transform.position = this.Follow.position + this.Follow.forward * this.IdleOffset;
		}
		else
		{
			base.transform.position = this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
		}
		base.transform.rotation = this.Follow.rotation;
		float num = Vector3.Distance(this.Enemy.transform.position, this.Camera.transform.position) * this.PupilSizeMultiplier;
		num = Mathf.Clamp(num, this.PupilMinSize, this.PupilMaxSize);
		this.PupilCurrentSize = Mathf.Lerp(this.PupilCurrentSize, num, this.PupilSizeSpeed * Time.deltaTime);
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0001DE98 File Offset: 0x0001C098
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

	// Token: 0x0400052A RID: 1322
	public Enemy Enemy;

	// Token: 0x0400052B RID: 1323
	public Transform Follow;

	// Token: 0x0400052C RID: 1324
	[Space]
	public Vector3 Limit;

	// Token: 0x0400052D RID: 1325
	public float Speed;

	// Token: 0x0400052E RID: 1326
	public bool DebugShow;

	// Token: 0x0400052F RID: 1327
	[Space]
	public bool Idle = true;

	// Token: 0x04000530 RID: 1328
	public float IdleOffset;

	// Token: 0x04000531 RID: 1329
	[Space]
	public float PupilSizeMultiplier;

	// Token: 0x04000532 RID: 1330
	public float PupilSizeSpeed;

	// Token: 0x04000533 RID: 1331
	public float PupilMinSize;

	// Token: 0x04000534 RID: 1332
	public float PupilMaxSize;

	// Token: 0x04000535 RID: 1333
	[HideInInspector]
	public float PupilCurrentSize;

	// Token: 0x04000536 RID: 1334
	private Camera Camera;
}
