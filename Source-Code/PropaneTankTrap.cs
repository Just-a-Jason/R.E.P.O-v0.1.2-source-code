using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000277 RID: 631
public class PropaneTankTrap : Trap
{
	// Token: 0x0600138B RID: 5003 RVA: 0x000AAFC8 File Offset: 0x000A91C8
	protected override void Start()
	{
		base.Start();
		this.initialTankRotation = this.Tank.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x000AB028 File Offset: 0x000A9228
	protected override void Update()
	{
		base.Update();
		this.FlyLoop.PlayLoop(this.LoopPlaying, 0.8f, 0.8f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 15f;
			this.LoopPlaying = true;
			this.hurtCollider.gameObject.SetActive(true);
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Tank.transform.localRotation = this.initialTankRotation * Quaternion.Euler(num3, 0f, z);
			this.Tank.transform.localPosition = new Vector3(this.Tank.transform.localPosition.x, this.Tank.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Tank.transform.localPosition.z);
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x000AB158 File Offset: 0x000A9358
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			this.rb.AddForce(-base.transform.forward * 0.3f * 40f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddForce(Vector3.up * 0.1f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddTorque(-base.transform.right * 0.1f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			if (this.timeToTwist > 200)
			{
				this.randomTorque = Random.insideUnitSphere.normalized * Random.Range(0f, 0.5f);
				this.timeToTwist = 0;
				if (this.rb.velocity.magnitude < 0.5f && !this.physgrabobject.grabbed)
				{
					this.rb.AddForce(base.transform.forward * 5f, ForceMode.Impulse);
					this.rb.AddTorque(this.randomTorque * 20f, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x000AB2E0 File Offset: 0x000A94E0
	public void TrapStop()
	{
		this.trapActive = false;
		this.flyEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.LoopPlaying = false;
		this.hurtCollider.gameObject.SetActive(false);
		this.DeparentAndDestroy(this.smokeParticleSystem);
		this.DeparentAndDestroy(this.fireParticleSystem);
		this.fireLight.SetActive(false);
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x000AB35C File Offset: 0x000A955C
	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		if (particleSystem != null && particleSystem.isPlaying)
		{
			particleSystem.gameObject.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			particleSystem.Stop(false);
			particleSystem = null;
		}
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x000AB3A4 File Offset: 0x000A95A4
	public void IncrementTimeToTwist()
	{
		this.timeToTwist++;
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x000AB3B4 File Offset: 0x000A95B4
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.tankTimer.Invoke();
			this.fireParticleSystem.Play(false);
			this.fireLight.SetActive(true);
			this.flyStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x000AB428 File Offset: 0x000A9628
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 0.8f, 50, 100, 1f, false, false, 1f);
		this.DeparentAndDestroy(this.smokeParticleSystem);
		this.DeparentAndDestroy(this.fireParticleSystem);
	}

	// Token: 0x0400215C RID: 8540
	public UnityEvent tankTimer;

	// Token: 0x0400215D RID: 8541
	private PhysGrabObject physgrabobject;

	// Token: 0x0400215E RID: 8542
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x0400215F RID: 8543
	[Space]
	public GameObject Tank;

	// Token: 0x04002160 RID: 8544
	public Transform Center;

	// Token: 0x04002161 RID: 8545
	[Space]
	[Header("Sounds")]
	public Sound Pop;

	// Token: 0x04002162 RID: 8546
	public Sound FlyLoop;

	// Token: 0x04002163 RID: 8547
	public Sound flyStart;

	// Token: 0x04002164 RID: 8548
	public Sound flyEnd;

	// Token: 0x04002165 RID: 8549
	[Space]
	private Quaternion initialTankRotation;

	// Token: 0x04002166 RID: 8550
	private Rigidbody rb;

	// Token: 0x04002167 RID: 8551
	private bool LoopPlaying;

	// Token: 0x04002168 RID: 8552
	private Vector3 randomTorque;

	// Token: 0x04002169 RID: 8553
	private int timeToTwist;

	// Token: 0x0400216A RID: 8554
	public HurtCollider hurtCollider;

	// Token: 0x0400216B RID: 8555
	public ParticleSystem smokeParticleSystem;

	// Token: 0x0400216C RID: 8556
	public ParticleSystem fireParticleSystem;

	// Token: 0x0400216D RID: 8557
	public GameObject fireLight;
}
