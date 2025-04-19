using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000276 RID: 630
public class IceSawValuable : Trap
{
	// Token: 0x0600137F RID: 4991 RVA: 0x000AA9FC File Offset: 0x000A8BFC
	protected override void Start()
	{
		base.Start();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
		this.initialTankRotation = this.meshTransform.transform.localRotation;
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x000AAA58 File Offset: 0x000A8C58
	private void FixedUpdate()
	{
		IceSawValuable.States states = this.currentState;
		if (states != IceSawValuable.States.Idle)
		{
			if (states == IceSawValuable.States.Active)
			{
				this.StateActive();
				return;
			}
		}
		else
		{
			this.StateIdle();
		}
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x000AAA80 File Offset: 0x000A8C80
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		this.soundBladeLoop.PlayLoop(this.loopPlaying, 5f, 5f, 1f);
		this.blade.Rotate(-Vector3.right * this.bladeSpeed * Time.deltaTime);
		this.bladeSpeed = Mathf.Lerp(0f, this.bladeMaxSpeed, this.bladeCurve.Evaluate(this.bladeLerp));
		float num = 0.3f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num2 = 60f * this.bladeCurve.Evaluate(this.bladeLerp);
		float num3 = num * Mathf.Sin(Time.time * num2);
		float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
		this.meshTransform.transform.localRotation = this.initialTankRotation * Quaternion.Euler(num3, 0f, z);
		this.meshTransform.transform.localPosition = new Vector3(this.meshTransform.transform.localPosition.x, this.meshTransform.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.meshTransform.transform.localPosition.z);
		if (this.currentState == IceSawValuable.States.Active)
		{
			if (this.bladeLerp < 1f)
			{
				this.bladeLerp += Time.deltaTime / this.secondsToStart;
				return;
			}
			this.bladeLerp = 1f;
			return;
		}
		else
		{
			if (this.bladeLerp > 0f)
			{
				this.bladeLerp -= Time.deltaTime / this.secondsToStop;
				return;
			}
			this.bladeLerp = 0f;
			return;
		}
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x000AAC5C File Offset: 0x000A8E5C
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.hurtCollider.gameObject.SetActive(true);
			this.soundBladeStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.loopPlaying = true;
			this.stateStart = false;
		}
		this.overLapBoxCheckTimer += Time.deltaTime;
		if (this.overLapBoxCheckTimer >= 0.1f)
		{
			Vector3 a = this.triggerCollider.bounds.size * 0.5f;
			a.x *= Mathf.Abs(base.transform.lossyScale.x);
			a.y *= Mathf.Abs(base.transform.lossyScale.y);
			a.z *= Mathf.Abs(base.transform.lossyScale.z);
			if (Physics.OverlapBox(this.triggerCollider.bounds.center, a / 2f, this.triggerCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Default"
			}), QueryTriggerInteraction.Collide).Length != 0)
			{
				this.Sparks();
			}
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 15f;
			this.overLapBoxCheckTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.AddTorque(base.transform.up * 1f * Time.fixedDeltaTime * 30f, ForceMode.Force);
			if (Random.Range(0, 100) < 7)
			{
				this.rb.AddForce(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
				this.rb.AddTorque(Random.insideUnitSphere * 0.1f, ForceMode.Impulse);
			}
			if (!this.physgrabobject.grabbed && !this.trapActive)
			{
				this.SetState(IceSawValuable.States.Idle);
			}
		}
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x000AAE64 File Offset: 0x000A9064
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.hurtCollider.gameObject.SetActive(false);
			this.loopPlaying = false;
			this.soundBladeEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && (this.physgrabobject.grabbed || this.trapActive))
		{
			this.SetState(IceSawValuable.States.Active);
		}
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x000AAEE6 File Offset: 0x000A90E6
	[PunRPC]
	public void SetStateRPC(IceSawValuable.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x000AAEF6 File Offset: 0x000A90F6
	private void SetState(IceSawValuable.States state)
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

	// Token: 0x06001386 RID: 4998 RVA: 0x000AAF2F File Offset: 0x000A912F
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.sawTimer.Invoke();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x000AAF52 File Offset: 0x000A9152
	public void TrapStop()
	{
		this.trapActive = false;
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x000AAF5B File Offset: 0x000A915B
	public void Sparks()
	{
		this.sparkParticles.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.rb.AddForce(base.transform.right * 2f, ForceMode.Impulse);
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x000AAF90 File Offset: 0x000A9190
	public void ImpactDamage()
	{
		this.physGrabObject.lightBreakImpulse = true;
	}

	// Token: 0x04002145 RID: 8517
	public Sound soundBladeLoop;

	// Token: 0x04002146 RID: 8518
	public Sound soundBladeStart;

	// Token: 0x04002147 RID: 8519
	public Sound soundBladeEnd;

	// Token: 0x04002148 RID: 8520
	[Space]
	public Transform meshTransform;

	// Token: 0x04002149 RID: 8521
	public UnityEvent sawTimer;

	// Token: 0x0400214A RID: 8522
	public Transform blade;

	// Token: 0x0400214B RID: 8523
	public AnimationCurve bladeCurve;

	// Token: 0x0400214C RID: 8524
	public float bladeSpeed;

	// Token: 0x0400214D RID: 8525
	private float bladeMaxSpeed = 1500f;

	// Token: 0x0400214E RID: 8526
	private float bladeLerp;

	// Token: 0x0400214F RID: 8527
	private float secondsToStart = 2f;

	// Token: 0x04002150 RID: 8528
	private float secondsToStop = 2f;

	// Token: 0x04002151 RID: 8529
	public HurtCollider hurtCollider;

	// Token: 0x04002152 RID: 8530
	public Collider triggerCollider;

	// Token: 0x04002153 RID: 8531
	private float overLapBoxCheckTimer;

	// Token: 0x04002154 RID: 8532
	public ParticleSystem sparkParticles;

	// Token: 0x04002155 RID: 8533
	internal IceSawValuable.States currentState;

	// Token: 0x04002156 RID: 8534
	private bool stateStart;

	// Token: 0x04002157 RID: 8535
	[Space]
	private Quaternion initialTankRotation;

	// Token: 0x04002158 RID: 8536
	private Animator animator;

	// Token: 0x04002159 RID: 8537
	private PhysGrabObject physgrabobject;

	// Token: 0x0400215A RID: 8538
	private Rigidbody rb;

	// Token: 0x0400215B RID: 8539
	private bool loopPlaying;

	// Token: 0x020003BF RID: 959
	public enum States
	{
		// Token: 0x040028D3 RID: 10451
		Idle,
		// Token: 0x040028D4 RID: 10452
		Active
	}
}
