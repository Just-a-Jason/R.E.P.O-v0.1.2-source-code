using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class ItemGunMuzzleFlash : MonoBehaviour
{
	// Token: 0x06000C08 RID: 3080 RVA: 0x0006AC18 File Offset: 0x00068E18
	public void ActivateAllEffects()
	{
		base.gameObject.SetActive(true);
		this.smoke = base.transform.Find("Particle Smoke").GetComponent<ParticleSystem>();
		this.impact = base.transform.Find("Particle Impact").GetComponent<ParticleSystem>();
		this.sparks = base.transform.Find("Particle Sparks").GetComponent<ParticleSystem>();
		this.shootLight = base.GetComponentInChildren<Light>();
		this.smoke.gameObject.SetActive(true);
		this.impact.gameObject.SetActive(true);
		this.sparks.gameObject.SetActive(true);
		this.shootLight.enabled = true;
		this.smoke.Play();
		this.impact.Play();
		this.sparks.Play();
		base.StartCoroutine(this.MuzzleFlashDestroy());
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0006ACFB File Offset: 0x00068EFB
	private IEnumerator MuzzleFlashDestroy()
	{
		yield return new WaitForSeconds(0.1f);
		while (this.smoke.isPlaying || this.impact.isPlaying || this.sparks.isPlaying || this.shootLight.enabled)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0006AD0C File Offset: 0x00068F0C
	private void Update()
	{
		if (this.shootLight)
		{
			this.shootLight.intensity = Mathf.Lerp(this.shootLight.intensity, 0f, Time.deltaTime * 10f);
			if (this.shootLight.intensity < 0.01f)
			{
				this.shootLight.enabled = false;
			}
		}
	}

	// Token: 0x04001352 RID: 4946
	private ParticleSystem smoke;

	// Token: 0x04001353 RID: 4947
	private ParticleSystem impact;

	// Token: 0x04001354 RID: 4948
	private ParticleSystem sparks;

	// Token: 0x04001355 RID: 4949
	private Light shootLight;
}
