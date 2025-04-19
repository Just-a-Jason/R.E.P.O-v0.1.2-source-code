using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class TruckSafetySpawnPoint : MonoBehaviour
{
	// Token: 0x060008B2 RID: 2226 RVA: 0x00053963 File Offset: 0x00051B63
	private void Awake()
	{
		TruckSafetySpawnPoint.instance = this;
	}

	// Token: 0x04000FD1 RID: 4049
	public static TruckSafetySpawnPoint instance;
}
