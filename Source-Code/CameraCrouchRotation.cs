using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class CameraCrouchRotation : MonoBehaviour
{
	// Token: 0x060000C6 RID: 198 RVA: 0x000077FC File Offset: 0x000059FC
	private void Start()
	{
		this.RotationLerp = 1f;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000780C File Offset: 0x00005A0C
	private void Update()
	{
		this.RotationLerp += Time.deltaTime * this.RotationSpeed;
		this.RotationLerp = Mathf.Clamp01(this.RotationLerp);
		float num;
		if (this.CameraCrouchPosition.Active)
		{
			num = this.RotationCurveIntro.Evaluate(this.RotationLerp) * this.Rotation;
		}
		else
		{
			num = this.RotationCurveOutro.Evaluate(this.RotationLerp) * this.Rotation;
		}
		num *= GameplayManager.instance.cameraAnimation;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
	}

	// Token: 0x04000204 RID: 516
	public CameraCrouchPosition CameraCrouchPosition;

	// Token: 0x04000205 RID: 517
	[Space]
	public float Rotation;

	// Token: 0x04000206 RID: 518
	public float RotationSpeed;

	// Token: 0x04000207 RID: 519
	public AnimationCurve RotationCurveIntro;

	// Token: 0x04000208 RID: 520
	public AnimationCurve RotationCurveOutro;

	// Token: 0x04000209 RID: 521
	[HideInInspector]
	public float RotationLerp;
}
