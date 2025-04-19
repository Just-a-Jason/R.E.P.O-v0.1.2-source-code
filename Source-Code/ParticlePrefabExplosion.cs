using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class ParticlePrefabExplosion : MonoBehaviour
{
	// Token: 0x06000F74 RID: 3956 RVA: 0x0008D874 File Offset: 0x0008BA74
	private void Start()
	{
		bool flag = false;
		if (this.explosionSize <= 0.25f)
		{
			flag = true;
		}
		ParticleSystem.MainModule main = this.particleFire.main;
		float startSpeedMultiplier = main.startSpeedMultiplier;
		float startLifetimeMultiplier = main.startLifetimeMultiplier;
		main.startSpeedMultiplier = startSpeedMultiplier * this.explosionSize;
		main.startLifetimeMultiplier = startLifetimeMultiplier * this.explosionSize;
		main.startLifetimeMultiplier = Mathf.Max(main.startLifetimeMultiplier, 0.2f);
		main.startSizeMultiplier = Mathf.Max(main.startSizeMultiplier, 0.1f);
		if (flag)
		{
			main.startSpeedMultiplier *= 0.5f;
			main.startLifetimeMultiplier *= 0.5f;
			main.startSizeMultiplier *= 0.8f;
		}
		this.particleFire.Play();
		ParticleSystem.MainModule main2 = this.particleSmoke.main;
		startSpeedMultiplier = main2.startSpeedMultiplier;
		startLifetimeMultiplier = main2.startLifetimeMultiplier;
		main2.startSpeedMultiplier = startSpeedMultiplier * this.explosionSize;
		main2.startLifetimeMultiplier = startLifetimeMultiplier * this.explosionSize;
		main2.startLifetimeMultiplier = Mathf.Max(main2.startLifetimeMultiplier * 1.2f, 2f);
		main2.startSizeMultiplier = Mathf.Max(main.startSizeMultiplier * 1.2f, 0.1f);
		this.particleSmoke.Play();
		this.light.enabled = true;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0008D9D4 File Offset: 0x0008BBD4
	private void Update()
	{
		if (!this.onlyParticleEffect && this.HurtColliderActive)
		{
			this.HurtColliderTimer += Time.deltaTime;
			if (this.HurtColliderFirstSetup)
			{
				if (!this.SkipHurtColliderSetup)
				{
					this.HurtCollider.playerDamage = this.explosionDamage;
					this.HurtCollider.enemyDamage = this.explosionDamageEnemy;
					this.HurtCollider.physHitForce = (float)this.explosionDamage * 0.5f;
					if (this.explosionDamage >= 50)
					{
						this.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
						this.HurtCollider.physHingeDestroy = true;
					}
					else if (this.explosionDamage >= 15)
					{
						this.HurtCollider.physImpact = HurtCollider.BreakImpact.Medium;
					}
					else
					{
						this.HurtCollider.physImpact = HurtCollider.BreakImpact.Light;
					}
				}
				this.HurtCollider.gameObject.SetActive(true);
				this.HurtCollider.transform.localScale = new Vector3(this.explosionSize, this.explosionSize, this.explosionSize);
				this.HurtColliderFirstSetup = false;
			}
			if (this.HurtColliderSecondSetup && this.HurtColliderTimer > 0.2f)
			{
				this.HurtCollider.playerDamage = 0;
				this.HurtCollider.playerHitForce *= 0.25f;
				this.HurtCollider.physHitForce *= 0.25f;
				if (this.HurtCollider.physImpact > HurtCollider.BreakImpact.None)
				{
					this.HurtCollider.physImpact--;
				}
				this.HurtCollider.transform.localScale = new Vector3(this.explosionSize * 2f, this.explosionSize * 2f, this.explosionSize * 2f);
				this.HurtColliderSecondSetup = false;
			}
			if (this.HurtColliderTimer > 1f)
			{
				this.HurtCollider.gameObject.SetActive(false);
				this.HurtColliderActive = false;
			}
		}
		this.smokeNullCheckTimer += Time.deltaTime;
		if (this.smokeNullCheckTimer > 1f)
		{
			if (!this.particleSmoke)
			{
				Object.Destroy(base.gameObject);
			}
			this.smokeNullCheckTimer = 0f;
		}
		if (this.light.enabled)
		{
			float num = this.explosionSize;
			num = Mathf.Max(num, 0.8f);
			this.lightIntensityCurveProgress += 0.5f * Time.deltaTime;
			this.light.intensity = 10f * num * this.lightIntensityCurve.Evaluate(this.lightIntensityCurveProgress);
			this.light.range = 10f * num * this.lightIntensityCurve.Evaluate(this.lightIntensityCurveProgress);
			this.light.color = this.lightColorOverTime.Evaluate(this.lightIntensityCurveProgress);
			if (this.lightIntensityCurveProgress > this.lightIntensityCurve.keys[this.lightIntensityCurve.length - 1].time)
			{
				this.light.enabled = false;
				this.lightIntensityCurveProgress = 0f;
			}
		}
	}

	// Token: 0x04001A37 RID: 6711
	public ParticleSystem particleFire;

	// Token: 0x04001A38 RID: 6712
	public ParticleSystem particleSmoke;

	// Token: 0x04001A39 RID: 6713
	public Light light;

	// Token: 0x04001A3A RID: 6714
	public AnimationCurve lightIntensityCurve;

	// Token: 0x04001A3B RID: 6715
	private float lightIntensityCurveProgress;

	// Token: 0x04001A3C RID: 6716
	public Gradient lightColorOverTime;

	// Token: 0x04001A3D RID: 6717
	internal float explosionSize = 1f;

	// Token: 0x04001A3E RID: 6718
	internal int explosionDamage;

	// Token: 0x04001A3F RID: 6719
	internal int explosionDamageEnemy;

	// Token: 0x04001A40 RID: 6720
	private float smokeNullCheckTimer;

	// Token: 0x04001A41 RID: 6721
	[HideInInspector]
	public float forceMultiplier = 1f;

	// Token: 0x04001A42 RID: 6722
	[HideInInspector]
	public bool onlyParticleEffect;

	// Token: 0x04001A43 RID: 6723
	internal bool SkipHurtColliderSetup;

	// Token: 0x04001A44 RID: 6724
	public HurtCollider HurtCollider;

	// Token: 0x04001A45 RID: 6725
	private bool HurtColliderActive = true;

	// Token: 0x04001A46 RID: 6726
	private bool HurtColliderFirstSetup = true;

	// Token: 0x04001A47 RID: 6727
	private bool HurtColliderSecondSetup = true;

	// Token: 0x04001A48 RID: 6728
	private float HurtColliderTimer;
}
