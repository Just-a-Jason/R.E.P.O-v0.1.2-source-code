using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class ItemGrenadeExplosive : MonoBehaviour
{
	// Token: 0x06000AFA RID: 2810 RVA: 0x00061DD8 File Offset: 0x0005FFD8
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		if (SemiFunc.RunIsShop() && SemiFunc.IsMasterClientOrSingleplayer())
		{
			ItemToggle component = base.GetComponent<ItemToggle>();
			if (ShopManager.instance.isThief)
			{
				base.StartCoroutine(this.ThiefLaunch());
				component.ToggleItem(true, -1);
				base.GetComponent<ItemGrenade>().isSpawnedGrenade = true;
			}
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00061E33 File Offset: 0x00060033
	private IEnumerator ThiefLaunch()
	{
		yield return new WaitForSeconds(0.2f);
		Rigidbody component = base.GetComponent<Rigidbody>();
		Vector3 a = ShopManager.instance.extractionPoint.forward;
		a += Vector3.up * Random.Range(0.1f, 0.5f);
		a += ShopManager.instance.extractionPoint.right * Random.Range(-0.5f, 0.5f);
		component.AddForce(a * (float)Random.Range(3, 7), ForceMode.Impulse);
		yield break;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00061E44 File Offset: 0x00060044
	public void Explosion()
	{
		this.particleScriptExplosion.Spawn(base.transform.position, 1.2f, 75, 160, 4f, false, false, 1f);
	}

	// Token: 0x040011C9 RID: 4553
	private ParticleScriptExplosion particleScriptExplosion;
}
