using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x02000263 RID: 611
public class TutorialUI : SemiUI
{
	// Token: 0x06001309 RID: 4873 RVA: 0x000A677C File Offset: 0x000A497C
	protected override void Start()
	{
		this.uiText = this.Text;
		base.Start();
		TutorialUI.instance = this;
		this.videoPlayer.clip = this.staticVideo;
		this.dummyTextTransform.gameObject.SetActive(false);
		this.dummyTextTimer = 30f;
		this.videoTransform.gameObject.SetActive(false);
		this.videoPlayer.gameObject.SetActive(false);
		base.transform.localScale = new Vector3(0f, 1f, 1f);
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x000A6810 File Offset: 0x000A4A10
	public void TutorialText(string message)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.messageTimer = 0.2f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x000A6884 File Offset: 0x000A4A84
	public void SetPage(VideoClip video, string text, string dummyTextString, bool transition = true)
	{
		base.SemiUISpringShakeY(10f, 8f, 0.5f);
		if (transition)
		{
			this.Text.text = "Good job! <sprite name=creepycrying>";
			this.videoPlayer.clip = this.staticVideo;
			this.nextVideo = video;
			this.nextText = text;
		}
		else
		{
			this.Text.text = text;
			this.videoPlayer.clip = video;
			this.nextVideo = video;
			this.nextText = text;
		}
		this.videoPlayer.Play();
		this.videoTransform.transform.localScale = new Vector3(1f, 1f, 1f);
		this.videoImage.color = new Color(1f, 1f, 1f, 1f);
		this.bigVideoTimer = 7f;
		this.currentDummyText = dummyTextString;
		this.dummyText.text = dummyTextString;
		this.dummyTextTimer = 30f;
		this.dummyTextAnimationEval = 0f;
		this.dummyTextTransform.gameObject.SetActive(false);
		base.StartCoroutine(this.SwitchPage());
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x000A69A8 File Offset: 0x000A4BA8
	public void SetTipPage(VideoClip video, string text)
	{
		this.videoTransform.gameObject.SetActive(true);
		this.videoPlayer.gameObject.SetActive(true);
		this.videoPlayer.clip = video;
		this.Text.text = text;
		this.videoPlayer.time = 0.0;
		this.videoPlayer.Play();
		this.videoTransform.transform.localScale = new Vector3(1f, 1f, 1f);
		this.videoImage.color = new Color(1f, 1f, 1f, 1f);
		this.bigVideoTimer = 6f;
		this.currentDummyText = "";
		this.dummyText.text = "";
		this.dummyTextTimer = 30f;
		this.dummyTextAnimationEval = 0f;
		this.dummyTextTransform.gameObject.SetActive(false);
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x000A6AA3 File Offset: 0x000A4CA3
	private IEnumerator SwitchPage()
	{
		yield return new WaitForSeconds(2f);
		if (this.videoPlayer.clip != this.nextVideo)
		{
			base.SemiUISpringShakeY(10f, 8f, 0.5f);
		}
		this.videoPlayer.clip = this.nextVideo;
		this.videoPlayer.Play();
		this.Text.text = this.nextText;
		yield break;
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x000A6AB4 File Offset: 0x000A4CB4
	protected override void Update()
	{
		base.Update();
		if (this.hideTimer <= 0f || this.showTimer > 0f)
		{
			this.videoTransform.gameObject.SetActive(true);
			this.videoPlayer.gameObject.SetActive(true);
			this.hideAllTimer = 2f;
			if (this.dummyTextTimer <= 0f)
			{
				if (this.currentDummyText != "" && !this.dummyTextTransform.gameObject.activeSelf)
				{
					this.dummyTextTransform.gameObject.SetActive(true);
					this.dummyText.text = this.currentDummyText;
					this.dummyTextAnimationEval = 0f;
					this.bigVideoTimer = 1f;
				}
				if (this.dummyTextAnimationEval < 1f)
				{
					this.dummyTextAnimationEval += Time.deltaTime * 3f;
					this.dummyTextAnimationEval = Mathf.Clamp01(this.dummyTextAnimationEval);
					float t = this.scaleInCurve.Evaluate(this.dummyTextAnimationEval);
					this.dummyTextTransform.localPosition = new Vector3(this.dummyTextTransform.localPosition.x, Mathf.LerpUnclamped(-20f, 20f, t), this.dummyTextTransform.localPosition.z);
				}
			}
			else
			{
				this.dummyTextTimer -= Time.deltaTime;
			}
			if (!SemiFunc.RunIsTutorial())
			{
				if (this.bigVideoTimer > 0f)
				{
					this.bigVideoTimer -= Time.deltaTime;
					float b = 1f;
					this.videoTransform.transform.localScale = new Vector3(Mathf.Lerp(this.videoTransform.transform.localScale.x, b, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.y, b, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.z, b, Time.deltaTime * 20f));
					float b2 = 1f;
					this.videoImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(this.videoImage.color.a, b2, Time.deltaTime * 20f));
				}
				else
				{
					float b3 = 0.7f;
					this.videoTransform.transform.localScale = new Vector3(Mathf.Lerp(this.videoTransform.transform.localScale.x, b3, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.y, b3, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.z, b3, Time.deltaTime * 20f));
					float b4 = 0.5f;
					this.videoImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(this.videoImage.color.a, b4, Time.deltaTime * 20f));
				}
			}
			this.progressBarTarget = TutorialDirector.instance.tutorialProgress;
			this.animationCurveEval += Time.deltaTime * 3f;
			this.animationCurveEval = Mathf.Clamp(this.animationCurveEval, 0f, 1f);
			float y = this.scaleInCurve.Evaluate(this.animationCurveEval);
			base.transform.localScale = new Vector3(1f, y, 1f);
			this.progressBarCurrent = this.progressBar.localScale.x;
			this.progressBar.localScale = new Vector3(Mathf.Lerp(this.progressBar.localScale.x, this.progressBarTarget, Time.deltaTime * 20f), 1f, 1f);
			if (this.currentDummyText == "" || this.dummyTextTimer > 0f)
			{
				this.dummyTextTransform.gameObject.SetActive(false);
			}
			return;
		}
		if (this.hideAllTimer > 0f)
		{
			this.hideAllTimer -= Time.deltaTime;
			return;
		}
		this.dummyTextTransform.gameObject.SetActive(false);
		this.dummyTextTimer = 30f;
		this.videoTransform.gameObject.SetActive(false);
		this.videoPlayer.gameObject.SetActive(false);
	}

	// Token: 0x0400202A RID: 8234
	public TextMeshProUGUI Text;

	// Token: 0x0400202B RID: 8235
	public Transform progressBar;

	// Token: 0x0400202C RID: 8236
	public AnimationCurve scaleInCurve;

	// Token: 0x0400202D RID: 8237
	public static TutorialUI instance;

	// Token: 0x0400202E RID: 8238
	private string messagePrev = "prev";

	// Token: 0x0400202F RID: 8239
	private Color bigMessageColor = Color.white;

	// Token: 0x04002030 RID: 8240
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x04002031 RID: 8241
	private float messageTimer;

	// Token: 0x04002032 RID: 8242
	private float progressBarTarget;

	// Token: 0x04002033 RID: 8243
	internal float progressBarCurrent;

	// Token: 0x04002034 RID: 8244
	[HideInInspector]
	public float animationCurveEval;

	// Token: 0x04002035 RID: 8245
	public VideoPlayer videoPlayer;

	// Token: 0x04002036 RID: 8246
	public VideoClip staticVideo;

	// Token: 0x04002037 RID: 8247
	public VideoClip nextVideo;

	// Token: 0x04002038 RID: 8248
	private string nextText;

	// Token: 0x04002039 RID: 8249
	private float bigVideoTimer = 5f;

	// Token: 0x0400203A RID: 8250
	public Transform videoTransform;

	// Token: 0x0400203B RID: 8251
	public RawImage videoImage;

	// Token: 0x0400203C RID: 8252
	public TextMeshProUGUI dummyText;

	// Token: 0x0400203D RID: 8253
	public TextMeshProUGUI dummyTextExclamation;

	// Token: 0x0400203E RID: 8254
	public Transform dummyTextTransform;

	// Token: 0x0400203F RID: 8255
	private float dummyTextAnimationEval;

	// Token: 0x04002040 RID: 8256
	private float dummyTextTimer = 30f;

	// Token: 0x04002041 RID: 8257
	private string currentDummyText;

	// Token: 0x04002042 RID: 8258
	private float hideAllTimer;
}
