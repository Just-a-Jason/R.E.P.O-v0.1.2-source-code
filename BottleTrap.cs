using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000279 RID: 633
public class BottleTrap : Trap
{
	// Token: 0x06001397 RID: 5015 RVA: 0x000AB48D File Offset: 0x000A968D
	protected override void Start()
	{
		base.Start();
		this.initialBottleRotation = this.Bottle.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x000AB4C4 File Offset: 0x000A96C4
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
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Bottle.transform.localRotation = this.initialBottleRotation * Quaternion.Euler(num3, 0f, z);
			this.Bottle.transform.localPosition = new Vector3(this.Bottle.transform.localPosition.x, this.Bottle.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Bottle.transform.localPosition.z);
		}
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x000AB5E0 File Offset: 0x000A97E0
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			this.rb.AddForce(-base.transform.up * 0.45f * 40f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddForce(Vector3.up * 0.15f * 10f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			this.rb.AddTorque(this.randomTorque * 30f * Time.fixedDeltaTime * 50f, ForceMode.Force);
			if (this.timeToTwist > 200)
			{
				this.randomTorque = Random.insideUnitSphere.normalized * Random.Range(0f, 0.5f);
				this.timeToTwist = 0;
				if (this.rb.velocity.magnitude < 0.5f && !this.physgrabobject.grabbed)
				{
					this.rb.AddForce(base.transform.up * 5f, ForceMode.Impulse);
					this.rb.AddTorque(this.randomTorque * 20f, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x000AB752 File Offset: 0x000A9952
	public void TrapStop()
	{
		this.trapActive = false;
		this.LoopPlaying = false;
		this.DeparentAndDestroy(this.bottleParticleSystem);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x000AB770 File Offset: 0x000A9970
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

	// Token: 0x0600139C RID: 5020 RVA: 0x000AB7B8 File Offset: 0x000A99B8
	public void IncrementTimeToTwist()
	{
		this.timeToTwist++;
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x000AB7C8 File Offset: 0x000A99C8
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.Pop.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.bottleTimer.Invoke();
			this.Cork.SetActive(false);
			this.corkParticleSystem.Play(false);
			this.bottleParticleSystem.Play(false);
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x000AB848 File Offset: 0x000A9A48
	private void OnDestroy()
	{
		if (!this.bottleParticleSystem)
		{
			return;
		}
		this.bottleParticleSystem.transform.parent = null;
		this.bottleParticleSystem.Stop(true);
		this.bottleParticleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
	}

	// Token: 0x0400216E RID: 8558
	public UnityEvent bottleTimer;

	// Token: 0x0400216F RID: 8559
	private PhysGrabObject physgrabobject;

	// Token: 0x04002170 RID: 8560
	[Space]
	[Header("Bottle Components")]
	public GameObject Bottle;

	// Token: 0x04002171 RID: 8561
	public GameObject Cork;

	// Token: 0x04002172 RID: 8562
	[Space]
	[Header("Sounds")]
	public Sound Pop;

	// Token: 0x04002173 RID: 8563
	public Sound FlyLoop;

	// Token: 0x04002174 RID: 8564
	[Space]
	private Quaternion initialBottleRotation;

	// Token: 0x04002175 RID: 8565
	private Rigidbody rb;

	// Token: 0x04002176 RID: 8566
	private bool LoopPlaying;

	// Token: 0x04002177 RID: 8567
	private Vector3 randomTorque;

	// Token: 0x04002178 RID: 8568
	private int timeToTwist;

	// Token: 0x04002179 RID: 8569
	public ParticleSystem bottleParticleSystem;

	// Token: 0x0400217A RID: 8570
	public ParticleSystem corkParticleSystem;
}
