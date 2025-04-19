using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class EnemyHeadAnimationSystem : MonoBehaviour
{
	// Token: 0x060002CA RID: 714 RVA: 0x0001C4A4 File Offset: 0x0001A6A4
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Animator.keepAnimatorStateOnDisable = true;
		this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
		this.AnimatorIdle = Animator.StringToHash("Idle");
		this.AnimatorIdleTeeth = Animator.StringToHash("IdleTeeth");
		this.AnimatorIdleBite = Animator.StringToHash("IdleBite");
		this.AnimatorChaseBite = Animator.StringToHash("ChaseBite");
		this.AnimatorChaseBegin = Animator.StringToHash("ChaseBegin");
		this.AnimatorChase = Animator.StringToHash("Chase");
		this.AnimatorDespawn = Animator.StringToHash("Despawn");
		this.AnimatorSpawn = Animator.StringToHash("Spawn");
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0001C560 File Offset: 0x0001A760
	public void SetChaseBeginToChase()
	{
		this.ChaseBeginToChase.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001C5F4 File Offset: 0x0001A7F4
	public void SetChaseToIdle()
	{
		this.ChaseToIdle.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0001C688 File Offset: 0x0001A888
	public void SetChaseBegin()
	{
		this.ChaseBegin.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.ChaseBeginGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0001C747 File Offset: 0x0001A947
	public void PlayTeethChatter()
	{
		this.TeethChatter.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0001C774 File Offset: 0x0001A974
	public void MaterialImpact()
	{
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, false, this.MaterialTrigger, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0001C800 File Offset: 0x0001AA00
	public void Slide()
	{
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 1f, false);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0001C88A File Offset: 0x0001AA8A
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0001C8B7 File Offset: 0x0001AAB7
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0001C8E4 File Offset: 0x0001AAE4
	public void AttackStuckPhysObject()
	{
		if (!this.IdleBiteTrigger)
		{
			this.IdleBiteTrigger = true;
			this.IdleBite();
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0001C8FC File Offset: 0x0001AAFC
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Spawn || this.Enemy.CurrentState == EnemyState.Roaming || this.Enemy.CurrentState == EnemyState.Investigate || this.Enemy.CurrentState == EnemyState.ChaseEnd || this.Enemy.IsStunned())
		{
			if (this.Enemy.MasterClient && this.IdleTeethTime > 0f)
			{
				this.IdleTeethTime -= Time.deltaTime;
				if (this.IdleTeethTime <= 0f)
				{
					this.IdleTeeth();
					this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
				}
			}
			if (this.IdleTeethTrigger)
			{
				this.Animator.SetTrigger(this.AnimatorIdleTeeth);
				this.IdleTeethTrigger = false;
			}
			this.Animator.SetBool(this.AnimatorIdle, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorIdle, false);
			if (this.IdleTeethTime < this.IdleTeethTimeMin)
			{
				this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
			}
		}
		if (this.Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			this.Animator.SetBool(this.AnimatorChaseBegin, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorChaseBegin, false);
		}
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			this.Animator.SetBool(this.AnimatorChase, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorChase, false);
		}
		if (this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chase Bite"))
		{
			this.EnemyTriggerAttack.Attack = false;
		}
		else if (this.EnemyTriggerAttack.Attack)
		{
			this.ChaseBiteTrigger();
		}
		if (this.Enemy.CurrentState == EnemyState.LookUnder && this.Enemy.StateLookUnder.WaitDone)
		{
			this.EnemyRigidbody.OverrideFollowPosition(0.1f, 50f, -1f);
			this.EnemyRigidbody.OverrideFollowRotation(0.1f, 2f);
			this.LookUnderOffset.Active(0.1f);
			this.EnemyHeadFloat.Disable(0.5f);
		}
		this.ChaseLoop.PlayLoop(this.ChaseLoopActive, 5f, 5f, 1f);
		this.ChaseLoop2.PlayLoop(this.ChaseLoopActive, 5f, 5f, 1f);
		if ((this.Enemy.CurrentState == EnemyState.Despawn || this.Enemy.Health.dead) && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Despawn") && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chase Bite"))
		{
			this.Animator.SetTrigger(this.AnimatorDespawn);
		}
		if (this.Enemy.CurrentState == EnemyState.Spawn && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
		{
			this.Animator.SetTrigger(this.AnimatorSpawn);
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0001CC16 File Offset: 0x0001AE16
	private void IdleTeeth()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.IdleTeethRPC();
			return;
		}
		this.PhotonView.RPC("IdleTeethRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0001CC41 File Offset: 0x0001AE41
	[PunRPC]
	private void IdleTeethRPC()
	{
		this.IdleTeethTrigger = true;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0001CC4A File Offset: 0x0001AE4A
	private void ChaseBiteTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ChaseBiteTriggerRPC();
			return;
		}
		this.PhotonView.RPC("ChaseBiteTriggerRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0001CC75 File Offset: 0x0001AE75
	[PunRPC]
	private void ChaseBiteTriggerRPC()
	{
		this.Animator.SetTrigger(this.AnimatorChaseBite);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0001CC88 File Offset: 0x0001AE88
	public void PlayChaseBiteStart()
	{
		this.BiteStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0001CCB8 File Offset: 0x0001AEB8
	public void PlayChaseBiteImpact()
	{
		this.Enemy.Rigidbody.GrabRelease();
		this.BiteEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.EnemyRigidbody.DisableFollowPosition(0.25f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(0.25f, 2f);
		this.EnemyRigidbody.rb.AddForce(this.EnemyRigidbody.transform.forward * 10f, ForceMode.Impulse);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0001CDB1 File Offset: 0x0001AFB1
	public void PlayBiteStart()
	{
		this.BiteStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0001CDDE File Offset: 0x0001AFDE
	private void IdleBite()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.IdleBiteRPC();
			return;
		}
		this.PhotonView.RPC("IdleBiteRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001CE09 File Offset: 0x0001B009
	[PunRPC]
	private void IdleBiteRPC()
	{
		this.Animator.SetTrigger(this.AnimatorIdleBite);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0001CE1C File Offset: 0x0001B01C
	public void IdleBiteImpact()
	{
		this.BiteEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.EnemyRigidbody.DisableFollowPosition(1f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(1f, 1f);
		this.EnemyRigidbody.rb.AddForce(this.EnemyRigidbody.transform.forward * 10f, ForceMode.Impulse);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0001CEAC File Offset: 0x0001B0AC
	public void IdleBiteDone()
	{
		this.Enemy.AttackStuckPhysObject.Reset();
		this.IdleBiteTrigger = false;
		this.EnemyRigidbody.DisableFollowPosition(0.2f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(0.5f, 1f);
		this.EnemyRigidbody.rb.AddForce(-this.EnemyRigidbody.transform.forward * 2f, ForceMode.Impulse);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0001CF2A File Offset: 0x0001B12A
	public void OnSpawn()
	{
		this.Animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0001CF44 File Offset: 0x0001B144
	public void PlayDespawn()
	{
		this.Despawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0001CFD8 File Offset: 0x0001B1D8
	public void PlaySpawn()
	{
		this.Spawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0001D06C File Offset: 0x0001B26C
	public void TeleportParticlesStart()
	{
		this.TeleportParticlesTop.Play();
		this.TeleportParticlesBot.Play();
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0001D084 File Offset: 0x0001B284
	public void TeleportParticlesStop()
	{
		this.TeleportParticlesTop.Stop();
		this.TeleportParticlesBot.Stop();
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0001D09C File Offset: 0x0001B29C
	private void DespawnSet()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DespawnSetRPC();
			return;
		}
		this.PhotonView.RPC("DespawnSetRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0001D0C7 File Offset: 0x0001B2C7
	[PunRPC]
	private void DespawnSetRPC()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.Enemy.StateDespawn.Despawn();
		}
	}

	// Token: 0x040004D5 RID: 1237
	private PhotonView PhotonView;

	// Token: 0x040004D6 RID: 1238
	public Enemy Enemy;

	// Token: 0x040004D7 RID: 1239
	public Animator Animator;

	// Token: 0x040004D8 RID: 1240
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x040004D9 RID: 1241
	public EnemyRigidbody EnemyRigidbody;

	// Token: 0x040004DA RID: 1242
	public AnimatedOffset LookUnderOffset;

	// Token: 0x040004DB RID: 1243
	public EnemyHeadFloat EnemyHeadFloat;

	// Token: 0x040004DC RID: 1244
	public ParticleSystem TeleportParticlesTop;

	// Token: 0x040004DD RID: 1245
	public ParticleSystem TeleportParticlesBot;

	// Token: 0x040004DE RID: 1246
	public EnemyTriggerAttack EnemyTriggerAttack;

	// Token: 0x040004DF RID: 1247
	[Space]
	public Sound ChaseBegin;

	// Token: 0x040004E0 RID: 1248
	public Sound ChaseBeginGlobal;

	// Token: 0x040004E1 RID: 1249
	public Sound ChaseBeginToChase;

	// Token: 0x040004E2 RID: 1250
	public Sound ChaseToIdle;

	// Token: 0x040004E3 RID: 1251
	public bool ChaseLoopActive;

	// Token: 0x040004E4 RID: 1252
	public Sound ChaseLoop;

	// Token: 0x040004E5 RID: 1253
	public Sound ChaseLoop2;

	// Token: 0x040004E6 RID: 1254
	public Sound TeethChatter;

	// Token: 0x040004E7 RID: 1255
	public Sound MoveLong;

	// Token: 0x040004E8 RID: 1256
	public Sound MoveShort;

	// Token: 0x040004E9 RID: 1257
	public Sound BiteStart;

	// Token: 0x040004EA RID: 1258
	public Sound BiteEnd;

	// Token: 0x040004EB RID: 1259
	public Sound Spawn;

	// Token: 0x040004EC RID: 1260
	public Sound Despawn;

	// Token: 0x040004ED RID: 1261
	public Sound Hurt;

	// Token: 0x040004EE RID: 1262
	[Space]
	public float IdleTeethTimeMin;

	// Token: 0x040004EF RID: 1263
	public float IdleTeethTimeMax;

	// Token: 0x040004F0 RID: 1264
	private float IdleTeethTime;

	// Token: 0x040004F1 RID: 1265
	private bool IdleTeethTrigger;

	// Token: 0x040004F2 RID: 1266
	private bool IdleBiteTrigger;

	// Token: 0x040004F3 RID: 1267
	private int AnimatorIdle;

	// Token: 0x040004F4 RID: 1268
	private int AnimatorChaseBegin;

	// Token: 0x040004F5 RID: 1269
	private int AnimatorChase;

	// Token: 0x040004F6 RID: 1270
	private int AnimatorIdleTeeth;

	// Token: 0x040004F7 RID: 1271
	private int AnimatorIdleBite;

	// Token: 0x040004F8 RID: 1272
	private int AnimatorChaseBite;

	// Token: 0x040004F9 RID: 1273
	private int AnimatorDespawn;

	// Token: 0x040004FA RID: 1274
	private int AnimatorSpawn;

	// Token: 0x040004FB RID: 1275
	private bool Idle;
}
