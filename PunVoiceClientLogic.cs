using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class PunVoiceClientLogic : MonoBehaviour
{
	// Token: 0x06000A83 RID: 2691 RVA: 0x0005CD6C File Offset: 0x0005AF6C
	private void Awake()
	{
		Debug.Log("PunVoiceClientLogic Awake");
		if (!PunVoiceClientLogic.instance)
		{
			PunVoiceClientLogic.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (PunVoiceClientLogic.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400110E RID: 4366
	public static PunVoiceClientLogic instance;
}
