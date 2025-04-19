using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class MenuElementAnimations : MonoBehaviour
{
	// Token: 0x060010C8 RID: 4296 RVA: 0x00096CE0 File Offset: 0x00094EE0
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.forceMiddlePivot)
		{
			this.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
		this.initialPosition = this.rectTransform.anchoredPosition;
		this.initialScale = this.rectTransform.localScale.x;
		this.initialRotation = this.rectTransform.localEulerAngles.z;
		this.springFloatPosX = new SpringFloat();
		this.springFloatPosY = new SpringFloat();
		this.springFloatScale = new SpringFloat();
		this.springFloatRotation = new SpringFloat();
		this.springFloatPosX.lastPosition = this.initialPosition.x;
		this.springFloatPosY.lastPosition = this.initialPosition.y;
		this.springFloatScale.lastPosition = this.initialScale;
		this.springFloatRotation.lastPosition = this.initialRotation;
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x00096DD4 File Offset: 0x00094FD4
	private void Update()
	{
		float x = SemiFunc.SpringFloatGet(this.springFloatPosX, this.initialPosition.x, -1f);
		float y = SemiFunc.SpringFloatGet(this.springFloatPosY, this.initialPosition.y, -1f);
		float num = SemiFunc.SpringFloatGet(this.springFloatScale, this.initialScale, -1f);
		float z = SemiFunc.SpringFloatGet(this.springFloatRotation, this.initialRotation, -1f);
		this.rectTransform.anchoredPosition = new Vector2(x, y);
		this.rectTransform.localScale = new Vector3(num, num, 1f);
		this.rectTransform.localEulerAngles = new Vector3(0f, 0f, z);
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.UIAniNudgeX(10f, 0.2f, 1f);
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			Debug.Log("Nudge Y");
			this.UIAniNudgeY(10f, 0.2f, 1f);
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			this.UIAniScale(2f, 0.2f, 1f);
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			this.UIAniRotate(2f, 0.2f, 1f);
		}
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x00096F19 File Offset: 0x00095119
	public void UIAniNewInitialPosition(Vector2 newPos)
	{
		this.initialPosition = newPos;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x00096F22 File Offset: 0x00095122
	public void UIAniNudgeX(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatPosX.damping = dampen;
		this.springFloatPosX.springVelocity = nudgeForce * 100f;
		this.springFloatPosX.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x00096F56 File Offset: 0x00095156
	public void UIAniNudgeY(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatPosY.damping = dampen;
		this.springFloatPosY.springVelocity = nudgeForce * 100f;
		this.springFloatPosY.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x00096F8A File Offset: 0x0009518A
	public void UIAniScale(float scaleForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatScale.damping = dampen;
		this.springFloatScale.springVelocity = scaleForce * 1f;
		this.springFloatScale.speed = scaleForce * 15f * springStrengthMultiplier;
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x00096FBE File Offset: 0x000951BE
	public void UIAniRotate(float rotateForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatRotation.damping = dampen;
		this.springFloatRotation.springVelocity = rotateForce * 100f;
		this.springFloatRotation.speed = rotateForce * 15f * springStrengthMultiplier;
	}

	// Token: 0x04001BFF RID: 7167
	private SpringFloat springFloatScale;

	// Token: 0x04001C00 RID: 7168
	private SpringFloat springFloatPosX;

	// Token: 0x04001C01 RID: 7169
	private SpringFloat springFloatPosY;

	// Token: 0x04001C02 RID: 7170
	private SpringFloat springFloatRotation;

	// Token: 0x04001C03 RID: 7171
	private RectTransform rectTransform;

	// Token: 0x04001C04 RID: 7172
	private Vector2 initialPosition;

	// Token: 0x04001C05 RID: 7173
	private float initialScale;

	// Token: 0x04001C06 RID: 7174
	private float initialRotation;

	// Token: 0x04001C07 RID: 7175
	public bool forceMiddlePivot = true;
}
