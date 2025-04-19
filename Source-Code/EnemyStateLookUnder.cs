using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class EnemyStateLookUnder : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060005C1 RID: 1473 RVA: 0x00038B5D File Offset: 0x00036D5D
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00038B78 File Offset: 0x00036D78
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.LookUnder)
		{
			if (this.Active)
			{
				this.WaitDone = false;
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.WaitDone = false;
			this.WaitTimer = Random.Range(this.WaitTimerMin, this.WaitTimerMax);
			this.LookTimer = Random.Range(this.LookTimerMin, this.LookTimerMax);
			this.Active = true;
		}
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		this.Enemy.SetChaseTimer();
		this.Enemy.NavMeshAgent.SetDestination(this.Enemy.StateChase.SawPlayerNavmeshPosition);
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		if (!this.WaitDone)
		{
			if (Vector3.Distance(base.transform.position, this.Enemy.StateChase.SawPlayerNavmeshPosition) < 0.1f)
			{
				this.WaitTimer -= Time.deltaTime;
				if (this.WaitTimer <= 0f)
				{
					this.WaitDone = true;
				}
			}
		}
		else
		{
			this.LookTimer -= Time.deltaTime;
			if (this.LookTimer <= 0f)
			{
				this.Enemy.CurrentState = EnemyState.ChaseSlow;
			}
		}
		if (this.Enemy.Vision.VisionTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] && this.Enemy.StateChase.SawPlayerNavmeshPosition != this.Enemy.TargetPlayerAvatar.LastNavmeshPosition)
		{
			this.Enemy.CurrentState = EnemyState.Chase;
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00038D25 File Offset: 0x00036F25
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.WaitDone);
			return;
		}
		this.WaitDone = (bool)stream.ReceiveNext();
	}

	// Token: 0x0400096A RID: 2410
	private Enemy Enemy;

	// Token: 0x0400096B RID: 2411
	private PlayerController Player;

	// Token: 0x0400096C RID: 2412
	private bool Active;

	// Token: 0x0400096D RID: 2413
	public float Speed;

	// Token: 0x0400096E RID: 2414
	public float Acceleration;

	// Token: 0x0400096F RID: 2415
	[Space]
	public float WaitTimerMin;

	// Token: 0x04000970 RID: 2416
	public float WaitTimerMax;

	// Token: 0x04000971 RID: 2417
	internal float WaitTimer = 999f;

	// Token: 0x04000972 RID: 2418
	internal bool WaitDone;

	// Token: 0x04000973 RID: 2419
	[Space]
	public float LookTimerMin;

	// Token: 0x04000974 RID: 2420
	public float LookTimerMax;

	// Token: 0x04000975 RID: 2421
	private float LookTimer = 999f;
}
