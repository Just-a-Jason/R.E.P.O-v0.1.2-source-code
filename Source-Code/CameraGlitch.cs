using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class CameraGlitch : MonoBehaviour
{
	// Token: 0x060000C9 RID: 201 RVA: 0x000078BA File Offset: 0x00005ABA
	private void Awake()
	{
		CameraGlitch.Instance = this;
		this.targetCameraFOV = this.targetCamera.fieldOfView;
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000078D3 File Offset: 0x00005AD3
	private void Start()
	{
		this.ActiveParent.SetActive(false);
	}

	// Token: 0x060000CB RID: 203 RVA: 0x000078E4 File Offset: 0x00005AE4
	private void Update()
	{
		float num = this.targetCamera.fieldOfView / this.targetCameraFOV;
		if (num > 1.5f)
		{
			num *= 1.2f;
		}
		if (num < 0.5f)
		{
			num *= 0.8f;
		}
		base.transform.localScale = new Vector3(num, num, num);
		if (this.doNotLookEffectTimer <= 0f)
		{
			this.doNotLookEffectSound.PlayLoop(false, 2f, 1f, 1f);
			return;
		}
		this.doNotLookEffectSound.PlayLoop(true, 2f, 1f, 1f);
		this.doNotLookEffectTimer -= Time.deltaTime;
		if (this.doNotLookEffectImpulseTimer <= 0f)
		{
			this.PlayShort();
			this.doNotLookEffectImpulseTimer = Random.Range(0.3f, 1f);
			return;
		}
		this.doNotLookEffectImpulseTimer -= Time.deltaTime;
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000079C8 File Offset: 0x00005BC8
	public void DoNotLookEffectSet()
	{
		this.doNotLookEffectTimer = 0.1f;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x000079D8 File Offset: 0x00005BD8
	public void PlayLong()
	{
		this.Animator.SetTrigger("Long");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.3f);
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00007A58 File Offset: 0x00005C58
	public void PlayShort()
	{
		this.Animator.SetTrigger("Short");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00007AD8 File Offset: 0x00005CD8
	public void PlayTiny()
	{
		this.Animator.SetTrigger("Tiny");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchTinyCount));
		this.GlitchTiny.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00007B3C File Offset: 0x00005D3C
	public void PlayLongHurt()
	{
		this.Animator.SetTrigger("HurtLong");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.HurtLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(5f, 0.2f);
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00007C00 File Offset: 0x00005E00
	public void PlayShortHurt()
	{
		this.Animator.SetTrigger("HurtShort");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.HurtShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(3f, 0.2f);
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00007CC4 File Offset: 0x00005EC4
	public void PlayLongHeal()
	{
		this.Animator.SetTrigger("HealLong");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.HealLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(2.5f, 0.2f);
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00007D88 File Offset: 0x00005F88
	public void PlayShortHeal()
	{
		this.Animator.SetTrigger("HealShort");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.HealShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(1.5f, 0.2f);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00007E4C File Offset: 0x0000604C
	public void PlayUpgrade()
	{
		this.Animator.SetTrigger("Upgrade");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.Upgrade.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(2f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
	}

	// Token: 0x0400020A RID: 522
	public static CameraGlitch Instance;

	// Token: 0x0400020B RID: 523
	public Camera targetCamera;

	// Token: 0x0400020C RID: 524
	private float targetCameraFOV;

	// Token: 0x0400020D RID: 525
	public Animator Animator;

	// Token: 0x0400020E RID: 526
	public GameObject ActiveParent;

	// Token: 0x0400020F RID: 527
	[Space]
	public int GlitchLongCount;

	// Token: 0x04000210 RID: 528
	public int GlitchShortCount;

	// Token: 0x04000211 RID: 529
	public int GlitchTinyCount;

	// Token: 0x04000212 RID: 530
	[Space]
	public Sound GlitchLong;

	// Token: 0x04000213 RID: 531
	public Sound GlitchShort;

	// Token: 0x04000214 RID: 532
	public Sound GlitchTiny;

	// Token: 0x04000215 RID: 533
	[Space]
	public Sound HurtShort;

	// Token: 0x04000216 RID: 534
	public Sound HurtLong;

	// Token: 0x04000217 RID: 535
	[Space]
	public Sound HealShort;

	// Token: 0x04000218 RID: 536
	public Sound HealLong;

	// Token: 0x04000219 RID: 537
	[Space]
	public Sound Upgrade;

	// Token: 0x0400021A RID: 538
	[Space]
	public Sound doNotLookEffectSound;

	// Token: 0x0400021B RID: 539
	private float doNotLookEffectTimer;

	// Token: 0x0400021C RID: 540
	private float doNotLookEffectImpulseTimer;
}
