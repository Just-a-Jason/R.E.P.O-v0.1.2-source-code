using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class Arena : MonoBehaviour
{
	// Token: 0x06000758 RID: 1880 RVA: 0x00045BE1 File Offset: 0x00043DE1
	private void Awake()
	{
		Arena.instance = this;
		this.ArenaInit();
		SessionManager.instance.ResetCrown();
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00045BF9 File Offset: 0x00043DF9
	private void ArenaInit()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.ArenaInitMultiplayer();
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00045C10 File Offset: 0x00043E10
	private void ArenaInitMultiplayer()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.itemVolumes.childCount; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < this.itemVolumes.childCount; j++)
		{
			int index = Random.Range(0, list.Count);
			this.itemVolumes.GetChild(j).SetSiblingIndex(list[index]);
			list.RemoveAt(index);
		}
		this.itemsMelee.Shuffle<Item>();
		this.itemsGuns.Shuffle<Item>();
		this.itemsCarts.Shuffle<Item>();
		this.itemsDronesAndOrbs.Shuffle<Item>();
		this.itemsHealth.Shuffle<Item>();
		this.itemsUsables.Shuffle<Item>();
		this.itemsMid = new List<Item>();
		this.itemsMid.AddRange(this.itemsMelee);
		this.itemsMid.AddRange(this.itemsGuns);
		ItemManager.instance.ResetAllItems();
		for (int k = 0; k < 1; k++)
		{
			ItemManager.instance.purchasedItems.Add(this.itemsUsables[k]);
		}
		for (int l = 0; l < 5; l++)
		{
			ItemManager.instance.purchasedItems.Add(this.itemsMelee[l]);
		}
		for (int m = 0; m < 3; m++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsGuns[m]);
			}
		}
		for (int n = 0; n < 1; n++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsCarts[n]);
			}
		}
		for (int num = 0; num < 3; num++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsDronesAndOrbs[num]);
			}
		}
		for (int num2 = 0; num2 < 3; num2++)
		{
			if (Random.Range(0, 100) < 30)
			{
				ItemManager.instance.purchasedItems.Add(this.itemsHealth[num2]);
			}
		}
		ItemManager.instance.GetAllItemVolumesInScene();
		PunManager.instance.TruckPopulateItemVolumes();
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00045E48 File Offset: 0x00044048
	private void Start()
	{
		if (this.crownTransform)
		{
			this.crownTransformPosition = this.crownTransform.position;
		}
		else
		{
			this.crownTransformPosition = Vector3.zero;
		}
		this.numberOfPlayers = SemiFunc.PlayerGetAll().Count;
		this.photonView = base.GetComponent<PhotonView>();
		this.platforms = new List<ArenaPlatform>();
		this.platforms.AddRange(base.GetComponentsInChildren<ArenaPlatform>());
		this.floorDoorStartPos = this.floorDoorTransform.localPosition;
		this.floorDoorEndPos = new Vector3(this.floorDoorStartPos.x, this.floorDoorStartPos.y, 8.25f);
		this.startPosCrownMechanicLineTransform = this.crownMechanicLineTransform.localPosition.y;
		this.playersAlive = SemiFunc.PlayerGetAll().Count;
		this.playersAlivePrev = this.playersAlive;
		this.pedistalScreens = new List<ArenaPedistalScreen>();
		this.pedistalScreens.AddRange(base.GetComponentsInChildren<ArenaPedistalScreen>());
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(this.playersAlive, false);
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00045F88 File Offset: 0x00044188
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00045F9C File Offset: 0x0004419C
	private void StateFalling()
	{
		if (this.stateStart)
		{
			this.pipeLights.SetActive(true);
			this.stateStart = false;
			this.hurtCollider.SetActive(true);
			this.stateTimer = 5f;
			this.soundArenaHatchOpen.Play(this.floorDoorTransform.position, 1f, 1f, 1f, 1f);
			CameraGlitch.Instance.PlayLong();
			GameDirector.instance.CameraShake.Shake(4f, 0.25f);
			GameDirector.instance.CameraImpact.Shake(4f, 0.1f);
			this.musicToggle = true;
			this.musicSource.time = 0f;
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetAll())
				{
					playerAvatar.playerHealth.InvincibleSet(5f);
				}
			}
			if (this.numberOfPlayers < 2 || !SemiFunc.IsMultiplayer())
			{
				foreach (ArenaPlatform arenaPlatform in this.platforms)
				{
					arenaPlatform.StateSet(ArenaPlatform.States.GoDown);
				}
			}
		}
		if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			ArenaMessageUI.instance.ArenaText("LAST LOSER STANDING");
		}
		else
		{
			ArenaMessageUI.instance.ArenaText("GAME OVER");
		}
		if (SemiFunc.FPSImpulse5())
		{
			foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetAll())
			{
				playerAvatar2.FallDamageResetSet(1f);
			}
		}
		this.floorDoorAnimationProgress += Time.deltaTime * 2f;
		if (this.floorDoorAnimationProgress >= 1f)
		{
			this.floorDoorAnimationProgress = 1f;
		}
		this.floorDoorTransform.localPosition = Vector3.Lerp(this.floorDoorStartPos, this.floorDoorEndPos, this.floorDoorCurve.Evaluate(this.floorDoorAnimationProgress));
		if (this.stateTimer <= 0f)
		{
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				this.StateSet(Arena.States.Starting);
				return;
			}
			this.StateSet(Arena.States.GameOver);
		}
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0004620C File Offset: 0x0004440C
	private void StateStarting()
	{
		if (this.stateStart)
		{
			this.pipeLights.SetActive(false);
			this.stateStart = false;
			this.hurtCollider.SetActive(false);
			this.safeBars.SetActive(false);
			this.stateTimer = 2f;
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(Arena.States.Level1);
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x0004626C File Offset: 0x0004446C
	private void StatePlatformWarning()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (this.stateStart)
		{
			this.level++;
			this.nextLevel = this.level + Arena.States.Level1;
			this.stateStart = false;
			int index = this.level - 1;
			this.platforms[index].StateSet(ArenaPlatform.States.Warning);
			this.platforms[index].PulsateLights();
			this.soundArenaWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
			GameDirector.instance.CameraShake.Shake(4f, 0.25f);
			GameDirector.instance.CameraImpact.Shake(4f, 0.1f);
			this.stateTimer = 3f;
		}
		if (this.stateTimer % 1f < 0.1f)
		{
			if (!this.warningSound)
			{
				this.warningSound = true;
				if (this.stateTimer > 1f)
				{
					this.soundArenaWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
					GameDirector.instance.CameraShake.Shake(2f, 0.25f);
					GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
				}
				int index2 = this.level - 1;
				this.platforms[index2].PulsateLights();
			}
		}
		else
		{
			this.warningSound = false;
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(Arena.States.PlatformRemove);
		}
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0004640C File Offset: 0x0004460C
	private void StatePlatformRemove()
	{
		if (this.stateStart)
		{
			int index = this.level - 1;
			this.platforms[index].StateSet(ArenaPlatform.States.GoDown);
			this.stateStart = false;
			this.soundArenaRemove.Play(base.transform.position, 1f, 1f, 1f, 1f);
			GameDirector.instance.CameraShake.Shake(8f, 0.5f);
			GameDirector.instance.CameraImpact.Shake(8f, 0.1f);
			this.stateTimer = 3f;
		}
		if (this.stateTimer <= 0f && !this.finalPlatform)
		{
			this.StateSet(this.nextLevel);
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000464CF File Offset: 0x000446CF
	private void StateLevel1()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.NextLevel();
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000464FE File Offset: 0x000446FE
	private void StateLevel2()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.NextLevel();
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0004652D File Offset: 0x0004472D
	private void StateLevel3()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 30f;
		}
		if (this.stateTimer <= 0f)
		{
			this.finalPlatform = true;
			this.NextLevel();
		}
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00046564 File Offset: 0x00044764
	private void StateGameOver()
	{
		if (this.stateStart)
		{
			this.musicToggle = false;
			this.stateStart = false;
			if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
			{
				this.stateTimer = 10f;
			}
			else
			{
				this.stateTimer = 3f;
			}
			if (!this.winnerPlayer)
			{
				this.soundArenaMusicLoseJingle.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		if (this.winnerPlayer)
		{
			ArenaMessageWinUI.instance.ArenaText("KING OF THE LOSERS!", true);
		}
		else if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			ArenaMessageWinUI.instance.ArenaText("EVERYONE'S A LOSER!", false);
		}
		if (this.stateTimer <= 0f && SemiFunc.IsMasterClientOrSingleplayer())
		{
			RunManager.instance.ChangeLevel(false, true, RunManager.ChangeLevelType.Normal);
		}
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00046647 File Offset: 0x00044847
	private void GameOver()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00046658 File Offset: 0x00044858
	private void NextLevel()
	{
		this.StateSet(Arena.States.PlatformWarning);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00046664 File Offset: 0x00044864
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case Arena.States.Idle:
			this.StateIdle();
			return;
		case Arena.States.Level1:
			this.StateLevel1();
			return;
		case Arena.States.Level2:
			this.StateLevel2();
			return;
		case Arena.States.Level3:
			this.StateLevel3();
			return;
		case Arena.States.Falling:
			this.StateFalling();
			return;
		case Arena.States.Starting:
			this.StateStarting();
			return;
		case Arena.States.PlatformWarning:
			this.StatePlatformWarning();
			return;
		case Arena.States.PlatformRemove:
			this.StatePlatformRemove();
			return;
		case Arena.States.GameOver:
			this.StateGameOver();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x000466E4 File Offset: 0x000448E4
	private void Update()
	{
		this.StateMachine();
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideGoal();
		this.MusicLogic();
		if (this.numberOfPlayers > 1 && SemiFunc.IsMultiplayer())
		{
			this.CrownVisuals();
			this.CrownLogic();
			this.MainLightAnimation();
			this.SpawnMidWeapons();
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00046758 File Offset: 0x00044958
	private void MusicLogic()
	{
		this.soundArenaMusic.PlayLoop(this.musicToggle, 20f, 2f, 1f);
		if (!this.musicTogglePrev && this.musicToggle)
		{
			this.soundArenaMusic.Source.time = 0f;
			this.musicTogglePrev = this.musicToggle;
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000467B8 File Offset: 0x000449B8
	private void SpawnMidWeapons()
	{
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient() && this.playersAlive > 1 && this.level >= 2 && this.currentState != Arena.States.GameOver)
		{
			if (this.midSpawnerTimer > 5f)
			{
				Item item = this.itemsMid[Random.Range(0, this.itemsMid.Count)];
				PhotonNetwork.Instantiate("Items/" + item.itemAssetName, this.itemsMidSpawner.position, this.itemsMidSpawner.rotation, 0, null);
				this.midSpawnerTimer = 0f;
				return;
			}
			this.midSpawnerTimer += Time.deltaTime;
		}
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00046870 File Offset: 0x00044A70
	private void MainLightAnimation()
	{
		if (!this.arenaMainLight.enabled)
		{
			return;
		}
		if (this.arenaMainLight.intensity > 0.05f)
		{
			this.arenaMainLight.intensity = Mathf.Lerp(this.arenaMainLight.intensity, 0f, Time.deltaTime * 2f);
			return;
		}
		this.arenaMainLight.enabled = false;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x000468D8 File Offset: 0x00044AD8
	private void CrownLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentState == Arena.States.GameOver)
		{
			return;
		}
		List<PlayerAvatar> list = SemiFunc.PlayerGetAll();
		int count = list.Count;
		this.playersAlive = count;
		using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isDisabled)
				{
					this.playersAlive--;
				}
			}
		}
		if (this.playersAlivePrev != this.playersAlive)
		{
			if ((this.playersAlive > 1 || this.playersAlive == 0) && SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("PlayerKilledRPC", RpcTarget.All, new object[]
				{
					this.playersAlive
				});
			}
			this.playersAlivePrev = this.playersAlive;
		}
		if (this.playersAlive <= 0)
		{
			this.StateSet(Arena.States.GameOver);
			return;
		}
		if (SemiFunc.FPSImpulse15() && !this.crownCageDestroyed && this.playersAlive < 2)
		{
			this.DestroyCrownCage();
			this.crownCageDestroyed = true;
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x000469E4 File Offset: 0x00044BE4
	private void AllPlayersKilled()
	{
		this.soundAllPlayersDead.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.arenaMainLight.color = new Color(0f, 1f, 0f);
		this.arenaMainLight.enabled = true;
		this.arenaMainLight.intensity = 10f;
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(1, true);
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00046A9C File Offset: 0x00044C9C
	[PunRPC]
	private void PlayerKilledRPC(int _playersAlive)
	{
		this.soundArenaPlayerEliminated.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.arenaMainLight.color = new Color(0.8f, 0.3f, 0f);
		this.arenaMainLight.enabled = true;
		this.arenaMainLight.intensity = 8f;
		this.playersAlive = _playersAlive;
		foreach (ArenaPedistalScreen arenaPedistalScreen in this.pedistalScreens)
		{
			arenaPedistalScreen.SwitchNumber(this.playersAlive, false);
		}
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00046B60 File Offset: 0x00044D60
	private void CrownVisuals()
	{
		if (this.currentState == Arena.States.GameOver)
		{
			return;
		}
		if (!this.crownTransform)
		{
			return;
		}
		this.crownTransform.Rotate(Vector3.up, Time.deltaTime * 50f);
		this.crownTransform.localRotation = Quaternion.Euler(this.crownTransform.localRotation.x, this.crownTransform.localRotation.y + Time.time * 50f, 20f * Mathf.Sin(Time.time * 2f));
		if (this.crownCageDestroyed)
		{
			return;
		}
		this.crownSphere.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
		this.crownMechanic.Rotate(Vector3.up, Time.deltaTime * 500f);
		float num = 0.05f;
		float num2 = 2f;
		this.crownMechanicLineTransform.localPosition = new Vector3(this.crownMechanicLineTransform.localPosition.x, this.startPosCrownMechanicLineTransform - num / 2f + num * Mathf.Sin(Time.time * num2), this.crownMechanicLineTransform.localPosition.z);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x00046C94 File Offset: 0x00044E94
	public void StateSet(Arena.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.StateSetRPC(state);
			return;
		}
		this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00046CCD File Offset: 0x00044ECD
	[PunRPC]
	public void StateSetRPC(Arena.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00046CDD File Offset: 0x00044EDD
	private void DestroyCrownCage()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("DestroyCrownCageRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.DestroyCrownCageRPC();
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00046D04 File Offset: 0x00044F04
	public void CrownGrab()
	{
		if (SemiFunc.IsMasterClient())
		{
			int viewID = this.crownGrab.playerGrabbing[0].photonView.ViewID;
			this.photonView.RPC("CrownGrabRPC", RpcTarget.All, new object[]
			{
				viewID
			});
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00046D54 File Offset: 0x00044F54
	[PunRPC]
	public void CrownGrabRPC(int photonViewID)
	{
		if (this.winnerPlayer)
		{
			return;
		}
		PhysGrabber component = PhotonView.Find(photonViewID).GetComponent<PhysGrabber>();
		this.winnerPlayer = component.playerAvatar;
		SessionManager.instance.crownedPlayerSteamID = this.winnerPlayer.steamID;
		ArenaMessageWinUI.instance.kingObject.GetComponent<MenuPlayerListed>().ForcePlayer(this.winnerPlayer);
		this.crownExplosion.SetActive(true);
		this.soundArenaMusicWinJingle.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00046DF0 File Offset: 0x00044FF0
	[PunRPC]
	public void DestroyCrownCageRPC()
	{
		if (this.crownCageDestroyed)
		{
			return;
		}
		this.musicToggle = false;
		this.AllPlayersKilled();
		this.soundCrownCageDestroy.Play(this.crownTransformPosition, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(10f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(10f, 0.1f);
		if (this.crownCageDestroyParticles)
		{
			this.crownCageDestroyParticles.SetActive(true);
		}
		if (this.crownSphere)
		{
			Object.Destroy(this.crownSphere.gameObject);
		}
		if (this.crownMechanic)
		{
			Object.Destroy(this.crownMechanic.gameObject);
		}
		this.crownCageDestroyed = true;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00046EC5 File Offset: 0x000450C5
	public void OpenHatch()
	{
		this.StateSet(Arena.States.Falling);
	}

	// Token: 0x04000CF4 RID: 3316
	private PhotonView photonView;

	// Token: 0x04000CF5 RID: 3317
	private List<ArenaPlatform> platforms;

	// Token: 0x04000CF6 RID: 3318
	public static Arena instance;

	// Token: 0x04000CF7 RID: 3319
	[Space(10f)]
	[Header("Items")]
	public List<Item> itemsUsables;

	// Token: 0x04000CF8 RID: 3320
	public List<Item> itemsMelee;

	// Token: 0x04000CF9 RID: 3321
	public List<Item> itemsGuns;

	// Token: 0x04000CFA RID: 3322
	public List<Item> itemsCarts;

	// Token: 0x04000CFB RID: 3323
	public List<Item> itemsDronesAndOrbs;

	// Token: 0x04000CFC RID: 3324
	public List<Item> itemsHealth;

	// Token: 0x04000CFD RID: 3325
	private List<Item> itemsMid;

	// Token: 0x04000CFE RID: 3326
	[Space(10f)]
	public Transform floorDoorTransform;

	// Token: 0x04000CFF RID: 3327
	public GameObject hurtCollider;

	// Token: 0x04000D00 RID: 3328
	public GameObject startStuff;

	// Token: 0x04000D01 RID: 3329
	public Transform pipeTransform;

	// Token: 0x04000D02 RID: 3330
	public GameObject safeBars;

	// Token: 0x04000D03 RID: 3331
	public AnimationCurve floorDoorCurve;

	// Token: 0x04000D04 RID: 3332
	public Transform itemVolumes;

	// Token: 0x04000D05 RID: 3333
	public MeshRenderer crownSphere;

	// Token: 0x04000D06 RID: 3334
	public Transform crownTransform;

	// Token: 0x04000D07 RID: 3335
	public GameObject crownPlatform;

	// Token: 0x04000D08 RID: 3336
	public Transform crownMechanic;

	// Token: 0x04000D09 RID: 3337
	public Transform crownMechanicLineTransform;

	// Token: 0x04000D0A RID: 3338
	public GameObject crownCageDestroyParticles;

	// Token: 0x04000D0B RID: 3339
	public StaticGrabObject crownGrab;

	// Token: 0x04000D0C RID: 3340
	public GameObject crownExplosion;

	// Token: 0x04000D0D RID: 3341
	public Transform itemsMidSpawner;

	// Token: 0x04000D0E RID: 3342
	private Vector3 crownTransformPosition;

	// Token: 0x04000D0F RID: 3343
	private bool stateStart;

	// Token: 0x04000D10 RID: 3344
	private float stateTimer;

	// Token: 0x04000D11 RID: 3345
	private Vector3 floorDoorStartPos;

	// Token: 0x04000D12 RID: 3346
	private Vector3 floorDoorEndPos;

	// Token: 0x04000D13 RID: 3347
	private float floorDoorAnimationProgress;

	// Token: 0x04000D14 RID: 3348
	private float startPosCrownMechanicLineTransform;

	// Token: 0x04000D15 RID: 3349
	private float midSpawnerTimer;

	// Token: 0x04000D16 RID: 3350
	private List<ArenaPedistalScreen> pedistalScreens;

	// Token: 0x04000D17 RID: 3351
	internal PlayerAvatar winnerPlayer;

	// Token: 0x04000D18 RID: 3352
	internal Arena.States currentState;

	// Token: 0x04000D19 RID: 3353
	internal Arena.States nextLevel = Arena.States.Level1;

	// Token: 0x04000D1A RID: 3354
	private int level;

	// Token: 0x04000D1B RID: 3355
	private bool warningSound;

	// Token: 0x04000D1C RID: 3356
	private AudioClip soundWarning;

	// Token: 0x04000D1D RID: 3357
	private bool finalPlatform;

	// Token: 0x04000D1E RID: 3358
	private bool crownCageDestroyed;

	// Token: 0x04000D1F RID: 3359
	private int numberOfPlayers;

	// Token: 0x04000D20 RID: 3360
	private bool musicToggle;

	// Token: 0x04000D21 RID: 3361
	private bool musicTogglePrev;

	// Token: 0x04000D22 RID: 3362
	public AudioSource musicSource;

	// Token: 0x04000D23 RID: 3363
	private int playersAlive = 6;

	// Token: 0x04000D24 RID: 3364
	private int playersAlivePrev = 6;

	// Token: 0x04000D25 RID: 3365
	public GameObject pipeLights;

	// Token: 0x04000D26 RID: 3366
	public Light arenaMainLight;

	// Token: 0x04000D27 RID: 3367
	public Sound soundArenaStart;

	// Token: 0x04000D28 RID: 3368
	public Sound soundArenaWarning;

	// Token: 0x04000D29 RID: 3369
	public Sound soundArenaRemove;

	// Token: 0x04000D2A RID: 3370
	public Sound soundArenaPlayerEliminated;

	// Token: 0x04000D2B RID: 3371
	public Sound soundArenaHatchOpen;

	// Token: 0x04000D2C RID: 3372
	public Sound soundArenaMusic;

	// Token: 0x04000D2D RID: 3373
	public Sound soundArenaMusicWinJingle;

	// Token: 0x04000D2E RID: 3374
	public Sound soundArenaMusicLoseJingle;

	// Token: 0x04000D2F RID: 3375
	public Sound soundCrownCageDestroy;

	// Token: 0x04000D30 RID: 3376
	public Sound soundAllPlayersDead;

	// Token: 0x020002FC RID: 764
	public enum States
	{
		// Token: 0x04002556 RID: 9558
		Idle,
		// Token: 0x04002557 RID: 9559
		Level1,
		// Token: 0x04002558 RID: 9560
		Level2,
		// Token: 0x04002559 RID: 9561
		Level3,
		// Token: 0x0400255A RID: 9562
		Falling,
		// Token: 0x0400255B RID: 9563
		Starting,
		// Token: 0x0400255C RID: 9564
		PlatformWarning,
		// Token: 0x0400255D RID: 9565
		PlatformRemove,
		// Token: 0x0400255E RID: 9566
		GameOver
	}
}
