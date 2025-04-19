using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x02000297 RID: 663
	public class FixedString : IDisposable
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x000B18DF File Offset: 0x000AFADF
		// (set) Token: 0x0600144D RID: 5197 RVA: 0x000B18E7 File Offset: 0x000AFAE7
		public IntPtr Address { get; private set; }

		// Token: 0x0600144E RID: 5198 RVA: 0x000B18F0 File Offset: 0x000AFAF0
		public FixedString(string text)
		{
			this.Address = Marshal.StringToHGlobalAnsi(text);
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x000B1904 File Offset: 0x000AFB04
		public void Dispose()
		{
			if (this.Address == IntPtr.Zero)
			{
				return;
			}
			Marshal.FreeHGlobal(this.Address);
			this.Address = IntPtr.Zero;
		}
	}
}
