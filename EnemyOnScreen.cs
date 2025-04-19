using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class EnemyOnScreen : MonoBehaviour
{
	// Token: 0x0600061C RID: 1564 RVA: 0x0003B020 File Offset: 0x00039220
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.MainCamera = Camera.main;
		if (this.points.Length == 0)
		{
			this.points = new Transform[1];
			this.points[0] = this.Enemy.CenterTransform;
		}
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0003B080 File Offset: 0x00039280
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0003B09E File Offset: 0x0003929E
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0003B0AD File Offset: 0x000392AD
	private IEnumerator Logic()
	{
		while (this.OnScreenPlayer.Count == 0)
		{
			yield return new WaitForSeconds(this.OnScreenTimer);
		}
		for (;;)
		{
			this.CulledLocal = true;
			this.CulledAny = true;
			this.OnScreenLocal = false;
			this.OnScreenAny = false;
			foreach (Transform transform in this.points)
			{
				if (Vector3.Distance(transform.position, CameraUtils.Instance.MainCamera.transform.position) <= this.maxDistance && SemiFunc.OnScreen(transform.position, this.paddingWidth, this.paddingHeight))
				{
					this.CulledLocal = false;
					this.CulledAny = false;
					Vector3 direction = this.MainCamera.transform.position - transform.position;
					float num = Mathf.Min(Vector3.Distance(this.MainCamera.transform.position, transform.position), 12f);
					RaycastHit raycastHit;
					if (!Physics.Raycast(transform.position, direction, out raycastHit, num, this.Enemy.VisionMask) || raycastHit.transform.CompareTag("Player") || raycastHit.transform.GetComponent<PlayerTumble>())
					{
						this.OnScreenLocal = true;
						this.OnScreenAny = true;
					}
				}
				if (this.OnScreenAny && !this.CulledAny)
				{
					break;
				}
			}
			if (GameManager.Multiplayer())
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled && playerAvatar.photonView.IsMine)
					{
						if (this.CulledLocal != this.CulledLocalPrevious || this.OnScreenLocal != this.OnScreenLocalPrevious)
						{
							this.CulledLocalPrevious = this.CulledLocal;
							this.OnScreenLocalPrevious = this.OnScreenLocal;
							this.OnScreenPlayerUpdate(playerAvatar.photonView.ViewID, this.OnScreenLocal, this.CulledLocal);
							break;
						}
						break;
					}
				}
				foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar2.isDisabled)
					{
						if (this.OnScreenPlayer[playerAvatar2.photonView.ViewID])
						{
							this.OnScreenAny = true;
						}
						if (!this.CulledPlayer[playerAvatar2.photonView.ViewID])
						{
							this.CulledAny = false;
						}
					}
				}
			}
			yield return new WaitForSeconds(this.OnScreenTimer);
		}
		yield break;
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0003B0BC File Offset: 0x000392BC
	public bool GetOnScreen(PlayerAvatar _playerAvatar)
	{
		if (!GameManager.Multiplayer())
		{
			return this.OnScreenLocal;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar == _playerAvatar && this.OnScreenPlayer[playerAvatar.photonView.ViewID])
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0003B144 File Offset: 0x00039344
	private void OnScreenPlayerUpdate(int playerID, bool onScreen, bool culled)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.OnScreenPlayerUpdateRPC(playerID, onScreen, culled);
			return;
		}
		this.Enemy.PhotonView.RPC("OnScreenPlayerUpdateRPC", RpcTarget.All, new object[]
		{
			playerID,
			onScreen,
			culled
		});
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0003B19E File Offset: 0x0003939E
	[PunRPC]
	private void OnScreenPlayerUpdateRPC(int playerID, bool onScreen, bool culled)
	{
		this.CulledPlayer[playerID] = culled;
		this.OnScreenPlayer[playerID] = onScreen;
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0003B1BA File Offset: 0x000393BA
	public void PlayerAdded(int photonID)
	{
		this.OnScreenPlayer.TryAdd(photonID, false);
		this.CulledPlayer.TryAdd(photonID, false);
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0003B1D8 File Offset: 0x000393D8
	public void PlayerRemoved(int photonID)
	{
		this.OnScreenPlayer.Remove(photonID);
		this.CulledPlayer.Remove(photonID);
	}

	// Token: 0x04000A0A RID: 2570
	private Enemy Enemy;

	// Token: 0x04000A0B RID: 2571
	private Camera MainCamera;

	// Token: 0x04000A0C RID: 2572
	public Transform[] points;

	// Token: 0x04000A0D RID: 2573
	[Space]
	public float maxDistance = 20f;

	// Token: 0x04000A0E RID: 2574
	[Space]
	public float paddingWidth = 0.1f;

	// Token: 0x04000A0F RID: 2575
	public float paddingHeight = 0.1f;

	// Token: 0x04000A10 RID: 2576
	private bool LogicActive;

	// Token: 0x04000A11 RID: 2577
	private float OnScreenTimer = 0.25f;

	// Token: 0x04000A12 RID: 2578
	internal bool OnScreenLocal;

	// Token: 0x04000A13 RID: 2579
	private bool OnScreenLocalPrevious;

	// Token: 0x04000A14 RID: 2580
	internal bool CulledLocal;

	// Token: 0x04000A15 RID: 2581
	private bool CulledLocalPrevious;

	// Token: 0x04000A16 RID: 2582
	internal bool OnScreenAny;

	// Token: 0x04000A17 RID: 2583
	internal bool CulledAny;

	// Token: 0x04000A18 RID: 2584
	internal Dictionary<int, bool> OnScreenPlayer = new Dictionary<int, bool>();

	// Token: 0x04000A19 RID: 2585
	internal Dictionary<int, bool> CulledPlayer = new Dictionary<int, bool>();
}
