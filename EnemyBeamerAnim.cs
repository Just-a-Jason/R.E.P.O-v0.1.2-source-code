using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class EnemyBeamerAnim : MonoBehaviour
{
	// Token: 0x06000173 RID: 371 RVA: 0x0000E810 File Offset: 0x0000CA10
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.springNose01Speed = this.springNose01.speed;
		this.springNose01Damping = this.springNose01.damping;
		this.springNose02Speed = this.springNose02.speed;
		this.springNose02Damping = this.springNose02.damping;
		this.springNose03Speed = this.springNose03.speed;
		this.springNose03Damping = this.springNose03.damping;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0000E89C File Offset: 0x0000CA9C
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
		if (this.controller.currentState == EnemyBeamer.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.soundStunIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger(this.animStun);
				this.stunImpulse = false;
			}
			this.animator.SetBool(this.animStunned, true);
			if (this.soundHurtPauseTimer > 0f)
			{
				this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			}
			else
			{
				this.soundStunLoop.PlayLoop(true, 5f, 5f, 1f);
			}
			this.stunEndImpulse = true;
		}
		else
		{
			if (this.stunEndImpulse)
			{
				this.soundStunOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.stunEndImpulse = false;
			}
			this.animator.SetBool(this.animStunned, false);
			this.soundStunLoop.PlayLoop(false, 5f, 5f, 1f);
			this.stunImpulse = true;
		}
		if (this.noseEmission)
		{
			this.emissionColor = Color.Lerp(this.emissionColor, Color.white, Time.deltaTime * 10f);
		}
		else
		{
			this.emissionColor = Color.Lerp(this.emissionColor, Color.black, Time.deltaTime * 10f);
		}
		if (this.emissionColor != this.emissionColorPrevious)
		{
			this.emissionColorPrevious = this.emissionColor;
			this.enemy.Health.instancedMaterials[0].SetColor("_EmissionColor", this.emissionColor);
		}
		this.springNose01.speed = this.springNose01Speed * this.springSpeedMultiplier;
		this.springNose01.damping = this.springNose01Damping * this.springDampingMultiplier;
		this.springNose01Source.rotation = SemiFunc.SpringQuaternionGet(this.springNose01, this.springNose01Target.transform.rotation, -1f);
		this.springNose02.speed = this.springNose02Speed * this.springSpeedMultiplier;
		this.springNose02.damping = this.springNose02Damping * this.springDampingMultiplier;
		this.springNose02Source.rotation = SemiFunc.SpringQuaternionGet(this.springNose02, this.springNose02Target.transform.rotation, -1f);
		this.springNose03.speed = this.springNose03Speed * this.springSpeedMultiplier;
		this.springNose03.damping = this.springNose03Damping * this.springDampingMultiplier;
		this.springNose03Source.rotation = SemiFunc.SpringQuaternionGet(this.springNose03, this.springNose03Target.transform.rotation, -1f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!this.enemy.Jump.jumping && !this.enemy.IsStunned() && (this.controller.currentState == EnemyBeamer.State.MeleeStart || this.controller.currentState == EnemyBeamer.State.Melee))
			{
				if (this.meleeImpulse)
				{
					if (SemiFunc.IsMultiplayer())
					{
						this.controller.photonView.RPC("MeleeTriggerRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.animator.SetTrigger(this.animMelee);
					this.meleeImpulse = false;
				}
			}
			else
			{
				this.meleeImpulse = true;
			}
		}
		else if (this.meleeImpulse)
		{
			this.animator.SetTrigger(this.animMelee);
			this.meleeImpulse = false;
		}
		if (this.controller.currentState == EnemyBeamer.State.AttackStart || this.controller.currentState == EnemyBeamer.State.Attack)
		{
			if (this.attackImpulse)
			{
				this.attackImpulse = false;
				this.animator.SetTrigger(this.animAttack);
			}
			this.animator.SetBool(this.animAttacking, true);
		}
		else
		{
			this.attackImpulse = true;
			this.animator.SetBool(this.animAttacking, false);
		}
		if (this.soundHurtPauseTimer > 0f)
		{
			this.soundAttackLoop.PlayLoop(false, 5f, 5f, 1f);
		}
		else
		{
			this.soundAttackLoop.PlayLoop(this.soundAttackLoopActive, 5f, 5f, 1f);
		}
		if ((this.controller.currentState == EnemyBeamer.State.Roam || this.controller.currentState == EnemyBeamer.State.Investigate || this.controller.currentState == EnemyBeamer.State.Seek || this.controller.currentState == EnemyBeamer.State.Leave) && (this.enemy.Rigidbody.velocity.magnitude > 0.5f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 5f))
		{
			this.moveTimer = 0.25f;
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
		this.animator.SetBool(this.animMovingFast, this.controller.moveFast);
		if (this.enemy.Jump.jumping || this.enemy.Jump.jumpingDelay)
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
			else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.5f)
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
		if (this.controller.currentState == EnemyBeamer.State.Spawn)
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
		if (this.controller.currentState == EnemyBeamer.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
		}
		else
		{
			this.animator.SetBool(this.animDespawning, false);
		}
		if (this.soundHurtPauseTimer > 0f)
		{
			this.soundHurtPauseTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000EF79 File Offset: 0x0000D179
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000EF8C File Offset: 0x0000D18C
	public void FootstepSmall()
	{
		this.soundFootstepSmall.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Light, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000F008 File Offset: 0x0000D208
	public void FootstepBig()
	{
		GameDirector.instance.CameraShake.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 5f, 10f, base.transform.position, 0.1f);
		this.soundFootstepBig.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000F0E0 File Offset: 0x0000D2E0
	public void FootstepHuge()
	{
		GameDirector.instance.CameraShake.ShakeDistance(2.5f, 5f, 15f, base.transform.position, 0.25f);
		GameDirector.instance.CameraImpact.ShakeDistance(2.5f, 5f, 15f, base.transform.position, 0.1f);
		this.soundFootstepHuge.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000F1B8 File Offset: 0x0000D3B8
	public void Jump()
	{
		this.controller.particleBottomSmoke.transform.position = this.controller.bottomTransform.position;
		this.controller.particleBottomSmoke.Play();
		this.soundJump.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000F2C4 File Offset: 0x0000D4C4
	public void Land()
	{
		this.controller.particleBottomSmoke.transform.position = this.controller.bottomTransform.position;
		this.controller.particleBottomSmoke.Play();
		this.soundLand.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, base.transform.position, 0.1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 0.5f, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000F3CE File Offset: 0x0000D5CE
	public void MoveShort()
	{
		this.soundMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000F3FB File Offset: 0x0000D5FB
	public void MoveLong()
	{
		this.soundMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000F428 File Offset: 0x0000D628
	public void MeleeTell()
	{
		this.soundMeleeTell.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000F455 File Offset: 0x0000D655
	public void MeleeKick()
	{
		this.soundMeleeKick.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000F482 File Offset: 0x0000D682
	public void AttackIntro()
	{
		this.soundAttackIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000F4AF File Offset: 0x0000D6AF
	public void AttackOutro()
	{
		this.soundAttackOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040002F9 RID: 761
	public Enemy enemy;

	// Token: 0x040002FA RID: 762
	public EnemyBeamer controller;

	// Token: 0x040002FB RID: 763
	public bool noseEmission;

	// Token: 0x040002FC RID: 764
	private Animator animator;

	// Token: 0x040002FD RID: 765
	private int animSpawn = Animator.StringToHash("spawn");

	// Token: 0x040002FE RID: 766
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x040002FF RID: 767
	private int animStun = Animator.StringToHash("stun");

	// Token: 0x04000300 RID: 768
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x04000301 RID: 769
	private int animMoving = Animator.StringToHash("moving");

	// Token: 0x04000302 RID: 770
	private int animMovingFast = Animator.StringToHash("movingFast");

	// Token: 0x04000303 RID: 771
	private int animJump = Animator.StringToHash("jump");

	// Token: 0x04000304 RID: 772
	private int animFalling = Animator.StringToHash("falling");

	// Token: 0x04000305 RID: 773
	private int animLand = Animator.StringToHash("land");

	// Token: 0x04000306 RID: 774
	private int animMelee = Animator.StringToHash("melee");

	// Token: 0x04000307 RID: 775
	private int animAttacking = Animator.StringToHash("attacking");

	// Token: 0x04000308 RID: 776
	private int animAttack = Animator.StringToHash("attack");

	// Token: 0x04000309 RID: 777
	public float springSpeedMultiplier;

	// Token: 0x0400030A RID: 778
	public float springDampingMultiplier;

	// Token: 0x0400030B RID: 779
	[Space]
	public SpringQuaternion springNose01;

	// Token: 0x0400030C RID: 780
	private float springNose01Speed;

	// Token: 0x0400030D RID: 781
	private float springNose01Damping;

	// Token: 0x0400030E RID: 782
	public Transform springNose01Target;

	// Token: 0x0400030F RID: 783
	public Transform springNose01Source;

	// Token: 0x04000310 RID: 784
	public SpringQuaternion springNose02;

	// Token: 0x04000311 RID: 785
	private float springNose02Speed;

	// Token: 0x04000312 RID: 786
	private float springNose02Damping;

	// Token: 0x04000313 RID: 787
	public Transform springNose02Target;

	// Token: 0x04000314 RID: 788
	public Transform springNose02Source;

	// Token: 0x04000315 RID: 789
	public SpringQuaternion springNose03;

	// Token: 0x04000316 RID: 790
	private float springNose03Speed;

	// Token: 0x04000317 RID: 791
	private float springNose03Damping;

	// Token: 0x04000318 RID: 792
	public Transform springNose03Target;

	// Token: 0x04000319 RID: 793
	public Transform springNose03Source;

	// Token: 0x0400031A RID: 794
	private float moveTimer;

	// Token: 0x0400031B RID: 795
	private bool spawnImpulse = true;

	// Token: 0x0400031C RID: 796
	private bool stunImpulse = true;

	// Token: 0x0400031D RID: 797
	private bool stunEndImpulse;

	// Token: 0x0400031E RID: 798
	private bool jumpImpulse = true;

	// Token: 0x0400031F RID: 799
	private bool landImpulse = true;

	// Token: 0x04000320 RID: 800
	internal bool meleeImpulse = true;

	// Token: 0x04000321 RID: 801
	private bool attackImpulse = true;

	// Token: 0x04000322 RID: 802
	private Color emissionColor = Color.black;

	// Token: 0x04000323 RID: 803
	private Color emissionColorPrevious = Color.black;

	// Token: 0x04000324 RID: 804
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x04000325 RID: 805
	internal float soundHurtPauseTimer;

	// Token: 0x04000326 RID: 806
	public Sound soundMoveShort;

	// Token: 0x04000327 RID: 807
	public Sound soundMoveLong;

	// Token: 0x04000328 RID: 808
	[Space]
	public Sound soundFootstepSmall;

	// Token: 0x04000329 RID: 809
	public Sound soundFootstepBig;

	// Token: 0x0400032A RID: 810
	public Sound soundFootstepHuge;

	// Token: 0x0400032B RID: 811
	[Space]
	public Sound soundStunIntro;

	// Token: 0x0400032C RID: 812
	public Sound soundStunLoop;

	// Token: 0x0400032D RID: 813
	public Sound soundStunOutro;

	// Token: 0x0400032E RID: 814
	[Space]
	public Sound soundJump;

	// Token: 0x0400032F RID: 815
	public Sound soundLand;

	// Token: 0x04000330 RID: 816
	[Space]
	public Sound soundMeleeTell;

	// Token: 0x04000331 RID: 817
	public Sound soundMeleeKick;

	// Token: 0x04000332 RID: 818
	[Space]
	public bool soundAttackLoopActive;

	// Token: 0x04000333 RID: 819
	public Sound soundAttackIntro;

	// Token: 0x04000334 RID: 820
	public Sound soundAttackLoop;

	// Token: 0x04000335 RID: 821
	public Sound soundAttackOutro;

	// Token: 0x04000336 RID: 822
	[Space]
	public Sound soundHurt;

	// Token: 0x04000337 RID: 823
	public Sound soundDeath;
}
