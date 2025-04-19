using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A1 RID: 673
	public class AudioEffectFader : MonoBehaviour
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060014C8 RID: 5320 RVA: 0x000B2D50 File Offset: 0x000B0F50
		// (set) Token: 0x060014C9 RID: 5321 RVA: 0x000B2D58 File Offset: 0x000B0F58
		public float Gain
		{
			get
			{
				return this._gain;
			}
			set
			{
				this._gain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x000B2D70 File Offset: 0x000B0F70
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Mute)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = 0f;
				}
				return;
			}
			for (int j = 0; j < data.Length; j++)
			{
				data[j] *= this.Gain;
			}
		}

		// Token: 0x040022F3 RID: 8947
		[SerializeField]
		private float _gain = 1f;

		// Token: 0x040022F4 RID: 8948
		public bool Mute;

		// Token: 0x040022F5 RID: 8949
		private LFO lfo = new LFO();
	}
}
