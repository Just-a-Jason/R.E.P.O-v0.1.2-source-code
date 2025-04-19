using System;
using UnityEngine;

// Token: 0x020000FD RID: 253
[CreateAssetMenu(fileName = "Material - _____", menuName = "Other/Material Preset", order = 1)]
public class MaterialPreset : ScriptableObject
{
	// Token: 0x060008D3 RID: 2259 RVA: 0x0005498B File Offset: 0x00052B8B
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00054994 File Offset: 0x00052B94
	public void Setup()
	{
		this.RareImpactLightCurrent = (float)Random.Range(this.RareImpactLightMin, this.RareImpactLightMax);
		this.RareImpactMediumCurrent = (float)Random.Range(this.RareImpactMediumMin, this.RareImpactMediumMax);
		this.RareImpactHeavyCurrent = (float)Random.Range(this.RareImpactHeavyMin, this.RareImpactHeavyMax);
		this.RareFootstepLightCurrent = (float)Random.Range(this.RareFootstepLightMin, this.RareFootstepLightMax);
		this.RareFootstepMediumCurrent = (float)Random.Range(this.RareFootstepMediumMin, this.RareFootstepMediumMax);
		this.RareFootstepHeavyCurrent = (float)Random.Range(this.RareFootstepHeavyMin, this.RareFootstepHeavyMax);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00054A34 File Offset: 0x00052C34
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.ImpactLight.Type = AudioManager.AudioType.MaterialImpact;
		this.ImpactMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.ImpactHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactLight.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.FootstepLight.Type = AudioManager.AudioType.Footstep;
		this.FootstepMedium.Type = AudioManager.AudioType.Footstep;
		this.FootstepHeavy.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepLight.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepMedium.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepHeavy.Type = AudioManager.AudioType.Footstep;
		this.SlideOneShot.Type = AudioManager.AudioType.MaterialImpact;
		this.SlideLoop.Type = AudioManager.AudioType.MaterialImpact;
	}

	// Token: 0x04000FFD RID: 4093
	public string Name;

	// Token: 0x04000FFE RID: 4094
	public Materials.Type Type;

	// Token: 0x04000FFF RID: 4095
	[Space]
	[Header("Impact Sounds")]
	public Sound ImpactLight;

	// Token: 0x04001000 RID: 4096
	public Sound ImpactMedium;

	// Token: 0x04001001 RID: 4097
	public Sound ImpactHeavy;

	// Token: 0x04001002 RID: 4098
	[Space]
	[Header("Rare Impact Sounds")]
	public Sound RareImpactLight;

	// Token: 0x04001003 RID: 4099
	public int RareImpactLightMin;

	// Token: 0x04001004 RID: 4100
	public int RareImpactLightMax;

	// Token: 0x04001005 RID: 4101
	[HideInInspector]
	public float RareImpactLightCurrent;

	// Token: 0x04001006 RID: 4102
	[Space]
	public Sound RareImpactMedium;

	// Token: 0x04001007 RID: 4103
	public int RareImpactMediumMin;

	// Token: 0x04001008 RID: 4104
	public int RareImpactMediumMax;

	// Token: 0x04001009 RID: 4105
	[HideInInspector]
	public float RareImpactMediumCurrent;

	// Token: 0x0400100A RID: 4106
	[Space]
	public Sound RareImpactHeavy;

	// Token: 0x0400100B RID: 4107
	public int RareImpactHeavyMin;

	// Token: 0x0400100C RID: 4108
	public int RareImpactHeavyMax;

	// Token: 0x0400100D RID: 4109
	[HideInInspector]
	public float RareImpactHeavyCurrent;

	// Token: 0x0400100E RID: 4110
	[Space]
	[Header("Footstep Sounds")]
	public Sound FootstepLight;

	// Token: 0x0400100F RID: 4111
	public Sound FootstepMedium;

	// Token: 0x04001010 RID: 4112
	public Sound FootstepHeavy;

	// Token: 0x04001011 RID: 4113
	[Space]
	[Header("Rare Footstep Sounds")]
	public Sound RareFootstepLight;

	// Token: 0x04001012 RID: 4114
	public int RareFootstepLightMin;

	// Token: 0x04001013 RID: 4115
	public int RareFootstepLightMax;

	// Token: 0x04001014 RID: 4116
	[HideInInspector]
	public float RareFootstepLightCurrent;

	// Token: 0x04001015 RID: 4117
	[Space]
	public Sound RareFootstepMedium;

	// Token: 0x04001016 RID: 4118
	public int RareFootstepMediumMin;

	// Token: 0x04001017 RID: 4119
	public int RareFootstepMediumMax;

	// Token: 0x04001018 RID: 4120
	[HideInInspector]
	public float RareFootstepMediumCurrent;

	// Token: 0x04001019 RID: 4121
	[Space]
	public Sound RareFootstepHeavy;

	// Token: 0x0400101A RID: 4122
	public int RareFootstepHeavyMin;

	// Token: 0x0400101B RID: 4123
	public int RareFootstepHeavyMax;

	// Token: 0x0400101C RID: 4124
	[HideInInspector]
	public float RareFootstepHeavyCurrent;

	// Token: 0x0400101D RID: 4125
	[Space]
	[Header("Slide Sounds")]
	public Sound SlideOneShot;

	// Token: 0x0400101E RID: 4126
	public Sound SlideLoop;
}
