using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class EnemyDuckAnim : MonoBehaviour
{
	// Token: 0x0600021A RID: 538 RVA: 0x00015BC7 File Offset: 0x00013DC7
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00015BE4 File Offset: 0x00013DE4
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !this.animator.IsInTransition(0))
		{
			this.animator.speed = Mathf.Clamp(this.enemy.Rigidbody.velocity.magnitude + 0.2f, 0.8f, 1.2f);
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.currentState != EnemyDuck.State.AttackStart && this.controller.currentState != EnemyDuck.State.Transform && this.controller.currentState != EnemyDuck.State.ChaseNavmesh && this.controller.currentState != EnemyDuck.State.ChaseTowards && this.controller.currentState != EnemyDuck.State.ChaseMoveBack && this.controller.currentState != EnemyDuck.State.DeTransform)
		{
			if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
			{
				this.animator.SetBool("move", true);
			}
			else
			{
				this.animator.SetBool("move", false);
			}
			if (!this.enemy.IsStunned())
			{
				if (!this.enemy.Grounded.grounded && (this.controller.currentState == EnemyDuck.State.FlyBackToNavmesh || this.controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop))
				{
					if (this.flyImpulse)
					{
						this.animator.SetTrigger("fly");
						this.animator.SetBool("falling", false);
						this.flyImpulse = false;
						this.landImpulse = true;
					}
					else if (this.controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop)
					{
						this.animator.SetBool("falling", true);
					}
				}
				else if (this.enemy.Jump.jumping)
				{
					if (this.jumpImpulse)
					{
						this.animator.SetTrigger("jump");
						this.animator.SetBool("falling", false);
						this.jumpImpulse = false;
						this.landImpulse = true;
					}
					else if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
					{
						this.animator.SetBool("falling", true);
					}
				}
				else
				{
					if (this.landImpulse)
					{
						this.animator.SetTrigger("land");
						this.landImpulse = false;
					}
					this.animator.SetBool("falling", false);
					this.jumpImpulse = true;
					this.flyImpulse = true;
				}
			}
		}
		if (this.controller.currentState == EnemyDuck.State.AttackStart)
		{
			if (this.transformImpulse)
			{
				this.animator.SetTrigger("transform");
				this.transformImpulse = false;
			}
		}
		else
		{
			this.transformImpulse = true;
		}
		if (this.controller.currentState == EnemyDuck.State.AttackStart || this.controller.currentState == EnemyDuck.State.Transform || this.controller.currentState == EnemyDuck.State.ChaseNavmesh || this.controller.currentState == EnemyDuck.State.ChaseTowards || this.controller.currentState == EnemyDuck.State.ChaseMoveBack)
		{
			this.animator.SetBool("move", false);
			this.animator.SetBool("chase", true);
			if (this.soundHurtPauseTimer > 0f)
			{
				this.StopAttackSound();
			}
			else
			{
				this.attackLoopSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.StopAttackSound();
			this.animator.SetBool("chase", false);
		}
		if (this.controller.currentState == EnemyDuck.State.Notice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger("notice");
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !this.animator.IsInTransition(0))
		{
			this.idleBreakerTimer += Time.deltaTime;
			if (this.idleBreakerTimer > 5f)
			{
				this.idleBreakerTimer = 0f;
				if (Random.Range(0, 100) < 35)
				{
					this.controller.IdleBreakerSet();
				}
			}
		}
		if (this.controller.idleBreakerTrigger)
		{
			this.animator.SetTrigger("idlebreak");
			this.controller.idleBreakerTrigger = false;
		}
		if (this.controller.currentState == EnemyDuck.State.Stun)
		{
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.animator.SetTrigger("stun");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			if (this.soundHurtPauseTimer > 0f)
			{
				this.stunSound.PlayLoop(false, 5f, 2f, 1f);
			}
			else
			{
				this.stunSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.animator.SetBool("stunned", false);
			this.stunSound.PlayLoop(false, 5f, 1f, 1f);
			if (!this.stunImpulse)
			{
				this.stunStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.stunImpulse = true;
			}
		}
		if (this.controller.currentState == EnemyDuck.State.Despawn)
		{
			this.animator.SetBool("despawning", true);
		}
		else
		{
			this.animator.SetBool("despawning", false);
		}
		if (this.controller.currentState == EnemyDuck.State.FlyBackToNavmesh)
		{
			this.flyLoopSound.PlayLoop(true, 5f, 2f, 1f);
		}
		else
		{
			this.flyLoopSound.PlayLoop(false, 5f, 2f, 1f);
		}
		if (this.soundHurtPauseTimer > 0f)
		{
			this.soundHurtPauseTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x000161E0 File Offset: 0x000143E0
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000161F8 File Offset: 0x000143F8
	private void Quack()
	{
		this.quackSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.controller.currentState == EnemyDuck.State.Idle || this.controller.currentState == EnemyDuck.State.Roam || this.controller.currentState == EnemyDuck.State.Investigate || this.controller.currentState == EnemyDuck.State.Leave || this.controller.currentState == EnemyDuck.State.MoveBackToNavmesh)
			{
				return;
			}
			EnemyDirector.instance.SetInvestigate(base.transform.position, 10f);
		}
	}

	// Token: 0x0600021E RID: 542 RVA: 0x000162A4 File Offset: 0x000144A4
	private void BiteSound()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.Rigidbody.GrabRelease();
		}
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.biteSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00016350 File Offset: 0x00014550
	private void TransformSound()
	{
		if (!base.enabled)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, Camera.main.transform.position) < 10f)
		{
			AudioScare.instance.PlayImpact();
		}
		this.transformSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x000163C1 File Offset: 0x000145C1
	private void JumpSound()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000163EE File Offset: 0x000145EE
	private void FootstepSound()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0001641B File Offset: 0x0001461B
	private void MouthExtendSound()
	{
		this.mouthExtendSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00016448 File Offset: 0x00014648
	private void MouthRetractSound()
	{
		this.mouthRetractSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00016475 File Offset: 0x00014675
	private void StopAttackSound()
	{
		this.attackLoopSound.PlayLoop(false, 5f, 5f, 1f);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00016492 File Offset: 0x00014692
	private void NoticeSound()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x000164BF File Offset: 0x000146BF
	private void FlyFlapSound()
	{
		this.flyFlapSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x000164EC File Offset: 0x000146EC
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x040003F0 RID: 1008
	public Enemy enemy;

	// Token: 0x040003F1 RID: 1009
	internal Animator animator;

	// Token: 0x040003F2 RID: 1010
	public EnemyDuck controller;

	// Token: 0x040003F3 RID: 1011
	public Sound quackSound;

	// Token: 0x040003F4 RID: 1012
	public Sound stunSound;

	// Token: 0x040003F5 RID: 1013
	public Sound stunStopSound;

	// Token: 0x040003F6 RID: 1014
	public Sound biteSound;

	// Token: 0x040003F7 RID: 1015
	public Sound transformSound;

	// Token: 0x040003F8 RID: 1016
	public Sound jumpSound;

	// Token: 0x040003F9 RID: 1017
	public Sound footstepSound;

	// Token: 0x040003FA RID: 1018
	public Sound mouthExtendSound;

	// Token: 0x040003FB RID: 1019
	public Sound mouthRetractSound;

	// Token: 0x040003FC RID: 1020
	public Sound attackLoopSound;

	// Token: 0x040003FD RID: 1021
	public Sound hurtSound;

	// Token: 0x040003FE RID: 1022
	public Sound deathSound;

	// Token: 0x040003FF RID: 1023
	public Sound noticeSound;

	// Token: 0x04000400 RID: 1024
	public Sound flyFlapSound;

	// Token: 0x04000401 RID: 1025
	public Sound flyLoopSound;

	// Token: 0x04000402 RID: 1026
	public float soundHurtPauseTimer;

	// Token: 0x04000403 RID: 1027
	private bool jumpImpulse;

	// Token: 0x04000404 RID: 1028
	private bool flyImpulse;

	// Token: 0x04000405 RID: 1029
	private bool landImpulse;

	// Token: 0x04000406 RID: 1030
	private bool stunImpulse;

	// Token: 0x04000407 RID: 1031
	private bool noticeImpulse;

	// Token: 0x04000408 RID: 1032
	private bool transformImpulse;

	// Token: 0x04000409 RID: 1033
	private float idleBreakerTimer;
}
