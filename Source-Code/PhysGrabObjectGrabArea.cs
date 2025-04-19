using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000199 RID: 409
public class PhysGrabObjectGrabArea : MonoBehaviour
{
	// Token: 0x06000DC3 RID: 3523 RVA: 0x0007CE8C File Offset: 0x0007B08C
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.staticGrabObject = base.GetComponentInParent<StaticGrabObject>();
		this.photonView = base.GetComponentInParent<PhotonView>();
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea in this.grabAreas)
		{
			if (grabArea.grabAreaTransform)
			{
				if (grabArea.grabAreaTransform.childCount == 0)
				{
					Collider component = grabArea.grabAreaTransform.GetComponent<Collider>();
					if (component != null)
					{
						grabArea.grabAreaColliders.Add(component);
					}
					else
					{
						Debug.LogWarning("Grab area '" + grabArea.grabAreaTransform.name + "' is missing a Collider component.");
					}
				}
				else
				{
					Collider[] componentsInChildren = grabArea.grabAreaTransform.GetComponentsInChildren<Collider>();
					if (componentsInChildren.Length != 0)
					{
						grabArea.grabAreaColliders.AddRange(componentsInChildren);
					}
					else
					{
						Debug.LogWarning("Grab area '" + grabArea.grabAreaTransform.name + "' has children but no colliders.");
					}
				}
			}
			else
			{
				Debug.LogWarning("Grab area in '" + base.gameObject.name + "' has a missing Transform. Please assign it.");
			}
		}
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x0007CFC4 File Offset: 0x0007B1C4
	public PlayerAvatar GetLatestGrabber()
	{
		if (this.listOfAllGrabbers.Count > 0)
		{
			return this.listOfAllGrabbers[this.listOfAllGrabbers.Count - 1].playerAvatar;
		}
		return null;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x0007CFF4 File Offset: 0x0007B1F4
	private void Update()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		foreach (PhysGrabber physGrabber in (this.physGrabObject ? this.physGrabObject.playerGrabbing : this.staticGrabObject.playerGrabbing).ToList<PhysGrabber>())
		{
			if (physGrabber.initialPressTimer > 0f)
			{
				Vector3 position = physGrabber.physGrabPoint.position;
				foreach (PhysGrabObjectGrabArea.GrabArea grabArea in this.grabAreas)
				{
					if (grabArea.grabAreaColliders.Count != 0)
					{
						bool flag = false;
						using (List<Collider>.Enumerator enumerator3 = grabArea.grabAreaColliders.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.ClosestPoint(position) == position)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							if (!grabArea.listOfGrabbers.Contains(physGrabber))
							{
								grabArea.listOfGrabbers.Add(physGrabber);
								if (!this.listOfAllGrabbers.Contains(physGrabber))
								{
									this.listOfAllGrabbers.Add(physGrabber);
									this.UpdateList(true, physGrabber);
								}
								UnityEvent grabAreaEventOnStart = grabArea.grabAreaEventOnStart;
								if (grabAreaEventOnStart != null)
								{
									grabAreaEventOnStart.Invoke();
								}
							}
							else
							{
								UnityEvent grabAreaEventOnHolding = grabArea.grabAreaEventOnHolding;
								if (grabAreaEventOnHolding != null)
								{
									grabAreaEventOnHolding.Invoke();
								}
							}
							grabArea.grabAreaActive = true;
							break;
						}
					}
				}
			}
		}
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea2 in this.grabAreas)
		{
			for (int i = grabArea2.listOfGrabbers.Count - 1; i >= 0; i--)
			{
				PhysGrabber physGrabber2 = grabArea2.listOfGrabbers[i];
				if (!physGrabber2.grabbed)
				{
					this.UpdateList(false, physGrabber2);
					this.listOfAllGrabbers.Remove(physGrabber2);
					grabArea2.listOfGrabbers.RemoveAt(i);
				}
			}
		}
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea3 in this.grabAreas)
		{
			if (grabArea3.listOfGrabbers.Count == 0 && grabArea3.grabAreaActive)
			{
				UnityEvent grabAreaEventOnRelease = grabArea3.grabAreaEventOnRelease;
				if (grabAreaEventOnRelease != null)
				{
					grabAreaEventOnRelease.Invoke();
				}
				grabArea3.grabAreaActive = false;
			}
		}
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x0007D2F4 File Offset: 0x0007B4F4
	[PunRPC]
	public void AddToGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = this.FindGrabberById(grabberId);
		if (physGrabber != null && !this.listOfAllGrabbers.Contains(physGrabber))
		{
			this.listOfAllGrabbers.Add(physGrabber);
		}
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0007D32C File Offset: 0x0007B52C
	[PunRPC]
	public void RemoveFromGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = this.FindGrabberById(grabberId);
		if (physGrabber != null)
		{
			this.listOfAllGrabbers.Remove(physGrabber);
		}
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0007D358 File Offset: 0x0007B558
	private PhysGrabber FindGrabberById(int id)
	{
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			PhysGrabber componentInChildren = playerAvatar.GetComponentInChildren<PhysGrabber>();
			if (componentInChildren != null && componentInChildren.photonView.ViewID == id)
			{
				return componentInChildren;
			}
		}
		return null;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0007D3C8 File Offset: 0x0007B5C8
	private void UpdateList(bool add, PhysGrabber grabber)
	{
		if (!SemiFunc.IsMultiplayer() || grabber == null)
		{
			return;
		}
		int viewID = grabber.photonView.ViewID;
		if (add)
		{
			this.photonView.RPC("AddToGrabbersList", RpcTarget.Others, new object[]
			{
				viewID
			});
			return;
		}
		this.photonView.RPC("RemoveFromGrabbersList", RpcTarget.Others, new object[]
		{
			viewID
		});
	}

	// Token: 0x040016AD RID: 5805
	private PhysGrabObject physGrabObject;

	// Token: 0x040016AE RID: 5806
	private StaticGrabObject staticGrabObject;

	// Token: 0x040016AF RID: 5807
	private PhotonView photonView;

	// Token: 0x040016B0 RID: 5808
	[HideInInspector]
	public List<PhysGrabber> listOfAllGrabbers = new List<PhysGrabber>();

	// Token: 0x040016B1 RID: 5809
	public List<PhysGrabObjectGrabArea.GrabArea> grabAreas = new List<PhysGrabObjectGrabArea.GrabArea>();

	// Token: 0x0200036B RID: 875
	[Serializable]
	public class GrabArea
	{
		// Token: 0x04002778 RID: 10104
		public Transform grabAreaTransform;

		// Token: 0x04002779 RID: 10105
		[Space(20f)]
		public UnityEvent grabAreaEventOnStart = new UnityEvent();

		// Token: 0x0400277A RID: 10106
		public UnityEvent grabAreaEventOnRelease = new UnityEvent();

		// Token: 0x0400277B RID: 10107
		public UnityEvent grabAreaEventOnHolding = new UnityEvent();

		// Token: 0x0400277C RID: 10108
		[HideInInspector]
		public bool grabAreaActive;

		// Token: 0x0400277D RID: 10109
		[HideInInspector]
		public List<PhysGrabber> listOfGrabbers = new List<PhysGrabber>();

		// Token: 0x0400277E RID: 10110
		[HideInInspector]
		public List<Collider> grabAreaColliders = new List<Collider>();
	}
}
