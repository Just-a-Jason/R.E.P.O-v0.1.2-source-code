using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class DebugRobin : MonoBehaviour
{
	// Token: 0x06000F6D RID: 3949 RVA: 0x0008D574 File Offset: 0x0008B774
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F6))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableMedium, base.transform.position + base.transform.forward * 2f, "Valuables/");
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

	// Token: 0x06000F6E RID: 3950 RVA: 0x0008D60E File Offset: 0x0008B80E
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

	// Token: 0x04001A31 RID: 6705
	private Transform playerTransform;
}
