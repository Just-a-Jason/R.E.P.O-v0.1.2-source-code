using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A7 RID: 679
	public class AudioEffectReverb : MonoBehaviour
	{
		// Token: 0x06001505 RID: 5381 RVA: 0x000B378F File Offset: 0x000B198F
		private void Awake()
		{
			Settings.SampleRate = (float)AudioSettings.outputSampleRate;
			this.Initialize();
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06001506 RID: 5382 RVA: 0x000B37A2 File Offset: 0x000B19A2
		// (set) Token: 0x06001507 RID: 5383 RVA: 0x000B37AA File Offset: 0x000B19AA
		public float ReverbTime
		{
			get
			{
				return this._reverbTime;
			}
			set
			{
				this._reverbTime = Mathf.Clamp(value, 0.5f, 10f);
				this.Callibrate();
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06001508 RID: 5384 RVA: 0x000B37C8 File Offset: 0x000B19C8
		// (set) Token: 0x06001509 RID: 5385 RVA: 0x000B37D0 File Offset: 0x000B19D0
		public float ReverbGain
		{
			get
			{
				return this._reverbGain;
			}
			set
			{
				this._reverbGain = Mathf.Clamp(value, 0.5f, 5f);
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600150A RID: 5386 RVA: 0x000B37E8 File Offset: 0x000B19E8
		// (set) Token: 0x0600150B RID: 5387 RVA: 0x000B37F0 File Offset: 0x000B19F0
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

		// Token: 0x0600150C RID: 5388 RVA: 0x000B3808 File Offset: 0x000B1A08
		private void Initialize()
		{
			this.combFilters = new CombFilter[4];
			this.combFilters[0] = new CombFilter(29.7f, 1f);
			this.combFilters[1] = new CombFilter(37.1f, 1f);
			this.combFilters[2] = new CombFilter(41.1f, 1f);
			this.combFilters[3] = new CombFilter(43.7f, 1f);
			this.Callibrate();
			this.allPassFilters = new AllPassFilter[2];
			this.allPassFilters[0] = new AllPassFilter(5f, 1f);
			this.allPassFilters[0].SetGainByDecayTime(1.683f);
			this.allPassFilters[1] = new AllPassFilter(1.7f, 1f);
			this.allPassFilters[1].SetGainByDecayTime(2.232f);
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x000B38E4 File Offset: 0x000B1AE4
		private void Callibrate()
		{
			if (this.combFilters == null)
			{
				return;
			}
			for (int i = 0; i < this.combFilters.Length; i++)
			{
				this.combFilters[i].SetGainByDecayTime(this.ReverbTime * 1000f);
			}
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x000B3928 File Offset: 0x000B1B28
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.combFilters == null || this.allPassFilters == null)
			{
				this.Initialize();
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j] * this.ReverbGain;
					for (int k = 0; k < this.combFilters.Length; k++)
					{
						num += this.combFilters[k].ProcessSample(j, data[i + j]);
					}
					num /= (float)this.combFilters.Length;
					float num2 = num / (float)this.combFilters.Length;
					for (int l = 0; l < this.allPassFilters.Length; l++)
					{
						num2 += this.allPassFilters[l].ProcessSample(j, num);
					}
					data[i + j] = data[i + j] * (1f - this.DryWet) + num2 * this.ReverbGain / (float)this.allPassFilters.Length * this.DryWet;
				}
				for (int m = 0; m < this.combFilters.Length; m++)
				{
					this.combFilters[m].MoveIndex();
				}
				for (int n = 0; n < this.allPassFilters.Length; n++)
				{
					this.allPassFilters[n].MoveIndex();
				}
			}
		}

		// Token: 0x0400231A RID: 8986
		[SerializeField]
		private float _reverbTime = 1.55f;

		// Token: 0x0400231B RID: 8987
		[SerializeField]
		private float _reverbGain = 1f;

		// Token: 0x0400231C RID: 8988
		[SerializeField]
		private float _dryWet = 0.16f;

		// Token: 0x0400231D RID: 8989
		private CombFilter[] combFilters;

		// Token: 0x0400231E RID: 8990
		private AllPassFilter[] allPassFilters;
	}
}
