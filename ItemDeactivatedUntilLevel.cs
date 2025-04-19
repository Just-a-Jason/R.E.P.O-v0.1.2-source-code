using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class ItemDeactivatedUntilLevel : MonoBehaviour
{
	// Token: 0x06000AE7 RID: 2791 RVA: 0x000618DD File Offset: 0x0005FADD
	private void Start()
	{
		if (SemiFunc.RunGetLevelsCompleted() < this.levelToActivate)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011AD RID: 4525
	public int levelToActivate;
}
