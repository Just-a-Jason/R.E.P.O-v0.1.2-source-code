using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class EnemyRobeAnim : MonoBehaviour
{
	// Token: 0x060003C7 RID: 967 RVA: 0x000256C2 File Offset: 0x000238C2
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x000256DC File Offset: 0x000238DC
	private void Update()
	{
		if (this.controller.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.isOnScreen)
		{
			this.animator.SetBool("isOnScreen", true);
		}
		else
		{
			this.animator.SetBool("isOnScreen", false);
		}
		if (this.controller.enemy.CurrentState == EnemyState.Despawn)
		{
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger("despawn");
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.despawnImpulse = true;
		}
		if (this.controller.attackImpulse)
		{
			this.controller.attackImpulse = false;
			if (!this.controller.enemy.IsStunned())
			{
				this.animator.SetTrigger("attack");
			}
		}
		if (this.controller.deathImpulse)
		{
			this.controller.deathImpulse = false;
			this.animator.SetTrigger("Death");
		}
		if (this.controller.idleBreakTrigger)
		{
			this.controller.idleBreakTrigger = false;
			if (!this.controller.enemy.IsStunned())
			{
				this.animator.SetTrigger("idleBreak");
			}
		}
		if (this.controller.currentState == EnemyRobe.State.LookUnder || this.controller.currentState == EnemyRobe.State.LookUnderAttack)
		{
			if (this.lookUnderImpulse)
			{
				this.animator.SetTrigger("LookUnder");
				this.lookUnderImpulse = false;
			}
			this.animator.SetBool("LookingUnder", true);
		}
		else
		{
			this.animator.SetBool("LookingUnder", false);
			this.lookUnderImpulse = true;
		}
		if (this.controller.lookUnderAttackImpulse)
		{
			this.animator.SetTrigger("LookUnderAttack");
			this.controller.lookUnderAttackImpulse = false;
		}
		if (this.controller.currentState == EnemyRobe.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.sfxStunStart.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("Stunned", true);
			this.sfxStunLoop.PlayLoop(true, 2f, 2f, 1f);
		}
		else
		{
			this.sfxStunLoop.PlayLoop(false, 2f, 2f, 1f);
			this.animator.SetBool("Stunned", false);
			this.stunImpulse = true;
		}
		this.sfxTargetPlayerLoop.PlayLoop(this.isPlayingTargetPlayerLoop, 2f, 2f, 1f);
		this.sfxHandIdle.PlayLoop(this.isPlayingHandIdle, 2f, 2f, 1f);
		this.sfxHandAggressive.PlayLoop(this.isPlayingHandAggressive, 2f, 2f, 1f);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x000259DC File Offset: 0x00023BDC
	public void SetSpawn()
	{
		this.animator.Play("Robe Spawn", 0, 0f);
	}

	// Token: 0x060003CA RID: 970 RVA: 0x000259F4 File Offset: 0x00023BF4
	public void SetDespawn()
	{
		this.controller.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00025A0B File Offset: 0x00023C0B
	public void TeleportParticlesStart()
	{
		this.teleportParticles.Play();
	}

	// Token: 0x060003CC RID: 972 RVA: 0x00025A18 File Offset: 0x00023C18
	public void TeleportParticlesStop()
	{
		this.teleportParticles.Stop();
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00025A25 File Offset: 0x00023C25
	public void SpawnParticlesImpulse()
	{
		this.spawnParticles.Play();
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00025A34 File Offset: 0x00023C34
	public void DeathParticlesImpulse()
	{
		ParticleSystem[] array = this.deathParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00025A60 File Offset: 0x00023C60
	public void LookUnderIntro()
	{
		if (this.controller.targetPlayer.isLocal)
		{
			AudioScare.instance.PlaySoft();
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00025AE5 File Offset: 0x00023CE5
	public void SfxTargetPlayerLoop()
	{
		if (this.controller.isOnScreen)
		{
			this.isPlayingTargetPlayerLoop = true;
			return;
		}
		this.isPlayingTargetPlayerLoop = false;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00025B03 File Offset: 0x00023D03
	public void SfxIdleBreak()
	{
		this.sfxIdleBreak.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00025B38 File Offset: 0x00023D38
	public void SfxAttack()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.sfxAttack.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackGlobal.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00025C04 File Offset: 0x00023E04
	public void LookUnderAttack()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.sfxAttackUnder.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackUnderGlobal.Play(this.controller.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04000664 RID: 1636
	public EnemyRobe controller;

	// Token: 0x04000665 RID: 1637
	internal Animator animator;

	// Token: 0x04000666 RID: 1638
	public ParticleSystem teleportParticles;

	// Token: 0x04000667 RID: 1639
	public ParticleSystem[] deathParticles;

	// Token: 0x04000668 RID: 1640
	public ParticleSystem spawnParticles;

	// Token: 0x04000669 RID: 1641
	[Header("Sounds")]
	public Sound sfxTargetPlayerLoop;

	// Token: 0x0400066A RID: 1642
	public Sound sfxIdleBreak;

	// Token: 0x0400066B RID: 1643
	public Sound sfxAttack;

	// Token: 0x0400066C RID: 1644
	public Sound sfxAttackGlobal;

	// Token: 0x0400066D RID: 1645
	public Sound sfxHurt;

	// Token: 0x0400066E RID: 1646
	public Sound sfxHandIdle;

	// Token: 0x0400066F RID: 1647
	public Sound sfxHandAggressive;

	// Token: 0x04000670 RID: 1648
	public Sound sfxStunStart;

	// Token: 0x04000671 RID: 1649
	public Sound sfxStunLoop;

	// Token: 0x04000672 RID: 1650
	public Sound sfxAttackUnder;

	// Token: 0x04000673 RID: 1651
	public Sound sfxAttackUnderGlobal;

	// Token: 0x04000674 RID: 1652
	public bool isPlayingTargetPlayerLoop;

	// Token: 0x04000675 RID: 1653
	public bool isPlayingHandIdle;

	// Token: 0x04000676 RID: 1654
	public bool isPlayingHandAggressive;

	// Token: 0x04000677 RID: 1655
	private bool stunImpulse;

	// Token: 0x04000678 RID: 1656
	private bool despawnImpulse;

	// Token: 0x04000679 RID: 1657
	private bool lookUnderImpulse;
}
