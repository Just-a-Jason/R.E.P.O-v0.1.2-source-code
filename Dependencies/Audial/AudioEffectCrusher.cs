using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x0200029E RID: 670
	public class AudioEffectCrusher : MonoBehaviour
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x000B26C0 File Offset: 0x000B08C0
		// (set) Token: 0x060014A5 RID: 5285 RVA: 0x000B26C8 File Offset: 0x000B08C8
		public int BitDepth
		{
			get
			{
				return this._bitDepth;
			}
			set
			{
				if (value == this._bitDepth)
				{
					return;
				}
				this._bitDepth = Mathf.Clamp(value, 1, 32);
				this.Callibrate();
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x000B26E9 File Offset: 0x000B08E9
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x000B26F1 File Offset: 0x000B08F1
		public float SampleRate
		{
			get
			{
				return this._sampleRate;
			}
			set
			{
				if (value == this._sampleRate)
				{
					return;
				}
				this._sampleRate = Mathf.Clamp(value, 0.001f, 1f);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x000B2713 File Offset: 0x000B0913
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x000B271B File Offset: 0x000B091B
		public float DryWet
		{
			get
			{
				return this._dryWet;
			}
			set
			{
				if (value == this._dryWet)
				{
					return;
				}
				this._dryWet = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x000B273D File Offset: 0x000B093D
		private void Awake()
		{
			this.y = new float[2];
			this.Callibrate();
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x000B2751 File Offset: 0x000B0951
		private void Callibrate()
		{
			this.m = 1L << (this._bitDepth - 1 & 31);
			this.m = ((this.m < 0L) ? 2147483647L : this.m);
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x000B2788 File Offset: 0x000B0988
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				this.cnt += this.SampleRate;
				if (this.cnt >= 1f)
				{
					this.cnt -= 1f;
					for (int j = 0; j < channels; j++)
					{
						this.y[j] = (float)((long)(data[i + j] * (float)this.m)) / (float)this.m;
					}
				}
				for (int k = 0; k < channels; k++)
				{
					float num = this.y[k];
					data[i + k] = data[i + k] * (1f - this.DryWet) + num * this.DryWet;
				}
			}
		}

		// Token: 0x040022DB RID: 8923
		[SerializeField]
		[HideInInspector]
		private int _bitDepth = 8;

		// Token: 0x040022DC RID: 8924
		private long m;

		// Token: 0x040022DD RID: 8925
		[SerializeField]
		[Range(0.001f, 1f)]
		private float _sampleRate = 0.1f;

		// Token: 0x040022DE RID: 8926
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 1f;

		// Token: 0x040022DF RID: 8927
		private float[] y;

		// Token: 0x040022E0 RID: 8928
		private float cnt;

		// Token: 0x040022E1 RID: 8929
		private LFO lfo;
	}
}
