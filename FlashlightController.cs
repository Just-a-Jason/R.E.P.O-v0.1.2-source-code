using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class FlashlightController : MonoBehaviour
{
	// Token: 0x060006A9 RID: 1705 RVA: 0x0003FDB4 File Offset: 0x0003DFB4
	private void Start()
	{
		if (this.PlayerAvatar.isLocal)
		{
			FlashlightController.Instance = this;
			base.transform.parent = this.FollowTransformLocal;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		else
		{
			Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Triggers");
			}
			this.toolBackAway.Active = false;
			this.lightOnAudio.SpatialBlend = 1f;
			this.lightOnAudio.Volume *= 0.5f;
			this.lightOffAudio.SpatialBlend = 1f;
			this.lightOffAudio.Volume *= 0.5f;
		}
		this.mesh.enabled = false;
		this.spotlight.enabled = false;
		this.halo.enabled = false;
		this.LightActive = false;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0003FEBC File Offset: 0x0003E0BC
	private void Update()
	{
		if (GameDirector.instance.currentState >= GameDirector.gameState.Main && RunManager.instance.levelCurrent != RunManager.instance.levelLobby && RunManager.instance.levelCurrent != RunManager.instance.levelShop && !SemiFunc.MenuLevel() && !this.hideFlashlight)
		{
			if (this.PlayerAvatar.isDisabled || this.PlayerAvatar.isCrouching || this.PlayerAvatar.isTumbling)
			{
				this.active = false;
				if ((this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding) && this.currentState < FlashlightController.State.Idle && this.currentState != FlashlightController.State.Hidden)
				{
					this.currentState = FlashlightController.State.Idle;
				}
			}
			else
			{
				this.active = true;
			}
		}
		else
		{
			this.active = false;
		}
		if (this.PlayerAvatar.isDisabled && this.currentState != FlashlightController.State.Hidden)
		{
			this.currentState = FlashlightController.State.Hidden;
			this.mesh.enabled = false;
			this.spotlight.enabled = false;
			this.halo.enabled = false;
			this.LightActive = false;
			this.hiddenScale = 0f;
		}
		if (this.currentState == FlashlightController.State.Hidden)
		{
			this.Hidden();
		}
		else if (this.currentState == FlashlightController.State.Intro)
		{
			this.Intro();
		}
		else if (this.currentState == FlashlightController.State.LightOn)
		{
			this.LightOn();
		}
		else if (this.currentState == FlashlightController.State.Idle)
		{
			this.Idle();
		}
		else if (this.currentState == FlashlightController.State.LightOff)
		{
			this.LightOff();
		}
		else if (this.currentState == FlashlightController.State.Outro)
		{
			this.Outro();
		}
		if (!this.PlayerAvatar.isLocal)
		{
			base.transform.position = this.FollowTransformClient.position;
			base.transform.rotation = this.FollowTransformClient.rotation;
			base.transform.localScale = this.FollowTransformClient.localScale * this.hiddenScale;
		}
		else
		{
			base.transform.localScale = Vector3.one * this.hiddenScale;
		}
		float intensity = this.baseIntensity;
		if (RoundDirector.instance.allExtractionPointsCompleted)
		{
			this.flickerMultiplier = Mathf.Lerp(this.flickerMultiplier, this.flickerMultiplierTarget, 10f * Time.deltaTime);
			intensity = (this.baseIntensity + this.flickerIntensity) * this.flickerMultiplier;
			if (this.flickerLerp < 1f)
			{
				this.flickerLerp += 1.5f * Time.deltaTime;
				this.flickerIntensity = this.flickerCurve.Evaluate(this.flickerLerp) * 0.15f;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (this.flickerTimer <= 0f)
				{
					this.flickerTimer = Random.Range(2f, 10f);
					this.PlayerAvatar.FlashlightFlicker(Random.Range(0.25f, 0.35f));
				}
				else
				{
					this.flickerTimer -= Time.deltaTime;
				}
			}
		}
		this.spotlight.intensity = intensity;
		this.ClickAnim();
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x000401B7 File Offset: 0x0003E3B7
	private void Hidden()
	{
		if (this.active)
		{
			this.currentState = FlashlightController.State.Intro;
			this.stateTimer = 1f;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000401D4 File Offset: 0x0003E3D4
	private void Intro()
	{
		this.mesh.enabled = true;
		float num = Mathf.LerpUnclamped(this.hiddenRot, 0f, this.introCurveRot.Evaluate(this.introRotLerp));
		this.hideTransform.localRotation = Quaternion.Euler(0f, -num, -num);
		float num2 = this.introRotSpeed;
		if (!this.PlayerAvatar.isLocal)
		{
			num2 *= 2f;
		}
		this.introRotLerp += num2 * Time.deltaTime;
		this.introRotLerp = Mathf.Clamp01(this.introRotLerp);
		this.hiddenScale = Mathf.LerpUnclamped(0f, 1f, this.introCurveScale.Evaluate(this.introRotLerp));
		if (this.PlayerAvatar.isLocal)
		{
			float y = Mathf.LerpUnclamped(this.hiddenY, 0f, this.introCurveY.Evaluate(this.introYLerp));
			this.hideTransform.localPosition = new Vector3(0f, y, 0f);
			this.introYLerp += this.introYSpeed * Time.deltaTime;
			this.introYLerp = Mathf.Clamp01(this.introYLerp);
		}
		else
		{
			this.hideTransform.localPosition = Vector3.zero;
		}
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.LightOn;
			this.stateTimer = 0.5f;
			this.introRotLerp = 0f;
			this.introYLerp = 0f;
			this.click = true;
			this.lightOnAudio.Play(base.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00040390 File Offset: 0x0003E590
	private void LightOn()
	{
		this.spotlight.enabled = true;
		this.halo.enabled = true;
		this.LightActive = true;
		this.baseIntensity = Mathf.LerpUnclamped(0f, 0.7f, this.lightOnCurve.Evaluate(this.lightOnLerp));
		this.lightOnLerp += this.lightOnSpeed * Time.deltaTime;
		this.lightOnLerp = Mathf.Clamp01(this.lightOnLerp);
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Idle;
			this.lightOnLerp = 0f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00040440 File Offset: 0x0003E640
	private void Idle()
	{
		if (!this.active)
		{
			this.currentState = FlashlightController.State.LightOff;
			this.stateTimer = 0.25f;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				this.stateTimer = 0f;
			}
			this.click = true;
			this.lightOffAudio.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x000404C0 File Offset: 0x0003E6C0
	private void LightOff()
	{
		this.spotlight.enabled = false;
		this.halo.enabled = false;
		this.LightActive = false;
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Outro;
			this.stateTimer = 1f;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				this.stateTimer = 0.25f;
				return;
			}
		}
		else
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00040544 File Offset: 0x0003E744
	private void Outro()
	{
		float num = Mathf.LerpUnclamped(0f, this.hiddenRot, this.outroCurveRot.Evaluate(this.outroRotLerp));
		this.hideTransform.localRotation = Quaternion.Euler(0f, num, -num);
		float num2 = this.outroRotSpeed;
		if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
		{
			num2 *= 5f;
		}
		else if (!this.PlayerAvatar.isLocal)
		{
			num2 *= 2f;
		}
		this.outroRotLerp += num2 * Time.deltaTime;
		this.outroRotLerp = Mathf.Clamp01(this.outroRotLerp);
		this.hiddenScale = Mathf.LerpUnclamped(1f, 0f, this.outroCurveScale.Evaluate(this.outroRotLerp));
		if (this.PlayerAvatar.isLocal)
		{
			float y = Mathf.LerpUnclamped(0f, this.hiddenY, this.outroCurveY.Evaluate(this.outroYLerp));
			this.hideTransform.localPosition = new Vector3(0f, y, 0f);
			float num3 = this.outroYSpeed;
			if (this.PlayerAvatar.isTumbling || this.PlayerAvatar.isSliding)
			{
				num3 *= 5f;
			}
			this.outroYLerp += num3 * Time.deltaTime;
			this.outroYLerp = Mathf.Clamp01(this.outroYLerp);
		}
		else
		{
			this.hideTransform.localPosition = Vector3.zero;
		}
		if (this.stateTimer <= 0f)
		{
			this.currentState = FlashlightController.State.Hidden;
			this.mesh.enabled = false;
			this.outroRotLerp = 0f;
			this.outroYLerp = 0f;
			return;
		}
		this.stateTimer -= Time.deltaTime;
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0004070C File Offset: 0x0003E90C
	private void ClickAnim()
	{
		if (this.click)
		{
			float num = Mathf.LerpUnclamped(0f, this.clickStrength, this.clickCurve.Evaluate(this.clickLerp));
			this.clickTransform.localRotation = Quaternion.Euler(0f, -num, 0f);
			this.clickLerp += this.clickSpeed * Time.deltaTime;
			this.clickLerp = Mathf.Clamp01(this.clickLerp);
			if (this.clickLerp == 1f)
			{
				this.clickLerp = 0f;
				this.click = false;
			}
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000407AB File Offset: 0x0003E9AB
	public void FlickerSet(float _multiplier)
	{
		this.flickerLerp = 0f;
		this.flickerMultiplierTarget = _multiplier;
	}

	// Token: 0x04000B30 RID: 2864
	public static FlashlightController Instance;

	// Token: 0x04000B31 RID: 2865
	public Transform FollowTransformLocal;

	// Token: 0x04000B32 RID: 2866
	public Transform FollowTransformClient;

	// Token: 0x04000B33 RID: 2867
	internal bool hideFlashlight;

	// Token: 0x04000B34 RID: 2868
	[Space]
	public PlayerAvatar PlayerAvatar;

	// Token: 0x04000B35 RID: 2869
	public MeshRenderer mesh;

	// Token: 0x04000B36 RID: 2870
	public Light spotlight;

	// Token: 0x04000B37 RID: 2871
	public Behaviour halo;

	// Token: 0x04000B38 RID: 2872
	public Transform hideTransform;

	// Token: 0x04000B39 RID: 2873
	public Transform clickTransform;

	// Token: 0x04000B3A RID: 2874
	public ToolBackAway toolBackAway;

	// Token: 0x04000B3B RID: 2875
	internal bool active;

	// Token: 0x04000B3C RID: 2876
	internal FlashlightController.State currentState;

	// Token: 0x04000B3D RID: 2877
	private float stateTimer;

	// Token: 0x04000B3E RID: 2878
	[HideInInspector]
	public bool LightActive;

	// Token: 0x04000B3F RID: 2879
	[Header("Hidden")]
	public float hiddenRot;

	// Token: 0x04000B40 RID: 2880
	public float hiddenY;

	// Token: 0x04000B41 RID: 2881
	private float hiddenScale;

	// Token: 0x04000B42 RID: 2882
	[Header("Intro")]
	public AnimationCurve introCurveScale;

	// Token: 0x04000B43 RID: 2883
	public AnimationCurve introCurveRot;

	// Token: 0x04000B44 RID: 2884
	public AnimationCurve introCurveY;

	// Token: 0x04000B45 RID: 2885
	public float introRotSpeed;

	// Token: 0x04000B46 RID: 2886
	private float introRotLerp;

	// Token: 0x04000B47 RID: 2887
	public float introYSpeed;

	// Token: 0x04000B48 RID: 2888
	private float introYLerp;

	// Token: 0x04000B49 RID: 2889
	[Header("Light")]
	public AnimationCurve lightOnCurve;

	// Token: 0x04000B4A RID: 2890
	public float lightOnSpeed;

	// Token: 0x04000B4B RID: 2891
	private float lightOnLerp;

	// Token: 0x04000B4C RID: 2892
	public AnimationCurve clickCurve;

	// Token: 0x04000B4D RID: 2893
	public float clickSpeed;

	// Token: 0x04000B4E RID: 2894
	public float clickStrength;

	// Token: 0x04000B4F RID: 2895
	private float clickLerp;

	// Token: 0x04000B50 RID: 2896
	private bool click;

	// Token: 0x04000B51 RID: 2897
	private float baseIntensity;

	// Token: 0x04000B52 RID: 2898
	public Sound lightOnAudio;

	// Token: 0x04000B53 RID: 2899
	public Sound lightOffAudio;

	// Token: 0x04000B54 RID: 2900
	[Header("Outro")]
	public AnimationCurve outroCurveScale;

	// Token: 0x04000B55 RID: 2901
	public AnimationCurve outroCurveRot;

	// Token: 0x04000B56 RID: 2902
	public AnimationCurve outroCurveY;

	// Token: 0x04000B57 RID: 2903
	public float outroRotSpeed;

	// Token: 0x04000B58 RID: 2904
	private float outroRotLerp;

	// Token: 0x04000B59 RID: 2905
	public float outroYSpeed;

	// Token: 0x04000B5A RID: 2906
	private float outroYLerp;

	// Token: 0x04000B5B RID: 2907
	[Header("Flicker")]
	public AnimationCurve flickerCurve;

	// Token: 0x04000B5C RID: 2908
	private float flickerIntensity;

	// Token: 0x04000B5D RID: 2909
	private float flickerMultiplier = 0.5f;

	// Token: 0x04000B5E RID: 2910
	private float flickerMultiplierTarget = 0.5f;

	// Token: 0x04000B5F RID: 2911
	private float flickerLerp;

	// Token: 0x04000B60 RID: 2912
	private float flickerTimer;

	// Token: 0x020002F2 RID: 754
	internal enum State
	{
		// Token: 0x04002521 RID: 9505
		Hidden,
		// Token: 0x04002522 RID: 9506
		Intro,
		// Token: 0x04002523 RID: 9507
		LightOn,
		// Token: 0x04002524 RID: 9508
		Idle,
		// Token: 0x04002525 RID: 9509
		LightOff,
		// Token: 0x04002526 RID: 9510
		Outro
	}
}
