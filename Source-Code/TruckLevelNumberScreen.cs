using System;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class TruckLevelNumberScreen : MonoBehaviour
{
	// Token: 0x06000851 RID: 2129 RVA: 0x00050083 File Offset: 0x0004E283
	private void Start()
	{
		this.arenaPedistalScreen = base.GetComponent<ArenaPedistalScreen>();
		this.arenaPedistalScreen.SwitchNumber(RunManager.instance.levelsCompleted + 1, false);
	}

	// Token: 0x04000F48 RID: 3912
	private ArenaPedistalScreen arenaPedistalScreen;
}
