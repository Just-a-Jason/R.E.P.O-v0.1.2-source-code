using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200022F RID: 559
public class TrapRadio : Trap
{
	// Token: 0x060011E4 RID: 4580 RVA: 0x0009EC64 File Offset: 0x0009CE64
	protected override void Start()
	{
		base.Start();
		this.initialRadioRotation = this.Radio.transform.localRotation;
		this.initialRadioMeterRotation = this.RadioMeter.localRotation;
		this.RadioLight.enabled = false;
		this.RadioDisplay.enabled = false;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0009ECB8 File Offset: 0x0009CEB8
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.RadioTrapActivated();
		}
		this.RadioLoop.PlayLoop(this.RadioPlaying, 2f, 2f, 1f);
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			if (this.RadioFlickerIntro || this.RadioFlickerOutro)
			{
				float num = this.RadioFlickerCurve.Evaluate(this.RadioFlickerTimer / this.RadioFlickerTime);
				this.RadioFlickerTimer += 1f * Time.deltaTime;
				if (num > 0.5f)
				{
					this.RadioLight.enabled = true;
					this.RadioDisplay.enabled = true;
				}
				else
				{
					this.RadioLight.enabled = false;
					this.RadioDisplay.enabled = false;
				}
				if (this.RadioFlickerTimer > this.RadioFlickerTime)
				{
					this.RadioFlickerIntro = false;
					this.RadioFlickerTimer = 0f;
					if (this.RadioFlickerOutro)
					{
						this.RadioLight.enabled = false;
						this.RadioDisplay.enabled = false;
					}
					else
					{
						this.RadioLight.enabled = true;
						this.RadioDisplay.enabled = true;
					}
				}
			}
			this.RadioPlaying = true;
			float num2 = 1f;
			if (this.StartSequenceProgress == 0f && !this.StartSequence)
			{
				this.StartSequence = true;
				this.RadioStart.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.RadioLight.enabled = true;
				this.RadioDisplay.enabled = true;
			}
			if (this.StartSequence)
			{
				num2 += this.RadioStartCurve.Evaluate(this.StartSequenceProgress) * this.RadioStartIntensity;
				this.StartSequenceProgress += Time.deltaTime / this.RadioStartDuration;
				if (this.StartSequenceProgress >= 1f)
				{
					this.StartSequence = false;
				}
			}
			if (this.endSequence)
			{
				num2 += this.RadioEndCurve.Evaluate(this.EndSequenceProgress) * this.RadioEndIntensity;
				this.EndSequenceProgress += Time.deltaTime / this.RadioEndDuration;
				if (this.EndSequenceProgress >= 1f)
				{
					this.EndSequenceDone();
				}
			}
			float num3 = 1f * num2;
			float num4 = 40f;
			float num5 = num3 * Mathf.Sin(Time.time * num4);
			float z = num3 * Mathf.Sin(Time.time * num4 + 1.5707964f);
			this.Radio.transform.localRotation = this.initialRadioRotation * Quaternion.Euler(num5, 0f, z);
			num4 = 20f;
			float num6 = num3 * Mathf.Sin(Time.time * num4);
			this.RadioMeter.localRotation = this.initialRadioMeterRotation * Quaternion.Euler(0f, num6 * 90f, 0f);
			this.Radio.transform.localPosition = new Vector3(this.Radio.transform.localPosition.x, this.Radio.transform.localPosition.y - num5 * 0.005f * Time.deltaTime, this.Radio.transform.localPosition.z);
		}
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0009EFE8 File Offset: 0x0009D1E8
	private void EndSequenceDone()
	{
		this.endSequence = false;
		this.RadioLight.enabled = false;
		this.RadioDisplay.enabled = false;
		this.RadioPlaying = false;
		this.trapActive = false;
		this.Radio.transform.localRotation = this.initialRadioRotation;
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0009F038 File Offset: 0x0009D238
	public void RadioTrapStop()
	{
		this.RadioEnd.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.endSequence = true;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0009F06C File Offset: 0x0009D26C
	public void RadioTrapActivated()
	{
		if (!this.trapTriggered)
		{
			this.radioTimer.Invoke();
			this.trapTriggered = true;
			this.trapActive = true;
		}
	}

	// Token: 0x04001E20 RID: 7712
	public UnityEvent radioTimer;

	// Token: 0x04001E21 RID: 7713
	public MeshRenderer RadioDisplay;

	// Token: 0x04001E22 RID: 7714
	public Light RadioLight;

	// Token: 0x04001E23 RID: 7715
	public Transform RadioMeter;

	// Token: 0x04001E24 RID: 7716
	public AnimationCurve RadioFlickerCurve;

	// Token: 0x04001E25 RID: 7717
	public float RadioFlickerTime = 0.5f;

	// Token: 0x04001E26 RID: 7718
	private float RadioFlickerTimer;

	// Token: 0x04001E27 RID: 7719
	private bool RadioFlickerIntro = true;

	// Token: 0x04001E28 RID: 7720
	private bool RadioFlickerOutro;

	// Token: 0x04001E29 RID: 7721
	[Space]
	[Header("Gramophone Components")]
	public GameObject Radio;

	// Token: 0x04001E2A RID: 7722
	[Space]
	[Header("Sounds")]
	public Sound RadioStart;

	// Token: 0x04001E2B RID: 7723
	public Sound RadioEnd;

	// Token: 0x04001E2C RID: 7724
	public Sound RadioLoop;

	// Token: 0x04001E2D RID: 7725
	[Space]
	[Header("Radio Animation")]
	public AnimationCurve RadioStartCurve;

	// Token: 0x04001E2E RID: 7726
	public float RadioStartIntensity;

	// Token: 0x04001E2F RID: 7727
	public float RadioStartDuration;

	// Token: 0x04001E30 RID: 7728
	[Space]
	public AnimationCurve RadioEndCurve;

	// Token: 0x04001E31 RID: 7729
	public float RadioEndIntensity;

	// Token: 0x04001E32 RID: 7730
	public float RadioEndDuration;

	// Token: 0x04001E33 RID: 7731
	private bool StartSequence;

	// Token: 0x04001E34 RID: 7732
	private bool endSequence;

	// Token: 0x04001E35 RID: 7733
	private float StartSequenceProgress;

	// Token: 0x04001E36 RID: 7734
	private float EndSequenceProgress;

	// Token: 0x04001E37 RID: 7735
	private bool RadioPlaying;

	// Token: 0x04001E38 RID: 7736
	private Quaternion initialRadioRotation;

	// Token: 0x04001E39 RID: 7737
	private Quaternion initialRadioMeterRotation;
}
