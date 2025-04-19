using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class CameraJump : MonoBehaviour
{
	// Token: 0x0600008F RID: 143 RVA: 0x00005D96 File Offset: 0x00003F96
	private void Awake()
	{
		CameraJump.instance = this;
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00005DA0 File Offset: 0x00003FA0
	public void Jump()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
		GameDirector.instance.CameraShake.Shake(2f, 0.1f);
		this.jumpActive = true;
		this.jumpLerp = 0f;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00005DF4 File Offset: 0x00003FF4
	public void Land()
	{
		if (this.landActive)
		{
			return;
		}
		GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
		GameDirector.instance.CameraShake.Shake(2f, 0.1f);
		this.landActive = true;
		this.landLerp = 0f;
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00005E50 File Offset: 0x00004050
	private void Update()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (this.jumpActive)
		{
			if (this.jumpLerp >= 1f)
			{
				this.jumpActive = false;
				this.jumpLerp = 0f;
			}
			else
			{
				vector += Vector3.LerpUnclamped(Vector3.zero, this.jumpPosition, this.jumpCurve.Evaluate(this.jumpLerp));
				vector2 += Vector3.LerpUnclamped(Vector3.zero, this.jumpRotation, this.jumpCurve.Evaluate(this.jumpLerp));
				this.jumpLerp += this.jumpSpeed * Time.deltaTime;
			}
		}
		if (this.landActive)
		{
			if (this.landLerp >= 1f)
			{
				this.landActive = false;
				this.landLerp = 0f;
			}
			else
			{
				vector += Vector3.LerpUnclamped(Vector3.zero, this.landPosition, this.landCurve.Evaluate(this.landLerp));
				vector2 += Vector3.LerpUnclamped(Vector3.zero, this.landRotation, this.landCurve.Evaluate(this.landLerp));
				this.landLerp += this.landSpeed * Time.deltaTime;
			}
		}
		vector *= GameplayManager.instance.cameraAnimation;
		vector2 *= GameplayManager.instance.cameraAnimation;
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, vector, 30f * Time.deltaTime);
		Quaternion localRotation = base.transform.localRotation;
		base.transform.localEulerAngles = vector2;
		Quaternion localRotation2 = base.transform.localRotation;
		base.transform.localRotation = localRotation;
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, localRotation2, 30f * Time.deltaTime);
	}

	// Token: 0x04000174 RID: 372
	public static CameraJump instance;

	// Token: 0x04000175 RID: 373
	internal bool jumpActive;

	// Token: 0x04000176 RID: 374
	public AnimationCurve jumpCurve;

	// Token: 0x04000177 RID: 375
	public float jumpSpeed = 1f;

	// Token: 0x04000178 RID: 376
	private float jumpLerp;

	// Token: 0x04000179 RID: 377
	public Vector3 jumpPosition;

	// Token: 0x0400017A RID: 378
	public Vector3 jumpRotation;

	// Token: 0x0400017B RID: 379
	[Space]
	private bool landActive;

	// Token: 0x0400017C RID: 380
	public AnimationCurve landCurve;

	// Token: 0x0400017D RID: 381
	public float landSpeed = 1f;

	// Token: 0x0400017E RID: 382
	private float landLerp;

	// Token: 0x0400017F RID: 383
	public Vector3 landPosition;

	// Token: 0x04000180 RID: 384
	public Vector3 landRotation;
}
