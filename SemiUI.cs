using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class SemiUI : MonoBehaviour
{
	// Token: 0x060012A3 RID: 4771 RVA: 0x000A2ED8 File Offset: 0x000A10D8
	protected virtual void Start()
	{
		if (this.uiText == null)
		{
			this.uiText = base.GetComponent<TextMeshProUGUI>();
		}
		if (this.uiText == null)
		{
			this.uiText = base.GetComponentInChildren<TextMeshProUGUI>();
		}
		this.initialPosition = base.transform.localPosition;
		if (this.uiText)
		{
			this.originalTextColor = this.uiText.color;
		}
		if (this.uiText)
		{
			this.originalFontColor = this.uiText.fontMaterial.GetColor(ShaderUtilities.ID_FaceColor);
		}
		if (this.uiText)
		{
			this.uiTextEnabledPrevious = this.uiText.enabled;
		}
		if (!this.textRectTransform)
		{
			this.textRectTransform = base.GetComponent<RectTransform>();
		}
		this.originalScale = this.textRectTransform.localScale;
		if (this.uiText)
		{
			this.originalGlowColor = this.uiText.fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
		}
		if (!this.animateTheEntireObject)
		{
			if (this.showPosition == new Vector2(0f, 0f))
			{
				this.showPosition = this.textRectTransform.localPosition;
			}
		}
		else if (this.showPosition == new Vector2(0f, 0f))
		{
			this.showPosition = base.transform.localPosition;
		}
		this.hidePosition += this.showPosition;
		base.StartCoroutine(this.LateStart());
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localPosition = this.hidePosition;
		}
		else
		{
			base.transform.localPosition = this.hidePosition;
		}
		this.hidePositionCurrent = this.hidePosition;
		this.hideAnimationEvaluation = 1f;
		if (this.uiText && !this.animateTheEntireObject)
		{
			this.uiText.enabled = false;
		}
		this.hideTimer = 0.2f;
		this.allChildren = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.allChildren.Add(transform.gameObject);
		}
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000A314C File Offset: 0x000A134C
	private void AllChildrenSetActive(bool active)
	{
		foreach (GameObject gameObject in this.allChildren)
		{
			gameObject.SetActive(active);
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x000A31A0 File Offset: 0x000A13A0
	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.2f);
		this.animationCurveWooshAway = AssetManager.instance.animationCurveWooshAway;
		this.animationCurveWooshIn = AssetManager.instance.animationCurveWooshIn;
		this.animationCurveInOut = AssetManager.instance.animationCurveInOut;
		this.initialized = true;
		yield break;
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x000A31B0 File Offset: 0x000A13B0
	protected virtual void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		this.FlashColorLogic(deltaTime);
		this.HideAnimationLogic(deltaTime);
		this.HideTimer(deltaTime);
		this.SpringScaleLogic(deltaTime);
		this.ScootPositionLogic(deltaTime);
		this.SpringShakeLogic(deltaTime);
		this.UpdatePositionLogic();
		this.prevShowTimer = this.showTimer;
		this.prevHideTimer = this.hideTimer;
		this.prevScootTimer = this.scootTimer;
		this.prevStopHidingTimer = this.stopHidingTimer;
		this.prevStopShowingTimer = this.stopShowingTimer;
		if (this.hideTimer >= 0f)
		{
			this.hideTimer -= deltaTime;
		}
		if (this.showTimer >= 0f)
		{
			this.showTimer -= deltaTime;
		}
		if (this.stopShowingTimer >= 0f)
		{
			this.stopShowingTimer -= deltaTime;
		}
		if (this.stopHidingTimer >= 0f)
		{
			this.stopHidingTimer -= deltaTime;
		}
		if (this.stopScootingTimer >= 0f)
		{
			this.stopScootingTimer -= deltaTime;
		}
		if (this.scootTimer >= 0f)
		{
			this.scootTimer -= deltaTime;
		}
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x000A32DA File Offset: 0x000A14DA
	public void SemiUISpringScale(float amount, float frequency, float time)
	{
		this.scaleTime = 0f;
		this.scaleAmount = amount;
		this.scaleFrequency = frequency;
		this.scaleDuration = time;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000A32FC File Offset: 0x000A14FC
	private void ScootPositionLogic(float deltaTime)
	{
		if (this.scootTimer <= 0f && this.prevScootTimer <= 0f && this.scootPositionCurrent != Vector2.zero)
		{
			this.scootPositionCurrent = Vector2.LerpUnclamped(this.scootPosition, Vector2.zero, this.scootEval);
			if (this.scootEval >= 1f)
			{
				this.scootPositionCurrent = Vector2.zero;
				this.scootAnimationEvaluation = 0f;
				this.scootEval = 0f;
			}
			else
			{
				this.scootAnimationEvaluation += 4f * deltaTime;
				this.scootAnimationEvaluation = Mathf.Clamp01(this.scootAnimationEvaluation);
				this.scootEval = this.animationCurveInOut.Evaluate(this.scootAnimationEvaluation);
			}
		}
		if (this.scootTimer > 0f && this.prevScootTimer > 0f)
		{
			this.stopScootingTimer = 0.1f;
			if (this.scootPositionCurrent != this.scootPosition)
			{
				this.scootPositionCurrent = Vector2.LerpUnclamped(Vector2.zero, this.scootPosition, this.scootEval);
				if (this.scootEval >= 1f)
				{
					this.scootPositionCurrent = this.scootPosition;
					this.scootAnimationEvaluation = 0f;
					this.scootEval = 0f;
					return;
				}
				this.scootAnimationEvaluation += 4f * deltaTime;
				this.scootAnimationEvaluation = Mathf.Clamp01(this.scootAnimationEvaluation);
				this.scootEval = this.animationCurveInOut.Evaluate(this.scootAnimationEvaluation);
			}
		}
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000A3490 File Offset: 0x000A1690
	private void UpdatePositionLogic()
	{
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localPosition = this.hidePositionCurrent + this.scootPositionCurrent + new Vector2(this.SpringShakeX, this.SpringShakeY);
			return;
		}
		base.transform.localPosition = this.hidePositionCurrent + this.scootPositionCurrent + new Vector2(this.SpringShakeX, this.SpringShakeY);
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x000A3514 File Offset: 0x000A1714
	private void SpringScaleLogic(float deltaTime)
	{
		if (this.scaleTime < this.scaleDuration)
		{
			float num = this.CalculateSpringOffset(this.scaleTime, this.scaleAmount, this.scaleFrequency, this.scaleDuration);
			Vector3 vector = this.originalScale * (1f + num);
			vector = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
			if (!this.animateTheEntireObject)
			{
				this.textRectTransform.localScale = vector;
			}
			else
			{
				base.transform.localScale = vector;
			}
			this.scaleTime += deltaTime;
			return;
		}
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localScale = this.originalScale;
			return;
		}
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x000A35E8 File Offset: 0x000A17E8
	private void HideAnimationLogic(float deltaTime)
	{
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			if (this.showTimer <= 0f && this.prevShowTimer <= 0f)
			{
				this.animationEval = 0f;
				this.showAnimationEvaluation = 0f;
				this.hideAnimationEvaluation = 0f;
			}
			this.showTimer = 0.1f;
		}
		if (this.showTimer > 0f && this.prevShowTimer > 0f)
		{
			this.stopShowingTimer = 0.1f;
			if (this.hidePositionCurrent != this.showPosition)
			{
				this.hidePositionCurrent = Vector2.LerpUnclamped(this.hidePosition + this.scootPositionCurrent, this.showPosition, this.animationEval);
				if (this.showAnimationEvaluation >= 1f)
				{
					this.hidePositionCurrent = this.showPosition;
					this.showAnimationEvaluation = 0f;
					this.animationEval = 0f;
				}
				else
				{
					this.showAnimationEvaluation += 4f * deltaTime;
					this.showAnimationEvaluation = Mathf.Clamp01(this.showAnimationEvaluation);
					this.animationEval = this.animationCurveWooshIn.Evaluate(this.showAnimationEvaluation);
				}
			}
		}
		if (this.hideTimer > 0f && this.prevHideTimer > 0f && this.showTimer <= 0f && this.prevShowTimer <= 0f)
		{
			this.stopHidingTimer = 0.1f;
			if (this.hidePositionCurrent != this.hidePosition)
			{
				this.hidePositionCurrent = Vector2.LerpUnclamped(this.showPosition, this.hidePosition, this.animationEval);
				if (this.hideAnimationEvaluation >= 1f)
				{
					this.hidePositionCurrent = this.hidePosition;
					this.hideAnimationEvaluation = 0f;
					this.animationEval = 0f;
					return;
				}
				this.hideAnimationEvaluation += 4f * deltaTime;
				this.hideAnimationEvaluation = Mathf.Clamp01(this.hideAnimationEvaluation);
				this.animationEval = this.animationCurveWooshAway.Evaluate(this.hideAnimationEvaluation);
			}
		}
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x000A3818 File Offset: 0x000A1A18
	private void HideTimer(float deltaTime)
	{
		if (this.showTimer > 0f && this.prevShowTimer > 0f && this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.AllChildrenSetActive(true);
				}
			}
			else
			{
				this.AllChildrenSetActive(true);
			}
			this.hideTimer = 0f;
			return;
		}
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f && this.stopHidingTimer <= 0f && this.prevStopHidingTimer <= 0f && this.hideAnimationEvaluation == 0f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.AllChildrenSetActive(true);
				}
			}
			else
			{
				this.AllChildrenSetActive(true);
			}
		}
		if (this.hideTimer > 0f && this.hideAnimationEvaluation >= 1f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && this.uiText.enabled)
				{
					this.uiText.enabled = false;
					this.AllChildrenSetActive(false);
					return;
				}
			}
			else
			{
				this.AllChildrenSetActive(false);
			}
		}
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x000A397C File Offset: 0x000A1B7C
	public void SemiUIResetAllShakeEffects()
	{
		this.shakeTimeX = 0f;
		this.shakeTimeY = 0f;
		this.shakeAmountX = 0f;
		this.shakeAmountY = 0f;
		this.shakeFrequencyX = 0f;
		this.shakeFrequencyY = 0f;
		this.shakeDurationX = 0f;
		this.shakeDurationY = 0f;
		this.SpringShakeX = 0f;
		this.SpringShakeY = 0f;
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000A39F8 File Offset: 0x000A1BF8
	private void FlashColorLogic(float deltaTime)
	{
		if (!this.uiText)
		{
			return;
		}
		if (this.flashColorTime > 0f)
		{
			this.flashColorTime -= deltaTime;
			this.uiText.color = this.flashColor;
			this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.flashColor);
			this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.flashColor);
			if (this.flashColorTime <= 0f)
			{
				this.uiText.color = this.originalTextColor;
				this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.originalFontColor);
				this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.originalGlowColor);
			}
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x000A3ACC File Offset: 0x000A1CCC
	public void SemiUISpringShakeY(float amount, float frequency, float time)
	{
		this.shakeTimeY = 0f;
		this.shakeAmountY = amount;
		this.shakeFrequencyY = frequency;
		this.shakeDurationY = time;
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x000A3AEE File Offset: 0x000A1CEE
	public void SemiUISpringShakeX(float amount, float frequency, float time)
	{
		this.shakeTimeX = 0f;
		this.shakeAmountX = amount;
		this.shakeFrequencyX = frequency;
		this.shakeDurationX = time;
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x000A3B10 File Offset: 0x000A1D10
	public void SemiUITextFlashColor(Color color, float time)
	{
		this.flashColor = color;
		this.flashColorTime = time;
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000A3B20 File Offset: 0x000A1D20
	private void SpringShakeLogic(float deltaTime)
	{
		float num = 0f;
		float num2 = 0f;
		if (this.shakeTimeX < this.shakeDurationX)
		{
			num = this.CalculateSpringOffset(this.shakeTimeX, this.shakeAmountX, this.shakeFrequencyX, this.shakeDurationX);
			this.SpringShakeX = num;
			this.shakeTimeX += deltaTime;
		}
		if (this.shakeTimeY < this.shakeDurationY)
		{
			num2 = this.CalculateSpringOffset(this.shakeTimeY, this.shakeAmountY, this.shakeFrequencyY, this.shakeDurationY);
			this.SpringShakeY = num2;
			this.shakeTimeY += deltaTime;
		}
		base.transform.localPosition = this.initialPosition + new Vector3(num, num2, 0f);
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000A3BE0 File Offset: 0x000A1DE0
	private float CalculateSpringOffset(float currentTime, float amount, float frequency, float duration)
	{
		float num = currentTime / duration;
		float num2 = frequency * (1f - num);
		return amount * Mathf.Sin(num2 * num * 3.1415927f * 2f) * (1f - num);
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x000A3C1C File Offset: 0x000A1E1C
	public void Hide()
	{
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			this.hideAnimationEvaluation = 0f;
			this.showAnimationEvaluation = 0f;
			this.animationEval = 0f;
			if (!this.animateTheEntireObject && this.uiText && !this.uiText.enabled)
			{
				this.uiText.enabled = false;
				this.AllChildrenSetActive(false);
			}
			this.hidePositionCurrent = this.showPosition;
			if (!this.animateTheEntireObject)
			{
				this.textRectTransform.localPosition = this.hidePositionCurrent;
			}
			else
			{
				base.transform.localPosition = this.hidePositionCurrent;
			}
		}
		this.hideTimer = 0.1f;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000A3CEC File Offset: 0x000A1EEC
	public void Show()
	{
		if (this.showTimer <= 0f && this.prevShowTimer <= 0f)
		{
			this.showAnimationEvaluation = 0f;
			this.hideAnimationEvaluation = 0f;
			this.animationEval = 0f;
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.AllChildrenSetActive(true);
				}
			}
			else
			{
				this.AllChildrenSetActive(true);
			}
			this.hidePositionCurrent = this.hidePosition;
			if (!this.animateTheEntireObject)
			{
				if (this.textRectTransform)
				{
					this.textRectTransform.localPosition = this.hidePositionCurrent;
				}
			}
			else
			{
				base.transform.localPosition = this.hidePositionCurrent;
			}
		}
		this.showTimer = 0.1f;
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x000A3DD4 File Offset: 0x000A1FD4
	public void SemiUIScoot(Vector2 position)
	{
		this.scootPosition = position;
		if ((this.scootTimer <= 0f && this.prevScootTimer <= 0f) || this.scootPositionPrev != this.scootPosition)
		{
			this.scootEval = 0f;
			this.scootAnimationEvaluation = 0f;
			this.scootPositionPrev = this.scootPosition;
		}
		this.scootTimer = 0.2f;
	}

	// Token: 0x04001F7D RID: 8061
	internal Vector3 initialPosition;

	// Token: 0x04001F7E RID: 8062
	private float shakeTimeX;

	// Token: 0x04001F7F RID: 8063
	private float shakeTimeY;

	// Token: 0x04001F80 RID: 8064
	private float shakeAmountX;

	// Token: 0x04001F81 RID: 8065
	private float shakeAmountY;

	// Token: 0x04001F82 RID: 8066
	private float shakeFrequencyX;

	// Token: 0x04001F83 RID: 8067
	private float shakeFrequencyY;

	// Token: 0x04001F84 RID: 8068
	private float shakeDurationX;

	// Token: 0x04001F85 RID: 8069
	private float shakeDurationY;

	// Token: 0x04001F86 RID: 8070
	public bool animateTheEntireObject;

	// Token: 0x04001F87 RID: 8071
	[HideInInspector]
	public TextMeshProUGUI uiText;

	// Token: 0x04001F88 RID: 8072
	private Color originalTextColor;

	// Token: 0x04001F89 RID: 8073
	private Color originalFontColor;

	// Token: 0x04001F8A RID: 8074
	private Color originalGlowColor;

	// Token: 0x04001F8B RID: 8075
	private Color flashColor;

	// Token: 0x04001F8C RID: 8076
	private float flashColorTime;

	// Token: 0x04001F8D RID: 8077
	private Material textMaterial;

	// Token: 0x04001F8E RID: 8078
	internal float hideTimer;

	// Token: 0x04001F8F RID: 8079
	internal float showTimer;

	// Token: 0x04001F90 RID: 8080
	private bool uiTextEnabledPrevious;

	// Token: 0x04001F91 RID: 8081
	private float scaleTime;

	// Token: 0x04001F92 RID: 8082
	private float scaleAmount;

	// Token: 0x04001F93 RID: 8083
	private float scaleFrequency;

	// Token: 0x04001F94 RID: 8084
	private float scaleDuration;

	// Token: 0x04001F95 RID: 8085
	private Vector3 originalScale;

	// Token: 0x04001F96 RID: 8086
	[HideInInspector]
	public Transform textRectTransform;

	// Token: 0x04001F97 RID: 8087
	private AnimationCurve animationCurveWooshAway;

	// Token: 0x04001F98 RID: 8088
	private AnimationCurve animationCurveWooshIn;

	// Token: 0x04001F99 RID: 8089
	private AnimationCurve animationCurveInOut;

	// Token: 0x04001F9A RID: 8090
	private float hideAnimationEvaluation;

	// Token: 0x04001F9B RID: 8091
	private float showAnimationEvaluation;

	// Token: 0x04001F9C RID: 8092
	public Vector2 hidePosition = new Vector2(0f, 0f);

	// Token: 0x04001F9D RID: 8093
	[HideInInspector]
	public Vector2 showPosition = new Vector2(0f, 0f);

	// Token: 0x04001F9E RID: 8094
	private Vector2 hidePositionCurrent = new Vector2(0f, 0f);

	// Token: 0x04001F9F RID: 8095
	private bool initialized;

	// Token: 0x04001FA0 RID: 8096
	private Vector2 scootPosition = new Vector2(0f, 0f);

	// Token: 0x04001FA1 RID: 8097
	private float scootTimer = -123f;

	// Token: 0x04001FA2 RID: 8098
	private Vector2 scootPositionPrev = new Vector2(0f, 0f);

	// Token: 0x04001FA3 RID: 8099
	private float scootAnimationEvaluation;

	// Token: 0x04001FA4 RID: 8100
	private Vector2 originalScootPosition = new Vector2(0f, 0f);

	// Token: 0x04001FA5 RID: 8101
	private Vector2 scootPositionCurrent = new Vector2(0f, 0f);

	// Token: 0x04001FA6 RID: 8102
	private List<GameObject> allChildren = new List<GameObject>();

	// Token: 0x04001FA7 RID: 8103
	private float SpringShakeX;

	// Token: 0x04001FA8 RID: 8104
	private float SpringShakeY;

	// Token: 0x04001FA9 RID: 8105
	private float stopScootingTimer;

	// Token: 0x04001FAA RID: 8106
	private float stopHidingTimer;

	// Token: 0x04001FAB RID: 8107
	private float stopShowingTimer;

	// Token: 0x04001FAC RID: 8108
	private float prevShowTimer;

	// Token: 0x04001FAD RID: 8109
	private float prevHideTimer;

	// Token: 0x04001FAE RID: 8110
	private float prevScootTimer;

	// Token: 0x04001FAF RID: 8111
	private float animationEval;

	// Token: 0x04001FB0 RID: 8112
	private float prevStopHidingTimer;

	// Token: 0x04001FB1 RID: 8113
	private float prevStopShowingTimer;

	// Token: 0x04001FB2 RID: 8114
	private float scootEval;
}
