using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class PaperInteraction : MonoBehaviour
{
	// Token: 0x060006E1 RID: 1761 RVA: 0x0004124C File Offset: 0x0003F44C
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				int num = Random.Range(0, this.papers.Count);
				Vector3 vector = new Vector3(0f, (float)Random.Range(0, 360), 0f);
				this.photonView.RPC("SyncPaperVisual", RpcTarget.AllBuffered, new object[]
				{
					num,
					vector
				});
				return;
			}
		}
		else
		{
			this.paperVisual = Object.Instantiate<GameObject>(this.papers[Random.Range(0, this.papers.Count)], base.transform.position, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f));
			this.paperVisual.transform.parent = this.PaperTransform;
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00041338 File Offset: 0x0003F538
	private void Update()
	{
		if (this.Picked)
		{
			if (GameManager.instance.gameMode == 1)
			{
				if (!this.destructionToMaster)
				{
					this.photonView.RPC("DestroyPaper", RpcTarget.MasterClient, Array.Empty<object>());
					this.destructionToMaster = true;
					return;
				}
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x0004138B File Offset: 0x0003F58B
	[PunRPC]
	public void SyncPaperVisual(int randomPaper, Vector3 randomRotation)
	{
		this.paperVisual = Object.Instantiate<GameObject>(this.papers[randomPaper], base.transform.position, Quaternion.Euler(randomRotation));
		this.paperVisual.transform.parent = this.PaperTransform;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000413CB File Offset: 0x0003F5CB
	[PunRPC]
	public void DestroyPaper()
	{
		if (!this.destructionToOthers)
		{
			PhotonNetwork.Destroy(base.gameObject);
			this.destructionToOthers = true;
		}
	}

	// Token: 0x04000B9B RID: 2971
	public List<GameObject> papers;

	// Token: 0x04000B9C RID: 2972
	[HideInInspector]
	public bool Picked;

	// Token: 0x04000B9D RID: 2973
	public Transform PaperTransform;

	// Token: 0x04000B9E RID: 2974
	[HideInInspector]
	public GameObject paperVisual;

	// Token: 0x04000B9F RID: 2975
	public CleanEffect CleanEffect;

	// Token: 0x04000BA0 RID: 2976
	private PhotonView photonView;

	// Token: 0x04000BA1 RID: 2977
	private bool destructionToMaster;

	// Token: 0x04000BA2 RID: 2978
	private bool destructionToOthers;
}
