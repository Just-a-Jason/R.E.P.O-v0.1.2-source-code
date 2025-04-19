using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000245 RID: 581
public class LoadingUI : MonoBehaviour
{
	// Token: 0x06001234 RID: 4660 RVA: 0x000A07F4 File Offset: 0x0009E9F4
	private void Awake()
	{
		LoadingUI.instance = this;
		this.fadeBehindImage.gameObject.SetActive(false);
		this.animator = base.GetComponent<Animator>();
		this.animator.enabled = false;
		this.animator.keepAnimatorStateOnDisable = true;
		if (RunManager.instance)
		{
			this.fadeImage.color = RunManager.instance.loadingFadeColor;
			this.animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
		}
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x000A0878 File Offset: 0x0009EA78
	private void LateUpdate()
	{
		if (RunManager.instance.skipLoadingUI)
		{
			return;
		}
		float num = Time.deltaTime;
		if (!this.levelAnimationStarted)
		{
			num = Mathf.Min(num, 0.01f);
		}
		this.animator.Update(num);
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait || (GameDirector.instance.currentState == GameDirector.gameState.Start && !this.levelAnimationCompleted))
		{
			this.fadeImage.color = Color.Lerp(this.fadeImage.color, new Color(0f, 0f, 0f, 0f), 5f * num);
		}
		else
		{
			this.fadeImage.color = Color.Lerp(this.fadeImage.color, Color.black, 20f * num);
		}
		RunManager.instance.loadingFadeColor = this.fadeImage.color;
		RunManager.instance.loadingAnimationTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (GameDirector.instance.PlayerList.Count > 0)
		{
			bool flag = true;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.levelAnimationCompleted)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.levelAnimationCompleted = true;
			}
		}
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x000A09F8 File Offset: 0x0009EBF8
	public void StopLoading()
	{
		RunManager.instance.loadingFadeColor = Color.black;
		RunManager.instance.skipLoadingUI = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x000A0A20 File Offset: 0x0009EC20
	public void StartLoading()
	{
		this.levelAnimationStarted = false;
		base.gameObject.SetActive(true);
		this.fadeImage.color = RunManager.instance.loadingFadeColor;
		this.animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x000A0A70 File Offset: 0x0009EC70
	public void LevelAnimationStart()
	{
		this.levelAnimationStarted = true;
		if (!RunManager.instance.skipLoadingUI && !this.debugDisableLevelAnimation && (SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena()))
		{
			this.loadingGraphic01.sprite = LevelGenerator.Instance.Level.LoadingGraphic01;
			this.loadingGraphic02.sprite = LevelGenerator.Instance.Level.LoadingGraphic02;
			this.loadingGraphic03.sprite = LevelGenerator.Instance.Level.LoadingGraphic03;
			if (SemiFunc.RunIsShop())
			{
				this.levelNumberText.text = "SHOP";
			}
			else if (SemiFunc.RunIsArena())
			{
				this.levelNumberText.text = "GAME OVER";
				this.levelNumberText.color = Color.red;
			}
			else
			{
				this.levelNumberText.text = "LEVEL " + (RunManager.instance.levelsCompleted + 1).ToString();
			}
			this.levelNameText.text = LevelGenerator.Instance.Level.NarrativeName.ToUpper();
			this.animator.SetTrigger("Level");
			return;
		}
		this.levelAnimationCompleted = true;
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x000A0BA6 File Offset: 0x0009EDA6
	public void LevelAnimationComplete()
	{
		PlayerController.instance.playerAvatarScript.LoadingLevelAnimationCompleted();
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x000A0BB7 File Offset: 0x0009EDB7
	public void PlayTurn()
	{
		this.soundTurn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x000A0BE4 File Offset: 0x0009EDE4
	public void PlayRevUp()
	{
		this.soundRevUp.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x000A0C11 File Offset: 0x0009EE11
	public void PlayCrash()
	{
		this.soundCrash.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000A0C3E File Offset: 0x0009EE3E
	public void PlayTextLevel()
	{
		this.soundtextLevel.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x000A0C6B File Offset: 0x0009EE6B
	public void PlayTextName()
	{
		this.soundtextName.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001EE9 RID: 7913
	public static LoadingUI instance;

	// Token: 0x04001EEA RID: 7914
	public Image fadeImage;

	// Token: 0x04001EEB RID: 7915
	public Image fadeBehindImage;

	// Token: 0x04001EEC RID: 7916
	[Space]
	public TextMeshProUGUI levelNumberText;

	// Token: 0x04001EED RID: 7917
	public TextMeshProUGUI levelNameText;

	// Token: 0x04001EEE RID: 7918
	[Space]
	public Image loadingGraphic01;

	// Token: 0x04001EEF RID: 7919
	public Image loadingGraphic02;

	// Token: 0x04001EF0 RID: 7920
	public Image loadingGraphic03;

	// Token: 0x04001EF1 RID: 7921
	private Animator animator;

	// Token: 0x04001EF2 RID: 7922
	internal bool levelAnimationStarted;

	// Token: 0x04001EF3 RID: 7923
	internal bool levelAnimationCompleted;

	// Token: 0x04001EF4 RID: 7924
	internal bool debugDisableLevelAnimation;

	// Token: 0x04001EF5 RID: 7925
	[Space]
	public Sound soundTurn;

	// Token: 0x04001EF6 RID: 7926
	public Sound soundRevUp;

	// Token: 0x04001EF7 RID: 7927
	public Sound soundCrash;

	// Token: 0x04001EF8 RID: 7928
	public Sound soundtextLevel;

	// Token: 0x04001EF9 RID: 7929
	public Sound soundtextName;
}
