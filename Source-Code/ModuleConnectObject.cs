using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class ModuleConnectObject : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000807 RID: 2055 RVA: 0x0004E35D File Offset: 0x0004C55D
	private void Start()
	{
		base.StartCoroutine(this.ConnectingCheck());
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0004E36C File Offset: 0x0004C56C
	private IEnumerator ConnectingCheck()
	{
		while (!this.MasterSetup)
		{
			yield return new WaitForSeconds(0.1f);
		}
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
		yield break;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0004E37C File Offset: 0x0004C57C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.ModuleConnecting);
			stream.SendNext(this.MasterSetup);
			return;
		}
		this.ModuleConnecting = (bool)stream.ReceiveNext();
		this.MasterSetup = (bool)stream.ReceiveNext();
	}

	// Token: 0x04000ECE RID: 3790
	public bool ModuleConnecting;

	// Token: 0x04000ECF RID: 3791
	public bool MasterSetup;
}
