using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class TruckScreenOpen : MonoBehaviour
{
	// Token: 0x06000853 RID: 2131 RVA: 0x000500B4 File Offset: 0x0004E2B4
	private void Start()
	{
		this.openScreenYPosOriginal = base.transform.localPosition.y;
		this.doorParticles = base.GetComponentInChildren<ParticleSystem>();
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.doorOpenPosition, base.transform.localPosition.z);
		base.StartCoroutine(this.DelayedClose());
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0005013C File Offset: 0x0004E33C
	private void TruckScreenOpenStartLogic()
	{
		this.openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.2f);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal, base.transform.localPosition.z);
		this.openScreenCurveTimer = 0f;
		this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.doorLoopPlaying = true;
		this.doorDone = false;
		this.doorParticles.Play();
		this.doorClose = false;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0005020A File Offset: 0x0004E40A
	public void TruckScreenOpenStart()
	{
		if (GameManager.Multiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("TruckScreenOpenStartRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.TruckScreenOpenStartLogic();
		}
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00050237 File Offset: 0x0004E437
	[PunRPC]
	private void TruckScreenOpenStartRPC()
	{
		this.TruckScreenOpenStartLogic();
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00050240 File Offset: 0x0004E440
	private void TruckScreenCloseStart()
	{
		this.openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.2f);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.doorOpenPosition, base.transform.localPosition.z);
		this.openScreenCurveTimer = 0f;
		this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.doorLoopPlaying = true;
		this.doorDone = false;
		this.doorParticles.Play();
		this.doorClose = true;
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00050315 File Offset: 0x0004E515
	private IEnumerator DelayedClose()
	{
		yield return new WaitForSeconds(2f);
		this.TruckScreenCloseStart();
		yield break;
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x00050324 File Offset: 0x0004E524
	private IEnumerator DelayedLevelSwitch()
	{
		yield return new WaitForSeconds(2f);
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
		yield break;
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0005032C File Offset: 0x0004E52C
	private void Update()
	{
		this.doorLoop.PlayLoop(this.doorLoopPlaying, 2f, 2f, 1f);
		if (!this.openScreenActive)
		{
			return;
		}
		if (this.openScreenCurveTimer < 1f)
		{
			this.openScreenCurveTimer += Time.deltaTime;
			float time = this.openScreenCurveTimer;
			if (this.doorClose)
			{
				time = 1f - this.openScreenCurveTimer;
			}
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.openScreenYPosOriginal + this.openScreenCurve.Evaluate(time) * this.doorOpenPosition, base.transform.localPosition.z);
			if (this.openScreenCurveTimer > 0.8f && !this.doorDone)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.1f);
				this.doorDone = true;
				this.doorLoopEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.doorSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.doorLoopPlaying = false;
				this.doorParticles.Play();
				if (!this.doorClose)
				{
					if (!GameManager.Multiplayer())
					{
						base.StartCoroutine(this.DelayedLevelSwitch());
						return;
					}
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						base.StartCoroutine(this.DelayedLevelSwitch());
						return;
					}
				}
			}
		}
		else
		{
			this.openScreenActive = false;
		}
	}

	// Token: 0x04000F49 RID: 3913
	public AnimationCurve openScreenCurve;

	// Token: 0x04000F4A RID: 3914
	private float openScreenCurveTimer;

	// Token: 0x04000F4B RID: 3915
	private float openScreenYPosOriginal;

	// Token: 0x04000F4C RID: 3916
	private bool openScreenActive;

	// Token: 0x04000F4D RID: 3917
	private bool doorDone;

	// Token: 0x04000F4E RID: 3918
	private bool doorLoopPlaying;

	// Token: 0x04000F4F RID: 3919
	public Sound doorLoop;

	// Token: 0x04000F50 RID: 3920
	public Sound doorLoopStart;

	// Token: 0x04000F51 RID: 3921
	public Sound doorLoopEnd;

	// Token: 0x04000F52 RID: 3922
	public Sound doorSound;

	// Token: 0x04000F53 RID: 3923
	private ParticleSystem doorParticles;

	// Token: 0x04000F54 RID: 3924
	private bool doorClose;

	// Token: 0x04000F55 RID: 3925
	private float doorOpenPosition = 4.13f;

	// Token: 0x04000F56 RID: 3926
	private PhotonView photonView;
}
