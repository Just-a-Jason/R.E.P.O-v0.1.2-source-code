using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class CameraAimOffset : MonoBehaviour
{
	// Token: 0x0600007F RID: 127 RVA: 0x000055F0 File Offset: 0x000037F0
	private void Awake()
	{
		CameraAimOffset.Instance = this;
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000055F8 File Offset: 0x000037F8
	private void Start()
	{
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00005607 File Offset: 0x00003807
	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.5f);
		if (!CameraNoPlayerTarget.instance)
		{
			this.Set(Vector3.zero, new Vector3(45f, 0f, 0f), 0f);
			this.Active = false;
			this.LerpAmount = 0.25f;
		}
		this.Active = false;
		this.PositionStart = this.PositionEnd;
		this.RotationStart = this.RotationEnd;
		this.PositionEnd = Vector3.zero;
		this.RotationEnd = Vector3.zero;
		yield break;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00005618 File Offset: 0x00003818
	public void Set(Vector3 position, Vector3 rotation, float time)
	{
		if (!this.Active || position != this.PositionEnd || rotation != this.RotationEnd)
		{
			this.PositionStart = base.transform.localPosition;
			this.RotationStart = base.transform.localEulerAngles;
			this.PositionEnd = position;
			this.RotationEnd = rotation;
			this.Active = true;
			this.LerpAmount = 0f;
		}
		this.ActiveTimer = time;
		if (!this.Animating)
		{
			this.Animating = true;
			base.StartCoroutine(this.Animate());
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000056AE File Offset: 0x000038AE
	private IEnumerator Animate()
	{
		while (GameDirector.instance.currentState != GameDirector.gameState.Main || this.IntroPauseTimer > 0f)
		{
			this.IntroPauseTimer -= Time.deltaTime;
			yield return null;
		}
		if (CameraNoPlayerTarget.instance)
		{
			yield break;
		}
		while (this.Animating)
		{
			if (this.Active)
			{
				base.transform.localPosition = Vector3.Lerp(this.PositionStart, this.PositionEnd, this.IntroCurve.Evaluate(this.LerpAmount));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.RotationStart), Quaternion.Euler(this.RotationEnd), this.IntroCurve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.IntroSpeed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.ActiveTimer > 0f)
				{
					this.ActiveTimer -= Time.deltaTime;
				}
				else
				{
					this.PositionStart = base.transform.localPosition;
					this.RotationStart = base.transform.localRotation.eulerAngles;
					this.PositionEnd = Vector3.zero;
					this.RotationEnd = Vector3.zero;
					this.Active = false;
					this.LerpAmount = 0f;
				}
			}
			else
			{
				base.transform.localPosition = Vector3.Lerp(this.PositionStart, this.PositionEnd, this.OutroCurve.Evaluate(this.LerpAmount));
				base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.RotationStart), Quaternion.Euler(this.RotationEnd), this.OutroCurve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.OutroSpeed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.LerpAmount >= 1f)
				{
					this.Animating = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000144 RID: 324
	public static CameraAimOffset Instance;

	// Token: 0x04000145 RID: 325
	private bool Animating;

	// Token: 0x04000146 RID: 326
	private bool Active;

	// Token: 0x04000147 RID: 327
	private float ActiveTimer;

	// Token: 0x04000148 RID: 328
	public AnimationCurve IntroCurve;

	// Token: 0x04000149 RID: 329
	public float IntroSpeed = 1f;

	// Token: 0x0400014A RID: 330
	public AnimationCurve OutroCurve;

	// Token: 0x0400014B RID: 331
	public float OutroSpeed = 1f;

	// Token: 0x0400014C RID: 332
	private float LerpAmount;

	// Token: 0x0400014D RID: 333
	private Vector3 PositionStart;

	// Token: 0x0400014E RID: 334
	private Vector3 RotationStart;

	// Token: 0x0400014F RID: 335
	private Vector3 PositionEnd;

	// Token: 0x04000150 RID: 336
	private Vector3 RotationEnd;

	// Token: 0x04000151 RID: 337
	private float IntroPauseTimer = 0.1f;
}
