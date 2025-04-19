using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x0200029F RID: 671
	public class AudioEffectDelay : MonoBehaviour
	{
		// Token: 0x060014AE RID: 5294 RVA: 0x000B2860 File Offset: 0x000B0A60
		private void Awake()
		{
			this.sampleRate = (float)AudioSettings.outputSampleRate;
			this.ChangeDelay();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x000B2874 File Offset: 0x000B0A74
		// (set) Token: 0x060014B0 RID: 5296 RVA: 0x000B287C File Offset: 0x000B0A7C
		public float BPM
		{
			get
			{
				return this._BPM;
			}
			set
			{
				this._BPM = Mathf.Clamp(value, 40f, 300f);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060014B1 RID: 5297 RVA: 0x000B289A File Offset: 0x000B0A9A
		// (set) Token: 0x060014B2 RID: 5298 RVA: 0x000B28A2 File Offset: 0x000B0AA2
		public int DelayCount
		{
			get
			{
				return this._delayCount;
			}
			set
			{
				this._delayCount = Mathf.Clamp(value, 1, 8);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060014B3 RID: 5299 RVA: 0x000B28B8 File Offset: 0x000B0AB8
		// (set) Token: 0x060014B4 RID: 5300 RVA: 0x000B28C0 File Offset: 0x000B0AC0
		public int DelayUnit
		{
			get
			{
				return this._delayUnit;
			}
			set
			{
				this._delayUnit = Mathf.Clamp(value, 1, 32);
				this.ChangeDelay();
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060014B5 RID: 5301 RVA: 0x000B28D7 File Offset: 0x000B0AD7
		// (set) Token: 0x060014B6 RID: 5302 RVA: 0x000B28DF File Offset: 0x000B0ADF
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060014B7 RID: 5303 RVA: 0x000B28F7 File Offset: 0x000B0AF7
		// (set) Token: 0x060014B8 RID: 5304 RVA: 0x000B28FF File Offset: 0x000B0AFF
		public float DecayLength
		{
			get
			{
				return this._decayLength;
			}
			set
			{
				this._decayLength = Mathf.Clamp(value, 0.1f, 1f);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x000B2917 File Offset: 0x000B0B17
		// (set) Token: 0x060014BA RID: 5306 RVA: 0x000B291F File Offset: 0x000B0B1F
		public float Pan
		{
			get
			{
				return this._pan;
			}
			set
			{
				this._pan = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x000B2938 File Offset: 0x000B0B38
		private void ChangeDelay()
		{
			this.delayLength = (float)this.DelayCount * (240f / this.BPM) / (float)this.DelayUnit;
			this.delaySamples = (int)(this.delayLength * this.sampleRate);
			this.delayBuffer = new float[2, this.delaySamples];
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x000B2990 File Offset: 0x000B0B90
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.delayBuffer == null)
			{
				this.ChangeDelay();
			}
			float[] array = new float[channels];
			float[] array2 = new float[]
			{
				1f,
				1f
			};
			if (this.Pan > 0f)
			{
				array2[0] = 1f - Mathf.Abs(this.Pan);
			}
			else if (this.Pan < 0f)
			{
				array2[1] = 1f - Mathf.Abs(this.Pan);
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				this.index %= this.delaySamples;
				if (this.PingPong)
				{
					for (int j = 0; j < channels; j++)
					{
						array[j] = this.delayBuffer[j, this.index];
						this.delayBuffer[j, this.index] = 0f;
					}
					for (int k = 0; k < channels; k++)
					{
						float num = data[i + k];
						float num2 = array[(k + 1) % channels];
						this.output = num * (1f - this.DryWet) + num2 * this.DryWet;
						data[i + k] = this.output;
						this.delayBuffer[k, this.index] += num2 * this.DecayLength;
						this.delayBuffer[(k + 1) % channels, this.index] += num * array2[k];
					}
				}
				else
				{
					for (int l = 0; l < channels; l++)
					{
						array[l] = this.delayBuffer[l, this.index];
						this.delayBuffer[l, this.index] = 0f;
						float num3 = data[i + l];
						float num4 = array[l];
						this.output = num3 * (1f - this.DryWet) + num4 * this.DryWet;
						data[i + l] = this.output;
						this.delayBuffer[l, this.index] += num4 * this.DecayLength;
						this.delayBuffer[l, this.index] += num3 * array2[l];
					}
				}
				this.index++;
			}
		}

		// Token: 0x040022E2 RID: 8930
		private float sampleRate;

		// Token: 0x040022E3 RID: 8931
		private float[,] delayBuffer;

		// Token: 0x040022E4 RID: 8932
		private int index;

		// Token: 0x040022E5 RID: 8933
		[SerializeField]
		private float _BPM = 120f;

		// Token: 0x040022E6 RID: 8934
		[SerializeField]
		private int _delayCount = 3;

		// Token: 0x040022E7 RID: 8935
		[SerializeField]
		private int _delayUnit = 8;

		// Token: 0x040022E8 RID: 8936
		[SerializeField]
		private float _dryWet = 0.5f;

		// Token: 0x040022E9 RID: 8937
		[SerializeField]
		private float _decayLength = 0.25f;

		// Token: 0x040022EA RID: 8938
		[SerializeField]
		private float _pan;

		// Token: 0x040022EB RID: 8939
		public bool PingPong;

		// Token: 0x040022EC RID: 8940
		private float delayLength;

		// Token: 0x040022ED RID: 8941
		private int delaySamples;

		// Token: 0x040022EE RID: 8942
		private float output;
	}
}
