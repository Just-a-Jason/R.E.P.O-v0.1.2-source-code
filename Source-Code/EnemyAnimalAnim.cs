using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class EnemyAnimalAnim : MonoBehaviour
{
	// Token: 0x060000EE RID: 238 RVA: 0x00008F48 File Offset: 0x00007148
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00008F64 File Offset: 0x00007164
	private void Update()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !this.animator.IsInTransition(0))
		{
			this.animator.speed = Mathf.Clamp(this.enemy.Rigidbody.velocity.magnitude / 5f, 0.6f, 2f);
			this.attackPitch = Mathf.Clamp(this.enemy.Rigidbody.velocity.magnitude / 4.75f, 0.75f, 1.25f);
			this.attack = true;
		}
		else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !this.animator.IsInTransition(0))
		{
			this.animator.speed = Mathf.Clamp(this.enemy.Rigidbody.velocity.magnitude / 2f, 0.8f, 2f);
		}
		else
		{
			this.animator.speed = 1f;
			this.attack = false;
		}
		this.attackLoop.PlayLoop(this.attack, 5f, 5f, this.attackPitch);
		if (!this.previousAttack && this.attack)
		{
			this.attackStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.previousAttack = true;
		}
		if (this.previousAttack && !this.attack)
		{
			this.attackStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.previousAttack = false;
		}
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		if (Vector3.Dot(this.enemy.Rigidbody.transform.up, Vector3.up) > 0.6f)
		{
			this.animator.SetBool("upright", true);
		}
		else
		{
			this.animator.SetBool("upright", false);
		}
		if (this.enemy.Rigidbody.velocity.y < -2f)
		{
			this.animator.SetBool("falling", true);
		}
		else
		{
			this.animator.SetBool("falling", false);
		}
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("jump", true);
		}
		else
		{
			this.animator.SetBool("jump", false);
		}
		if (this.enemy.IsStunned())
		{
			this.stun = true;
			this.animator.SetBool("stun", true);
		}
		else
		{
			this.stun = false;
			this.animator.SetBool("stun", false);
		}
		if (this.stun && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
		{
			this.animator.SetTrigger("Stun Impulse");
		}
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.animator.SetBool("despawn", true);
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		if (this.enemy.Rigidbody.velocity.magnitude > 0.2f)
		{
			this.animator.SetBool("move", true);
		}
		else
		{
			this.animator.SetBool("move", false);
		}
		if (this.controller.currentState == EnemyAnimal.State.WreakHavoc)
		{
			this.animator.SetBool("attack", true);
			return;
		}
		this.animator.SetBool("attack", false);
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000931C File Offset: 0x0000751C
	public void SetSpawn()
	{
		this.stun = false;
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x0000933B File Offset: 0x0000753B
	public void SetDespawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0000934D File Offset: 0x0000754D
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00009360 File Offset: 0x00007560
	public void MaterialImpactShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.stompSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00009424 File Offset: 0x00007624
	public void ImpactLight()
	{
		if (this.enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, false, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00009470 File Offset: 0x00007670
	public void ImpactMedium()
	{
		if (this.enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, false, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x000094BC File Offset: 0x000076BC
	public void ImpactHeavy()
	{
		if (this.enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, false, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00009508 File Offset: 0x00007708
	public void ImpactFootstep()
	{
		if (this.enemy.Grounded.grounded)
		{
			this.stepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000957F File Offset: 0x0000777F
	public void Dig()
	{
		this.particleDig.Play();
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x0000958C File Offset: 0x0000778C
	public void Notice()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00009620 File Offset: 0x00007820
	public void StunStop()
	{
		this.attackStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000964D File Offset: 0x0000784D
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x0000967A File Offset: 0x0000787A
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000096A7 File Offset: 0x000078A7
	public void DespawnSound()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400022E RID: 558
	private Animator animator;

	// Token: 0x0400022F RID: 559
	public Enemy enemy;

	// Token: 0x04000230 RID: 560
	public EnemyAnimal controller;

	// Token: 0x04000231 RID: 561
	public Materials.MaterialTrigger material;

	// Token: 0x04000232 RID: 562
	private bool stun;

	// Token: 0x04000233 RID: 563
	private bool attack;

	// Token: 0x04000234 RID: 564
	private bool previousAttack;

	// Token: 0x04000235 RID: 565
	private float attackPitch;

	// Token: 0x04000236 RID: 566
	[Space]
	public ParticleSystem particleBits;

	// Token: 0x04000237 RID: 567
	public ParticleSystem particleImpact;

	// Token: 0x04000238 RID: 568
	public ParticleSystem particleDirectionalBits;

	// Token: 0x04000239 RID: 569
	public ParticleSystem particleLegBits;

	// Token: 0x0400023A RID: 570
	public ParticleSystem particleDig;

	// Token: 0x0400023B RID: 571
	[Space]
	public Sound stepSound;

	// Token: 0x0400023C RID: 572
	public Sound stompSound;

	// Token: 0x0400023D RID: 573
	[Space]
	public Sound moveShortSound;

	// Token: 0x0400023E RID: 574
	public Sound moveLongSound;

	// Token: 0x0400023F RID: 575
	[Space]
	public Sound spawnSound;

	// Token: 0x04000240 RID: 576
	public Sound despawnSound;

	// Token: 0x04000241 RID: 577
	[Space]
	public Sound jumpSound;

	// Token: 0x04000242 RID: 578
	public Sound landSound;

	// Token: 0x04000243 RID: 579
	public Sound noticeSound;

	// Token: 0x04000244 RID: 580
	[Space]
	public Sound attackStartSound;

	// Token: 0x04000245 RID: 581
	public Sound attackLoop;

	// Token: 0x04000246 RID: 582
	public Sound attackStopSound;

	// Token: 0x04000247 RID: 583
	[Space]
	public Sound hurtSound;

	// Token: 0x04000248 RID: 584
	public Sound deathSound;
}
