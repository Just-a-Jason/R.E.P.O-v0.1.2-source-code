using System;
using System.Collections;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002B6 RID: 694
	public class LFO
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600155C RID: 5468 RVA: 0x000B477B File Offset: 0x000B297B
		// (set) Token: 0x0600155D RID: 5469 RVA: 0x000B4783 File Offset: 0x000B2983
		public float Index
		{
			get
			{
				return this._index;
			}
			set
			{
				this._index = value;
				if (this._index >= (float)this.tableLength - 0.5f)
				{
					this._index -= (float)this.tableLength;
				}
			}
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x000B47B5 File Offset: 0x000B29B5
		public IEnumerator Run()
		{
			this.runState = RunState.Running;
			while (this.runState != RunState.Stopped)
			{
				if (this.runState == RunState.Running)
				{
					this.Index += (float)this.tableLength / this.StepSize * Time.deltaTime;
				}
				yield return new WaitForSeconds(0.002f);
			}
			yield break;
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000B47C4 File Offset: 0x000B29C4
		public void Pause()
		{
			this.runState = RunState.Paused;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x000B47CD File Offset: 0x000B29CD
		public void Resume()
		{
			this.runState = RunState.Running;
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x000B47D6 File Offset: 0x000B29D6
		public void Stop()
		{
			this.runState = RunState.Stopped;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06001562 RID: 5474 RVA: 0x000B47DF File Offset: 0x000B29DF
		// (set) Token: 0x06001563 RID: 5475 RVA: 0x000B47E7 File Offset: 0x000B29E7
		public float StepSize
		{
			get
			{
				return this._stepSize;
			}
			set
			{
				this._stepSize = value;
			}
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000B47F0 File Offset: 0x000B29F0
		public void SetRate(float rate)
		{
			this.StepSize = rate;
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x000B47F9 File Offset: 0x000B29F9
		public int GetIndex()
		{
			return Mathf.RoundToInt(this.Index) % LFO.waveTable.Length;
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x000B480E File Offset: 0x000B2A0E
		public float GetValue()
		{
			return LFO.waveTable[this.GetIndex()];
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x000B481C File Offset: 0x000B2A1C
		public void MoveIndex()
		{
			this.Index += (float)this.tableLength / this.StepSize / Settings.SampleRate;
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x000B4840 File Offset: 0x000B2A40
		public float[] GetChunkValue(int chunkLength)
		{
			float[] array = new float[chunkLength];
			for (int i = 0; i < chunkLength; i++)
			{
				array[i] = this.GetValue();
			}
			return array;
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x000B486A File Offset: 0x000B2A6A
		public LFO()
		{
			if (LFO.waveTable == null)
			{
				this.CreateWavetable();
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x000B4895 File Offset: 0x000B2A95
		public LFO(float speed)
		{
			if (LFO.waveTable == null)
			{
				this.CreateWavetable();
			}
			this.StepSize = speed;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x000B48C8 File Offset: 0x000B2AC8
		private void CreateWavetable()
		{
			LFO.waveTable = new float[this.tableLength];
			for (int i = 0; i < this.tableLength; i++)
			{
				LFO.waveTable[i] = 0.5f + Mathf.Sin(6.2831855f * (float)i / (float)this.tableLength) / 2f;
			}
		}

		// Token: 0x0400236D RID: 9069
		private int tableLength = 128;

		// Token: 0x0400236E RID: 9070
		public static float[] waveTable;

		// Token: 0x0400236F RID: 9071
		private float _index;

		// Token: 0x04002370 RID: 9072
		private RunState runState;

		// Token: 0x04002371 RID: 9073
		private float _stepSize = 0.3f;
	}
}
