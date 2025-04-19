using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class PlayerAvatarTalkAnimation : MonoBehaviour
{
	// Token: 0x06000E70 RID: 3696 RVA: 0x00081E7D File Offset: 0x0008007D
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x00081E8C File Offset: 0x0008008C
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!GameManager.Multiplayer())
		{
			return;
		}
		if (!this.playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (!this.audioSourceFetched)
		{
			this.audioSource = this.playerAvatar.voiceChat.audioSource;
			this.audioSourceFetched = true;
		}
		if (!this.audioSource)
		{
			return;
		}
		float x = 0f;
		if (this.playerAvatar.voiceChat.clipLoudness > 0.005f)
		{
			x = Mathf.Lerp(0f, -this.rotationMaxAngle, this.playerAvatar.voiceChat.clipLoudness * 4f);
		}
		this.objectToRotate.transform.localRotation = Quaternion.Slerp(this.objectToRotate.transform.localRotation, Quaternion.Euler(x, 0f, 0f), 100f * Time.deltaTime);
	}

	// Token: 0x040017B6 RID: 6070
	public AudioSource audioSource;

	// Token: 0x040017B7 RID: 6071
	public PlayerAvatar playerAvatar;

	// Token: 0x040017B8 RID: 6072
	public GameObject objectToRotate;

	// Token: 0x040017B9 RID: 6073
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x040017BA RID: 6074
	[Space]
	public float threshold = 0.01f;

	// Token: 0x040017BB RID: 6075
	public float rotationMaxAngle = 45f;

	// Token: 0x040017BC RID: 6076
	public float amountMultiplier = 1f;

	// Token: 0x040017BD RID: 6077
	private bool audioSourceFetched;
}
