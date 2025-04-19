using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class DebugComputerCheck : MonoBehaviour
{
	// Token: 0x06000F60 RID: 3936 RVA: 0x0008CE94 File Offset: 0x0008B094
	private void Start()
	{
		if (!Application.isEditor)
		{
			this.Active = false;
			base.gameObject.SetActive(false);
			return;
		}
		bool flag = false;
		foreach (string b in this.computerNames)
		{
			if (SystemInfo.deviceName == b)
			{
				DebugComputerCheck.instance = this;
				flag = true;
			}
		}
		if (!flag)
		{
			this.Active = false;
			base.gameObject.SetActive(false);
			return;
		}
		if (this.DebugDisable)
		{
			this.Active = false;
			return;
		}
		if (this.Mode == DebugComputerCheck.StartMode.SinglePlayer)
		{
			RunManager.instance.skipMainMenu = true;
			if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu)
			{
				RunManager.instance.SetRunLevel();
			}
		}
		if (this.Mode == DebugComputerCheck.StartMode.Multiplayer)
		{
			RunManager.instance.localMultiplayerTest = true;
		}
		if (this.PlayerDebug && this.InfiniteEnergy)
		{
			PlayerController.instance.DebugEnergy = true;
		}
		if (this.PlayerDebug && this.NoTumbleMode)
		{
			PlayerController.instance.DebugNoTumble = true;
		}
		if (this.LevelDebug && RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu && RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu)
		{
			if (this.LevelOverride)
			{
				RunManager.instance.debugLevel = this.LevelOverride;
				RunManager.instance.levelCurrent = this.LevelOverride;
			}
			if (this.LevelsCompleted > 0)
			{
				RunManager.instance.levelsCompleted = this.LevelsCompleted;
			}
			if (this.StartRoomOverride)
			{
				LevelGenerator.Instance.DebugStartRoom = this.StartRoomOverride;
			}
			LevelGenerator.Instance.DebugLevelSize = this.LevelSizeMultiplier;
			if (!this.ModuleOverrideActive)
			{
				this.ModuleOverride = null;
			}
			if (this.ModuleOverride)
			{
				LevelGenerator.Instance.DebugModule = this.ModuleOverride;
				if (this.ModuleType == Module.Type.Normal)
				{
					LevelGenerator.Instance.DebugNormal = true;
				}
				else if (this.ModuleType == Module.Type.Passage)
				{
					LevelGenerator.Instance.DebugPassage = true;
					LevelGenerator.Instance.DebugAmount = 6;
				}
				else if (this.ModuleType == Module.Type.DeadEnd || this.ModuleType == Module.Type.Extraction)
				{
					LevelGenerator.Instance.DebugDeadEnd = true;
					LevelGenerator.Instance.DebugAmount = 1;
				}
			}
			if (this.OnlyOneModule)
			{
				LevelGenerator.Instance.DebugAmount = 1;
			}
		}
		if (this.OtherDebug)
		{
			ValuableDirector.instance.valuableDebug = this.valuableDebug;
		}
		if (this.EnemyDebug)
		{
			if (this.EnemyDisable)
			{
				LevelGenerator.Instance.DebugNoEnemy = true;
			}
			if (this.EnemyNoVision)
			{
				EnemyDirector.instance.debugNoVision = true;
			}
			if (this.EnemyEasyGrab)
			{
				EnemyDirector.instance.debugEasyGrab = true;
			}
			if (this.EnemySpawnClose)
			{
				EnemyDirector.instance.debugSpawnClose = true;
			}
			if (this.EnemyDespawnClose)
			{
				EnemyDirector.instance.debugDespawnClose = true;
			}
			if (this.EnemyInvestigates)
			{
				EnemyDirector.instance.debugInvestigate = true;
			}
			if (this.EnemyShortActionTimer)
			{
				EnemyDirector.instance.debugShortActionTimer = true;
			}
			if (this.EnemyNoSpawnedPause)
			{
				EnemyDirector.instance.debugNoSpawnedPause = true;
			}
			if (this.EnemyNoSpawnIdlePause)
			{
				EnemyDirector.instance.debugNoSpawnIdlePause = true;
			}
			if (this.EnemyNoGrabMaxTime)
			{
				EnemyDirector.instance.debugNoGrabMaxTime = true;
			}
			if (this.EnemyOverride.Length != 0)
			{
				EnemyDirector.instance.debugEnemy = this.EnemyOverride;
				EnemyDirector.instance.debugEnemyEnableTime = this.EnemyEnableTimeOverride;
				EnemyDirector.instance.debugEnemyDisableTime = this.EnemyDisableTimeOverride;
			}
		}
		if (this.OtherDebug && this.LowHaul)
		{
			RoundDirector.instance.debugLowHaul = true;
		}
		if (this.OtherDebug && this.DisableMusic)
		{
			LevelMusic.instance.gameObject.SetActive(false);
			ConstantMusic.instance.gameObject.SetActive(false);
			MusicEnemyNear.instance.gameObject.SetActive(false);
		}
		if (this.OtherDebug && this.DisableLoadingLevelAnimation)
		{
			LoadingUI.instance.debugDisableLevelAnimation = true;
		}
		base.StartCoroutine(this.StatsUpdate());
		if (this.PlayerDebug && this.DebugVoice)
		{
			base.StartCoroutine(this.VoiceUpdate());
		}
		if (this.OtherDebug && this.SimulateLag)
		{
			base.StartCoroutine(this.SimulateLagUpdate());
		}
		base.StartCoroutine(this.AfterLevelGen());
		if (this.PlayerDebug && this.DebugMapActive)
		{
			Map.Instance.debugActive = true;
		}
		if (this.PlayerDebug && (this.StickyGrabber || this.PowerGrabber))
		{
			base.StartCoroutine(this.PhysGrabber());
		}
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0008D311 File Offset: 0x0008B511
	private IEnumerator SimulateLagUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		PunManager.instance.lagSimulationGui.enabled = true;
		yield break;
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0008D319 File Offset: 0x0008B519
	private IEnumerator AfterLevelGen()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.EmptyBatteries)
		{
			StatsManager.instance.EmptyAllBatteries();
		}
		yield break;
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0008D328 File Offset: 0x0008B528
	private IEnumerator StatsUpdate()
	{
		if (!this.StatsDebug)
		{
			yield break;
		}
		while (!StatsManager.instance || !StatsManager.instance.statsSynced || !PunManager.instance.statsManager)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.Currency > 0)
		{
			SemiFunc.StatSetRunCurrency(this.Currency);
		}
		if (this.BuyAllItems)
		{
			StatsManager.instance.BuyAllItems();
		}
		if (this.StartCrystals != 1)
		{
			StatsManager.instance.itemsPurchased["Item Power Crystal"] = this.StartCrystals;
		}
		yield break;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0008D337 File Offset: 0x0008B537
	private IEnumerator VoiceUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		foreach (PlayerAvatar _player in GameDirector.instance.PlayerList)
		{
			while (!_player.voiceChat)
			{
				yield return new WaitForSeconds(0.1f);
			}
			_player.voiceChat.SetDebug();
			_player = null;
		}
		List<PlayerAvatar>.Enumerator enumerator = default(List<PlayerAvatar>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0008D33F File Offset: 0x0008B53F
	private IEnumerator PhysGrabber()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.StickyGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.debugStickyGrabber = true;
		}
		if (this.PowerGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.grabStrength = 5f;
		}
		yield break;
	}

	// Token: 0x040019FA RID: 6650
	public static DebugComputerCheck instance;

	// Token: 0x040019FB RID: 6651
	internal bool Active = true;

	// Token: 0x040019FC RID: 6652
	public bool DebugDisable;

	// Token: 0x040019FD RID: 6653
	public SemiFunc.User DebugUser;

	// Token: 0x040019FE RID: 6654
	public string[] computerNames;

	// Token: 0x040019FF RID: 6655
	public DebugComputerCheck.StartMode Mode;

	// Token: 0x04001A00 RID: 6656
	public bool LevelDebug = true;

	// Token: 0x04001A01 RID: 6657
	public Level LevelOverride;

	// Token: 0x04001A02 RID: 6658
	public GameObject StartRoomOverride;

	// Token: 0x04001A03 RID: 6659
	public bool ModuleOverrideActive = true;

	// Token: 0x04001A04 RID: 6660
	public GameObject ModuleOverride;

	// Token: 0x04001A05 RID: 6661
	public Module.Type ModuleType;

	// Token: 0x04001A06 RID: 6662
	public bool OnlyOneModule;

	// Token: 0x04001A07 RID: 6663
	public float LevelSizeMultiplier = 1f;

	// Token: 0x04001A08 RID: 6664
	public int LevelsCompleted;

	// Token: 0x04001A09 RID: 6665
	public bool EnemyDebug = true;

	// Token: 0x04001A0A RID: 6666
	public EnemySetup[] EnemyOverride;

	// Token: 0x04001A0B RID: 6667
	public float EnemyEnableTimeOverride;

	// Token: 0x04001A0C RID: 6668
	public float EnemyDisableTimeOverride;

	// Token: 0x04001A0D RID: 6669
	public bool EnemyDisable;

	// Token: 0x04001A0E RID: 6670
	public bool EnemyNoVision;

	// Token: 0x04001A0F RID: 6671
	public bool EnemySpawnClose;

	// Token: 0x04001A10 RID: 6672
	public bool EnemyDespawnClose;

	// Token: 0x04001A11 RID: 6673
	public bool EnemyInvestigates;

	// Token: 0x04001A12 RID: 6674
	public bool EnemyShortActionTimer;

	// Token: 0x04001A13 RID: 6675
	public bool EnemyNoSpawnedPause;

	// Token: 0x04001A14 RID: 6676
	public bool EnemyNoSpawnIdlePause;

	// Token: 0x04001A15 RID: 6677
	public bool EnemyNoGrabMaxTime;

	// Token: 0x04001A16 RID: 6678
	public bool EnemyEasyGrab;

	// Token: 0x04001A17 RID: 6679
	public bool PlayerDebug;

	// Token: 0x04001A18 RID: 6680
	public bool InfiniteEnergy;

	// Token: 0x04001A19 RID: 6681
	public bool GodMode;

	// Token: 0x04001A1A RID: 6682
	public bool NoTumbleMode;

	// Token: 0x04001A1B RID: 6683
	public bool DebugVoice;

	// Token: 0x04001A1C RID: 6684
	public bool DebugMapActive;

	// Token: 0x04001A1D RID: 6685
	public bool PowerGrabber;

	// Token: 0x04001A1E RID: 6686
	public bool StickyGrabber;

	// Token: 0x04001A1F RID: 6687
	public bool OtherDebug;

	// Token: 0x04001A20 RID: 6688
	public ValuableDirector.ValuableDebug valuableDebug;

	// Token: 0x04001A21 RID: 6689
	public bool DisableMusic;

	// Token: 0x04001A22 RID: 6690
	public bool DisableLoadingLevelAnimation;

	// Token: 0x04001A23 RID: 6691
	public bool LowHaul;

	// Token: 0x04001A24 RID: 6692
	public bool SimulateLag;

	// Token: 0x04001A25 RID: 6693
	public bool StatsDebug;

	// Token: 0x04001A26 RID: 6694
	public int Currency;

	// Token: 0x04001A27 RID: 6695
	public bool EmptyBatteries;

	// Token: 0x04001A28 RID: 6696
	public int StartCrystals = 1;

	// Token: 0x04001A29 RID: 6697
	public bool BuyAllItems;

	// Token: 0x0200037C RID: 892
	public enum StartMode
	{
		// Token: 0x040027B1 RID: 10161
		Normal,
		// Token: 0x040027B2 RID: 10162
		SinglePlayer,
		// Token: 0x040027B3 RID: 10163
		Multiplayer
	}
}
