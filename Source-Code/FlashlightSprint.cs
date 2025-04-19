using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class FlashlightSprint : MonoBehaviour
{
	// Token: 0x060006B8 RID: 1720 RVA: 0x0004092C File Offset: 0x0003EB2C
	private void Update()
	{
		if (!this.PlayerAvatar.isLocal)
		{
			return;
		}
		if (PlayerController.instance.CanSlide)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f, this.Offset * GameplayManager.instance.cameraAnimation), this.Speed * Time.deltaTime);
			return;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f, 0f), this.Speed * Time.deltaTime);
	}

	// Token: 0x04000B66 RID: 2918
	public float Offset;

	// Token: 0x04000B67 RID: 2919
	public float Speed;

	// Token: 0x04000B68 RID: 2920
	public PlayerAvatar PlayerAvatar;
}
