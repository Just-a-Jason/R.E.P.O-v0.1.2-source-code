using System;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class ItemGrenadeStun : MonoBehaviour
{
	// Token: 0x06000AFE RID: 2814 RVA: 0x00061E88 File Offset: 0x00060088
	private void Start()
	{
		this.stunExplosion = base.GetComponentInChildren<StunExplosion>().transform;
		this.stunExplosion.gameObject.SetActive(false);
		this.itemGrenade = base.GetComponent<ItemGrenade>();
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00061EB8 File Offset: 0x000600B8
	public void Explosion()
	{
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundTinnitus.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameObject gameObject = Object.Instantiate<GameObject>(this.stunExplosion.gameObject, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = null;
		gameObject.SetActive(true);
		gameObject.GetComponent<StunExplosion>().itemGrenade = this.itemGrenade;
	}

	// Token: 0x040011CA RID: 4554
	public Sound soundExplosion;

	// Token: 0x040011CB RID: 4555
	public Sound soundTinnitus;

	// Token: 0x040011CC RID: 4556
	private Transform stunExplosion;

	// Token: 0x040011CD RID: 4557
	private ItemGrenade itemGrenade;
}
