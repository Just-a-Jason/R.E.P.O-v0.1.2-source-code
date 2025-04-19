using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class ToolFollowPush : MonoBehaviour
{
	// Token: 0x0600073C RID: 1852 RVA: 0x00044AC2 File Offset: 0x00042CC2
	public void Push(Vector3 position, Quaternion rotation, float amount)
	{
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, position, amount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, rotation, amount);
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00044B00 File Offset: 0x00042D00
	private void Update()
	{
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Vector3.zero, this.SettleSpeed * Time.deltaTime);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.identity, this.SettleSpeed * Time.deltaTime);
	}

	// Token: 0x04000CA5 RID: 3237
	private Vector3 PushPosition;

	// Token: 0x04000CA6 RID: 3238
	private Quaternion PushRotation;

	// Token: 0x04000CA7 RID: 3239
	public float SettleSpeed;
}
