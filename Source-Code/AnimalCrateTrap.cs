using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class AnimalCrateTrap : Trap
{
	// Token: 0x06001357 RID: 4951 RVA: 0x000A9C68 File Offset: 0x000A7E68
	protected override void Start()
	{
		base.Start();
		this.initialAnimalCrateRotation = this.Crate.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.material = this.Crate.GetComponent<Renderer>().material;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x000A9CCB File Offset: 0x000A7ECB
	protected override void Update()
	{
		base.Update();
		this.berzerk.PlayLoop(this.poundActive, 1f, 1f, 1f);
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x000A9CF4 File Offset: 0x000A7EF4
	private void FixedUpdate()
	{
		if (this.poundActive)
		{
			if (this.shakeFrequencyMultiplier > 5f)
			{
				this.material.EnableKeyword("_EMISSION");
			}
			this.enemyInvestigate = true;
			GameDirector.instance.CameraImpact.ShakeDistance(0.75f * this.poundIntensityMuilplier, 1f, 6f, base.transform.position, 0.01f * this.poundIntensityMuilplier);
			float num = this.shakeAmplitudeMultiplier * (1f - this.poundLerp);
			float num2 = this.shakeFrequencyMultiplier * (1f - this.poundLerp);
			float x = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Crate.transform.localRotation = this.initialAnimalCrateRotation * Quaternion.Euler(x, 0f, z);
			this.poundLerp += this.poundSpeed / this.poundIntensityMuilplier * Time.deltaTime;
			if (this.poundLerp >= 1f)
			{
				this.poundLerp = 0f;
				this.material.DisableKeyword("_EMISSION");
				this.poundActive = false;
			}
			this.Crate.transform.localScale = new Vector3(1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier), 1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier), 1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier));
		}
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
		{
			if (this.timeToPound > 0f)
			{
				this.timeToPound -= Time.deltaTime;
				return;
			}
			this.poundActive = true;
			this.BigBump();
			this.timeToPound = 1f;
			this.physgrabobject.lightBreakImpulse = true;
		}
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000A9F10 File Offset: 0x000A8110
	public void TinyBump()
	{
		this.particleBitsTiny.Play();
		this.tinyBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 0.2f;
		this.shakeFrequencyMultiplier = 5f;
		this.poundIntensityMuilplier = 1.5f;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.05f, 0.2f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 2f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x000A9FD0 File Offset: 0x000A81D0
	public void SmallBump()
	{
		this.particleBitsSmall.Play();
		this.smallBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 0.5f;
		this.shakeFrequencyMultiplier = 10f;
		this.poundIntensityMuilplier = 2f;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.1f, 0.5f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 3f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x000AA090 File Offset: 0x000A8290
	public void MediumBump()
	{
		this.particleBitsMedium.Play();
		this.mediumBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 1f;
		this.shakeFrequencyMultiplier = 20f;
		this.poundIntensityMuilplier = 3f;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.3f, 1f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 5f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x000AA150 File Offset: 0x000A8350
	public void BigBump()
	{
		this.particleBitsBig.Play();
		this.bigBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 2f;
		this.shakeFrequencyMultiplier = 20f;
		this.poundIntensityMuilplier = 4f;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.8f, 2f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 6f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x000AA210 File Offset: 0x000A8410
	public void TrapStop()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 300, 1f, false, false, 1f);
	}

	// Token: 0x0400210D RID: 8461
	private PhysGrabObject physgrabobject;

	// Token: 0x0400210E RID: 8462
	[Space]
	[Header("Animal Crate Components")]
	public GameObject Crate;

	// Token: 0x0400210F RID: 8463
	public Transform Center;

	// Token: 0x04002110 RID: 8464
	private Material material;

	// Token: 0x04002111 RID: 8465
	[Space]
	[Header("Sounds")]
	public Sound tinyBump;

	// Token: 0x04002112 RID: 8466
	public Sound smallBump;

	// Token: 0x04002113 RID: 8467
	public Sound mediumBump;

	// Token: 0x04002114 RID: 8468
	public Sound bigBump;

	// Token: 0x04002115 RID: 8469
	public Sound berzerk;

	// Token: 0x04002116 RID: 8470
	[Space]
	private Quaternion initialAnimalCrateRotation;

	// Token: 0x04002117 RID: 8471
	private Rigidbody rb;

	// Token: 0x04002118 RID: 8472
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002119 RID: 8473
	public ParticleSystem particleBitsTiny;

	// Token: 0x0400211A RID: 8474
	public ParticleSystem particleBitsSmall;

	// Token: 0x0400211B RID: 8475
	public ParticleSystem particleBitsMedium;

	// Token: 0x0400211C RID: 8476
	public ParticleSystem particleBitsBig;

	// Token: 0x0400211D RID: 8477
	private Vector3 randomTorque;

	// Token: 0x0400211E RID: 8478
	private float timeToPound = 1f;

	// Token: 0x0400211F RID: 8479
	[Space]
	[Header("Pound Animation")]
	public AnimationCurve poundCurve;

	// Token: 0x04002120 RID: 8480
	private bool poundActive;

	// Token: 0x04002121 RID: 8481
	public float poundSpeed;

	// Token: 0x04002122 RID: 8482
	public float poundIntensity;

	// Token: 0x04002123 RID: 8483
	private float poundIntensityMuilplier;

	// Token: 0x04002124 RID: 8484
	private float poundLerp;

	// Token: 0x04002125 RID: 8485
	private float shakeAmplitudeMultiplier;

	// Token: 0x04002126 RID: 8486
	private float shakeFrequencyMultiplier;
}
