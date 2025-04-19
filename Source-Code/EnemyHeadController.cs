using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class EnemyHeadController : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060002F3 RID: 755 RVA: 0x0001D451 File Offset: 0x0001B651
	private void Awake()
	{
		this.Enemy = base.GetComponentInParent<Enemy>();
		this.Hairs = this.HairParent.GetComponentsInChildren<EnemyHeadHair>().ToList<EnemyHeadHair>();
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0001D478 File Offset: 0x0001B678
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.Enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
		if (this.Enemy.TeleportedTimer > 0f)
		{
			foreach (EnemyHeadHair enemyHeadHair in this.Hairs)
			{
				enemyHeadHair.transform.position = enemyHeadHair.Target.position;
			}
		}
		if (!this.Enemy.MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = this.RotationDistance / num;
			this.LookAtTransform.rotation = Quaternion.RotateTowards(this.LookAtTransform.rotation, this.RotationTarget, num2 * Time.deltaTime);
			return;
		}
		if (this.Enemy.AttackStuckPhysObject.TargetObject)
		{
			if (this.Enemy.AttackStuckPhysObject != null)
			{
				Vector3 vector = this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.transform.position;
				vector += this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.transform.TransformDirection(this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.CheckPosition);
				this.LookAtTransform.LookAt(vector);
			}
		}
		else if (this.Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
		}
		else if (this.Enemy.CurrentState == EnemyState.Chase && this.Enemy.StateChase.VisionTimer > 0f)
		{
			if (this.Enemy.NavMeshAgent.Agent.velocity.normalized.magnitude > 0.1f)
			{
				Quaternion b = Quaternion.LookRotation(this.Enemy.NavMeshAgent.Agent.velocity.normalized);
				this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
				this.LookAtTransform.rotation = Quaternion.Lerp(this.LookAtTransform.rotation, b, 0.25f);
			}
			else
			{
				this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			this.LookAtTransform.LookAt(this.Enemy.StateChase.SawPlayerHidePosition);
			this.LookAtTransform.localEulerAngles = new Vector3(0f, this.LookAtTransform.localEulerAngles.y, 0f);
		}
		else if (this.Enemy.NavMeshAgent.Agent.velocity.magnitude > 0.1f)
		{
			this.LookAtTransform.rotation = Quaternion.LookRotation(this.Enemy.NavMeshAgent.Agent.velocity.normalized);
			this.LookAtTransform.localEulerAngles = new Vector3(0f, this.LookAtTransform.localEulerAngles.y, 0f);
		}
		if (this.Enemy.CurrentState == EnemyState.Despawn)
		{
			this.Enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
			this.Enemy.Rigidbody.DisableFollowRotation(0.1f, 1f);
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0001D8D8 File Offset: 0x0001BAD8
	public void VisionTriggered()
	{
		if (this.Enemy.DisableChaseTimer > 0f)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Chase && this.Enemy.CurrentState != EnemyState.LookUnder)
		{
			if (this.Enemy.CurrentState == EnemyState.ChaseSlow)
			{
				this.Enemy.CurrentState = EnemyState.Chase;
			}
			else if (this.Enemy.Vision.onVisionTriggeredCulled && !this.Enemy.Vision.onVisionTriggeredNear)
			{
				if (this.Enemy.CurrentState != EnemyState.Sneak)
				{
					if (Random.Range(0f, 100f) <= 30f)
					{
						this.Enemy.CurrentState = EnemyState.ChaseBegin;
					}
					else
					{
						this.Enemy.CurrentState = EnemyState.Sneak;
					}
				}
			}
			else if (this.Enemy.Vision.onVisionTriggeredDistance >= 7f)
			{
				this.Enemy.CurrentState = EnemyState.Chase;
				this.Enemy.StateChase.ChaseCanReach = true;
			}
			else
			{
				this.Enemy.CurrentState = EnemyState.ChaseBegin;
			}
			this.Enemy.TargetPlayerViewID = this.Enemy.Vision.onVisionTriggeredPlayer.photonView.ViewID;
			this.Enemy.TargetPlayerAvatar = this.Enemy.Vision.onVisionTriggeredPlayer;
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0001DA20 File Offset: 0x0001BC20
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			SemiFunc.EnemySpawn(this.Enemy);
		}
		this.Visual.Spawn();
		if (this.AnimationSystem.isActiveAndEnabled)
		{
			this.AnimationSystem.OnSpawn();
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0001DA58 File Offset: 0x0001BC58
	public void OnHurt()
	{
		this.AnimationSystem.Hurt.Play(this.Enemy.Rigidbody.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0001DA94 File Offset: 0x0001BC94
	public void OnDeath()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.Enemy.CurrentState = EnemyState.Despawn;
		}
		foreach (GameObject gameObject in this.DeathParticles)
		{
			gameObject.SetActive(true);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.DeathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Enemy.EnemyParent.Despawn();
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0001DB90 File Offset: 0x0001BD90
	public void OnStunnedEnd()
	{
		this.Enemy.CurrentState = EnemyState.Roaming;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0001DBA0 File Offset: 0x0001BDA0
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.LookAtTransform.rotation);
			return;
		}
		this.RotationTarget = (Quaternion)stream.ReceiveNext();
		this.RotationDistance = Quaternion.Angle(base.transform.rotation, this.RotationTarget);
	}

	// Token: 0x04000510 RID: 1296
	private Quaternion RotationTarget;

	// Token: 0x04000511 RID: 1297
	private float RotationDistance;

	// Token: 0x04000512 RID: 1298
	private Enemy Enemy;

	// Token: 0x04000513 RID: 1299
	public EnemyHeadVisual Visual;

	// Token: 0x04000514 RID: 1300
	public EnemyHeadAnimationSystem AnimationSystem;

	// Token: 0x04000515 RID: 1301
	[Space]
	public Transform LookAtTransform;

	// Token: 0x04000516 RID: 1302
	public Transform AnimationTransform;

	// Token: 0x04000517 RID: 1303
	public Transform HairParent;

	// Token: 0x04000518 RID: 1304
	private List<EnemyHeadHair> Hairs;

	// Token: 0x04000519 RID: 1305
	[Space]
	public List<GameObject> DeathParticles;

	// Token: 0x0400051A RID: 1306
	public Sound DeathSound;
}
