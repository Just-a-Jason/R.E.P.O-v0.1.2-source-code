using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DF RID: 223
[CreateAssetMenu(fileName = "Level - _____", menuName = "Level/Level Preset", order = 0)]
public class Level : ScriptableObject
{
	// Token: 0x060007FD RID: 2045 RVA: 0x0004DF9F File Offset: 0x0004C19F
	public void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (Application.isPlaying && LevelGenerator.Instance != null && LevelGenerator.Instance.Generated)
		{
			EnvironmentDirector.Instance.Setup();
			PostProcessing.Instance.Setup();
		}
	}

	// Token: 0x04000E86 RID: 3718
	public string ResourcePath = "";

	// Token: 0x04000E87 RID: 3719
	[Space]
	public string NarrativeName = "";

	// Token: 0x04000E88 RID: 3720
	[Space]
	public int ModuleAmount = 6;

	// Token: 0x04000E89 RID: 3721
	public int PassageMaxAmount = 2;

	// Token: 0x04000E8A RID: 3722
	[Space]
	public bool HasEnemies = true;

	// Token: 0x04000E8B RID: 3723
	[Space]
	public GameObject ConnectObject;

	// Token: 0x04000E8C RID: 3724
	public GameObject BlockObject;

	// Token: 0x04000E8D RID: 3725
	public List<GameObject> StartRooms;

	// Token: 0x04000E8E RID: 3726
	[Space]
	public Sprite LoadingGraphic01;

	// Token: 0x04000E8F RID: 3727
	public Sprite LoadingGraphic02;

	// Token: 0x04000E90 RID: 3728
	public Sprite LoadingGraphic03;

	// Token: 0x04000E91 RID: 3729
	[Header("Difficulty 1")]
	public List<GameObject> ModulesNormal1;

	// Token: 0x04000E92 RID: 3730
	public List<GameObject> ModulesPassage1;

	// Token: 0x04000E93 RID: 3731
	public List<GameObject> ModulesDeadEnd1;

	// Token: 0x04000E94 RID: 3732
	public List<GameObject> ModulesExtraction1;

	// Token: 0x04000E95 RID: 3733
	[Space]
	[Header("Difficulty 2")]
	public List<GameObject> ModulesNormal2;

	// Token: 0x04000E96 RID: 3734
	public List<GameObject> ModulesPassage2;

	// Token: 0x04000E97 RID: 3735
	public List<GameObject> ModulesDeadEnd2;

	// Token: 0x04000E98 RID: 3736
	public List<GameObject> ModulesExtraction2;

	// Token: 0x04000E99 RID: 3737
	[Space]
	[Header("Difficulty 3")]
	public List<GameObject> ModulesNormal3;

	// Token: 0x04000E9A RID: 3738
	public List<GameObject> ModulesPassage3;

	// Token: 0x04000E9B RID: 3739
	public List<GameObject> ModulesDeadEnd3;

	// Token: 0x04000E9C RID: 3740
	public List<GameObject> ModulesExtraction3;

	// Token: 0x04000E9D RID: 3741
	public List<LevelValuables> ValuablePresets;

	// Token: 0x04000E9E RID: 3742
	public LevelMusicAsset MusicPreset;

	// Token: 0x04000E9F RID: 3743
	public ConstantMusicAsset ConstantMusicPreset;

	// Token: 0x04000EA0 RID: 3744
	public List<LevelAmbience> AmbiencePresets;

	// Token: 0x04000EA1 RID: 3745
	[Header("Fog")]
	public Color FogColor = Color.black;

	// Token: 0x04000EA2 RID: 3746
	public float FogStartDistance;

	// Token: 0x04000EA3 RID: 3747
	public float FogEndDistance = 15f;

	// Token: 0x04000EA4 RID: 3748
	[Space(20f)]
	[Header("Environment")]
	public Color AmbientColor = new Color(0f, 0f, 0.2f);

	// Token: 0x04000EA5 RID: 3749
	public Color AmbientColorAdaptation = new Color(0.06f, 0.06f, 0.39f);

	// Token: 0x04000EA6 RID: 3750
	[Space(20f)]
	[Header("Color")]
	public float ColorTemperature;

	// Token: 0x04000EA7 RID: 3751
	public Color ColorFilter = Color.white;

	// Token: 0x04000EA8 RID: 3752
	[Space(20f)]
	[Header("Bloom")]
	public float BloomIntensity = 10f;

	// Token: 0x04000EA9 RID: 3753
	public float BloomThreshold = 0.9f;

	// Token: 0x04000EAA RID: 3754
	[Space(20f)]
	[Header("Vignette")]
	public Color VignetteColor = new Color(0.02f, 0f, 0.22f, 0f);

	// Token: 0x04000EAB RID: 3755
	[Range(0f, 1f)]
	public float VignetteIntensity = 0.4f;

	// Token: 0x04000EAC RID: 3756
	[Range(0f, 1f)]
	public float VignetteSmoothness = 0.7f;
}
