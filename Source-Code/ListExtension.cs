using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000222 RID: 546
public static class ListExtension
{
	// Token: 0x0600119C RID: 4508 RVA: 0x0009C758 File Offset: 0x0009A958
	public static void Shuffle<T>(this IList<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list.Swap(i, Random.Range(0, list.Count));
		}
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0009C78C File Offset: 0x0009A98C
	public static void Swap<T>(this IList<T> list, int i, int j)
	{
		T value = list[j];
		T value2 = list[i];
		list[i] = value;
		list[j] = value2;
	}
}
