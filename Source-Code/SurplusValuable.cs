using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class SurplusValuable : MonoBehaviour
{
	// Token: 0x060013CB RID: 5067 RVA: 0x000ACC9C File Offset: 0x000AAE9C
	private void Start()
	{
		this.impactDetector = base.GetComponentInChildren<PhysGrabObjectImpactDetector>();
		this.impactDetector.indestructibleSpawnTimer = 0.1f;
		this.coinParticles.Emit((int)(30f * this.coinMultiplier));
		this.spawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x000ACD08 File Offset: 0x000AAF08
	private void Update()
	{
		if (this.indestructibleTimer > 0f)
		{
			this.indestructibleTimer -= Time.deltaTime;
			if (this.indestructibleTimer <= 0f)
			{
				this.impactDetector.destroyDisable = false;
			}
		}
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x000ACD42 File Offset: 0x000AAF42
	public void BreakLight()
	{
		this.coinParticles.Emit((int)(3f * this.coinMultiplier));
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x000ACD5C File Offset: 0x000AAF5C
	public void BreakMedium()
	{
		this.coinParticles.Emit((int)(5f * this.coinMultiplier));
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x000ACD76 File Offset: 0x000AAF76
	public void BreakHeavy()
	{
		this.coinParticles.Emit((int)(10f * this.coinMultiplier));
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x000ACD90 File Offset: 0x000AAF90
	public void DestroyImpulse()
	{
		this.coinParticles.Emit((int)(20f * this.coinMultiplier));
		this.coinParticles.transform.parent = null;
		this.coinParticles.main.stopAction = ParticleSystemStopAction.Destroy;
	}

	// Token: 0x040021D6 RID: 8662
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040021D7 RID: 8663
	private float indestructibleTimer = 3f;

	// Token: 0x040021D8 RID: 8664
	public float coinMultiplier = 1f;

	// Token: 0x040021D9 RID: 8665
	public ParticleSystem coinParticles;

	// Token: 0x040021DA RID: 8666
	public Sound spawnSound;
}
