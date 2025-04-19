using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class CameraTilt : MonoBehaviour
{
	// Token: 0x060000AC RID: 172 RVA: 0x00006BE7 File Offset: 0x00004DE7
	private void Awake()
	{
		CameraTilt.Instance = this;
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00006BF0 File Offset: 0x00004DF0
	private void Update()
	{
		if (SemiFunc.MenuLevel())
		{
			base.transform.localRotation = Quaternion.identity;
			return;
		}
		if (PlayerController.instance.Crouching)
		{
			this.AmountCurrent = Mathf.Lerp(this.AmountCurrent, this.Amount * this.CrouchMultiplier, Time.deltaTime * 5f);
		}
		else
		{
			this.AmountCurrent = Mathf.Lerp(this.AmountCurrent, this.Amount, Time.deltaTime * 5f);
		}
		float num = SemiFunc.InputMovementX();
		if (GameDirector.instance.DisableInput || SpectateCamera.instance || PlayerController.instance.InputDisableTimer > 0f)
		{
			num = 0f;
		}
		if (base.transform.rotation.x != this.previousX && base.transform.rotation.y != this.previousY)
		{
			if (Mathf.Abs(base.transform.rotation.eulerAngles.y - this.previousY) < 180f && Mathf.Abs(base.transform.rotation.eulerAngles.x - this.previousX) < 180f)
			{
				this.tiltXresult = (this.previousX - base.transform.rotation.eulerAngles.x) / Time.deltaTime * this.tiltX;
				this.tiltXresult = Mathf.Clamp(this.tiltXresult, -this.tiltXMax, this.tiltXMax);
				this.tiltZresult = (base.transform.rotation.eulerAngles.y - this.previousY) / Time.deltaTime * this.tiltZ + num * this.strafeAmount;
				this.tiltZresult = Mathf.Clamp(this.tiltZresult, -this.tiltZMax, this.tiltZMax);
				float num2 = 1f;
				if (SpectateCamera.instance)
				{
					num2 = 0.1f;
				}
				num2 *= GameplayManager.instance.cameraAnimation;
				this.targetAngle = Quaternion.Euler(this.tiltXresult * this.AmountCurrent * num2, 0f, this.tiltZresult * this.AmountCurrent * num2);
			}
			this.previousX = base.transform.rotation.eulerAngles.x;
			this.previousY = base.transform.rotation.eulerAngles.y;
		}
		float num3 = 3f;
		if (this.targetAngle == Quaternion.identity)
		{
			num3 = 10f;
		}
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, this.targetAngle, num3 * Time.deltaTime);
	}

	// Token: 0x040001B9 RID: 441
	public static CameraTilt Instance;

	// Token: 0x040001BA RID: 442
	public float tiltZ = 250f;

	// Token: 0x040001BB RID: 443
	public float tiltZMax = 10f;

	// Token: 0x040001BC RID: 444
	[Space]
	public float tiltX = 250f;

	// Token: 0x040001BD RID: 445
	public float tiltXMax = 10f;

	// Token: 0x040001BE RID: 446
	[Space]
	public float strafeAmount = 1f;

	// Token: 0x040001BF RID: 447
	public float CrouchMultiplier = 1f;

	// Token: 0x040001C0 RID: 448
	private float Amount = 1f;

	// Token: 0x040001C1 RID: 449
	private float AmountCurrent = 1f;

	// Token: 0x040001C2 RID: 450
	private float previousX;

	// Token: 0x040001C3 RID: 451
	private float previousY;

	// Token: 0x040001C4 RID: 452
	private Quaternion targetAngle;

	// Token: 0x040001C5 RID: 453
	[HideInInspector]
	public float tiltXresult;

	// Token: 0x040001C6 RID: 454
	[HideInInspector]
	public float tiltZresult;
}
