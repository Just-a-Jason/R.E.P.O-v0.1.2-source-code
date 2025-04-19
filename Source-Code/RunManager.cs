using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000129 RID: 297
public class RunManager : MonoBehaviour
{
	// Token: 0x06000977 RID: 2423 RVA: 0x00057A79 File Offset: 0x00055C79
	private void Awake()
	{
		if (!RunManager.instance)
		{
			RunManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.levelPrevious = this.levelCurrent;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00057AB4 File Offset: 0x00055CB4
	private void Update()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (LevelGenerator.Instance.Generated && !SteamClient.IsValid && !SteamManager.instance.enabled)
		{
			Debug.LogError("Steam not initialized. Quitting game.");
			Application.Quit();
		}
		if (SemiFunc.DebugDev())
		{
			if (Input.GetKeyDown(KeyCode.F3))
			{
				if (SemiFunc.RunIsArena())
				{
					this.ChangeLevel(true, true, RunManager.ChangeLevelType.Normal);
				}
				else
				{
					this.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
				}
			}
			if (!this.restarting && ChatManager.instance && !ChatManager.instance.chatActive && Input.GetKeyDown(KeyCode.Backspace))
			{
				this.ResetProgress();
				this.RestartScene();
				if (this.levelCurrent != this.levelTutorial)
				{
					SemiFunc.OnSceneSwitch(this.gameOver, false);
				}
			}
		}
		if (this.restarting)
		{
			this.RestartScene();
		}
		if (!this.restarting && this.runStarted && GameDirector.instance.PlayerList.Count > 0 && !SemiFunc.RunIsArena() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			bool flag = true;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.isDisabled)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.allPlayersDead = true;
				if (SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
				{
					this.ChangeLevel(false, true, RunManager.ChangeLevelType.Normal);
				}
			}
		}
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00057C44 File Offset: 0x00055E44
	private void OnApplicationQuit()
	{
		DataDirector.instance.SaveDeleteCheck(true);
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00057C54 File Offset: 0x00055E54
	public void ChangeLevel(bool _completedLevel, bool _levelFailed, RunManager.ChangeLevelType _changeLevelType = RunManager.ChangeLevelType.Normal)
	{
		if ((!SemiFunc.MenuLevel() && !SemiFunc.IsMasterClientOrSingleplayer()) || this.restarting)
		{
			return;
		}
		this.gameOver = false;
		if (_levelFailed && this.levelCurrent != this.levelLobby && this.levelCurrent != this.levelShop)
		{
			if (this.levelCurrent == this.levelArena)
			{
				this.ResetProgress();
				if (SemiFunc.IsMultiplayer())
				{
					this.levelCurrent = this.levelLobbyMenu;
				}
				else
				{
					this.SetRunLevel();
				}
				this.gameOver = true;
			}
			else
			{
				this.levelCurrent = this.levelArena;
			}
		}
		if (!this.gameOver && this.levelCurrent != this.levelArena)
		{
			if (_changeLevelType == RunManager.ChangeLevelType.RunLevel)
			{
				this.SetRunLevel();
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.LobbyMenu)
			{
				this.levelCurrent = this.levelLobbyMenu;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.MainMenu)
			{
				this.levelCurrent = this.levelMainMenu;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Tutorial)
			{
				this.levelCurrent = this.levelTutorial;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Recording)
			{
				this.levelCurrent = this.levelRecording;
			}
			else if (_changeLevelType == RunManager.ChangeLevelType.Shop)
			{
				this.levelCurrent = this.levelShop;
			}
			else if (this.levelCurrent == this.levelMainMenu || this.levelCurrent == this.levelLobbyMenu)
			{
				this.levelCurrent = this.levelLobby;
			}
			else if (_completedLevel && this.levelCurrent != this.levelLobby && this.levelCurrent != this.levelShop)
			{
				this.previousRunLevel = this.levelCurrent;
				this.levelsCompleted++;
				SemiFunc.StatSetRunLevel(this.levelsCompleted);
				SemiFunc.LevelSuccessful();
				this.levelCurrent = this.levelShop;
			}
			else if (this.levelCurrent == this.levelLobby)
			{
				this.SetRunLevel();
			}
			else if (this.levelCurrent == this.levelShop)
			{
				this.levelCurrent = this.levelLobby;
			}
		}
		if (this.debugLevel && this.levelCurrent != this.levelMainMenu && this.levelCurrent != this.levelLobbyMenu)
		{
			this.levelCurrent = this.debugLevel;
		}
		if (GameManager.Multiplayer())
		{
			this.runManagerPUN.photonView.RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, new object[]
			{
				this.levelCurrent.name,
				this.levelsCompleted,
				this.gameOver
			});
		}
		Debug.Log("Changed level to: " + this.levelCurrent.name);
		if (this.levelCurrent == this.levelShop)
		{
			this.saveLevel = 1;
		}
		else
		{
			this.saveLevel = 0;
		}
		SemiFunc.StatSetSaveLevel(this.saveLevel);
		this.RestartScene();
		if (_changeLevelType != RunManager.ChangeLevelType.Tutorial)
		{
			SemiFunc.OnSceneSwitch(this.gameOver, false);
		}
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00057F48 File Offset: 0x00056148
	public void RestartScene()
	{
		if (!this.restarting)
		{
			this.restarting = true;
			if (!GameDirector.instance)
			{
				return;
			}
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerAvatar playerAvatar = enumerator.Current;
					playerAvatar.OutroStart();
				}
				return;
			}
		}
		if (!this.restartingDone)
		{
			bool flag = true;
			if (!GameDirector.instance)
			{
				flag = false;
			}
			else
			{
				using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.outroDone)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				if (this.gameOver)
				{
					NetworkManager.instance.DestroyAll();
					this.gameOver = false;
				}
				if (this.lobbyJoin)
				{
					this.lobbyJoin = false;
					this.restartingDone = true;
					SceneManager.LoadSceneAsync("LobbyJoin");
					return;
				}
				if (!this.waitToChangeScene)
				{
					this.restartingDone = true;
					if (!GameManager.Multiplayer())
					{
						SceneManager.LoadSceneAsync("Main");
						return;
					}
					if (PhotonNetwork.IsMasterClient)
					{
						PhotonNetwork.LoadLevel("Reload");
					}
				}
			}
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00058094 File Offset: 0x00056294
	public void UpdateLevel(string _levelName, int _levelsCompleted, bool _gameOver)
	{
		if (LobbyMenuOpen.instance)
		{
			DataDirector.instance.RunsPlayedAdd();
		}
		SemiFunc.OnSceneSwitch(_gameOver, false);
		this.levelsCompleted = _levelsCompleted;
		SemiFunc.StatSetRunLevel(this.levelsCompleted);
		if (_levelName == this.levelLobbyMenu.name)
		{
			this.levelCurrent = this.levelLobbyMenu;
		}
		else if (_levelName == this.levelLobby.name)
		{
			this.levelCurrent = this.levelLobby;
		}
		else if (_levelName == this.levelShop.name)
		{
			this.levelCurrent = this.levelShop;
		}
		else if (_levelName == this.levelArena.name)
		{
			this.levelCurrent = this.levelArena;
		}
		else if (_levelName == this.levelRecording.name)
		{
			this.levelCurrent = this.levelRecording;
		}
		else
		{
			foreach (Level level in this.levels)
			{
				if (level.name == _levelName)
				{
					this.levelCurrent = level;
					break;
				}
			}
		}
		Debug.Log("updated level to: " + this.levelCurrent.name);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x000581F0 File Offset: 0x000563F0
	public void ResetProgress()
	{
		if (StatsManager.instance)
		{
			StatsManager.instance.ResetAllStats();
		}
		this.levelsCompleted = 0;
		this.loadLevel = 0;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00058218 File Offset: 0x00056418
	public void EnemiesSpawnedRemoveStart()
	{
		this.enemiesSpawnedToDelete.Clear();
		foreach (EnemySetup enemySetup in this.enemiesSpawned)
		{
			bool flag = false;
			foreach (EnemySetup y in this.enemiesSpawnedToDelete)
			{
				if (enemySetup == y)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.enemiesSpawnedToDelete.Add(enemySetup);
			}
		}
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000582CC File Offset: 0x000564CC
	public void EnemiesSpawnedRemoveEnd()
	{
		foreach (EnemySetup item in this.enemiesSpawnedToDelete)
		{
			this.enemiesSpawned.Remove(item);
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00058328 File Offset: 0x00056528
	public void SetRunLevel()
	{
		this.levelCurrent = this.previousRunLevel;
		while (this.levelCurrent == this.previousRunLevel)
		{
			this.levelCurrent = this.levels[Random.Range(0, this.levels.Count)];
		}
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00058378 File Offset: 0x00056578
	public IEnumerator LeaveToMainMenu()
	{
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		Debug.Log("Leave to Main Menu");
		SemiFunc.OnSceneSwitch(false, true);
		this.levelCurrent = this.levelMainMenu;
		SceneManager.LoadSceneAsync("Reload");
		yield return null;
		yield break;
	}

	// Token: 0x040010D1 RID: 4305
	public static RunManager instance;

	// Token: 0x040010D2 RID: 4306
	internal int saveLevel;

	// Token: 0x040010D3 RID: 4307
	internal int loadLevel;

	// Token: 0x040010D4 RID: 4308
	internal Level debugLevel;

	// Token: 0x040010D5 RID: 4309
	internal bool skipMainMenu;

	// Token: 0x040010D6 RID: 4310
	internal bool localMultiplayerTest;

	// Token: 0x040010D7 RID: 4311
	internal bool runStarted;

	// Token: 0x040010D8 RID: 4312
	internal RunManagerPUN runManagerPUN;

	// Token: 0x040010D9 RID: 4313
	public int levelsCompleted;

	// Token: 0x040010DA RID: 4314
	public Level levelCurrent;

	// Token: 0x040010DB RID: 4315
	internal Level levelPrevious;

	// Token: 0x040010DC RID: 4316
	private Level previousRunLevel;

	// Token: 0x040010DD RID: 4317
	internal bool restarting;

	// Token: 0x040010DE RID: 4318
	internal bool restartingDone;

	// Token: 0x040010DF RID: 4319
	internal int levelsMax = 10;

	// Token: 0x040010E0 RID: 4320
	[Space]
	public Level levelMainMenu;

	// Token: 0x040010E1 RID: 4321
	public Level levelLobbyMenu;

	// Token: 0x040010E2 RID: 4322
	public Level levelLobby;

	// Token: 0x040010E3 RID: 4323
	public Level levelShop;

	// Token: 0x040010E4 RID: 4324
	public Level levelTutorial;

	// Token: 0x040010E5 RID: 4325
	public Level levelRecording;

	// Token: 0x040010E6 RID: 4326
	public Level levelArena;

	// Token: 0x040010E7 RID: 4327
	public List<Level> levels;

	// Token: 0x040010E8 RID: 4328
	internal int runLives = 3;

	// Token: 0x040010E9 RID: 4329
	internal bool levelFailed;

	// Token: 0x040010EA RID: 4330
	internal bool waitToChangeScene;

	// Token: 0x040010EB RID: 4331
	internal bool lobbyJoin;

	// Token: 0x040010EC RID: 4332
	internal bool masterSwitched;

	// Token: 0x040010ED RID: 4333
	internal bool gameOver;

	// Token: 0x040010EE RID: 4334
	internal bool allPlayersDead;

	// Token: 0x040010EF RID: 4335
	[Space]
	public List<EnemySetup> enemiesSpawned;

	// Token: 0x040010F0 RID: 4336
	private List<EnemySetup> enemiesSpawnedToDelete = new List<EnemySetup>();

	// Token: 0x040010F1 RID: 4337
	internal bool skipLoadingUI = true;

	// Token: 0x040010F2 RID: 4338
	internal Color loadingFadeColor = Color.black;

	// Token: 0x040010F3 RID: 4339
	internal float loadingAnimationTime;

	// Token: 0x040010F4 RID: 4340
	internal List<PlayerVoiceChat> voiceChats = new List<PlayerVoiceChat>();

	// Token: 0x02000334 RID: 820
	public enum ChangeLevelType
	{
		// Token: 0x04002663 RID: 9827
		Normal,
		// Token: 0x04002664 RID: 9828
		RunLevel,
		// Token: 0x04002665 RID: 9829
		Tutorial,
		// Token: 0x04002666 RID: 9830
		LobbyMenu,
		// Token: 0x04002667 RID: 9831
		MainMenu,
		// Token: 0x04002668 RID: 9832
		Shop,
		// Token: 0x04002669 RID: 9833
		Recording
	}

	// Token: 0x02000335 RID: 821
	public enum SaveLevel
	{
		// Token: 0x0400266B RID: 9835
		Lobby,
		// Token: 0x0400266C RID: 9836
		Shop
	}
}
