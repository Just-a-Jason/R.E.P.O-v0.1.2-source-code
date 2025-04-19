using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A3 RID: 675
	public class AudioEffectFoldbackDistortion : MonoBehaviour
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060014D8 RID: 5336 RVA: 0x000B2FB0 File Offset: 0x000B11B0
		// (set) Token: 0x060014D9 RID: 5337 RVA: 0x000B2FB8 File Offset: 0x000B11B8
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060014DA RID: 5338 RVA: 0x000B2FD0 File Offset: 0x000B11D0
		// (set) Token: 0x060014DB RID: 5339 RVA: 0x000B2FD8 File Offset: 0x000B11D8
		public float SoftDistortAmount
		{
			get
			{
				return this._softDistortAmount;
			}
			set
			{
				this._softDistortAmount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060014DC RID: 5340 RVA: 0x000B2FF0 File Offset: 0x000B11F0
		// (set) Token: 0x060014DD RID: 5341 RVA: 0x000B2FF8 File Offset: 0x000B11F8
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 1E-06f, 1f);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x000B3010 File Offset: 0x000B1210
		// (set) Token: 0x060014DF RID: 5343 RVA: 0x000B3018 File Offset: 0x000B1218
		public float DistortAmount
		{
			get
			{
				return this._distortAmount;
			}
			set
			{
				this._distortAmount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x000B3030 File Offset: 0x000B1230
		// (set) Token: 0x060014E1 RID: 5345 RVA: 0x000B3038 File Offset: 0x000B1238
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

		// Token: 0x060014E2 RID: 5346 RVA: 0x000B3050 File Offset: 0x000B1250
		private float foldBack(float sample, float threshold)
		{
			if (Mathf.Abs(sample) > this.Threshold)
			{
				return Mathf.Abs(Mathf.Abs(sample - this.Threshold % (this.Threshold * 4f)) - this.Threshold * 2f) - this.Threshold + 0.3f * sample;
			}
			return sample;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x000B30A8 File Offset: 0x000B12A8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] *= this.InputGain;
					float num = this.foldBack(data[i + j], this.softThreshold);
					data[i + j] = (1f - this.SoftDistortAmount) * data[i + j] + this.SoftDistortAmount * num;
					data[i + j] *= this.OutputGain;
					float num2 = this.foldBack(data[i + j], this.Threshold);
					data[i + j] = (1f - this.DistortAmount) * data[i + j] + this.DistortAmount * num2;
					data[i + j] *= this.OutputGain;
				}
			}
		}

		// Token: 0x040022FE RID: 8958
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1.14f;

		// Token: 0x040022FF RID: 8959
		[SerializeField]
		[Range(0f, 1f)]
		private float _softDistortAmount = 0.177f;

		// Token: 0x04002300 RID: 8960
		private float softThreshold = 0.002f;

		// Token: 0x04002301 RID: 8961
		[SerializeField]
		[Range(1E-06f, 1f)]
		private float _threshold = 0.244f;

		// Token: 0x04002302 RID: 8962
		[SerializeField]
		[Range(0f, 1f)]
		private float _distortAmount = 0.904f;

		// Token: 0x04002303 RID: 8963
		[SerializeField]
		[Range(0f, 5f)]
		private float _outputGain = 1f;
	}
}
