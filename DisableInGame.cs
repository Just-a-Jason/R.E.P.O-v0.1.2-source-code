using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class DisableInGame : MonoBehaviour
{
	// Token: 0x06001141 RID: 4417 RVA: 0x0009A155 File Offset: 0x00098355
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
