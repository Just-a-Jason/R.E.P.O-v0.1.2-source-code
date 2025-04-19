using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class PlayerHealthGrab : MonoBehaviour
{
	// Token: 0x06000EF4 RID: 3828 RVA: 0x00088757 File Offset: 0x00086957
	private void Start()
	{
		this.physCollider = base.GetComponent<Collider>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		if (this.playerAvatar.isLocal)
		{
			this.staticGrabObject.enabled = false;
		}
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0008878C File Offset: 0x0008698C
	private void Update()
	{
		if (this.playerAvatar.isTumbling || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
		{
			if (this.hideLerp < 1f)
			{
				this.hideLerp += Time.deltaTime * 5f;
				this.hideLerp = Mathf.Clamp(this.hideLerp, 0f, 1f);
				this.hideTransform.localScale = new Vector3(1f, this.hideCurve.Evaluate(this.hideLerp), 1f);
				if (this.hideLerp >= 1f)
				{
					this.hideTransform.gameObject.SetActive(false);
				}
			}
		}
		else if (this.hideLerp > 0f)
		{
			if (!this.hideTransform.gameObject.activeSelf)
			{
				this.hideTransform.gameObject.SetActive(true);
			}
			this.hideLerp -= Time.deltaTime * 2f;
			this.hideLerp = Mathf.Clamp(this.hideLerp, 0f, 1f);
			this.hideTransform.localScale = new Vector3(1f, this.hideCurve.Evaluate(this.hideLerp), 1f);
		}
		bool flag = true;
		if (this.playerAvatar.isDisabled || this.hideLerp > 0f)
		{
			flag = false;
		}
		if (this.colliderActive != flag)
		{
			this.colliderActive = flag;
			this.physCollider.enabled = this.colliderActive;
		}
		base.transform.position = this.followTransform.position;
		base.transform.rotation = this.followTransform.rotation;
		if (this.colliderActive && (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient))
		{
			if (this.staticGrabObject.playerGrabbing.Count > 0)
			{
				this.grabbingTimer += Time.deltaTime;
				foreach (PhysGrabber physGrabber in this.staticGrabObject.playerGrabbing)
				{
					if (this.grabbingTimer >= 1f)
					{
						PlayerAvatar playerAvatar = physGrabber.playerAvatar;
						if (this.playerAvatar.playerHealth.health != this.playerAvatar.playerHealth.maxHealth && playerAvatar.playerHealth.health > 10)
						{
							this.playerAvatar.playerHealth.HealOther(10, true);
							playerAvatar.playerHealth.HurtOther(10, Vector3.zero, false, -1);
							playerAvatar.HealedOther();
						}
					}
				}
				if (this.grabbingTimer >= 1f)
				{
					this.grabbingTimer = 0f;
					return;
				}
			}
			else
			{
				this.grabbingTimer = 0f;
			}
		}
	}

	// Token: 0x04001929 RID: 6441
	public Transform followTransform;

	// Token: 0x0400192A RID: 6442
	public Transform hideTransform;

	// Token: 0x0400192B RID: 6443
	public PlayerAvatar playerAvatar;

	// Token: 0x0400192C RID: 6444
	internal StaticGrabObject staticGrabObject;

	// Token: 0x0400192D RID: 6445
	private Collider physCollider;

	// Token: 0x0400192E RID: 6446
	private bool colliderActive = true;

	// Token: 0x0400192F RID: 6447
	private float grabbingTimer;

	// Token: 0x04001930 RID: 6448
	[Space]
	public AnimationCurve hideCurve;

	// Token: 0x04001931 RID: 6449
	private float hideLerp;
}
