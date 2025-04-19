using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class ItemGrenadeDuctTaped : MonoBehaviour
{
	// Token: 0x06000AF1 RID: 2801 RVA: 0x00061B34 File Offset: 0x0005FD34
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00061B50 File Offset: 0x0005FD50
	public void Explosion()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			for (int i = 0; i < 3; i++)
			{
				Vector3 b = new Vector3(0f, 0.2f * (float)i, 0f);
				ItemGrenadeHuman component = Object.Instantiate<GameObject>(this.grenadePrefab, base.transform.position + b, Quaternion.identity).GetComponent<ItemGrenadeHuman>();
				component.Initialize();
				component.Spawn();
			}
		}
		else if (SemiFunc.IsMasterClient())
		{
			for (int j = 0; j < 3; j++)
			{
				Vector3 b2 = new Vector3(0f, 0.2f * (float)j, 0f);
				GameObject gameObject = PhotonNetwork.Instantiate("Items/Item Grenade Human", base.transform.position + b2, Quaternion.identity, 0, null);
				gameObject.GetComponent<ItemGrenadeHuman>().Initialize();
				gameObject.GetComponent<ItemGrenadeHuman>().Spawn();
			}
		}
		this.particleScriptExplosion.Spawn(base.transform.position, 0.8f, 50, 100, 4f, false, true, 1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040011BC RID: 4540
	public GameObject grenadePrefab;

	// Token: 0x040011BD RID: 4541
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040011BE RID: 4542
	private PhotonView photonView;

	// Token: 0x040011BF RID: 4543
	public Sound soundExplosion;

	// Token: 0x040011C0 RID: 4544
	public Sound soundExplosionGlobal;
}
