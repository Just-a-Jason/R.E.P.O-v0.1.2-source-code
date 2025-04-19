using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class PlayerVisionTarget : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000F2A RID: 3882 RVA: 0x0008A484 File Offset: 0x00088684
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0008A492 File Offset: 0x00088692
	private void Start()
	{
		this.PlayerAvatar = base.GetComponent<PlayerAvatar>();
		this.CurrentPosition = this.StandPosition;
		this.PlayerController = PlayerController.instance;
		this.MainCamera = Camera.main;
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0008A4C4 File Offset: 0x000886C4
	private void Update()
	{
		if (this.PlayerAvatar.isLocal)
		{
			if (this.PlayerController.Crouching)
			{
				if (this.PlayerController.Crawling)
				{
					this.TargetPosition = this.CrawlPosition;
				}
				else
				{
					this.TargetPosition = this.CrouchPosition;
				}
			}
			else
			{
				this.TargetPosition = this.StandPosition;
			}
			this.TargetRotation = this.MainCamera.transform.rotation;
		}
		this.CurrentPosition = Mathf.Lerp(this.CurrentPosition, this.TargetPosition, Time.deltaTime * this.LerpSpeed);
		this.CurrentRotation = Quaternion.Slerp(this.CurrentRotation, this.TargetRotation, Time.deltaTime * 20f);
		this.VisionTransform.localPosition = new Vector3(0f, this.CurrentPosition, 0f);
		this.VisionTransform.rotation = this.CurrentRotation;
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0008A5AC File Offset: 0x000887AC
	private void OnDrawGizmos()
	{
		if (this.DebugMeshActive)
		{
			Gizmos.color = new Color(0f, 1f, 0.13f, 0.75f);
			Gizmos.matrix = this.VisionTransform.localToWorldMatrix;
			Gizmos.DrawMesh(this.DebugMesh, 0, Vector3.zero, Quaternion.identity, Vector3.one * 0.15f);
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0008A614 File Offset: 0x00088814
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.TargetPosition);
			stream.SendNext(this.TargetRotation);
			return;
		}
		this.TargetPosition = (float)stream.ReceiveNext();
		this.TargetRotation = (Quaternion)stream.ReceiveNext();
	}

	// Token: 0x0400196A RID: 6506
	private PhotonView PhotonView;

	// Token: 0x0400196B RID: 6507
	private PlayerAvatar PlayerAvatar;

	// Token: 0x0400196C RID: 6508
	private PlayerController PlayerController;

	// Token: 0x0400196D RID: 6509
	public Transform VisionTransform;

	// Token: 0x0400196E RID: 6510
	private Camera MainCamera;

	// Token: 0x0400196F RID: 6511
	[Space]
	public float StandPosition;

	// Token: 0x04001970 RID: 6512
	public float CrouchPosition;

	// Token: 0x04001971 RID: 6513
	public float CrawlPosition;

	// Token: 0x04001972 RID: 6514
	[Space]
	public float HeadStandPosition;

	// Token: 0x04001973 RID: 6515
	public float HeadCrouchPosition;

	// Token: 0x04001974 RID: 6516
	public float HeadCrawlPosition;

	// Token: 0x04001975 RID: 6517
	private float TargetPosition;

	// Token: 0x04001976 RID: 6518
	private Quaternion TargetRotation;

	// Token: 0x04001977 RID: 6519
	internal float CurrentPosition;

	// Token: 0x04001978 RID: 6520
	internal Quaternion CurrentRotation;

	// Token: 0x04001979 RID: 6521
	public float LerpSpeed;

	// Token: 0x0400197A RID: 6522
	[Space]
	public bool DebugMeshActive = true;

	// Token: 0x0400197B RID: 6523
	public Mesh DebugMesh;
}
