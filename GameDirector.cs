using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000219 RID: 537
public class GameDirector : MonoBehaviour
{
	// Token: 0x0600115F RID: 4447 RVA: 0x0009AE51 File Offset: 0x00099051
	private void Awake()
	{
		this.MainCamera = Camera.main;
		this.MainCameraParent = this.MainCamera.transform.parent;
		GameDirector.instance = this;
		this.currentState = GameDirector.gameState.Load;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0009AE81 File Offset: 0x00099081
	private void Start()
	{
		RunManager.instance.runStarted = true;
		RunManager.instance.allPlayersDead = false;
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0009AE9C File Offset: 0x0009909C
	private void gameStateLoad()
	{
		if (this.gameStateStartImpulse)
		{
			this.cameraPosition.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Off, 0f);
			this.gameStateStartImpulse = false;
		}
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0009AEEC File Offset: 0x000990EC
	private void gameStateStart()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.1f);
			this.gameStateTimer = 0.5f;
			LoadingUI.instance.LevelAnimationStart();
			this.gameStateStartImpulse = false;
			return;
		}
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
		{
			if (LoadingUI.instance.levelAnimationCompleted)
			{
				this.gameStateTimer -= Time.deltaTime;
			}
		}
		else
		{
			this.gameStateTimer -= Time.deltaTime;
		}
		if (this.gameStateTimer <= 0f)
		{
			LoadingUI.instance.StopLoading();
			if (RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Spectate, 0.1f);
			}
			else
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.On, 0.1f);
			}
			MusicManager.Instance.MusicMixerOff.TransitionTo(0f);
			MusicManager.Instance.MusicMixerOn.TransitionTo(0.1f);
			this.SoundIntro.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (!SemiFunc.MenuLevel())
			{
				this.SoundIntroRun.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.currentState = GameDirector.gameState.Main;
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0009B07E File Offset: 0x0009927E
	private void gameStateMain()
	{
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0009B080 File Offset: 0x00099280
	private void gameStateOutro()
	{
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.25f);
			MusicManager.Instance.MusicMixerScareOnly.TransitionTo(0.25f);
			this.SoundOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (!SemiFunc.MenuLevel())
			{
				this.SoundOutroRun.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.gameStateTimer = 1f;
			this.gameStateStartImpulse = false;
			HUD.instance.Hide();
			using (List<PlayerAvatar>.Enumerator enumerator = this.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerAvatar playerAvatar = enumerator.Current;
					if (playerAvatar.voiceChat)
					{
						playerAvatar.voiceChat.ToggleLobby(true);
					}
				}
				return;
			}
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			this.currentState = GameDirector.gameState.End;
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0009B1C8 File Offset: 0x000993C8
	private void gameStateEnd()
	{
		if (this.gameStateStartImpulse)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Off, 0.5f);
			PlayerController.instance.playerAvatarScript.SetDisabled();
			LoadingUI.instance.StartLoading();
			this.gameStateTimer = 0.5f;
			this.gameStateStartImpulse = false;
			return;
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			PlayerController.instance.playerAvatarScript.OutroDone();
			this.currentState = GameDirector.gameState.EndWait;
		}
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0009B250 File Offset: 0x00099450
	private void gameStateDeath()
	{
		SemiFunc.UIShowSpectate();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideAim();
		if (this.gameStateStartImpulse)
		{
			this.gameStateTimer = 0.5f;
			this.deathFreezeTimer = this.deathFreezeTime;
			this.SoundDeath.Play(base.transform.position, 1f, 1f, 1f, 1f);
			HUD.instance.Hide();
			RenderTextureMain.instance.ChangeResolution(RenderTextureMain.instance.textureWidthOriginal * 0.2f, RenderTextureMain.instance.textureHeightOriginal * 0.2f, this.gameStateTimer);
			this.gameStateStartImpulse = false;
			return;
		}
		if (this.deathFreezeTimer > 0f)
		{
			this.deathFreezeTimer -= Time.deltaTime;
			if (this.deathFreezeTimer <= 0f)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.CutsceneOnly, 0.1f);
				RenderTextureMain.instance.Shake(this.gameStateTimer);
				RenderTextureMain.instance.ChangeSize(1.25f, 1.25f, this.gameStateTimer);
				CameraFreeze.Freeze(this.gameStateTimer + 0.1f);
			}
		}
		this.gameStateTimer -= Time.deltaTime;
		if (this.gameStateTimer <= 0f)
		{
			AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.Spectate, 0.1f);
			HUD.instance.Show();
			RenderTextureMain.instance.Shake(0f);
			RenderTextureMain.instance.sizeResetTimer = 0f;
			RenderTextureMain.instance.textureResetTimer = 0f;
			CameraFreeze.Freeze(0f);
			PlayerController.instance.playerAvatarScript.SetSpectate();
			this.currentState = GameDirector.gameState.Main;
		}
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0009B404 File Offset: 0x00099604
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Menu) && !SemiFunc.MenuLevel() && !ChatManager.instance.chatActive)
		{
			if (SemiFunc.InputDown(InputKey.Back) && ChatManager.instance.StateIsActive())
			{
				return;
			}
			if (!MenuManager.instance.currentMenuPage)
			{
				MenuManager.instance.PageOpen(MenuPageIndex.Escape, false);
			}
			else if (MenuManager.instance.currentMenuPage.menuPageIndex == MenuPageIndex.Escape)
			{
				MenuManager.instance.PageCloseAll();
			}
		}
		if (this.outroStart)
		{
			this.OutroStart();
		}
		if (this.currentState == GameDirector.gameState.Load)
		{
			this.gameStateLoad();
		}
		else if (this.currentState == GameDirector.gameState.Start)
		{
			this.gameStateStart();
		}
		else if (this.currentState == GameDirector.gameState.Main)
		{
			this.gameStateMain();
		}
		else if (this.currentState == GameDirector.gameState.Outro)
		{
			this.gameStateOutro();
		}
		else if (this.currentState == GameDirector.gameState.End)
		{
			this.gameStateEnd();
		}
		else if (this.currentState == GameDirector.gameState.Death)
		{
			this.gameStateDeath();
		}
		if (this.DisableInput)
		{
			this.DisableInputTimer -= Time.deltaTime;
			if (this.DisableInputTimer <= 0f)
			{
				this.DisableInput = false;
			}
		}
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0009B520 File Offset: 0x00099720
	public void OutroStart()
	{
		this.outroStart = true;
		if (this.currentState == GameDirector.gameState.Main)
		{
			this.currentState = GameDirector.gameState.Outro;
			if (FadeOverlay.Instance)
			{
				FadeOverlay.Instance.Image.color = Color.black;
			}
			this.gameStateStartImpulse = true;
			this.gameStateTimer = 0f;
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0009B576 File Offset: 0x00099776
	public void DeathStart()
	{
		this.currentState = GameDirector.gameState.Death;
		this.gameStateStartImpulse = true;
		this.gameStateTimer = 0f;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0009B591 File Offset: 0x00099791
	public void Revive()
	{
		this.currentState = GameDirector.gameState.Main;
		this.gameStateStartImpulse = true;
		this.gameStateTimer = 0f;
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0009B5AC File Offset: 0x000997AC
	public void CommandSetFPS(int _fps)
	{
		Application.targetFrameRate = _fps;
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x0009B5B4 File Offset: 0x000997B4
	public void CommandRecordingDirectorToggle()
	{
		if (RecordingDirector.instance != null)
		{
			Object.Destroy(RecordingDirector.instance.gameObject);
			FlashlightController.Instance.hideFlashlight = false;
			return;
		}
		Object.Instantiate(Resources.Load("Recording Director"));
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0009B5F0 File Offset: 0x000997F0
	public void CommandGreenScreenToggle()
	{
		if (this.greenScreenActive)
		{
			Object.Destroy(VideoGreenScreen.instance.gameObject);
			HurtVignette.instance.gameObject.SetActive(true);
			this.greenScreenActive = false;
			return;
		}
		Object.Instantiate<GameObject>(this.greenScreenPrefab);
		HurtVignette.instance.gameObject.SetActive(false);
		this.greenScreenActive = true;
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0009B64F File Offset: 0x0009984F
	public void SetDisableInput(float time)
	{
		this.DisableInput = true;
		this.DisableInputTimer = time;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0009B65F File Offset: 0x0009985F
	public void SetStart()
	{
		this.gameStateStartImpulse = true;
		this.currentState = GameDirector.gameState.Start;
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0009B66F File Offset: 0x0009986F
	private void LateUpdate()
	{
		this.FPSImpulses();
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0009B678 File Offset: 0x00099878
	private void FPSImpulses()
	{
		float deltaTime = Time.deltaTime;
		this.fpsImpulse1 = false;
		this.fpsImpulse5 = false;
		this.fpsImpulse15 = false;
		this.fpsImpulse30 = false;
		this.fpsImpulse60 = false;
		this.timer1FPS += deltaTime;
		this.timer5FPS += deltaTime;
		this.timer15FPS += deltaTime;
		this.timer30FPS += deltaTime;
		this.timer60FPS += deltaTime;
		while (this.timer1FPS >= 1f)
		{
			this.fpsImpulse1 = true;
			this.timer1FPS -= 1f;
		}
		while (this.timer5FPS >= 0.2f)
		{
			this.fpsImpulse5 = true;
			this.timer5FPS -= 0.2f;
		}
		while (this.timer15FPS >= 0.06666667f)
		{
			this.fpsImpulse15 = true;
			this.timer15FPS -= 0.06666667f;
		}
		while (this.timer30FPS >= 0.033333335f)
		{
			this.fpsImpulse30 = true;
			this.timer30FPS -= 0.033333335f;
		}
		while (this.timer60FPS >= 0.016666668f)
		{
			this.fpsImpulse60 = true;
			this.timer60FPS -= 0.016666668f;
		}
	}

	// Token: 0x04001D2A RID: 7466
	public static GameDirector instance;

	// Token: 0x04001D2B RID: 7467
	public GameDirector.gameState currentState = GameDirector.gameState.Start;

	// Token: 0x04001D2C RID: 7468
	public bool LevelCompleted;

	// Token: 0x04001D2D RID: 7469
	public bool LevelCompletedDone;

	// Token: 0x04001D2E RID: 7470
	[Header("Debug")]
	public float TimeScale = 1f;

	// Token: 0x04001D2F RID: 7471
	[Space(15f)]
	public bool RandomSeed;

	// Token: 0x04001D30 RID: 7472
	public int Seed;

	// Token: 0x04001D31 RID: 7473
	[Space(15f)]
	[Header("Audio")]
	public AudioMixerSnapshot volumeOff;

	// Token: 0x04001D32 RID: 7474
	public AudioMixerSnapshot volumeOn;

	// Token: 0x04001D33 RID: 7475
	public AudioMixerSnapshot volumeCutsceneOnly;

	// Token: 0x04001D34 RID: 7476
	public Sound SoundIntro;

	// Token: 0x04001D35 RID: 7477
	public Sound SoundIntroRun;

	// Token: 0x04001D36 RID: 7478
	public Sound SoundOutro;

	// Token: 0x04001D37 RID: 7479
	public Sound SoundOutroRun;

	// Token: 0x04001D38 RID: 7480
	public Sound SoundDeath;

	// Token: 0x04001D39 RID: 7481
	[Space(15f)]
	[Header("Enemy")]
	public bool LevelEnemyChasing;

	// Token: 0x04001D3A RID: 7482
	[Space(15f)]
	[Header("Other")]
	public Camera MainCamera;

	// Token: 0x04001D3B RID: 7483
	[HideInInspector]
	public Transform MainCameraParent;

	// Token: 0x04001D3C RID: 7484
	public RenderTexture MainRenderTexture;

	// Token: 0x04001D3D RID: 7485
	[Space(15f)]
	public CameraTarget camTarget;

	// Token: 0x04001D3E RID: 7486
	public AnimNoise camNoise;

	// Token: 0x04001D3F RID: 7487
	private float gameStateTimer;

	// Token: 0x04001D40 RID: 7488
	private bool gameStateStartImpulse = true;

	// Token: 0x04001D41 RID: 7489
	[Space(15f)]
	public GameObject cameraPosition;

	// Token: 0x04001D42 RID: 7490
	public Animator cameraTargetAnimator;

	// Token: 0x04001D43 RID: 7491
	public CameraShake CameraShake;

	// Token: 0x04001D44 RID: 7492
	public CameraShake CameraImpact;

	// Token: 0x04001D45 RID: 7493
	public CameraBob CameraBob;

	// Token: 0x04001D46 RID: 7494
	[Space(15f)]
	public int InitialCleaningSpots;

	// Token: 0x04001D47 RID: 7495
	public List<PlayerAvatar> PlayerList = new List<PlayerAvatar>();

	// Token: 0x04001D48 RID: 7496
	public bool DisableInput;

	// Token: 0x04001D49 RID: 7497
	private float DisableInputTimer;

	// Token: 0x04001D4A RID: 7498
	internal bool outroStart;

	// Token: 0x04001D4B RID: 7499
	private float deathFreezeTime = 0.2f;

	// Token: 0x04001D4C RID: 7500
	private float deathFreezeTimer;

	// Token: 0x04001D4D RID: 7501
	private float timer1FPS;

	// Token: 0x04001D4E RID: 7502
	private float timer5FPS;

	// Token: 0x04001D4F RID: 7503
	private float timer15FPS;

	// Token: 0x04001D50 RID: 7504
	private float timer30FPS;

	// Token: 0x04001D51 RID: 7505
	private float timer60FPS;

	// Token: 0x04001D52 RID: 7506
	private const float INTERVAL_1FPS = 1f;

	// Token: 0x04001D53 RID: 7507
	private const float INTERVAL_5FPS = 0.2f;

	// Token: 0x04001D54 RID: 7508
	private const float INTERVAL_15FPS = 0.06666667f;

	// Token: 0x04001D55 RID: 7509
	private const float INTERVAL_30FPS = 0.033333335f;

	// Token: 0x04001D56 RID: 7510
	private const float INTERVAL_60FPS = 0.016666668f;

	// Token: 0x04001D57 RID: 7511
	internal bool fpsImpulse1;

	// Token: 0x04001D58 RID: 7512
	internal bool fpsImpulse5;

	// Token: 0x04001D59 RID: 7513
	internal bool fpsImpulse15;

	// Token: 0x04001D5A RID: 7514
	internal bool fpsImpulse30;

	// Token: 0x04001D5B RID: 7515
	internal bool fpsImpulse60;

	// Token: 0x04001D5C RID: 7516
	internal bool greenScreenActive;

	// Token: 0x04001D5D RID: 7517
	public GameObject greenScreenPrefab;

	// Token: 0x020003A5 RID: 933
	public enum gameState
	{
		// Token: 0x0400286B RID: 10347
		Load,
		// Token: 0x0400286C RID: 10348
		Start,
		// Token: 0x0400286D RID: 10349
		Main,
		// Token: 0x0400286E RID: 10350
		Outro,
		// Token: 0x0400286F RID: 10351
		End,
		// Token: 0x04002870 RID: 10352
		EndWait,
		// Token: 0x04002871 RID: 10353
		Death
	}
}
