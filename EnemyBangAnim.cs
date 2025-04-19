using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class EnemyBangAnim : MonoBehaviour
{
	// Token: 0x0600012B RID: 299 RVA: 0x0000B610 File Offset: 0x00009810
	private void Awake()
	{
		this.soundImpactLight.Volume *= this.volumeMultiplier;
		this.soundImpactMedium.Volume *= this.volumeMultiplier;
		this.soundImpactHeavy.Volume *= this.volumeMultiplier;
		this.soundMoveShort.Volume *= this.volumeMultiplier;
		this.soundMoveLong.Volume *= this.volumeMultiplier;
		this.soundFootstep.Volume *= this.volumeMultiplier;
		this.soundHurt.Volume *= this.volumeMultiplier;
		this.soundDeathSFX.Volume *= this.volumeMultiplier;
		this.soundDeathVO.Volume *= this.volumeMultiplier;
		this.soundJumpSFX.Volume *= this.volumeMultiplier;
		this.soundJumpVO.Volume *= this.volumeMultiplier;
		this.soundLandSFX.Volume *= this.volumeMultiplier;
		this.soundLandVO.Volume *= this.volumeMultiplier;
		this.soundStunIntro.Volume *= this.volumeMultiplier;
		this.soundStunLoop.Volume *= this.volumeMultiplier;
		this.soundStunOutro.Volume *= this.volumeMultiplier;
		this.soundIdleBreaker.Volume *= this.volumeMultiplier;
		this.soundAttackBreaker.Volume *= this.volumeMultiplier;
		this.soundFuseTell.Volume *= this.volumeMultiplier;
		this.soundFuseIgnite.Volume *= this.volumeMultiplier;
		this.soundFuseLoop.Volume *= this.volumeMultiplier;
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000B830 File Offset: 0x00009A30
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
		if (this.controller.currentState == EnemyBang.State.Spawn)
		{
			if (this.spawnImpulse)
			{
				this.spawnImpulse = false;
				this.animator.SetTrigger(this.animSpawn);
			}
		}
		else
		{
			this.spawnImpulse = true;
		}
		if ((this.controller.currentState == EnemyBang.State.Roam || this.controller.currentState == EnemyBang.State.Move || this.controller.currentState == EnemyBang.State.MoveUnder || this.controller.currentState == EnemyBang.State.MoveOver || this.controller.currentState == EnemyBang.State.MoveBack) && (this.enemy.Rigidbody.velocity.magnitude > 1f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 1f))
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
		if (this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				if (!this.enemy.IsStunned())
				{
					this.animator.SetTrigger(this.animJump);
				}
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
				if (!this.enemy.IsStunned())
				{
					this.animator.SetTrigger(this.animLand);
				}
				this.moveTimer = 0f;
				this.landImpulse = false;
			}
			this.animator.SetBool(this.animFalling, false);
			this.jumpImpulse = true;
		}
		if (this.controller.currentState == EnemyBang.State.Fuse)
		{
			if (this.fuseImpulse)
			{
				this.animator.SetTrigger(this.animFuse);
				this.fuseImpulse = false;
			}
		}
		else
		{
			this.fuseImpulse = true;
		}
		if (this.fuseLoopTimer > 0f)
		{
			this.fuseLoopTimer -= Time.deltaTime;
			this.soundFuseLoop.PlayLoop(true, 2f, 2f, 1f + this.soundFuseLoopCurve.Evaluate(this.controller.fuseLerp) * 4f);
		}
		else
		{
			this.soundFuseLoop.PlayLoop(false, 2f, 2f, 1f + this.soundFuseLoopCurve.Evaluate(this.controller.fuseLerp) * 4f);
		}
		if (this.controller.currentState == EnemyBang.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.soundStunIntro.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger(this.animStun);
				this.stunImpulse = false;
			}
			if (this.stunLoopPauseTimer <= 0f)
			{
				this.soundStunLoop.PlayLoop(true, 5f, 5f, 1f);
			}
			else
			{
				this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			}
			this.animator.SetBool(this.animStunned, true);
		}
		else
		{
			this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			this.animator.SetBool(this.animStunned, false);
			if (!this.stunImpulse)
			{
				this.soundStunOutro.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.stunImpulse = true;
			}
		}
		if (this.stunLoopPauseTimer > 0f)
		{
			this.stunLoopPauseTimer -= Time.deltaTime;
		}
		if (this.controller.currentState == EnemyBang.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000BCB1 File Offset: 0x00009EB1
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600012E RID: 302 RVA: 0x0000BCC3 File Offset: 0x00009EC3
	public void FuseTellPlay()
	{
		this.soundFuseTell.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000BCFC File Offset: 0x00009EFC
	public void FootstepPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
		this.soundFootstep.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0000BD80 File Offset: 0x00009F80
	public void JumpPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Medium, false, this.material, Materials.HostType.Enemy);
		this.soundJumpSFX.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundJumpVO.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000BE38 File Offset: 0x0000A038
	public void LandPlay()
	{
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.25f, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		this.soundLandSFX.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		this.soundLandVO.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000BEF0 File Offset: 0x0000A0F0
	public void MoveShortPlay()
	{
		this.soundMoveShort.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000BF27 File Offset: 0x0000A127
	public void MoveLongPlay()
	{
		this.soundMoveLong.Play(this.controller.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000BF5E File Offset: 0x0000A15E
	public void FuseLoop()
	{
		this.fuseLoopTimer = 0.1f;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000BF6B File Offset: 0x0000A16B
	public void StunLoopPause(float _time)
	{
		this.stunLoopPauseTimer = _time;
	}

	// Token: 0x04000279 RID: 633
	public Enemy enemy;

	// Token: 0x0400027A RID: 634
	public EnemyBang controller;

	// Token: 0x0400027B RID: 635
	internal Animator animator;

	// Token: 0x0400027C RID: 636
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x0400027D RID: 637
	private int animSpawn = Animator.StringToHash("spawn");

	// Token: 0x0400027E RID: 638
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x0400027F RID: 639
	private int animStun = Animator.StringToHash("stun");

	// Token: 0x04000280 RID: 640
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x04000281 RID: 641
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x04000282 RID: 642
	private int animJump = Animator.StringToHash("jump");

	// Token: 0x04000283 RID: 643
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x04000284 RID: 644
	private int animLand = Animator.StringToHash("land");

	// Token: 0x04000285 RID: 645
	private int animFuse = Animator.StringToHash("fuse");

	// Token: 0x04000286 RID: 646
	private float moveTimer;

	// Token: 0x04000287 RID: 647
	private bool spawnImpulse = true;

	// Token: 0x04000288 RID: 648
	private bool stunImpulse = true;

	// Token: 0x04000289 RID: 649
	private bool jumpImpulse = true;

	// Token: 0x0400028A RID: 650
	private bool landImpulse = true;

	// Token: 0x0400028B RID: 651
	private bool fuseImpulse = true;

	// Token: 0x0400028C RID: 652
	[Range(0f, 1f)]
	public float volumeMultiplier;

	// Token: 0x0400028D RID: 653
	[Space]
	public Sound soundImpactLight;

	// Token: 0x0400028E RID: 654
	public Sound soundImpactMedium;

	// Token: 0x0400028F RID: 655
	public Sound soundImpactHeavy;

	// Token: 0x04000290 RID: 656
	[Space]
	public Sound soundMoveShort;

	// Token: 0x04000291 RID: 657
	public Sound soundMoveLong;

	// Token: 0x04000292 RID: 658
	[Space]
	public Sound soundFootstep;

	// Token: 0x04000293 RID: 659
	[Space]
	public Sound soundHurt;

	// Token: 0x04000294 RID: 660
	public Sound soundDeathSFX;

	// Token: 0x04000295 RID: 661
	public Sound soundDeathVO;

	// Token: 0x04000296 RID: 662
	[Space]
	public Sound soundJumpSFX;

	// Token: 0x04000297 RID: 663
	public Sound soundJumpVO;

	// Token: 0x04000298 RID: 664
	[Space]
	public Sound soundLandSFX;

	// Token: 0x04000299 RID: 665
	public Sound soundLandVO;

	// Token: 0x0400029A RID: 666
	[Space]
	public Sound soundStunIntro;

	// Token: 0x0400029B RID: 667
	public Sound soundStunLoop;

	// Token: 0x0400029C RID: 668
	public Sound soundStunOutro;

	// Token: 0x0400029D RID: 669
	private float stunLoopPauseTimer;

	// Token: 0x0400029E RID: 670
	[Space]
	public Sound soundIdleBreaker;

	// Token: 0x0400029F RID: 671
	public Sound soundAttackBreaker;

	// Token: 0x040002A0 RID: 672
	[Space]
	public Sound soundFuseTell;

	// Token: 0x040002A1 RID: 673
	public Sound soundExplosionTell;

	// Token: 0x040002A2 RID: 674
	[Space]
	public Sound soundFuseIgnite;

	// Token: 0x040002A3 RID: 675
	public Sound soundFuseLoop;

	// Token: 0x040002A4 RID: 676
	public AnimationCurve soundFuseLoopCurve;

	// Token: 0x040002A5 RID: 677
	private float fuseLoopTimer;
}
