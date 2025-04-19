using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200027B RID: 635
public class ClownTrap : Trap
{
	// Token: 0x060013A3 RID: 5027 RVA: 0x000AB9EC File Offset: 0x000A9BEC
	protected override void Start()
	{
		base.Start();
		this.initialClownRotation = this.Body.transform.localRotation;
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.noseMesh = this.Nose.GetComponent<MeshRenderer>();
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x000ABA40 File Offset: 0x000A9C40
	protected override void Update()
	{
		base.Update();
		if (this.trapActive)
		{
			this.enemyInvestigateRange = 15f;
			if (this.HeadSpinOneShotActive)
			{
				if (this.HeadSpinLerp == 0f)
				{
					this.HeadSpin.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
					this.enemyInvestigate = true;
				}
				this.HeadSpinLerp += this.HeadSpinSpeed * Time.deltaTime;
				if (this.HeadSpinLerp >= 1f)
				{
					this.HeadSpinLerp = 0f;
					if (this.WarningCount > 0)
					{
						this.WarningCount--;
						int warningCount = this.WarningCount;
						if (warningCount != 0)
						{
							if (warningCount == 1)
							{
								this.previousAudioSource = this.WarningVO1.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
							}
						}
						else
						{
							if (this.previousAudioSource != null)
							{
								this.previousAudioSource.Stop();
							}
							this.previousAudioSource = this.WarningVO2.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
						}
						this.HeadSpinOneShotActive = false;
						this.HeadSpinDoneLogic();
					}
				}
			}
			this.Head.transform.localEulerAngles = new Vector3(0f, this.HeadSpinCurve.Evaluate(this.HeadSpinLerp) * this.HeadSpinIntensity, 0f);
			if (this.ArmRaiseActive)
			{
				if (this.ArmRaiseLerp == 0f)
				{
					this.ArmRaise.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				}
				this.ArmRaiseLerp += this.ArmRaiseSpeed * Time.deltaTime;
				if (this.ArmRaiseLerp >= 1f)
				{
					this.ArmRaiseLerp = 0f;
					this.HeadSpinOneShotActive = true;
					if (this.WarningCount > 0)
					{
						this.ArmRaiseActive = false;
					}
				}
			}
			this.Arms.transform.localEulerAngles = new Vector3(0f, 0f, this.ArmRaiseCurve.Evaluate(this.ArmRaiseLerp) * this.ArmRaiseIntensity);
			if (this.NoseSqueezeActive)
			{
				this.NoseSqueezeLerp += this.NoseSqueezeSpeed * Time.deltaTime;
				this.enemyInvestigate = true;
				if (this.NoseSqueezeLerp >= 1f)
				{
					this.NoseSqueezeLerp = 0f;
					if (!this.CountDownActive)
					{
						this.noseMesh.material.DisableKeyword("_EMISSION");
					}
					this.NoseSqueezeActive = false;
				}
			}
			this.Nose.transform.localScale = new Vector3(1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity, 1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity, 1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity);
			if (this.CountDownActive)
			{
				float num = (float)(this.ExplosionCountDown / 50);
				float num2 = (float)(this.ExplosionCountDown / 10);
				float num3 = num * Mathf.Sin(Time.time * num2);
				float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
				this.Body.transform.localRotation = this.initialClownRotation * Quaternion.Euler(0f, num3, z);
				this.Body.transform.localPosition = new Vector3(this.Body.transform.localPosition.x, this.Body.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Body.transform.localPosition.z);
			}
		}
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x000ABE2F File Offset: 0x000AA02F
	private void FixedUpdate()
	{
		if (this.CountDownActive)
		{
			this.ExplosionCountDown++;
		}
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x000ABE48 File Offset: 0x000AA048
	public void TrapStop()
	{
		this.trapActive = false;
		if (this.previousAudioSource != null)
		{
			this.previousAudioSource.Stop();
		}
		this.particleScriptExplosion.Spawn(this.Center.position, 1.5f, 100, 300, 1f, false, false, 1f);
		this.physgrabobject.dead = true;
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x000ABEB0 File Offset: 0x000AA0B0
	private void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.NoseSqeak.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.noseMesh.material.EnableKeyword("_EMISSION");
			this.NoseSqueezeActive = true;
			this.ArmRaiseActive = true;
			this.trapActive = true;
			this.trapTriggered = true;
			if (this.WarningCount <= 0)
			{
				if (this.previousAudioSource != null)
				{
					this.previousAudioSource.Stop();
				}
				this.previousAudioSource = this.GonnaBlowVO.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				this.ArmRaiseActive = true;
				this.noseMesh.material.EnableKeyword("_EMISSION");
				this.clownTimer.Invoke();
				this.CountDownActive = true;
			}
		}
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x000ABFA4 File Offset: 0x000AA1A4
	private void TouchNoseLogic()
	{
		this.TrapActivate();
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x000ABFAC File Offset: 0x000AA1AC
	public void TouchNose()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.TouchNoseLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("TouchNoseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x000ABFDE File Offset: 0x000AA1DE
	[PunRPC]
	private void TouchNoseRPC()
	{
		this.TouchNoseLogic();
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x000ABFE6 File Offset: 0x000AA1E6
	private void HeadSpinDoneLogic()
	{
		this.trapTriggered = false;
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x000ABFEF File Offset: 0x000AA1EF
	private void HeadSpinDone()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.HeadSpinDoneLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("HeadSpinDoneRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x000AC021 File Offset: 0x000AA221
	[PunRPC]
	private void HeadSpinDoneRPC()
	{
		this.HeadSpinDoneLogic();
	}

	// Token: 0x04002180 RID: 8576
	public UnityEvent clownTimer;

	// Token: 0x04002181 RID: 8577
	public Transform Center;

	// Token: 0x04002182 RID: 8578
	private PhysGrabObject physgrabobject;

	// Token: 0x04002183 RID: 8579
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002184 RID: 8580
	private MeshRenderer noseMesh;

	// Token: 0x04002185 RID: 8581
	[Space]
	[Header("Clown Components")]
	public GameObject Body;

	// Token: 0x04002186 RID: 8582
	public GameObject Arms;

	// Token: 0x04002187 RID: 8583
	public GameObject Head;

	// Token: 0x04002188 RID: 8584
	public GameObject Nose;

	// Token: 0x04002189 RID: 8585
	[Space]
	[Header("Sounds")]
	public Sound NoseSqeak;

	// Token: 0x0400218A RID: 8586
	public Sound HeadSpin;

	// Token: 0x0400218B RID: 8587
	public Sound ArmRaise;

	// Token: 0x0400218C RID: 8588
	public Sound WarningVO1;

	// Token: 0x0400218D RID: 8589
	public Sound WarningVO2;

	// Token: 0x0400218E RID: 8590
	private AudioSource previousAudioSource;

	// Token: 0x0400218F RID: 8591
	public Sound GonnaBlowVO;

	// Token: 0x04002190 RID: 8592
	[Space]
	private Quaternion initialClownRotation;

	// Token: 0x04002191 RID: 8593
	private int WarningCount = 2;

	// Token: 0x04002192 RID: 8594
	private int ExplosionCountDown;

	// Token: 0x04002193 RID: 8595
	private bool CountDownActive;

	// Token: 0x04002194 RID: 8596
	[Space]
	[Header("Head Spin Animation")]
	public AnimationCurve HeadSpinCurve;

	// Token: 0x04002195 RID: 8597
	private bool HeadSpinOneShotActive;

	// Token: 0x04002196 RID: 8598
	public float HeadSpinSpeed;

	// Token: 0x04002197 RID: 8599
	public float HeadSpinIntensity;

	// Token: 0x04002198 RID: 8600
	private float HeadSpinLerp;

	// Token: 0x04002199 RID: 8601
	[Space]
	[Header("Arm raise Animation")]
	public AnimationCurve ArmRaiseCurve;

	// Token: 0x0400219A RID: 8602
	private bool ArmRaiseActive;

	// Token: 0x0400219B RID: 8603
	public float ArmRaiseSpeed;

	// Token: 0x0400219C RID: 8604
	public float ArmRaiseIntensity;

	// Token: 0x0400219D RID: 8605
	private float ArmRaiseLerp;

	// Token: 0x0400219E RID: 8606
	[Space]
	[Header("Nose Squeeze Animation")]
	public AnimationCurve NoseSqueezeCurve;

	// Token: 0x0400219F RID: 8607
	private bool NoseSqueezeActive;

	// Token: 0x040021A0 RID: 8608
	public float NoseSqueezeSpeed;

	// Token: 0x040021A1 RID: 8609
	public float NoseSqueezeIntensity;

	// Token: 0x040021A2 RID: 8610
	private float NoseSqueezeLerp;
}
