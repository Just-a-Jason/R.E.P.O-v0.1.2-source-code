using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002AF RID: 687
	public class AudioTester : MonoBehaviour
	{
		// Token: 0x06001543 RID: 5443 RVA: 0x000B43D8 File Offset: 0x000B25D8
		public void ClearBuffer()
		{
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x000B43DA File Offset: 0x000B25DA
		public void SetRunEffectInEditMode()
		{
		}

		// Token: 0x1700004C RID: 76
		// (set) Token: 0x06001545 RID: 5445 RVA: 0x000B43DC File Offset: 0x000B25DC
		public bool playAudio
		{
			set
			{
				base.gameObject.SendMessage("ClearBuffer");
				base.gameObject.SendMessage("ResetUtils", SendMessageOptions.DontRequireReceiver);
				if (this.hasAudioSource && this.audioSource.clip != null)
				{
					this.audioSource.Play();
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (set) Token: 0x06001546 RID: 5446 RVA: 0x000B4430 File Offset: 0x000B2630
		public bool stopAudio
		{
			set
			{
				base.gameObject.SendMessage("ClearBuffer");
				if (this.hasAudioSource)
				{
					this.audioSource.Stop();
				}
			}
		}

		// Token: 0x0400234F RID: 9039
		[HideInInspector]
		public bool hasAudioSource = true;

		// Token: 0x04002350 RID: 9040
		[HideInInspector]
		public AudioSource audioSource;
	}
}
