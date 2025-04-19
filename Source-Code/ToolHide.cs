using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class ToolHide : MonoBehaviour
{
	// Token: 0x0600073F RID: 1855 RVA: 0x00044B70 File Offset: 0x00042D70
	public void Show()
	{
		this.ShowTimer = 0.02f;
		base.transform.localPosition = this.ToolController.CurrentHidePosition;
		base.transform.localRotation = Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z);
		this.ActiveLerp = 0f;
		this.Active = true;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00044BF0 File Offset: 0x00042DF0
	public void Hide()
	{
		this.ActiveLerp = 0f;
		this.Active = false;
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x00044C04 File Offset: 0x00042E04
	private void Update()
	{
		if (this.ActiveLerp < 1f)
		{
			this.ActiveLerp += this.ToolController.CurrentHideSpeed * Time.deltaTime;
			this.ActiveLerp = Mathf.Clamp01(this.ActiveLerp);
			if (this.ActiveLerp >= 1f && !this.Active)
			{
				this.ToolController.HideTool();
			}
		}
		if (this.Active)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(this.ToolController.CurrentHidePosition, new Vector3(0f, 0f, 0f), this.ShowCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z), Quaternion.Euler(0f, 0f, 0f), this.ShowCurve.Evaluate(this.ActiveLerp));
			base.transform.localScale = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), this.ShowScaleCurve.Evaluate(this.ActiveLerp));
		}
		else if (this.ActiveLerp < 1f)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), this.ToolController.CurrentHidePosition, this.HideCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z), this.HideCurve.Evaluate(this.ActiveLerp));
			base.transform.localScale = Vector3.LerpUnclamped(new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), this.HideScaleCurve.Evaluate(this.ActiveLerp));
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		if (this.ShowTimer > 0f)
		{
			this.ShowTimer -= 1f * Time.deltaTime;
			if (this.ShowTimer <= 0f)
			{
				this.ToolController.ShowTool();
			}
		}
	}

	// Token: 0x04000CA8 RID: 3240
	public ToolController ToolController;

	// Token: 0x04000CA9 RID: 3241
	public AnimationCurve ShowCurve;

	// Token: 0x04000CAA RID: 3242
	public AnimationCurve ShowScaleCurve;

	// Token: 0x04000CAB RID: 3243
	public AnimationCurve HideCurve;

	// Token: 0x04000CAC RID: 3244
	public AnimationCurve HideScaleCurve;

	// Token: 0x04000CAD RID: 3245
	[HideInInspector]
	public bool Active;

	// Token: 0x04000CAE RID: 3246
	[HideInInspector]
	public float ActiveLerp;

	// Token: 0x04000CAF RID: 3247
	private float ShowTimer;
}
