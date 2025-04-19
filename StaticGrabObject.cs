using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class StaticGrabObject : MonoBehaviour
{
	// Token: 0x06000DD2 RID: 3538 RVA: 0x0007DA97 File Offset: 0x0007BC97
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient)
		{
			this.isMaster = true;
			this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
		}
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0007DAD0 File Offset: 0x0007BCD0
	private void Update()
	{
		if (this.grabbed)
		{
			for (int i = 0; i < this.playerGrabbing.Count; i++)
			{
				if (!this.playerGrabbing[i])
				{
					this.playerGrabbing.RemoveAt(i);
				}
			}
		}
		if (GameManager.instance.gameMode == 0 || this.isMaster)
		{
			this.velocity = Vector3.zero;
			foreach (PhysGrabber physGrabber in this.playerGrabbing)
			{
				Vector3 a = (physGrabber.physGrabPointPullerPosition - physGrabber.physGrabPoint.position) * 5f;
				this.velocity += a * Time.deltaTime;
			}
			if (this.dead && this.playerGrabbing.Count == 0)
			{
				this.DestroyPhysGrabObject();
			}
		}
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0007DBD4 File Offset: 0x0007BDD4
	private void OnDisable()
	{
		this.playerGrabbing.Clear();
		this.grabbed = false;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0007DBE8 File Offset: 0x0007BDE8
	public void GrabLink(int playerPhotonID, Vector3 point)
	{
		this.photonView.RPC("GrabLinkRPC", RpcTarget.All, new object[]
		{
			playerPhotonID,
			point
		});
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0007DC14 File Offset: 0x0007BE14
	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, Vector3 point)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = this.colliderTransform.InverseTransformPoint(point);
		component.grabbedObjectTransform = this.colliderTransform;
		component.grabbed = true;
		if (component.photonView.IsMine)
		{
			Vector3 localPosition = component.physGrabPoint.localPosition;
			this.photonView.RPC("GrabPointSyncRPC", RpcTarget.MasterClient, new object[]
			{
				playerPhotonID,
				localPosition
			});
		}
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0007DCA0 File Offset: 0x0007BEA0
	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>().physGrabPoint.localPosition = localPointInBox;
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0007DCB8 File Offset: 0x0007BEB8
	public void GrabStarted(PhysGrabber player)
	{
		if (!this.grabbed)
		{
			this.grabbed = true;
			if (GameManager.instance.gameMode == 0)
			{
				if (!this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Add(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabStartedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0007DD28 File Offset: 0x0007BF28
	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		if (!this.playerGrabbing.Contains(component))
		{
			this.playerGrabbing.Add(component);
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0007DD5C File Offset: 0x0007BF5C
	public void GrabEnded(PhysGrabber player)
	{
		if (this.grabbed)
		{
			this.grabbed = false;
			if (GameManager.instance.gameMode == 0)
			{
				if (this.playerGrabbing.Contains(player))
				{
					this.playerGrabbing.Remove(player);
					return;
				}
			}
			else
			{
				this.photonView.RPC("GrabEndedRPC", RpcTarget.MasterClient, new object[]
				{
					player.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0007DDCC File Offset: 0x0007BFCC
	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID)
	{
		PhysGrabber component = PhotonView.Find(playerPhotonID).GetComponent<PhysGrabber>();
		component.grabbed = false;
		if (this.playerGrabbing.Contains(component))
		{
			this.playerGrabbing.Remove(component);
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x0007DE07 File Offset: 0x0007C007
	private void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyPhysObjectFailsafe();
			Object.Destroy(base.gameObject);
			return;
		}
		this.photonView.RPC("DestroyPhysGrabObjectRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0007DE3D File Offset: 0x0007C03D
	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		this.DestroyPhysObjectFailsafe();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0007DE50 File Offset: 0x0007C050
	private void DestroyPhysObjectFailsafe()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.CompareTag("Phys Grab Controller"))
			{
				transform.SetParent(null);
			}
		}
	}

	// Token: 0x040016BB RID: 5819
	private PhotonView photonView;

	// Token: 0x040016BC RID: 5820
	private bool isMaster;

	// Token: 0x040016BD RID: 5821
	public Transform colliderTransform;

	// Token: 0x040016BE RID: 5822
	[HideInInspector]
	public Vector3 velocity;

	// Token: 0x040016BF RID: 5823
	[HideInInspector]
	public bool grabbed;

	// Token: 0x040016C0 RID: 5824
	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	// Token: 0x040016C1 RID: 5825
	[HideInInspector]
	public bool dead;
}
