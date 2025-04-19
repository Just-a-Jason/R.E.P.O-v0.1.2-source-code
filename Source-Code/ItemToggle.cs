using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class ItemToggle : MonoBehaviour
{
	// Token: 0x06000AE9 RID: 2793 RVA: 0x00061900 File Offset: 0x0005FB00
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x00061928 File Offset: 0x0005FB28
	private void Update()
	{
		if (this.autoTurnOffWhenEquipped && this.itemEquippable && this.itemEquippable.isEquipped && this.toggleState)
		{
			this.ToggleItem(false, -1);
		}
		if (this.playSound && !this.fetchSound)
		{
			this.soundOn = AssetManager.instance.soundDeviceTurnOn;
			this.soundOff = AssetManager.instance.soundDeviceTurnOff;
			this.fetchSound = true;
		}
		if (this.physGrabObject.heldByLocalPlayer && !this.disabled && SemiFunc.InputDown(InputKey.Interact))
		{
			TutorialDirector.instance.playerUsedToggle = true;
			bool toggle = !this.toggleState;
			int player = SemiFunc.PhotonViewIDPlayerAvatarLocal();
			this.ToggleItem(toggle, player);
		}
		if (this.toggleImpulseTimer > 0f)
		{
			this.toggleImpulse = true;
			this.toggleImpulseTimer -= Time.deltaTime;
			return;
		}
		this.toggleImpulse = false;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00061A0C File Offset: 0x0005FC0C
	private void ToggleItemLogic(bool toggle, int player = -1)
	{
		this.toggleStatePrevious = this.toggleState;
		this.toggleState = toggle;
		this.playerTogglePhotonID = player;
		if (this.playSound)
		{
			if (this.toggleState)
			{
				this.soundOn.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.soundOff.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		this.toggleImpulseTimer = 0.2f;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x00061AA6 File Offset: 0x0005FCA6
	public void ToggleItem(bool toggle, int player = -1)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("ToggleItemRPC", RpcTarget.All, new object[]
			{
				toggle,
				player
			});
			return;
		}
		this.ToggleItemLogic(toggle, player);
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x00061AE1 File Offset: 0x0005FCE1
	[PunRPC]
	private void ToggleItemRPC(bool toggle, int player = -1)
	{
		this.ToggleItemLogic(toggle, player);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x00061AEB File Offset: 0x0005FCEB
	public void ToggleDisable(bool _disable)
	{
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("ToggleDisableRPC", RpcTarget.All, new object[]
			{
				_disable
			});
			return;
		}
		this.ToggleDisableRPC(_disable);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x00061B1C File Offset: 0x0005FD1C
	[PunRPC]
	private void ToggleDisableRPC(bool _disable)
	{
		this.disabled = _disable;
	}

	// Token: 0x040011AE RID: 4526
	[HideInInspector]
	public bool toggleState;

	// Token: 0x040011AF RID: 4527
	public bool playSound;

	// Token: 0x040011B0 RID: 4528
	private bool fetchSound;

	// Token: 0x040011B1 RID: 4529
	internal bool toggleStatePrevious;

	// Token: 0x040011B2 RID: 4530
	private PhotonView photonView;

	// Token: 0x040011B3 RID: 4531
	private PhysGrabObject physGrabObject;

	// Token: 0x040011B4 RID: 4532
	private ItemEquippable itemEquippable;

	// Token: 0x040011B5 RID: 4533
	private Sound soundOn;

	// Token: 0x040011B6 RID: 4534
	private Sound soundOff;

	// Token: 0x040011B7 RID: 4535
	internal int playerTogglePhotonID;

	// Token: 0x040011B8 RID: 4536
	internal bool toggleImpulse;

	// Token: 0x040011B9 RID: 4537
	private float toggleImpulseTimer;

	// Token: 0x040011BA RID: 4538
	internal bool disabled;

	// Token: 0x040011BB RID: 4539
	public bool autoTurnOffWhenEquipped = true;
}
