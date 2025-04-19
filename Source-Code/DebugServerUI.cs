using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class DebugServerUI : MonoBehaviour
{
	// Token: 0x06001220 RID: 4640 RVA: 0x000A043C File Offset: 0x0009E63C
	private void Start()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.Text.text = "Local";
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.Text.text = "Server";
			return;
		}
		this.Text.text = "Client";
	}

	// Token: 0x04001ED1 RID: 7889
	public TextMeshProUGUI Text;
}
