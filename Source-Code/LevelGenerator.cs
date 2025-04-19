using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.AI.Navigation;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class LevelGenerator : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060007DD RID: 2013 RVA: 0x0004CF1B File Offset: 0x0004B11B
	private void Awake()
	{
		LevelGenerator.Instance = this;
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0004CF2F File Offset: 0x0004B12F
	private void Start()
	{
		base.StartCoroutine(this.Generate());
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004CF3E File Offset: 0x0004B13E
	private IEnumerator Generate()
	{
		yield return new WaitForSeconds(0.2f);
		if (!SemiFunc.IsMultiplayer())
		{
			this.AllPlayersReady = true;
		}
		while (!this.AllPlayersReady)
		{
			this.State = LevelGenerator.LevelState.Load;
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.2f);
		this.Level = RunManager.instance.levelCurrent;
		RunManager.instance.levelPrevious = this.Level;
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.ModuleAmount = this.Level.ModuleAmount;
			if (this.DebugAmount > 0)
			{
				this.ModuleAmount = this.DebugAmount;
			}
			base.StartCoroutine(this.TileGeneration());
			while (this.waitingForSubCoroutine)
			{
				yield return null;
			}
			base.StartCoroutine(this.StartRoomGeneration());
			while (this.waitingForSubCoroutine)
			{
				yield return null;
			}
			base.StartCoroutine(this.GenerateConnectObjects());
			while (this.waitingForSubCoroutine)
			{
				yield return null;
			}
			base.StartCoroutine(this.ModuleGeneration());
			while (this.waitingForSubCoroutine)
			{
				yield return null;
			}
			base.StartCoroutine(this.GenerateBlockObjects());
			while (this.waitingForSubCoroutine)
			{
				yield return null;
			}
			if (GameManager.instance.gameMode == 1)
			{
				this.PhotonView.RPC("ModuleAmountRPC", RpcTarget.AllBuffered, new object[]
				{
					this.ModuleAmount
				});
			}
		}
		while (this.ModulesSpawned < this.ModuleAmount - 1)
		{
			this.State = LevelGenerator.LevelState.ModuleSpawnLocal;
			yield return new WaitForSeconds(0.1f);
		}
		if (GameManager.instance.gameMode == 1)
		{
			this.PhotonView.RPC("ModulesReadyRPC", RpcTarget.AllBuffered, Array.Empty<object>());
		}
		while (GameManager.instance.gameMode == 1 && this.ModulesReadyPlayers < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			this.State = LevelGenerator.LevelState.ModuleSpawnRemote;
			yield return new WaitForSeconds(0.1f);
		}
		EnvironmentDirector.Instance.Setup();
		PostProcessing.Instance.Setup();
		LevelMusic.instance.Setup();
		ConstantMusic.instance.Setup();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			while (this.LevelPathPoints.Count == 0)
			{
				this.State = LevelGenerator.LevelState.LevelPoint;
				yield return new WaitForSeconds(0.1f);
			}
			this.State = LevelGenerator.LevelState.Item;
			if (!SemiFunc.IsMultiplayer())
			{
				this.ItemSetup();
			}
			else
			{
				this.PhotonView.RPC("ItemSetup", RpcTarget.AllBuffered, Array.Empty<object>());
			}
			base.StartCoroutine(ValuableDirector.instance.SetupHost());
			while (!ValuableDirector.instance.setupComplete)
			{
				this.State = LevelGenerator.LevelState.Valuable;
				yield return new WaitForSeconds(0.1f);
			}
			this.NavMeshSetup();
			yield return null;
			while (GameDirector.instance.PlayerList.Count == 0)
			{
				this.State = LevelGenerator.LevelState.PlayerSetup;
				yield return new WaitForSeconds(0.1f);
			}
			this.PlayerSpawn();
			yield return null;
			while (this.playerSpawned < GameDirector.instance.PlayerList.Count)
			{
				this.State = LevelGenerator.LevelState.PlayerSpawn;
				yield return new WaitForSeconds(0.1f);
			}
			if (this.Level.HasEnemies && !this.DebugNoEnemy)
			{
				this.EnemySetup();
				yield return null;
				if (GameManager.Multiplayer())
				{
					while (!this.EnemyReady)
					{
						this.State = LevelGenerator.LevelState.Enemy;
						if (this.EnemyReadyPlayers >= PhotonNetwork.CurrentRoom.PlayerCount || this.EnemiesSpawnTarget <= 0)
						{
							this.PhotonView.RPC("EnemyReadyAllRPC", RpcTarget.AllBuffered, Array.Empty<object>());
							this.EnemyReady = true;
						}
						yield return new WaitForSeconds(0.1f);
					}
				}
				else
				{
					while (this.EnemiesSpawned < this.EnemiesSpawnTarget)
					{
						yield return new WaitForSeconds(0.1f);
					}
				}
			}
			this.State = LevelGenerator.LevelState.Done;
			if (!SemiFunc.IsMultiplayer())
			{
				this.GenerateDone();
			}
			else
			{
				this.PhotonView.RPC("GenerateDone", RpcTarget.AllBuffered, Array.Empty<object>());
			}
			SessionManager.instance.CrownPlayer();
		}
		else
		{
			while (!this.Generated)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield break;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004CF50 File Offset: 0x0004B150
	public void PlayerSpawn()
	{
		List<SpawnPoint> list = Object.FindObjectsOfType<SpawnPoint>().ToList<SpawnPoint>();
		list.Shuffle<SpawnPoint>();
		List<SpawnPoint> list2 = new List<SpawnPoint>();
		bool flag = false;
		foreach (SpawnPoint spawnPoint in list)
		{
			if (spawnPoint.debug)
			{
				list2.Add(spawnPoint);
				flag = true;
			}
		}
		if (flag)
		{
			int num = 0;
			using (List<PlayerAvatar>.Enumerator enumerator2 = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					PlayerAvatar playerAvatar = enumerator2.Current;
					playerAvatar.Spawn(list2[num].transform.position, list2[num].transform.rotation);
					num++;
					if (num >= list2.Count)
					{
						num = 0;
					}
				}
				goto IL_141;
			}
		}
		int num2 = 0;
		foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
		{
			playerAvatar2.Spawn(list[num2].transform.position, list[num2].transform.rotation);
			num2++;
			if (num2 >= list.Count)
			{
				num2 = 0;
			}
		}
		IL_141:
		if (!SemiFunc.MenuLevel())
		{
			foreach (PlayerAvatar playerAvatar3 in GameDirector.instance.PlayerList)
			{
				if (!playerAvatar3.playerDeathHead)
				{
					GameObject gameObject;
					if (!GameManager.Multiplayer())
					{
						gameObject = Object.Instantiate<GameObject>(this.PlayerDeathHeadPrefab, new Vector3(0f, 3000f, 0f), Quaternion.identity);
					}
					else
					{
						gameObject = PhotonNetwork.Instantiate(this.PlayerDeathHeadPrefab.name, new Vector3(0f, 3000f, 0f), Quaternion.identity, 0, null);
					}
					PlayerDeathHead component = gameObject.GetComponent<PlayerDeathHead>();
					component.playerAvatar = playerAvatar3;
					component.playerAvatar.playerDeathHead = component;
				}
				if (!playerAvatar3.tumble)
				{
					GameObject gameObject2;
					if (!GameManager.Multiplayer())
					{
						gameObject2 = Object.Instantiate<GameObject>(this.PlayerTumblePrefab, new Vector3(0f, 3000f, 0f), Quaternion.identity);
					}
					else
					{
						gameObject2 = PhotonNetwork.Instantiate(this.PlayerTumblePrefab.name, new Vector3(0f, 3000f, 0f), Quaternion.identity, 0, null);
					}
					PlayerTumble component2 = gameObject2.GetComponent<PlayerTumble>();
					component2.playerAvatar = playerAvatar3;
					component2.playerAvatar.tumble = component2;
				}
			}
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004D25C File Offset: 0x0004B45C
	public void NavMeshSetup()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.NavMeshSetupRPC();
			return;
		}
		this.PhotonView.RPC("NavMeshSetupRPC", RpcTarget.AllBuffered, Array.Empty<object>());
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004D288 File Offset: 0x0004B488
	public void MeshColliderFinder()
	{
		List<MeshCollider> list = new List<MeshCollider>();
		Debug.LogWarning("");
		Debug.LogWarning("Mesh Colliders:");
		foreach (MeshCollider meshCollider in Object.FindObjectsOfType<MeshCollider>())
		{
			bool flag = false;
			using (List<MeshCollider>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.sharedMesh == meshCollider.sharedMesh)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add(meshCollider);
			}
		}
		foreach (MeshCollider meshCollider2 in list)
		{
			Debug.LogWarning("    " + meshCollider2.sharedMesh.name, meshCollider2.gameObject);
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0004D380 File Offset: 0x0004B580
	[PunRPC]
	public void ItemSetup()
	{
		if (!SemiFunc.RunIsArena())
		{
			ShopManager.instance.ShopInitialize();
			ItemManager.instance.ItemsInitialize();
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0004D3A0 File Offset: 0x0004B5A0
	[PunRPC]
	private void NavMeshSetupRPC()
	{
		this.NavMeshSurface = base.GetComponent<NavMeshSurface>();
		this.NavMeshSurface.RemoveData();
		this.NavMeshSurface.BuildNavMesh();
		base.transform.localPosition = new Vector3(0f, 0.001f, 0f);
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004D3EE File Offset: 0x0004B5EE
	private IEnumerator StartRoomGeneration()
	{
		this.waitingForSubCoroutine = true;
		this.State = LevelGenerator.LevelState.StartRoom;
		List<GameObject> list = new List<GameObject>();
		list.AddRange(this.Level.StartRooms);
		list.Shuffle<GameObject>();
		if (this.DebugStartRoom)
		{
			list[0] = this.DebugStartRoom;
		}
		GameObject gameObject;
		if (GameManager.instance.gameMode == 0)
		{
			gameObject = Object.Instantiate<GameObject>(list[0], Vector3.zero, Quaternion.identity);
		}
		else
		{
			gameObject = PhotonNetwork.InstantiateRoomObject(string.Concat(new string[]
			{
				this.ResourceParent,
				"/",
				this.Level.ResourcePath,
				"/",
				this.ResourceStart,
				"/",
				list[0].name
			}), Vector3.zero, Quaternion.identity, 0, null);
		}
		gameObject.transform.parent = this.LevelParent.transform;
		yield return null;
		this.waitingForSubCoroutine = false;
		yield break;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004D3FD File Offset: 0x0004B5FD
	private IEnumerator TileGeneration()
	{
		this.waitingForSubCoroutine = true;
		this.State = LevelGenerator.LevelState.Tiles;
		this.LevelWidth = Mathf.Max(2, Mathf.CeilToInt((float)this.LevelWidth * this.DebugLevelSize));
		this.LevelHeight = Mathf.Max(2, Mathf.CeilToInt((float)this.LevelHeight * this.DebugLevelSize));
		this.LevelGrid = new LevelGenerator.Tile[this.LevelWidth, this.LevelHeight];
		for (int i = 0; i < this.LevelWidth; i++)
		{
			for (int j = 0; j < this.LevelHeight; j++)
			{
				this.LevelGrid[i, j] = new LevelGenerator.Tile
				{
					x = i,
					y = j,
					active = false
				};
			}
		}
		this.ExtractionAmount = 0;
		if (this.ModuleAmount > 4)
		{
			this.ModuleAmount = Mathf.Min(5 + RunManager.instance.levelsCompleted, 10);
			this.ModuleAmount = Mathf.CeilToInt((float)this.ModuleAmount * this.DebugLevelSize);
			if (!this.DebugModule)
			{
				this.DeadEndAmount = Mathf.CeilToInt((float)(this.ModuleAmount / 3));
				if (this.ModuleAmount >= 10)
				{
					this.ExtractionAmount = 3;
				}
				else if (this.ModuleAmount >= 8)
				{
					this.ExtractionAmount = 2;
				}
				else if (this.ModuleAmount >= 6)
				{
					this.ExtractionAmount = 1;
				}
				else
				{
					this.ExtractionAmount = 0;
				}
			}
		}
		if (this.Level == RunManager.instance.levelShop)
		{
			this.DeadEndAmount = 1;
		}
		int k = this.ModuleAmount;
		this.LevelGrid[this.LevelWidth / 2, 0].active = true;
		this.LevelGrid[this.LevelWidth / 2, 0].first = true;
		k--;
		int num = this.LevelWidth / 2;
		int num2 = 0;
		while (k > 0)
		{
			int num3 = -999;
			int num4 = -999;
			while (num + num3 < 0 || num + num3 >= this.LevelWidth || num2 + num4 < 0 || num2 + num4 >= this.LevelHeight)
			{
				num3 = 0;
				num4 = 0;
				int num5 = Random.Range(0, 4);
				if (num2 == 1)
				{
					num5 = Random.Range(0, 3);
				}
				if (this.DebugPassage)
				{
					num5 = 2;
				}
				switch (num5)
				{
				case 0:
					num3--;
					break;
				case 1:
					num3++;
					break;
				case 2:
					num4++;
					break;
				case 3:
					num4--;
					break;
				}
			}
			num += num3;
			num2 += num4;
			if (!this.LevelGrid[num, num2].active)
			{
				this.LevelGrid[num, num2].active = true;
				k--;
			}
		}
		yield return null;
		List<LevelGenerator.Tile> possibleExtractionTiles = new List<LevelGenerator.Tile>();
		if (!this.DebugNormal && !this.DebugPassage && !this.DebugDeadEnd)
		{
			for (int l = 0; l < this.LevelWidth; l++)
			{
				for (int m = 0; m < this.LevelHeight; m++)
				{
					if (!this.LevelGrid[l, m].active)
					{
						int num6 = 0;
						LevelGenerator.Tile tile = this.GridGetTile(l, m + 1);
						if (tile != null && tile.active)
						{
							num6++;
						}
						LevelGenerator.Tile tile2 = this.GridGetTile(l + 1, m);
						if (tile2 != null && tile2.active)
						{
							num6++;
						}
						LevelGenerator.Tile tile3 = this.GridGetTile(l, m - 1);
						if (tile3 != null && tile3.active)
						{
							num6++;
						}
						LevelGenerator.Tile tile4 = this.GridGetTile(l - 1, m);
						if (tile4 != null && tile4.active)
						{
							num6++;
						}
						if (num6 == 1)
						{
							possibleExtractionTiles.Add(this.LevelGrid[l, m]);
						}
					}
				}
			}
		}
		yield return null;
		int num7 = this.ExtractionAmount;
		LevelGenerator.Tile tile5 = new LevelGenerator.Tile();
		tile5.x = this.LevelWidth / 2;
		tile5.y = -1;
		List<LevelGenerator.Tile> _extractionTiles = new List<LevelGenerator.Tile>();
		_extractionTiles.Add(tile5);
		while (num7 > 0 && possibleExtractionTiles.Count > 0)
		{
			LevelGenerator.Tile tile6 = null;
			float num8 = 0f;
			foreach (LevelGenerator.Tile tile7 in possibleExtractionTiles)
			{
				float num9 = 9999999f;
				foreach (LevelGenerator.Tile tile8 in _extractionTiles)
				{
					float num10 = Vector2.Distance(new Vector2((float)tile8.x, (float)tile8.y), new Vector2((float)tile7.x, (float)tile7.y));
					if (num10 < num9)
					{
						num9 = num10;
					}
				}
				if (num9 > num8)
				{
					num8 = num9;
					tile6 = tile7;
				}
			}
			this.SetExtractionTile(Module.Type.Extraction, tile6, ref _extractionTiles, ref possibleExtractionTiles);
			num7--;
		}
		yield return null;
		int num11 = this.DeadEndAmount;
		while (num11 > 0 && possibleExtractionTiles.Count > 0)
		{
			LevelGenerator.Tile tile9 = possibleExtractionTiles[Random.Range(0, possibleExtractionTiles.Count)];
			this.SetExtractionTile(Module.Type.DeadEnd, tile9, ref _extractionTiles, ref possibleExtractionTiles);
			num11--;
		}
		yield return null;
		this.waitingForSubCoroutine = false;
		yield break;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004D40C File Offset: 0x0004B60C
	private void SetExtractionTile(Module.Type _type, LevelGenerator.Tile _tile, ref List<LevelGenerator.Tile> _extractionTiles, ref List<LevelGenerator.Tile> _possibleExtractionTiles)
	{
		_tile.type = _type;
		_tile.active = true;
		_extractionTiles.Add(_tile);
		_possibleExtractionTiles.Remove(_tile);
		bool flag = false;
		while (!flag)
		{
			flag = true;
			foreach (LevelGenerator.Tile tile in _possibleExtractionTiles)
			{
				if (this.GridGetTile(tile.x, tile.y - 1) == _tile || this.GridGetTile(tile.x + 1, tile.y) == _tile || this.GridGetTile(tile.x, tile.y + 1) == _tile || this.GridGetTile(tile.x - 1, tile.y) == _tile)
				{
					_possibleExtractionTiles.Remove(tile);
					flag = false;
					break;
				}
			}
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004D4F0 File Offset: 0x0004B6F0
	private IEnumerator ModuleGeneration()
	{
		this.waitingForSubCoroutine = true;
		this.State = LevelGenerator.LevelState.ModuleGeneration;
		this.ModulesNormalShuffled_1 = new List<GameObject>();
		this.ModulesNormalShuffled_2 = new List<GameObject>();
		this.ModulesNormalShuffled_3 = new List<GameObject>();
		this.ModulesPassageShuffled_1 = new List<GameObject>();
		this.ModulesPassageShuffled_2 = new List<GameObject>();
		this.ModulesPassageShuffled_3 = new List<GameObject>();
		this.ModulesDeadEndShuffled_1 = new List<GameObject>();
		this.ModulesDeadEndShuffled_2 = new List<GameObject>();
		this.ModulesDeadEndShuffled_3 = new List<GameObject>();
		this.ModulesExtractionShuffled_1 = new List<GameObject>();
		this.ModulesExtractionShuffled_2 = new List<GameObject>();
		this.ModulesExtractionShuffled_3 = new List<GameObject>();
		this.ModuleRarity1 = this.DifficultyCurve1.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.ModuleRarity2 = this.DifficultyCurve2.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.ModuleRarity3 = this.DifficultyCurve3.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		if (!this.DebugModule)
		{
			this.ModulesNormalShuffled_1.AddRange(this.Level.ModulesNormal1);
			this.ModulesNormalShuffled_1.Shuffle<GameObject>();
			this.ModulesNormalShuffled_2.AddRange(this.Level.ModulesNormal2);
			this.ModulesNormalShuffled_2.Shuffle<GameObject>();
			this.ModulesNormalShuffled_3.AddRange(this.Level.ModulesNormal3);
			this.ModulesNormalShuffled_3.Shuffle<GameObject>();
			this.ModulesPassageShuffled_1.AddRange(this.Level.ModulesPassage1);
			this.ModulesPassageShuffled_1.Shuffle<GameObject>();
			this.ModulesPassageShuffled_2.AddRange(this.Level.ModulesPassage2);
			this.ModulesPassageShuffled_2.Shuffle<GameObject>();
			this.ModulesPassageShuffled_3.AddRange(this.Level.ModulesPassage3);
			this.ModulesPassageShuffled_3.Shuffle<GameObject>();
			this.ModulesDeadEndShuffled_1.AddRange(this.Level.ModulesDeadEnd1);
			this.ModulesDeadEndShuffled_1.Shuffle<GameObject>();
			this.ModulesDeadEndShuffled_2.AddRange(this.Level.ModulesDeadEnd2);
			this.ModulesDeadEndShuffled_2.Shuffle<GameObject>();
			this.ModulesDeadEndShuffled_3.AddRange(this.Level.ModulesDeadEnd3);
			this.ModulesDeadEndShuffled_3.Shuffle<GameObject>();
			this.ModulesExtractionShuffled_1.AddRange(this.Level.ModulesExtraction1);
			this.ModulesExtractionShuffled_1.Shuffle<GameObject>();
			this.ModulesExtractionShuffled_2.AddRange(this.Level.ModulesExtraction2);
			this.ModulesExtractionShuffled_2.Shuffle<GameObject>();
			this.ModulesExtractionShuffled_3.AddRange(this.Level.ModulesExtraction3);
			this.ModulesExtractionShuffled_3.Shuffle<GameObject>();
		}
		else
		{
			this.ModulesNormalShuffled_1.Add(this.DebugModule);
			this.ModulesPassageShuffled_1.Add(this.DebugModule);
			this.ModulesDeadEndShuffled_1.Add(this.DebugModule);
			this.ModulesExtractionShuffled_1.Add(this.DebugModule);
		}
		if (this.ModulesNormalShuffled_1.Count == 0)
		{
			this.waitingForSubCoroutine = false;
			yield break;
		}
		int num;
		for (int x = 0; x < this.LevelWidth; x = num + 1)
		{
			for (int y = 0; y < this.LevelHeight; y = num + 1)
			{
				if (this.LevelGrid[x, y].active)
				{
					Vector3 rotation = Vector3.zero;
					Vector3 position = new Vector3((float)x * LevelGenerator.ModuleWidth * LevelGenerator.TileSize - (float)(this.LevelWidth / 2) * LevelGenerator.ModuleWidth * LevelGenerator.TileSize, 0f, (float)y * LevelGenerator.ModuleWidth * LevelGenerator.TileSize + LevelGenerator.ModuleWidth * LevelGenerator.TileSize / 2f);
					if (!this.DebugNormal && !this.DebugPassage && !this.DebugDeadEnd && this.LevelGrid[x, y].type == Module.Type.Extraction)
					{
						if (this.GridCheckActive(x, y - 1))
						{
							rotation = Vector3.zero;
						}
						if (this.GridCheckActive(x - 1, y))
						{
							rotation = new Vector3(0f, 90f, 0f);
						}
						if (this.GridCheckActive(x, y + 1))
						{
							rotation = new Vector3(0f, 180f, 0f);
						}
						if (this.GridCheckActive(x + 1, y))
						{
							rotation = new Vector3(0f, -90f, 0f);
						}
						this.SpawnModule(x, y, position, rotation, Module.Type.Extraction);
					}
					else if (this.DebugDeadEnd || (!this.DebugNormal && !this.DebugPassage && this.LevelGrid[x, y].type == Module.Type.DeadEnd))
					{
						if (this.GridCheckActive(x, y - 1))
						{
							rotation = Vector3.zero;
						}
						if (this.GridCheckActive(x - 1, y))
						{
							rotation = new Vector3(0f, 90f, 0f);
						}
						if (this.GridCheckActive(x, y + 1))
						{
							rotation = new Vector3(0f, 180f, 0f);
						}
						if (this.GridCheckActive(x + 1, y))
						{
							rotation = new Vector3(0f, -90f, 0f);
						}
						this.SpawnModule(x, y, position, rotation, Module.Type.DeadEnd);
					}
					else
					{
						if (!this.DebugNormal && (this.DebugPassage || this.PassageAmount < this.Level.PassageMaxAmount))
						{
							if (this.DebugPassage || (this.GridCheckActive(x, y + 1) && (this.GridCheckActive(x, y - 1) || this.LevelGrid[x, y].first) && !this.GridCheckActive(x + 1, y) && !this.GridCheckActive(x - 1, y)))
							{
								if (Random.Range(0, 100) < 50)
								{
									rotation = new Vector3(0f, 180f, 0f);
								}
								this.SpawnModule(x, y, position, rotation, Module.Type.Passage);
								this.PassageAmount++;
								goto IL_793;
							}
							if (!this.LevelGrid[x, y].first && this.GridCheckActive(x + 1, y) && this.GridCheckActive(x - 1, y) && !this.GridCheckActive(x, y + 1) && !this.GridCheckActive(x, y - 1))
							{
								rotation = new Vector3(0f, 90f, 0f);
								if (Random.Range(0, 100) < 50)
								{
									rotation = new Vector3(0f, -90f, 0f);
								}
								this.SpawnModule(x, y, position, rotation, Module.Type.Passage);
								this.PassageAmount++;
								goto IL_793;
							}
						}
						rotation = this.ModuleRotations[Random.Range(0, this.ModuleRotations.Length)];
						this.SpawnModule(x, y, position, rotation, Module.Type.Normal);
						yield return null;
					}
				}
				IL_793:
				num = y;
			}
			num = x;
		}
		this.waitingForSubCoroutine = false;
		yield break;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004D4FF File Offset: 0x0004B6FF
	private bool GridCheckActive(int x, int y)
	{
		return x >= 0 && x < this.LevelWidth && y >= 0 && y < this.LevelHeight && this.LevelGrid[x, y].active;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0004D52F File Offset: 0x0004B72F
	private LevelGenerator.Tile GridGetTile(int x, int y)
	{
		if (x >= 0 && x < this.LevelWidth && y >= 0 && y < this.LevelHeight)
		{
			return this.LevelGrid[x, y];
		}
		return null;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0004D55C File Offset: 0x0004B75C
	private GameObject PickModule(List<GameObject> _list1, List<GameObject> _list2, List<GameObject> _list3, ref int _index1, ref int _index2, ref int _index3, ref int _loops1, ref int _loops2, ref int _loops3)
	{
		GameObject result = null;
		float[] array = new float[]
		{
			this.ModuleRarity1,
			this.ModuleRarity2,
			this.ModuleRarity3
		};
		if (_list2.Count <= 0)
		{
			array[1] = 0f;
		}
		if (_list3.Count <= 0)
		{
			array[2] = 0f;
		}
		int num = Mathf.Max(new int[]
		{
			_loops1,
			_loops2,
			_loops3
		});
		int num2 = Mathf.Min(new int[]
		{
			_loops1,
			_loops2,
			_loops3
		});
		if (num != num2)
		{
			if (_loops1 == num2 && array[0] > 0f)
			{
				if (_loops1 != _loops2)
				{
					array[1] = 0f;
				}
				if (_loops1 != _loops3)
				{
					array[2] = 0f;
				}
			}
			else if (_loops2 == num2 && array[1] > 0f)
			{
				if (_loops2 != _loops1)
				{
					array[0] = 0f;
				}
				if (_loops2 != _loops3)
				{
					array[2] = 0f;
				}
			}
			else if (_loops3 == num2 && array[2] > 0f)
			{
				if (_loops3 != _loops1)
				{
					array[0] = 0f;
				}
				if (_loops3 != _loops2)
				{
					array[1] = 0f;
				}
			}
		}
		float num3 = -1f;
		int num4 = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] > 0f)
			{
				float num5 = Random.Range(0f, array[i]);
				if (num5 > num3)
				{
					num3 = num5;
					num4 = i;
				}
			}
		}
		if (num4 == 0)
		{
			result = _list1[_index1];
			_index1++;
			if (_index1 >= _list1.Count)
			{
				_list1.Shuffle<GameObject>();
				_index1 = 0;
				_loops1++;
			}
		}
		else if (num4 == 1)
		{
			result = _list2[_index2];
			_index2++;
			if (_index2 >= _list2.Count)
			{
				_list2.Shuffle<GameObject>();
				_index2 = 0;
				_loops2++;
			}
		}
		else if (num4 == 2)
		{
			result = _list3[_index3];
			_index3++;
			if (_index3 >= _list3.Count)
			{
				_list3.Shuffle<GameObject>();
				_index3 = 0;
				_loops3++;
			}
		}
		return result;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0004D76C File Offset: 0x0004B96C
	private void SpawnModule(int x, int y, Vector3 position, Vector3 rotation, Module.Type type)
	{
		GameObject gameObject = null;
		if (type == Module.Type.Normal)
		{
			gameObject = this.PickModule(this.ModulesNormalShuffled_1, this.ModulesNormalShuffled_2, this.ModulesNormalShuffled_3, ref this.ModulesNormalIndex_1, ref this.ModulesNormalIndex_2, ref this.ModulesNormalIndex_3, ref this.ModulesNormalIndexLoops_1, ref this.ModulesNormalIndexLoops_2, ref this.ModulesNormalIndexLoops_3);
			this.LevelGrid[x, y].type = Module.Type.Normal;
		}
		else if (type == Module.Type.Passage)
		{
			gameObject = this.PickModule(this.ModulesPassageShuffled_1, this.ModulesPassageShuffled_2, this.ModulesPassageShuffled_3, ref this.ModulesPassageIndex_1, ref this.ModulesPassageIndex_2, ref this.ModulesPassageIndex_3, ref this.ModulesPassageIndexLoops_1, ref this.ModulesPassageIndexLoops_2, ref this.ModulesPassageIndexLoops_3);
			this.LevelGrid[x, y].type = Module.Type.Passage;
		}
		else if (type == Module.Type.DeadEnd)
		{
			gameObject = this.PickModule(this.ModulesDeadEndShuffled_1, this.ModulesDeadEndShuffled_2, this.ModulesDeadEndShuffled_3, ref this.ModulesDeadEndIndex_1, ref this.ModulesDeadEndIndex_2, ref this.ModulesDeadEndIndex_3, ref this.ModulesDeadEndIndexLoops_1, ref this.ModulesDeadEndIndexLoops_2, ref this.ModulesDeadEndIndexLoops_3);
			this.LevelGrid[x, y].type = Module.Type.DeadEnd;
		}
		else if (type == Module.Type.Extraction)
		{
			gameObject = this.PickModule(this.ModulesExtractionShuffled_1, this.ModulesExtractionShuffled_2, this.ModulesExtractionShuffled_3, ref this.ModulesExtractionIndex_1, ref this.ModulesExtractionIndex_2, ref this.ModulesExtractionIndex_3, ref this.ModulesExtractionIndexLoops_1, ref this.ModulesExtractionIndexLoops_2, ref this.ModulesExtractionIndexLoops_3);
			this.LevelGrid[x, y].type = Module.Type.Extraction;
		}
		GameObject gameObject2;
		if (GameManager.instance.gameMode == 0)
		{
			gameObject2 = Object.Instantiate<GameObject>(gameObject, position, Quaternion.Euler(rotation));
		}
		else
		{
			gameObject2 = PhotonNetwork.InstantiateRoomObject(string.Concat(new string[]
			{
				this.ResourceParent,
				"/",
				this.Level.ResourcePath,
				"/",
				this.ResourceModules,
				"/",
				gameObject.name
			}), position, Quaternion.Euler(rotation), 0, null);
		}
		gameObject2.transform.parent = this.LevelParent.transform;
		Module component = gameObject2.GetComponent<Module>();
		component.GridX = x;
		component.GridY = y;
		component.First = this.LevelGrid[x, y].first;
		if (this.GridCheckActive(x, y + 1))
		{
			component.ConnectingTop = true;
		}
		if (this.GridCheckActive(x, y - 1) || component.First)
		{
			component.ConnectingBottom = true;
		}
		if (this.GridCheckActive(x + 1, y))
		{
			component.ConnectingRight = true;
		}
		if (this.GridCheckActive(x - 1, y))
		{
			component.ConnectingLeft = true;
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0004D9EA File Offset: 0x0004BBEA
	private IEnumerator GenerateConnectObjects()
	{
		this.waitingForSubCoroutine = true;
		this.State = LevelGenerator.LevelState.ConnectObjects;
		float moduleWidth = LevelGenerator.ModuleWidth * LevelGenerator.TileSize;
		int num3;
		for (int x = 0; x < this.LevelWidth; x = num3 + 1)
		{
			for (int y = 0; y < this.LevelHeight; y = num3 + 1)
			{
				if (this.LevelGrid[x, y].active)
				{
					if (this.GridCheckActive(x, y + 1))
					{
						this.LevelGrid[x, y].connections++;
					}
					if (this.GridCheckActive(x, y - 1))
					{
						this.LevelGrid[x, y].connections++;
					}
					if (this.GridCheckActive(x + 1, y))
					{
						this.LevelGrid[x, y].connections++;
					}
					if (this.GridCheckActive(x - 1, y))
					{
						this.LevelGrid[x, y].connections++;
					}
					float num = (float)x * moduleWidth - (float)(this.LevelWidth / 2) * moduleWidth;
					float num2 = (float)y * moduleWidth + moduleWidth / 2f;
					if (y + 1 < this.LevelHeight && this.LevelGrid[x, y + 1].active && !this.LevelGrid[x, y + 1].connectedBot)
					{
						this.SpawnConnectObject(new Vector3(num, 0f, num2 + moduleWidth / 2f), Vector3.zero);
						this.LevelGrid[x, y].connectedTop = true;
					}
					if (x + 1 < this.LevelWidth && this.LevelGrid[x + 1, y].active && !this.LevelGrid[x + 1, y].connectedLeft)
					{
						this.SpawnConnectObject(new Vector3(num + moduleWidth / 2f, 0f, num2), new Vector3(0f, 90f, 0f));
						this.LevelGrid[x, y].connectedRight = true;
					}
					if ((y - 1 >= 0 && this.LevelGrid[x, y - 1].active && !this.LevelGrid[x, y - 1].connectedTop) || (x == this.LevelWidth / 2 && y == 0))
					{
						this.SpawnConnectObject(new Vector3(num, 0f, num2 - moduleWidth / 2f), Vector3.zero);
						this.LevelGrid[x, y].connectedBot = true;
					}
					if (x - 1 >= 0 && this.LevelGrid[x - 1, y].active && !this.LevelGrid[x - 1, y].connectedRight)
					{
						this.SpawnConnectObject(new Vector3(num - moduleWidth / 2f, 0f, num2), Vector3.zero);
						this.LevelGrid[x, y].connectedLeft = true;
					}
					yield return null;
				}
				num3 = y;
			}
			num3 = x;
		}
		this.waitingForSubCoroutine = false;
		yield break;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0004D9FC File Offset: 0x0004BBFC
	private void SpawnConnectObject(Vector3 position, Vector3 rotation)
	{
		if (!this.Level.ConnectObject)
		{
			return;
		}
		GameObject gameObject;
		if (GameManager.instance.gameMode == 0)
		{
			gameObject = Object.Instantiate<GameObject>(this.Level.ConnectObject, position, Quaternion.Euler(rotation));
		}
		else
		{
			gameObject = PhotonNetwork.InstantiateRoomObject(string.Concat(new string[]
			{
				this.ResourceParent,
				"/",
				this.Level.ResourcePath,
				"/",
				this.ResourceOther,
				"/",
				this.Level.ConnectObject.name
			}), position, Quaternion.Euler(rotation), 0, null);
		}
		gameObject.transform.parent = this.LevelParent.transform;
		ModuleConnectObject component = gameObject.GetComponent<ModuleConnectObject>();
		component.ModuleConnecting = true;
		component.MasterSetup = true;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0004DAD0 File Offset: 0x0004BCD0
	private IEnumerator GenerateBlockObjects()
	{
		this.waitingForSubCoroutine = true;
		this.State = LevelGenerator.LevelState.BlockObjects;
		float moduleWidth = LevelGenerator.ModuleWidth * LevelGenerator.TileSize;
		int num3;
		for (int x = 0; x < this.LevelWidth; x = num3 + 1)
		{
			for (int y = 0; y < this.LevelHeight; y = num3 + 1)
			{
				if (this.LevelGrid[x, y].active && this.LevelGrid[x, y].type == Module.Type.Normal)
				{
					float num = (float)x * moduleWidth - (float)(this.LevelWidth / 2) * moduleWidth;
					float num2 = (float)y * moduleWidth + moduleWidth / 2f;
					if (y + 1 >= this.LevelHeight || !this.LevelGrid[x, y + 1].active)
					{
						this.SpawnBlockObject(new Vector3(num, 0f, num2 + moduleWidth / 2f), new Vector3(0f, 180f, 0f));
					}
					if (x + 1 >= this.LevelWidth || !this.LevelGrid[x + 1, y].active)
					{
						this.SpawnBlockObject(new Vector3(num + moduleWidth / 2f, 0f, num2), new Vector3(0f, -90f, 0f));
					}
					if ((y - 1 < 0 || !this.LevelGrid[x, y - 1].active) && (x != this.LevelWidth / 2 || y != 0))
					{
						this.SpawnBlockObject(new Vector3(num, 0f, num2 - moduleWidth / 2f), new Vector3(0f, 0f, 0f));
					}
					if (x - 1 < 0 || !this.LevelGrid[x - 1, y].active)
					{
						this.SpawnBlockObject(new Vector3(num - moduleWidth / 2f, 0f, num2), new Vector3(0f, 90f, 0f));
					}
					yield return null;
				}
				num3 = y;
			}
			num3 = x;
		}
		this.waitingForSubCoroutine = false;
		yield break;
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0004DAE0 File Offset: 0x0004BCE0
	private void SpawnBlockObject(Vector3 position, Vector3 rotation)
	{
		if (!this.Level.BlockObject)
		{
			return;
		}
		GameObject gameObject;
		if (GameManager.instance.gameMode == 0)
		{
			gameObject = Object.Instantiate<GameObject>(this.Level.BlockObject, position, Quaternion.Euler(rotation));
		}
		else
		{
			gameObject = PhotonNetwork.InstantiateRoomObject(string.Concat(new string[]
			{
				this.ResourceParent,
				"/",
				this.Level.ResourcePath,
				"/",
				this.ResourceOther,
				"/",
				this.Level.BlockObject.name
			}), position, Quaternion.Euler(rotation), 0, null);
		}
		gameObject.transform.parent = this.LevelParent.transform;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0004DBA4 File Offset: 0x0004BDA4
	private void EnemySetup()
	{
		RoomVolume roomVolume = null;
		foreach (RoomVolume roomVolume2 in Object.FindObjectsOfType<RoomVolume>().ToList<RoomVolume>())
		{
			if (roomVolume2.Truck)
			{
				roomVolume = roomVolume2;
				break;
			}
		}
		LevelPoint levelPoint = null;
		float num = 0f;
		foreach (LevelPoint levelPoint2 in this.LevelPathPoints)
		{
			float num2 = Vector3.Distance(levelPoint2.transform.position, roomVolume.transform.position);
			if (num2 > num)
			{
				num = num2;
				levelPoint = levelPoint2;
			}
		}
		RunManager.instance.EnemiesSpawnedRemoveStart();
		bool flag = false;
		if (EnemyDirector.instance.debugEnemy != null)
		{
			flag = true;
			foreach (EnemySetup enemySetup in EnemyDirector.instance.debugEnemy)
			{
				this.EnemySpawn(enemySetup, levelPoint.transform.position);
			}
		}
		if (!flag)
		{
			EnemyDirector.instance.AmountSetup();
			for (int j = 0; j < EnemyDirector.instance.totalAmount; j++)
			{
				this.EnemySpawn(EnemyDirector.instance.GetEnemy(), levelPoint.transform.position);
			}
			EnemyDirector.instance.DebugResult();
		}
		RunManager.instance.EnemiesSpawnedRemoveEnd();
		if (GameManager.Multiplayer())
		{
			this.PhotonView.RPC("EnemySpawnTargetRPC", RpcTarget.AllBuffered, new object[]
			{
				this.EnemiesSpawnTarget
			});
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0004DD48 File Offset: 0x0004BF48
	private void EnemySpawn(EnemySetup enemySetup, Vector3 position)
	{
		foreach (GameObject gameObject in enemySetup.spawnObjects)
		{
			GameObject gameObject2;
			if (GameManager.instance.gameMode == 0)
			{
				gameObject2 = Object.Instantiate<GameObject>(gameObject, position, Quaternion.identity);
			}
			else
			{
				gameObject2 = PhotonNetwork.InstantiateRoomObject(this.ResourceEnemies + "/" + gameObject.name, position, Quaternion.identity, 0, null);
			}
			EnemyParent component = gameObject2.GetComponent<EnemyParent>();
			if (component)
			{
				component.SetupDone = true;
				gameObject2.GetComponentInChildren<Enemy>().EnemyTeleported(position);
				this.EnemiesSpawnTarget++;
				EnemyDirector.instance.FirstSpawnPointAdd(component);
			}
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0004DE14 File Offset: 0x0004C014
	[PunRPC]
	private void GenerateDone()
	{
		SemiFunc.OnLevelGenDone();
		GameDirector.instance.SetStart();
		this.Generated = true;
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0004DE2C File Offset: 0x0004C02C
	[PunRPC]
	private void EnemySpawnTargetRPC(int _amount)
	{
		this.EnemiesSpawnTarget = _amount;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0004DE35 File Offset: 0x0004C035
	[PunRPC]
	private void EnemyReadyRPC()
	{
		this.EnemyReadyPlayers++;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0004DE45 File Offset: 0x0004C045
	[PunRPC]
	private void EnemyReadyAllRPC()
	{
		this.EnemyReady = true;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0004DE4E File Offset: 0x0004C04E
	[PunRPC]
	private void PlayerSpawnedRPC()
	{
		this.playerSpawned++;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004DE5E File Offset: 0x0004C05E
	[PunRPC]
	private void ModulesReadyRPC()
	{
		this.ModulesReadyPlayers++;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0004DE6E File Offset: 0x0004C06E
	[PunRPC]
	private void ModuleAmountRPC(int amount)
	{
		this.ModuleAmount = amount;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0004DE77 File Offset: 0x0004C077
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.ModulesReadyPlayers);
			return;
		}
		this.ModulesReadyPlayers = (int)stream.ReceiveNext();
	}

	// Token: 0x04000E2E RID: 3630
	public LevelGenerator.LevelState State;

	// Token: 0x04000E2F RID: 3631
	public static LevelGenerator Instance;

	// Token: 0x04000E30 RID: 3632
	[HideInInspector]
	public PhotonView PhotonView;

	// Token: 0x04000E31 RID: 3633
	internal GameObject DebugModule;

	// Token: 0x04000E32 RID: 3634
	internal GameObject DebugStartRoom;

	// Token: 0x04000E33 RID: 3635
	internal bool DebugNormal;

	// Token: 0x04000E34 RID: 3636
	internal bool DebugPassage;

	// Token: 0x04000E35 RID: 3637
	internal bool DebugDeadEnd;

	// Token: 0x04000E36 RID: 3638
	internal int DebugAmount;

	// Token: 0x04000E37 RID: 3639
	internal bool DebugNoEnemy;

	// Token: 0x04000E38 RID: 3640
	internal float DebugLevelSize = 1f;

	// Token: 0x04000E39 RID: 3641
	internal bool AllPlayersReady;

	// Token: 0x04000E3A RID: 3642
	public bool Generated;

	// Token: 0x04000E3B RID: 3643
	internal int ModulesSpawned;

	// Token: 0x04000E3C RID: 3644
	private int ModulesReadyPlayers;

	// Token: 0x04000E3D RID: 3645
	[Space]
	internal int EnemiesSpawnTarget;

	// Token: 0x04000E3E RID: 3646
	internal int EnemiesSpawned;

	// Token: 0x04000E3F RID: 3647
	private int EnemyReadyPlayers;

	// Token: 0x04000E40 RID: 3648
	private bool EnemyReady;

	// Token: 0x04000E41 RID: 3649
	internal int playerSpawned;

	// Token: 0x04000E42 RID: 3650
	[Space]
	public Level Level;

	// Token: 0x04000E43 RID: 3651
	internal int ModuleAmount;

	// Token: 0x04000E44 RID: 3652
	private int PassageAmount;

	// Token: 0x04000E45 RID: 3653
	private int DeadEndAmount;

	// Token: 0x04000E46 RID: 3654
	private int ExtractionAmount;

	// Token: 0x04000E47 RID: 3655
	public static float TileSize = 5f;

	// Token: 0x04000E48 RID: 3656
	public static float ModuleWidth = 3f;

	// Token: 0x04000E49 RID: 3657
	public static float ModuleHeight = 1f;

	// Token: 0x04000E4A RID: 3658
	[Space]
	public GameObject LevelParent;

	// Token: 0x04000E4B RID: 3659
	public GameObject EnemyParent;

	// Token: 0x04000E4C RID: 3660
	public GameObject ItemParent;

	// Token: 0x04000E4D RID: 3661
	private List<GameObject> ModulesNormalShuffled_1;

	// Token: 0x04000E4E RID: 3662
	private List<GameObject> ModulesNormalShuffled_2;

	// Token: 0x04000E4F RID: 3663
	private List<GameObject> ModulesNormalShuffled_3;

	// Token: 0x04000E50 RID: 3664
	private int ModulesNormalIndex_1;

	// Token: 0x04000E51 RID: 3665
	private int ModulesNormalIndex_2;

	// Token: 0x04000E52 RID: 3666
	private int ModulesNormalIndex_3;

	// Token: 0x04000E53 RID: 3667
	private int ModulesNormalIndexLoops_1;

	// Token: 0x04000E54 RID: 3668
	private int ModulesNormalIndexLoops_2;

	// Token: 0x04000E55 RID: 3669
	private int ModulesNormalIndexLoops_3;

	// Token: 0x04000E56 RID: 3670
	private List<GameObject> ModulesPassageShuffled_1;

	// Token: 0x04000E57 RID: 3671
	private List<GameObject> ModulesPassageShuffled_2;

	// Token: 0x04000E58 RID: 3672
	private List<GameObject> ModulesPassageShuffled_3;

	// Token: 0x04000E59 RID: 3673
	private int ModulesPassageIndex_1;

	// Token: 0x04000E5A RID: 3674
	private int ModulesPassageIndex_2;

	// Token: 0x04000E5B RID: 3675
	private int ModulesPassageIndex_3;

	// Token: 0x04000E5C RID: 3676
	private int ModulesPassageIndexLoops_1;

	// Token: 0x04000E5D RID: 3677
	private int ModulesPassageIndexLoops_2;

	// Token: 0x04000E5E RID: 3678
	private int ModulesPassageIndexLoops_3;

	// Token: 0x04000E5F RID: 3679
	private List<GameObject> ModulesDeadEndShuffled_1;

	// Token: 0x04000E60 RID: 3680
	private List<GameObject> ModulesDeadEndShuffled_2;

	// Token: 0x04000E61 RID: 3681
	private List<GameObject> ModulesDeadEndShuffled_3;

	// Token: 0x04000E62 RID: 3682
	private int ModulesDeadEndIndex_1;

	// Token: 0x04000E63 RID: 3683
	private int ModulesDeadEndIndex_2;

	// Token: 0x04000E64 RID: 3684
	private int ModulesDeadEndIndex_3;

	// Token: 0x04000E65 RID: 3685
	private int ModulesDeadEndIndexLoops_1;

	// Token: 0x04000E66 RID: 3686
	private int ModulesDeadEndIndexLoops_2;

	// Token: 0x04000E67 RID: 3687
	private int ModulesDeadEndIndexLoops_3;

	// Token: 0x04000E68 RID: 3688
	private List<GameObject> ModulesExtractionShuffled_1;

	// Token: 0x04000E69 RID: 3689
	private List<GameObject> ModulesExtractionShuffled_2;

	// Token: 0x04000E6A RID: 3690
	private List<GameObject> ModulesExtractionShuffled_3;

	// Token: 0x04000E6B RID: 3691
	private int ModulesExtractionIndex_1;

	// Token: 0x04000E6C RID: 3692
	private int ModulesExtractionIndex_2;

	// Token: 0x04000E6D RID: 3693
	private int ModulesExtractionIndex_3;

	// Token: 0x04000E6E RID: 3694
	private int ModulesExtractionIndexLoops_1;

	// Token: 0x04000E6F RID: 3695
	private int ModulesExtractionIndexLoops_2;

	// Token: 0x04000E70 RID: 3696
	private int ModulesExtractionIndexLoops_3;

	// Token: 0x04000E71 RID: 3697
	[Space]
	public AnimationCurve DifficultyCurve1;

	// Token: 0x04000E72 RID: 3698
	public AnimationCurve DifficultyCurve2;

	// Token: 0x04000E73 RID: 3699
	public AnimationCurve DifficultyCurve3;

	// Token: 0x04000E74 RID: 3700
	private float ModuleRarity1;

	// Token: 0x04000E75 RID: 3701
	private float ModuleRarity2;

	// Token: 0x04000E76 RID: 3702
	private float ModuleRarity3;

	// Token: 0x04000E77 RID: 3703
	[Space]
	public int LevelWidth = 3;

	// Token: 0x04000E78 RID: 3704
	public int LevelHeight = 3;

	// Token: 0x04000E79 RID: 3705
	private LevelGenerator.Tile[,] LevelGrid;

	// Token: 0x04000E7A RID: 3706
	private Vector3[] ModuleRotations = new Vector3[]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 180f, 0f),
		new Vector3(0f, 270f, 0f)
	};

	// Token: 0x04000E7B RID: 3707
	[Space]
	public List<LevelPoint> LevelPathPoints;

	// Token: 0x04000E7C RID: 3708
	public LevelPoint LevelPathTruck;

	// Token: 0x04000E7D RID: 3709
	private NavMeshSurface NavMeshSurface;

	// Token: 0x04000E7E RID: 3710
	private string ResourceParent = "Level";

	// Token: 0x04000E7F RID: 3711
	private string ResourceStart = "Start Room";

	// Token: 0x04000E80 RID: 3712
	private string ResourceModules = "Modules";

	// Token: 0x04000E81 RID: 3713
	private string ResourceOther = "Other";

	// Token: 0x04000E82 RID: 3714
	private string ResourceEnemies = "Enemies";

	// Token: 0x04000E83 RID: 3715
	[Space]
	public GameObject PlayerDeathHeadPrefab;

	// Token: 0x04000E84 RID: 3716
	public GameObject PlayerTumblePrefab;

	// Token: 0x04000E85 RID: 3717
	private bool waitingForSubCoroutine;

	// Token: 0x02000307 RID: 775
	public enum LevelState
	{
		// Token: 0x0400258F RID: 9615
		Start,
		// Token: 0x04002590 RID: 9616
		Load,
		// Token: 0x04002591 RID: 9617
		Tiles,
		// Token: 0x04002592 RID: 9618
		StartRoom,
		// Token: 0x04002593 RID: 9619
		ConnectObjects,
		// Token: 0x04002594 RID: 9620
		ModuleGeneration,
		// Token: 0x04002595 RID: 9621
		BlockObjects,
		// Token: 0x04002596 RID: 9622
		ModuleSpawnLocal,
		// Token: 0x04002597 RID: 9623
		ModuleSpawnRemote,
		// Token: 0x04002598 RID: 9624
		LevelPoint,
		// Token: 0x04002599 RID: 9625
		Item,
		// Token: 0x0400259A RID: 9626
		Valuable,
		// Token: 0x0400259B RID: 9627
		PlayerSetup,
		// Token: 0x0400259C RID: 9628
		PlayerSpawn,
		// Token: 0x0400259D RID: 9629
		Enemy,
		// Token: 0x0400259E RID: 9630
		Done
	}

	// Token: 0x02000308 RID: 776
	[Serializable]
	public class Tile
	{
		// Token: 0x0400259F RID: 9631
		public int x;

		// Token: 0x040025A0 RID: 9632
		public int y;

		// Token: 0x040025A1 RID: 9633
		public bool active;

		// Token: 0x040025A2 RID: 9634
		public bool first;

		// Token: 0x040025A3 RID: 9635
		public int connections;

		// Token: 0x040025A4 RID: 9636
		public bool connectedTop;

		// Token: 0x040025A5 RID: 9637
		public bool connectedRight;

		// Token: 0x040025A6 RID: 9638
		public bool connectedBot;

		// Token: 0x040025A7 RID: 9639
		public bool connectedLeft;

		// Token: 0x040025A8 RID: 9640
		public Module.Type type;
	}
}
