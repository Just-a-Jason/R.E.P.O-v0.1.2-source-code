using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class ItemGrenadeHuman : MonoBehaviour
{
	// Token: 0x06000AF4 RID: 2804 RVA: 0x00061CA9 File Offset: 0x0005FEA9
	private void Start()
	{
		this.Initialize();
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00061CB4 File Offset: 0x0005FEB4
	public void Initialize()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemGrenade = base.GetComponent<ItemGrenade>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00061D09 File Offset: 0x0005FF09
	public void Spawn()
	{
		base.StartCoroutine(this.LateSpawn());
		this.itemGrenade.isSpawnedGrenade = true;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00061D24 File Offset: 0x0005FF24
	private IEnumerator LateSpawn()
	{
		while (!this.physGrabObject.spawned || this.rb.isKinematic)
		{
			yield return null;
		}
		this.itemToggle.ToggleItem(true, -1);
		this.itemGrenade.tickTime = Random.Range(1.5f, 3f);
		Vector3 a = Quaternion.Euler((float)Random.Range(-45, 45), (float)Random.Range(-180, 180), 0f) * Vector3.forward;
		this.rb.AddForce(a * (float)Random.Range(5, 10), ForceMode.Impulse);
		this.rb.AddTorque(Random.insideUnitSphere * Random.Range(5f, 10f), ForceMode.Impulse);
		this.itemGrenade.isSpawnedGrenade = true;
		yield break;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00061D34 File Offset: 0x0005FF34
	public void Explosion()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.particleScriptExplosion.Spawn(base.transform.position, 0.8f, 50, 100, 2f, false, true, 1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040011C1 RID: 4545
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040011C2 RID: 4546
	private ItemToggle itemToggle;

	// Token: 0x040011C3 RID: 4547
	private ItemGrenade itemGrenade;

	// Token: 0x040011C4 RID: 4548
	private PhotonView photonView;

	// Token: 0x040011C5 RID: 4549
	private PhysGrabObject physGrabObject;

	// Token: 0x040011C6 RID: 4550
	private Rigidbody rb;

	// Token: 0x040011C7 RID: 4551
	public Sound soundExplosion;

	// Token: 0x040011C8 RID: 4552
	public Sound soundExplosionGlobal;
}
