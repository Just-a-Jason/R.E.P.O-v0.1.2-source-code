using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class PlayerPhysPusher : MonoBehaviour
{
	// Token: 0x06000EFE RID: 3838 RVA: 0x00088D4F File Offset: 0x00086F4F
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x00088D69 File Offset: 0x00086F69
	private void Start()
	{
		if (GameManager.instance.gameMode == 0 || !PhotonNetwork.IsMasterClient || this.PhotonView.IsMine)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x00088D98 File Offset: 0x00086F98
	private void FixedUpdate()
	{
		if (this.Player.isDisabled || this.Player.isTumbling || this.Player.rbVelocity.magnitude < 0.1f)
		{
			this.Collider.gameObject.SetActive(false);
		}
		else
		{
			this.Collider.gameObject.SetActive(true);
		}
		float num = Vector3.Distance(base.transform.position, this.ColliderTarget.position);
		if ((this.Reset && num > 0.5f) || num > 1f || this.Player.rbVelocity.magnitude < 0.1f || Vector3.Dot(this.Player.rbVelocity, this.PreviousVelocity) < 0.25f)
		{
			this.Rigidbody.MovePosition(this.ColliderTarget.position);
			this.Reset = false;
		}
		this.Rigidbody.MoveRotation(this.ColliderTarget.rotation);
		Vector3 b = base.transform.InverseTransformDirection(this.Rigidbody.velocity);
		this.Rigidbody.AddRelativeForce(this.Player.rbVelocity - b, ForceMode.Impulse);
		this.PreviousVelocity = this.Player.rbVelocity;
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00088EDA File Offset: 0x000870DA
	private void Update()
	{
		this.Collider.localScale = this.ColliderTarget.localScale;
	}

	// Token: 0x04001938 RID: 6456
	private PhotonView PhotonView;

	// Token: 0x04001939 RID: 6457
	private Rigidbody Rigidbody;

	// Token: 0x0400193A RID: 6458
	public PlayerAvatar Player;

	// Token: 0x0400193B RID: 6459
	[Space]
	public Transform ColliderTarget;

	// Token: 0x0400193C RID: 6460
	public Transform Collider;

	// Token: 0x0400193D RID: 6461
	internal bool Reset;

	// Token: 0x0400193E RID: 6462
	private Vector3 PreviousVelocity;
}
