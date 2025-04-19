using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x0200029D RID: 669
	public class AudioEffectCompressor : MonoBehaviour
	{
		// Token: 0x06001491 RID: 5265 RVA: 0x000B2364 File Offset: 0x000B0564
		private void Awake()
		{
			Settings.SampleRate = (float)AudioSettings.outputSampleRate;
			this.envelope = new Envelope(this.Attack, this.Release);
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06001492 RID: 5266 RVA: 0x000B2388 File Offset: 0x000B0588
		// (set) Token: 0x06001493 RID: 5267 RVA: 0x000B2390 File Offset: 0x000B0590
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06001494 RID: 5268 RVA: 0x000B23A8 File Offset: 0x000B05A8
		// (set) Token: 0x06001495 RID: 5269 RVA: 0x000B23B0 File Offset: 0x000B05B0
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x000B23C8 File Offset: 0x000B05C8
		// (set) Token: 0x06001497 RID: 5271 RVA: 0x000B23D0 File Offset: 0x000B05D0
		public float Slope
		{
			get
			{
				return this._slope;
			}
			set
			{
				this._slope = Mathf.Clamp(value, 0f, 2f);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06001498 RID: 5272 RVA: 0x000B23E8 File Offset: 0x000B05E8
		// (set) Token: 0x06001499 RID: 5273 RVA: 0x000B23F0 File Offset: 0x000B05F0
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Attack = this._attack;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600149A RID: 5274 RVA: 0x000B2419 File Offset: 0x000B0619
		// (set) Token: 0x0600149B RID: 5275 RVA: 0x000B2421 File Offset: 0x000B0621
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				this._release = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Release = this._release;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600149C RID: 5276 RVA: 0x000B244A File Offset: 0x000B064A
		// (set) Token: 0x0600149D RID: 5277 RVA: 0x000B2452 File Offset: 0x000B0652
		public float DryGain
		{
			get
			{
				return this._dryGain;
			}
			set
			{
				this._dryGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x000B246A File Offset: 0x000B066A
		// (set) Token: 0x0600149F RID: 5279 RVA: 0x000B2472 File Offset: 0x000B0672
		public float CompressedGain
		{
			get
			{
				return this._compressedGain;
			}
			set
			{
				this._compressedGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x000B248A File Offset: 0x000B068A
		// (set) Token: 0x060014A1 RID: 5281 RVA: 0x000B2492 File Offset: 0x000B0692
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

		// Token: 0x060014A2 RID: 5282 RVA: 0x000B24AC File Offset: 0x000B06AC
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.envelope == null)
			{
				return;
			}
			if (this.input == null || this.input.Length != channels)
			{
				this.input = new float[channels];
				this.compressed = new float[channels];
				this.dry = new float[channels];
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				this.rms = 0f;
				for (int j = 0; j < channels; j++)
				{
					this.input[j] = data[i + j] * this.InputGain;
					this.rms += this.input[j] * this.input[j];
				}
				this.rms = Mathf.Pow(this.rms, 1f / (float)channels);
				this.env = this.envelope.ProcessSample(this.rms);
				this.compressMod = 1f;
				if (this.env > this.Threshold)
				{
					this.compressMod = Mathf.Clamp(this.compressMod - (this.env - this.Threshold) * this.Slope, 0f, 1f);
				}
				this.mergedData = 0f;
				for (int k = 0; k < channels; k++)
				{
					this.compressed[k] = this.input[k] * this.compressMod;
					this.mergedData += this.compressed[k] * this.compressed[k];
					data[i + k] = (this.compressed[k] * this.CompressedGain + this.input[k] * this.DryGain) * this.OutputGain;
				}
				this.mergedData = Mathf.Pow(this.mergedData, 1f / (float)channels);
			}
		}

		// Token: 0x040022CB RID: 8907
		[SerializeField]
		public Envelope envelope;

		// Token: 0x040022CC RID: 8908
		[SerializeField]
		private float _inputGain = 1f;

		// Token: 0x040022CD RID: 8909
		[SerializeField]
		private float _threshold = 0.247f;

		// Token: 0x040022CE RID: 8910
		[SerializeField]
		public float _slope = 1.727f;

		// Token: 0x040022CF RID: 8911
		[SerializeField]
		private float _attack = 0.0001f;

		// Token: 0x040022D0 RID: 8912
		[SerializeField]
		public float _release = 0.68f;

		// Token: 0x040022D1 RID: 8913
		[SerializeField]
		private float _dryGain;

		// Token: 0x040022D2 RID: 8914
		[SerializeField]
		private float _compressedGain = 1f;

		// Token: 0x040022D3 RID: 8915
		[SerializeField]
		private float _outputGain = 1f;

		// Token: 0x040022D4 RID: 8916
		private float env;

		// Token: 0x040022D5 RID: 8917
		private float mergedData;

		// Token: 0x040022D6 RID: 8918
		private float[] input;

		// Token: 0x040022D7 RID: 8919
		private float rms;

		// Token: 0x040022D8 RID: 8920
		private float[] compressed;

		// Token: 0x040022D9 RID: 8921
		private float[] dry;

		// Token: 0x040022DA RID: 8922
		private float compressMod;
	}
}
