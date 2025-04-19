using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200022A RID: 554
public class GrandfatherClockTrap : Trap
{
	// Token: 0x060011CB RID: 4555 RVA: 0x0009DB1C File Offset: 0x0009BD1C
	protected override void Start()
	{
		base.Start();
		this.ticVolume = this.Tic.Volume;
		this.tocVolume = this.Toc.Volume;
		this.bellVolume = this.Bell.Volume;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0009DB70 File Offset: 0x0009BD70
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.GrandfatherClockActivate();
		}
		this.Tic.Volume = this.ticVolume * this.masterVolume;
		this.Toc.Volume = this.tocVolume * this.masterVolume;
		this.Bell.Volume = this.bellVolume * this.masterVolume;
		if (!this.angleLerpRev)
		{
			this.angleLerp += Time.deltaTime + this.pendulumSpeed * Time.deltaTime;
			if (this.angleLerp > 1f)
			{
				this.angleLerpRev = true;
			}
		}
		else
		{
			this.angleLerp -= Time.deltaTime + this.pendulumSpeed * Time.deltaTime;
			if (this.angleLerp < 0f)
			{
				this.angleLerpRev = false;
			}
		}
		float y = Mathf.Lerp(-this.angle, this.angle, this.angleCurve.Evaluate(this.angleLerp));
		this.pendulum.localEulerAngles = new Vector3(0f, y, 0f);
		if (this.angleLerp >= 1f - this.offset && !this.ticPlayed)
		{
			this.Tic.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.ticPlayed = true;
			this.tocPlayed = false;
		}
		if (this.angleLerp <= 0f + this.offset && !this.tocPlayed)
		{
			this.Toc.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.tocPlayed = true;
			this.ticPlayed = false;
		}
		if (this.trapTriggered)
		{
			this.pendulumSpeedLerp += Time.deltaTime / 15f;
			this.pendulumSpeed = Mathf.Lerp(this.pendulumSpeedMin, this.pendulumSpeedMax, this.pendulumSpeedCurve.Evaluate(this.pendulumSpeedLerp));
		}
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0009DD7C File Offset: 0x0009BF7C
	public void GrandfatherClockBell()
	{
		if (this.bellRingCount < 3)
		{
			this.Bell.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 60f;
			this.bellRingCount++;
			this.bellRingTimer.Invoke();
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 12f, this.pendulum.position, 0.2f);
		}
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0009DE1C File Offset: 0x0009C01C
	public void GrandfatherClockActivate()
	{
		if (!this.trapTriggered)
		{
			this.Bell.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.trapTriggered = true;
			this.bellRingTimer.Invoke();
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 12f, this.pendulum.position, 0.2f);
		}
	}

	// Token: 0x04001DE4 RID: 7652
	public UnityEvent bellRingTimer;

	// Token: 0x04001DE5 RID: 7653
	[Header("Trap Activated Animation")]
	[Header("Pendulum")]
	public Transform pendulum;

	// Token: 0x04001DE6 RID: 7654
	private float angle = 6.25f;

	// Token: 0x04001DE7 RID: 7655
	private bool angleLerpRev;

	// Token: 0x04001DE8 RID: 7656
	private float angleLerp;

	// Token: 0x04001DE9 RID: 7657
	public AnimationCurve angleCurve;

	// Token: 0x04001DEA RID: 7658
	private float pendulumSpeedMin;

	// Token: 0x04001DEB RID: 7659
	public float pendulumSpeedMax = 5f;

	// Token: 0x04001DEC RID: 7660
	private float pendulumSpeed;

	// Token: 0x04001DED RID: 7661
	public AnimationCurve pendulumSpeedCurve;

	// Token: 0x04001DEE RID: 7662
	private float pendulumSpeedLerp;

	// Token: 0x04001DEF RID: 7663
	private float offset = 0.49f;

	// Token: 0x04001DF0 RID: 7664
	private bool ticPlayed;

	// Token: 0x04001DF1 RID: 7665
	private bool tocPlayed;

	// Token: 0x04001DF2 RID: 7666
	[Header("Sounds")]
	public Sound Tic;

	// Token: 0x04001DF3 RID: 7667
	private float ticVolume;

	// Token: 0x04001DF4 RID: 7668
	public Sound Toc;

	// Token: 0x04001DF5 RID: 7669
	private float tocVolume;

	// Token: 0x04001DF6 RID: 7670
	public Sound Bell;

	// Token: 0x04001DF7 RID: 7671
	private float bellVolume;

	// Token: 0x04001DF8 RID: 7672
	private int bellRingCount;

	// Token: 0x04001DF9 RID: 7673
	private float masterVolume = 1f;

	// Token: 0x04001DFA RID: 7674
	private Rigidbody rb;
}
