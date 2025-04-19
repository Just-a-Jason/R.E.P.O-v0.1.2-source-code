using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class EnemyRunnerAnim : MonoBehaviour
{
	// Token: 0x060003FB RID: 1019 RVA: 0x00027AED File Offset: 0x00025CED
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00027B08 File Offset: 0x00025D08
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
		if (!this.stunned && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger(this.animJump);
				this.animator.SetBool(this.animFalling, false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool(this.animFalling, true);
			}
		}
		else
		{
			if (this.landImpulse)
			{
				this.animator.SetTrigger(this.animLand);
				this.landImpulse = false;
			}
			this.animator.SetBool(this.animFalling, false);
			this.jumpImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.LookUnder)
		{
			if (this.lookUnderImpulse)
			{
				this.animator.SetTrigger(this.animLookUnder);
				this.lookUnderImpulse = false;
			}
			this.animator.SetBool(this.animLookingUnder, true);
		}
		else
		{
			this.animator.SetBool(this.animLookingUnder, false);
			this.lookUnderImpulse = true;
		}
		float num = 0.05f;
		if (this.IsMoving() && (this.enemy.Rigidbody.velocity.magnitude > num || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > num))
		{
			this.moveTimer = 0.1f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool(this.animMoving, true);
		}
		else
		{
			this.animator.SetBool(this.animMoving, false);
		}
		if (this.controller.currentState == EnemyRunner.State.SeekPlayer || this.controller.currentState == EnemyRunner.State.Sneak)
		{
			this.animator.SetBool(this.animSeeking, true);
		}
		else
		{
			this.animator.SetBool(this.animSeeking, false);
		}
		if (this.controller.currentState == EnemyRunner.State.AttackPlayer || this.controller.currentState == EnemyRunner.State.AttackPlayerOver || this.controller.currentState == EnemyRunner.State.AttackPlayerBackToNavMesh || this.controller.currentState == EnemyRunner.State.StuckAttack || this.controller.currentState == EnemyRunner.State.LookUnderStart)
		{
			this.animator.SetBool(this.animAttacking, true);
		}
		else
		{
			this.animator.SetBool(this.animAttacking, false);
		}
		if (this.controller.currentState == EnemyRunner.State.Notice || this.controller.currentState == EnemyRunner.State.StuckAttackNotice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger(this.animNotice);
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger(this.animStun);
				this.stunImpulse = false;
			}
			this.animator.SetBool(this.animStunned, true);
			this.stunned = true;
		}
		else
		{
			this.animator.SetBool(this.animStunned, false);
			this.stunImpulse = true;
			this.stunned = false;
		}
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
		if (this.controller.currentState == EnemyRunner.State.Despawn)
		{
			if (this.despawnImpulse)
			{
				this.animator.SetTrigger(this.animDespawn);
				this.despawnImpulse = false;
			}
		}
		else
		{
			this.despawnImpulse = true;
		}
		if (this.controller.currentState == EnemyRunner.State.Leave)
		{
			this.animator.SetBool(this.animLeaving, true);
			return;
		}
		this.animator.SetBool(this.animLeaving, false);
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00027EF0 File Offset: 0x000260F0
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00027F08 File Offset: 0x00026108
	private bool IsMoving()
	{
		return this.controller.currentState == EnemyRunner.State.Roam || this.controller.currentState == EnemyRunner.State.Investigate || this.controller.currentState == EnemyRunner.State.SeekPlayer || this.controller.currentState == EnemyRunner.State.Sneak || this.controller.currentState == EnemyRunner.State.Leave;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00027F5E File Offset: 0x0002615E
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00027F70 File Offset: 0x00026170
	public void LookUnderIntro()
	{
		if (!Camera.main)
		{
			return;
		}
		if (!AudioScare.instance)
		{
			return;
		}
		if (!GameDirector.instance)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, Camera.main.transform.position) < 10f)
		{
			AudioScare.instance.PlaySoft();
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00028030 File Offset: 0x00026230
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000280C4 File Offset: 0x000262C4
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000280F1 File Offset: 0x000262F1
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x0002811E File Offset: 0x0002631E
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0002814B File Offset: 0x0002634B
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00028178 File Offset: 0x00026378
	public void SfxFootstepSlow()
	{
		this.sfxFootstepSlow.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x000281F4 File Offset: 0x000263F4
	public void SfxFootstepFast()
	{
		this.sfxFootstepFast.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x0002826D File Offset: 0x0002646D
	public void SfxAttackSlash()
	{
		this.sfxAttackSlash.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0002829C File Offset: 0x0002649C
	public void SfxAttackGrunt()
	{
		if (this.attackGruntImpulse)
		{
			this.sfxAttackGrunt.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.attackGruntImpulse = false;
			return;
		}
		this.attackGruntImpulse = true;
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x000282EC File Offset: 0x000264EC
	public void SfxAttackUnderGrunt()
	{
		this.attackGruntCounter++;
		if (this.attackGruntCounter >= 3)
		{
			this.sfxAttackGrunt.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.attackGruntCounter = 0;
		}
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00028342 File Offset: 0x00026542
	public void ResetAttackGruntCounter()
	{
		this.attackGruntCounter = 3;
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0002834B File Offset: 0x0002654B
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x0400069B RID: 1691
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x0400069C RID: 1692
	private int animSeeking = Animator.StringToHash("seeking");

	// Token: 0x0400069D RID: 1693
	private int animAttacking = Animator.StringToHash("attacking");

	// Token: 0x0400069E RID: 1694
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x0400069F RID: 1695
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x040006A0 RID: 1696
	private int animLookingUnder = Animator.StringToHash("lookingUnder");

	// Token: 0x040006A1 RID: 1697
	private int animLeaving = Animator.StringToHash("leaving");

	// Token: 0x040006A2 RID: 1698
	private int animLand = Animator.StringToHash("Land");

	// Token: 0x040006A3 RID: 1699
	private int animLookUnder = Animator.StringToHash("LookUnder");

	// Token: 0x040006A4 RID: 1700
	private int animJump = Animator.StringToHash("Jump");

	// Token: 0x040006A5 RID: 1701
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x040006A6 RID: 1702
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x040006A7 RID: 1703
	private int animDespawn = Animator.StringToHash("Despawn");

	// Token: 0x040006A8 RID: 1704
	public Enemy enemy;

	// Token: 0x040006A9 RID: 1705
	public EnemyRunner controller;

	// Token: 0x040006AA RID: 1706
	internal Animator animator;

	// Token: 0x040006AB RID: 1707
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040006AC RID: 1708
	private bool stunned;

	// Token: 0x040006AD RID: 1709
	private float moveTimer;

	// Token: 0x040006AE RID: 1710
	private bool stunImpulse;

	// Token: 0x040006AF RID: 1711
	private bool despawnImpulse;

	// Token: 0x040006B0 RID: 1712
	internal bool spawnImpulse;

	// Token: 0x040006B1 RID: 1713
	private bool landImpulse;

	// Token: 0x040006B2 RID: 1714
	private bool lookUnderImpulse;

	// Token: 0x040006B3 RID: 1715
	private bool noticeImpulse;

	// Token: 0x040006B4 RID: 1716
	private bool jumpImpulse;

	// Token: 0x040006B5 RID: 1717
	private float jumpedTimer;

	// Token: 0x040006B6 RID: 1718
	[Header("One Shots")]
	public Sound sfxJump;

	// Token: 0x040006B7 RID: 1719
	public Sound sfxHurt;

	// Token: 0x040006B8 RID: 1720
	public Sound sfxDeath;

	// Token: 0x040006B9 RID: 1721
	public Sound sfxMoveShort;

	// Token: 0x040006BA RID: 1722
	public Sound sfxMoveLong;

	// Token: 0x040006BB RID: 1723
	public Sound sfxFootstepSlow;

	// Token: 0x040006BC RID: 1724
	public Sound sfxFootstepFast;

	// Token: 0x040006BD RID: 1725
	public Sound sfxAttackSlash;

	// Token: 0x040006BE RID: 1726
	public Sound sfxAttackGrunt;

	// Token: 0x040006BF RID: 1727
	private bool attackGruntImpulse = true;

	// Token: 0x040006C0 RID: 1728
	private int attackGruntCounter;

	// Token: 0x040006C1 RID: 1729
	[Header("Loops")]
	public Sound sfxStunnedLoop;
}
