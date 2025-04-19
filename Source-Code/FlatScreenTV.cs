using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class FlatScreenTV : MonoBehaviour
{
	// Token: 0x060008DA RID: 2266 RVA: 0x00054BE5 File Offset: 0x00052DE5
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00054C0C File Offset: 0x00052E0C
	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 8) == 0)
			{
				this.broken = false;
			}
			this.BrokenOrNot();
		}
		yield break;
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00054C1B File Offset: 0x00052E1B
	private void BrokenOrNot()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.BrokenOrNotRPC(this.broken);
			return;
		}
		this.photonView.RPC("BrokenOrNotRPC", RpcTarget.All, new object[]
		{
			this.broken
		});
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00054C58 File Offset: 0x00052E58
	[PunRPC]
	public void BrokenOrNotRPC(bool _broken)
	{
		this.broken = _broken;
		if (this.broken)
		{
			this.jumpScare.gameObject.SetActive(false);
			this.brokenPlane.gameObject.SetActive(true);
			return;
		}
		this.brokenPlane.gameObject.SetActive(false);
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00054CA8 File Offset: 0x00052EA8
	private void Update()
	{
		if (this.timer > 0f)
		{
			if (this.timer < 1.3f && !this.regularHurtCollider.gameObject.activeSelf)
			{
				this.regularHurtCollider.gameObject.SetActive(true);
			}
			this.timer -= Time.deltaTime;
			this.regularPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + Mathf.Sin(this.timer * 100f) * 0.1f, 1f + Mathf.Sin(this.timer * 100f) * 0.1f);
			this.regularPlane.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-Mathf.Sin(this.timer * 100f) * 0.05f, -Mathf.Sin(this.timer * 100f) * 0.05f);
			this.isActive = true;
			return;
		}
		if (this.isActive)
		{
			this.regularPlane.gameObject.SetActive(false);
			this.regularHurtCollider.gameObject.SetActive(false);
			this.broken = true;
			this.BrokenOrNotRPC(this.broken);
		}
		this.isActive = false;
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00054DF1 File Offset: 0x00052FF1
	public void actionTime()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("actionTimeRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.actionTimeRPC();
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00054E20 File Offset: 0x00053020
	[PunRPC]
	public void actionTimeRPC()
	{
		if (this.timer > 0f)
		{
			return;
		}
		this.timer = 1.5f;
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 12f, base.transform.position, 0.1f);
		this.regularPlane.gameObject.SetActive(true);
		this.regularSoundGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.regularSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001025 RID: 4133
	private float timer;

	// Token: 0x04001026 RID: 4134
	public Transform regularPlane;

	// Token: 0x04001027 RID: 4135
	public Sound regularSound;

	// Token: 0x04001028 RID: 4136
	public Sound regularSoundGlobal;

	// Token: 0x04001029 RID: 4137
	private bool isActive;

	// Token: 0x0400102A RID: 4138
	public Transform regularHurtCollider;

	// Token: 0x0400102B RID: 4139
	public Transform visionPoint;

	// Token: 0x0400102C RID: 4140
	private PhotonView photonView;

	// Token: 0x0400102D RID: 4141
	private StaticGrabObject staticGrabObject;

	// Token: 0x0400102E RID: 4142
	private bool broken = true;

	// Token: 0x0400102F RID: 4143
	public Transform jumpScare;

	// Token: 0x04001030 RID: 4144
	public Transform brokenPlane;
}
