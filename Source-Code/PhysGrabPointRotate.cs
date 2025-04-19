using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class PhysGrabPointRotate : MonoBehaviour
{
	// Token: 0x06000D75 RID: 3445 RVA: 0x00078648 File Offset: 0x00076848
	private void Start()
	{
		this.popIn = AssetManager.instance.animationCurveWooshIn;
		this.popOut = AssetManager.instance.animationCurveWooshAway;
		base.transform.localScale = Vector3.zero;
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.material = this.originalMaterial;
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000786A4 File Offset: 0x000768A4
	private void OnEnable()
	{
		if (!this.meshRenderer)
		{
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}
		if (this.meshRenderer)
		{
			if (!VideoGreenScreen.instance)
			{
				this.meshRenderer.material = this.originalMaterial;
				return;
			}
			this.meshRenderer.material = this.greenScreenMaterial;
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00078708 File Offset: 0x00076908
	private void Update()
	{
		if (!this.physGrabber)
		{
			return;
		}
		if (this.physGrabber)
		{
			Vector3 mouseTurningVelocity = this.physGrabber.mouseTurningVelocity;
			if (this.physGrabber.isRotating)
			{
				this.rotationActiveTimer = 0.1f;
			}
			if (this.rotationActiveTimer > 0f)
			{
				this.physGrabber.OverrideColorToPurple(0.1f);
				base.transform.LookAt(this.physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform.position);
				this.animationEval += Time.deltaTime * 2f;
				this.animationEval = Mathf.Clamp(this.animationEval, 0f, 1f);
				float d = this.popIn.Evaluate(this.animationEval);
				base.transform.localScale = Vector3.one * 0.5f * d;
				base.transform.Rotate(0f, 0f, -Mathf.Atan2(mouseTurningVelocity.y, mouseTurningVelocity.x) * 57.29578f);
				this.smoothRotation = Quaternion.Slerp(this.smoothRotation, base.transform.rotation, Time.deltaTime * 10f);
				base.transform.rotation = this.smoothRotation;
				this.rotationActiveTimer -= Time.deltaTime;
			}
			else
			{
				this.animationEval -= Time.deltaTime * 6f;
				this.animationEval = Mathf.Clamp(this.animationEval, 0f, 1f);
				float d2 = this.popOut.Evaluate(1f - this.animationEval);
				Vector3 a = Vector3.one * 0.5f;
				base.transform.localScale = a - a * d2;
			}
		}
		if (base.transform.localScale.magnitude < 0.01f)
		{
			this.meshRenderer.enabled = false;
		}
		else
		{
			this.meshRenderer.enabled = true;
		}
		float magnitude = this.physGrabber.mouseTurningVelocity.magnitude;
		this.offsetX -= magnitude * 0.2f * Time.deltaTime;
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(this.offsetX, 0f);
		float num = Mathf.Sin(Time.time * 10f) * 0.2f;
		base.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f, 1f + num);
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(this.offsetX, -num * 0.5f);
	}

	// Token: 0x04001604 RID: 5636
	[HideInInspector]
	public PhysGrabber physGrabber;

	// Token: 0x04001605 RID: 5637
	private Quaternion smoothRotation;

	// Token: 0x04001606 RID: 5638
	[HideInInspector]
	public float rotationActiveTimer;

	// Token: 0x04001607 RID: 5639
	private float rotationSpeed;

	// Token: 0x04001608 RID: 5640
	private float offsetX;

	// Token: 0x04001609 RID: 5641
	private AnimationCurve popIn;

	// Token: 0x0400160A RID: 5642
	private AnimationCurve popOut;

	// Token: 0x0400160B RID: 5643
	private MeshRenderer meshRenderer;

	// Token: 0x0400160C RID: 5644
	public Material originalMaterial;

	// Token: 0x0400160D RID: 5645
	public Material greenScreenMaterial;

	// Token: 0x0400160E RID: 5646
	[HideInInspector]
	public float animationEval;
}
