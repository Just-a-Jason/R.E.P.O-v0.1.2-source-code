using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class CameraTopFade : MonoBehaviour
{
	// Token: 0x060000AF RID: 175 RVA: 0x00006F1F File Offset: 0x0000511F
	private void Awake()
	{
		CameraTopFade.Instance = this;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00006F28 File Offset: 0x00005128
	public void Set(float amount, float time)
	{
		this.ActiveTimer = time;
		if (!this.Active)
		{
			this.Active = true;
			this.AmountStart = this.AmountCurrent;
			this.AmountEnd = amount;
			this.LerpAmount = 0f;
		}
		if (!this.Fading)
		{
			Color color = this.Mesh.material.color;
			color.a = 0f;
			this.Mesh.material.color = color;
			this.MeshTransform.gameObject.SetActive(true);
			this.Fading = true;
			base.StartCoroutine(this.Fade());
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00006FC4 File Offset: 0x000051C4
	private IEnumerator Fade()
	{
		while (this.Fading)
		{
			if (this.Active)
			{
				this.AmountCurrent = Mathf.Lerp(this.AmountStart, this.AmountEnd, this.Curve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.Speed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.ActiveTimer > 0f)
				{
					this.ActiveTimer -= Time.deltaTime;
				}
				else
				{
					this.AmountStart = this.AmountCurrent;
					this.AmountEnd = 0f;
					this.Active = false;
					this.LerpAmount = 0f;
				}
			}
			else
			{
				this.AmountCurrent = Mathf.Lerp(this.AmountStart, this.AmountEnd, this.Curve.Evaluate(this.LerpAmount));
				this.LerpAmount += this.Speed * Time.deltaTime;
				this.LerpAmount = Mathf.Clamp01(this.LerpAmount);
				if (this.LerpAmount >= 1f)
				{
					this.Fading = false;
					this.MeshTransform.gameObject.SetActive(false);
				}
			}
			Color color = this.Mesh.material.color;
			color.a = this.AmountCurrent;
			this.Mesh.material.color = color;
			yield return null;
		}
		yield break;
	}

	// Token: 0x040001C7 RID: 455
	public static CameraTopFade Instance;

	// Token: 0x040001C8 RID: 456
	public Transform MeshTransform;

	// Token: 0x040001C9 RID: 457
	public MeshRenderer Mesh;

	// Token: 0x040001CA RID: 458
	[Space]
	public AnimationCurve Curve;

	// Token: 0x040001CB RID: 459
	public float Speed = 1f;

	// Token: 0x040001CC RID: 460
	private bool Fading;

	// Token: 0x040001CD RID: 461
	private bool Active;

	// Token: 0x040001CE RID: 462
	private float ActiveTimer;

	// Token: 0x040001CF RID: 463
	private float Amount;

	// Token: 0x040001D0 RID: 464
	private float AmountCurrent;

	// Token: 0x040001D1 RID: 465
	private float AmountStart;

	// Token: 0x040001D2 RID: 466
	private float AmountEnd;

	// Token: 0x040001D3 RID: 467
	private float LerpAmount;
}
