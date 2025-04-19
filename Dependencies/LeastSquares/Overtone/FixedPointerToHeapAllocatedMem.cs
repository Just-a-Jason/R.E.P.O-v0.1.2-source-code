using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x02000296 RID: 662
	public class FixedPointerToHeapAllocatedMem
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06001445 RID: 5189 RVA: 0x000B186C File Offset: 0x000AFA6C
		// (set) Token: 0x06001446 RID: 5190 RVA: 0x000B1874 File Offset: 0x000AFA74
		public IntPtr Address { get; private set; }

		// Token: 0x06001447 RID: 5191 RVA: 0x000B187D File Offset: 0x000AFA7D
		public void Free()
		{
			this._handle.Free();
			this.Address = IntPtr.Zero;
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x000B1895 File Offset: 0x000AFA95
		public static FixedPointerToHeapAllocatedMem Create<T>(T Object, uint SizeInBytes)
		{
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem = new FixedPointerToHeapAllocatedMem();
			fixedPointerToHeapAllocatedMem._handle = GCHandle.Alloc(Object, GCHandleType.Pinned);
			fixedPointerToHeapAllocatedMem.SizeInBytes = SizeInBytes;
			fixedPointerToHeapAllocatedMem.Address = fixedPointerToHeapAllocatedMem._handle.AddrOfPinnedObject();
			return fixedPointerToHeapAllocatedMem;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06001449 RID: 5193 RVA: 0x000B18C6 File Offset: 0x000AFAC6
		// (set) Token: 0x0600144A RID: 5194 RVA: 0x000B18CE File Offset: 0x000AFACE
		public uint SizeInBytes { get; private set; }

		// Token: 0x040022AA RID: 8874
		private GCHandle _handle;
	}
}
