using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class DebugRuben : MonoBehaviour
{
	// Token: 0x06000F70 RID: 3952 RVA: 0x0008D64E File Offset: 0x0008B84E
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0008D670 File Offset: 0x0008B870
	private void Update()
	{
		if (SemiFunc.KeyDownRuben(KeyCode.F6))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableSmall, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben(KeyCode.F7))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableBig, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben(KeyCode.F5))
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 999f);
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
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(75, true);
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0008D834 File Offset: 0x0008BA34
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

	// Token: 0x04001A32 RID: 6706
	private PhotonView photonView;

	// Token: 0x04001A33 RID: 6707
	private HurtCollider hurtCollider;

	// Token: 0x04001A34 RID: 6708
	private float hurtColliderTimer;

	// Token: 0x04001A35 RID: 6709
	private Transform playerTransform;

	// Token: 0x04001A36 RID: 6710
	public List<GameObject> spawnObjects;
}
