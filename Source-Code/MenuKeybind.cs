using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001E3 RID: 483
public class MenuKeybind : MonoBehaviour
{
	// Token: 0x06001013 RID: 4115 RVA: 0x00091DB3 File Offset: 0x0008FFB3
	private void Start()
	{
		this.menuBigButton = base.GetComponent<MenuBigButton>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.settingElement = base.GetComponent<MenuSettingElement>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x00091DE6 File Offset: 0x0008FFE6
	private IEnumerator LateStart()
	{
		yield return null;
		this.UpdateBindingDisplay();
		yield break;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x00091DF8 File Offset: 0x0008FFF8
	private void UpdateBindingDisplay()
	{
		string text = InputManager.instance.InputDisplayGet(this.inputKey, this.keyType, this.movementDirection);
		this.menuBigButton.buttonName = text;
		this.menuBigButton.menuButton.buttonText.text = text;
		if (MenuPageLobby.instance)
		{
			MenuPageLobby.instance.UpdateChatPrompt();
		}
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00091E5C File Offset: 0x0009005C
	public void OnClick()
	{
		if (this.parentPage.SettingElementActiveCheckFree(this.settingElement.settingElementID))
		{
			this.menuBigButton.state = MenuBigButton.State.Edit;
			this.parentPage.SettingElementActiveSet(this.settingElement.settingElementID);
			this.StartRebinding();
		}
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x00091EAC File Offset: 0x000900AC
	private void StartRebinding()
	{
		if (this.keyType == MenuKeybind.KeyType.InputKey)
		{
			InputAction action = InputManager.instance.GetAction(this.inputKey);
			if (action != null)
			{
				int bindingIndex = 0;
				action.Disable();
				this.rebindingOperation = action.PerformInteractiveRebinding(bindingIndex).WithExpectedControlType("Axis").OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					action.Enable();
					this.rebindingOperation.Dispose();
					this.menuBigButton.state = MenuBigButton.State.Main;
					this.UpdateBindingDisplay();
					MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, this.parentPage, 0.2f, -1f, false);
				}).Start();
				return;
			}
		}
		else if (this.keyType == MenuKeybind.KeyType.MovementKey)
		{
			InputAction action = InputManager.instance.GetMovementAction();
			int movementBindingIndex = InputManager.instance.GetMovementBindingIndex(this.movementDirection);
			if (action != null && movementBindingIndex >= 0)
			{
				action.Disable();
				this.rebindingOperation = action.PerformInteractiveRebinding(movementBindingIndex).OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					action.Enable();
					this.rebindingOperation.Dispose();
					this.menuBigButton.state = MenuBigButton.State.Main;
					this.UpdateBindingDisplay();
					MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, this.parentPage, 0.2f, -1f, false);
				}).Start();
			}
		}
	}

	// Token: 0x04001AE8 RID: 6888
	public MenuKeybind.KeyType keyType;

	// Token: 0x04001AE9 RID: 6889
	public InputKey inputKey;

	// Token: 0x04001AEA RID: 6890
	public MovementDirection movementDirection;

	// Token: 0x04001AEB RID: 6891
	private MenuBigButton menuBigButton;

	// Token: 0x04001AEC RID: 6892
	private MenuPage parentPage;

	// Token: 0x04001AED RID: 6893
	private MenuSettingElement settingElement;

	// Token: 0x04001AEE RID: 6894
	private float actionValue;

	// Token: 0x04001AEF RID: 6895
	private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

	// Token: 0x0200038B RID: 907
	public enum KeyType
	{
		// Token: 0x040027EC RID: 10220
		InputKey,
		// Token: 0x040027ED RID: 10221
		MovementKey
	}
}
