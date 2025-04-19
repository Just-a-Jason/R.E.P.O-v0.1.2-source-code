using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000F1 RID: 241
public class TruckScreenText : MonoBehaviour
{
	// Token: 0x0600085C RID: 2140 RVA: 0x000504EE File Offset: 0x0004E6EE
	private void Awake()
	{
		TruckScreenText.instance = this;
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000504F8 File Offset: 0x0004E6F8
	private void Start()
	{
		this.truckScreenLocked = base.GetComponentInChildren<TuckScreenLocked>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				if (textLine.typingSpeed)
				{
					textLine.letterRevealDelay = textLine.typingSpeed.GetDelay();
				}
				else
				{
					textLine.letterRevealDelay = this.textRevealNormalSetting.GetDelay();
				}
				if (textLine.messageDelayTime)
				{
					textLine.delayAfter = textLine.messageDelayTime.GetDelay();
				}
				else
				{
					textLine.delayAfter = this.nextMessageDelayNormalSetting.GetDelay();
				}
			}
		}
		this.screenActive = true;
		if (this.textMesh == null)
		{
			Debug.LogError("TextMeshProUGUI component is not assigned.");
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.currentNickname = this.nicknameTaxman;
		this.chatMessageString = SemiFunc.EmojiText(this.chatMessageString);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x00050648 File Offset: 0x0004E848
	private void LobbyScreenStartLogic()
	{
		if (this.lobbyStarted)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.screenType == TruckScreenText.ScreenType.TruckLobbyScreen && RunManager.instance.levelFailed)
		{
			if (RunManager.instance.runLives == 2)
			{
				this.GotoPage(1);
			}
			if (RunManager.instance.runLives == 1)
			{
				this.GotoPage(2);
			}
			if (RunManager.instance.runLives == 0)
			{
				this.GotoPage(3);
			}
		}
		this.lobbyStarted = true;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x000506BC File Offset: 0x0004E8BC
	public void TutorialFinish()
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			playerAvatar.OutroStartRPC();
		}
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0005071C File Offset: 0x0004E91C
	public void ArrowPointAtGoal()
	{
		this.arrowPointAtGoalTimer = 4f;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0005072C File Offset: 0x0004E92C
	private void ArrowPointAtGoalLogic()
	{
		if (this.arrowPointAtGoalTimer > 0f)
		{
			if (PlayerAvatar.instance.RoomVolumeCheck.inTruck)
			{
				SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(610f, 330f, 0f), 45f);
			}
			this.arrowPointAtGoalTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0005079C File Offset: 0x0004E99C
	private void ChatMessageIdleStringTick()
	{
		if (this.chatActive)
		{
			return;
		}
		if (this.chatMessageIdleStringTimer < 1f)
		{
			this.chatMessageIdleStringTimer += Time.deltaTime;
			return;
		}
		if (this.chatMessageIdleStringCurrent == this.chatMessageIdleString1)
		{
			this.chatMessageIdleStringCurrent = this.chatMessageIdleString2;
		}
		else
		{
			this.chatMessageIdleStringCurrent = this.chatMessageIdleString1;
		}
		this.chatMessage.text = this.chatMessageIdleStringCurrent;
		this.chatMessageIdleStringTimer = 0f;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0005081B File Offset: 0x0004EA1B
	private void PlayerSelfDestruction()
	{
		if (SemiFunc.FPSImpulse5() && this.selfDestructingPlayers && SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			this.selfDestructingPlayers = false;
		}
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00050850 File Offset: 0x0004EA50
	private void Update()
	{
		this.PlayerChatBoxStateMachine();
		this.PlayerSelfDestruction();
		this.ChatMessageResultTick();
		this.ChatMessageIdleStringTick();
		this.HoldChat();
		float b = this.chatMessageTimer / this.chatMessageTimerMax;
		this.chatMessageLoadingBar.localScale = new Vector3(Mathf.Lerp(this.chatMessageLoadingBar.localScale.x, b, Time.deltaTime * 10f), this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		if (this.chatActive)
		{
			this.chatActiveTimer = 0.2f;
		}
		else
		{
			this.chatActiveTimer -= Time.deltaTime;
			if (this.chatActiveTimer <= 0f)
			{
				this.chatMessageTimer = 0f;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.started && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.startWaitTimer < 1f)
			{
				this.startWaitTimer += Time.deltaTime;
			}
			else
			{
				this.InitializeTextTyping();
				this.LobbyScreenStartLogic();
			}
		}
		if (!this.started)
		{
			return;
		}
		this.UpdateBackgroundColor();
		if (this.isTyping)
		{
			this.TypingUpdate();
		}
		else
		{
			this.DelayUpdate();
		}
		this.CheckTextMeshLines();
		this.ArrowPointAtGoalLogic();
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x00050994 File Offset: 0x0004EB94
	private void InitializeTextTypingLogic()
	{
		this.textMesh.text = "";
		this.currentPageIndex = 0;
		this.currentLineIndex = 0;
		this.currentCharIndex = 0;
		this.typingTimer = 0f;
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				if (textLine.textLines.Count > 0)
				{
					textLine.text = textLine.textLines[0];
				}
				else
				{
					textLine.text = "Missing line!!";
				}
				textLine.text = SemiFunc.EmojiText(textLine.text);
				textLine.textOriginal = textLine.text;
			}
		}
		this.started = true;
		this.NextLine(this.currentLineIndex);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00050AAC File Offset: 0x0004ECAC
	private void InitializeTextTyping()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.InitializeTextTypingLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("InitializeTextTypingRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x00050ADE File Offset: 0x0004ECDE
	[PunRPC]
	public void InitializeTextTypingRPC()
	{
		this.InitializeTextTypingLogic();
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00050AE8 File Offset: 0x0004ECE8
	private void CheckTextMeshLines()
	{
		if (this.textMesh.textInfo.lineCount > 12)
		{
			int num = this.textMesh.text.IndexOf('\n');
			if (num != -1)
			{
				this.textMesh.text = this.textMesh.text.Substring(num + 1);
			}
		}
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00050B40 File Offset: 0x0004ED40
	private void ChatMessageResultSuccess()
	{
		this.chatMessageLoadingBar.localScale = new Vector3(0f, this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		this.chatMessageResultBar.gameObject.SetActive(true);
		this.chatMessageResultSuccess.Play(this.chatMessageResultBar.position, 1f, 1f, 1f, 1f);
		this.chatMessageResultBarLight.color = new Color(0f, 1f, 0f, 1f);
		this.chatMessageResultBar.GetComponent<RawImage>().color = new Color(0f, 1f, 0f, 1f);
		this.chatMessageResultBarTimer = 0.2f;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00050C18 File Offset: 0x0004EE18
	private void ChatMessageResultFail()
	{
		this.chatMessageLoadingBar.localScale = new Vector3(0f, this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		this.chatMessageResultBar.gameObject.SetActive(true);
		this.chatMessageResultFail.Play(this.chatMessageResultBar.position, 1f, 1f, 1f, 1f);
		this.chatMessageResultBarLight.color = new Color(1f, 0f, 0f, 1f);
		this.chatMessageResultBar.GetComponent<RawImage>().color = new Color(1f, 0f, 0f, 1f);
		this.chatMessageResultBarTimer = 0.2f;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00050CF0 File Offset: 0x0004EEF0
	private void ChatMessageResultTick()
	{
		if (this.chatMessageResultBarTimer > 0f)
		{
			this.chatMessageResultBarTimer -= Time.deltaTime;
			return;
		}
		if (this.chatMessageResultBarTimer != -123f)
		{
			this.chatMessageResultBar.gameObject.SetActive(false);
			this.chatMessageResultBarTimer = -123f;
		}
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00050D48 File Offset: 0x0004EF48
	public void ChatMessageLevel()
	{
		if (RoundDirector.instance.extractionPointsCompleted != RoundDirector.instance.extractionPoints)
		{
			this.ChatMessageResultFail();
			this.GotoPage(1);
			return;
		}
		if (SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			return;
		}
		this.ChatMessageResultFail();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
		this.GotoPage(3);
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00050DA9 File Offset: 0x0004EFA9
	public void ChatMessageLobby()
	{
		this.ChatMessageResultSuccess();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
		this.GotoPage(4);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00050DBF File Offset: 0x0004EFBF
	public void ChatMessageTutorial()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
		this.ChatMessageResultSuccess();
		this.GotoPage(2);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00050DD5 File Offset: 0x0004EFD5
	public void ChatMessageShop()
	{
		if (SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			return;
		}
		this.ChatMessageResultFail();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
		this.GotoPage(4);
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00050E07 File Offset: 0x0004F007
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.textMesh.text = SemiFunc.EmojiText(this.testingText);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00050E27 File Offset: 0x0004F027
	private void ReplaceStringWithVariable(string variableString, string variableValueString, TruckScreenText.TextLine currentLine)
	{
		currentLine.text = currentLine.text.Remove(this.currentCharIndex, variableString.Length - 1);
		currentLine.text = currentLine.text.Insert(this.currentCharIndex + 1, variableValueString);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00050E64 File Offset: 0x0004F064
	private void TypingUpdate()
	{
		if (!this.screenActive)
		{
			return;
		}
		if (this.currentPageIndex < this.pages.Count)
		{
			TruckScreenText.TextPages textPages = this.pages[this.currentPageIndex];
			if (this.currentLineIndex < textPages.textLines.Count)
			{
				TruckScreenText.TextLine textLine = textPages.textLines[this.currentLineIndex];
				if (this.currentCharIndex < textLine.text.Length)
				{
					if (this.currentCharIndex == 0 && this.typingTimer == 0f)
					{
						if (this.currentLineIndex != 0)
						{
							this.textMesh.text = this.textMesh.text;
						}
						this.textMesh.text = this.textMesh.text + this.currentNickname;
						this.NewLineSoundEffect();
					}
					this.typingTimer += Time.deltaTime;
					if (this.typingTimer >= textLine.letterRevealDelay)
					{
						string emojiString = "";
						bool flag = false;
						int num = this.currentCharIndex;
						if (textLine.text[num] == '<')
						{
							while (textLine.text[num] != '>')
							{
								emojiString += textLine.text[num].ToString();
								num++;
							}
							flag = true;
						}
						if (flag)
						{
							TextMeshProUGUI textMeshProUGUI = this.textMesh;
							textMeshProUGUI.text += emojiString;
							this.currentCharIndex = num;
						}
						string text = "";
						int num2 = 0;
						bool flag2 = false;
						if (textLine.text[this.currentCharIndex] == '[')
						{
							flag2 = true;
							while (textLine.text[this.currentCharIndex] != ']')
							{
								text += textLine.text[this.currentCharIndex].ToString();
								this.currentCharIndex++;
								num2++;
							}
							text += textLine.text[this.currentCharIndex].ToString();
							this.currentCharIndex++;
							num2++;
							this.currentCharIndex -= num2;
							if (text == "[haul]")
							{
								string text2 = RoundDirector.instance.totalHaul.ToString();
								text2 = this.FormatDollarValueStrings(text2);
								text2 = "<color=#003300>$</color>" + text2;
								this.ReplaceStringWithVariable(text, text2, textLine);
							}
							if (text == "[goal]")
							{
								int haulGoal = RoundDirector.instance.haulGoal;
								string text3 = haulGoal.ToString();
								text3 = this.FormatDollarValueStrings(text3);
								this.ReplaceStringWithVariable(text, text3, textLine);
							}
							if (text == "[goalmax]")
							{
								string text4 = RoundDirector.instance.haulGoalMax.ToString();
								text4 = this.FormatDollarValueStrings(text4);
								this.ReplaceStringWithVariable(text, text4, textLine);
							}
							if (text == "[hitroad]")
							{
								if (this.currentNickname == this.nicknameTaxman)
								{
									this.chatMessageString = this.chatMessageString.Replace("?", "!");
								}
								this.ReplaceStringWithVariable(text, this.chatMessageString, textLine);
								this.currentNickname = this.nicknameTaxman;
							}
							if (text == "[allplayerintruck]")
							{
								List<string> list = new List<string>();
								foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
								{
									if (!playerAvatar.isDisabled && !playerAvatar.RoomVolumeCheck.inTruck)
									{
										list.Add(playerAvatar.playerName);
									}
								}
								string text5 = "";
								for (int i = 0; i < list.Count; i++)
								{
									if (i == 0)
									{
										text5 += list[i];
									}
									else if (i == list.Count - 1)
									{
										text5 = text5 + " & " + list[i];
									}
									else
									{
										text5 = text5 + ", " + list[i];
									}
								}
								text5 += "...<sprite name=fedup>";
								text5 += "\n";
								text5 += SemiFunc.EmojiText("{pointright}{truck}{pointleft}");
								this.ReplaceStringWithVariable(text, text5, textLine);
							}
							if (text == "[betrayplayers]")
							{
								List<string> list2 = new List<string>();
								foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
								{
									if (!playerAvatar2.isDisabled && !playerAvatar2.RoomVolumeCheck.inTruck)
									{
										list2.Add(playerAvatar2.playerName);
									}
								}
								string text6 = "";
								for (int j = 0; j < list2.Count; j++)
								{
									if (j == 0)
									{
										text6 += list2[j];
									}
									else if (j == list2.Count - 1)
									{
										text6 = text6 + " & " + list2[j];
									}
									else
									{
										text6 = text6 + ", " + list2[j];
									}
								}
								text6 += "... <sprite name=fedup>";
								text6 += "\n";
								text6 += SemiFunc.EmojiText("{pointright}{truck}{pointleft}");
								this.ReplaceStringWithVariable(text, text6, textLine);
							}
						}
						if (!flag2)
						{
							TextMeshProUGUI textMeshProUGUI2 = this.textMesh;
							textMeshProUGUI2.text += textLine.text[this.currentCharIndex].ToString();
						}
						if (flag)
						{
							emojiString += textLine.text[this.currentCharIndex].ToString();
						}
						if (textLine.text[this.currentCharIndex] != ' ')
						{
							if (!flag)
							{
								this.TypeSoundEffect();
							}
							else if (this.customEmojiSounds.Any((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString))
							{
								this.customEmojiSounds.Find((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString).emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
							}
							else
							{
								this.emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
							}
						}
						this.currentCharIndex++;
						this.typingTimer = 0f;
						if (this.currentCharIndex >= textLine.text.Length)
						{
							this.textMesh.text = this.textMesh.text;
							this.isTyping = false;
							this.delayTimer = 0f;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00051578 File Offset: 0x0004F778
	private void UpdateBackgroundColor()
	{
		if (this.backgroundColorChangeTimer < this.backgroundColorChangeDuration)
		{
			this.backgroundColorChangeTimer += Time.deltaTime;
			return;
		}
		if (this.screenActive)
		{
			this.background.color = this.mainBackgroundColor;
			return;
		}
		this.background.color = this.offBackgroundColor;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x000515D4 File Offset: 0x0004F7D4
	private void DelayUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentLineIndex >= this.pages[this.currentPageIndex].textLines.Count)
		{
			return;
		}
		TruckScreenText.TextLine textLine = this.pages[this.currentPageIndex].textLines[this.currentLineIndex];
		this.delayTimer += Time.deltaTime;
		if (this.delayTimer >= textLine.delayAfter)
		{
			UnityEvent onLineEnd = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].onLineEnd;
			if (onLineEnd != null)
			{
				onLineEnd.Invoke();
			}
			this.currentLineIndex++;
			if ((this.currentLineIndex >= this.pages[this.currentPageIndex].textLines.Count && this.pages[this.currentPageIndex].goToNextPageAutomatically) || this.nextPageOverride != -1)
			{
				if (this.nextPageOverride != -1)
				{
					this.GotoPage(this.nextPageOverride);
					this.nextPageOverride = -1;
				}
				else
				{
					this.GotoPage(this.currentPageIndex + 1);
				}
				this.currentLineIndex = 0;
				if (this.currentPageIndex >= this.pages.Count)
				{
					this.currentPageIndex = this.pages.Count;
				}
			}
			if (this.currentLineIndex < this.pages[this.currentPageIndex].textLines.Count)
			{
				this.NextLine(this.currentLineIndex);
			}
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0005175A File Offset: 0x0004F95A
	private void RestartTyping()
	{
		this.InitializeTextTyping();
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00051764 File Offset: 0x0004F964
	private void TypeSoundEffect()
	{
		if (this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customTypingSoundEffect.Sounds.Count<AudioClip>() > 0)
		{
			this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customTypingSoundEffect.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.typingSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00051828 File Offset: 0x0004FA28
	private void NewLineSoundEffect()
	{
		if (this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customMessageSoundEffect.Sounds.Count<AudioClip>() > 0)
		{
			this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customMessageSoundEffect.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.newLineSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x000518E9 File Offset: 0x0004FAE9
	public void StartChat()
	{
		if (!this.screenActive)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.chatActive = true;
			return;
		}
		this.photonView.RPC("StartChatRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0005191E File Offset: 0x0004FB1E
	[PunRPC]
	public void StartChatRPC()
	{
		this.chatActive = true;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00051928 File Offset: 0x0004FB28
	public void HoldChat()
	{
		this.chargeChatLoop.LoopPitch = 1f + this.chatMessageTimer / this.chatMessageTimerMax;
		this.chargeChatLoop.PlayLoop(this.chatActive, 0.9f, 0.9f, 1f);
		if (!this.screenActive && this.chatActiveTimer <= 0f)
		{
			return;
		}
		if (this.chatDeactivatedTimer > 0f && this.chatActiveTimer <= 0f)
		{
			this.chatDeactivatedTimer -= Time.deltaTime;
			return;
		}
		if (!this.chatActive && this.chatActiveTimer <= 0f)
		{
			return;
		}
		if (this.playerChatBoxState != TruckScreenText.PlayerChatBoxState.Idle)
		{
			this.ForceReleaseChat();
			this.staticGrabCollider.SetActive(false);
			this.chatActive = false;
			return;
		}
		this.chatMessageTimer += 1.5f * Time.deltaTime;
		if (this.chatMessageTimer >= this.chatMessageTimerMax)
		{
			this.chatMessageTimer = 0f;
			this.chatActiveTimer = 0f;
			this.chatActive = false;
			if (this.staticGrabObject.playerGrabbing.Count > 0)
			{
				PhysGrabber physGrabber = this.staticGrabObject.playerGrabbing[0];
				if (physGrabber)
				{
					string playerName = physGrabber.playerAvatar.playerName;
					if (GameManager.instance.gameMode == 0)
					{
						this.ChatMessageSend(playerName);
						return;
					}
					if (PhotonNetwork.IsMasterClient)
					{
						this.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, new object[]
						{
							playerName
						});
					}
				}
			}
			return;
		}
		int num = (int)(this.chatMessageTimer / this.chatMessageTimerMax * (float)this.chatMessageString.Length);
		num = Mathf.Min(num, this.chatMessageString.Length);
		bool flag = false;
		string emojiString = "";
		while (this.chatCharacterIndex < num)
		{
			if (this.chatMessageString[this.chatCharacterIndex] == '<')
			{
				flag = true;
				int num2 = this.chatMessageString.IndexOf('>', this.chatCharacterIndex);
				if (num2 != -1)
				{
					num = Mathf.Min(num + (num2 - this.chatCharacterIndex), this.chatMessageString.Length);
					this.chatCharacterIndex = num2 + 1;
				}
				else
				{
					this.chatCharacterIndex++;
					emojiString += this.chatMessageString[this.chatCharacterIndex].ToString();
				}
			}
			else
			{
				this.chatCharacterIndex++;
			}
			if (!flag)
			{
				this.TypeSoundEffect();
			}
			else if (this.customEmojiSounds.Any((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString))
			{
				this.customEmojiSounds.Find((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString).emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			}
			this.chatMessage.text = this.chatMessageString.Substring(0, this.chatCharacterIndex);
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00051C58 File Offset: 0x0004FE58
	private void ForceReleaseChat()
	{
		if (this.staticGrabObject.playerGrabbing.Count > 0)
		{
			List<PhysGrabber> list = new List<PhysGrabber>();
			list.AddRange(this.staticGrabObject.playerGrabbing);
			foreach (PhysGrabber physGrabber in list)
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObject(0.1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						0.1f
					});
				}
			}
		}
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00051D0C File Offset: 0x0004FF0C
	private void NextLineLogic(int _lineIndex, int index)
	{
		UnityEvent onLineStart = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].onLineStart;
		if (onLineStart != null)
		{
			onLineStart.Invoke();
		}
		this.pages[this.currentPageIndex].textLines[this.currentLineIndex].text = SemiFunc.EmojiText(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines[index]);
		this.currentCharIndex = 0;
		this.currentLineIndex = _lineIndex;
		this.isTyping = true;
		this.typingTimer = 0f;
		this.delayTimer = 0f;
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00051DCC File Offset: 0x0004FFCC
	private void NextLine(int _currentLineIndex)
	{
		if (this.pages[this.currentPageIndex].textLines.Count == 0)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			int maxExclusive = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines.Count<string>();
			int index = Random.Range(0, maxExclusive);
			this.NextLineLogic(_currentLineIndex, index);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			int maxExclusive2 = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines.Count<string>();
			int num = Random.Range(0, maxExclusive2);
			this.photonView.RPC("NextLineRPC", RpcTarget.All, new object[]
			{
				num,
				_currentLineIndex
			});
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00051EA4 File Offset: 0x000500A4
	[PunRPC]
	public void NextLineRPC(int index, int _currentLineIndex)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			bool flag = this.isTyping;
			this.currentLineIndex = _currentLineIndex;
		}
		this.NextLineLogic(_currentLineIndex, index);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00051EC4 File Offset: 0x000500C4
	private void ForceCompleteChatMessage()
	{
		int length = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].text.Length;
		if (this.currentCharIndex <= length)
		{
			for (int i = this.currentCharIndex; i < length; i++)
			{
				this.TypingUpdate();
			}
			this.currentCharIndex = length;
			this.isTyping = false;
			this.typingTimer = 0f;
		}
		this.TypeSoundEffect();
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x00051F3C File Offset: 0x0005013C
	private void ChatMessageSend(string playerName)
	{
		string text = ColorUtility.ToHtmlStringRGB(SemiFunc.PlayerGetFromName(playerName).playerAvatarVisuals.color);
		this.currentNickname = string.Concat(new string[]
		{
			"\n\n<color=#",
			text,
			"><b>",
			playerName,
			":</b></color>\n"
		});
		this.onChatMessage.Invoke();
		this.chatDeactivatedTimer = 3f;
		this.chatMessage.text = "";
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00051FB6 File Offset: 0x000501B6
	public void SelfDestructPlayersOutsideTruck()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SelfDestructPlayersOutsideTruckRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("SelfDestructPlayersOutsideTruckRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00051FE3 File Offset: 0x000501E3
	[PunRPC]
	public void SelfDestructPlayersOutsideTruckRPC()
	{
		this.selfDestructingPlayers = true;
		ChatManager.instance.PossessLeftBehind();
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00051FF6 File Offset: 0x000501F6
	[PunRPC]
	public void ChatMessageSendRPC(string playerName)
	{
		this.ChatMessageSend(playerName);
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00051FFF File Offset: 0x000501FF
	public void GotoNextLevel()
	{
		this.EngineStartSound();
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00052014 File Offset: 0x00050214
	public void ShopCompleted()
	{
		base.StartCoroutine(this.ShopGotoNextLevel());
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00052023 File Offset: 0x00050223
	private void EngineStartSound()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("EngineStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.EngineStartRPC();
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x00052051 File Offset: 0x00050251
	[PunRPC]
	public void EngineStartRPC()
	{
		this.engineSuccessSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00052083 File Offset: 0x00050283
	public IEnumerator ShopGotoNextLevel()
	{
		yield return new WaitForSeconds(0.5f);
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
		yield break;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0005208C File Offset: 0x0005028C
	private void PageTransitionEffect()
	{
		this.background.color = this.transitionBackgroundColor;
		this.backgroundColorChangeTimer = 0f;
		this.backgroundColorChangeDuration = 0.5f;
		this.newPageSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x000520F0 File Offset: 0x000502F0
	public void GotoPage(int pageIndex)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.GotoPageLogic(pageIndex);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GotoPageRPC", RpcTarget.All, new object[]
			{
				pageIndex
			});
		}
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00052130 File Offset: 0x00050330
	[PunRPC]
	public void MessageSendCustomRPC(string playerSteamID, string message)
	{
		if (this.isTyping)
		{
			return;
		}
		if (playerSteamID != "")
		{
			PlayerAvatar playerAvatar = SemiFunc.PlayerGetFromSteamID(playerSteamID);
			Color color = playerAvatar.playerAvatarVisuals.color;
			string playerName = playerAvatar.playerName;
			string text = ColorUtility.ToHtmlStringRGB(color);
			this.currentNickname = string.Concat(new string[]
			{
				"\n\n<color=#",
				text,
				"><b>",
				playerName,
				":</b></color>\n"
			});
		}
		if (playerSteamID == "")
		{
			this.currentNickname = this.nicknameTaxman;
		}
		TextMeshProUGUI textMeshProUGUI = this.textMesh;
		textMeshProUGUI.text = textMeshProUGUI.text + this.currentNickname + SemiFunc.EmojiText(message);
		this.newLineSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
		this.currentNickname = this.nicknameTaxman;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0005221C File Offset: 0x0005041C
	public void MessageSendCustom(string playerSteamID, string message, int emojis)
	{
		if (playerSteamID != "")
		{
			List<string> list = new List<string>
			{
				"{:)}",
				"{:D}",
				"{:P}",
				"{eyes}",
				"{:o}",
				"{heart}"
			};
			List<string> list2 = new List<string>
			{
				"{:(}",
				"{:'(}",
				"{heartbreak}",
				"{fedup}"
			};
			string text = "";
			if (emojis != 0)
			{
				List<string> list3 = list;
				if (emojis == 1)
				{
					list3 = list;
				}
				if (emojis == 2)
				{
					list3 = list2;
				}
				text += list3[Random.Range(0, list3.Count)];
				if (Random.Range(0, 2) == 1)
				{
					text += list3[Random.Range(0, list3.Count)];
				}
				if (Random.Range(0, 5) == 1)
				{
					text += list3[Random.Range(0, list3.Count)];
				}
				if (Random.Range(0, 30) == 1)
				{
					text = list3[Random.Range(0, list3.Count)];
					text += text;
					text += text;
				}
			}
			message += text;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("MessageSendCustomRPC", RpcTarget.All, new object[]
			{
				playerSteamID,
				message
			});
			return;
		}
		this.MessageSendCustomRPC(playerSteamID, message);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00052394 File Offset: 0x00050594
	private void GotoPageLogic(int pageIndex)
	{
		this.currentPageIndex = pageIndex;
		this.currentLineIndex = 0;
		this.currentCharIndex = 0;
		this.typingTimer = 0f;
		this.isTyping = true;
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				textLine.text = textLine.textOriginal;
			}
		}
		this.NextLine(this.currentLineIndex);
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00052458 File Offset: 0x00050658
	[PunRPC]
	public void GotoPageRPC(int pageIndex)
	{
		this.GotoPageLogic(pageIndex);
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00052464 File Offset: 0x00050664
	public void ReleaseChat()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.chatActive = false;
			this.chatMessageTimer = 0f;
			this.chatCharacterIndex = 0;
			this.chatMessage.text = "";
			return;
		}
		this.photonView.RPC("ReleaseChatRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x000524BD File Offset: 0x000506BD
	private string FormatDollarValueStrings(string valueString)
	{
		valueString = SemiFunc.DollarGetString(int.Parse(valueString));
		return valueString;
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x000524CD File Offset: 0x000506CD
	[PunRPC]
	public void ReleaseChatRPC()
	{
		this.chatActive = false;
		this.chatMessageTimer = 0f;
		this.chatCharacterIndex = 0;
		this.chatMessage.text = "";
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x000524F8 File Offset: 0x000506F8
	private void GotoPageAfterPageIsDone(int pageIndex)
	{
		this.nextPageOverride = pageIndex;
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00052501 File Offset: 0x00050701
	private void PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState _state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("PlayerChatBoxStateUpdateRPC", RpcTarget.All, new object[]
				{
					_state
				});
				return;
			}
			this.PlayerChatBoxStateUpdateRPC(_state);
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00052539 File Offset: 0x00050739
	public void PlayerChatBoxStateUpdateToLockedDestroySlackers()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00052542 File Offset: 0x00050742
	public void PlayerChatBoxStateUpdateToLockedStartingTruck()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0005254B File Offset: 0x0005074B
	[PunRPC]
	private void PlayerChatBoxStateUpdateRPC(TruckScreenText.PlayerChatBoxState _state)
	{
		this.playerChatBoxState = _state;
		this.playerChatBoxStateStart = true;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0005255C File Offset: 0x0005075C
	private void PlayerChatBoxStateIdle()
	{
		if (this.playerChatBoxStateStart)
		{
			this.truckScreenLocked.LockChatToggle(false, "", default(Color), default(Color));
			this.staticGrabCollider.SetActive(true);
			this.playerChatBoxStateStart = false;
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x000525A8 File Offset: 0x000507A8
	private void PlayerChatBoxStateLockedStartingTruck()
	{
		if (this.playerChatBoxStateStart)
		{
			this.ForceReleaseChat();
			Color darkColor = new Color(0.8f, 0.2f, 0.1f, 1f);
			Color lightColor = new Color(1f, 0.8f, 0f, 1f);
			string lockedText = "STARTING ENGINE";
			if (SemiFunc.RunIsLobby())
			{
				lockedText = "HITTING THE ROAD";
			}
			this.truckScreenLocked.LockChatToggle(true, lockedText, lightColor, darkColor);
			this.playerChatBoxStateStart = false;
		}
		if (!SemiFunc.RunIsLobby())
		{
			if (this.engineSoundTimer > 0f)
			{
				this.engineSoundTimer -= Time.deltaTime;
				return;
			}
			this.engineSoundTimer = Random.Range(2f, 4f);
			this.engineRevSound.Play(this.truckScreenLocked.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x00052690 File Offset: 0x00050890
	private void PlayerChatBoxStateLockedDestroySlackers()
	{
		if (this.playerChatBoxStateStart)
		{
			this.ForceReleaseChat();
			Color darkColor = new Color(0.4f, 0f, 0.3f, 1f);
			Color lightColor = new Color(1f, 0f, 0f, 1f);
			this.truckScreenLocked.LockChatToggle(true, "DESTROYING SLACKERS", lightColor, darkColor);
			this.playerChatBoxStateStart = false;
		}
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x000526FC File Offset: 0x000508FC
	private void PlayerChatBoxStateMachine()
	{
		switch (this.playerChatBoxState)
		{
		case TruckScreenText.PlayerChatBoxState.Idle:
			this.PlayerChatBoxStateIdle();
			return;
		case TruckScreenText.PlayerChatBoxState.Typing:
			break;
		case TruckScreenText.PlayerChatBoxState.LockedDestroySlackers:
			this.PlayerChatBoxStateLockedDestroySlackers();
			break;
		case TruckScreenText.PlayerChatBoxState.LockedStartingTruck:
			this.PlayerChatBoxStateLockedStartingTruck();
			return;
		default:
			return;
		}
	}

	// Token: 0x04000F57 RID: 3927
	public ScreenTextRevealDelaySettings textRevealNormalSetting;

	// Token: 0x04000F58 RID: 3928
	public ScreenNextMessageDelaySettings nextMessageDelayNormalSetting;

	// Token: 0x04000F59 RID: 3929
	public GameObject staticGrabCollider;

	// Token: 0x04000F5A RID: 3930
	public static TruckScreenText instance;

	// Token: 0x04000F5B RID: 3931
	private StaticGrabObject staticGrabObject;

	// Token: 0x04000F5C RID: 3932
	public TruckScreenText.ScreenType screenType;

	// Token: 0x04000F5D RID: 3933
	public string chatMessageString;

	// Token: 0x04000F5E RID: 3934
	public TextMeshProUGUI chatMessage;

	// Token: 0x04000F5F RID: 3935
	public UnityEvent onChatMessage;

	// Token: 0x04000F60 RID: 3936
	private float chatMessageTimer;

	// Token: 0x04000F61 RID: 3937
	private float chatMessageTimerMax = 2f;

	// Token: 0x04000F62 RID: 3938
	private bool chatActive;

	// Token: 0x04000F63 RID: 3939
	private PhotonView photonView;

	// Token: 0x04000F64 RID: 3940
	private int chatCharacterIndex;

	// Token: 0x04000F65 RID: 3941
	private float chatDeactivatedTimer;

	// Token: 0x04000F66 RID: 3942
	private string nicknameTaxman = "\n\n<color=#4d0000><b>TAXMAN:</b></color>\n";

	// Token: 0x04000F67 RID: 3943
	private string currentNickname = "";

	// Token: 0x04000F68 RID: 3944
	private bool screenActive;

	// Token: 0x04000F69 RID: 3945
	private int nextPageOverride = -1;

	// Token: 0x04000F6A RID: 3946
	public Transform chatMessageLoadingBar;

	// Token: 0x04000F6B RID: 3947
	public Transform chatMessageResultBar;

	// Token: 0x04000F6C RID: 3948
	public Light chatMessageResultBarLight;

	// Token: 0x04000F6D RID: 3949
	internal float chatMessageResultBarTimer;

	// Token: 0x04000F6E RID: 3950
	private float chatActiveTimer;

	// Token: 0x04000F6F RID: 3951
	private bool selfDestructingPlayers;

	// Token: 0x04000F70 RID: 3952
	private TuckScreenLocked truckScreenLocked;

	// Token: 0x04000F71 RID: 3953
	private string chatMessageIdleString1 = "<sprite name=message>";

	// Token: 0x04000F72 RID: 3954
	private string chatMessageIdleString2 = "<sprite name=message_arrow>";

	// Token: 0x04000F73 RID: 3955
	private string chatMessageIdleStringCurrent = "<sprite name=message>";

	// Token: 0x04000F74 RID: 3956
	private float chatMessageIdleStringTimer;

	// Token: 0x04000F75 RID: 3957
	internal TruckScreenText.PlayerChatBoxState playerChatBoxState;

	// Token: 0x04000F76 RID: 3958
	private bool playerChatBoxStateStart;

	// Token: 0x04000F77 RID: 3959
	public List<TruckScreenText.CustomEmojiSounds> customEmojiSounds = new List<TruckScreenText.CustomEmojiSounds>();

	// Token: 0x04000F78 RID: 3960
	public Sound typingSound;

	// Token: 0x04000F79 RID: 3961
	public Sound emojiSound;

	// Token: 0x04000F7A RID: 3962
	public Sound newLineSound;

	// Token: 0x04000F7B RID: 3963
	public Sound newPageSound;

	// Token: 0x04000F7C RID: 3964
	public Sound chargeChatLoop;

	// Token: 0x04000F7D RID: 3965
	public Sound chatMessageResultSuccess;

	// Token: 0x04000F7E RID: 3966
	public Sound chatMessageResultFail;

	// Token: 0x04000F7F RID: 3967
	public RawImage background;

	// Token: 0x04000F80 RID: 3968
	public Color mainBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	// Token: 0x04000F81 RID: 3969
	public Color offBackgroundColor = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04000F82 RID: 3970
	public Color evilBackgroundColor = new Color(0.5f, 0f, 0f, 1f);

	// Token: 0x04000F83 RID: 3971
	public Color transitionBackgroundColor = new Color(0.5f, 0.5f, 0f, 1f);

	// Token: 0x04000F84 RID: 3972
	private float arrowPointAtGoalTimer;

	// Token: 0x04000F85 RID: 3973
	private float engineSoundTimer;

	// Token: 0x04000F86 RID: 3974
	public Transform engineSoundTransform;

	// Token: 0x04000F87 RID: 3975
	public Sound engineRevSound;

	// Token: 0x04000F88 RID: 3976
	public Sound engineSuccessSound;

	// Token: 0x04000F89 RID: 3977
	public TextMeshProUGUI textMesh;

	// Token: 0x04000F8A RID: 3978
	public string testingText;

	// Token: 0x04000F8B RID: 3979
	public List<TruckScreenText.TextPages> pages = new List<TruckScreenText.TextPages>();

	// Token: 0x04000F8C RID: 3980
	private int currentPageIndex;

	// Token: 0x04000F8D RID: 3981
	private int currentLineIndex;

	// Token: 0x04000F8E RID: 3982
	private int currentCharIndex;

	// Token: 0x04000F8F RID: 3983
	private float typingTimer;

	// Token: 0x04000F90 RID: 3984
	internal float delayTimer;

	// Token: 0x04000F91 RID: 3985
	internal bool isTyping;

	// Token: 0x04000F92 RID: 3986
	private float backgroundColorChangeTimer;

	// Token: 0x04000F93 RID: 3987
	private float backgroundColorChangeDuration = 0.5f;

	// Token: 0x04000F94 RID: 3988
	private float startWaitTimer;

	// Token: 0x04000F95 RID: 3989
	private bool started;

	// Token: 0x04000F96 RID: 3990
	private bool lobbyStarted;

	// Token: 0x0200031A RID: 794
	public enum ScreenType
	{
		// Token: 0x040025E9 RID: 9705
		TruckScreen,
		// Token: 0x040025EA RID: 9706
		TruckLobbyScreen,
		// Token: 0x040025EB RID: 9707
		TruckShopScreen
	}

	// Token: 0x0200031B RID: 795
	public enum TruckScreenPage
	{
		// Token: 0x040025ED RID: 9709
		Start,
		// Token: 0x040025EE RID: 9710
		EndNotEnough,
		// Token: 0x040025EF RID: 9711
		EndEnough,
		// Token: 0x040025F0 RID: 9712
		AllPlayersInTruck
	}

	// Token: 0x0200031C RID: 796
	public enum LobbyScreenPage
	{
		// Token: 0x040025F2 RID: 9714
		Start,
		// Token: 0x040025F3 RID: 9715
		FailFirst,
		// Token: 0x040025F4 RID: 9716
		FailSecond,
		// Token: 0x040025F5 RID: 9717
		FailThird,
		// Token: 0x040025F6 RID: 9718
		HitRoad
	}

	// Token: 0x0200031D RID: 797
	public enum ShopScreenPage
	{
		// Token: 0x040025F8 RID: 9720
		Start,
		// Token: 0x040025F9 RID: 9721
		NotEnough,
		// Token: 0x040025FA RID: 9722
		Enough,
		// Token: 0x040025FB RID: 9723
		Stealing,
		// Token: 0x040025FC RID: 9724
		AllPlayersInTruck
	}

	// Token: 0x0200031E RID: 798
	public enum PlayerChatBoxState
	{
		// Token: 0x040025FE RID: 9726
		Idle,
		// Token: 0x040025FF RID: 9727
		Typing,
		// Token: 0x04002600 RID: 9728
		LockedDestroySlackers,
		// Token: 0x04002601 RID: 9729
		LockedStartingTruck
	}

	// Token: 0x0200031F RID: 799
	[Serializable]
	public class CustomEmojiSounds
	{
		// Token: 0x04002602 RID: 9730
		public string emojiString;

		// Token: 0x04002603 RID: 9731
		public Sound emojiSound;
	}

	// Token: 0x02000320 RID: 800
	[Serializable]
	public class TextLine
	{
		// Token: 0x04002604 RID: 9732
		public string messageName;

		// Token: 0x04002605 RID: 9733
		[Multiline]
		public List<string> textLines = new List<string>();

		// Token: 0x04002606 RID: 9734
		public bool customSoundEffects;

		// Token: 0x04002607 RID: 9735
		public bool customEvents;

		// Token: 0x04002608 RID: 9736
		public bool customRevealSettings;

		// Token: 0x04002609 RID: 9737
		public Sound customMessageSoundEffect;

		// Token: 0x0400260A RID: 9738
		public Sound customTypingSoundEffect;

		// Token: 0x0400260B RID: 9739
		public UnityEvent onLineStart;

		// Token: 0x0400260C RID: 9740
		public UnityEvent onLineEnd;

		// Token: 0x0400260D RID: 9741
		public ScreenTextRevealDelaySettings typingSpeed;

		// Token: 0x0400260E RID: 9742
		[HideInInspector]
		public float letterRevealDelay;

		// Token: 0x0400260F RID: 9743
		public ScreenNextMessageDelaySettings messageDelayTime;

		// Token: 0x04002610 RID: 9744
		[HideInInspector]
		public float delayAfter;

		// Token: 0x04002611 RID: 9745
		[HideInInspector]
		public string text;

		// Token: 0x04002612 RID: 9746
		[HideInInspector]
		public string textOriginal;
	}

	// Token: 0x02000321 RID: 801
	[Serializable]
	public class TextPages
	{
		// Token: 0x04002613 RID: 9747
		public string pageName;

		// Token: 0x04002614 RID: 9748
		public List<TruckScreenText.TextLine> textLines = new List<TruckScreenText.TextLine>();

		// Token: 0x04002615 RID: 9749
		public bool goToNextPageAutomatically;
	}
}
