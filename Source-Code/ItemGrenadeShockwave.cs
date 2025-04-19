using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class ItemGrenadeShockwave : MonoBehaviour
{
	// Token: 0x06000B13 RID: 2835 RVA: 0x00062BB1 File Offset: 0x00060DB1
	public void Explosion()
	{
		Object.Instantiate<GameObject>(this.shockwavePrefab, base.transform.position, Quaternion.identity);
	}

	// Token: 0x040011FC RID: 4604
	public GameObject shockwavePrefab;
}
