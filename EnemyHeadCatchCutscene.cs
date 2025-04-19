using System;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class EnemyHeadCatchCutscene : MonoBehaviour
{
	// Token: 0x060002E8 RID: 744 RVA: 0x0001D0EF File Offset: 0x0001B2EF
	public void PlayBiteBegin()
	{
		this.BiteBegin.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0001D11C File Offset: 0x0001B31C
	public void PlayBiteFirst()
	{
		this.BiteFirst.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.CameraGlitchShort01.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlassBreak01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0001D1AC File Offset: 0x0001B3AC
	public void PlayBiteLast()
	{
		this.BiteLast.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.CameraGlitchShort02.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlassBreak02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x0001D23A File Offset: 0x0001B43A
	public void PlayMusic01()
	{
		this.Music01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0001D267 File Offset: 0x0001B467
	public void PlayMusic02()
	{
		this.Music02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x0001D294 File Offset: 0x0001B494
	public void PlayCameraGlitchLong01()
	{
		this.CameraGlitchLong01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0001D2C1 File Offset: 0x0001B4C1
	public void PlayCameraGlitchLong02()
	{
		this.CameraGlitchLong02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0001D2EE File Offset: 0x0001B4EE
	public void PlayGlassTension()
	{
		this.GlassTension.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040004FC RID: 1276
	public Sound BiteBegin;

	// Token: 0x040004FD RID: 1277
	public Sound BiteFirst;

	// Token: 0x040004FE RID: 1278
	public Sound BiteLast;

	// Token: 0x040004FF RID: 1279
	[Space]
	public Sound Music01;

	// Token: 0x04000500 RID: 1280
	public Sound Music02;

	// Token: 0x04000501 RID: 1281
	[Space]
	public Sound CameraGlitchLong01;

	// Token: 0x04000502 RID: 1282
	public Sound CameraGlitchLong02;

	// Token: 0x04000503 RID: 1283
	[Space]
	public Sound CameraGlitchShort01;

	// Token: 0x04000504 RID: 1284
	public Sound CameraGlitchShort02;

	// Token: 0x04000505 RID: 1285
	[Space]
	public Sound GlassBreak01;

	// Token: 0x04000506 RID: 1286
	public Sound GlassBreak02;

	// Token: 0x04000507 RID: 1287
	public Sound GlassTension;
}
