using System;

namespace Audial.Utils
{
	// Token: 0x020002B0 RID: 688
	public class AllPassFilter : BufferedComponent
	{
		// Token: 0x06001548 RID: 5448 RVA: 0x000B4464 File Offset: 0x000B2664
		public AllPassFilter(float delayLength, float gain) : base(delayLength, gain)
		{
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000B446E File Offset: 0x000B266E
		public new float ProcessSample(int channel, float sample)
		{
			return base.ProcessSample(channel, sample) - this.gain * sample;
		}
	}
}
