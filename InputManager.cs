using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Token: 0x020001D8 RID: 472
public class InputManager : MonoBehaviour
{
	// Token: 0x06000FBA RID: 4026 RVA: 0x0008FAF2 File Offset: 0x0008DCF2
	private void Awake()
	{
		if (InputManager.instance == null)
		{
			InputManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.InitializeInputs();
			this.StoreDefaultBindings();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0008FB2C File Offset: 0x0008DD2C
	private void Start()
	{
		InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableAllDevices;
		this.tagDictionary.Add("[move]", InputKey.Movement);
		this.tagDictionary.Add("[jump]", InputKey.Jump);
		this.tagDictionary.Add("[grab]", InputKey.Grab);
		this.tagDictionary.Add("[grab2]", InputKey.Rotate);
		this.tagDictionary.Add("[sprint]", InputKey.Sprint);
		this.tagDictionary.Add("[crouch]", InputKey.Crouch);
		this.tagDictionary.Add("[map]", InputKey.Map);
		this.tagDictionary.Add("[inventory1]", InputKey.Inventory1);
		this.tagDictionary.Add("[inventory2]", InputKey.Inventory2);
		this.tagDictionary.Add("[inventory3]", InputKey.Inventory3);
		this.tagDictionary.Add("[tumble]", InputKey.Tumble);
		this.tagDictionary.Add("[interact]", InputKey.Interact);
		this.tagDictionary.Add("[push]", InputKey.Push);
		this.tagDictionary.Add("[pull]", InputKey.Pull);
		this.tagDictionary.Add("[chat]", InputKey.Chat);
		ES3.DeleteFile("DefaultKeyBindings.es3");
		if (!ES3.FileExists(new ES3Settings("DefaultKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		})))
		{
			this.SaveDefaultKeyBindings();
		}
		if (!ES3.FileExists(new ES3Settings("CurrentKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		})))
		{
			this.SaveCurrentKeyBindings();
		}
		this.LoadKeyBindings("CurrentKeyBindings.es3");
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0008FCAC File Offset: 0x0008DEAC
	private void FixedUpdate()
	{
		float num = Mathf.Min(Time.fixedDeltaTime, 0.05f);
		if (this.disableMovementTimer > 0f)
		{
			this.disableMovementTimer -= num;
		}
		if (this.disableAimingTimer > 0f)
		{
			this.disableAimingTimer -= num;
		}
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0008FD00 File Offset: 0x0008DF00
	private void InitializeInputs()
	{
		this.inputActions = new Dictionary<InputKey, InputAction>();
		this.movementBindingIndices = new Dictionary<MovementDirection, int>();
		this.inputToggle = new Dictionary<InputKey, bool>();
		InputAction inputAction = new InputAction("Movement", InputActionType.Value, null, null, null, null);
		InputActionSetupExtensions.CompositeSyntax compositeSyntax = inputAction.AddCompositeBinding("2DVector", null, null);
		compositeSyntax.With("Up", "<Keyboard>/w", null, null);
		compositeSyntax.With("Down", "<Keyboard>/s", null, null);
		compositeSyntax.With("Left", "<Keyboard>/a", null, null);
		compositeSyntax.With("Right", "<Keyboard>/d", null, null);
		this.inputActions[InputKey.Movement] = inputAction;
		ReadOnlyArray<InputBinding> bindings = inputAction.bindings;
		for (int i = 0; i < bindings.Count; i++)
		{
			InputBinding inputBinding = bindings[i];
			if (inputBinding.isPartOfComposite)
			{
				string a = inputBinding.name.ToLower();
				if (!(a == "up"))
				{
					if (!(a == "down"))
					{
						if (!(a == "left"))
						{
							if (a == "right")
							{
								this.movementBindingIndices[MovementDirection.Right] = i;
							}
						}
						else
						{
							this.movementBindingIndices[MovementDirection.Left] = i;
						}
					}
					else
					{
						this.movementBindingIndices[MovementDirection.Down] = i;
					}
				}
				else
				{
					this.movementBindingIndices[MovementDirection.Up] = i;
				}
			}
		}
		InputAction inputAction2 = new InputAction("Scroll", InputActionType.Value, null, null, null, null);
		inputAction2.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Scroll] = inputAction2;
		InputAction value = new InputAction("Jump", InputActionType.Value, "<Keyboard>/space", null, null, null);
		this.inputActions[InputKey.Jump] = value;
		value = new InputAction("Use", InputActionType.Value, "<Keyboard>/e", null, null, null);
		this.inputActions[InputKey.Interact] = value;
		value = new InputAction("MouseInput", InputActionType.Value, "<Pointer>/position", null, null, null);
		this.inputActions[InputKey.MouseInput] = value;
		InputAction inputAction3 = new InputAction("Push", InputActionType.Value, null, null, null, null);
		inputAction3.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Push] = inputAction3;
		InputAction inputAction4 = new InputAction("Pull", InputActionType.Value, null, null, null, null);
		inputAction4.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Pull] = inputAction4;
		value = new InputAction("Menu", InputActionType.Value, "<Keyboard>/escape", null, null, null);
		this.inputActions[InputKey.Menu] = value;
		value = new InputAction("Back", InputActionType.Value, "<Keyboard>/escape", null, null, null);
		this.inputActions[InputKey.Back] = value;
		value = new InputAction("BackEditor", InputActionType.Value, "<Keyboard>/F1", null, null, null);
		this.inputActions[InputKey.BackEditor] = value;
		value = new InputAction("Chat", InputActionType.Value, "<Keyboard>/t", null, null, null);
		this.inputActions[InputKey.Chat] = value;
		value = new InputAction("Map", InputActionType.Value, "<Keyboard>/tab", null, null, null);
		this.inputActions[InputKey.Map] = value;
		value = new InputAction("Confirm", InputActionType.Value, "<Keyboard>/enter", null, null, null);
		this.inputActions[InputKey.Confirm] = value;
		value = new InputAction("Grab", InputActionType.Value, "<Mouse>/leftButton", null, null, null);
		this.inputActions[InputKey.Grab] = value;
		value = new InputAction("Rotate", InputActionType.Value, "<Mouse>/rightButton", null, null, null);
		this.inputActions[InputKey.Rotate] = value;
		value = new InputAction("Crouch", InputActionType.Value, "<Keyboard>/ctrl", null, null, null);
		this.inputActions[InputKey.Crouch] = value;
		value = new InputAction("Chat Delete", InputActionType.Value, "<Keyboard>/backspace", null, null, null);
		this.inputActions[InputKey.ChatDelete] = value;
		value = new InputAction("Tumble", InputActionType.Value, "<Keyboard>/q", null, null, null);
		this.inputActions[InputKey.Tumble] = value;
		value = new InputAction("Sprint", InputActionType.Value, "<Keyboard>/leftShift", null, null, null);
		this.inputActions[InputKey.Sprint] = value;
		value = new InputAction("MouseDelta", InputActionType.Value, "<Pointer>/delta", null, null, null);
		this.inputActions[InputKey.MouseDelta] = value;
		value = new InputAction("Inventory1", InputActionType.Value, "<Keyboard>/1", null, null, null);
		this.inputActions[InputKey.Inventory1] = value;
		value = new InputAction("Inventory2", InputActionType.Value, "<Keyboard>/2", null, null, null);
		this.inputActions[InputKey.Inventory2] = value;
		value = new InputAction("Inventory3", InputActionType.Value, "<Keyboard>/3", null, null, null);
		this.inputActions[InputKey.Inventory3] = value;
		value = new InputAction("SpectateNext", InputActionType.Value, "<Mouse>/rightButton", null, null, null);
		this.inputActions[InputKey.SpectateNext] = value;
		value = new InputAction("SpectatePrevious", InputActionType.Value, "<Mouse>/leftButton", null, null, null);
		this.inputActions[InputKey.SpectatePrevious] = value;
		value = new InputAction("PushToTalk", InputActionType.Value, "<Keyboard>/v", null, null, null);
		this.inputActions[InputKey.PushToTalk] = value;
		this.inputToggle.Add(InputKey.Sprint, false);
		this.inputToggle.Add(InputKey.Crouch, false);
		this.inputToggle.Add(InputKey.Map, false);
		this.inputToggle.Add(InputKey.Grab, false);
		this.inputPercentSettings = new Dictionary<InputPercentSetting, int>();
		this.inputPercentSettings[InputPercentSetting.MouseSensitivity] = 50;
		foreach (InputAction inputAction5 in this.inputActions.Values)
		{
			inputAction5.Enable();
		}
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x000902A0 File Offset: 0x0008E4A0
	private void StoreDefaultBindings()
	{
		this.defaultBindingPaths = new Dictionary<InputKey, List<string>>();
		this.defaultInputToggleStates = new Dictionary<InputKey, bool>();
		this.defaultInputPercentSettings = new Dictionary<InputPercentSetting, int>();
		foreach (InputKey key in this.inputActions.Keys)
		{
			InputAction inputAction = this.inputActions[key];
			List<string> list = new List<string>();
			foreach (InputBinding inputBinding in inputAction.bindings)
			{
				list.Add(inputBinding.path);
			}
			this.defaultBindingPaths[key] = list;
		}
		foreach (KeyValuePair<InputKey, bool> keyValuePair in this.inputToggle)
		{
			this.defaultInputToggleStates[keyValuePair.Key] = keyValuePair.Value;
		}
		foreach (KeyValuePair<InputPercentSetting, int> keyValuePair2 in this.inputPercentSettings)
		{
			this.defaultInputPercentSettings[keyValuePair2.Key] = keyValuePair2.Value;
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x0009042C File Offset: 0x0008E62C
	public void SaveDefaultKeyBindings()
	{
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = this.defaultBindingPaths;
		keyBindingSaveData.inputToggleStates = this.defaultInputToggleStates;
		keyBindingSaveData.inputPercentSettings = this.defaultInputPercentSettings;
		ES3Settings settings = new ES3Settings("DefaultKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		});
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, settings);
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x0009048C File Offset: 0x0008E68C
	public void SaveCurrentKeyBindings()
	{
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = new Dictionary<InputKey, List<string>>();
		keyBindingSaveData.inputToggleStates = new Dictionary<InputKey, bool>(this.inputToggle);
		keyBindingSaveData.inputPercentSettings = new Dictionary<InputPercentSetting, int>(this.inputPercentSettings);
		foreach (InputKey key in this.inputActions.Keys)
		{
			InputAction inputAction = this.inputActions[key];
			List<string> list = new List<string>();
			foreach (InputBinding inputBinding in inputAction.bindings)
			{
				list.Add(string.IsNullOrEmpty(inputBinding.overridePath) ? inputBinding.path : inputBinding.overridePath);
			}
			keyBindingSaveData.bindingOverrides[key] = list;
		}
		ES3Settings settings = new ES3Settings("CurrentKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		});
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, settings);
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x000905C4 File Offset: 0x0008E7C4
	public void LoadKeyBindings(string filename)
	{
		try
		{
			ES3Settings settings = new ES3Settings(filename, new Enum[]
			{
				ES3.Location.File
			});
			if (ES3.FileExists(settings))
			{
				KeyBindingSaveData saveData = ES3.Load<KeyBindingSaveData>("KeyBindings", settings);
				this.ApplyLoadedKeyBindings(saveData);
			}
			else
			{
				Debug.LogWarning("Keybindings file not found: " + filename);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load keybindings: " + ex.Message);
		}
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x00090640 File Offset: 0x0008E840
	private void ApplyLoadedKeyBindings(KeyBindingSaveData saveData)
	{
		foreach (InputKey key in saveData.bindingOverrides.Keys)
		{
			InputAction inputAction;
			if (this.inputActions.TryGetValue(key, out inputAction))
			{
				List<string> list = saveData.bindingOverrides[key];
				inputAction.Disable();
				for (int i = 0; i < list.Count; i++)
				{
					string text = list[i];
					if (!string.IsNullOrEmpty(text) && inputAction.bindings.Count > i)
					{
						inputAction.ApplyBindingOverride(i, text);
					}
				}
				inputAction.Enable();
			}
		}
		if (saveData.inputToggleStates != null)
		{
			foreach (KeyValuePair<InputKey, bool> keyValuePair in saveData.inputToggleStates)
			{
				this.inputToggle[keyValuePair.Key] = keyValuePair.Value;
			}
		}
		if (saveData.inputPercentSettings != null)
		{
			foreach (KeyValuePair<InputPercentSetting, int> keyValuePair2 in saveData.inputPercentSettings)
			{
				this.inputPercentSettings[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x000907BC File Offset: 0x0008E9BC
	public void ResetKeyToDefault(InputKey key)
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(key, out inputAction))
		{
			inputAction.Disable();
			List<string> list = this.defaultBindingPaths[key];
			for (int i = 0; i < list.Count; i++)
			{
				inputAction.ApplyBindingOverride(i, list[i]);
			}
			inputAction.Enable();
			if (this.defaultInputToggleStates.ContainsKey(key))
			{
				this.inputToggle[key] = this.defaultInputToggleStates[key];
			}
			if (this.defaultInputPercentSettings.ContainsKey((InputPercentSetting)key))
			{
				this.inputPercentSettings[(InputPercentSetting)key] = this.defaultInputPercentSettings[(InputPercentSetting)key];
				return;
			}
		}
		else
		{
			Debug.LogWarning("InputKey not found: " + key.ToString());
		}
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x00090880 File Offset: 0x0008EA80
	public bool KeyDown(InputKey key)
	{
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble || key == InputKey.Inventory1 || key == InputKey.Inventory2 || key == InputKey.Inventory3 || key == InputKey.Interact) && this.disableMovementTimer > 0f)
		{
			return false;
		}
		InputAction inputAction;
		if (!this.inputActions.TryGetValue(key, out inputAction))
		{
			return false;
		}
		if (key == InputKey.Push)
		{
			return inputAction.ReadValue<Vector2>().y > 0f;
		}
		if (key == InputKey.Pull)
		{
			return inputAction.ReadValue<Vector2>().y < 0f;
		}
		return inputAction.WasPressedThisFrame();
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x00090904 File Offset: 0x0008EB04
	public bool KeyUp(InputKey key)
	{
		InputAction inputAction;
		return ((key != InputKey.Jump && key != InputKey.Crouch && key != InputKey.Tumble) || this.disableMovementTimer <= 0f) && this.inputActions.TryGetValue(key, out inputAction) && key != InputKey.Push && key != InputKey.Pull && inputAction.WasReleasedThisFrame();
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x00090950 File Offset: 0x0008EB50
	public float KeyPullAndPush()
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Push, out inputAction))
		{
			if (inputAction.bindings[0].effectivePath.EndsWith("scroll/y"))
			{
				if (inputAction.ReadValue<float>() > 0f)
				{
					return inputAction.ReadValue<float>();
				}
			}
			else if (inputAction.IsPressed())
			{
				return 1f;
			}
		}
		InputAction inputAction2;
		if (this.inputActions.TryGetValue(InputKey.Pull, out inputAction2))
		{
			if (inputAction2.bindings[0].effectivePath.EndsWith("scroll/y"))
			{
				if (inputAction2.ReadValue<float>() < 0f)
				{
					return inputAction2.ReadValue<float>();
				}
			}
			else if (inputAction2.IsPressed())
			{
				return -1f;
			}
		}
		return 0f;
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x00090A0C File Offset: 0x0008EC0C
	public InputAction GetAction(InputKey key)
	{
		InputAction result;
		if (this.inputActions.TryGetValue(key, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00090A2C File Offset: 0x0008EC2C
	public InputAction GetMovementAction()
	{
		InputAction result;
		if (this.inputActions.TryGetValue(InputKey.Movement, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00090A4C File Offset: 0x0008EC4C
	public int GetMovementBindingIndex(MovementDirection direction)
	{
		int result;
		if (this.movementBindingIndices.TryGetValue(direction, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00090A6C File Offset: 0x0008EC6C
	public bool KeyHold(InputKey key)
	{
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble) && this.disableMovementTimer > 0f)
		{
			return false;
		}
		InputAction inputAction;
		if (!this.inputActions.TryGetValue(key, out inputAction))
		{
			return false;
		}
		if (key == InputKey.Push)
		{
			return inputAction.ReadValue<Vector2>().y > 0f;
		}
		if (key == InputKey.Pull)
		{
			return inputAction.ReadValue<Vector2>().y < 0f;
		}
		return inputAction.IsPressed();
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00090ADC File Offset: 0x0008ECDC
	public float GetMovementX()
	{
		if (this.disableMovementTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, out inputAction))
		{
			return inputAction.ReadValue<Vector2>().x;
		}
		return 0f;
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00090B20 File Offset: 0x0008ED20
	public float GetScrollY()
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Scroll, out inputAction))
		{
			return inputAction.ReadValue<float>();
		}
		return 0f;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00090B4C File Offset: 0x0008ED4C
	public float GetMovementY()
	{
		if (this.disableMovementTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, out inputAction))
		{
			return inputAction.ReadValue<Vector2>().y;
		}
		return 0f;
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x00090B90 File Offset: 0x0008ED90
	public Vector2 GetMovement()
	{
		if (this.disableMovementTimer > 0f)
		{
			return Vector2.zero;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, out inputAction))
		{
			return inputAction.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00090BCC File Offset: 0x0008EDCC
	public float GetMouseX()
	{
		if (this.disableAimingTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseDelta, out inputAction))
		{
			return inputAction.ReadValue<Vector2>().x * this.mouseSensitivity;
		}
		return 0f;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00090C18 File Offset: 0x0008EE18
	public float GetMouseY()
	{
		if (this.disableAimingTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseDelta, out inputAction))
		{
			return inputAction.ReadValue<Vector2>().y * this.mouseSensitivity;
		}
		return 0f;
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00090C64 File Offset: 0x0008EE64
	public Vector2 GetMousePosition()
	{
		if (this.disableAimingTimer > 0f)
		{
			return Vector2.zero;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseInput, out inputAction))
		{
			return inputAction.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x00090CA0 File Offset: 0x0008EEA0
	public void Rebind(InputKey key, string newBinding)
	{
		InputAction action;
		if (this.inputActions.TryGetValue(key, out action))
		{
			action.ApplyBindingOverride(newBinding, null, null);
		}
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00090CC8 File Offset: 0x0008EEC8
	public void RebindMovementKey(MovementDirection direction, string newBinding)
	{
		InputAction action;
		if (this.inputActions.TryGetValue(InputKey.Movement, out action))
		{
			int bindingIndex;
			if (this.movementBindingIndices.TryGetValue(direction, out bindingIndex))
			{
				action.ApplyBindingOverride(bindingIndex, newBinding);
				return;
			}
			Debug.LogWarning(string.Format("Binding index for {0} not found.", direction));
		}
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00090D13 File Offset: 0x0008EF13
	public void DisableMovement()
	{
		this.disableMovementTimer = 0.1f;
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x00090D20 File Offset: 0x0008EF20
	public void DisableAiming()
	{
		this.disableAimingTimer = 0.1f;
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x00090D2D File Offset: 0x0008EF2D
	public void InputToggleRebind(InputKey key, bool toggle)
	{
		this.inputToggle[key] = toggle;
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00090D3C File Offset: 0x0008EF3C
	public bool InputToggleGet(InputKey key)
	{
		return this.inputToggle[key];
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00090D4C File Offset: 0x0008EF4C
	public string GetKeyString(InputKey key)
	{
		InputAction action = this.GetAction(key);
		if (action == null)
		{
			return null;
		}
		return action.bindings[0].effectivePath;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00090D7C File Offset: 0x0008EF7C
	public string GetMovementKeyString(MovementDirection direction)
	{
		InputAction movementAction = this.GetMovementAction();
		int movementBindingIndex = this.GetMovementBindingIndex(direction);
		if (movementAction == null)
		{
			return null;
		}
		return movementAction.bindings[movementBindingIndex].effectivePath;
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00090DB4 File Offset: 0x0008EFB4
	public string InputDisplayGet(InputKey _inputKey, MenuKeybind.KeyType _keyType, MovementDirection _movementDirection)
	{
		if (_keyType == MenuKeybind.KeyType.InputKey)
		{
			InputAction action = this.GetAction(_inputKey);
			if (action != null)
			{
				int bindingIndex = 0;
				return this.InputDisplayGetString(action, bindingIndex);
			}
		}
		else if (_keyType == MenuKeybind.KeyType.MovementKey)
		{
			InputAction movementAction = this.GetMovementAction();
			int movementBindingIndex = this.GetMovementBindingIndex(_movementDirection);
			if (movementAction != null && movementBindingIndex >= 0)
			{
				return this.InputDisplayGetString(movementAction, movementBindingIndex);
			}
		}
		return "Unassigned";
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00090E04 File Offset: 0x0008F004
	public string InputDisplayGetString(InputAction action, int bindingIndex)
	{
		InputBinding inputBinding = action.bindings[bindingIndex];
		string text = InputControlPath.ToHumanReadableString(inputBinding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice, null);
		bool flag = false;
		if (text.EndsWith("Scroll/Y"))
		{
			text = "Mouse Scroll";
			flag = true;
		}
		if (inputBinding.effectivePath.Contains("Mouse") && !flag)
		{
			text = this.InputDisplayMouseStringGet(inputBinding.effectivePath);
		}
		return text.ToUpper();
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00090E74 File Offset: 0x0008F074
	private string InputDisplayMouseStringGet(string path)
	{
		if (path.Contains("leftButton"))
		{
			return "Mouse Left";
		}
		if (path.Contains("rightButton"))
		{
			return "Mouse Right";
		}
		if (path.Contains("middleButton"))
		{
			return "Mouse Middle";
		}
		if (path.Contains("press"))
		{
			return "Mouse Press";
		}
		if (path.Contains("backButton"))
		{
			return "Mouse Back";
		}
		if (path.Contains("forwardButton"))
		{
			return "Mouse Forward";
		}
		if (path.Contains("button"))
		{
			int num = path.IndexOf("button");
			string str = path.Substring(num + "button".Length);
			return "Mouse " + str;
		}
		return "Mouse Button";
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x00090F30 File Offset: 0x0008F130
	public string InputDisplayReplaceTags(string _text)
	{
		foreach (KeyValuePair<string, InputKey> keyValuePair in this.tagDictionary)
		{
			string text;
			if (keyValuePair.Value == InputKey.Movement)
			{
				text = this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Up);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Left);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Down);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Right);
			}
			else
			{
				text = this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.InputKey, MovementDirection.Up);
			}
			_text = _text.Replace(keyValuePair.Key, "<u><b>" + text + "</b></u>");
		}
		return _text;
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00091018 File Offset: 0x0008F218
	public void ResetInput()
	{
		InputSystem.ResetHaptics();
		InputSystem.ResetDevice(Keyboard.current, false);
		foreach (KeyValuePair<InputKey, InputAction> keyValuePair in this.inputActions)
		{
			keyValuePair.Value.Reset();
		}
	}

	// Token: 0x04001AA5 RID: 6821
	public static InputManager instance;

	// Token: 0x04001AA6 RID: 6822
	private Dictionary<InputKey, InputAction> inputActions;

	// Token: 0x04001AA7 RID: 6823
	private Dictionary<InputKey, bool> inputToggle;

	// Token: 0x04001AA8 RID: 6824
	internal Dictionary<InputPercentSetting, int> inputPercentSettings;

	// Token: 0x04001AA9 RID: 6825
	private Dictionary<MovementDirection, int> movementBindingIndices;

	// Token: 0x04001AAA RID: 6826
	private Dictionary<InputKey, List<string>> defaultBindingPaths;

	// Token: 0x04001AAB RID: 6827
	private Dictionary<InputKey, bool> defaultInputToggleStates;

	// Token: 0x04001AAC RID: 6828
	private Dictionary<InputPercentSetting, int> defaultInputPercentSettings;

	// Token: 0x04001AAD RID: 6829
	internal float disableMovementTimer;

	// Token: 0x04001AAE RID: 6830
	internal float disableAimingTimer;

	// Token: 0x04001AAF RID: 6831
	internal float mouseSensitivity = 0.1f;

	// Token: 0x04001AB0 RID: 6832
	private Dictionary<string, InputKey> tagDictionary = new Dictionary<string, InputKey>();
}
