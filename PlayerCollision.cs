using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class PlayerCollision : MonoBehaviour
{
	// Token: 0x06000E8C RID: 3724 RVA: 0x00083A72 File Offset: 0x00081C72
	private void Awake()
	{
		PlayerCollision.instance = this;
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00083A7C File Offset: 0x00081C7C
	private void Update()
	{
		if (this.Player.Crouching && CameraCrouchPosition.instance.Active && CameraCrouchPosition.instance.Lerp > 0.5f)
		{
			base.transform.localScale = this.CrouchCollision.localScale;
			return;
		}
		base.transform.localScale = this.StandCollision.localScale;
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00083AE0 File Offset: 0x00081CE0
	public void SetCrouchCollision()
	{
		base.transform.localScale = this.CrouchCollision.localScale;
	}

	// Token: 0x04001808 RID: 6152
	public static PlayerCollision instance;

	// Token: 0x04001809 RID: 6153
	public PlayerController Player;

	// Token: 0x0400180A RID: 6154
	public Transform StandCollision;

	// Token: 0x0400180B RID: 6155
	public Transform CrouchCollision;
}
