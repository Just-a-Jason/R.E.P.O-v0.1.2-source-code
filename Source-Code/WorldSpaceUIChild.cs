using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class WorldSpaceUIChild : MonoBehaviour
{
	// Token: 0x0600133E RID: 4926 RVA: 0x000A8CC8 File Offset: 0x000A6EC8
	protected virtual void Start()
	{
		this.myRect = base.GetComponent<RectTransform>();
		this.SetPosition();
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x000A8CDC File Offset: 0x000A6EDC
	protected virtual void Update()
	{
		this.SetPosition();
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x000A8CE4 File Offset: 0x000A6EE4
	private void SetPosition()
	{
		Vector3 a = SemiFunc.UIWorldToCanvasPosition(this.worldPosition);
		this.myRect.anchoredPosition = a + this.positionOffset;
	}

	// Token: 0x040020CA RID: 8394
	internal Vector3 worldPosition;

	// Token: 0x040020CB RID: 8395
	private RectTransform myRect;

	// Token: 0x040020CC RID: 8396
	internal Vector3 positionOffset;
}
