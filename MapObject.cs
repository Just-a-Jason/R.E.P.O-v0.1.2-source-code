using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class MapObject : MonoBehaviour
{
	// Token: 0x06000CF0 RID: 3312 RVA: 0x00071484 File Offset: 0x0006F684
	public void Hide()
	{
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			if (transform != base.transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x000714CC File Offset: 0x0006F6CC
	public void Show()
	{
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			if (transform != base.transform)
			{
				transform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x040014B1 RID: 5297
	public Transform parent;
}
