using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000229 RID: 553
public class TrapGramophone : Trap
{
	// Token: 0x060011C5 RID: 4549 RVA: 0x0009D77C File Offset: 0x0009B97C
	protected override void Start()
	{
		base.Start();
		this.initialGramophoneRotation = this.Gramophone.transform.localRotation;
		this.randomRange = base.GetComponent<SyncedEventRandom>();
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x0009D7A8 File Offset: 0x0009B9A8
	protected override void Update()
	{
		base.Update();
		this.GramophoneMusic.PlayLoop(this.MusicPlaying, 0.5f, 0.5f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.MusicStart && !this.MusicStartPointFetched)
		{
			this.randomRange.RandomRangeFloat(0f, this.GramophoneMusic.Source.clip.length);
			if (this.randomRange.resultReceivedRandomRangeFloat)
			{
				this.MusicStartPointFetched = true;
				this.GramophoneMusic.StartTimeOverride = this.randomRange.resultRandomRangeFloat;
				this.MusicPlaying = true;
			}
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.GramophoneRecord.transform.Rotate(0f, 20f * Time.deltaTime, 0f);
			this.GramophoneCrank.transform.Rotate(0f, 0f, 20f * Time.deltaTime);
			float num = 1f;
			if (this.StartSequenceProgress == 0f && !this.StartSequence)
			{
				this.StartSequence = true;
				this.GramophoneStart.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.GramophoneMusic.LoopPitch = 0.2f;
			}
			if (this.StartSequence)
			{
				num += this.GramophoneStartCurve.Evaluate(this.StartSequenceProgress) * this.GramophoneStartIntensity;
				this.GramophoneMusic.LoopPitch = 1f - this.GramophoneStartCurve.Evaluate(this.StartSequenceProgress);
				this.StartSequenceProgress += Time.deltaTime / this.GramophoneStartDuration;
				if (this.StartSequenceProgress >= 1f)
				{
					this.StartSequence = false;
				}
			}
			if (this.endSequence)
			{
				num += this.GramophoneEndCurve.Evaluate(this.EndSequenceProgress) * this.GramophoneEndIntensity;
				this.EndSequenceProgress += Time.deltaTime / this.GramophoneEndDuration;
				this.GramophoneMusic.LoopPitch -= 0.5f * Time.deltaTime;
				if (this.EndSequenceProgress >= 1f)
				{
					this.EndSequenceDone();
				}
			}
			float num2 = 1f * num;
			float num3 = 40f;
			float num4 = num2 * Mathf.Sin(Time.time * num3);
			float z = num2 * Mathf.Sin(Time.time * num3 + 1.5707964f);
			this.Gramophone.transform.localRotation = this.initialGramophoneRotation * Quaternion.Euler(num4, 0f, z);
			this.Gramophone.transform.localPosition = new Vector3(this.Gramophone.transform.localPosition.x, this.Gramophone.transform.localPosition.y - num4 * 0.005f * Time.deltaTime, this.Gramophone.transform.localPosition.z);
		}
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x0009DA9F File Offset: 0x0009BC9F
	private void EndSequenceDone()
	{
		this.trapActive = false;
		this.endSequence = false;
		this.MusicPlaying = false;
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0009DAB6 File Offset: 0x0009BCB6
	public void TrapStop()
	{
		this.endSequence = true;
		this.GramophoneEnd.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0009DAEA File Offset: 0x0009BCEA
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.MusicStart = true;
			this.gramophoneTimer.Invoke();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x04001DCE RID: 7630
	public UnityEvent gramophoneTimer;

	// Token: 0x04001DCF RID: 7631
	[Space]
	[Header("Gramophone Components")]
	public GameObject Gramophone;

	// Token: 0x04001DD0 RID: 7632
	public GameObject GramophoneRecord;

	// Token: 0x04001DD1 RID: 7633
	public GameObject GramophoneCrank;

	// Token: 0x04001DD2 RID: 7634
	[Space]
	[Header("Sounds")]
	public Sound GramophoneStart;

	// Token: 0x04001DD3 RID: 7635
	public Sound GramophoneEnd;

	// Token: 0x04001DD4 RID: 7636
	public Sound GramophoneMusic;

	// Token: 0x04001DD5 RID: 7637
	[Space]
	[Header("Gramophone Animation")]
	public AnimationCurve GramophoneStartCurve;

	// Token: 0x04001DD6 RID: 7638
	public float GramophoneStartIntensity;

	// Token: 0x04001DD7 RID: 7639
	public float GramophoneStartDuration;

	// Token: 0x04001DD8 RID: 7640
	[Space]
	public AnimationCurve GramophoneEndCurve;

	// Token: 0x04001DD9 RID: 7641
	public float GramophoneEndIntensity;

	// Token: 0x04001DDA RID: 7642
	public float GramophoneEndDuration;

	// Token: 0x04001DDB RID: 7643
	private bool StartSequence;

	// Token: 0x04001DDC RID: 7644
	private bool endSequence;

	// Token: 0x04001DDD RID: 7645
	private float StartSequenceProgress;

	// Token: 0x04001DDE RID: 7646
	private float EndSequenceProgress;

	// Token: 0x04001DDF RID: 7647
	private bool MusicPlaying;

	// Token: 0x04001DE0 RID: 7648
	private bool MusicStartPointFetched;

	// Token: 0x04001DE1 RID: 7649
	private bool MusicStart;

	// Token: 0x04001DE2 RID: 7650
	private SyncedEventRandom randomRange;

	// Token: 0x04001DE3 RID: 7651
	private Quaternion initialGramophoneRotation;
}
