using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class EnemyFloaterAnim : MonoBehaviour
{
	// Token: 0x06000246 RID: 582 RVA: 0x000176AC File Offset: 0x000158AC
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.springHeadSpeed = this.springHead.speed;
		this.springHeadDamping = this.springHead.damping;
		this.springLegLSpeed = this.springLegL.speed;
		this.springLegLDamping = this.springLegL.damping;
		this.springLegRSpeed = this.springLegR.speed;
		this.springLegRDamping = this.springLegR.damping;
		this.springArmLSpeed = this.springArmL.speed;
		this.springArmLDamping = this.springArmL.damping;
		this.springArmRSpeed = this.springArmR.speed;
		this.springArmRDamping = this.springArmR.damping;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0001777C File Offset: 0x0001597C
	private void Update()
	{
		this.SpringLogic();
		if (this.enemy.Rigidbody.frozen)
		{
			this.animator.speed = 0f;
		}
		else
		{
			this.animator.speed = 1f;
		}
		if (this.controller.currentState == EnemyFloater.State.Stun)
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
		if (this.controller.currentState == EnemyFloater.State.Notice)
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
		if (this.controller.currentState == EnemyFloater.State.ChargeAttack)
		{
			if (this.chargeAttackImpulse)
			{
				this.animator.SetTrigger(this.animChargeAttack);
				this.chargeAttackImpulse = false;
				this.sfxChargeAttackStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		else
		{
			this.chargeAttackImpulse = true;
		}
		this.SfxChargeAttackLoop();
		if (this.controller.currentState == EnemyFloater.State.DelayAttack)
		{
			if (this.delayAttackImpulse)
			{
				this.animator.SetTrigger(this.animDelayAttack);
				this.delayAttackImpulse = false;
			}
		}
		else
		{
			this.delayAttackImpulse = true;
		}
		this.SfxDelayAttackLoop();
		if (this.controller.currentState == EnemyFloater.State.Attack)
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
		if (this.controller.currentState == EnemyFloater.State.Despawn)
		{
			this.animator.SetBool(this.animDespawning, true);
			return;
		}
		this.animator.SetBool(this.animDespawning, false);
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0001797B File Offset: 0x00015B7B
	public void OnSpawn()
	{
		this.animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00017993 File Offset: 0x00015B93
	public void NoticeSet(int _playerID)
	{
		this.animator.SetTrigger(this.animNotice);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000179A8 File Offset: 0x00015BA8
	private void SpringLogic()
	{
		this.springHead.speed = this.springHeadSpeed * this.springSpeedMultiplier;
		this.springHead.damping = this.springHeadDamping * this.springDampingMultiplier;
		this.springHeadSource.rotation = SemiFunc.SpringQuaternionGet(this.springHead, this.springHeadTarget.transform.rotation, -1f);
		this.springLegL.speed = this.springLegLSpeed * this.springSpeedMultiplier;
		this.springLegL.damping = this.springLegLDamping * this.springDampingMultiplier;
		this.springLegLSource.rotation = SemiFunc.SpringQuaternionGet(this.springLegL, this.springLegLTarget.transform.rotation, -1f);
		this.springLegR.speed = this.springLegRSpeed * this.springSpeedMultiplier;
		this.springLegR.damping = this.springLegRDamping * this.springDampingMultiplier;
		this.springLegRSource.rotation = SemiFunc.SpringQuaternionGet(this.springLegR, this.springLegRTarget.transform.rotation, -1f);
		this.springArmL.speed = this.springArmLSpeed * this.springSpeedMultiplier;
		this.springArmL.damping = this.springArmLDamping * this.springDampingMultiplier;
		this.springArmLSource.rotation = SemiFunc.SpringQuaternionGet(this.springArmL, this.springArmLTarget.transform.rotation, -1f);
		this.springArmR.speed = this.springArmRSpeed * this.springSpeedMultiplier;
		this.springArmR.damping = this.springArmRDamping * this.springDampingMultiplier;
		this.springArmRSource.rotation = SemiFunc.SpringQuaternionGet(this.springArmR, this.springArmRTarget.transform.rotation, -1f);
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00017B7C File Offset: 0x00015D7C
	public void Despawn()
	{
		this.enemy.EnemyParent.Despawn();
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00017B8E File Offset: 0x00015D8E
	public void DelayAttack()
	{
		this.attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.stop);
		this.SfxDelayAttack();
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00017BA2 File Offset: 0x00015DA2
	public void Attack()
	{
		this.attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.smash);
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00017BB0 File Offset: 0x00015DB0
	public void SfxDelayAttack()
	{
		this.sfxDelayAttackLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxDelayAttackGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00017C14 File Offset: 0x00015E14
	public void SfxAttackUp()
	{
		this.sfxAttackUpLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackUpGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00017C78 File Offset: 0x00015E78
	public void SfxAttackDown()
	{
		this.sfxAttackDownLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.sfxAttackDownGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00017CDB File Offset: 0x00015EDB
	public void SfxMoveShort()
	{
		this.sfxMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00017D08 File Offset: 0x00015F08
	public void SfxMoveLong()
	{
		this.sfxMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00017D35 File Offset: 0x00015F35
	public void SfxHurt()
	{
		this.sfxHurt.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00017D62 File Offset: 0x00015F62
	public void SfxDeath()
	{
		this.sfxDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00017D8F File Offset: 0x00015F8F
	public void SfxChargeAttackLoop()
	{
		this.sfxChargeAttackLoop.PlayLoop(this.sfxChargeAttackLoopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00017DB1 File Offset: 0x00015FB1
	public void SfxDelayAttackLoop()
	{
		this.sfxDelayAttackLoop.PlayLoop(this.sfxDelayAttackLoopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00017DD3 File Offset: 0x00015FD3
	public void SfxStunnedLoop()
	{
		this.sfxStunnedLoop.PlayLoop(this.stunned, 5f, 5f, 1f);
	}

	// Token: 0x04000423 RID: 1059
	private int animStunned = Animator.StringToHash("stunned");

	// Token: 0x04000424 RID: 1060
	private int animDespawning = Animator.StringToHash("despawning");

	// Token: 0x04000425 RID: 1061
	private int animAttacking = Animator.StringToHash("attacking");

	// Token: 0x04000426 RID: 1062
	private int animStun = Animator.StringToHash("Stun");

	// Token: 0x04000427 RID: 1063
	private int animNotice = Animator.StringToHash("Notice");

	// Token: 0x04000428 RID: 1064
	private int animChargeAttack = Animator.StringToHash("ChargeAttack");

	// Token: 0x04000429 RID: 1065
	private int animDelayAttack = Animator.StringToHash("DelayAttack");

	// Token: 0x0400042A RID: 1066
	private int animAttack = Animator.StringToHash("Attack");

	// Token: 0x0400042B RID: 1067
	public Enemy enemy;

	// Token: 0x0400042C RID: 1068
	public EnemyFloater controller;

	// Token: 0x0400042D RID: 1069
	public FloaterAttackLogic attackLogic;

	// Token: 0x0400042E RID: 1070
	public float springSpeedMultiplier = 1f;

	// Token: 0x0400042F RID: 1071
	public float springDampingMultiplier = 1f;

	// Token: 0x04000430 RID: 1072
	public SpringQuaternion springHead;

	// Token: 0x04000431 RID: 1073
	private float springHeadSpeed;

	// Token: 0x04000432 RID: 1074
	private float springHeadDamping;

	// Token: 0x04000433 RID: 1075
	public Transform springHeadTarget;

	// Token: 0x04000434 RID: 1076
	public Transform springHeadSource;

	// Token: 0x04000435 RID: 1077
	public SpringQuaternion springLegL;

	// Token: 0x04000436 RID: 1078
	private float springLegLSpeed;

	// Token: 0x04000437 RID: 1079
	private float springLegLDamping;

	// Token: 0x04000438 RID: 1080
	public Transform springLegLTarget;

	// Token: 0x04000439 RID: 1081
	public Transform springLegLSource;

	// Token: 0x0400043A RID: 1082
	public SpringQuaternion springLegR;

	// Token: 0x0400043B RID: 1083
	private float springLegRSpeed;

	// Token: 0x0400043C RID: 1084
	private float springLegRDamping;

	// Token: 0x0400043D RID: 1085
	public Transform springLegRTarget;

	// Token: 0x0400043E RID: 1086
	public Transform springLegRSource;

	// Token: 0x0400043F RID: 1087
	public SpringQuaternion springArmL;

	// Token: 0x04000440 RID: 1088
	private float springArmLSpeed;

	// Token: 0x04000441 RID: 1089
	private float springArmLDamping;

	// Token: 0x04000442 RID: 1090
	public Transform springArmLTarget;

	// Token: 0x04000443 RID: 1091
	public Transform springArmLSource;

	// Token: 0x04000444 RID: 1092
	public SpringQuaternion springArmR;

	// Token: 0x04000445 RID: 1093
	private float springArmRSpeed;

	// Token: 0x04000446 RID: 1094
	private float springArmRDamping;

	// Token: 0x04000447 RID: 1095
	public Transform springArmRTarget;

	// Token: 0x04000448 RID: 1096
	public Transform springArmRSource;

	// Token: 0x04000449 RID: 1097
	[Header("One Shots")]
	public Sound sfxChargeAttackStart;

	// Token: 0x0400044A RID: 1098
	public Sound sfxDelayAttackLocal;

	// Token: 0x0400044B RID: 1099
	public Sound sfxDelayAttackGlobal;

	// Token: 0x0400044C RID: 1100
	public Sound sfxAttackUpLocal;

	// Token: 0x0400044D RID: 1101
	public Sound sfxAttackUpGlobal;

	// Token: 0x0400044E RID: 1102
	public Sound sfxAttackDownLocal;

	// Token: 0x0400044F RID: 1103
	public Sound sfxAttackDownGlobal;

	// Token: 0x04000450 RID: 1104
	public Sound sfxMoveShort;

	// Token: 0x04000451 RID: 1105
	public Sound sfxMoveLong;

	// Token: 0x04000452 RID: 1106
	public Sound sfxHurt;

	// Token: 0x04000453 RID: 1107
	public Sound sfxDeath;

	// Token: 0x04000454 RID: 1108
	[Header("Loops")]
	public Sound sfxChargeAttackLoop;

	// Token: 0x04000455 RID: 1109
	public Sound sfxDelayAttackLoop;

	// Token: 0x04000456 RID: 1110
	public Sound sfxStunnedLoop;

	// Token: 0x04000457 RID: 1111
	[Header("Animation Booleans")]
	public bool sfxChargeAttackLoopPlaying;

	// Token: 0x04000458 RID: 1112
	public bool sfxDelayAttackLoopPlaying;

	// Token: 0x04000459 RID: 1113
	internal Animator animator;

	// Token: 0x0400045A RID: 1114
	private bool idling;

	// Token: 0x0400045B RID: 1115
	private bool stunned;

	// Token: 0x0400045C RID: 1116
	private bool stunImpulse;

	// Token: 0x0400045D RID: 1117
	private bool noticeImpulse;

	// Token: 0x0400045E RID: 1118
	private bool delayAttackImpulse;

	// Token: 0x0400045F RID: 1119
	private bool attackImpulse;

	// Token: 0x04000460 RID: 1120
	private bool chargeAttackImpulse;
}
