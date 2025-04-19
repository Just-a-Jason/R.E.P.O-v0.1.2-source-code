using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class MaterialSlidingLoop : MonoBehaviour
{
	// Token: 0x060008D7 RID: 2263 RVA: 0x00054AF9 File Offset: 0x00052CF9
	private void Start()
	{
		this.lowPassLogic = base.GetComponent<AudioLowPassLogic>();
		this.source = base.GetComponent<AudioSource>();
		this.activeTimer = 1f;
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00054B20 File Offset: 0x00052D20
	private void Update()
	{
		this.material.SlideLoop.Source = this.source;
		if (this.getMaterialTimer > 0f)
		{
			this.getMaterialTimer -= Time.deltaTime;
		}
		if (this.activeTimer > 0f)
		{
			this.activeTimer -= Time.deltaTime;
			this.material.SlideLoop.PlayLoop(true, 5f, 5f, this.pitchMultiplier);
			return;
		}
		this.lowPassLogic.Volume -= 5f * Time.deltaTime;
		if (this.lowPassLogic.Volume <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400101F RID: 4127
	private AudioSource source;

	// Token: 0x04001020 RID: 4128
	public float activeTimer;

	// Token: 0x04001021 RID: 4129
	public MaterialPreset material;

	// Token: 0x04001022 RID: 4130
	public float pitchMultiplier;

	// Token: 0x04001023 RID: 4131
	public float getMaterialTimer;

	// Token: 0x04001024 RID: 4132
	private AudioLowPassLogic lowPassLogic;
}
