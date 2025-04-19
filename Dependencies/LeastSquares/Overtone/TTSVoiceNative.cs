using System;
using System.Threading;
using UnityEngine;

namespace LeastSquares.Overtone
{
	// Token: 0x0200029B RID: 667
	public class TTSVoiceNative
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x000B1B47 File Offset: 0x000AFD47
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x000B1B4F File Offset: 0x000AFD4F
		public IntPtr Pointer { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x000B1B58 File Offset: 0x000AFD58
		// (set) Token: 0x06001467 RID: 5223 RVA: 0x000B1B60 File Offset: 0x000AFD60
		public FixedPointerToHeapAllocatedMem ConfigPointer { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06001468 RID: 5224 RVA: 0x000B1B69 File Offset: 0x000AFD69
		// (set) Token: 0x06001469 RID: 5225 RVA: 0x000B1B71 File Offset: 0x000AFD71
		public FixedPointerToHeapAllocatedMem ModelPointer { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600146A RID: 5226 RVA: 0x000B1B7A File Offset: 0x000AFD7A
		// (set) Token: 0x0600146B RID: 5227 RVA: 0x000B1B82 File Offset: 0x000AFD82
		public bool Disposed { get; private set; }

		// Token: 0x0600146C RID: 5228 RVA: 0x000B1B8C File Offset: 0x000AFD8C
		public static TTSVoiceNative LoadVoiceFromResources(string voiceName)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(voiceName ?? "");
			TextAsset textAsset2 = Resources.Load<TextAsset>(voiceName + ".config");
			if (textAsset == null)
			{
				Debug.LogError("Failed to find voice model " + voiceName + ".bytes in Resources");
				return null;
			}
			if (textAsset2 == null)
			{
				Debug.LogError("Failed to find voice model " + voiceName + ".config.json in Resources");
				return null;
			}
			byte[] bytes = textAsset2.bytes;
			byte[] bytes2 = textAsset.bytes;
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem = FixedPointerToHeapAllocatedMem.Create<byte[]>(bytes, (uint)bytes.Length);
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem2 = FixedPointerToHeapAllocatedMem.Create<byte[]>(bytes2, (uint)bytes2.Length);
			IntPtr intPtr = TTSNative.OvertoneLoadVoice(fixedPointerToHeapAllocatedMem.Address, fixedPointerToHeapAllocatedMem.SizeInBytes, fixedPointerToHeapAllocatedMem2.Address, fixedPointerToHeapAllocatedMem2.SizeInBytes);
			TTSNative.OvertoneSetSpeakerId(intPtr, 0L);
			return new TTSVoiceNative
			{
				Pointer = intPtr,
				ConfigPointer = fixedPointerToHeapAllocatedMem,
				ModelPointer = fixedPointerToHeapAllocatedMem2
			};
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x000B1C61 File Offset: 0x000AFE61
		public void SetSpeakerId(int speakerId)
		{
			TTSNative.OvertoneSetSpeakerId(this.Pointer, (long)speakerId);
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x000B1C70 File Offset: 0x000AFE70
		public void AcquireReaderLock()
		{
			this._lock.AcquireReaderLock(8000);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000B1C82 File Offset: 0x000AFE82
		public void ReleaseReaderLock()
		{
			this._lock.ReleaseReaderLock();
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x000B1C90 File Offset: 0x000AFE90
		public void Dispose()
		{
			this._lock.AcquireWriterLock(8000);
			this.Disposed = true;
			this.ConfigPointer.Free();
			this.ModelPointer.Free();
			TTSNative.OvertoneFreeVoice(this.Pointer);
			this._lock.ReleaseWriterLock();
		}

		// Token: 0x040022B6 RID: 8886
		public const int Timeout = 8000;

		// Token: 0x040022BB RID: 8891
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
	}
}
