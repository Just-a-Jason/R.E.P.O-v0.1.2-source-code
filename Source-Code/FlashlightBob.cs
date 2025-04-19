using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class FlashlightBob : MonoBehaviour
{
	// Token: 0x060006A7 RID: 1703 RVA: 0x0003FD38 File Offset: 0x0003DF38
	private void Update()
	{
		if (!this.PlayerAvatar.isLocal)
		{
			return;
		}
		Vector3 positionResult = CameraBob.Instance.positionResult;
		base.transform.localPosition = new Vector3(-positionResult.y * 0.2f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, positionResult.y * 30f, 0f);
	}

	// Token: 0x04000B2F RID: 2863
	public PlayerAvatar PlayerAvatar;
}
