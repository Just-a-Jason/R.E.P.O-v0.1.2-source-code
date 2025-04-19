using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class PlayerBattery : MonoBehaviour
{
	// Token: 0x06000E86 RID: 3718 RVA: 0x0008380B File Offset: 0x00081A0B
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00083828 File Offset: 0x00081A28
	private void Update()
	{
		if ((this.isLocal || this.playerAvatar.isLocal) && !this.isLocal)
		{
			base.GetComponent<Collider>().enabled = false;
			base.GetComponent<MeshRenderer>().enabled = false;
			this.isLocal = true;
		}
		base.transform.position = this.batteryPlacement.position;
		base.transform.rotation = this.batteryPlacement.rotation;
		if (PhotonNetwork.IsMasterClient)
		{
			if (this.staticGrabObject.playerGrabbing.Count > 0 && !this.masterCharging)
			{
				this.masterCharging = true;
				this.photonView.RPC("BatteryChargeStart", RpcTarget.All, Array.Empty<object>());
			}
			if (this.staticGrabObject.playerGrabbing.Count <= 0 && this.masterCharging)
			{
				this.masterCharging = false;
				this.photonView.RPC("BatteryChargeEnd", RpcTarget.All, Array.Empty<object>());
			}
		}
		if (this.chargeBattery)
		{
			if (this.chargeTimer < this.chargeRate)
			{
				this.chargeTimer += Time.deltaTime;
				return;
			}
			this.batteryChargeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (PhotonNetwork.IsMasterClient)
			{
				foreach (PhysGrabber physGrabber in this.staticGrabObject.playerGrabbing)
				{
				}
				this.amountPlayersGrabbing = this.staticGrabObject.playerGrabbing.Count;
				if (this.amountPlayersGrabbing != this.amountPlayersGrabbingPrevious)
				{
					this.photonView.RPC("UpdateAmountPlayersGrabbing", RpcTarget.Others, new object[]
					{
						this.amountPlayersGrabbing
					});
					this.amountPlayersGrabbingPrevious = this.amountPlayersGrabbing;
				}
			}
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.EnergyCurrent += 1f * (float)this.amountPlayersGrabbing;
			}
			this.chargeTimer = 0f;
		}
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x00083A44 File Offset: 0x00081C44
	[PunRPC]
	private void UpdateAmountPlayersGrabbing(int amount)
	{
		this.amountPlayersGrabbing = amount;
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x00083A4D File Offset: 0x00081C4D
	[PunRPC]
	private void BatteryChargeStart()
	{
		this.chargeBattery = true;
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x00083A56 File Offset: 0x00081C56
	[PunRPC]
	private void BatteryChargeEnd()
	{
		this.chargeBattery = false;
	}

	// Token: 0x040017FC RID: 6140
	public PlayerAvatar playerAvatar;

	// Token: 0x040017FD RID: 6141
	private PhotonView photonView;

	// Token: 0x040017FE RID: 6142
	private StaticGrabObject staticGrabObject;

	// Token: 0x040017FF RID: 6143
	private bool masterCharging;

	// Token: 0x04001800 RID: 6144
	private bool isLocal;

	// Token: 0x04001801 RID: 6145
	private bool chargeBattery;

	// Token: 0x04001802 RID: 6146
	private float chargeRate = 0.5f;

	// Token: 0x04001803 RID: 6147
	private float chargeTimer;

	// Token: 0x04001804 RID: 6148
	private int amountPlayersGrabbing;

	// Token: 0x04001805 RID: 6149
	private int amountPlayersGrabbingPrevious;

	// Token: 0x04001806 RID: 6150
	public Transform batteryPlacement;

	// Token: 0x04001807 RID: 6151
	public Sound batteryChargeSound;
}
