using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000260 RID: 608
public class TutorialDirector : MonoBehaviour
{
	// Token: 0x060012C9 RID: 4809 RVA: 0x000A45B0 File Offset: 0x000A27B0
	private void Awake()
	{
		if (!TutorialDirector.instance)
		{
			TutorialDirector.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x000A45DB File Offset: 0x000A27DB
	private void Start()
	{
		this.currentPage = -1;
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x000A45E4 File Offset: 0x000A27E4
	private void FixedUpdate()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.tutorialActiveTimer > 0f)
		{
			this.tutorialActiveTimer -= Time.fixedDeltaTime;
			this.tutorialActive = true;
		}
		else
		{
			this.tutorialActive = false;
		}
		this.TipBoolChecks();
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x000A4640 File Offset: 0x000A2840
	private void Update()
	{
		if (SemiFunc.RunIsArena() || SemiFunc.RunIsLobbyMenu() || SemiFunc.MenuLevel())
		{
			TutorialUI.instance.Hide();
			return;
		}
		if (!this.tutorialActive)
		{
			TutorialUI.instance.Hide();
			this.tutorialCheckActiveTimer -= Time.deltaTime;
			if (this.tutorialCheckActiveTimer < 0f)
			{
				this.tutorialCheckActiveTimer = 0.5f;
				if (LevelGenerator.Instance.Level == RunManager.instance.levelTutorial)
				{
					this.tutorialActive = true;
					this.TutorialActive();
				}
			}
			TutorialUI.instance.Hide();
			this.TipsTick();
			return;
		}
		this.TutorialActive();
		if (this.tutorialActive)
		{
			if (this.currentPage == -1)
			{
				this.NextPage();
			}
			SemiFunc.UIFocusText(this.tutorialPages[this.currentPage].focusText, Color.white, AssetManager.instance.colorYellow, 0.2f);
			if (this.currentPage < 6)
			{
				HealthUI.instance.Hide();
			}
			if (this.currentPage < 14)
			{
				HaulUI.instance.Hide();
				CurrencyUI.instance.Hide();
				GoalUI.instance.Hide();
			}
			if (this.currentPage < 4)
			{
				EnergyUI.instance.Hide();
			}
			if (this.currentPage < 10)
			{
				InventoryUI.instance.Hide();
			}
			if (this.currentPage == 0)
			{
				this.TaskMove();
			}
			if (this.currentPage == 1)
			{
				this.TaskJump();
			}
			if (this.currentPage == 2)
			{
				this.TaskSneak();
			}
			if (this.currentPage == 3)
			{
				this.TaskSneakUnder();
			}
			if (this.currentPage == 4)
			{
				this.TaskSprint();
			}
			if (this.currentPage == 5)
			{
				this.TaskTumble();
			}
			if (this.currentPage == 6)
			{
				this.TaskGrab();
			}
			if (this.currentPage == 7)
			{
				this.TaskPushAndPull();
			}
			if (this.currentPage == 8)
			{
				this.TaskRotate();
			}
			if (this.currentPage == 9)
			{
				this.TaskInteract();
			}
			if (this.currentPage == 10)
			{
				this.TaskInventoryFill();
			}
			if (this.currentPage == 11)
			{
				this.TaskInventoryEmpty();
			}
			if (this.currentPage == 12)
			{
				this.TaskMap();
			}
			if (this.currentPage == 13)
			{
				this.TaskCartMove();
			}
			if (this.currentPage == 14)
			{
				this.TaskCartFill();
			}
			if (this.currentPage == 15)
			{
				this.TaskExtractionPoint();
			}
			if (this.currentPage == 16)
			{
				this.TaskEnterTuck();
			}
			if (this.arrowDelay > 0f)
			{
				this.arrowDelay -= Time.deltaTime;
			}
			if (TutorialUI.instance.progressBarCurrent > 0.98f && this.tutorialProgress > 0.98f)
			{
				this.NextPage();
				this.tutorialProgress = 0f;
				TutorialUI.instance.animationCurveEval = 0f;
				TutorialUI.instance.progressBar.localScale = new Vector3(0f, 1f, 1f);
				TutorialUI.instance.progressBarCurrent = 0f;
			}
		}
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000A4928 File Offset: 0x000A2B28
	public void SetPageID(string pageName)
	{
		for (int i = 0; i < this.tutorialPages.Count; i++)
		{
			if (this.tutorialPages[i].pageName == pageName)
			{
				this.currentPage = i;
				return;
			}
		}
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x000A496C File Offset: 0x000A2B6C
	public void NextPage()
	{
		this.currentPage++;
		if (this.currentPage > this.tutorialPages.Count - 1)
		{
			this.currentPage = this.tutorialPages.Count - 1;
		}
		int num = this.currentPage;
		string text = this.tutorialPages[num].text;
		string text2 = this.tutorialPages[num].dummyText;
		text2 = InputManager.instance.InputDisplayReplaceTags(text2);
		VideoClip video = this.tutorialPages[num].video;
		text = InputManager.instance.InputDisplayReplaceTags(text);
		if (num == 0)
		{
			TutorialUI.instance.SetPage(video, text, text2, false);
		}
		else
		{
			TutorialUI.instance.SetPage(video, text, text2, true);
		}
		this.arrowDelay = 4f;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x000A4A30 File Offset: 0x000A2C30
	private void TipsClear()
	{
		this.potentialTips.Clear();
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x000A4A40 File Offset: 0x000A2C40
	public void TipsStore()
	{
		if (!this.playerJumped && this.TutorialSettingCheck(DataDirector.Setting.TutorialJumping, 3))
		{
			this.potentialTips.Add("Jumping");
		}
		if (!this.playerSprinted && this.TutorialSettingCheck(DataDirector.Setting.TutorialSprinting, 3))
		{
			this.potentialTips.Add("Sprinting");
		}
		if (!this.playerCrouched && this.TutorialSettingCheck(DataDirector.Setting.TutorialSneaking, 3))
		{
			this.potentialTips.Add("Sneaking");
		}
		if (!this.playerCrawled && this.TutorialSettingCheck(DataDirector.Setting.TutorialHiding, 3))
		{
			this.potentialTips.Add("Hiding");
		}
		if (!this.playerTumbled && this.TutorialSettingCheck(DataDirector.Setting.TutorialTumbling, 3))
		{
			this.potentialTips.Add("Tumbling");
		}
		if (!this.playerPushedAndPulled && this.TutorialSettingCheck(DataDirector.Setting.TutorialPushingAndPulling, 3))
		{
			this.potentialTips.Add("Pushing and Pulling");
		}
		if (!this.playerRotated && this.TutorialSettingCheck(DataDirector.Setting.TutorialRotating, 3))
		{
			this.potentialTips.Add("Rotating");
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (this.playerSawHead && !this.playerRevived && this.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 3))
			{
				this.playerReviveTipDone = true;
				this.potentialTips.Add("Reviving");
			}
			if (!this.playerHealed && this.TutorialSettingCheck(DataDirector.Setting.TutorialHealing, 3))
			{
				bool flag = true;
				bool flag2 = false;
				foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
				{
					if (playerAvatar.isLocal)
					{
						if (playerAvatar.playerHealth.health > 50)
						{
							flag2 = true;
						}
					}
					else if (playerAvatar.playerHealth.health < 50)
					{
						flag = false;
					}
				}
				if (flag2 && !flag)
				{
					this.potentialTips.Add("Healing");
				}
			}
			if (!this.playerChatted && this.numberOfRoundsWithoutChatting > 2 && this.TutorialSettingCheck(DataDirector.Setting.TutorialChat, 3))
			{
				this.potentialTips.Add("Chat");
			}
		}
		if (!this.playerUsedCart && this.numberOfRoundsWithoutCart > 2 && this.TutorialSettingCheck(DataDirector.Setting.TutorialCartHandling, 3))
		{
			this.potentialTips.Add("Cart Handling 2");
		}
		if (!this.playerUsedToggle && this.numberOfRoundsWithoutCart > 5 && this.TutorialSettingCheck(DataDirector.Setting.TutorialItemToggling, 3))
		{
			this.potentialTips.Add("Item Toggling");
		}
		if (!this.playerHadItemsAndUsedInventory && this.numberOfRoundsWithoutInventory > 3 && this.TutorialSettingCheck(DataDirector.Setting.TutorialInventoryFill, 3))
		{
			this.potentialTips.Add("Inventory Fill");
		}
		if (!this.playerUsedMap && this.numberOfRoundsWithoutMap > 1 && this.TutorialSettingCheck(DataDirector.Setting.TutorialMap, 3))
		{
			this.potentialTips.Add("Map");
		}
		if (!this.playerUsedChargingStation && this.numberOfRoundsWithoutCharging > 5 && this.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 3))
		{
			this.potentialTips.Add("Charging Station");
		}
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000A4D20 File Offset: 0x000A2F20
	public void TipsShow()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		for (int i = 0; i < this.potentialTips.Count; i++)
		{
			for (int j = 0; j < this.shownTips.Count; j++)
			{
				if (this.potentialTips[i] == this.shownTips[j])
				{
					this.potentialTips.RemoveAt(i);
					i--;
					break;
				}
			}
		}
		if (this.potentialTips.Count > 0)
		{
			int index = Random.Range(0, this.potentialTips.Count);
			this.ActivateTip(this.potentialTips[index], 4f, true);
		}
		this.TipsClear();
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x000A4DE0 File Offset: 0x000A2FE0
	public void ActivateTip(string tipName, float _delay, bool _interrupt)
	{
		if (!GameplayManager.instance.tips)
		{
			return;
		}
		if (!_interrupt && (this.delayBeforeTip > 0f || this.showTipTimer > 0f))
		{
			return;
		}
		this.TutorialSettingSet(tipName);
		this.shownTips.Add(tipName);
		this.SetPageID(tipName);
		this.SetTipPageUI();
		this.delayBeforeTip = _delay;
		this.showTipTimer = 12f;
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x000A4E4C File Offset: 0x000A304C
	private void SetTipPageUI()
	{
		if (this.currentPage == -1)
		{
			return;
		}
		int index = this.currentPage;
		string text = this.tutorialPages[index].text;
		VideoClip video = this.tutorialPages[index].video;
		text = InputManager.instance.InputDisplayReplaceTags(text);
		TutorialUI.instance.SetTipPage(video, text);
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x000A4EA8 File Offset: 0x000A30A8
	private void TipBoolChecks()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.tutorialActive)
		{
			return;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PhysGrabber.instance.isRotating)
		{
			this.playerRotated = true;
		}
		if (PhysGrabber.instance.isPushing || PhysGrabber.instance.isPulling)
		{
			this.playerPushedAndPulled = true;
		}
		if (Map.Instance.Active)
		{
			this.playerUsedMap = true;
		}
		if (SemiFunc.FPSImpulse1())
		{
			bool flag = Inventory.instance && Inventory.instance.inventorySpots != null && Inventory.instance.InventorySpotsOccupied() > 0;
			if (!SemiFunc.RunIsShop() && ItemManager.instance.purchasedItems.Count > 2 && flag)
			{
				this.playerHadItemsAndUsedInventory = true;
			}
		}
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000A4F7F File Offset: 0x000A317F
	public void TipCancel()
	{
		this.showTipTimer = 0f;
		this.tutorialProgress = 0f;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000A4F98 File Offset: 0x000A3198
	private void TipsTick()
	{
		if (this.delayBeforeTip > 0f)
		{
			this.delayBeforeTip -= Time.deltaTime;
			return;
		}
		if (this.showTipTimer > 0f)
		{
			this.showTipTimer -= Time.deltaTime;
			this.tutorialProgress = 1f - this.showTipTimer / 12f;
			TutorialUI.instance.Show();
		}
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000A5006 File Offset: 0x000A3206
	private void TutorialProgressFill(float amount)
	{
		this.tutorialProgress += amount * Time.deltaTime;
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x000A501C File Offset: 0x000A321C
	public void EndTutorial()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x000A5029 File Offset: 0x000A3229
	public bool TutorialSettingCheck(DataDirector.Setting _setting, int _max)
	{
		return DataDirector.instance.SettingValueFetch(_setting) < _max;
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x000A503C File Offset: 0x000A323C
	private void TutorialSettingSet(string _tutorial)
	{
		DataDirector.Setting setting = DataDirector.Setting.TutorialJumping;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_tutorial);
		if (num <= 1668480752U)
		{
			if (num <= 690938701U)
			{
				if (num <= 578410699U)
				{
					if (num != 183262573U)
					{
						if (num == 578410699U)
						{
							if (_tutorial == "Chat")
							{
								setting = DataDirector.Setting.TutorialChat;
							}
						}
					}
					else if (_tutorial == "Item Toggling")
					{
						setting = DataDirector.Setting.TutorialItemToggling;
					}
				}
				else if (num != 678702281U)
				{
					if (num == 690938701U)
					{
						if (_tutorial == "Rotating")
						{
							setting = DataDirector.Setting.TutorialRotating;
						}
					}
				}
				else if (_tutorial == "Shop")
				{
					setting = DataDirector.Setting.TutorialShop;
				}
			}
			else if (num <= 1151856721U)
			{
				if (num != 1057014026U)
				{
					if (num == 1151856721U)
					{
						if (_tutorial == "Map")
						{
							setting = DataDirector.Setting.TutorialMap;
						}
					}
				}
				else if (_tutorial == "Charging Station")
				{
					setting = DataDirector.Setting.TutorialChargingStation;
				}
			}
			else if (num != 1164769509U)
			{
				if (num != 1522540328U)
				{
					if (num == 1668480752U)
					{
						if (_tutorial == "Final Extraction")
						{
							setting = DataDirector.Setting.TutorialFinalExtraction;
						}
					}
				}
				else if (_tutorial == "Only One Extraction")
				{
					setting = DataDirector.Setting.TutorialOnlyOneExtraction;
				}
			}
			else if (_tutorial == "Tumbling")
			{
				setting = DataDirector.Setting.TutorialTumbling;
			}
		}
		else if (num <= 3538838859U)
		{
			if (num <= 2128769280U)
			{
				if (num != 1846982131U)
				{
					if (num == 2128769280U)
					{
						if (_tutorial == "Inventory Fill")
						{
							setting = DataDirector.Setting.TutorialInventoryFill;
						}
					}
				}
				else if (_tutorial == "Jumping")
				{
					setting = DataDirector.Setting.TutorialJumping;
				}
			}
			else if (num != 2213922758U)
			{
				if (num != 3192934482U)
				{
					if (num == 3538838859U)
					{
						if (_tutorial == "Sprinting")
						{
							setting = DataDirector.Setting.TutorialSprinting;
						}
					}
				}
				else if (_tutorial == "Cart Handling 2")
				{
					setting = DataDirector.Setting.TutorialCartHandling;
				}
			}
			else if (_tutorial == "Hiding")
			{
				setting = DataDirector.Setting.TutorialHiding;
			}
		}
		else if (num <= 3907977003U)
		{
			if (num != 3684092241U)
			{
				if (num == 3907977003U)
				{
					if (_tutorial == "Reviving")
					{
						setting = DataDirector.Setting.TutorialReviving;
					}
				}
			}
			else if (_tutorial == "Sneaking")
			{
				setting = DataDirector.Setting.TutorialSneaking;
			}
		}
		else if (num != 4004552029U)
		{
			if (num != 4071788383U)
			{
				if (num == 4290211947U)
				{
					if (_tutorial == "Pushing and Pulling")
					{
						setting = DataDirector.Setting.TutorialPushingAndPulling;
					}
				}
			}
			else if (_tutorial == "Multiple Extractions")
			{
				setting = DataDirector.Setting.TutorialMultipleExtractions;
			}
		}
		else if (_tutorial == "Healing")
		{
			setting = DataDirector.Setting.TutorialHealing;
		}
		int num2 = DataDirector.instance.SettingValueFetch(setting);
		DataDirector.instance.SettingValueSet(setting, num2 + 1);
		DataDirector.instance.SaveSettings();
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x000A5376 File Offset: 0x000A3576
	public void Reset()
	{
		this.currentPage = -1;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000A5380 File Offset: 0x000A3580
	public void UpdateRoundEnd()
	{
		if (!this.playerUsedCart)
		{
			this.numberOfRoundsWithoutCart++;
		}
		if (!this.playerUsedMap)
		{
			this.numberOfRoundsWithoutMap++;
		}
		if (!this.playerHadItemsAndUsedInventory)
		{
			this.numberOfRoundsWithoutInventory++;
		}
		if (!this.playerUsedToggle)
		{
			this.numberOfRoundsWithoutToggle++;
		}
		if (!this.playerUsedChargingStation)
		{
			this.numberOfRoundsWithoutCharging++;
		}
		if (!this.playerChatted)
		{
			this.numberOfRoundsWithoutChatting++;
		}
		if (!this.playerReviveTipDone)
		{
			this.playerSawHead = false;
			this.playerRevived = false;
		}
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000A5427 File Offset: 0x000A3627
	public void TutorialActive()
	{
		this.tutorialActiveTimer = 0.2f;
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000A5434 File Offset: 0x000A3634
	private void TaskMove()
	{
		Vector3 velocity = PlayerController.instance.rb.velocity;
		velocity.y = 0f;
		if (velocity.magnitude > 0.05f)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000A5476 File Offset: 0x000A3676
	private void TaskJump()
	{
		if (!PlayerController.instance)
		{
			return;
		}
		if (PlayerController.instance.rb.velocity.y > 2f)
		{
			this.TutorialProgressFill(0.8f);
		}
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000A54AC File Offset: 0x000A36AC
	private void TaskSneak()
	{
		Vector3 velocity = PlayerController.instance.rb.velocity;
		bool crouching = PlayerController.instance.Crouching;
		velocity.y = 0f;
		if (velocity.magnitude > 0.05f && crouching)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x000A54FD File Offset: 0x000A36FD
	private void TaskSneakUnder()
	{
		if (PlayerController.instance.Crawling)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x000A5518 File Offset: 0x000A3718
	private void TaskSprint()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(70f, 320f, 0f), 145f);
		}
		bool sprinting = PlayerController.instance.sprinting;
		Vector3 velocity = PlayerController.instance.rb.velocity;
		velocity.y = 0f;
		if (velocity.magnitude > 2f && sprinting)
		{
			this.TutorialProgressFill(0.3f);
		}
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x000A55A8 File Offset: 0x000A37A8
	private void TaskTumble()
	{
		if (!PlayerAvatar.instance.tumble)
		{
			return;
		}
		Vector3 velocity = PlayerAvatar.instance.tumble.rb.velocity;
		bool isTumbling = PlayerAvatar.instance.isTumbling;
		if (velocity.magnitude > 1f && isTumbling)
		{
			this.TutorialProgressFill(0.3f);
		}
		if (isTumbling)
		{
			this.TutorialProgressFill(0.025f);
		}
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x000A5612 File Offset: 0x000A3812
	private void TaskGrab()
	{
		if (PhysGrabber.instance.grabbed)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x000A562B File Offset: 0x000A382B
	private void TaskPushAndPull()
	{
		if (PhysGrabber.instance.isPushing || PhysGrabber.instance.isPulling)
		{
			this.TutorialProgressFill(0.6f);
		}
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x000A5650 File Offset: 0x000A3850
	private void TaskRotate()
	{
		if (PhysGrabber.instance.isRotating)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000A566C File Offset: 0x000A386C
	private void TaskInteract()
	{
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		ItemToggle itemToggle = null;
		if (grabbedObjectTransform)
		{
			itemToggle = grabbedObjectTransform.GetComponent<ItemToggle>();
		}
		if (itemToggle && itemToggle.toggleImpulse)
		{
			this.TutorialProgressFill(0.8f);
		}
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x000A56B0 File Offset: 0x000A38B0
	private void TaskInventoryFill()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 340f, 0f), new Vector3(370f, 20f, 0f), 200f);
		}
		int num = 3;
		int num2 = Inventory.instance.InventorySpotsOccupied();
		this.tutorialProgress = (float)num2 / (float)num;
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x000A5714 File Offset: 0x000A3914
	private void TaskInventoryEmpty()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 340f, 0f), new Vector3(370f, 20f, 0f), 200f);
		}
		int num = 3;
		int num2 = Inventory.instance.InventorySpotsOccupied();
		this.tutorialProgress = 1f - (float)num2 / (float)num;
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x000A577E File Offset: 0x000A397E
	private void TaskMap()
	{
		if (Map.Instance.Active)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x000A5798 File Offset: 0x000A3998
	private void TaskCartMove()
	{
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		PhysGrabCart physGrabCart = null;
		if (grabbedObjectTransform)
		{
			physGrabCart = grabbedObjectTransform.GetComponent<PhysGrabCart>();
		}
		if (!physGrabCart)
		{
			return;
		}
		this.tutorialCart = physGrabCart;
		Vector3 vector = Vector3.zero;
		if (physGrabCart)
		{
			vector = physGrabCart.rb.velocity;
		}
		vector.y = 0f;
		if (physGrabCart && vector.magnitude > 0.5f && physGrabCart.cartBeingPulled)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x000A5820 File Offset: 0x000A3A20
	private void TaskCartFill()
	{
		if (this.tutorialCart)
		{
			int num = 3;
			int itemsInCartCount = this.tutorialCart.itemsInCartCount;
			this.tutorialProgress = (float)itemsInCartCount / (float)num;
			return;
		}
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		PhysGrabCart exists = null;
		if (grabbedObjectTransform)
		{
			exists = grabbedObjectTransform.GetComponent<PhysGrabCart>();
		}
		if (!exists)
		{
			return;
		}
		this.tutorialCart = exists;
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x000A5880 File Offset: 0x000A3A80
	private void TaskExtractionPoint()
	{
		GoalUI.instance.Show();
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(610f, 330f, 0f), 45f);
		}
		float currentHaul = (float)RoundDirector.instance.currentHaul;
		int haulGoal = RoundDirector.instance.haulGoal;
		float value = currentHaul / (float)haulGoal;
		value = Mathf.Clamp(value, 0f, 0.95f);
		if (RoundDirector.instance.extractionPointCurrent)
		{
			this.tutorialExtractionPoint = RoundDirector.instance.extractionPointCurrent;
		}
		if (this.tutorialExtractionPoint)
		{
			if (this.tutorialExtractionPoint.currentState != ExtractionPoint.State.Extracting && this.tutorialExtractionPoint.currentState != ExtractionPoint.State.Complete)
			{
				this.tutorialProgress = value;
			}
			if (this.tutorialExtractionPoint.currentState == ExtractionPoint.State.Complete && this.tutorialProgress < 0.95f)
			{
				this.tutorialProgress = 0.95f;
			}
			if (this.tutorialExtractionPoint.currentState == ExtractionPoint.State.Complete)
			{
				this.TutorialProgressFill(0.2f);
			}
		}
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000A5991 File Offset: 0x000A3B91
	private void TaskEnterTuck()
	{
	}

	// Token: 0x04001FCD RID: 8141
	public static TutorialDirector instance;

	// Token: 0x04001FCE RID: 8142
	public List<TutorialDirector.TutorialPage> tutorialPages = new List<TutorialDirector.TutorialPage>();

	// Token: 0x04001FCF RID: 8143
	internal int currentPage = -1;

	// Token: 0x04001FD0 RID: 8144
	internal bool tutorialActive;

	// Token: 0x04001FD1 RID: 8145
	private float tutorialCheckActiveTimer;

	// Token: 0x04001FD2 RID: 8146
	private float tutorialActiveTimer;

	// Token: 0x04001FD3 RID: 8147
	internal float tutorialProgress;

	// Token: 0x04001FD4 RID: 8148
	private PhysGrabCart tutorialCart;

	// Token: 0x04001FD5 RID: 8149
	private ExtractionPoint tutorialExtractionPoint;

	// Token: 0x04001FD6 RID: 8150
	internal bool deadPlayer;

	// Token: 0x04001FD7 RID: 8151
	private float arrowDelay;

	// Token: 0x04001FD8 RID: 8152
	internal bool playerSprinted;

	// Token: 0x04001FD9 RID: 8153
	internal bool playerJumped;

	// Token: 0x04001FDA RID: 8154
	internal bool playerSawHead;

	// Token: 0x04001FDB RID: 8155
	internal bool playerRevived;

	// Token: 0x04001FDC RID: 8156
	internal bool playerHealed;

	// Token: 0x04001FDD RID: 8157
	internal bool playerRotated;

	// Token: 0x04001FDE RID: 8158
	internal bool playerTumbled;

	// Token: 0x04001FDF RID: 8159
	internal bool playerCrouched;

	// Token: 0x04001FE0 RID: 8160
	internal bool playerCrawled;

	// Token: 0x04001FE1 RID: 8161
	internal bool playerUsedCart;

	// Token: 0x04001FE2 RID: 8162
	internal bool playerPushedAndPulled;

	// Token: 0x04001FE3 RID: 8163
	internal bool playerUsedToggle;

	// Token: 0x04001FE4 RID: 8164
	internal bool playerHadItemsAndUsedInventory;

	// Token: 0x04001FE5 RID: 8165
	internal bool playerUsedMap;

	// Token: 0x04001FE6 RID: 8166
	internal bool playerUsedChargingStation;

	// Token: 0x04001FE7 RID: 8167
	internal bool playerReviveTipDone;

	// Token: 0x04001FE8 RID: 8168
	internal bool playerChatted;

	// Token: 0x04001FE9 RID: 8169
	internal int numberOfRoundsWithoutChatting;

	// Token: 0x04001FEA RID: 8170
	internal int numberOfRoundsWithoutCharging;

	// Token: 0x04001FEB RID: 8171
	internal int numberOfRoundsWithoutMap;

	// Token: 0x04001FEC RID: 8172
	internal int numberOfRoundsWithoutInventory;

	// Token: 0x04001FED RID: 8173
	internal int numberOfRoundsWithoutCart;

	// Token: 0x04001FEE RID: 8174
	internal int numberOfRoundsWithoutToggle;

	// Token: 0x04001FEF RID: 8175
	internal List<string> potentialTips = new List<string>();

	// Token: 0x04001FF0 RID: 8176
	internal List<string> shownTips = new List<string>();

	// Token: 0x04001FF1 RID: 8177
	private float showTipTimer;

	// Token: 0x04001FF2 RID: 8178
	private float delayBeforeTip;

	// Token: 0x020003B8 RID: 952
	[Serializable]
	public class TutorialPage
	{
		// Token: 0x040028B0 RID: 10416
		public string pageName = "";

		// Token: 0x040028B1 RID: 10417
		[Space(10f)]
		public VideoClip video;

		// Token: 0x040028B2 RID: 10418
		public string text;

		// Token: 0x040028B3 RID: 10419
		public string focusText;

		// Token: 0x040028B4 RID: 10420
		[TextArea(3, 10)]
		public string dummyText;
	}
}
