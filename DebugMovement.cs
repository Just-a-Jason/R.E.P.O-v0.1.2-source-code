using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class DebugMovement : MonoBehaviour
{
	// Token: 0x06000F67 RID: 3943 RVA: 0x0008D384 File Offset: 0x0008B584
	private void Start()
	{
		this.startPos = base.transform.position;
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0008D398 File Offset: 0x0008B598
	private void Update()
	{
		base.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.speed), Mathf.Cos(Time.time * 0.5f * this.speed), Mathf.Cos(Time.time * 0.25f * this.speed));
	}

	// Token: 0x04001A2A RID: 6698
	public float speed = 1f;

	// Token: 0x04001A2B RID: 6699
	public float leftRight = 1f;

	// Token: 0x04001A2C RID: 6700
	public float upDown = 1f;

	// Token: 0x04001A2D RID: 6701
	private Vector3 startPos;
}
