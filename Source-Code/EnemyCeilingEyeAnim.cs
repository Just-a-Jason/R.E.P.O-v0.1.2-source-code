using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class EnemyCeilingEyeAnim : MonoBehaviour
{
	// Token: 0x060001D6 RID: 470 RVA: 0x00012DF9 File Offset: 0x00010FF9
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00012E14 File Offset: 0x00011014
	private void Update()
	{
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget || this.controller.currentState == EnemyCeilingEye.State.TargetLost)
		{
			this.animator.SetBool("hasTarget", true);
		}
		else
		{
			this.animator.SetBool("hasTarget", false);
		}
		if (this.controller.enemy.CurrentState == EnemyState.Despawn || this.controller.currentState == EnemyCeilingEye.State.Move)
		{
			this.animator.SetBool("despawn", true);
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		this.SfxStaringLoop();
		if (this.controller.deathImpulse)
		{
			this.controller.deathImpulse = false;
			this.animator.SetTrigger("Death");
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00012ED6 File Offset: 0x000110D6
	public void SetSpawn()
	{
		this.animator.Play("Ceiling Eye Spawn", 0, 0f);
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00012EEE File Offset: 0x000110EE
	public void SetAttack()
	{
		this.animator.Play("Ceiling Eye Attack", 0, 0f);
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00012F06 File Offset: 0x00011106
	public void SetDespawn()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.controller.enemy.CurrentState == EnemyState.Despawn)
		{
			this.controller.enemy.EnemyParent.Despawn();
			return;
		}
		this.controller.OnSpawn();
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00012F45 File Offset: 0x00011145
	public void AttackFinished()
	{
		this.controller.enemy.EnemyParent.SpawnedTimerSet(0f);
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00012F64 File Offset: 0x00011164
	public void Explosion()
	{
		Vector3 b = new Vector3(0f, -0.5f, 0f);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + b, Vector3.down, out raycastHit, 30f, SemiFunc.LayerMaskGetVisionObstruct()))
		{
			this.particleScriptExplosion.Spawn(raycastHit.point, 2f, 50, 50, 1f, false, false, 1f);
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00012FE0 File Offset: 0x000111E0
	public void DeathEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.particleImpact.Play();
		this.particleBits.Play();
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0001308A File Offset: 0x0001128A
	public void TeleportParticlesStart()
	{
		this.TeleportParticles.Play();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x00013097 File Offset: 0x00011297
	public void TeleportParticlesStop()
	{
		this.TeleportParticles.Stop();
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x000130A4 File Offset: 0x000112A4
	public void SfxBlink()
	{
		this.sfxBlink.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000130D1 File Offset: 0x000112D1
	public void SfxDespawn()
	{
		this.sfxDespawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x000130FE File Offset: 0x000112FE
	public void SfxSpawn()
	{
		this.sfxSpawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0001312B File Offset: 0x0001132B
	public void SfxLaserBuildup()
	{
		this.sfxLaserBuildup.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00013158 File Offset: 0x00011358
	public void SfxLaserBeam()
	{
		this.sfxLaserBeam.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x000131EC File Offset: 0x000113EC
	public void SfxStaringStart()
	{
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget && this.controller.targetPlayer.isLocal)
		{
			AudioScare.instance.PlayCustom(this.sfxStaringStart, 0.3f, 20f);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00013290 File Offset: 0x00011490
	public void SfxStaringLoop()
	{
		this.sfxStareLoop.PlayLoop(this.isPlayingStaringLoop, 0.05f, 0.25f, 1f);
		this.sfxTwitchLoop.PlayLoop(this.isPlayingTwitchLoop, 0.1f, 0.25f, 1f);
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget)
		{
			this.isPlayingStaringLoop = true;
		}
		else
		{
			this.isPlayingStaringLoop = false;
		}
		if (this.controller.currentState == EnemyCeilingEye.State.HasTarget && this.controller.targetPlayer.isLocal)
		{
			this.isPlayingTwitchLoop = true;
			return;
		}
		this.isPlayingTwitchLoop = false;
	}

	// Token: 0x040003BA RID: 954
	public EnemyCeilingEye controller;

	// Token: 0x040003BB RID: 955
	private Animator animator;

	// Token: 0x040003BC RID: 956
	public Enemy enemy;

	// Token: 0x040003BD RID: 957
	public ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040003BE RID: 958
	public ParticleSystem TeleportParticles;

	// Token: 0x040003BF RID: 959
	public ParticleSystem particleImpact;

	// Token: 0x040003C0 RID: 960
	public ParticleSystem particleBits;

	// Token: 0x040003C1 RID: 961
	[Header("Sounds")]
	public Sound sfxBlink;

	// Token: 0x040003C2 RID: 962
	public Sound sfxDespawn;

	// Token: 0x040003C3 RID: 963
	public Sound sfxSpawn;

	// Token: 0x040003C4 RID: 964
	public Sound sfxDeath;

	// Token: 0x040003C5 RID: 965
	public Sound sfxLaserBuildup;

	// Token: 0x040003C6 RID: 966
	public Sound sfxLaserBeam;

	// Token: 0x040003C7 RID: 967
	public AudioClip sfxStaringStart;

	// Token: 0x040003C8 RID: 968
	public Sound sfxStareLoop;

	// Token: 0x040003C9 RID: 969
	public Sound sfxTwitchLoop;

	// Token: 0x040003CA RID: 970
	private bool isPlayingTwitchLoop = true;

	// Token: 0x040003CB RID: 971
	private bool isPlayingStaringLoop = true;
}
