using System;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class StunExplosion : MonoBehaviour
{
	// Token: 0x06000B18 RID: 2840 RVA: 0x00062EFE File Offset: 0x000610FE
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x00062F0C File Offset: 0x0006110C
	public void StunExplosionReset()
	{
		this.removeTimer = 0f;
		this.lightEval = 0f;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00062F30 File Offset: 0x00061130
	private void Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.light)
		{
			if (this.lightEval < 1f)
			{
				this.light.intensity = 10f * this.lightCurve.Evaluate(this.lightEval);
				this.lightEval += 0.2f * Time.deltaTime;
			}
			else
			{
				this.light.intensity = 0f;
			}
		}
		if (this.removeTimer > 0.5f)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
		else
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
		this.removeTimer += Time.deltaTime;
		if (this.removeTimer >= 20f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001207 RID: 4615
	public Light light;

	// Token: 0x04001208 RID: 4616
	public AnimationCurve lightCurve;

	// Token: 0x04001209 RID: 4617
	private float lightEval;

	// Token: 0x0400120A RID: 4618
	private float removeTimer;

	// Token: 0x0400120B RID: 4619
	private HurtCollider hurtCollider;

	// Token: 0x0400120C RID: 4620
	public ItemGrenade itemGrenade;
}
