using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class EnemyGnomeAnim : MonoBehaviour
{
	// Token: 0x060002A2 RID: 674 RVA: 0x0001B05C File Offset: 0x0001925C
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0001B078 File Offset: 0x00019278
	private void Update()
	{
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
			if (this.enemyGnome.currentState == EnemyGnome.State.Stun)
			{
				this.animator.speed = Mathf.Clamp(this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude, 1f, 3f);
			}
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Attack)
		{
			if (this.attackImpulse)
			{
				this.animator.SetTrigger("Attack");
				this.attackImpulse = false;
			}
		}
		else
		{
			this.attackImpulse = true;
		}
		bool flag = false;
		if (this.enemyGnome.currentState == EnemyGnome.State.Stun)
		{
			flag = true;
			this.landImpulse = false;
			if (this.stunImpulse)
			{
				this.soundStun.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
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
		if (!flag && this.enemy.Jump.jumping)
		{
			if (this.jumpImpulse)
			{
				this.animator.SetTrigger("Jump");
				this.animator.SetBool("Falling", false);
				this.jumpImpulse = false;
				this.landImpulse = true;
			}
			else if (this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				this.animator.SetBool("Falling", true);
			}
		}
		else
		{
			if (this.landImpulse)
			{
				this.animator.SetTrigger("Land");
				this.landImpulse = false;
			}
			this.animator.SetBool("Falling", false);
			this.jumpImpulse = true;
		}
		if (this.idleBreakerImpulse)
		{
			this.animator.SetTrigger("IdleBreaker");
			this.idleBreakerImpulse = false;
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Notice)
		{
			if (this.noticeImpulse)
			{
				this.animator.SetTrigger("Notice");
				this.noticeImpulse = false;
			}
		}
		else
		{
			this.noticeImpulse = true;
		}
		if (this.enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.magnitude > 0.2f || this.enemyGnome.enemy.Rigidbody.physGrabObject.rbAngularVelocity.magnitude > 0.5f)
		{
			this.animator.SetBool("Moving", true);
		}
		else
		{
			this.animator.SetBool("Moving", false);
		}
		if (this.enemyGnome.currentState == EnemyGnome.State.Despawn)
		{
			this.animator.SetBool("Despawning", true);
			return;
		}
		this.animator.SetBool("Despawning", false);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0001B384 File Offset: 0x00019584
	public void OnSpawn()
	{
		this.animator.SetBool("Despawning", false);
		this.animator.SetBool("Stunned", false);
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0001B3BE File Offset: 0x000195BE
	public void AttackDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemyGnome.UpdateState(EnemyGnome.State.AttackDone);
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001B3D4 File Offset: 0x000195D4
	public void Footstep()
	{
		this.soundFootstep.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		Materials.Instance.Impulse(this.enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, true, this.material, Materials.HostType.Enemy);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0001B43E File Offset: 0x0001963E
	public void DespawnSet()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0001B450 File Offset: 0x00019650
	public void MoveShort()
	{
		this.soundMoveShort.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001B482 File Offset: 0x00019682
	public void MoveLong()
	{
		this.soundMoveLong.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001B4B4 File Offset: 0x000196B4
	public void PickaxeTell()
	{
		this.soundPickaxeTell.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0001B4E8 File Offset: 0x000196E8
	public void PickaxeHit()
	{
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, this.enemy.CenterTransform.position, 0.05f);
		this.soundPickaxeHit.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0001B58B File Offset: 0x0001978B
	public void IdleBreaker()
	{
		this.soundIdleBreaker.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001B5BD File Offset: 0x000197BD
	public void Notice()
	{
		this.soundNotice.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001B5EF File Offset: 0x000197EF
	public void Jump()
	{
		this.soundJump.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0001B621 File Offset: 0x00019821
	public void Land()
	{
		this.soundLand.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0001B653 File Offset: 0x00019853
	public void Spawn()
	{
		this.soundSpawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001B685 File Offset: 0x00019885
	public void Despawn()
	{
		this.soundDespawn.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001B6B7 File Offset: 0x000198B7
	public void StunOutro()
	{
		this.soundStunOutro.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040004A9 RID: 1193
	public Enemy enemy;

	// Token: 0x040004AA RID: 1194
	public EnemyGnome enemyGnome;

	// Token: 0x040004AB RID: 1195
	internal Animator animator;

	// Token: 0x040004AC RID: 1196
	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	// Token: 0x040004AD RID: 1197
	private bool attackImpulse;

	// Token: 0x040004AE RID: 1198
	private bool stunImpulse;

	// Token: 0x040004AF RID: 1199
	private bool jumpImpulse;

	// Token: 0x040004B0 RID: 1200
	private bool landImpulse;

	// Token: 0x040004B1 RID: 1201
	internal bool idleBreakerImpulse;

	// Token: 0x040004B2 RID: 1202
	private bool noticeImpulse;

	// Token: 0x040004B3 RID: 1203
	[Space]
	public Sound soundFootstep;

	// Token: 0x040004B4 RID: 1204
	[Space]
	public Sound soundMoveShort;

	// Token: 0x040004B5 RID: 1205
	public Sound soundMoveLong;

	// Token: 0x040004B6 RID: 1206
	[Space]
	public Sound soundPickaxeTell;

	// Token: 0x040004B7 RID: 1207
	public Sound soundPickaxeHit;

	// Token: 0x040004B8 RID: 1208
	[Space]
	public Sound soundIdleBreaker;

	// Token: 0x040004B9 RID: 1209
	public Sound soundNotice;

	// Token: 0x040004BA RID: 1210
	[Space]
	public Sound soundSpawn;

	// Token: 0x040004BB RID: 1211
	public Sound soundDespawn;

	// Token: 0x040004BC RID: 1212
	[Space]
	public Sound soundJump;

	// Token: 0x040004BD RID: 1213
	public Sound soundLand;

	// Token: 0x040004BE RID: 1214
	[Space]
	public Sound soundStun;

	// Token: 0x040004BF RID: 1215
	public Sound soundStunOutro;
}
