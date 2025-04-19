using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A0 RID: 672
	public class AudioEffectDistortion : MonoBehaviour
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x000B2C14 File Offset: 0x000B0E14
		// (set) Token: 0x060014BF RID: 5311 RVA: 0x000B2C1C File Offset: 0x000B0E1C
		public float InputGain
		{
			get
			{
				return this._inputGain;
			}
			set
			{
				this._inputGain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x000B2C34 File Offset: 0x000B0E34
		// (set) Token: 0x060014C1 RID: 5313 RVA: 0x000B2C3C File Offset: 0x000B0E3C
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 1E-05f, 1f);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x000B2C54 File Offset: 0x000B0E54
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x000B2C5C File Offset: 0x000B0E5C
		public float DryWet
		{
			get
			{
				return this._dryWet;
			}
			set
			{
				this._dryWet = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x000B2C74 File Offset: 0x000B0E74
		// (set) Token: 0x060014C5 RID: 5317 RVA: 0x000B2C7C File Offset: 0x000B0E7C
		public float OutputGain
		{
			get
			{
				return this._outputGain;
			}
			set
			{
				this._outputGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x000B2C94 File Offset: 0x000B0E94
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] *= this.InputGain;
					float num = data[i + j];
					if (Mathf.Abs(num) > this.Threshold)
					{
						num = Mathf.Sign(num);
					}
					data[i + j] = (1f - this.DryWet) * data[i + j] + this.DryWet * num;
					data[i + j] *= this.OutputGain;
				}
			}
		}

		// Token: 0x040022EF RID: 8943
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1f;

		// Token: 0x040022F0 RID: 8944
		[SerializeField]
		[Range(1E-05f, 1f)]
		private float _threshold = 0.036f;

		// Token: 0x040022F1 RID: 8945
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 0.258f;

		// Token: 0x040022F2 RID: 8946
		[SerializeField]
		[Range(0f, 5f)]
		private float _outputGain = 1f;
	}
}
