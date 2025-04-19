using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class EnemyStateSneak : MonoBehaviour
{
	// Token: 0x060005CD RID: 1485 RVA: 0x000393B6 File Offset: 0x000375B6
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.Player = PlayerController.instance;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x000393D0 File Offset: 0x000375D0
	private void Update()
	{
		if (this.Enemy.CurrentState != EnemyState.Sneak)
		{
			if (this.Active)
			{
				this.Active = false;
			}
			return;
		}
		if (!this.Active)
		{
			this.TargetPlayer = PlayerController.instance.playerAvatarScript;
			if (GameManager.instance.gameMode == 1)
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled && playerAvatar.photonView.ViewID == this.Enemy.TargetPlayerViewID)
					{
						this.TargetPlayer = playerAvatar;
					}
				}
			}
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
			this.Active = true;
		}
		if (!this.Enemy.MasterClient)
		{
			return;
		}
		this.Enemy.NavMeshAgent.UpdateAgent(this.Speed, this.Acceleration);
		this.Enemy.NavMeshAgent.SetDestination(this.TargetPlayer.transform.position);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.IdleSet(0.1f);
		}
		if (this.Enemy.Vision.VisionsTriggered[this.Enemy.TargetPlayerAvatar.photonView.ViewID] >= this.Enemy.Vision.VisionsToTrigger)
		{
			this.StateTimer = Random.Range(this.StateTimeMin, this.StateTimeMax);
		}
		this.StateTimer -= Time.deltaTime;
		if (this.StateTimer <= 0f)
		{
			this.Enemy.CurrentState = EnemyState.Roaming;
		}
	}

	// Token: 0x0400099B RID: 2459
	private Enemy Enemy;

	// Token: 0x0400099C RID: 2460
	private PlayerController Player;

	// Token: 0x0400099D RID: 2461
	private bool Active;

	// Token: 0x0400099E RID: 2462
	public float Speed;

	// Token: 0x0400099F RID: 2463
	public float Acceleration;

	// Token: 0x040009A0 RID: 2464
	[Space]
	public float StateTimeMin;

	// Token: 0x040009A1 RID: 2465
	public float StateTimeMax;

	// Token: 0x040009A2 RID: 2466
	private float StateTimer;

	// Token: 0x040009A3 RID: 2467
	private PlayerAvatar TargetPlayer;
}
