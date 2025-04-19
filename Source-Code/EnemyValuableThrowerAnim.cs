using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class EnemyValuableThrowerAnim : MonoBehaviour
{
	// Token: 0x06000561 RID: 1377 RVA: 0x00035894 File Offset: 0x00033A94
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x000358B0 File Offset: 0x00033AB0
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
		if (this.enemy.Rigidbody.velocity.magnitude > 0.5f)
		{
			this.animator.SetBool("Move", true);
			this.animator.SetBool("Move Slow", false);
		}
		else if (this.enemy.Rigidbody.velocity.magnitude > 0.2f)
		{
			this.animator.SetBool("Move", false);
			this.animator.SetBool("Move Slow", true);
		}
		else
		{
			this.animator.SetBool("Move", false);
			this.animator.SetBool("Move Slow", false);
		}
		if (this.enemy.CurrentState == EnemyState.Despawn)
		{
			this.stun = false;
			this.animator.SetBool("Despawn", true);
		}
		else
		{
			this.animator.SetBool("Despawn", false);
		}
		if (this.controller.currentState == EnemyValuableThrower.State.PickUpTarget || this.controller.currentState == EnemyValuableThrower.State.TargetPlayer)
		{
			this.animator.SetBool("Pickup", true);
		}
		else
		{
			this.animator.SetBool("Pickup", false);
		}
		if (this.enemy.Jump.jumping)
		{
			this.animator.SetBool("Jumping", true);
		}
		else
		{
			this.animator.SetBool("Jumping", false);
		}
		if (this.enemy.IsStunned())
		{
			this.stun = true;
			this.animator.SetBool("Stun", true);
		}
		else
		{
			this.stun = false;
			this.animator.SetBool("Stun", false);
		}
		if (this.stun && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
		{
			this.animator.SetTrigger("Stun Impulse");
		}
		this.stunSound.PlayLoop(this.stun, 10f, 10f, 1f);
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00035B01 File Offset: 0x00033D01
	public void OnSpawn()
	{
		this.stun = false;
		this.animator.SetBool("Stun", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00035B31 File Offset: 0x00033D31
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger("Notice");
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00035B43 File Offset: 0x00033D43
	public void ResetStateTimer()
	{
		this.controller.ResetStateTimer();
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00035B50 File Offset: 0x00033D50
	public void SpawnStart()
	{
		this.spawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x00035B7D File Offset: 0x00033D7D
	public void DespawnStart()
	{
		this.despawnSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x00035BAA File Offset: 0x00033DAA
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x00035BBC File Offset: 0x00033DBC
	public void Throw()
	{
		this.pickupOutroThrowSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.controller.Throw();
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x00035BF4 File Offset: 0x00033DF4
	public void Footstep()
	{
		this.footstepSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x00035C5C File Offset: 0x00033E5C
	public void FootstepSmall()
	{
		this.footstepSmallSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00035CC1 File Offset: 0x00033EC1
	public void MoveShort()
	{
		this.moveShortSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00035CEE File Offset: 0x00033EEE
	public void MoveLong()
	{
		this.moveLongSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00035D1B File Offset: 0x00033F1B
	public void Jump()
	{
		this.jumpSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x00035D48 File Offset: 0x00033F48
	public void Land()
	{
		this.landSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x00035D75 File Offset: 0x00033F75
	public void PickupIntro()
	{
		this.pickupIntroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00035DA2 File Offset: 0x00033FA2
	public void PickupOutro()
	{
		this.pickupOutroTellSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00035DCF File Offset: 0x00033FCF
	public void StunStop()
	{
		this.stunStopSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00035DFC File Offset: 0x00033FFC
	public void Notice()
	{
		this.noticeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x04000866 RID: 2150
	public Transform followTarget;

	// Token: 0x04000867 RID: 2151
	public EnemyValuableThrower controller;

	// Token: 0x04000868 RID: 2152
	public Enemy enemy;

	// Token: 0x04000869 RID: 2153
	public Materials.MaterialTrigger material;

	// Token: 0x0400086A RID: 2154
	internal Animator animator;

	// Token: 0x0400086B RID: 2155
	[Space]
	public ParticleSystem particleBits;

	// Token: 0x0400086C RID: 2156
	public ParticleSystem particleImpact;

	// Token: 0x0400086D RID: 2157
	public ParticleSystem particleDirectionalBits;

	// Token: 0x0400086E RID: 2158
	private bool stun;

	// Token: 0x0400086F RID: 2159
	[Space]
	public Sound footstepSound;

	// Token: 0x04000870 RID: 2160
	public Sound footstepSmallSound;

	// Token: 0x04000871 RID: 2161
	[Space]
	public Sound moveShortSound;

	// Token: 0x04000872 RID: 2162
	public Sound moveLongSound;

	// Token: 0x04000873 RID: 2163
	[Space]
	public Sound spawnSound;

	// Token: 0x04000874 RID: 2164
	public Sound despawnSound;

	// Token: 0x04000875 RID: 2165
	[Space]
	public Sound jumpSound;

	// Token: 0x04000876 RID: 2166
	public Sound landSound;

	// Token: 0x04000877 RID: 2167
	public Sound noticeSound;

	// Token: 0x04000878 RID: 2168
	[Space]
	public Sound pickupIntroSound;

	// Token: 0x04000879 RID: 2169
	public Sound pickupOutroTellSound;

	// Token: 0x0400087A RID: 2170
	public Sound pickupOutroThrowSound;

	// Token: 0x0400087B RID: 2171
	[Space]
	public Sound stunSound;

	// Token: 0x0400087C RID: 2172
	public Sound stunStopSound;

	// Token: 0x0400087D RID: 2173
	[Space]
	public Sound hurtSound;

	// Token: 0x0400087E RID: 2174
	public Sound deathSound;
}
