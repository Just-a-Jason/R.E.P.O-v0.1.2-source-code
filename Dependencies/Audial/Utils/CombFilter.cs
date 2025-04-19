using System;

namespace Audial.Utils
{
	// Token: 0x020002B2 RID: 690
	public class CombFilter : BufferedComponent
	{
		// Token: 0x06001554 RID: 5460 RVA: 0x000B46A5 File Offset: 0x000B28A5
		public CombFilter(float delayLength, float gain) : base(delayLength, gain)
		{
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x000B46AF File Offset: 0x000B28AF
		public new float ProcessSample(int channel, float sample)
		{
			return base.ProcessSample(channel, sample);
		}
	}
}
