using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class ToolBackAway : MonoBehaviour
{
	// Token: 0x06000723 RID: 1827 RVA: 0x00043D85 File Offset: 0x00041F85
	private void Start()
	{
		this.StartPosition = base.transform.localPosition;
		this.Mask = SemiFunc.LayerMaskGetVisionObstruct();
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00043DA4 File Offset: 0x00041FA4
	private void FixedUpdate()
	{
		if (this.Active)
		{
			if (this.RaycastTimer <= 0f)
			{
				this.RaycastTimer = this.RaycastTime;
				this.LengthHit = this.Length;
				foreach (RaycastHit raycastHit in Physics.RaycastAll(this.ParentTransform.position, this.ParentTransform.forward, this.Length, this.Mask))
				{
					if ((!raycastHit.transform.CompareTag("Player") || !raycastHit.transform.GetComponent<PlayerController>()) && raycastHit.distance < this.LengthHit)
					{
						this.LengthHit = raycastHit.distance;
					}
				}
				return;
			}
			this.RaycastTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00043E7C File Offset: 0x0004207C
	private void Update()
	{
		if (this.Active)
		{
			this.BackAwayAmount = Mathf.Max(-this.BackAwayAmountMax, this.LengthHit - this.Length);
		}
		else
		{
			this.BackAwayAmount = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.springParams, Time.deltaTime, this.springFreq, this.springDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.current, ref this.velocity, this.BackAwayAmount, this.springParams);
		base.transform.localPosition = new Vector3(this.StartPosition.x, this.StartPosition.y, this.StartPosition.z + this.current);
	}

	// Token: 0x04000C69 RID: 3177
	public bool Active;

	// Token: 0x04000C6A RID: 3178
	public Transform ParentTransform;

	// Token: 0x04000C6B RID: 3179
	private LayerMask Mask;

	// Token: 0x04000C6C RID: 3180
	private float RaycastTime = 0.1f;

	// Token: 0x04000C6D RID: 3181
	private float RaycastTimer;

	// Token: 0x04000C6E RID: 3182
	public float Length = 1f;

	// Token: 0x04000C6F RID: 3183
	private float LengthHit;

	// Token: 0x04000C70 RID: 3184
	private float BackAwayAmount;

	// Token: 0x04000C71 RID: 3185
	public float BackAwayAmountMax;

	// Token: 0x04000C72 RID: 3186
	private Vector3 StartPosition;

	// Token: 0x04000C73 RID: 3187
	public float springFreq = 15f;

	// Token: 0x04000C74 RID: 3188
	public float springDamping = 0.5f;

	// Token: 0x04000C75 RID: 3189
	private float target;

	// Token: 0x04000C76 RID: 3190
	private float current;

	// Token: 0x04000C77 RID: 3191
	private float velocity;

	// Token: 0x04000C78 RID: 3192
	private SpringUtils.tDampedSpringMotionParams springParams = new SpringUtils.tDampedSpringMotionParams();
}
