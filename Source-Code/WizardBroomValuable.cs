using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000283 RID: 643
public class WizardBroomValuable : Trap
{
	// Token: 0x060013DB RID: 5083 RVA: 0x000AD1E4 File Offset: 0x000AB3E4
	private bool CheckForwardRaycast(out RaycastHit _hit)
	{
		_hit = default(RaycastHit);
		if (this.raycastTimer < this.raycastCooldown)
		{
			return false;
		}
		this.raycastTimer = 0f;
		Vector3 vector = this.broom.transform.TransformPoint(this.rayOffset);
		Vector3 vector2 = -this.broom.transform.right;
		Debug.DrawRay(vector, vector2, Color.red);
		return Physics.Raycast(vector, vector2, out _hit, this.rayDistance, this.visionObstruct);
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x000AD263 File Offset: 0x000AB463
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.visionObstruct = SemiFunc.LayerMaskGetVisionObstruct();
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x000AD28E File Offset: 0x000AB48E
	protected override void Update()
	{
		base.Update();
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x000AD298 File Offset: 0x000AB498
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (this.currentState)
			{
			case WizardBroomValuable.States.Idle:
				this.StateIdle();
				break;
			case WizardBroomValuable.States.MoveForward:
				this.StateMoveForward();
				break;
			case WizardBroomValuable.States.Turn:
				this.StateTurn();
				break;
			case WizardBroomValuable.States.Unstick:
				this.StateUnstick();
				break;
			case WizardBroomValuable.States.grabbed:
				this.StateGrabbed();
				break;
			case WizardBroomValuable.States.Sleep:
				this.StateSleep();
				break;
			}
			if (this.stuckTimer > 0f)
			{
				this.stuckTimer -= Time.fixedDeltaTime;
			}
			if (this.physGrabObject.grabbed && this.trapActive)
			{
				this.SetState(WizardBroomValuable.States.grabbed);
			}
			if (this.impactDetector.inCart && this.trapTriggered)
			{
				this.TrapStop();
			}
		}
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x000AD35C File Offset: 0x000AB55C
	private void StateMoveForward()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.raycastTimer += Time.fixedDeltaTime;
		this.physGrabObject.OverrideZeroGravity(0.1f);
		if (this.stuckTimer <= 0f)
		{
			if (this.rb.velocity.magnitude > 0.1f)
			{
				this.stuckTimer = Random.Range(0.1f, 2f);
				return;
			}
			this.SetState(WizardBroomValuable.States.Unstick);
		}
		RaycastHit raycastHit;
		if (this.CheckForwardRaycast(out raycastHit) || Vector3.Dot(this.broom.transform.right, Vector3.up) > 0.1f)
		{
			this.SetState(WizardBroomValuable.States.Turn);
			return;
		}
		this.rb.AddForce(-this.broom.transform.right * 2000f * Time.fixedDeltaTime, ForceMode.Force);
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x000AD448 File Offset: 0x000AB648
	private void StateTurn()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.rb.AddForce(Vector3.up * 50f * Time.fixedDeltaTime, ForceMode.Impulse);
		}
		this.raycastTimer += Time.fixedDeltaTime;
		this.physGrabObject.OverrideZeroGravity(0.1f);
		RaycastHit raycastHit;
		if (this.CheckForwardRaycast(out raycastHit))
		{
			Vector3 a = Vector3.Cross(raycastHit.normal, this.broom.transform.right);
			this.rb.AddTorque(a * 200f * Time.fixedDeltaTime, ForceMode.Force);
			this.rb.AddForce(this.broom.transform.right * 500f * Time.fixedDeltaTime, ForceMode.Force);
			return;
		}
		this.SetState(WizardBroomValuable.States.MoveForward);
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x000AD52C File Offset: 0x000AB72C
	private void StateUnstick()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.unStickTimer = Random.Range(1f, 2f);
			this.randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			this.rb.AddForce(this.randomDirection * 500f * Time.fixedDeltaTime, ForceMode.Impulse);
			this.randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		}
		if (this.unStickTimer > 0f)
		{
			this.rb.AddTorque(this.randomTorque * 5000f * Time.fixedDeltaTime, ForceMode.Force);
			this.unStickTimer -= Time.fixedDeltaTime;
			return;
		}
		this.stuckTimer = Random.Range(0.1f, 2f);
		this.SetState(WizardBroomValuable.States.MoveForward);
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x000AD668 File Offset: 0x000AB868
	private void StateIdle()
	{
		this.stateStart = false;
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x000AD674 File Offset: 0x000AB874
	private void StateGrabbed()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.physGrabObject.OverrideZeroGravity(0.1f);
		this.physGrabObject.OverrideDrag(0.2f, 0.1f);
		this.physGrabObject.OverrideAngularDrag(0.2f, 0.1f);
		if (!this.physGrabObject.grabbed)
		{
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x000AD6DE File Offset: 0x000AB8DE
	private void StateSleep()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (!this.impactDetector.inCart && this.trapTriggered)
		{
			this.trapActive = true;
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x000AD714 File Offset: 0x000AB914
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.broomBoxBreakSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.plankParticles.Play();
			this.bitParticles.Play();
			this.box.SetActive(false);
			this.broom.SetActive(true);
			this.trapActive = true;
			this.trapTriggered = true;
			this.SetState(WizardBroomValuable.States.MoveForward);
		}
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x000AD797 File Offset: 0x000AB997
	public void TrapStop()
	{
		this.trapActive = false;
		this.SetState(WizardBroomValuable.States.Sleep);
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x000AD7A7 File Offset: 0x000AB9A7
	private void SetState(WizardBroomValuable.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x040021EC RID: 8684
	public UnityEvent broomTimer;

	// Token: 0x040021ED RID: 8685
	private Rigidbody rb;

	// Token: 0x040021EE RID: 8686
	private LayerMask visionObstruct;

	// Token: 0x040021EF RID: 8687
	private Vector3 rayOffset = new Vector3(-1.92f, 0f, 0f);

	// Token: 0x040021F0 RID: 8688
	private float rayDistance = 1f;

	// Token: 0x040021F1 RID: 8689
	private float raycastCooldown = 0.2f;

	// Token: 0x040021F2 RID: 8690
	private float raycastTimer;

	// Token: 0x040021F3 RID: 8691
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040021F4 RID: 8692
	public GameObject box;

	// Token: 0x040021F5 RID: 8693
	public GameObject broom;

	// Token: 0x040021F6 RID: 8694
	public ParticleSystem plankParticles;

	// Token: 0x040021F7 RID: 8695
	public ParticleSystem bitParticles;

	// Token: 0x040021F8 RID: 8696
	public Sound broomBoxBreakSound;

	// Token: 0x040021F9 RID: 8697
	private float stuckTimer = 2f;

	// Token: 0x040021FA RID: 8698
	private float unStickTimer;

	// Token: 0x040021FB RID: 8699
	private Vector3 randomDirection;

	// Token: 0x040021FC RID: 8700
	private Vector3 randomTorque;

	// Token: 0x040021FD RID: 8701
	public WizardBroomValuable.States currentState;

	// Token: 0x040021FE RID: 8702
	private bool stateStart;

	// Token: 0x020003C2 RID: 962
	public enum States
	{
		// Token: 0x040028DC RID: 10460
		Idle,
		// Token: 0x040028DD RID: 10461
		MoveForward,
		// Token: 0x040028DE RID: 10462
		Turn,
		// Token: 0x040028DF RID: 10463
		Unstick,
		// Token: 0x040028E0 RID: 10464
		grabbed,
		// Token: 0x040028E1 RID: 10465
		Sleep
	}
}
