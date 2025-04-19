using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A2 RID: 674
	[Serializable]
	public class AudioEffectFlanger : MonoBehaviour
	{
		// Token: 0x060014CC RID: 5324 RVA: 0x000B2DD9 File Offset: 0x000B0FD9
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x000B2DF3 File Offset: 0x000B0FF3
		private void ResetUtils()
		{
			this.combFilter = new CombFilter(this.Intensity, 0.5f);
			this.lfo = new LFO(this.Rate);
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060014CE RID: 5326 RVA: 0x000B2E1C File Offset: 0x000B101C
		// (set) Token: 0x060014CF RID: 5327 RVA: 0x000B2E24 File Offset: 0x000B1024
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x000B2E57 File Offset: 0x000B1057
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x000B2E5F File Offset: 0x000B105F
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = Mathf.Clamp(value, 0.1f, 0.9f);
				this.combFilter.gain = this._intensity;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x000B2E88 File Offset: 0x000B1088
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x000B2E90 File Offset: 0x000B1090
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x000B2EA8 File Offset: 0x000B10A8
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x000B2EB1 File Offset: 0x000B10B1
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

		// Token: 0x060014D6 RID: 5334 RVA: 0x000B2EBC File Offset: 0x000B10BC
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				this.combFilter.Offset = (int)Mathf.Lerp(1f * this.sampleRate / 1000f, 5f * this.sampleRate / 1000f, this.lfo.GetValue());
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = this.combFilter.ProcessSample(j, num);
					this.output = num * (1f - this.DryWet / 2f) + num2 * this.DryWet / 2f;
					data[i + j] = this.output;
				}
				this.combFilter.MoveIndex();
				this.lfo.MoveIndex();
			}
		}

		// Token: 0x040022F6 RID: 8950
		private float sampleRate;

		// Token: 0x040022F7 RID: 8951
		private float output;

		// Token: 0x040022F8 RID: 8952
		private LFO lfo;

		// Token: 0x040022F9 RID: 8953
		[SerializeField]
		private CombFilter combFilter;

		// Token: 0x040022FA RID: 8954
		[SerializeField]
		private float _rate = 0.3f;

		// Token: 0x040022FB RID: 8955
		[SerializeField]
		private float _intensity = 0.25f;

		// Token: 0x040022FC RID: 8956
		[SerializeField]
		[Range(0f, 1f)]
		private float _dryWet = 0.75f;

		// Token: 0x040022FD RID: 8957
		private float _offset;
	}
}
