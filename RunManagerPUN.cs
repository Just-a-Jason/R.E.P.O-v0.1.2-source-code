using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class RunManagerPUN : MonoBehaviour
{
	// Token: 0x06000983 RID: 2435 RVA: 0x000583C6 File Offset: 0x000565C6
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.runManager = RunManager.instance;
		this.runManager.runManagerPUN = this;
		this.runManager.restarting = false;
		this.runManager.restartingDone = false;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00058403 File Offset: 0x00056603
	[PunRPC]
	private void UpdateLevelRPC(string _levelName, int _levelsCompleted, bool _gameOver)
	{
		this.runManager.UpdateLevel(_levelName, _levelsCompleted, _gameOver);
	}

	// Token: 0x040010F5 RID: 4341
	internal PhotonView photonView;

	// Token: 0x040010F6 RID: 4342
	private RunManager runManager;
}
