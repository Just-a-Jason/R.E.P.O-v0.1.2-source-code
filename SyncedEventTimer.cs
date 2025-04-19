using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200019D RID: 413
public class SyncedEventTimer : MonoBehaviour
{
	// Token: 0x06000DE6 RID: 3558 RVA: 0x0007DFE2 File Offset: 0x0007C1E2
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			this.singlePlayer = true;
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.isMaster = true;
		}
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x0007E014 File Offset: 0x0007C214
	public void StartTimer()
	{
		if (this.singlePlayer || this.isMaster)
		{
			this.timer = Random.Range(this.timerMin, this.timerMax);
			this.onTimerStart.Invoke();
			base.StartCoroutine(this.Timer());
			this.timerActive = true;
			if (this.isMaster)
			{
				this.photonView.RPC("StartTimerRPC", RpcTarget.Others, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x0007E085 File Offset: 0x0007C285
	[PunRPC]
	private void StartTimerRPC()
	{
		this.timerActive = true;
		this.onTimerStart.Invoke();
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0007E099 File Offset: 0x0007C299
	private IEnumerator Timer()
	{
		while (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			yield return null;
		}
		this.EndTimer();
		if (this.isMaster)
		{
			this.photonView.RPC("EndTimerRPC", RpcTarget.Others, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0007E0A8 File Offset: 0x0007C2A8
	private void Update()
	{
		if (this.timerActive)
		{
			this.onTimerTick.Invoke();
		}
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0007E0BD File Offset: 0x0007C2BD
	[PunRPC]
	private void EndTimerRPC()
	{
		this.timerActive = false;
		this.onTimerEnd.Invoke();
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0007E0D1 File Offset: 0x0007C2D1
	public void EndTimer()
	{
		this.timerActive = false;
		this.onTimerEnd.Invoke();
	}

	// Token: 0x040016C7 RID: 5831
	private PhotonView photonView;

	// Token: 0x040016C8 RID: 5832
	private float timer;

	// Token: 0x040016C9 RID: 5833
	public float timerMin = 4f;

	// Token: 0x040016CA RID: 5834
	public float timerMax = 5f;

	// Token: 0x040016CB RID: 5835
	public UnityEvent onTimerStart;

	// Token: 0x040016CC RID: 5836
	public UnityEvent onTimerEnd;

	// Token: 0x040016CD RID: 5837
	public UnityEvent onTimerTick;

	// Token: 0x040016CE RID: 5838
	private bool singlePlayer;

	// Token: 0x040016CF RID: 5839
	private bool isMaster;

	// Token: 0x040016D0 RID: 5840
	private bool timerActive;
}
