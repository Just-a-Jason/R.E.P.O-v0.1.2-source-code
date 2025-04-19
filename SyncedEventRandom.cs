using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class SyncedEventRandom : MonoBehaviour
{
	// Token: 0x06000DE0 RID: 3552 RVA: 0x0007DECB File Offset: 0x0007C0CB
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0007DEDC File Offset: 0x0007C0DC
	public void RandomRangeFloat(float min, float max)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.resultRandomRangeFloat = Random.Range(min, max);
			this.resultReceivedRandomRangeFloat = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.resultRandomRangeFloat = Random.Range(min, max);
			this.resultReceivedRandomRangeFloat = true;
			this.photonView.RPC("RandomRangeFloatRPC", RpcTarget.Others, new object[]
			{
				this.resultRandomRangeFloat
			});
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0007DF4A File Offset: 0x0007C14A
	[PunRPC]
	private void RandomRangeFloatRPC(float result)
	{
		this.resultRandomRangeFloat = result;
		this.resultReceivedRandomRangeFloat = true;
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0007DF5C File Offset: 0x0007C15C
	public void RandomRangeInt(int min, int max)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.resultRandomRangeInt = Random.Range(min, max);
			this.resultReceivedRandomRangeInt = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.resultRandomRangeInt = Random.Range(min, max);
			this.resultReceivedRandomRangeInt = true;
			this.photonView.RPC("RandomRangeIntRPC", RpcTarget.Others, new object[]
			{
				this.resultRandomRangeInt
			});
		}
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0007DFCA File Offset: 0x0007C1CA
	[PunRPC]
	private void RandomRangeIntRPC(int result)
	{
		this.resultRandomRangeInt = result;
		this.resultReceivedRandomRangeInt = true;
	}

	// Token: 0x040016C2 RID: 5826
	[HideInInspector]
	public float resultRandomRangeFloat;

	// Token: 0x040016C3 RID: 5827
	[HideInInspector]
	public int resultRandomRangeInt;

	// Token: 0x040016C4 RID: 5828
	[HideInInspector]
	public bool resultReceivedRandomRangeFloat;

	// Token: 0x040016C5 RID: 5829
	[HideInInspector]
	public bool resultReceivedRandomRangeInt;

	// Token: 0x040016C6 RID: 5830
	private PhotonView photonView;
}
