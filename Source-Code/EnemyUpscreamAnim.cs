using System;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class EnemyUpscreamAnim : MonoBehaviour
{
	// Token: 0x06000531 RID: 1329 RVA: 0x00033A28 File Offset: 0x00031C28
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x00033A44 File Offset: 0x00031C44
	private void Update()
	{
		this.SetAnimationSpeed();
		if (this.controller.enemy.CurrentState == EnemyState.Despawn)
		{
			this.animator.SetBool("despawn", true);
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		if (this.controller.currentState == EnemyUpscream.State.IdleBreak)
		{
			if (this.idleBreakImpulse)
			{
				this.animator.SetTrigger("IdleBreak");
				this.idleBreakImpulse = false;
			}
		}
		else
		{
			this.idleBreakImpulse = true;
		}
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("jumping", true);
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump");
				this.animator.SetBool("falling", false);
				this.jumpImpulse = false;
			}
		}
		else
		{
			this.animator.SetBool("jumping", false);
			this.jumpImpulse = true;
		}
		if (this.enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
		{
			this.animator.SetBool("falling", true);
		}
		else
		{
			this.animator.SetBool("falling", false);
		}
		if (this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 0.1f || this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.5f)
		{
			this.moveTimer = 0.2f;
		}
		if (this.moveTimer > 0f)
		{
			this.moveTimer -= Time.deltaTime;
			this.animator.SetBool("move", true);
		}
		else
		{
			this.animator.SetBool("move", false);
		}
		if (this.controller.currentState == EnemyUpscream.State.Stun)
		{
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("Stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			return;
		}
		this.animator.SetBool("stunned", false);
		this.stunImpulse = true;
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x00033C5F File Offset: 0x00031E5F
	public void SetSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x00033C77 File Offset: 0x00031E77
	public void SetDespawn()
	{
		this.controller.UpdateState(EnemyUpscream.State.Spawn);
		this.controller.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x00033C9A File Offset: 0x00031E9A
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00033CAC File Offset: 0x00031EAC
	public void TeleportParticlesStart()
	{
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x00033CAE File Offset: 0x00031EAE
	public void TeleportParticlesStop()
	{
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x00033CB0 File Offset: 0x00031EB0
	public void SfxImpactFootstep()
	{
		if (this.enemy.Grounded.grounded)
		{
			this.stepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
		}
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x00033D27 File Offset: 0x00031F27
	public void SfxIdleBreak()
	{
		this.sfxIdleBreak.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x00033D54 File Offset: 0x00031F54
	public void SfxAttack()
	{
		this.sfxAttackLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00033DB7 File Offset: 0x00031FB7
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00033DE4 File Offset: 0x00031FE4
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x00033E11 File Offset: 0x00032011
	public void DespawnSound()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00033E40 File Offset: 0x00032040
	public void AttackImpulse()
	{
		if ((!SemiFunc.IsMultiplayer() || SemiFunc.IsMasterClient()) && this.controller.targetPlayer)
		{
			Vector3 a = (this.controller.targetPlayer.transform.position - base.transform.position).normalized;
			a = Vector3.Lerp(a, Vector3.up, 0.6f);
			this.controller.targetPlayer.tumble.TumbleRequest(true, false);
			this.controller.targetPlayer.tumble.TumbleForce(a * 45f);
			this.controller.targetPlayer.tumble.TumbleTorque(-this.controller.targetPlayer.transform.right * 45f);
			this.controller.targetPlayer.tumble.TumbleOverrideTime(3f);
			this.controller.targetPlayer.tumble.ImpactHurtSet(3f, 10);
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x00033F58 File Offset: 0x00032158
	private void SetAnimationSpeed()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
		{
			float num = this.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude + this.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude;
			num = Mathf.Clamp(num, 0.5f, 4f);
			this.animator.speed = num * 0.6f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
	}

	// Token: 0x04000846 RID: 2118
	public Enemy enemy;

	// Token: 0x04000847 RID: 2119
	public EnemyUpscream controller;

	// Token: 0x04000848 RID: 2120
	internal Animator animator;

	// Token: 0x04000849 RID: 2121
	public Materials.MaterialTrigger material;

	// Token: 0x0400084A RID: 2122
	private bool idleBreakImpulse;

	// Token: 0x0400084B RID: 2123
	private bool stunImpulse;

	// Token: 0x0400084C RID: 2124
	private bool jumpImpulse;

	// Token: 0x0400084D RID: 2125
	public Sound sfxAttackLocal;

	// Token: 0x0400084E RID: 2126
	public Sound sfxAttackGlobal;

	// Token: 0x0400084F RID: 2127
	public Sound hurtSound;

	// Token: 0x04000850 RID: 2128
	public Sound jumpSound;

	// Token: 0x04000851 RID: 2129
	public Sound landSound;

	// Token: 0x04000852 RID: 2130
	public Sound stepSound;

	// Token: 0x04000853 RID: 2131
	public Sound sfxIdleBreak;

	// Token: 0x04000854 RID: 2132
	public Sound despawnSound;

	// Token: 0x04000855 RID: 2133
	public bool isPlayingTargetPlayerLoop;

	// Token: 0x04000856 RID: 2134
	private float currentSpeed;

	// Token: 0x04000857 RID: 2135
	private float moveTimer;
}
