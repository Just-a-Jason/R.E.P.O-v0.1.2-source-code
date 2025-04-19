using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class EnemyHunterAnim : MonoBehaviour
{
	// Token: 0x0600038E RID: 910 RVA: 0x000237AF File Offset: 0x000219AF
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x0600038F RID: 911 RVA: 0x000237CC File Offset: 0x000219CC
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if ((this.enemyHunter.currentState == EnemyHunter.State.Roam || this.enemyHunter.currentState == EnemyHunter.State.InvestigateWalk || this.enemyHunter.currentState == EnemyHunter.State.Leave) && (this.enemy.Rigidbody.velocity.magnitude > 0.2f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.25f))
		{
			this.moveTimer = 0.1f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool("Moving", true);
		}
		else
		{
			this.animator.SetBool("Moving", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			this.animator.SetBool("Leaving", true);
		}
		else
		{
			this.animator.SetBool("Leaving", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("Stunned", true);
		}
		else
		{
			this.animator.SetBool("Stunned", false);
			this.stunImpulse = true;
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Aim)
		{
			this.animator.SetBool("Aiming", true);
		}
		else
		{
			this.animator.SetBool("Aiming", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Shoot || this.enemyHunter.currentState == EnemyHunter.State.ShootEnd)
		{
			this.animator.SetBool("Shooting", true);
		}
		else
		{
			this.animator.SetBool("Shooting", false);
		}
		if (this.enemyHunter.currentState == EnemyHunter.State.Despawn)
		{
			this.animator.SetBool("Despawning", true);
			return;
		}
		this.animator.SetBool("Despawning", false);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x000239F2 File Offset: 0x00021BF2
	public void OnSpawn()
	{
		this.animator.SetBool("Stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00023A1C File Offset: 0x00021C1C
	public void TeleportEffect()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.05f);
		foreach (ParticleSystem particleSystem in this.teleportEffects)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00023AC8 File Offset: 0x00021CC8
	public void FootstepShort()
	{
		this.soundFootstepShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00023B48 File Offset: 0x00021D48
	public void FootstepLong()
	{
		this.soundFootstepLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00023BC8 File Offset: 0x00021DC8
	public void AimStart()
	{
		int num = Random.Range(0, this.aimStartClips.Length);
		this.soundAimStart.Sounds[0] = this.aimStartClips[num];
		this.soundAimStartGlobal.Sounds[0] = this.aimStartGlobalClips[num];
		this.soundAimStart.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundAimStartGlobal.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00023C6E File Offset: 0x00021E6E
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00023C80 File Offset: 0x00021E80
	public void Reload01()
	{
		this.soundReload01.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00023CB2 File Offset: 0x00021EB2
	public void Reload02()
	{
		this.soundReload02.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00023CE4 File Offset: 0x00021EE4
	public void MoveShort()
	{
		this.soundMoveShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00023D16 File Offset: 0x00021F16
	public void MoveLong()
	{
		this.soundMoveLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00023D48 File Offset: 0x00021F48
	public void GunLong()
	{
		this.soundGunLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00023D7A File Offset: 0x00021F7A
	public void GunShort()
	{
		this.soundGunShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00023DAC File Offset: 0x00021FAC
	public void Spawn()
	{
		this.soundSpawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00023DDE File Offset: 0x00021FDE
	public void DespawnSound()
	{
		this.soundDespawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00023E10 File Offset: 0x00022010
	public void LeaveStartSound()
	{
		this.soundLeaveStart.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00023E42 File Offset: 0x00022042
	public void LeaveStartDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			this.enemyHunter.stateTimer = 0f;
		}
	}

	// Token: 0x04000630 RID: 1584
	public Enemy enemy;

	// Token: 0x04000631 RID: 1585
	public EnemyHunter enemyHunter;

	// Token: 0x04000632 RID: 1586
	internal Animator animator;

	// Token: 0x04000633 RID: 1587
	public Materials.MaterialTrigger material;

	// Token: 0x04000634 RID: 1588
	private float moveTimer;

	// Token: 0x04000635 RID: 1589
	private bool stunImpulse;

	// Token: 0x04000636 RID: 1590
	internal bool spawnImpulse;

	// Token: 0x04000637 RID: 1591
	[Space]
	public List<ParticleSystem> teleportEffects;

	// Token: 0x04000638 RID: 1592
	[Space]
	public Sound soundFootstepShort;

	// Token: 0x04000639 RID: 1593
	public Sound soundFootstepLong;

	// Token: 0x0400063A RID: 1594
	public Sound soundReload01;

	// Token: 0x0400063B RID: 1595
	public Sound soundAimStart;

	// Token: 0x0400063C RID: 1596
	public Sound soundAimStartGlobal;

	// Token: 0x0400063D RID: 1597
	public Sound soundReload02;

	// Token: 0x0400063E RID: 1598
	public Sound soundMoveShort;

	// Token: 0x0400063F RID: 1599
	public Sound soundMoveLong;

	// Token: 0x04000640 RID: 1600
	public Sound soundGunLong;

	// Token: 0x04000641 RID: 1601
	public Sound soundGunShort;

	// Token: 0x04000642 RID: 1602
	public Sound soundSpawn;

	// Token: 0x04000643 RID: 1603
	public Sound soundDespawn;

	// Token: 0x04000644 RID: 1604
	public Sound soundLeaveStart;

	// Token: 0x04000645 RID: 1605
	[Space]
	public AudioClip[] aimStartClips;

	// Token: 0x04000646 RID: 1606
	public AudioClip[] aimStartGlobalClips;
}
