using System;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class MenuHolder : MonoBehaviour
{
	// Token: 0x06001008 RID: 4104 RVA: 0x00091D00 File Offset: 0x0008FF00
	private void Start()
	{
		MenuHolder.instance = this;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00091D08 File Offset: 0x0008FF08
	private void Update()
	{
	}

	// Token: 0x04001AE4 RID: 6884
	public static MenuHolder instance;
}
