using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002AC RID: 684
	public class AudioEffectStateVariableFilter : MonoBehaviour
	{
		// Token: 0x0600152A RID: 5418 RVA: 0x000B3E91 File Offset: 0x000B2091
		private void Awake()
		{
			this.sampleFrequency = (float)AudioSettings.outputSampleRate;
			this.UpdateFrequency();
			this.UpdateDamp();
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x000B3EAB File Offset: 0x000B20AB
		// (set) Token: 0x0600152C RID: 5420 RVA: 0x000B3EB3 File Offset: 0x000B20B3
		public float Frequency
		{
			get
			{
				return this._frequency;
			}
			set
			{
				this._frequency = Mathf.Clamp(value, 50f, 12000f);
				this.UpdateFrequency();
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600152D RID: 5421 RVA: 0x000B3ED1 File Offset: 0x000B20D1
		// (set) Token: 0x0600152E RID: 5422 RVA: 0x000B3ED9 File Offset: 0x000B20D9
		public float Resonance
		{
			get
			{
				return this._resonance;
			}
			set
			{
				this._resonance = Mathf.Clamp(value, 0f, 1f);
				this.UpdateDamp();
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x000B3EF7 File Offset: 0x000B20F7
		// (set) Token: 0x06001530 RID: 5424 RVA: 0x000B3EFF File Offset: 0x000B20FF
		public float Drive
		{
			get
			{
				return this._drive;
			}
			set
			{
				this._drive = Mathf.Clamp(value, 0f, 0.1f);
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x000B3F17 File Offset: 0x000B2117
		// (set) Token: 0x06001532 RID: 5426 RVA: 0x000B3F1F File Offset: 0x000B211F
		public float AdditiveGain
		{
			get
			{
				return this._additiveGain;
			}
			set
			{
				this._additiveGain = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x000B3F37 File Offset: 0x000B2137
		private void UpdateFrequency()
		{
			this.freq = 2.0 * Math.Sin(3.141592653589793 * (double)this.Frequency / (double)(this.sampleFrequency * (float)this.passes));
			this.UpdateDamp();
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x000B3F78 File Offset: 0x000B2178
		private void UpdateDamp()
		{
			this.damp = Math.Min(2.0 * (1.0 - Math.Pow((double)this.Resonance, 0.25)), Math.Min(2.0 - this.freq, 2.0 / this.freq - this.freq * 0.5));
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x000B3FF0 File Offset: 0x000B21F0
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Filter == FilterState.Bypass)
			{
				return;
			}
			double[] array = new double[channels];
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					array[j] = (double)(((double)Math.Abs(data[i + j]) > 1E-07) ? data[i + j] : 0f);
					this.output[j] = 0.0;
					for (int k = 0; k < this.passes; k++)
					{
						this.high[j] = array[j] - this.low[j] - this.damp * this.band[j];
						this.band[j] = this.freq * this.high[j] + this.band[j] - (double)(0.1f - this.Drive + 0.001f) * Math.Pow(this.band[j], 3.0);
						this.low[j] = this.freq * this.band[j] + this.low[j];
					}
					switch (this.Filter)
					{
					case FilterState.LowPass:
					case FilterState.LowShelf:
						this.output[j] = this.low[j];
						break;
					case FilterState.HighPass:
					case FilterState.HighShelf:
						this.output[j] = this.high[j];
						break;
					case FilterState.BandPass:
					case FilterState.BandAdd:
						this.output[j] = this.band[j];
						break;
					}
					if (this.Filter == FilterState.HighShelf || this.Filter == FilterState.LowShelf || this.Filter == FilterState.BandAdd)
					{
						data[i + j] += (float)this.output[j] * this.AdditiveGain;
					}
					else
					{
						data[i + j] = (float)this.output[j];
					}
				}
			}
		}

		// Token: 0x0400233B RID: 9019
		private float sampleFrequency;

		// Token: 0x0400233C RID: 9020
		private int passes = 2;

		// Token: 0x0400233D RID: 9021
		[SerializeField]
		[Range(50f, 12000f)]
		private float _frequency = 440f;

		// Token: 0x0400233E RID: 9022
		private double freq;

		// Token: 0x0400233F RID: 9023
		[SerializeField]
		[Range(0f, 1f)]
		private float _resonance = 0.5f;

		// Token: 0x04002340 RID: 9024
		[SerializeField]
		[Range(0f, 0.1f)]
		private float _drive = 0.1f;

		// Token: 0x04002341 RID: 9025
		public FilterState Filter = FilterState.BandPass;

		// Token: 0x04002342 RID: 9026
		[SerializeField]
		[Range(-1f, 1f)]
		private float _additiveGain = 0.25f;

		// Token: 0x04002343 RID: 9027
		private double[] notch = new double[2];

		// Token: 0x04002344 RID: 9028
		private double[] low = new double[2];

		// Token: 0x04002345 RID: 9029
		private double[] high = new double[2];

		// Token: 0x04002346 RID: 9030
		private double[] band = new double[2];

		// Token: 0x04002347 RID: 9031
		private double[] output = new double[2];

		// Token: 0x04002348 RID: 9032
		private double damp;
	}
}
