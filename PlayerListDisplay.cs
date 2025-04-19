using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class PlayerListDisplay : MonoBehaviourPunCallbacks
{
	// Token: 0x06000905 RID: 2309 RVA: 0x00056298 File Offset: 0x00054498
	private void Start()
	{
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0005629A File Offset: 0x0005449A
	private void Update()
	{
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0005629C File Offset: 0x0005449C
	[PunRPC]
	private void StartLoadingRPC()
	{
	}

	// Token: 0x04001066 RID: 4198
	public TextMeshProUGUI playerListText;

	// Token: 0x04001067 RID: 4199
	public TextMeshProUGUI instructionText;

	// Token: 0x04001068 RID: 4200
	public TextMeshProUGUI roomNameText;

	// Token: 0x04001069 RID: 4201
	public GameObject loadingUI;

	// Token: 0x0400106A RID: 4202
	private bool loading;

	// Token: 0x0400106B RID: 4203
	public GameObject punVoiceClient;

	// Token: 0x0400106C RID: 4204
	internal int playersCount;

	// Token: 0x0400106D RID: 4205
	private bool voiceInitialized;

	// Token: 0x0400106E RID: 4206
	public MenuPageMain menuPageMain;
}
