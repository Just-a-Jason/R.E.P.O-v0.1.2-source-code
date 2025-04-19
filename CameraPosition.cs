using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class CameraPosition : MonoBehaviour
{
	// Token: 0x060000A0 RID: 160 RVA: 0x000062CB File Offset: 0x000044CB
	private void Awake()
	{
		CameraPosition.instance = this;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000062D4 File Offset: 0x000044D4
	private void Update()
	{
		float num = this.positionSmooth;
		if (this.tumbleSetTimer > 0f)
		{
			num *= 0.5f;
			this.tumbleSetTimer -= Time.deltaTime;
		}
		Vector3 vector = this.playerTransform.localPosition + this.playerOffset;
		if (SemiFunc.MenuLevel() && CameraNoPlayerTarget.instance)
		{
			vector = CameraNoPlayerTarget.instance.transform.position;
		}
		base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, vector, num * Time.deltaTime);
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.identity, num * Time.deltaTime);
		if (SemiFunc.MenuLevel())
		{
			base.transform.localPosition = vector;
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000063A6 File Offset: 0x000045A6
	public void TumbleSet()
	{
		this.tumbleSetTimer = 0.5f;
	}

	// Token: 0x0400018B RID: 395
	public static CameraPosition instance;

	// Token: 0x0400018C RID: 396
	public Transform playerTransform;

	// Token: 0x0400018D RID: 397
	public Vector3 playerOffset;

	// Token: 0x0400018E RID: 398
	public CameraTarget camController;

	// Token: 0x0400018F RID: 399
	public float positionSmooth = 2f;

	// Token: 0x04000190 RID: 400
	private float tumbleSetTimer;
}
