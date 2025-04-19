using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class EnemyHeadVisual : MonoBehaviour, IPunObservable
{
	// Token: 0x06000316 RID: 790 RVA: 0x0001E60C File Offset: 0x0001C80C
	private void Update()
	{
		if (this.enemy.FreezeTimer > 0f)
		{
			return;
		}
		if (this.enemy.MasterClient)
		{
			if (this.enemy.CheckChase())
			{
				this.PositionFollowCurrent = this.PositionFollowChasing;
				this.RotationFollowCurrent = this.RotationFollowChasing;
			}
			else
			{
				this.PositionFollowCurrent = this.PositionFollowIdle;
				this.RotationFollowCurrent = this.RotationFollowIdle;
			}
		}
		if (this.spawnTimer > 0f || this.enemy.TeleportedTimer > 0f)
		{
			base.transform.position = this.FollowPosition.position;
			this.TargetRotation.rotation = this.FollowRotation.rotation;
			if (LevelGenerator.Instance.Generated)
			{
				this.spawnTimer -= Time.deltaTime;
				return;
			}
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.FollowPosition.position, this.PositionFollowCurrent * Time.deltaTime);
			this.TargetRotation.rotation = Quaternion.Lerp(this.TargetRotation.rotation, this.FollowRotation.rotation, this.RotationFollowCurrent * Time.deltaTime);
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0001E746 File Offset: 0x0001C946
	public void Spawn()
	{
		base.transform.position = this.FollowPosition.position;
		this.TargetRotation.rotation = this.FollowRotation.rotation;
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0001E774 File Offset: 0x0001C974
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.PositionFollowCurrent);
			stream.SendNext(this.RotationFollowCurrent);
			return;
		}
		this.PositionFollowCurrent = (float)stream.ReceiveNext();
		this.RotationFollowCurrent = (float)stream.ReceiveNext();
	}

	// Token: 0x0400055F RID: 1375
	public EnemyHeadController Controller;

	// Token: 0x04000560 RID: 1376
	public Enemy enemy;

	// Token: 0x04000561 RID: 1377
	private float spawnTimer = 1f;

	// Token: 0x04000562 RID: 1378
	[Space]
	public Transform FollowPosition;

	// Token: 0x04000563 RID: 1379
	public Transform FollowRotation;

	// Token: 0x04000564 RID: 1380
	public Transform TargetRotation;

	// Token: 0x04000565 RID: 1381
	private float PositionFollowCurrent;

	// Token: 0x04000566 RID: 1382
	private float RotationFollowCurrent;

	// Token: 0x04000567 RID: 1383
	[Space]
	[Header("Idle")]
	public float PositionFollowIdle;

	// Token: 0x04000568 RID: 1384
	public float RotationFollowIdle;

	// Token: 0x04000569 RID: 1385
	[Space]
	[Header("Chasing")]
	public float PositionFollowChasing;

	// Token: 0x0400056A RID: 1386
	public float RotationFollowChasing;
}
