using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class ValuableDirector : MonoBehaviour
{
	// Token: 0x06001424 RID: 5156 RVA: 0x000B0E90 File Offset: 0x000AF090
	private void Awake()
	{
		ValuableDirector.instance = this;
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x000B0EA4 File Offset: 0x000AF0A4
	private void Start()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			base.StartCoroutine(this.SetupClient());
		}
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x000B0EC7 File Offset: 0x000AF0C7
	public IEnumerator SetupClient()
	{
		while (this.valuableTargetAmount == -1)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (this.valuableSpawnAmount < this.valuableTargetAmount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.PhotonView.RPC("PlayerReadyRPC", RpcTarget.All, Array.Empty<object>());
		yield break;
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x000B0ED6 File Offset: 0x000AF0D6
	public IEnumerator SetupHost()
	{
		float time = SemiFunc.RunGetDifficultyMultiplier();
		if (SemiFunc.RunIsArena())
		{
			time = 0.75f;
		}
		this.totalMaxAmount = Mathf.RoundToInt(this.totalMaxAmountCurve.Evaluate(time));
		this.tinyMaxAmount = Mathf.RoundToInt(this.tinyMaxAmountCurve.Evaluate(time));
		this.smallMaxAmount = Mathf.RoundToInt(this.smallMaxAmountCurve.Evaluate(time));
		this.mediumMaxAmount = Mathf.RoundToInt(this.mediumMaxAmountCurve.Evaluate(time));
		this.bigMaxAmount = Mathf.RoundToInt(this.bigMaxAmountCurve.Evaluate(time));
		this.wideMaxAmount = Mathf.RoundToInt(this.wideMaxAmountCurve.Evaluate(time));
		this.tallMaxAmount = Mathf.RoundToInt(this.tallMaxAmountCurve.Evaluate(time));
		this.veryTallMaxAmount = Mathf.RoundToInt(this.veryTallMaxAmountCurve.Evaluate(time));
		if (SemiFunc.RunIsArena())
		{
			this.totalMaxAmount /= 2;
			this.tinyMaxAmount /= 3;
			this.smallMaxAmount /= 3;
			this.mediumMaxAmount /= 3;
			this.bigMaxAmount /= 3;
			this.wideMaxAmount /= 2;
			this.tallMaxAmount /= 2;
			this.veryTallMaxAmount /= 2;
		}
		foreach (LevelValuables levelValuables in LevelGenerator.Instance.Level.ValuablePresets)
		{
			this.tinyValuables.AddRange(levelValuables.tiny);
			this.smallValuables.AddRange(levelValuables.small);
			this.mediumValuables.AddRange(levelValuables.medium);
			this.bigValuables.AddRange(levelValuables.big);
			this.wideValuables.AddRange(levelValuables.wide);
			this.tallValuables.AddRange(levelValuables.tall);
			this.veryTallValuables.AddRange(levelValuables.veryTall);
		}
		List<ValuableVolume> list = Object.FindObjectsOfType<ValuableVolume>(false).ToList<ValuableVolume>();
		this.tinyVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tiny);
		this.tinyVolumes.Shuffle<ValuableVolume>();
		this.smallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Small);
		this.smallVolumes.Shuffle<ValuableVolume>();
		this.mediumVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Medium);
		this.mediumVolumes.Shuffle<ValuableVolume>();
		this.bigVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Big);
		this.bigVolumes.Shuffle<ValuableVolume>();
		this.wideVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Wide);
		this.wideVolumes.Shuffle<ValuableVolume>();
		this.tallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tall);
		this.tallVolumes.Shuffle<ValuableVolume>();
		this.veryTallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.VeryTall);
		this.veryTallVolumes.Shuffle<ValuableVolume>();
		if (this.valuableDebug == ValuableDirector.ValuableDebug.All)
		{
			this.totalMaxAmount = list.Count;
			this.tinyMaxAmount = this.tinyVolumes.Count;
			this.smallMaxAmount = this.smallVolumes.Count;
			this.mediumMaxAmount = this.mediumVolumes.Count;
			this.bigMaxAmount = this.bigVolumes.Count;
			this.wideMaxAmount = this.wideVolumes.Count;
			this.tallMaxAmount = this.tallVolumes.Count;
			this.veryTallMaxAmount = this.veryTallVolumes.Count;
		}
		if (this.valuableDebug == ValuableDirector.ValuableDebug.None || LevelGenerator.Instance.Level.ValuablePresets.Count <= 0)
		{
			this.totalMaxAmount = 0;
			this.tinyMaxAmount = 0;
			this.smallMaxAmount = 0;
			this.mediumMaxAmount = 0;
			this.bigMaxAmount = 0;
			this.wideMaxAmount = 0;
			this.tallMaxAmount = 0;
			this.veryTallMaxAmount = 0;
		}
		this.valuableTargetAmount = 0;
		string[] _names = new string[]
		{
			"Tiny",
			"Small",
			"Medium",
			"Big",
			"Wide",
			"Tall",
			"Very Tall"
		};
		int[] _maxAmount = new int[]
		{
			this.tinyMaxAmount,
			this.smallMaxAmount,
			this.mediumMaxAmount,
			this.bigMaxAmount,
			this.wideMaxAmount,
			this.tallMaxAmount,
			this.veryTallMaxAmount
		};
		List<ValuableVolume>[] _volumes = new List<ValuableVolume>[]
		{
			this.tinyVolumes,
			this.smallVolumes,
			this.mediumVolumes,
			this.bigVolumes,
			this.wideVolumes,
			this.tallVolumes,
			this.veryTallVolumes
		};
		string[] _path = new string[]
		{
			this.tinyPath,
			this.smallPath,
			this.mediumPath,
			this.bigPath,
			this.widePath,
			this.tallPath,
			this.veryTallPath
		};
		int[] _chance = new int[]
		{
			this.tinyChance,
			this.smallChance,
			this.mediumChance,
			this.bigChance,
			this.wideChance,
			this.tallChance,
			this.veryTallChance
		};
		List<GameObject>[] _valuables = new List<GameObject>[]
		{
			this.tinyValuables,
			this.smallValuables,
			this.mediumValuables,
			this.bigValuables,
			this.wideValuables,
			this.tallValuables,
			this.veryTallValuables
		};
		int[] _volumeIndex = new int[7];
		int num4;
		for (int _i = 0; _i < this.totalMaxAmount; _i = num4 + 1)
		{
			float num = -1f;
			int num2 = -1;
			for (int i = 0; i < _names.Length; i++)
			{
				if (_volumeIndex[i] < _maxAmount[i] && _volumeIndex[i] < _volumes[i].Count)
				{
					int num3 = Random.Range(0, _chance[i]);
					if ((float)num3 > num)
					{
						num = (float)num3;
						num2 = i;
					}
				}
			}
			if (num2 == -1)
			{
				break;
			}
			ValuableVolume volume = _volumes[num2][_volumeIndex[num2]];
			GameObject valuable = _valuables[num2][Random.Range(0, _valuables[num2].Count)];
			this.Spawn(valuable, volume, _path[num2]);
			_volumeIndex[num2]++;
			yield return null;
			num4 = _i;
		}
		if (this.valuableTargetAmount < this.totalMaxAmount && DebugComputerCheck.instance && (!DebugComputerCheck.instance.enabled || !DebugComputerCheck.instance.LevelDebug || !DebugComputerCheck.instance.ModuleOverrideActive || !DebugComputerCheck.instance.ModuleOverride))
		{
			for (int j = 0; j < _names.Length; j++)
			{
				if (_volumeIndex[j] < _maxAmount[j])
				{
					Debug.LogError("Could not spawn enough ''" + _names[j] + "'' valuables!");
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			this.PhotonView.RPC("ValuablesTargetSetRPC", RpcTarget.All, new object[]
			{
				this.valuableTargetAmount
			});
		}
		this.valuableSpawnPlayerReady++;
		while (GameManager.instance.gameMode == 1 && this.valuableSpawnPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.VolumesAndSwitchSetup();
		while (GameManager.instance.gameMode == 1 && this.switchSetupPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.setupComplete = true;
		yield break;
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x000B0EE8 File Offset: 0x000AF0E8
	private void Spawn(GameObject _valuable, ValuableVolume _volume, string _path)
	{
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(_valuable, _volume.transform.position, _volume.transform.rotation);
		}
		else
		{
			PhotonNetwork.InstantiateRoomObject(this.resourcePath + _path + "/" + _valuable.name, _volume.transform.position, _volume.transform.rotation, 0, null);
		}
		this.valuableTargetAmount++;
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x000B0F63 File Offset: 0x000AF163
	[PunRPC]
	private void ValuablesTargetSetRPC(int _amount)
	{
		this.valuableTargetAmount = _amount;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x000B0F6C File Offset: 0x000AF16C
	[PunRPC]
	private void PlayerReadyRPC()
	{
		this.valuableSpawnPlayerReady++;
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x000B0F7C File Offset: 0x000AF17C
	public void VolumesAndSwitchSetup()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.VolumesAndSwitchSetupRPC();
			return;
		}
		this.PhotonView.RPC("VolumesAndSwitchSetupRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x000B0FA8 File Offset: 0x000AF1A8
	[PunRPC]
	private void VolumesAndSwitchSetupRPC()
	{
		ValuableVolume[] array = Object.FindObjectsOfType<ValuableVolume>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup();
		}
		ValuablePropSwitch[] array2 = Object.FindObjectsOfType<ValuablePropSwitch>(true);
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Setup();
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.VolumesAndSwitchReadyRPC();
			return;
		}
		this.PhotonView.RPC("VolumesAndSwitchReadyRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x000B1018 File Offset: 0x000AF218
	[PunRPC]
	private void VolumesAndSwitchReadyRPC()
	{
		this.switchSetupPlayerReady++;
	}

	// Token: 0x0400224F RID: 8783
	public static ValuableDirector instance;

	// Token: 0x04002250 RID: 8784
	private PhotonView PhotonView;

	// Token: 0x04002251 RID: 8785
	internal ValuableDirector.ValuableDebug valuableDebug;

	// Token: 0x04002252 RID: 8786
	[HideInInspector]
	public bool setupComplete;

	// Token: 0x04002253 RID: 8787
	[HideInInspector]
	public bool valuablesSpawned;

	// Token: 0x04002254 RID: 8788
	internal int valuableSpawnPlayerReady;

	// Token: 0x04002255 RID: 8789
	internal int valuableSpawnAmount;

	// Token: 0x04002256 RID: 8790
	internal int valuableTargetAmount = -1;

	// Token: 0x04002257 RID: 8791
	internal int switchSetupPlayerReady;

	// Token: 0x04002258 RID: 8792
	private string resourcePath = "Valuables/";

	// Token: 0x04002259 RID: 8793
	[Space(20f)]
	public AnimationCurve totalMaxAmountCurve;

	// Token: 0x0400225A RID: 8794
	private int totalMaxAmount;

	// Token: 0x0400225B RID: 8795
	[Space(20f)]
	public AnimationCurve tinyMaxAmountCurve;

	// Token: 0x0400225C RID: 8796
	public int tinyChance;

	// Token: 0x0400225D RID: 8797
	private int tinyMaxAmount;

	// Token: 0x0400225E RID: 8798
	private string tinyPath = "01 Tiny";

	// Token: 0x0400225F RID: 8799
	private List<GameObject> tinyValuables = new List<GameObject>();

	// Token: 0x04002260 RID: 8800
	private List<ValuableVolume> tinyVolumes = new List<ValuableVolume>();

	// Token: 0x04002261 RID: 8801
	[Space]
	public AnimationCurve smallMaxAmountCurve;

	// Token: 0x04002262 RID: 8802
	public int smallChance;

	// Token: 0x04002263 RID: 8803
	private int smallMaxAmount;

	// Token: 0x04002264 RID: 8804
	private string smallPath = "02 Small";

	// Token: 0x04002265 RID: 8805
	private List<GameObject> smallValuables = new List<GameObject>();

	// Token: 0x04002266 RID: 8806
	private List<ValuableVolume> smallVolumes = new List<ValuableVolume>();

	// Token: 0x04002267 RID: 8807
	[Space]
	public AnimationCurve mediumMaxAmountCurve;

	// Token: 0x04002268 RID: 8808
	public int mediumChance;

	// Token: 0x04002269 RID: 8809
	private int mediumMaxAmount;

	// Token: 0x0400226A RID: 8810
	private string mediumPath = "03 Medium";

	// Token: 0x0400226B RID: 8811
	private List<GameObject> mediumValuables = new List<GameObject>();

	// Token: 0x0400226C RID: 8812
	private List<ValuableVolume> mediumVolumes = new List<ValuableVolume>();

	// Token: 0x0400226D RID: 8813
	[Space]
	public AnimationCurve bigMaxAmountCurve;

	// Token: 0x0400226E RID: 8814
	public int bigChance;

	// Token: 0x0400226F RID: 8815
	private int bigMaxAmount;

	// Token: 0x04002270 RID: 8816
	private string bigPath = "04 Big";

	// Token: 0x04002271 RID: 8817
	private List<GameObject> bigValuables = new List<GameObject>();

	// Token: 0x04002272 RID: 8818
	private List<ValuableVolume> bigVolumes = new List<ValuableVolume>();

	// Token: 0x04002273 RID: 8819
	[Space]
	public AnimationCurve wideMaxAmountCurve;

	// Token: 0x04002274 RID: 8820
	public int wideChance;

	// Token: 0x04002275 RID: 8821
	private int wideMaxAmount;

	// Token: 0x04002276 RID: 8822
	private string widePath = "05 Wide";

	// Token: 0x04002277 RID: 8823
	private List<GameObject> wideValuables = new List<GameObject>();

	// Token: 0x04002278 RID: 8824
	private List<ValuableVolume> wideVolumes = new List<ValuableVolume>();

	// Token: 0x04002279 RID: 8825
	[Space]
	public AnimationCurve tallMaxAmountCurve;

	// Token: 0x0400227A RID: 8826
	public int tallChance;

	// Token: 0x0400227B RID: 8827
	private int tallMaxAmount;

	// Token: 0x0400227C RID: 8828
	private string tallPath = "06 Tall";

	// Token: 0x0400227D RID: 8829
	private List<GameObject> tallValuables = new List<GameObject>();

	// Token: 0x0400227E RID: 8830
	private List<ValuableVolume> tallVolumes = new List<ValuableVolume>();

	// Token: 0x0400227F RID: 8831
	[Space]
	public AnimationCurve veryTallMaxAmountCurve;

	// Token: 0x04002280 RID: 8832
	public int veryTallChance;

	// Token: 0x04002281 RID: 8833
	private int veryTallMaxAmount;

	// Token: 0x04002282 RID: 8834
	private string veryTallPath = "07 Very Tall";

	// Token: 0x04002283 RID: 8835
	private List<GameObject> veryTallValuables = new List<GameObject>();

	// Token: 0x04002284 RID: 8836
	private List<ValuableVolume> veryTallVolumes = new List<ValuableVolume>();

	// Token: 0x04002285 RID: 8837
	[Space(20f)]
	public List<ValuableObject> valuableList = new List<ValuableObject>();

	// Token: 0x020003C7 RID: 967
	public enum ValuableDebug
	{
		// Token: 0x040028F4 RID: 10484
		Normal,
		// Token: 0x040028F5 RID: 10485
		All,
		// Token: 0x040028F6 RID: 10486
		None
	}
}
