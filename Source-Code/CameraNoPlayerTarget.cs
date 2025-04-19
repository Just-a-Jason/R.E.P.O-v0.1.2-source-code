using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class CameraNoPlayerTarget : MonoBehaviour
{
	// Token: 0x0600009D RID: 157 RVA: 0x00006279 File Offset: 0x00004479
	protected virtual void Awake()
	{
		CameraNoPlayerTarget.instance = this;
		this.cam = base.GetComponent<Camera>();
		this.cam.enabled = false;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00006299 File Offset: 0x00004499
	protected virtual void Update()
	{
		SemiFunc.UIHideAim();
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideGoal();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideShopCost();
	}

	// Token: 0x04000189 RID: 393
	public static CameraNoPlayerTarget instance;

	// Token: 0x0400018A RID: 394
	internal Camera cam;
}
