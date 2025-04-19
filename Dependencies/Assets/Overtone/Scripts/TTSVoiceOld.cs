using System;
using LeastSquares.Overtone;
using UnityEngine;

namespace Assets.Overtone.Scripts
{
	// Token: 0x02000293 RID: 659
	public class TTSVoiceOld : MonoBehaviour
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x000B16B2 File Offset: 0x000AF8B2
		// (set) Token: 0x06001440 RID: 5184 RVA: 0x000B16BA File Offset: 0x000AF8BA
		public TTSVoiceNative VoiceModel { get; private set; }

		// Token: 0x06001441 RID: 5185 RVA: 0x000B16C4 File Offset: 0x000AF8C4
		private void Update()
		{
			if (this.voiceName != this.oldVoiceName)
			{
				this.oldVoiceName = this.voiceName;
				this.VoiceModel = TTSVoiceNative.LoadVoiceFromResources(this.voiceName);
			}
			if (this.speakerId != this.oldSpeakerId)
			{
				this.oldSpeakerId = this.speakerId;
				this.VoiceModel.SetSpeakerId(this.speakerId);
			}
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x000B172C File Offset: 0x000AF92C
		private void OnDestroy()
		{
			TTSVoiceNative voiceModel = this.VoiceModel;
			if (voiceModel == null)
			{
				return;
			}
			voiceModel.Dispose();
		}

		// Token: 0x040022A4 RID: 8868
		public string voiceName;

		// Token: 0x040022A5 RID: 8869
		public int speakerId;

		// Token: 0x040022A6 RID: 8870
		private string oldVoiceName;

		// Token: 0x040022A7 RID: 8871
		private int oldSpeakerId;
	}
}
