using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class SquirtCode : MonoBehaviour
{
	// Token: 0x060013A0 RID: 5024 RVA: 0x000AB89C File Offset: 0x000A9A9C
	private void Start()
	{
		this.SquirtPlane1OriginalScale = this.SquirtPlane1.localScale;
		this.SquirtPlane2OriginalScale = this.SquirtPlane2.localScale;
		this.mainOriginalScale = base.transform.localScale;
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x000AB8D4 File Offset: 0x000A9AD4
	private void Update()
	{
		base.transform.Rotate(Vector3.right * Time.deltaTime * 800f);
		this.SquirtPlane1.localScale = new Vector3(this.SquirtPlane1OriginalScale.x, this.SquirtPlane1OriginalScale.y, this.SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f) * 0.15f);
		this.SquirtPlane2.localScale = new Vector3(this.SquirtPlane1OriginalScale.x, this.SquirtPlane1OriginalScale.y, this.SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f + 50f) * 0.15f);
		base.transform.localScale = new Vector3(this.mainOriginalScale.x + Mathf.Sin(Time.time * 50f) * 0.15f, this.mainOriginalScale.y, this.mainOriginalScale.z);
	}

	// Token: 0x0400217B RID: 8571
	public Transform SquirtPlane1;

	// Token: 0x0400217C RID: 8572
	public Transform SquirtPlane2;

	// Token: 0x0400217D RID: 8573
	private Vector3 SquirtPlane1OriginalScale;

	// Token: 0x0400217E RID: 8574
	private Vector3 SquirtPlane2OriginalScale;

	// Token: 0x0400217F RID: 8575
	private Vector3 mainOriginalScale;
}
