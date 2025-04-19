using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D3 RID: 467
public class ChatManager : MonoBehaviour
{
	// Token: 0x06000F87 RID: 3975 RVA: 0x0008E2D9 File Offset: 0x0008C4D9
	private void Awake()
	{
		if (ChatManager.instance == null)
		{
			ChatManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (ChatManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0008E312 File Offset: 0x0008C512
	private void SetChatColor(Color color)
	{
		this.chatText.color = color;
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0008E320 File Offset: 0x0008C520
	public void ClearAllChatBatches()
	{
		this.possessBatchQueue.Clear();
		this.currentBatch = null;
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0008E334 File Offset: 0x0008C534
	public void ForceSendMessage(string _message)
	{
		this.chatMessage = _message;
		this.ForceConfirmChat();
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0008E344 File Offset: 0x0008C544
	private void CharRemoveEffect()
	{
		ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
		ChatUI.instance.SemiUISpringShakeX(5f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Dud, null, 2f, 1f, true);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0008E395 File Offset: 0x0008C595
	public void AddLetterToChat(string letter)
	{
		this.prevChatMessage = this.chatMessage;
		this.chatMessage += letter;
		this.chatText.text = this.chatMessage;
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x0008E3C6 File Offset: 0x0008C5C6
	public void ForceConfirmChat()
	{
		this.StateSet(ChatManager.ChatState.Send);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x0008E3CF File Offset: 0x0008C5CF
	private void ChatReset()
	{
		this.chatMessage = "";
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0008E3DC File Offset: 0x0008C5DC
	private void PossessChatLovePotion()
	{
		this.playerAvatar.OverridePupilSize(3f, 4, 1f, 1f, 15f, 0.3f, 0.1f);
		this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Love, 0.25f, 0);
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0008E42C File Offset: 0x0008C62C
	private void PossessChatCustomLogic()
	{
		switch (this.activePossession)
		{
		case ChatManager.PossessChatID.LovePotion:
			this.PossessChatLovePotion();
			break;
		case ChatManager.PossessChatID.SelfDestruct:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case ChatManager.PossessChatID.Betrayal:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case ChatManager.PossessChatID.SelfDestructCancel:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 0.25f, 0);
			break;
		}
		if (this.isSpeakingTimer > 0f)
		{
			this.isSpeakingTimer -= Time.deltaTime;
		}
		if (this.isSpeakingTimer < 0.2f && this.playerAvatar && this.playerAvatar.voiceChat && this.playerAvatar.voiceChat.ttsVoice && this.playerAvatar.voiceChat.ttsVoice.isSpeaking)
		{
			this.isSpeakingTimer = 0.2f;
		}
		if (this.isSpeakingTimer <= 0f && this.possessBatchQueue.Count == 0 && this.currentBatch == null)
		{
			this.currentPossessChatID = ChatManager.PossessChatID.None;
		}
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x0008E588 File Offset: 0x0008C788
	public void PossessChatScheduleStart(int messagePrio)
	{
		bool flag = false;
		if (this.currentBatch != null && messagePrio < this.currentBatch.messagePrio)
		{
			this.InterruptCurrentPossessBatch();
			this.ChatReset();
			flag = true;
		}
		if (this.currentBatch == null)
		{
			flag = true;
		}
		if (flag)
		{
			this.isScheduling = true;
			this.scheduledBatch = new ChatManager.PossessMessageBatch(messagePrio);
		}
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0008E5DB File Offset: 0x0008C7DB
	public void PossessChatScheduleEnd()
	{
		if (!this.isScheduling)
		{
			return;
		}
		this.isScheduling = false;
		this.EnqueuePossessBatch(this.scheduledBatch);
		this.scheduledBatch = null;
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x0008E600 File Offset: 0x0008C800
	public void PossessChat(ChatManager.PossessChatID _possessChatID, string message, float typingSpeed, Color _possessColor, float _messageDelay = 0f, bool sendInTaxmanChat = false, int sendInTaxmanChatEmojiInt = 0, UnityEvent eventExecutionAfterMessageIsDone = null)
	{
		this.isSpeakingTimer = 1f;
		ChatManager.PossessMessage item = new ChatManager.PossessMessage(_possessChatID, message, typingSpeed, _possessColor, _messageDelay, sendInTaxmanChat, sendInTaxmanChatEmojiInt, eventExecutionAfterMessageIsDone);
		if (this.isScheduling)
		{
			this.scheduledBatch.messages.Add(item);
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0008E644 File Offset: 0x0008C844
	private void EnqueuePossessBatch(ChatManager.PossessMessageBatch batch)
	{
		if (this.currentBatch == null)
		{
			this.StartPossessBatch(batch);
			return;
		}
		if (batch.messagePrio < this.currentBatch.messagePrio)
		{
			this.InterruptCurrentPossessBatch();
			this.StartPossessBatch(batch);
			return;
		}
		if (batch.messagePrio <= this.currentBatch.messagePrio)
		{
			this.possessBatchQueue.Add(batch);
		}
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0008E6A1 File Offset: 0x0008C8A1
	private void StartPossessBatch(ChatManager.PossessMessageBatch batch)
	{
		this.currentBatch = batch;
		this.currentBatch.isProcessing = true;
		this.currentMessageIndex = 0;
		this.StartPossessMessage(this.currentBatch.messages[this.currentMessageIndex]);
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0008E6D9 File Offset: 0x0008C8D9
	private void InterruptCurrentPossessBatch()
	{
		this.ChatReset();
		this.currentBatch = null;
		this.possessBatchQueue.Clear();
		this.wasPossessed = false;
		this.wasPossessedPrio = 0;
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0008E701 File Offset: 0x0008C901
	private void StartPossessMessage(ChatManager.PossessMessage message)
	{
		this.ChatReset();
		this.possessLetterDelay = 0f;
		this.SetChatColor(message.possessColor);
		this.currentPossessMessage = message;
		this.StateSet(ChatManager.ChatState.Possessed);
		this.currentPossessChatID = message.possessChatID;
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0008E73A File Offset: 0x0008C93A
	private void PossessionReset()
	{
		this.currentPossessChatID = ChatManager.PossessChatID.None;
		this.currentBatch = null;
		this.possessBatchQueue.Clear();
		this.wasPossessed = false;
		this.wasPossessedPrio = 0;
		this.ChatReset();
		this.StateSet(ChatManager.ChatState.Inactive);
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0008E770 File Offset: 0x0008C970
	private void TypeEffect(Color _color)
	{
		ChatUI.instance.SemiUITextFlashColor(_color, 0.2f);
		ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 2f, 0.2f, true);
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x0008E7C0 File Offset: 0x0008C9C0
	public void TumbleInterruption()
	{
		if (this.activePossessionTimer > 0f)
		{
			return;
		}
		this.PossessionReset();
		if (!this.playerAvatar || !this.playerAvatar.voiceChatFetched || !this.playerAvatar.voiceChat.ttsVoice.isSpeaking)
		{
			return;
		}
		List<string> list = new List<string>
		{
			"Ouch! Ouch! Ouch!",
			"Ow! Ow! Ow!",
			"Oof! Oof! Oof!",
			"Owie! Wowie! Zowie!",
			"Ouchie! Ouchie! Ouchie!",
			"error error error",
			"system error",
			"fatal error",
			"error 404",
			"runtime error",
			"imma falling",
			"falling over",
			"ooooooooh!",
			"oh nooooo!",
			"AAAAAAH! AAH!",
			"AAAAAAAAAAAAAAH!",
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAH!",
			"OH! OH! OH!",
			"AH! AH! AH!"
		};
		int index = Random.Range(0, list.Count);
		string message = list[index];
		this.PossessChatScheduleStart(3);
		this.PossessChat(ChatManager.PossessChatID.Ouch, message, 1f, Color.red, 0f, false, 0, null);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0008E928 File Offset: 0x0008CB28
	private void StateInactive()
	{
		ChatUI.instance.Hide();
		this.chatMessage = "";
		this.chatActive = false;
		if ((!MenuManager.instance.currentMenuPage || (MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Escape && MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Settings)) && SemiFunc.InputDown(InputKey.Chat))
		{
			TutorialDirector.instance.playerChatted = true;
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, null, 1f, 1f, true);
			this.chatActive = !this.chatActive;
			this.StateSet(ChatManager.ChatState.Active);
			this.chatHistoryIndex = 0;
		}
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0008E9D0 File Offset: 0x0008CBD0
	private void StateActive()
	{
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.StateSet(ChatManager.ChatState.Inactive);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && this.chatHistory.Count > 0)
		{
			if (this.chatHistoryIndex > 0)
			{
				this.chatHistoryIndex--;
			}
			else
			{
				this.chatHistoryIndex = this.chatHistory.Count - 1;
			}
			this.chatMessage = this.chatHistory[this.chatHistoryIndex];
			this.chatText.text = this.chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, true);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && this.chatHistory.Count > 0)
		{
			if (this.chatHistoryIndex < this.chatHistory.Count - 1)
			{
				this.chatHistoryIndex++;
			}
			else
			{
				this.chatHistoryIndex = 0;
			}
			this.chatMessage = this.chatHistory[this.chatHistoryIndex];
			this.chatText.text = this.chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, true);
		}
		SemiFunc.InputDisableMovement();
		if (SemiFunc.InputDown(InputKey.ChatDelete))
		{
			if (this.chatMessage.Length > 0)
			{
				this.chatMessage = this.chatMessage.Remove(this.chatMessage.Length - 1);
				this.chatText.text = this.chatMessage;
				this.CharRemoveEffect();
			}
		}
		else
		{
			if (this.chatMessage == "\b")
			{
				this.chatMessage = "";
			}
			this.prevChatMessage = this.chatMessage;
			string text = this.chatMessage;
			this.chatMessage += Input.inputString;
			this.chatMessage = this.chatMessage.Replace("\n", "");
			if (this.chatMessage.Length > 50)
			{
				ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
				ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
				ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
				this.chatMessage = text;
			}
			if (this.prevChatMessage != this.chatMessage)
			{
				bool flag = false;
				if (Input.inputString == "\b")
				{
					this.chatMessage = this.chatMessage.Remove(Mathf.Max(this.chatMessage.Length - 2, 0));
					flag = true;
				}
				else
				{
					this.chatText.text = this.chatMessage;
				}
				this.chatMessage = this.chatMessage.Replace("\r", "");
				this.prevChatMessage = this.chatMessage;
				if (!flag)
				{
					this.TypeEffect(Color.yellow);
				}
				else
				{
					this.CharRemoveEffect();
				}
			}
		}
		if (SemiFunc.InputDown(InputKey.Confirm))
		{
			if (this.chatMessage != "")
			{
				this.StateSet(ChatManager.ChatState.Send);
			}
			else
			{
				this.StateSet(ChatManager.ChatState.Inactive);
			}
		}
		if (Mathf.Sin(Time.time * 10f) > 0f)
		{
			this.chatText.text = this.chatMessage + "<b>|</b>";
		}
		else
		{
			this.chatText.text = this.chatMessage;
		}
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.StateSet(ChatManager.ChatState.Inactive);
		}
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0008EDF4 File Offset: 0x0008CFF4
	private void StatePossessed()
	{
		this.chatActive = true;
		this.spamTimer = 0f;
		if (this.currentPossessMessage != null)
		{
			this.SetChatColor(this.currentPossessMessage.possessColor);
		}
		if (this.currentPossessMessage != null)
		{
			bool flag = false;
			if (this.currentPossessMessage.typingSpeed == -1f)
			{
				flag = true;
			}
			if (this.possessLetterDelay <= 0f)
			{
				if (this.currentPossessMessage.message.Length > 0 && !flag)
				{
					string letter = this.currentPossessMessage.message[0].ToString();
					this.currentPossessMessage.message = this.currentPossessMessage.message.Substring(1);
					this.possessLetterDelay = Random.Range(0.005f, 0.05f);
					this.TypeEffect(this.currentPossessMessage.possessColor);
					this.AddLetterToChat(letter);
					return;
				}
				if (this.isSpeakingTimer <= 0f || !this.wasPossessed || this.wasPossessedPrio > this.currentBatch.messagePrio)
				{
					if (this.currentPossessMessage.messageDelay > 0f)
					{
						this.currentPossessMessage.messageDelay -= Time.deltaTime;
						return;
					}
					if (flag)
					{
						this.chatMessage = this.currentPossessMessage.message;
					}
					this.wasPossessed = true;
					if (this.currentBatch != null)
					{
						this.wasPossessedPrio = this.currentBatch.messagePrio;
					}
					this.StateSet(ChatManager.ChatState.Send);
					return;
				}
			}
			else
			{
				this.possessLetterDelay -= Time.deltaTime * this.currentPossessMessage.typingSpeed;
				if (this.currentPossessMessage.typingSpeed == -1f)
				{
					this.possessLetterDelay = 0f;
				}
			}
			return;
		}
		this.currentMessageIndex++;
		if (this.currentBatch != null && this.currentMessageIndex < this.currentBatch.messages.Count)
		{
			this.StartPossessMessage(this.currentBatch.messages[this.currentMessageIndex]);
			return;
		}
		if (this.currentBatch != null && this.currentBatch.messages.Count == this.currentMessageIndex && this.currentBatch.isProcessing)
		{
			this.currentBatch.isProcessing = false;
			this.currentBatch = null;
		}
		if (this.possessBatchQueue.Count > 0)
		{
			this.StartPossessBatch(this.possessBatchQueue[0]);
			this.possessBatchQueue.RemoveAt(0);
			return;
		}
		this.StateSet(ChatManager.ChatState.Inactive);
		this.currentBatch = null;
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0008F06C File Offset: 0x0008D26C
	private void SelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		base.StartCoroutine(this.SelfDestructCoroutine(delay));
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0008F098 File Offset: 0x0008D298
	private void BetrayalSelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		base.StartCoroutine(this.SelfDestructCoroutine(delay));
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0008F0C4 File Offset: 0x0008D2C4
	public void PossessLeftBehind()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		if (this.playerAvatar.RoomVolumeCheck.inTruck)
		{
			return;
		}
		this.betrayalActive = true;
		this.PossessChatScheduleStart(2);
		string message = SemiFunc.MessageGeneratedGetLeftBehind();
		this.PossessChat(ChatManager.PossessChatID.Betrayal, message, 0.5f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "I need to get to the truck in...", 0.4f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "10...", 0.25f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "9...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "8...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "7...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "6...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "5...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "4...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "3...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "2...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "1...", 0.5f, Color.red, 0.3f, true, 2, null);
		UnityEvent unityEvent = new UnityEvent();
		unityEvent.AddListener(new UnityAction(this.BetrayalSelfDestruct));
		List<string> list = new List<string>
		{
			"betrayal",
			"i'm sorry",
			"I failed",
			"teamwork makes the dream work",
			"I thought we were friends",
			"I thought we were a team",
			"I thought we were in this together"
		};
		string message2 = list[Random.Range(0, list.Count)];
		this.PossessChat(ChatManager.PossessChatID.SelfDestruct, message2, 2f, Color.red, 0f, true, 2, unityEvent);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0008F318 File Offset: 0x0008D518
	public void PossessCancelSelfDestruction()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		this.PossessChatScheduleEnd();
		this.possessBatchQueue.Clear();
		this.currentBatch = null;
		this.betrayalActive = false;
		this.PossessChatScheduleStart(1);
		this.PossessChat(ChatManager.PossessChatID.SelfDestructCancel, "SELF DESTRUCT SEQUENCE CANCELLED!", 2f, Color.green, 0f, false, 0, null);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0008F38C File Offset: 0x0008D58C
	public void PossessSelfDestruction()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		this.PossessChatScheduleStart(-1);
		UnityEvent unityEvent = new UnityEvent();
		unityEvent.AddListener(new UnityAction(this.SelfDestruct));
		List<string> list = new List<string>
		{
			"i'm out",
			"Farewell",
			"Adieu",
			"sayonara",
			"Auf Wiedersehen",
			"adios",
			"ciao",
			"Au Revoir",
			"hasta la vista",
			"see You Later",
			"later",
			"peace OUT",
			"catch you later",
			"later gator",
			"toodles",
			"bye bye",
			"bye",
			"AAAAAAAAAAAAH!",
			"AAAAAAAAAAAAAAAAAAAAAAAH!",
			"bye... ... oh?",
			"this will hurt",
			"it's over for me",
			"why me?",
			"I'm sorry",
			"i see the light",
			"sad but necessary",
			"HEJ DÅ!"
		};
		string message = list[Random.Range(0, list.Count)];
		this.PossessChat(ChatManager.PossessChatID.SelfDestruct, message, 2f, Color.red, 0f, true, 2, unityEvent);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0008F536 File Offset: 0x0008D736
	private IEnumerator BetrayalSelfDestructCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.betrayalActive)
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		}
		yield break;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0008F54C File Offset: 0x0008D74C
	private IEnumerator SelfDestructCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayerAvatar.instance.playerHealth.health = 0;
		PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		yield break;
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0008F55B File Offset: 0x0008D75B
	public bool IsPossessed(ChatManager.PossessChatID _possessChatID)
	{
		return this.activePossession == _possessChatID;
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0008F568 File Offset: 0x0008D768
	private void StateSend()
	{
		bool possessed = false;
		if (this.currentPossessMessage != null && this.currentPossessMessage.sendInTaxmanChat && TruckScreenText.instance)
		{
			TruckScreenText.instance.MessageSendCustom(PlayerController.instance.playerSteamID, this.chatMessage, this.currentPossessMessage.sendInTaxmanChatEmojiInt);
		}
		if (this.currentPossessMessage != null)
		{
			possessed = true;
		}
		this.MessageSend(possessed);
		if (this.currentPossessMessage != null && this.currentPossessMessage.eventExecutionAfterMessageIsDone != null)
		{
			this.currentPossessMessage.eventExecutionAfterMessageIsDone.Invoke();
		}
		this.currentPossessMessage = null;
		this.StateSet(ChatManager.ChatState.Possessed);
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x0008F601 File Offset: 0x0008D801
	private void StateSet(ChatManager.ChatState state)
	{
		this.chatState = state;
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x0008F60C File Offset: 0x0008D80C
	private void ImportantFetches()
	{
		if (!this.chatText)
		{
			this.textMeshFetched = false;
		}
		if (!this.playerAvatar)
		{
			this.localPlayerAvatarFetched = false;
		}
		if (!this.textMeshFetched && ChatUI.instance && ChatUI.instance.chatText)
		{
			this.chatText = ChatUI.instance.chatText;
			this.textMeshFetched = true;
		}
		if (!this.localPlayerAvatarFetched)
		{
			if (SemiFunc.IsMultiplayer())
			{
				List<PlayerAvatar> list = SemiFunc.PlayerGetList();
				if (list.Count <= 0)
				{
					return;
				}
				using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerAvatar playerAvatar = enumerator.Current;
						if (playerAvatar.isLocal)
						{
							this.playerAvatar = playerAvatar;
							this.localPlayerAvatarFetched = true;
							break;
						}
					}
					return;
				}
			}
			this.playerAvatar = PlayerAvatar.instance;
			this.localPlayerAvatarFetched = true;
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0008F700 File Offset: 0x0008D900
	private void NewLevelResets()
	{
		this.betrayalActive = false;
		this.localPlayerAvatarFetched = false;
		this.textMeshFetched = false;
		this.PossessionReset();
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0008F720 File Offset: 0x0008D920
	private void PossessionActive()
	{
		if (this.activePossessionTimer <= 0f)
		{
			this.activePossession = ChatManager.PossessChatID.None;
		}
		if (this.activePossessionTimer > 0f)
		{
			this.activePossessionTimer -= Time.deltaTime;
		}
		if (this.currentPossessChatID != ChatManager.PossessChatID.None || (this.activePossession != ChatManager.PossessChatID.None && this.isSpeakingTimer > 0f))
		{
			this.activePossessionTimer = 0.5f;
			this.activePossession = this.currentPossessChatID;
		}
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0008F794 File Offset: 0x0008D994
	private void Update()
	{
		this.PossessionActive();
		if (this.playerAvatar && this.playerAvatar.isDisabled && (this.possessBatchQueue.Count > 0 || this.currentBatch != null))
		{
			this.InterruptCurrentPossessBatch();
		}
		if (!SemiFunc.IsMultiplayer())
		{
			ChatUI.instance.Hide();
			return;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			this.NewLevelResets();
			return;
		}
		this.ImportantFetches();
		this.PossessChatCustomLogic();
		if (!this.textMeshFetched || !this.localPlayerAvatarFetched)
		{
			return;
		}
		switch (this.chatState)
		{
		case ChatManager.ChatState.Inactive:
			this.StateInactive();
			break;
		case ChatManager.ChatState.Active:
			this.StateActive();
			break;
		case ChatManager.ChatState.Possessed:
			this.StatePossessed();
			break;
		case ChatManager.ChatState.Send:
			this.StateSend();
			break;
		}
		this.PossessChatCustomLogic();
		if (!SemiFunc.IsMultiplayer())
		{
			if (this.chatState != ChatManager.ChatState.Inactive)
			{
				this.StateSet(ChatManager.ChatState.Inactive);
			}
			this.chatActive = false;
			return;
		}
		if (this.spamTimer > 0f)
		{
			this.spamTimer -= Time.deltaTime;
		}
		if (SemiFunc.FPSImpulse15() && this.betrayalActive && PlayerController.instance.playerAvatarScript.RoomVolumeCheck.inTruck)
		{
			this.PossessCancelSelfDestruction();
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0008F8CA File Offset: 0x0008DACA
	public bool StateIsActive()
	{
		return this.chatState == ChatManager.ChatState.Active;
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0008F8D5 File Offset: 0x0008DAD5
	public bool StateIsPossessed()
	{
		return this.chatState == ChatManager.ChatState.Possessed;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0008F8E0 File Offset: 0x0008DAE0
	public bool StateIsSend()
	{
		return this.chatState == ChatManager.ChatState.Send;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0008F8EB File Offset: 0x0008DAEB
	public bool StateIsInactive()
	{
		return this.chatState == ChatManager.ChatState.Inactive;
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x0008F8F8 File Offset: 0x0008DAF8
	private void MessageSend(bool _possessed = false)
	{
		if (this.chatMessage == "")
		{
			return;
		}
		if (this.spamTimer <= 0f)
		{
			this.playerAvatar.ChatMessageSend(this.chatMessage, false);
			if (!_possessed)
			{
				this.chatHistory.Add(this.chatMessage);
			}
			if (this.chatHistory.Count > 20)
			{
				this.chatHistory.RemoveAt(0);
			}
			this.chatHistory = this.chatHistory.AsEnumerable<string>().Reverse<string>().Distinct<string>().Reverse<string>().ToList<string>();
			this.ChatReset();
			this.chatText.text = this.chatMessage;
			this.chatActive = false;
			this.isSpeakingTimer = 0.2f;
			ChatUI.instance.SemiUITextFlashColor(Color.green, 0.2f);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, true);
			this.spamTimer = 1f;
		}
	}

	// Token: 0x04001A65 RID: 6757
	public static ChatManager instance;

	// Token: 0x04001A66 RID: 6758
	internal bool chatActive;

	// Token: 0x04001A67 RID: 6759
	internal bool localPlayerAvatarFetched;

	// Token: 0x04001A68 RID: 6760
	internal bool textMeshFetched;

	// Token: 0x04001A69 RID: 6761
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001A6A RID: 6762
	internal string prevChatMessage = "";

	// Token: 0x04001A6B RID: 6763
	internal string chatMessage = "";

	// Token: 0x04001A6C RID: 6764
	public TextMeshProUGUI chatText;

	// Token: 0x04001A6D RID: 6765
	private float spamTimer;

	// Token: 0x04001A6E RID: 6766
	private List<string> chatHistory = new List<string>();

	// Token: 0x04001A6F RID: 6767
	private int chatHistoryIndex;

	// Token: 0x04001A70 RID: 6768
	private float possessLetterDelay;

	// Token: 0x04001A71 RID: 6769
	private bool wasPossessed;

	// Token: 0x04001A72 RID: 6770
	private int wasPossessedPrio;

	// Token: 0x04001A73 RID: 6771
	private bool betrayalActive;

	// Token: 0x04001A74 RID: 6772
	internal ChatManager.PossessChatID activePossession;

	// Token: 0x04001A75 RID: 6773
	internal float activePossessionTimer;

	// Token: 0x04001A76 RID: 6774
	public ChatManager.PossessChatID currentPossessChatID;

	// Token: 0x04001A77 RID: 6775
	private List<ChatManager.PossessMessageBatch> possessBatchQueue = new List<ChatManager.PossessMessageBatch>();

	// Token: 0x04001A78 RID: 6776
	private ChatManager.PossessMessageBatch currentBatch;

	// Token: 0x04001A79 RID: 6777
	private int currentMessageIndex;

	// Token: 0x04001A7A RID: 6778
	private bool isScheduling;

	// Token: 0x04001A7B RID: 6779
	private ChatManager.PossessMessageBatch scheduledBatch;

	// Token: 0x04001A7C RID: 6780
	private float isSpeakingTimer;

	// Token: 0x04001A7D RID: 6781
	private ChatManager.ChatState chatState;

	// Token: 0x04001A7E RID: 6782
	private ChatManager.PossessMessage currentPossessMessage;

	// Token: 0x02000383 RID: 899
	public enum PossessChatID
	{
		// Token: 0x040027C7 RID: 10183
		None,
		// Token: 0x040027C8 RID: 10184
		LovePotion,
		// Token: 0x040027C9 RID: 10185
		Ouch,
		// Token: 0x040027CA RID: 10186
		SelfDestruct,
		// Token: 0x040027CB RID: 10187
		Betrayal,
		// Token: 0x040027CC RID: 10188
		SelfDestructCancel
	}

	// Token: 0x02000384 RID: 900
	public enum ChatState
	{
		// Token: 0x040027CE RID: 10190
		Inactive,
		// Token: 0x040027CF RID: 10191
		Active,
		// Token: 0x040027D0 RID: 10192
		Possessed,
		// Token: 0x040027D1 RID: 10193
		Send
	}

	// Token: 0x02000385 RID: 901
	public class PossessMessage
	{
		// Token: 0x060017E9 RID: 6121 RVA: 0x000BED2C File Offset: 0x000BCF2C
		public PossessMessage(ChatManager.PossessChatID _possessChatID, string message, float typingSpeed, Color possessColor, float messageDelay, bool sendInTaxmanChat, int sendInTaxmanChatEmojiInt, UnityEvent eventExecutionAfterMessageIsDone)
		{
			this.possessChatID = _possessChatID;
			this.message = message;
			this.typingSpeed = typingSpeed;
			this.possessColor = possessColor;
			this.messageDelay = messageDelay;
			this.sendInTaxmanChat = sendInTaxmanChat;
			this.sendInTaxmanChatEmojiInt = sendInTaxmanChatEmojiInt;
			this.eventExecutionAfterMessageIsDone = eventExecutionAfterMessageIsDone;
		}

		// Token: 0x040027D2 RID: 10194
		public ChatManager.PossessChatID possessChatID;

		// Token: 0x040027D3 RID: 10195
		public string message;

		// Token: 0x040027D4 RID: 10196
		public float typingSpeed;

		// Token: 0x040027D5 RID: 10197
		public Color possessColor;

		// Token: 0x040027D6 RID: 10198
		public float messageDelay;

		// Token: 0x040027D7 RID: 10199
		public bool sendInTaxmanChat;

		// Token: 0x040027D8 RID: 10200
		public int sendInTaxmanChatEmojiInt;

		// Token: 0x040027D9 RID: 10201
		public UnityEvent eventExecutionAfterMessageIsDone;
	}

	// Token: 0x02000386 RID: 902
	public class PossessMessageBatch
	{
		// Token: 0x060017EA RID: 6122 RVA: 0x000BED7C File Offset: 0x000BCF7C
		public PossessMessageBatch(int messagePrio)
		{
			this.messagePrio = messagePrio;
		}

		// Token: 0x040027DA RID: 10202
		public List<ChatManager.PossessMessage> messages = new List<ChatManager.PossessMessage>();

		// Token: 0x040027DB RID: 10203
		public int messagePrio;

		// Token: 0x040027DC RID: 10204
		public bool isProcessing;
	}
}
