using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class ToolActiveOffset : MonoBehaviour
{
	// Token: 0x06000721 RID: 1825 RVA: 0x00043B40 File Offset: 0x00041D40
	private void Update()
	{
		if (this.Active != this.ActivePrev && this.ActiveLerp >= 1f)
		{
			if (this.MoveSoundAutomatic || this.MoveSoundManual)
			{
				this.MoveSoundManual = false;
				this.MoveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveLerp = 0f;
			this.ActivePrev = this.Active;
			this.ActiveCurrent = this.Active;
		}
		else
		{
			if (this.ActiveCurrent)
			{
				this.ActiveLerp += this.IntroSpeed * Time.deltaTime;
			}
			else
			{
				this.ActiveLerp += this.OutroSpeed * Time.deltaTime;
			}
			this.ActiveLerp = Mathf.Clamp01(this.ActiveLerp);
		}
		if (this.ActiveCurrent)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(this.InactivePosition, this.ActivePosition, this.IntroCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.InactiveRotation.x, this.InactiveRotation.y, this.InactiveRotation.z), Quaternion.Euler(this.ActiveRotation.x, this.ActiveRotation.y, this.ActiveRotation.z), this.IntroCurve.Evaluate(this.ActiveLerp));
			return;
		}
		base.transform.localPosition = Vector3.LerpUnclamped(this.ActivePosition, this.InactivePosition, this.OutroCurve.Evaluate(this.ActiveLerp));
		base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.ActiveRotation.x, this.ActiveRotation.y, this.ActiveRotation.z), Quaternion.Euler(this.InactiveRotation.x, this.InactiveRotation.y, this.InactiveRotation.z), this.OutroCurve.Evaluate(this.ActiveLerp));
	}

	// Token: 0x04000C5A RID: 3162
	public bool Active;

	// Token: 0x04000C5B RID: 3163
	private bool ActivePrev;

	// Token: 0x04000C5C RID: 3164
	private bool ActiveCurrent;

	// Token: 0x04000C5D RID: 3165
	[HideInInspector]
	public float ActiveLerp = 1f;

	// Token: 0x04000C5E RID: 3166
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04000C5F RID: 3167
	public float IntroSpeed = 1.5f;

	// Token: 0x04000C60 RID: 3168
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C61 RID: 3169
	public float OutroSpeed = 1.5f;

	// Token: 0x04000C62 RID: 3170
	[Space]
	public Vector3 InactivePosition;

	// Token: 0x04000C63 RID: 3171
	public Vector3 InactiveRotation;

	// Token: 0x04000C64 RID: 3172
	[Space]
	public Vector3 ActivePosition;

	// Token: 0x04000C65 RID: 3173
	public Vector3 ActiveRotation;

	// Token: 0x04000C66 RID: 3174
	[Space]
	[Header("Sound")]
	public bool MoveSoundAutomatic;

	// Token: 0x04000C67 RID: 3175
	[HideInInspector]
	public bool MoveSoundManual;

	// Token: 0x04000C68 RID: 3176
	public Sound MoveSound;
}
