using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class ItemGunBullet : MonoBehaviour
{
	// Token: 0x06000C03 RID: 3075 RVA: 0x0006A890 File Offset: 0x00068A90
	public void ActivateAll()
	{
		base.gameObject.SetActive(true);
		this.hitEffectTransform = base.transform.Find("Hit Effect");
		this.particleSparks = this.hitEffectTransform.Find("Particle Sparks").GetComponent<ParticleSystem>();
		this.particleSmoke = this.hitEffectTransform.Find("Particle Smoke").GetComponent<ParticleSystem>();
		this.particleImpact = this.hitEffectTransform.Find("Particle Impact").GetComponent<ParticleSystem>();
		this.hitLight = this.hitEffectTransform.Find("Hit Light").GetComponent<Light>();
		this.shootLine = base.GetComponentInChildren<LineRenderer>();
		Vector3 position = base.transform.position;
		Vector3 forward = this.hitPosition - position;
		this.shootLine.enabled = true;
		this.shootLine.SetPosition(0, base.transform.position);
		this.shootLine.SetPosition(1, base.transform.position + forward.normalized * 0.5f);
		this.shootLine.SetPosition(2, this.hitPosition - forward.normalized * 0.5f);
		this.shootLine.SetPosition(3, this.hitPosition);
		this.shootLineActive = true;
		this.shootLineLerp = 0f;
		if (this.bulletHit)
		{
			this.hitEffectTransform.gameObject.SetActive(true);
			this.particleSparks.gameObject.SetActive(true);
			this.particleSmoke.gameObject.SetActive(true);
			this.particleImpact.gameObject.SetActive(true);
			this.hitLight.enabled = true;
			this.hurtCollider.gameObject.SetActive(true);
			Quaternion rotation = Quaternion.LookRotation(forward);
			this.hurtCollider.transform.rotation = rotation;
			this.hurtCollider.transform.position = this.hitPosition;
			this.hurtCollider.gameObject.SetActive(true);
			this.hitEffectTransform.position = this.hitPosition;
			this.hitEffectTransform.rotation = rotation;
		}
		base.StartCoroutine(this.BulletDestroy());
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0006AAC1 File Offset: 0x00068CC1
	private IEnumerator BulletDestroy()
	{
		yield return new WaitForSeconds(0.2f);
		while (this.particleSparks.isPlaying || this.particleSmoke.isPlaying || this.particleImpact.isPlaying || this.hitLight.enabled || this.shootLine.enabled || this.hurtCollider.gameObject.activeSelf)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0006AAD0 File Offset: 0x00068CD0
	private void LineRendererLogic()
	{
		if (this.shootLineActive)
		{
			this.shootLine.widthMultiplier = this.shootLineWidthCurve.Evaluate(this.shootLineLerp);
			this.shootLineLerp += Time.deltaTime * 5f;
			if (this.shootLineLerp >= 1f)
			{
				this.shootLine.enabled = false;
				this.shootLine.gameObject.SetActive(false);
				this.shootLineActive = false;
			}
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0006AB4C File Offset: 0x00068D4C
	private void Update()
	{
		this.LineRendererLogic();
		if (this.bulletHit)
		{
			if (this.hurtColliderTimer > 0f)
			{
				this.hurtColliderTimer -= Time.deltaTime;
				this.hurtCollider.gameObject.SetActive(true);
			}
			else
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
			if (this.hitLight)
			{
				this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, 0f, Time.deltaTime * 10f);
				if (this.hitLight.intensity < 0.01f)
				{
					this.hitLight.enabled = false;
				}
			}
		}
	}

	// Token: 0x04001345 RID: 4933
	private Transform hitEffectTransform;

	// Token: 0x04001346 RID: 4934
	private ParticleSystem particleSparks;

	// Token: 0x04001347 RID: 4935
	private ParticleSystem particleSmoke;

	// Token: 0x04001348 RID: 4936
	private ParticleSystem particleImpact;

	// Token: 0x04001349 RID: 4937
	private Light hitLight;

	// Token: 0x0400134A RID: 4938
	private LineRenderer shootLine;

	// Token: 0x0400134B RID: 4939
	public HurtCollider hurtCollider;

	// Token: 0x0400134C RID: 4940
	internal bool bulletHit;

	// Token: 0x0400134D RID: 4941
	internal Vector3 hitPosition;

	// Token: 0x0400134E RID: 4942
	public float hurtColliderTimer = 0.25f;

	// Token: 0x0400134F RID: 4943
	private bool shootLineActive;

	// Token: 0x04001350 RID: 4944
	private float shootLineLerp;

	// Token: 0x04001351 RID: 4945
	internal AnimationCurve shootLineWidthCurve;
}
