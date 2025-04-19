using System;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002B1 RID: 689
	public class BufferedComponent
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x000B4481 File Offset: 0x000B2681
		// (set) Token: 0x0600154B RID: 5451 RVA: 0x000B4489 File Offset: 0x000B2689
		public float DelayLength
		{
			get
			{
				return this.delayLength;
			}
			set
			{
				this.delayLength = value;
				this.Offset = (int)(this.delayLength * (Settings.SampleRate / 1000f));
			}
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x000B44AC File Offset: 0x000B26AC
		public BufferedComponent(float delayLength, float gain)
		{
			this.DelayLength = delayLength;
			this.gain = gain;
			this.bufferLength = (int)Settings.SampleRate * 10;
			this.buffer = new float[this.channelCount, this.bufferLength];
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x000B44FA File Offset: 0x000B26FA
		public void SetGainByDecayTime(float decayLength)
		{
			this.gain = Mathf.Pow(0.001f, this.delayLength / decayLength);
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x000B4514 File Offset: 0x000B2714
		public float ProcessSample(int channel, float sample)
		{
			if (channel >= this.channelCount)
			{
				this.channelCount = channel + 1;
				this.buffer = new float[this.channelCount, this.bufferLength];
			}
			this.readIndex = ((this.Offset > this.writeIndex) ? (this.bufferLength + this.writeIndex - this.Offset) : (this.writeIndex - this.Offset));
			float num = this.buffer[channel, this.readIndex];
			this.buffer[channel, this.writeIndex] = sample + num * this.gain;
			return num;
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000B45B4 File Offset: 0x000B27B4
		public float ProcessSample(float sample)
		{
			this.readIndex = ((this.Offset > this.writeIndex) ? (this.bufferLength + this.writeIndex - this.Offset) : (this.writeIndex - this.Offset));
			float num = this.buffer[0, this.readIndex];
			this.buffer[0, this.writeIndex] = sample + num * this.gain;
			return num;
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x000B4628 File Offset: 0x000B2828
		public void MoveIndex()
		{
			this.writeIndex = (this.writeIndex + 1) % this.bufferLength;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x000B4640 File Offset: 0x000B2840
		public void Reset()
		{
			for (int i = 0; i < this.buffer.Length; i++)
			{
				Array.Clear(this.buffer, 0, this.buffer.Length);
			}
			this.readIndex = 0;
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06001552 RID: 5458 RVA: 0x000B4681 File Offset: 0x000B2881
		// (set) Token: 0x06001553 RID: 5459 RVA: 0x000B4689 File Offset: 0x000B2889
		public int Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = (int)Mathf.Lerp((float)this._offset, (float)value, 0.8f);
			}
		}

		// Token: 0x04002351 RID: 9041
		public float[,] buffer;

		// Token: 0x04002352 RID: 9042
		private float loopTime;

		// Token: 0x04002353 RID: 9043
		public float delayLength;

		// Token: 0x04002354 RID: 9044
		public float decayLength;

		// Token: 0x04002355 RID: 9045
		public int bufferLength;

		// Token: 0x04002356 RID: 9046
		public float gain;

		// Token: 0x04002357 RID: 9047
		public int readIndex;

		// Token: 0x04002358 RID: 9048
		public int writeIndex;

		// Token: 0x04002359 RID: 9049
		public int channelCount = 1;

		// Token: 0x0400235A RID: 9050
		private float sampleRate;

		// Token: 0x0400235B RID: 9051
		[SerializeField]
		private int _offset;
	}
}
