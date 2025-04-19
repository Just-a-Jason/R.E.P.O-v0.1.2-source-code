using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class DebugAxel : MonoBehaviour
{
	// Token: 0x06000F59 RID: 3929 RVA: 0x0008CB7D File Offset: 0x0008AD7D
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0008CBA0 File Offset: 0x0008ADA0
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F7))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableSmall, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 999f);
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				playerAvatar.playerDeathHead.inExtractionPoint = true;
				playerAvatar.playerDeathHead.Revive();
			}
		}
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
			PlayerController.instance.playerAvatarScript.playerHealth.Hurt(10, true, -1);
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(10, true);
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0008CD98 File Offset: 0x0008AF98
	private void SpawnObject(GameObject _object, Vector3 _position, string _path)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(_object, _position, Quaternion.identity);
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			PhotonNetwork.InstantiateRoomObject(_path + _object.name, _position, Quaternion.identity, 0, null);
		}
	}

	// Token: 0x040019F3 RID: 6643
	private HurtCollider hurtCollider;

	// Token: 0x040019F4 RID: 6644
	private float hurtColliderTimer;

	// Token: 0x040019F5 RID: 6645
	private Transform playerTransform;

	// Token: 0x040019F6 RID: 6646
	public Sound sound;
}
