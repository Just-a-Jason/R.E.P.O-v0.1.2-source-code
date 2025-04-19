using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class DebugJannek : MonoBehaviour
{
	// Token: 0x06000F6A RID: 3946 RVA: 0x0008D428 File Offset: 0x0008B628
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0008D448 File Offset: 0x0008B648
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated && SpectateCamera.instance && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			return;
		}
		if (!PlayerController.instance.playerAvatarScript || PlayerController.instance.playerAvatarScript.deadSet)
		{
			return;
		}
		base.transform.position = Camera.main.transform.position;
		base.transform.rotation = Camera.main.transform.rotation;
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				this.hurtCollider.gameObject.SetActive(true);
				this.hurtColliderTimer = 0.2f;
			}
			if (this.hurtColliderTimer > 0f)
			{
				this.hurtColliderTimer -= Time.deltaTime;
				if (this.hurtColliderTimer <= 0f)
				{
					this.hurtCollider.gameObject.SetActive(false);
				}
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				PlayerController.instance.playerAvatarScript.playerHealth.Heal(30, true);
			}
		}
	}

	// Token: 0x04001A2E RID: 6702
	private HurtCollider hurtCollider;

	// Token: 0x04001A2F RID: 6703
	private float hurtColliderTimer;

	// Token: 0x04001A30 RID: 6704
	private Transform playerTransform;
}
