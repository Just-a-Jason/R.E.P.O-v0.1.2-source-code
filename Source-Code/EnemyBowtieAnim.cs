using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class EnemyBowtieAnim : MonoBehaviour
{
	// Token: 0x060001AC RID: 428 RVA: 0x000117F3 File Offset: 0x0000F9F3
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060001AD RID: 429 RVA: 0x00011810 File Offset: 0x0000FA10
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
		base.transform.position = this.followTarget.position;
		base.transform.rotation = this.followTarget.rotation;
		if (this.enemy.Rigidbody.velocity.magnitude > 0.1f)
		{
			this.animator.SetBool("move", true);
		}
		else
		{
			this.animator.SetBool("move", false);
		}
		if (this.controller.currentState == EnemyBowtie.State.Leave)
		{
			this.animator.SetBool("leaving", true);
		}
		else
		{
			this.animator.SetBool("leaving", false);
		}
		if (this.controller.currentState == EnemyBowtie.State.Idle || this.controller.currentState == EnemyBowtie.State.Roam || this.controller.currentState == EnemyBowtie.State.Investigate)
		{
			if (this.soundGroanPauseTimer > 0f)
			{
				this.StopGroaning();
			}
			else
			{
				this.GroanLoopSound.PlayLoop(true, 5f, 5f, 1f);
			}
		}
		else
		{
			this.StopGroaning();
		}
		if (this.soundGroanPauseTimer > 0f)
		{
			this.soundGroanPauseTimer -= Time.deltaTime;
		}
		if (this.controller.currentState == EnemyBowtie.State.Yell)
		{
			this.animator.SetBool("yell", true);
			this.yell = true;
		}
		else
		{
			this.animator.SetBool("yell", false);
			this.yell = false;
			this.particleYell.Stop();
			this.particleYellSmall.Stop();
		}
		this.YellLoopSound.PlayLoop(this.yell, 5f, 5f, 1f);
		this.YellLoopSoundGlobal.PlayLoop(this.yell, 5f, 5f, 1f);
		if (this.controller.currentState == EnemyBowtie.State.Despawn)
		{
			this.animator.SetBool("despawn", true);
			this.particleYell.Stop();
			this.particleYellSmall.Stop();
		}
		else
		{
			this.animator.SetBool("despawn", false);
		}
		bool flag = false;
		if (this.controller.currentState == EnemyBowtie.State.Stun)
		{
			flag = true;
			this.stunSound.PlayLoop(true, 2f, 5f, 1f);
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.StopGroaning();
				this.stunSound.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
				this.animator.SetTrigger("Stun Impulse");
				this.stunImpulse = false;
			}
			this.animator.SetBool("stunned", true);
			if (this.soundStunPauseTimer > 0f)
			{
				this.StopStunSound();
			}
			else
			{
				this.stunSound.PlayLoop(true, 2f, 5f, 1f);
			}
		}
		else
		{
			this.StopStunSound();
			this.animator.SetBool("stunned", false);
			this.stunImpulse = true;
		}
		if (this.soundStunPauseTimer > 0f)
		{
			this.soundStunPauseTimer -= Time.deltaTime;
		}
		if (!flag && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump Impulse");
				this.animator.SetBool("falling", false);
				this.jumpImpulse = false;
				this.landImpulse = true;
				return;
			}
			if (this.controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool("falling", true);
				return;
			}
		}
		else
		{
			if (this.landImpulse)
			{
				this.animator.SetTrigger("Land Impulse");
				this.landImpulse = false;
			}
			this.animator.SetBool("falling", false);
			this.jumpImpulse = true;
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00011C1A File Offset: 0x0000FE1A
	public void OnSpawn()
	{
		this.animator.SetBool("stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00011C43 File Offset: 0x0000FE43
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00011C55 File Offset: 0x0000FE55
	public void DespawnStart()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00011C84 File Offset: 0x0000FE84
	public void Despawn()
	{
		this.particleDespawnSpark.Play();
		this.despawnSparkSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x00011CD8 File Offset: 0x0000FED8
	public void Footstep()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00011D9C File Offset: 0x0000FF9C
	public void FootstepSmall()
	{
		this.footstepSmallSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(1.5f, 3f, 10f, base.transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(1.5f, 3f, 10f, base.transform.position, 0.1f);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00011E5D File Offset: 0x0001005D
	public void StompLeft()
	{
		this.particleStompL.Play();
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00011E6A File Offset: 0x0001006A
	public void StompRight()
	{
		this.particleStompR.Play();
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x00011E77 File Offset: 0x00010077
	public void MoveShort()
	{
		this.moveShortSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00011EA4 File Offset: 0x000100A4
	public void MoveLong()
	{
		this.moveLongSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00011ED1 File Offset: 0x000100D1
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x00011EFE File Offset: 0x000100FE
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00011F2C File Offset: 0x0001012C
	public void Notice()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00011FC0 File Offset: 0x000101C0
	public void YellStart()
	{
		this.yellStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.yellStartSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleYell.Play();
		this.particleYellSmall.Play();
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0001203C File Offset: 0x0001023C
	public void yellShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 12f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 12f, base.transform.position, 0.1f);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x000120A5 File Offset: 0x000102A5
	public void EnemyInvestigate()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 15f);
		}
	}

	// Token: 0x060001BE RID: 446 RVA: 0x000120D0 File Offset: 0x000102D0
	public void YellStop()
	{
		this.yellEndSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.yellEndSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00012133 File Offset: 0x00010333
	public void StopStunSound()
	{
		this.stunSound.PlayLoop(false, 2f, 5f, 1f);
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00012150 File Offset: 0x00010350
	public void StopGroaning()
	{
		this.GroanLoopSound.PlayLoop(false, 5f, 5f, 1f);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0001216D File Offset: 0x0001036D
	public void StunPause()
	{
		this.soundStunPauseTimer = 0.5f;
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0001217A File Offset: 0x0001037A
	public void GroanPause()
	{
		this.soundGroanPauseTimer = 1f;
	}

	// Token: 0x04000381 RID: 897
	public Transform followTarget;

	// Token: 0x04000382 RID: 898
	public EnemyBowtie controller;

	// Token: 0x04000383 RID: 899
	public Enemy enemy;

	// Token: 0x04000384 RID: 900
	public Materials.MaterialTrigger material;

	// Token: 0x04000385 RID: 901
	internal Animator animator;

	// Token: 0x04000386 RID: 902
	private bool attackImpulse;

	// Token: 0x04000387 RID: 903
	private bool stunImpulse;

	// Token: 0x04000388 RID: 904
	private bool jumpImpulse;

	// Token: 0x04000389 RID: 905
	private bool landImpulse;

	// Token: 0x0400038A RID: 906
	private bool noticeImpulse;

	// Token: 0x0400038B RID: 907
	[Space]
	public ParticleSystem particleBits;

	// Token: 0x0400038C RID: 908
	public ParticleSystem particleImpact;

	// Token: 0x0400038D RID: 909
	public ParticleSystem particleDirectionalBits;

	// Token: 0x0400038E RID: 910
	public ParticleSystem particleEyes;

	// Token: 0x0400038F RID: 911
	public ParticleSystem particleDespawnSpark;

	// Token: 0x04000390 RID: 912
	public ParticleSystem particleYell;

	// Token: 0x04000391 RID: 913
	public ParticleSystem particleYellSmall;

	// Token: 0x04000392 RID: 914
	public ParticleSystem particleStompL;

	// Token: 0x04000393 RID: 915
	public ParticleSystem particleStompR;

	// Token: 0x04000394 RID: 916
	private float soundStunPauseTimer;

	// Token: 0x04000395 RID: 917
	private float soundGroanPauseTimer;

	// Token: 0x04000396 RID: 918
	[Space]
	public Sound footstepSound;

	// Token: 0x04000397 RID: 919
	public Sound footstepSmallSound;

	// Token: 0x04000398 RID: 920
	[Space]
	public Sound moveShortSound;

	// Token: 0x04000399 RID: 921
	public Sound moveLongSound;

	// Token: 0x0400039A RID: 922
	public Sound GroanLoopSound;

	// Token: 0x0400039B RID: 923
	[Space]
	public Sound despawnSound;

	// Token: 0x0400039C RID: 924
	public Sound despawnSparkSound;

	// Token: 0x0400039D RID: 925
	[Space]
	public Sound jumpSound;

	// Token: 0x0400039E RID: 926
	public Sound landSound;

	// Token: 0x0400039F RID: 927
	public Sound noticeSound;

	// Token: 0x040003A0 RID: 928
	[Space]
	public Sound yellStartSound;

	// Token: 0x040003A1 RID: 929
	public Sound yellStartSoundGlobal;

	// Token: 0x040003A2 RID: 930
	public Sound yellEndSound;

	// Token: 0x040003A3 RID: 931
	public Sound yellEndSoundGlobal;

	// Token: 0x040003A4 RID: 932
	public Sound YellLoopSound;

	// Token: 0x040003A5 RID: 933
	public Sound YellLoopSoundGlobal;

	// Token: 0x040003A6 RID: 934
	private bool yell;

	// Token: 0x040003A7 RID: 935
	[Space]
	public Sound stunSound;

	// Token: 0x040003A8 RID: 936
	[Space]
	public Sound hurtSound;

	// Token: 0x040003A9 RID: 937
	public Sound deathSound;
}
