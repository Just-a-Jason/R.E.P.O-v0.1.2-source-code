using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class EnemySlowWalkerAnim : MonoBehaviour
{
	// Token: 0x06000681 RID: 1665 RVA: 0x0003EE68 File Offset: 0x0003D068
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.springNeck01Speed = this.springNeck01.speed;
		this.springNeck01Damping = this.springNeck01.damping;
		this.springNeck02Speed = this.springNeck02.speed;
		this.springNeck02Damping = this.springNeck02.damping;
		this.springNeck03Speed = this.springNeck03.speed;
		this.springNeck03Damping = this.springNeck03.damping;
		this.springEyeFleshSpeed = this.springEyeFlesh.speed;
		this.springEyeFleshDamping = this.springEyeFlesh.damping;
		this.springEyeBallSpeed = this.springEyeBall.speed;
		this.springEyeBallDamping = this.springEyeBall.damping;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0003EF38 File Offset: 0x0003D138
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
		if (!this.stunned && (this.enemy.Jump.jumping || this.enemy.Jump.jumpingDelay))
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger(this.animJump);
				this.animator.SetBool(this.animFalling, false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < -1f)
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
		if (this.controller.currentState == EnemySlowWalker.State.LookUnder || this.controller.currentState == EnemySlowWalker.State.LookUnderIntro || this.controller.currentState == EnemySlowWalker.State.LookUnderAttack)
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
		if (this.controller.currentState == EnemySlowWalker.State.LookUnderAttack)
		{
			if (this.lookUnderAttackImpulse)
			{
				this.animator.SetTrigger(this.animLookUnderAttack);
				this.lookUnderAttackImpulse = false;
			}
		}
		else
		{
			this.lookUnderAttackImpulse = true;
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
		if (this.controller.currentState == EnemySlowWalker.State.Stun)
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
		this.SfxStunnedLoop();
		if (this.controller.currentState == EnemySlowWalker.State.Notice)
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
		if (this.controller.currentState == EnemySlowWalker.State.Attack)
		{
			if (this.attackImpulse)
			{
				this.animator.SetTrigger(this.animAttack);
				this.attackImpulse = false;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
		if (this.controller.currentState == EnemySlowWalker.State.StuckAttack)
		{
			if (this.stuckAttackImpulse)
			{
				this.animator.SetTrigger(this.animStuckAttack);
				this.stuckAttackImpulse = false;
			}
		}
		else
		{
			this.stuckAttackImpulse = true;
		}
		if (this.controller.currentState == EnemySlowWalker.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0003F2E4 File Offset: 0x0003D4E4
	public void AttackOffsetStart()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.controller.attackOffsetActive = true;
		}
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0003F2F9 File Offset: 0x0003D4F9
	public void AttackOffsetStop()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.controller.attackOffsetActive = false;
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0003F30E File Offset: 0x0003D50E
	public void AttackStart()
	{
		this.slowWalkerAttack.AttackStart();
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0003F31B File Offset: 0x0003D51B
	private void VfxJump()
	{
		this.slowWalkerJumpEffect.JumpEffect();
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0003F328 File Offset: 0x0003D528
	private void VfxLand()
	{
		this.slowWalkerJumpEffect.LandEffect();
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0003F335 File Offset: 0x0003D535
	public void VfxSparkStart()
	{
		this.slowWalkerSparkEffect.PlaySparkEffect();
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0003F342 File Offset: 0x0003D542
	public void VfxSparkStop()
	{
		this.slowWalkerSparkEffect.StopSparkEffect();
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0003F350 File Offset: 0x0003D550
	public void SfxFootstepSmall()
	{
		this.sfxFootstepSmall.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 5f, 10f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Light, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0003F428 File Offset: 0x0003D628
	public void SfxFootstepBig()
	{
		this.sfxFootstepBig.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0003F500 File Offset: 0x0003D700
	public void SfxJump()
	{
		this.sfxJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
		this.VfxJump();
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0003F5DC File Offset: 0x0003D7DC
	public void SfxLand()
	{
		this.sfxLand.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
		this.VfxLand();
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0003F6B7 File Offset: 0x0003D8B7
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.SfxNoiseShort();
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0003F6EA File Offset: 0x0003D8EA
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.SfxNoiseLong();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0003F71D File Offset: 0x0003D91D
	public void SfxAttackBuildupVoice()
	{
		this.sfxAttackBuildupVoice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0003F74A File Offset: 0x0003D94A
	public void SfxAttackImpact()
	{
		this.sfxAttackImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0003F777 File Offset: 0x0003D977
	public void SfxAttackImplosionBuildup()
	{
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x0003F779 File Offset: 0x0003D979
	public void SfxAttackImplosionHit()
	{
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0003F77B File Offset: 0x0003D97B
	public void SfxAttackImplosionImpact()
	{
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0003F77D File Offset: 0x0003D97D
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0003F7AA File Offset: 0x0003D9AA
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0003F7D7 File Offset: 0x0003D9D7
	public void SfxNoiseShort()
	{
		if (Random.value <= 0.6f)
		{
			this.sfxNoiseShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x0003F810 File Offset: 0x0003DA10
	public void SfxNoiseLong()
	{
		if (Random.value <= 0.6f)
		{
			this.sfxNoiseLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0003F849 File Offset: 0x0003DA49
	public void SfxNoticeVoice()
	{
		this.sfxNoticeVoice.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0003F876 File Offset: 0x0003DA76
	public void SfxSwingShort()
	{
		this.sfxSwingShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x0003F8A3 File Offset: 0x0003DAA3
	public void SfxSwingLong()
	{
		this.sfxSwingLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0003F8D0 File Offset: 0x0003DAD0
	public void SfxMaceTrailing()
	{
		this.sfxMaceTrailing.Play(this.maceTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0003F8FD File Offset: 0x0003DAFD
	public void SfxLookUnderIntro()
	{
		this.sfxLookUnderIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0003F92A File Offset: 0x0003DB2A
	public void SfxLookUnderAttack()
	{
		this.sfxLookUnderAttack.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x0003F957 File Offset: 0x0003DB57
	public void SfxLookUnderOutro()
	{
		this.sfxLookUnderOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0003F984 File Offset: 0x0003DB84
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0003F9A6 File Offset: 0x0003DBA6
	public void OnSpawn()
	{
		this.animator.SetBool(this.animStunned, false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0003F9D0 File Offset: 0x0003DBD0
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0003F9E2 File Offset: 0x0003DBE2
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger(this.animNotice);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x0003F9F8 File Offset: 0x0003DBF8
	private bool IsMoving()
	{
		return this.controller.currentState == EnemySlowWalker.State.Roam || this.controller.currentState == EnemySlowWalker.State.Investigate || this.controller.currentState == EnemySlowWalker.State.GoToPlayer || this.controller.currentState == EnemySlowWalker.State.LookUnderStart || this.controller.currentState == EnemySlowWalker.State.Sneak || this.controller.currentState == EnemySlowWalker.State.Leave;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x0003FA60 File Offset: 0x0003DC60
	public void SpringLogic()
	{
		this.springNeck01.speed = this.springNeck01Speed * this.springSpeedMultiplier;
		this.springNeck01.damping = this.springNeck01Damping * this.springDampingMultiplier;
		this.springNeck01Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck01, this.springNeck01Target.transform.rotation, -1f);
		this.springNeck02.speed = this.springNeck02Speed * this.springSpeedMultiplier;
		this.springNeck02.damping = this.springNeck02Damping * this.springDampingMultiplier;
		this.springNeck02Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck02, this.springNeck02Target.transform.rotation, -1f);
		this.springNeck03.speed = this.springNeck03Speed * this.springSpeedMultiplier;
		this.springNeck03.damping = this.springNeck03Damping * this.springDampingMultiplier;
		this.springNeck03Source.rotation = SemiFunc.SpringQuaternionGet(this.springNeck03, this.springNeck03Target.transform.rotation, -1f);
		this.springEyeFlesh.speed = this.springEyeFleshSpeed * this.springSpeedMultiplier;
		this.springEyeFlesh.damping = this.springEyeFleshDamping * this.springDampingMultiplier;
		this.springEyeFleshSource.rotation = SemiFunc.SpringQuaternionGet(this.springEyeFlesh, this.springEyeFleshTarget.transform.rotation, -1f);
		this.springEyeBall.speed = this.springEyeBallSpeed * this.springSpeedMultiplier;
		this.springEyeBall.damping = this.springEyeBallDamping * this.springDampingMultiplier;
		this.springEyeBallSource.rotation = SemiFunc.SpringQuaternionGet(this.springEyeBall, this.springEyeBallTarget.transform.rotation, -1f);
	}

	// Token: 0x04000AD9 RID: 2777
	public Enemy enemy;

	// Token: 0x04000ADA RID: 2778
	public EnemySlowWalker controller;

	// Token: 0x04000ADB RID: 2779
	public Transform maceTransform;

	// Token: 0x04000ADC RID: 2780
	internal Animator animator;

	// Token: 0x04000ADD RID: 2781
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x04000ADE RID: 2782
	public SlowWalkerSparkEffect slowWalkerSparkEffect;

	// Token: 0x04000ADF RID: 2783
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x04000AE0 RID: 2784
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x04000AE1 RID: 2785
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x04000AE2 RID: 2786
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x04000AE3 RID: 2787
	private int animLookingUnder = Animator.StringToHash("lookingUnder");

	// Token: 0x04000AE4 RID: 2788
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x04000AE5 RID: 2789
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x04000AE6 RID: 2790
	private int animAttack = Animator.StringToHash("Attack");

	// Token: 0x04000AE7 RID: 2791
	private int animJump = Animator.StringToHash("Jump");

	// Token: 0x04000AE8 RID: 2792
	private int animLand = Animator.StringToHash("Land");

	// Token: 0x04000AE9 RID: 2793
	private int animLookUnder = Animator.StringToHash("LookUnder");

	// Token: 0x04000AEA RID: 2794
	private int animLookUnderAttack = Animator.StringToHash("LookUnderAttack");

	// Token: 0x04000AEB RID: 2795
	private int animStuckAttack = Animator.StringToHash("StuckAttack");

	// Token: 0x04000AEC RID: 2796
	public float springSpeedMultiplier = 1f;

	// Token: 0x04000AED RID: 2797
	public float springDampingMultiplier = 1f;

	// Token: 0x04000AEE RID: 2798
	public SpringQuaternion springNeck01;

	// Token: 0x04000AEF RID: 2799
	private float springNeck01Speed;

	// Token: 0x04000AF0 RID: 2800
	private float springNeck01Damping;

	// Token: 0x04000AF1 RID: 2801
	public Transform springNeck01Target;

	// Token: 0x04000AF2 RID: 2802
	public Transform springNeck01Source;

	// Token: 0x04000AF3 RID: 2803
	public SpringQuaternion springNeck02;

	// Token: 0x04000AF4 RID: 2804
	private float springNeck02Speed;

	// Token: 0x04000AF5 RID: 2805
	private float springNeck02Damping;

	// Token: 0x04000AF6 RID: 2806
	public Transform springNeck02Target;

	// Token: 0x04000AF7 RID: 2807
	public Transform springNeck02Source;

	// Token: 0x04000AF8 RID: 2808
	public SpringQuaternion springNeck03;

	// Token: 0x04000AF9 RID: 2809
	private float springNeck03Speed;

	// Token: 0x04000AFA RID: 2810
	private float springNeck03Damping;

	// Token: 0x04000AFB RID: 2811
	public Transform springNeck03Target;

	// Token: 0x04000AFC RID: 2812
	public Transform springNeck03Source;

	// Token: 0x04000AFD RID: 2813
	public SpringQuaternion springEyeFlesh;

	// Token: 0x04000AFE RID: 2814
	private float springEyeFleshSpeed;

	// Token: 0x04000AFF RID: 2815
	private float springEyeFleshDamping;

	// Token: 0x04000B00 RID: 2816
	public Transform springEyeFleshTarget;

	// Token: 0x04000B01 RID: 2817
	public Transform springEyeFleshSource;

	// Token: 0x04000B02 RID: 2818
	public SpringQuaternion springEyeBall;

	// Token: 0x04000B03 RID: 2819
	private float springEyeBallSpeed;

	// Token: 0x04000B04 RID: 2820
	private float springEyeBallDamping;

	// Token: 0x04000B05 RID: 2821
	public Transform springEyeBallTarget;

	// Token: 0x04000B06 RID: 2822
	public Transform springEyeBallSource;

	// Token: 0x04000B07 RID: 2823
	private bool stunned;

	// Token: 0x04000B08 RID: 2824
	private bool stunImpulse;

	// Token: 0x04000B09 RID: 2825
	private bool noticeImpulse;

	// Token: 0x04000B0A RID: 2826
	private bool delayAttackImpulse;

	// Token: 0x04000B0B RID: 2827
	private bool attackImpulse;

	// Token: 0x04000B0C RID: 2828
	private bool chargeAttackImpulse;

	// Token: 0x04000B0D RID: 2829
	private bool jumpImpulse;

	// Token: 0x04000B0E RID: 2830
	private bool landImpulse;

	// Token: 0x04000B0F RID: 2831
	private bool lookUnderImpulse;

	// Token: 0x04000B10 RID: 2832
	private bool lookUnderAttackImpulse;

	// Token: 0x04000B11 RID: 2833
	private bool stuckAttackImpulse;

	// Token: 0x04000B12 RID: 2834
	private float moveTimer;

	// Token: 0x04000B13 RID: 2835
	private float jumpedTimer;

	// Token: 0x04000B14 RID: 2836
	public Sound sfxFootstepSmall;

	// Token: 0x04000B15 RID: 2837
	public Sound sfxFootstepBig;

	// Token: 0x04000B16 RID: 2838
	public Sound sfxJump;

	// Token: 0x04000B17 RID: 2839
	public Sound sfxLand;

	// Token: 0x04000B18 RID: 2840
	public Sound sfxMoveShort;

	// Token: 0x04000B19 RID: 2841
	public Sound sfxMoveLong;

	// Token: 0x04000B1A RID: 2842
	public Sound sfxAttackBuildupVoice;

	// Token: 0x04000B1B RID: 2843
	public Sound sfxAttackImpact;

	// Token: 0x04000B1C RID: 2844
	public Sound sfxAttackImplosionBuildup;

	// Token: 0x04000B1D RID: 2845
	public Sound sfxAttackImplosionHitLocal;

	// Token: 0x04000B1E RID: 2846
	public Sound sfxAttackImplosionHitGlobal;

	// Token: 0x04000B1F RID: 2847
	public Sound sfxAttackImplosionImpactLocal;

	// Token: 0x04000B20 RID: 2848
	public Sound sfxAttackImplosionImpactGlobal;

	// Token: 0x04000B21 RID: 2849
	public Sound sfxDeath;

	// Token: 0x04000B22 RID: 2850
	public Sound sfxHurt;

	// Token: 0x04000B23 RID: 2851
	public Sound sfxNoiseShort;

	// Token: 0x04000B24 RID: 2852
	public Sound sfxNoiseLong;

	// Token: 0x04000B25 RID: 2853
	public Sound sfxNoticeVoice;

	// Token: 0x04000B26 RID: 2854
	public Sound sfxSwingShort;

	// Token: 0x04000B27 RID: 2855
	public Sound sfxSwingLong;

	// Token: 0x04000B28 RID: 2856
	public Sound sfxMaceTrailing;

	// Token: 0x04000B29 RID: 2857
	public Sound sfxLookUnderIntro;

	// Token: 0x04000B2A RID: 2858
	public Sound sfxLookUnderAttack;

	// Token: 0x04000B2B RID: 2859
	public Sound sfxLookUnderOutro;

	// Token: 0x04000B2C RID: 2860
	public Sound sfxStunnedLoop;

	// Token: 0x04000B2D RID: 2861
	public SlowWalkerAttack slowWalkerAttack;

	// Token: 0x04000B2E RID: 2862
	public SlowWalkerJumpEffect slowWalkerJumpEffect;
}
