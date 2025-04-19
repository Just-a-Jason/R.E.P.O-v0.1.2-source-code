using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class PlayerAvatarCollision : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000E5D RID: 3677 RVA: 0x000812E1 File Offset: 0x0007F4E1
	private void Start()
	{
		this.PlayerController = PlayerController.instance;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x000812F0 File Offset: 0x0007F4F0
	private void Update()
	{
		if (this.PlayerAvatar.isLocal)
		{
			this.Scale = this.PlayerController.PlayerCollision.transform.localScale;
			this.Collider.enabled = false;
		}
		this.CollisionTransform.localScale = this.Scale;
		this.deathHeadPosition = this.CollisionTransform.position + Vector3.up * (this.Collider.height * this.CollisionTransform.localScale.y - 0.18f);
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00081384 File Offset: 0x0007F584
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.Scale);
			return;
		}
		this.Scale = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x000813B1 File Offset: 0x0007F5B1
	public void SetCrouch()
	{
		this.Scale = PlayerCollision.instance.CrouchCollision.localScale;
		this.CollisionTransform.localScale = this.Scale;
	}

	// Token: 0x04001775 RID: 6005
	public PlayerAvatar PlayerAvatar;

	// Token: 0x04001776 RID: 6006
	private PlayerController PlayerController;

	// Token: 0x04001777 RID: 6007
	public Transform CollisionTransform;

	// Token: 0x04001778 RID: 6008
	public CapsuleCollider Collider;

	// Token: 0x04001779 RID: 6009
	private Vector3 Scale;

	// Token: 0x0400177A RID: 6010
	internal Vector3 deathHeadPosition;
}
