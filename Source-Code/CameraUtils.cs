using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class CameraUtils : MonoBehaviour
{
	// Token: 0x060000B5 RID: 181 RVA: 0x000070C3 File Offset: 0x000052C3
	private void Awake()
	{
		CameraUtils.Instance = this;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000070CB File Offset: 0x000052CB
	private void Start()
	{
		this.MainCamera = Camera.main;
	}

	// Token: 0x040001D7 RID: 471
	public static CameraUtils Instance;

	// Token: 0x040001D8 RID: 472
	public Camera MainCamera;
}
