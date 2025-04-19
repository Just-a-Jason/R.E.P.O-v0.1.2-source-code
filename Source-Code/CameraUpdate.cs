using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class CameraUpdate : MonoBehaviour
{
	// Token: 0x060000B3 RID: 179 RVA: 0x00006FE8 File Offset: 0x000051E8
	private void Update()
	{
		if (this.updateTimer <= -this.updateRate)
		{
			foreach (Camera camera in this.cams)
			{
				camera.enabled = true;
			}
			this.updateTimer += this.updateRate;
			return;
		}
		foreach (Camera camera2 in this.cams)
		{
			camera2.enabled = false;
		}
		this.updateTimer -= Time.deltaTime;
	}

	// Token: 0x040001D4 RID: 468
	public float updateRate = 0.5f;

	// Token: 0x040001D5 RID: 469
	private float updateTimer;

	// Token: 0x040001D6 RID: 470
	public List<Camera> cams;
}
