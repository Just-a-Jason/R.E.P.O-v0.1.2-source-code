using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class CameraFreeze : MonoBehaviour
{
	// Token: 0x0600008A RID: 138 RVA: 0x00005C1C File Offset: 0x00003E1C
	private void Awake()
	{
		CameraFreeze.instance = this;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00005C24 File Offset: 0x00003E24
	private void Update()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				foreach (Camera camera in this.cameras)
				{
					camera.enabled = true;
				}
			}
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00005CA4 File Offset: 0x00003EA4
	public static void Freeze(float _time)
	{
		if (_time <= 0f)
		{
			foreach (Camera camera in CameraFreeze.instance.cameras)
			{
				camera.enabled = true;
			}
			CameraFreeze.instance.timer = _time;
			return;
		}
		if (CameraFreeze.instance.timer <= 0f)
		{
			foreach (Camera camera2 in CameraFreeze.instance.cameras)
			{
				camera2.enabled = false;
			}
		}
		CameraFreeze.instance.timer = _time;
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00005D70 File Offset: 0x00003F70
	public static bool IsFrozen()
	{
		return CameraFreeze.instance.timer > 0f;
	}

	// Token: 0x04000171 RID: 369
	public static CameraFreeze instance;

	// Token: 0x04000172 RID: 370
	public List<Camera> cameras = new List<Camera>();

	// Token: 0x04000173 RID: 371
	private float timer;
}
