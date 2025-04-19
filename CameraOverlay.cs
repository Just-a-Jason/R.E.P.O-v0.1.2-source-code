using System;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class CameraOverlay : MonoBehaviour
{
	// Token: 0x0600132C RID: 4908 RVA: 0x000A7C0A File Offset: 0x000A5E0A
	private void Start()
	{
		this.overlayCamera = base.GetComponent<Camera>();
		CameraOverlay.instance = this;
	}

	// Token: 0x0400208D RID: 8333
	internal Camera overlayCamera;

	// Token: 0x0400208E RID: 8334
	public static CameraOverlay instance;
}
