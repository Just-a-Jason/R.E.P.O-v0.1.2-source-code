using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class PropLight : MonoBehaviour
{
	// Token: 0x060008C7 RID: 2247 RVA: 0x000542A8 File Offset: 0x000524A8
	private void Awake()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.originalIntensity = this.lightComponent.intensity;
		this.halo = (base.GetComponent("Halo") as Behaviour);
		if (this.halo)
		{
			this.hasHalo = true;
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x000542FC File Offset: 0x000524FC
	private void Start()
	{
		if (LevelGenerator.Instance.Generated)
		{
			SemiFunc.LightAdd(this);
		}
	}

	// Token: 0x04000FEC RID: 4076
	public bool levelLight = true;

	// Token: 0x04000FED RID: 4077
	internal bool turnedOff;

	// Token: 0x04000FEE RID: 4078
	[Range(0f, 2f)]
	public float lightRangeMultiplier = 1f;

	// Token: 0x04000FEF RID: 4079
	internal Light lightComponent;

	// Token: 0x04000FF0 RID: 4080
	internal float originalIntensity;

	// Token: 0x04000FF1 RID: 4081
	internal Behaviour halo;

	// Token: 0x04000FF2 RID: 4082
	internal bool hasHalo;
}
