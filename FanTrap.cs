using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200027C RID: 636
public class FanTrap : Trap
{
	// Token: 0x060013AF RID: 5039 RVA: 0x000AC038 File Offset: 0x000AA238
	protected override void Start()
	{
		base.Start();
		this.hurtCollider.gameObject.SetActive(false);
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.initialPlayerHitForce = this.hurtCollider.playerHitForce;
		this.initialPhysHitForce = this.hurtCollider.physHitForce;
		this.initialEnemyHitForce = this.hurtCollider.enemyHitForce;
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x000AC0A7 File Offset: 0x000AA2A7
	private void FixedUpdate()
	{
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x000AC0AC File Offset: 0x000AA2AC
	protected override void Update()
	{
		base.Update();
		FanTrap.States states = this.currentState;
		if (states != FanTrap.States.Idle)
		{
			if (states == FanTrap.States.Active)
			{
				this.StateActive();
			}
		}
		else
		{
			this.StateIdle();
		}
		this.hurtCollider.gameObject.SetActive(this.currentState == FanTrap.States.Active);
		this.sfxFanLoop.PlayLoop(this.currentState == FanTrap.States.Active, 0.1f, 0.025f, 1f);
		this.sfxFanLoop.LoopPitch = Mathf.Lerp(0.1f, 1f, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.playerHitForce = Mathf.Lerp(0f, this.initialPlayerHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.physHitForce = Mathf.Lerp(0f, this.initialPhysHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.hurtCollider.enemyHitForce = Mathf.Lerp(0f, this.initialEnemyHitForce, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
		this.fanBlades.Rotate(-Vector3.forward * this.fanBladeSpeed * Time.deltaTime);
		this.fanBladeSpeed = Mathf.Lerp(0f, this.fanBladeMaxSpeed, this.fanBladeSpeedCurve.Evaluate(this.fanBladeLerp));
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x000AC218 File Offset: 0x000AA418
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.sfxButtonOn.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.windParticles.Play();
			this.windSmallParticles.Play();
			this.buttonMesh.material.EnableKeyword("_EMISSION");
			this.fanTimer.Invoke();
			this.stateStart = false;
		}
		this.enemyInvestigate = true;
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.physgrabobject.grabbed)
		{
			this.SetState(FanTrap.States.Idle);
		}
		this.fanButton.localEulerAngles = new Vector3(21f, 0f, 0f);
		if (this.fanBladeLerp < 1f)
		{
			this.fanBladeLerp += Time.deltaTime / this.secondsToStart;
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x000AC2FC File Offset: 0x000AA4FC
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.sfxButtonOff.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.windParticles.Stop();
			this.windSmallParticles.Stop();
			this.buttonMesh.material.DisableKeyword("_EMISSION");
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physgrabobject.grabbed)
		{
			this.SetState(FanTrap.States.Active);
		}
		this.fanButton.localEulerAngles = new Vector3(0f, 0f, 0f);
		if (this.fanBladeLerp > 0f)
		{
			this.fanBladeLerp -= Time.deltaTime / this.secondsToStop;
		}
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x000AC3CE File Offset: 0x000AA5CE
	[PunRPC]
	public void SetStateRPC(FanTrap.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x000AC3DE File Offset: 0x000AA5DE
	private void SetState(FanTrap.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state);
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x040021A3 RID: 8611
	public UnityEvent fanTimer;

	// Token: 0x040021A4 RID: 8612
	public Transform fanButton;

	// Token: 0x040021A5 RID: 8613
	public HurtCollider hurtCollider;

	// Token: 0x040021A6 RID: 8614
	public MeshRenderer buttonMesh;

	// Token: 0x040021A7 RID: 8615
	private float initialPlayerHitForce;

	// Token: 0x040021A8 RID: 8616
	private float initialPhysHitForce;

	// Token: 0x040021A9 RID: 8617
	private float initialEnemyHitForce;

	// Token: 0x040021AA RID: 8618
	[Header("Fan Blade")]
	public Transform fanBlades;

	// Token: 0x040021AB RID: 8619
	public AnimationCurve fanBladeSpeedCurve;

	// Token: 0x040021AC RID: 8620
	private PhysGrabObject physgrabobject;

	// Token: 0x040021AD RID: 8621
	private float fanBladeSpeed;

	// Token: 0x040021AE RID: 8622
	private float fanBladeMaxSpeed = 1500f;

	// Token: 0x040021AF RID: 8623
	private float fanBladeLerp;

	// Token: 0x040021B0 RID: 8624
	private float secondsToStart = 2f;

	// Token: 0x040021B1 RID: 8625
	private float secondsToStop = 4f;

	// Token: 0x040021B2 RID: 8626
	internal FanTrap.States currentState;

	// Token: 0x040021B3 RID: 8627
	private bool stateStart;

	// Token: 0x040021B4 RID: 8628
	[Header("Sounds")]
	public Sound sfxButtonOn;

	// Token: 0x040021B5 RID: 8629
	public Sound sfxButtonOff;

	// Token: 0x040021B6 RID: 8630
	public Sound sfxFanLoop;

	// Token: 0x040021B7 RID: 8631
	[Header("Particles")]
	public ParticleSystem windParticles;

	// Token: 0x040021B8 RID: 8632
	public ParticleSystem windSmallParticles;

	// Token: 0x020003C0 RID: 960
	public enum States
	{
		// Token: 0x040028D6 RID: 10454
		Idle,
		// Token: 0x040028D7 RID: 10455
		Active
	}
}
