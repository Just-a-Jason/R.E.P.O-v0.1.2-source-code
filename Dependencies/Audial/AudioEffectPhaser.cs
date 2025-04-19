using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A6 RID: 678
	public class AudioEffectPhaser : MonoBehaviour
	{
		// Token: 0x060014F6 RID: 5366 RVA: 0x000B3460 File Offset: 0x000B1660
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x000B347C File Offset: 0x000B167C
		private void ResetUtils()
		{
			this.allPassFilters[0] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[1] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[2] = new AllPassFilter(this.Rate, this.Intensity);
			this.allPassFilters[3] = new AllPassFilter(this.Rate, this.Intensity);
			this.SetIntensity(this.Intensity);
			this.lfo = new LFO(this.Rate);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x000B350C File Offset: 0x000B170C
		private void SetIntensity(float i)
		{
			for (int j = 0; j < this.allPassFilters.Length; j++)
			{
				this.allPassFilters[j].gain = i * 0.6f;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x000B3540 File Offset: 0x000B1740
		// (set) Token: 0x060014FA RID: 5370 RVA: 0x000B3548 File Offset: 0x000B1748
		public float Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				if (this._rate == value)
				{
					return;
				}
				this._rate = Mathf.Clamp(value, 0.1f, 8f);
				this.lfo.SetRate(this._rate);
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060014FB RID: 5371 RVA: 0x000B357B File Offset: 0x000B177B
		// (set) Token: 0x060014FC RID: 5372 RVA: 0x000B3583 File Offset: 0x000B1783
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				if (this._width == value)
				{
					return;
				}
				this._width = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x000B35A5 File Offset: 0x000B17A5
		// (set) Token: 0x060014FE RID: 5374 RVA: 0x000B35AD File Offset: 0x000B17AD
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = Mathf.Clamp(value, 0f, 1f);
				this.SetIntensity(this._intensity);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x000B35D1 File Offset: 0x000B17D1
		// (set) Token: 0x06001500 RID: 5376 RVA: 0x000B35D9 File Offset: 0x000B17D9
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x000B35F1 File Offset: 0x000B17F1
		// (set) Token: 0x06001502 RID: 5378 RVA: 0x000B35FA File Offset: 0x000B17FA
		private int Offset
		{
			get
			{
				return (int)this._offset;
			}
			set
			{
				this._offset = (float)value;
			}
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x000B3604 File Offset: 0x000B1804
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				float num = Mathf.Lerp(Mathf.Lerp(this.fromMin, this.fromMax, this.Width), Mathf.Lerp(this.toMin, this.toMax, this.Width), this.lfo.GetValue()) * this.sampleRate / 1000f;
				for (int j = 0; j < this.allPassFilters.Length; j++)
				{
					this.allPassFilters[j].Offset = (int)num;
					for (int k = 0; k < channels; k++)
					{
						float num2 = data[i + k];
						float num3 = this.allPassFilters[j].ProcessSample(k, num2);
						this.output = num2 * (1f - this.DryWet / 2f) + num3 * this.DryWet / 2f;
						data[i + k] = this.output;
					}
					this.allPassFilters[j].MoveIndex();
				}
				this.lfo.MoveIndex();
			}
		}

		// Token: 0x0400230D RID: 8973
		private float output;

		// Token: 0x0400230E RID: 8974
		private float sampleRate;

		// Token: 0x0400230F RID: 8975
		public LFO lfo;

		// Token: 0x04002310 RID: 8976
		public AllPassFilter[] allPassFilters = new AllPassFilter[4];

		// Token: 0x04002311 RID: 8977
		[SerializeField]
		private float _rate = 0.3f;

		// Token: 0x04002312 RID: 8978
		[SerializeField]
		private float _width = 0.5f;

		// Token: 0x04002313 RID: 8979
		[SerializeField]
		private float _intensity = 0.25f;

		// Token: 0x04002314 RID: 8980
		[SerializeField]
		private float _dryWet = 0.75f;

		// Token: 0x04002315 RID: 8981
		private float _offset;

		// Token: 0x04002316 RID: 8982
		private float fromMin = 0.43f;

		// Token: 0x04002317 RID: 8983
		private float fromMax = 0.193f;

		// Token: 0x04002318 RID: 8984
		private float toMin = 0.772f;

		// Token: 0x04002319 RID: 8985
		private float toMax = 0.962f;
	}
}
