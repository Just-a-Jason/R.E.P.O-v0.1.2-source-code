using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200004E RID: 78
public class EnemyGnome : MonoBehaviour
{
	// Token: 0x06000279 RID: 633 RVA: 0x00019747 File Offset: 0x00017947
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00019758 File Offset: 0x00017958
	private void Start()
	{
		this.enemy.NavMeshAgent.DefaultSpeed = Random.Range(this.speedMin, this.speedMax);
		this.enemy.NavMeshAgent.Agent.speed = this.enemy.NavMeshAgent.DefaultSpeed;
	}

	// Token: 0x0600027B RID: 635 RVA: 0x000197AC File Offset: 0x000179AC
	private void Update()
	{
		if (!EnemyGnomeDirector.instance || !EnemyGnomeDirector.instance.setup)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.AvoidLogic();
			this.RotationLogic();
			this.BackAwayOffsetLogic();
			this.MoveOffsetLogic();
			this.TimerLogic();
			if (this.enemy.CurrentState == EnemyState.Despawn)
			{
				this.UpdateState(EnemyGnome.State.Despawn);
			}
			if (this.enemy.IsStunned())
			{
				this.UpdateState(EnemyGnome.State.Stun);
			}
			switch (this.currentState)
			{
			case EnemyGnome.State.Spawn:
				this.StateSpawn();
				return;
			case EnemyGnome.State.Idle:
				this.StateIdle();
				return;
			case EnemyGnome.State.NoticeDelay:
				this.StateNoticeDelay();
				return;
			case EnemyGnome.State.Notice:
				this.StateNotice();
				return;
			case EnemyGnome.State.Move:
				this.StateMove();
				return;
			case EnemyGnome.State.MoveUnder:
				this.StateMoveUnder();
				return;
			case EnemyGnome.State.MoveOver:
				this.StateMoveOver();
				return;
			case EnemyGnome.State.MoveBack:
				this.StateMoveBack();
				return;
			case EnemyGnome.State.AttackMove:
				this.StateAttackMove();
				return;
			case EnemyGnome.State.Attack:
				this.StateAttack();
				return;
			case EnemyGnome.State.AttackDone:
				this.StateAttackDone();
				return;
			case EnemyGnome.State.Stun:
				this.StateStun();
				return;
			case EnemyGnome.State.Despawn:
				this.StateDespawn();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000198C4 File Offset: 0x00017AC4
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.avoidForce != Vector3.zero)
		{
			this.enemy.Rigidbody.rb.AddForce(this.avoidForce * 2f, ForceMode.Force);
		}
	}

	// Token: 0x0600027D RID: 637 RVA: 0x00019910 File Offset: 0x00017B10
	private void StateSpawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 1f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.Idle);
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00019960 File Offset: 0x00017B60
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
		}
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 0.5f);
		this.IdleBreakerLogic();
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.NoticeDelay);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) > 2f)
		{
			this.UpdateState(EnemyGnome.State.Move);
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00019A30 File Offset: 0x00017C30
	private void StateMove()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.enemy.NavMeshAgent.SetDestination(EnemyGnomeDirector.instance.destinations[this.directorIndex]);
		this.MoveBackPosition();
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.NoticeDelay);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) <= 0.2f)
		{
			this.UpdateState(EnemyGnome.State.Idle);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.destinations[this.directorIndex]) <= 2f)
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.Idle);
			}
		}
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00019B50 File Offset: 0x00017D50
	private void StateNoticeDelay()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(0f, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.Notice);
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00019BA8 File Offset: 0x00017DA8
	private void StateNotice()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateImpulse = false;
			this.stateTimer = 0.5f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00019C2C File Offset: 0x00017E2C
	private void StateAttackMove()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		Vector3 destination = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.SetDestination(destination);
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		this.MoveBackPosition();
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.Move);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		if (!this.enemy.NavMeshAgent.CanReach(this.AttackVisionDynamic(), 1f) && Vector3.Distance(this.enemy.Rigidbody.transform.position, this.enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			if (this.AttackPositionLogic().y > this.enemy.Rigidbody.transform.position.y + 0.2f)
			{
				this.enemy.Jump.StuckTrigger(this.AttackVisionPosition() - this.enemy.Vision.VisionTransform.position);
			}
			NavMeshHit navMeshHit;
			if (!this.VisionBlocked() && !NavMesh.SamplePosition(this.AttackVisionDynamic(), out navMeshHit, 0.5f, -1))
			{
				if (Mathf.Abs(this.AttackVisionDynamic().y - this.enemy.Rigidbody.transform.position.y) < 0.2f)
				{
					this.UpdateState(EnemyGnome.State.MoveUnder);
					return;
				}
				if (this.AttackPositionLogic().y > this.enemy.Rigidbody.transform.position.y)
				{
					this.UpdateState(EnemyGnome.State.MoveOver);
				}
			}
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00019E04 File Offset: 0x00018004
	private void StateMoveUnder()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 vector = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (this.VisionBlocked())
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.MoveBack);
				return;
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00019F2C File Offset: 0x0001812C
	private void StateMoveOver()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 2f;
			this.stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 vector = this.AttackPositionLogic();
		this.enemy.NavMeshAgent.Disable(0.1f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		if (this.AttackVisionDynamic().y > this.enemy.Rigidbody.transform.position.y + 0.2f && !flag)
		{
			this.enemy.Jump.StuckTrigger(this.AttackVisionDynamic() - this.enemy.Rigidbody.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.AttackVisionDynamic(), 2f);
		}
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.MoveBack);
			return;
		}
		if (this.VisionBlocked())
		{
			this.stateTimer -= Time.deltaTime;
			if (this.stateTimer <= 0f)
			{
				this.UpdateState(EnemyGnome.State.MoveBack);
				return;
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			this.stateTimer = 2f;
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0001A0DC File Offset: 0x000182DC
	private void StateMoveBack()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 2f;
		}
		this.enemy.NavMeshAgent.Disable(0.1f);
		if (!this.enemy.Jump.jumping)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.moveBackPosition, this.enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		this.MoveOffsetSet();
		SemiFunc.EnemyCartJump(this.enemy);
		this.stateTimer -= Time.deltaTime;
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		if (this.stateTimer <= 0f && (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) > 2f || this.enemy.Rigidbody.notMovingTimer > 2f) && !this.enemy.Jump.jumping)
		{
			Vector3 normalized = (base.transform.position - this.moveBackPosition).normalized;
			this.enemy.Jump.StuckTrigger(base.transform.position - this.moveBackPosition);
			base.transform.position = this.enemy.Rigidbody.transform.position;
			base.transform.position += normalized * 2f;
		}
		if (flag)
		{
			SemiFunc.EnemyCartJumpReset(this.enemy);
			this.UpdateState(EnemyGnome.State.Attack);
			return;
		}
		if (Vector3.Distance(this.enemy.Rigidbody.transform.position, this.moveBackPosition) <= 0.2f)
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
			return;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0001A2F0 File Offset: 0x000184F0
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			this.enemy.NavMeshAgent.ResetPath();
			this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			this.stateTimer = 3f;
			this.stateImpulse = false;
		}
		if (this.stateTimer > 0.5f && !this.enemyGnomeAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			this.UpdateState(EnemyGnome.State.AttackMove);
			return;
		}
		this.enemy.StuckCount = 0;
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyGnome.State.AttackDone);
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0001A3D0 File Offset: 0x000185D0
	private void StateAttackDone()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.enemy.StuckCount = 0;
		this.enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.moveBackTimer = 2f;
			this.attackCooldown = 2f;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.enemy.Rigidbody.transform.position, out navMeshHit, 0.5f, -1))
			{
				this.UpdateState(EnemyGnome.State.AttackMove);
				return;
			}
			this.UpdateState(this.attackMoveState);
		}
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0001A48A File Offset: 0x0001868A
	private void StateStun()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
		if (!this.enemy.IsStunned())
		{
			this.UpdateState(EnemyGnome.State.Idle);
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0001A4AF File Offset: 0x000186AF
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0001A4C0 File Offset: 0x000186C0
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(this.enemy))
		{
			EnemyGnomeDirector.instance.OnSpawn();
			this.UpdateState(EnemyGnome.State.Spawn);
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0001A4E7 File Offset: 0x000186E7
	public void OnHurt()
	{
		this.soundHurt.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0001A51C File Offset: 0x0001871C
	public void OnDeath()
	{
		this.soundDeath.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, this.enemy.CenterTransform.position, 0.05f);
		ParticleSystem[] array = this.deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemy.EnemyParent.Despawn();
		}
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0001A5F3 File Offset: 0x000187F3
	public void OnInvestigate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.Investigate(this.enemy.StateInvestigate.onInvestigateTriggeredPosition);
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0001A616 File Offset: 0x00018816
	public void OnVision()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.SetTarget(this.enemy.Vision.onVisionTriggeredPlayer);
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0001A639 File Offset: 0x00018839
	public void OnImpactLight()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactLight.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0001A679 File Offset: 0x00018879
	public void OnImpactMedium()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactMedium.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0001A6B9 File Offset: 0x000188B9
	public void OnImpactHeavy()
	{
		if (!this.enemy.IsStunned())
		{
			return;
		}
		this.soundImpactHeavy.Play(this.enemy.CenterTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0001A6FC File Offset: 0x000188FC
	public void UpdateState(EnemyGnome.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		if (_state == EnemyGnome.State.Attack)
		{
			this.attackMoveState = this.currentState;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.All, new object[]
			{
				this.currentState
			});
			return;
		}
		this.UpdateStateRPC(this.currentState);
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0001A778 File Offset: 0x00018978
	private void RotationLogic()
	{
		if (this.currentState == EnemyGnome.State.Move || this.currentState == EnemyGnome.State.Notice || this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver || this.currentState == EnemyGnome.State.MoveBack || this.currentState == EnemyGnome.State.Attack)
		{
			if (this.currentState == EnemyGnome.State.Notice || ((this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver || this.currentState == EnemyGnome.State.Attack) && Vector3.Distance(this.enemy.Rigidbody.transform.position, EnemyGnomeDirector.instance.attackPosition) < 5f))
			{
				Quaternion rotation = this.rotationTransform.rotation;
				this.rotationTransform.rotation = Quaternion.LookRotation(this.AttackVisionPosition() - this.enemy.Rigidbody.transform.position);
				this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
				Quaternion rotation2 = this.rotationTransform.rotation;
				this.rotationTransform.rotation = rotation;
				this.rotationTarget = rotation2;
			}
			else if (this.enemy.Rigidbody.rb.velocity.magnitude > 0.1f)
			{
				Vector3 position = this.rotationTransform.position;
				Quaternion rotation3 = this.rotationTransform.rotation;
				this.rotationTransform.position = this.enemy.Rigidbody.transform.position;
				this.rotationTransform.rotation = Quaternion.LookRotation(this.enemy.Rigidbody.rb.velocity.normalized);
				this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
				Quaternion rotation4 = this.rotationTransform.rotation;
				this.rotationTransform.position = position;
				this.rotationTransform.rotation = rotation3;
				this.rotationTarget = rotation4;
			}
		}
		else if (this.currentState == EnemyGnome.State.Idle && Vector3.Distance(EnemyGnomeDirector.instance.transform.position, this.enemy.Rigidbody.transform.position) > 0.1f)
		{
			Quaternion rotation5 = this.rotationTransform.rotation;
			this.rotationTransform.rotation = Quaternion.LookRotation(EnemyGnomeDirector.instance.transform.position - this.enemy.Rigidbody.transform.position);
			this.rotationTransform.eulerAngles = new Vector3(0f, this.rotationTransform.eulerAngles.y, 0f);
			Quaternion rotation6 = this.rotationTransform.rotation;
			this.rotationTransform.rotation = rotation5;
			this.rotationTarget = rotation6;
		}
		this.rotationTransform.rotation = SemiFunc.SpringQuaternionGet(this.rotationSpring, this.rotationTarget, -1f);
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0001AA90 File Offset: 0x00018C90
	private void AvoidLogic()
	{
		if (this.currentState == EnemyGnome.State.Move || this.currentState == EnemyGnome.State.AttackMove || this.currentState == EnemyGnome.State.MoveUnder || this.currentState == EnemyGnome.State.MoveOver)
		{
			if (this.avoidTimer > 0f)
			{
				this.avoidTimer -= Time.deltaTime;
				return;
			}
			this.avoidForce = Vector3.zero;
			this.avoidTimer = 0.25f;
			if (!this.enemy.Jump.jumping)
			{
				Collider[] array = Physics.OverlapBox(this.avoidCollider.transform.position, this.avoidCollider.size / 2f, this.avoidCollider.transform.rotation, LayerMask.GetMask(new string[]
				{
					"PhysGrabObject"
				}));
				for (int i = 0; i < array.Length; i++)
				{
					EnemyRigidbody componentInParent = array[i].GetComponentInParent<EnemyRigidbody>();
					if (componentInParent)
					{
						EnemyGnome component = componentInParent.enemy.GetComponent<EnemyGnome>();
						if (component)
						{
							Vector3 normalized = (base.transform.position - component.transform.position).normalized;
							this.avoidForce += normalized.normalized;
						}
					}
				}
				return;
			}
		}
		else
		{
			this.avoidForce = Vector3.zero;
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0001ABE0 File Offset: 0x00018DE0
	private Vector3 AttackPositionLogic()
	{
		Vector3 result = EnemyGnomeDirector.instance.attackPosition + new Vector3(Mathf.Cos(this.attackAngle), 0f, Mathf.Sin(this.attackAngle)) * 0.7f;
		this.attackAngle += Time.deltaTime * 1f;
		return result;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001AC3E File Offset: 0x00018E3E
	private Vector3 AttackVisionPosition()
	{
		return EnemyGnomeDirector.instance.attackVisionPosition;
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0001AC4A File Offset: 0x00018E4A
	private Vector3 AttackVisionDynamic()
	{
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer)
		{
			return this.AttackPositionLogic();
		}
		return this.AttackVisionPosition();
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0001AC66 File Offset: 0x00018E66
	private void MoveBackPosition()
	{
		if (Vector3.Distance(base.transform.position, this.enemy.Rigidbody.transform.position) < 1f)
		{
			this.moveBackPosition = base.transform.position;
		}
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0001ACA8 File Offset: 0x00018EA8
	private void BackAwayOffsetLogic()
	{
		if (this.moveBackTimer > 0f)
		{
			this.moveBackTimer -= Time.deltaTime;
			this.backAwayOffset.localPosition = Vector3.Lerp(this.backAwayOffset.localPosition, new Vector3(0f, 0f, -1f), Time.deltaTime * 10f);
			return;
		}
		this.backAwayOffset.localPosition = Vector3.Lerp(this.backAwayOffset.localPosition, Vector3.zero, Time.deltaTime * 10f);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0001AD3C File Offset: 0x00018F3C
	private void MoveOffsetLogic()
	{
		if (this.moveOffsetTimer > 0f)
		{
			this.moveOffsetTimer -= Time.deltaTime;
			if (this.enemy.Jump.jumping)
			{
				this.moveOffsetTimer = 0f;
			}
			if (this.moveOffsetTimer <= 0f)
			{
				this.moveOffsetPosition = Vector3.zero;
			}
			else
			{
				this.moveOffsetSetTimer -= Time.deltaTime;
				if (this.moveOffsetSetTimer <= 0f)
				{
					Vector3 vector = Random.insideUnitSphere * 0.5f;
					vector.y = 0f;
					this.moveOffsetPosition = vector;
					this.moveOffsetSetTimer = Random.Range(0.2f, 1f);
				}
			}
		}
		this.moveOffsetTransform.localPosition = Vector3.Lerp(this.moveOffsetTransform.localPosition, this.moveOffsetPosition, Time.deltaTime * 20f);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0001AE25 File Offset: 0x00019025
	private void MoveOffsetSet()
	{
		this.moveOffsetTimer = 0.2f;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0001AE34 File Offset: 0x00019034
	private bool VisionBlocked()
	{
		if (this.visionTimer <= 0f)
		{
			this.visionTimer = 0.1f;
			Vector3 direction = this.AttackVisionPosition() - this.enemy.Vision.VisionTransform.position;
			this.visionPrevious = Physics.Raycast(this.enemy.Vision.VisionTransform.position, direction, direction.magnitude, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
		}
		return this.visionPrevious;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0001AEBC File Offset: 0x000190BC
	private void TimerLogic()
	{
		this.visionTimer -= Time.deltaTime;
		this.attackCooldown -= Time.deltaTime;
		this.overlapCheckTimer -= Time.deltaTime;
		if (this.overlapCheckCooldown > 0f)
		{
			this.overlapCheckCooldown -= Time.deltaTime;
			if (this.overlapCheckCooldown <= 0f)
			{
				this.overlapCheckPrevious = false;
			}
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0001AF34 File Offset: 0x00019134
	public void IdleBreakerLogic()
	{
		bool flag = false;
		foreach (EnemyGnome enemyGnome in EnemyGnomeDirector.instance.gnomes)
		{
			if (enemyGnome != this && Vector3.Distance(base.transform.position, enemyGnome.transform.position) < 2f)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (this.idleBreakerTimer <= 0f)
			{
				this.idleBreakerTimer = Random.Range(2f, 15f);
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("IdleBreakerRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.IdleBreakerRPC();
				return;
			}
			else
			{
				this.idleBreakerTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0001B014 File Offset: 0x00019214
	[PunRPC]
	private void UpdateStateRPC(EnemyGnome.State _state)
	{
		this.currentState = _state;
		if (this.currentState == EnemyGnome.State.Spawn)
		{
			this.enemyGnomeAnim.OnSpawn();
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001B030 File Offset: 0x00019230
	[PunRPC]
	private void IdleBreakerRPC()
	{
		this.enemyGnomeAnim.idleBreakerImpulse = true;
	}

	// Token: 0x04000484 RID: 1156
	[Space]
	public EnemyGnome.State currentState;

	// Token: 0x04000485 RID: 1157
	private bool stateImpulse;

	// Token: 0x04000486 RID: 1158
	private float stateTimer;

	// Token: 0x04000487 RID: 1159
	private EnemyGnome.State attackMoveState;

	// Token: 0x04000488 RID: 1160
	[Space]
	public Enemy enemy;

	// Token: 0x04000489 RID: 1161
	public EnemyGnomeAnim enemyGnomeAnim;

	// Token: 0x0400048A RID: 1162
	private PhotonView photonView;

	// Token: 0x0400048B RID: 1163
	internal int directorIndex;

	// Token: 0x0400048C RID: 1164
	[Space]
	public SpringQuaternion rotationSpring;

	// Token: 0x0400048D RID: 1165
	private Quaternion rotationTarget;

	// Token: 0x0400048E RID: 1166
	[Space]
	public BoxCollider avoidCollider;

	// Token: 0x0400048F RID: 1167
	private float avoidTimer;

	// Token: 0x04000490 RID: 1168
	private Vector3 avoidForce;

	// Token: 0x04000491 RID: 1169
	[Space]
	public float speedMin = 1f;

	// Token: 0x04000492 RID: 1170
	public float speedMax = 2f;

	// Token: 0x04000493 RID: 1171
	[Space]
	public Transform backAwayOffset;

	// Token: 0x04000494 RID: 1172
	public Transform moveOffsetTransform;

	// Token: 0x04000495 RID: 1173
	public Transform rotationTransform;

	// Token: 0x04000496 RID: 1174
	private float moveOffsetTimer;

	// Token: 0x04000497 RID: 1175
	private float moveOffsetSetTimer;

	// Token: 0x04000498 RID: 1176
	private Vector3 moveOffsetPosition;

	// Token: 0x04000499 RID: 1177
	private float attackAngle;

	// Token: 0x0400049A RID: 1178
	private Vector3 moveBackPosition;

	// Token: 0x0400049B RID: 1179
	private float moveBackTimer;

	// Token: 0x0400049C RID: 1180
	private bool visionPrevious;

	// Token: 0x0400049D RID: 1181
	private float visionTimer;

	// Token: 0x0400049E RID: 1182
	internal float attackCooldown;

	// Token: 0x0400049F RID: 1183
	private float idleBreakerTimer;

	// Token: 0x040004A0 RID: 1184
	internal float overlapCheckTimer;

	// Token: 0x040004A1 RID: 1185
	internal float overlapCheckCooldown;

	// Token: 0x040004A2 RID: 1186
	internal bool overlapCheckPrevious;

	// Token: 0x040004A3 RID: 1187
	[Space]
	public ParticleSystem[] deathEffects;

	// Token: 0x040004A4 RID: 1188
	[Space]
	public Sound soundHurt;

	// Token: 0x040004A5 RID: 1189
	public Sound soundDeath;

	// Token: 0x040004A6 RID: 1190
	[Space]
	public Sound soundImpactLight;

	// Token: 0x040004A7 RID: 1191
	public Sound soundImpactMedium;

	// Token: 0x040004A8 RID: 1192
	public Sound soundImpactHeavy;

	// Token: 0x020002D2 RID: 722
	public enum State
	{
		// Token: 0x0400241B RID: 9243
		Spawn,
		// Token: 0x0400241C RID: 9244
		Idle,
		// Token: 0x0400241D RID: 9245
		NoticeDelay,
		// Token: 0x0400241E RID: 9246
		Notice,
		// Token: 0x0400241F RID: 9247
		Move,
		// Token: 0x04002420 RID: 9248
		MoveUnder,
		// Token: 0x04002421 RID: 9249
		MoveOver,
		// Token: 0x04002422 RID: 9250
		MoveBack,
		// Token: 0x04002423 RID: 9251
		AttackMove,
		// Token: 0x04002424 RID: 9252
		Attack,
		// Token: 0x04002425 RID: 9253
		AttackDone,
		// Token: 0x04002426 RID: 9254
		Stun,
		// Token: 0x04002427 RID: 9255
		Despawn
	}
}
