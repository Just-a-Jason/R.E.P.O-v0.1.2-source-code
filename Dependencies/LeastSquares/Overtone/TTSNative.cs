using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x0200029A RID: 666
	public static class TTSNative
	{
		// Token: 0x0600145D RID: 5213
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_start")]
		public static extern IntPtr OvertoneStart();

		// Token: 0x0600145E RID: 5214
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_text_2_audio")]
		public static extern TTSNative.OvertoneResult OvertoneText2Audio(IntPtr ctx, IntPtr text, IntPtr voice);

		// Token: 0x0600145F RID: 5215
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_load_voice")]
		public static extern IntPtr OvertoneLoadVoice(IntPtr configBuffer, uint configBufferSize, IntPtr modelBuffer, uint modelBufferSize);

		// Token: 0x06001460 RID: 5216
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_set_speaker_id")]
		public static extern void OvertoneSetSpeakerId(IntPtr voice, long speakerId);

		// Token: 0x06001461 RID: 5217
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_free_voice")]
		public static extern void OvertoneFreeVoice(IntPtr voice);

		// Token: 0x06001462 RID: 5218
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_free_result")]
		public static extern void OvertoneFreeResult(TTSNative.OvertoneResult result);

		// Token: 0x06001463 RID: 5219
		[DllImport("overtone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "overtone_free")]
		public static extern void OvertoneFree(IntPtr ctx);

		// Token: 0x040022B5 RID: 8885
		private const string OvertoneLibrary = "overtone";

		// Token: 0x020003D0 RID: 976
		public struct OvertoneResult
		{
			// Token: 0x0400292C RID: 10540
			public uint Channels;

			// Token: 0x0400292D RID: 10541
			public uint SampleRate;

			// Token: 0x0400292E RID: 10542
			public uint LengthSamples;

			// Token: 0x0400292F RID: 10543
			public IntPtr Samples;
		}
	}
}
