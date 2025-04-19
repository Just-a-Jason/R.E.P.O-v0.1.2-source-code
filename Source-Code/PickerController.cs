using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000C2 RID: 194
public class PickerController : MonoBehaviour
{
	// Token: 0x06000702 RID: 1794 RVA: 0x00042308 File Offset: 0x00040508
	private void Start()
	{
		this.IntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.AnimatedPicker.SetActive(false);
		this.StabPoint.SetActive(false);
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0004239C File Offset: 0x0004059C
	private void Update()
	{
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.OutroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
		if (this.isAnimating)
		{
			this.AnimatePicker();
		}
		this.AlignStabObjects();
		if (this.ShowTimer > 0f)
		{
			this.ShowTimer -= Time.deltaTime;
		}
		if (ToolController.instance.Interact)
		{
			this.isStabbing = true;
		}
		Interaction activeInteraction = ToolController.instance.ActiveInteraction;
		if (activeInteraction && !this.isAnimating && this.ShowTimer <= 0f && ToolController.instance.ToolHide.Active && this.isStabbing)
		{
			this.StabPoint.SetActive(true);
			PaperPick component = activeInteraction.GetComponent<PaperPick>();
			this.stabObject = component.PaperInteraction.GameObject();
			this.StabPoint.transform.position = component.PaperInteraction.PaperTransform.position;
			Vector3 forward = this.StabPoint.transform.position - base.transform.position;
			this.StabPoint.transform.rotation = Quaternion.LookRotation(forward);
			GameDirector.instance.CameraShake.Shake(3f, 0.2f);
			this.StartAnimation();
			this.isStabbing = false;
		}
		base.transform.position = ToolController.instance.ToolFollow.transform.position;
		base.transform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		base.transform.localScale = ToolController.instance.ToolHide.transform.localScale;
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000425A4 File Offset: 0x000407A4
	private void AlignStabObjects()
	{
		for (int i = 0; i < this.stabObjects.Count; i++)
		{
			Vector3 position = this.PickerPoint.position;
			Vector3 position2 = this.PickerPointEnd.position;
			if (this.isAnimating)
			{
				position = this.PickerPointAnimate.position;
				position2 = this.PickerPointEndAnimate.position;
			}
			float num = this.StabObjectSpacing * (float)i;
			float num2 = Vector3.Distance(position, position2);
			float t = num / num2;
			this.stabObjects[i].transform.position = Vector3.Lerp(position, position2, t);
			this.stabObjects[i].transform.LookAt(base.transform.position);
			this.stabObjects[i].transform.Rotate(90f, 0f, 0f, Space.Self);
			this.stabObjects[i].transform.Rotate(0f, this.stabObjectsAngles[i], 0f, Space.Self);
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x000426B0 File Offset: 0x000408B0
	private void AnimatePicker()
	{
		if (this.animationProgress < 2f)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
			int num = 0;
			if (!this.introAnimation)
			{
				num = 1;
			}
			if (!this.introAnimation)
			{
				this.animationProgress += Time.deltaTime * this.AnimationSpeedOutro;
				float t = this.PickerStabOutro.Evaluate(this.animationProgress - (float)num);
				this.AnimatedPicker.transform.position = Vector3.LerpUnclamped(this.PositionStart, this.meshObject.transform.position, t);
				this.AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(this.RotationStart, this.meshObject.transform.rotation, t);
				this.AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(this.ScaleStart, this.meshObject.transform.localScale, t);
			}
			else
			{
				this.animationProgress += Time.deltaTime * this.AnimationSpeedIntro;
				float t2 = this.PickerStabIntro.Evaluate(this.animationProgress - (float)num);
				this.AnimatedPicker.transform.position = Vector3.LerpUnclamped(this.PositionStart, this.StabPointChild.transform.position, t2);
				this.AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(this.RotationStart, this.StabPointChild.transform.rotation, t2);
				this.AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(this.ScaleStart, this.StabPointChild.transform.localScale, t2);
			}
			if (this.animationProgress > 1f && !this.stab)
			{
				GameDirector.instance.CameraImpact.Shake(3f, 0f);
				PaperInteraction component = this.stabObject.GetComponent<PaperInteraction>();
				component.Picked = true;
				component.CleanEffect.Clean();
				component.CleanEffect.transform.parent = null;
				GameObject paperVisual = component.paperVisual;
				paperVisual.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
				paperVisual.layer = LayerMask.NameToLayer("TopLayer");
				this.StabSound.Play(paperVisual.transform.position, 1f, 1f, 1f, 1f);
				paperVisual.transform.parent = this.ParentTransform;
				this.stabObjects.Add(paperVisual);
				this.stabObjectsAngles.Add((float)Random.Range(0, 360));
				this.introAnimation = false;
				this.stab = true;
				this.AnimationSet(this.introAnimation);
				GameDirector.instance.CameraShake.Shake(2f, 0.25f);
				return;
			}
		}
		else
		{
			this.EndAnimation();
		}
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0004297C File Offset: 0x00040B7C
	private void AnimationSet(bool intro)
	{
		if (intro)
		{
			this.RotationStart = this.meshObject.transform.rotation;
			this.PositionStart = this.meshObject.transform.position;
			this.ScaleStart = this.meshObject.transform.localScale;
		}
		else
		{
			this.RotationStart = this.StabPointChild.transform.rotation;
			this.PositionStart = this.StabPointChild.transform.position;
			this.ScaleStart = this.StabPointChild.transform.localScale;
		}
		this.AnimatedPicker.transform.rotation = this.RotationStart;
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00042A28 File Offset: 0x00040C28
	public void StartAnimation()
	{
		this.isAnimating = true;
		this.animationProgress = 0f;
		this.StabPoint.SetActive(true);
		this.AnimatedPicker.SetActive(true);
		this.AnimatedPicker.transform.position = base.transform.position;
		this.AnimatedPicker.transform.rotation = base.transform.rotation;
		this.AnimatedPicker.transform.localScale = base.transform.localScale;
		this.meshRenderer.enabled = false;
		this.AnimationSet(this.introAnimation);
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00042AC8 File Offset: 0x00040CC8
	private void EndAnimation()
	{
		this.stab = false;
		this.introAnimation = true;
		this.isAnimating = false;
		this.StabPoint.SetActive(false);
		this.AnimatedPicker.SetActive(false);
		this.meshRenderer.enabled = true;
	}

	// Token: 0x04000BEF RID: 3055
	public Transform ParentTransform;

	// Token: 0x04000BF0 RID: 3056
	public AnimationCurve PickerStabIntro;

	// Token: 0x04000BF1 RID: 3057
	public AnimationCurve PickerStabOutro;

	// Token: 0x04000BF2 RID: 3058
	public float AnimationSpeedIntro = 1f;

	// Token: 0x04000BF3 RID: 3059
	public float AnimationSpeedOutro = 1f;

	// Token: 0x04000BF4 RID: 3060
	public GameObject AnimatedPicker;

	// Token: 0x04000BF5 RID: 3061
	[Space]
	public GameObject StabPoint;

	// Token: 0x04000BF6 RID: 3062
	public GameObject StabPointChild;

	// Token: 0x04000BF7 RID: 3063
	[Space]
	private LayerMask Mask;

	// Token: 0x04000BF8 RID: 3064
	private Camera MainCamera;

	// Token: 0x04000BF9 RID: 3065
	public GameObject meshObject;

	// Token: 0x04000BFA RID: 3066
	public MeshRenderer meshRenderer;

	// Token: 0x04000BFB RID: 3067
	private bool isAnimating;

	// Token: 0x04000BFC RID: 3068
	private float animationProgress;

	// Token: 0x04000BFD RID: 3069
	private float ShowTimer = 0.3f;

	// Token: 0x04000BFE RID: 3070
	private bool stab;

	// Token: 0x04000BFF RID: 3071
	private GameObject stabObject;

	// Token: 0x04000C00 RID: 3072
	private List<GameObject> stabObjects = new List<GameObject>();

	// Token: 0x04000C01 RID: 3073
	private List<float> stabObjectsAngles = new List<float>();

	// Token: 0x04000C02 RID: 3074
	[Space]
	public Transform PickerPoint;

	// Token: 0x04000C03 RID: 3075
	public Transform PickerPointEnd;

	// Token: 0x04000C04 RID: 3076
	public Transform PickerPointAnimate;

	// Token: 0x04000C05 RID: 3077
	public Transform PickerPointEndAnimate;

	// Token: 0x04000C06 RID: 3078
	public float StabObjectSpacing = 10f;

	// Token: 0x04000C07 RID: 3079
	private bool isStabbing;

	// Token: 0x04000C08 RID: 3080
	private bool introAnimation = true;

	// Token: 0x04000C09 RID: 3081
	private Quaternion RotationStart;

	// Token: 0x04000C0A RID: 3082
	private Quaternion RotationEnd;

	// Token: 0x04000C0B RID: 3083
	private Vector3 PositionStart;

	// Token: 0x04000C0C RID: 3084
	private Vector3 PositionEnd;

	// Token: 0x04000C0D RID: 3085
	private Vector3 ScaleStart;

	// Token: 0x04000C0E RID: 3086
	private Vector3 ScaleEnd;

	// Token: 0x04000C0F RID: 3087
	[Space]
	public Sound IntroSound;

	// Token: 0x04000C10 RID: 3088
	public Sound OutroSound;

	// Token: 0x04000C11 RID: 3089
	public Sound StabSound;

	// Token: 0x04000C12 RID: 3090
	private bool OutroAudioPlay = true;
}
