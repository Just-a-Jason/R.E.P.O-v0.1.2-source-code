using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class ToolController : MonoBehaviour
{
	// Token: 0x06000727 RID: 1831 RVA: 0x00043F6D File Offset: 0x0004216D
	private void Awake()
	{
		ToolController.instance = this;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00043F78 File Offset: 0x00042178
	private void Start()
	{
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Interaction"
		});
		this.VisibilityMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00043FCC File Offset: 0x000421CC
	private void Update()
	{
		this.UpdateInput();
		this.InteractionCheck();
		this.UpdateDirtFinder();
		this.UpdateActive();
		this.UpdateInteract();
		this.ToolFollow.position = Vector3.Lerp(this.ToolFollow.position, this.FollowTargetTransform.position, 20f * Time.deltaTime);
		this.ToolFollow.rotation = Quaternion.Lerp(this.ToolFollow.rotation, this.FollowTargetTransform.rotation, 20f * Time.deltaTime);
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00044059 File Offset: 0x00042259
	public void Disable(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00044064 File Offset: 0x00042264
	private void UpdateInput()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (PlayerAvatar.instance.isDisabled)
		{
			return;
		}
		if (SemiFunc.InputDown(InputKey.Interact) || this.InteractInputDelayed)
		{
			if (this.ActiveInteractionType != Interaction.InteractionType.None && this.CurrentInteractionType != Interaction.InteractionType.None && this.CurrentInteractionType != this.ActiveInteractionType)
			{
				this.InteractInputDelayed = true;
				this.InteractInput = false;
			}
			else
			{
				this.InteractInput = true;
			}
		}
		else
		{
			this.InteractInput = false;
		}
		if (PlayerController.instance.CanInteract && (Input.GetButton("Dirt Finder") || Input.GetAxis("Dirt Finder") == 1f || GameDirector.instance.LevelCompleted))
		{
			this.DirtFinderInput = true;
		}
		else
		{
			this.DirtFinderInput = false;
		}
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= 1f * Time.deltaTime;
			this.ActiveTimer = 0f;
			this.InteractInputDelayed = false;
			this.InteractInput = false;
			this.DirtFinderInput = false;
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00044164 File Offset: 0x00042364
	private void InteractionCheck()
	{
		if (this.InteractionCheckTimer <= 0f)
		{
			this.InteractionCheckTimer = this.InteractionCheckTime;
			this.CurrentInteractionType = Interaction.InteractionType.None;
			if (PlayerController.instance.CanInteract)
			{
				RaycastHit[] array = Physics.BoxCastAll(this.MainCamera.transform.position, new Vector3(0.01f, 0.01f, 0.01f), this.MainCamera.transform.forward, this.MainCamera.transform.rotation, this.InteractionRange, this.Mask);
				if (array.Length != 0)
				{
					RaycastHit raycastHit;
					bool flag = Physics.Raycast(this.MainCamera.transform.position, this.MainCamera.transform.forward, out raycastHit, this.InteractionRange, this.VisibilityMask);
					bool flag2 = false;
					Interaction hitPicked = null;
					float num = 360f;
					RaycastHit[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						RaycastHit raycastHit2 = array2[i];
						if (!flag || raycastHit2.distance <= raycastHit.distance)
						{
							Interaction interaction = raycastHit2.transform.GetComponent<Interaction>();
							if (!(interaction == null))
							{
								float range = this.Tools.Find((ToolController.Tool x) => x.InteractionType == interaction.Type).Range;
								if (raycastHit2.distance <= range)
								{
									float num2 = Quaternion.Angle(Quaternion.LookRotation(raycastHit2.transform.position - this.MainCamera.transform.position), this.MainCamera.transform.rotation);
									if (num2 < num)
									{
										num = num2;
										hitPicked = interaction;
										flag2 = true;
									}
								}
							}
						}
					}
					if (flag2)
					{
						this.CurrentInteraction = hitPicked;
						this.CurrentInteractionType = hitPicked.Type;
						if (this.CurrentInteractionType == this.ActiveInteractionType)
						{
							this.ActiveInteraction = this.CurrentInteraction;
						}
						this.CurrentSprite = this.Tools.Find((ToolController.Tool x) => x.InteractionType == this.CurrentInteractionType).Icon;
						this.CurrentRange = this.Tools.Find((ToolController.Tool x) => x.InteractionType == hitPicked.Type).Range;
						return;
					}
				}
			}
		}
		else
		{
			this.InteractionCheckTimer -= 1f * Time.deltaTime;
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x000443DC File Offset: 0x000425DC
	private void UpdateDirtFinder()
	{
		if (GameDirector.instance.LevelCompletedDone || (this.DirtFinderInput && this.ForceActiveTimer <= 0f))
		{
			this.CurrentInteractionType = Interaction.InteractionType.DirtFinder;
			this.CurrentSprite = this.Tools.Find((ToolController.Tool x) => x.InteractionType == this.CurrentInteractionType).Icon;
			if (this.ActiveInteractionType != Interaction.InteractionType.DirtFinder && this.ActiveInteractionType != Interaction.InteractionType.None)
			{
				this.DeactivateTool();
			}
			else if (!this.Active && this.CurrentObject == null)
			{
				this.ActivateTool();
			}
			if (this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
			{
				this.ActiveTimer = this.ActiveTime;
			}
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00044480 File Offset: 0x00042680
	private void UpdateActive()
	{
		if (this.ForceActiveTimer > 0f)
		{
			this.ForceActiveTimer -= 1f * Time.deltaTime;
			this.ForceActiveTimer = Mathf.Max(this.ForceActiveTimer, 0f);
		}
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.ActiveInteractionType != Interaction.InteractionType.DirtFinder && (this.CurrentInteractionType != Interaction.InteractionType.None || this.ActiveInteractionType != Interaction.InteractionType.None) && (this.InteractInput || this.ForceActiveTimer > 0f))
			{
				this.ActiveTimer = this.ActiveTime;
				if (!this.Active && this.CurrentObject == null)
				{
					this.ActivateTool();
				}
			}
			if (this.Active && this.ActiveTimer <= 0f)
			{
				this.DeactivateTool();
			}
			if (this.Active)
			{
				if (this.CurrentInteraction == null || this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
				{
					this.ActiveTimer -= 1f * Time.deltaTime;
				}
				else if (!this.Interact)
				{
					if (this.RangeCheckTimer <= 0f)
					{
						this.RangeCheck = false;
						Vector3 direction = this.CurrentInteraction.transform.position - this.MainCamera.transform.position;
						RaycastHit[] array = Physics.BoxCastAll(this.MainCamera.transform.position, new Vector3(0.01f, 0.01f, 0.01f), direction, Quaternion.identity, this.InteractionRange, this.Mask);
						if (array.Length != 0)
						{
							foreach (RaycastHit raycastHit in array)
							{
								if (raycastHit.transform.GetComponent<Interaction>() == this.ActiveInteraction && raycastHit.distance <= this.CurrentRange)
								{
									this.RangeCheck = true;
									break;
								}
							}
						}
						this.RangeCheckTimer = 0.2f;
					}
					else
					{
						this.RangeCheckTimer -= 1f * Time.deltaTime;
					}
					if (!this.RangeCheck)
					{
						this.ActiveTimer -= 1f * Time.deltaTime;
					}
				}
			}
		}
		if (this.ActiveInteractionType != Interaction.InteractionType.None)
		{
			PlayerController.instance.CrouchDisable(0.5f);
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x000446BC File Offset: 0x000428BC
	private void UpdateInteract()
	{
		if (this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
		{
			this.Interact = false;
			return;
		}
		if (this.CurrentInteractionType == this.ActiveInteractionType && (this.InteractInput || this.DebugAlwaysInteract))
		{
			this.InteractTimer = 0.25f;
			this.InteractInputDelayed = false;
		}
		if (this.InteractTimer > 0f)
		{
			this.Interact = true;
			this.InteractTimer -= 1f * Time.deltaTime;
			if (this.InteractTimer <= 0f)
			{
				this.Interact = false;
			}
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0004474C File Offset: 0x0004294C
	private void ActivateTool()
	{
		this.ActiveInteractionType = this.CurrentInteractionType;
		this.ActiveInteraction = this.CurrentInteraction;
		this.Active = true;
		foreach (ToolController.Tool tool in this.Tools)
		{
			if (tool.InteractionType == this.CurrentInteractionType)
			{
				this.ToolOffset.transform.localPosition = tool.OffsetPosition;
				this.ToolOffset.transform.localRotation = Quaternion.Euler(tool.OffsetRotation);
				if (tool.HeadBob)
				{
					this.ToolHeadbob.Activate();
				}
				else
				{
					this.ToolHeadbob.Deactivate();
				}
				this.CurrentHidePosition = tool.HidePosition;
				this.CurrentHideRotation = tool.HideRotation;
				this.CurrentHideSpeed = tool.HideSpeed;
				break;
			}
		}
		this.ToolHide.Show();
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00044850 File Offset: 0x00042A50
	public void ShowTool()
	{
		foreach (ToolController.Tool tool in this.Tools)
		{
			if (tool.InteractionType == this.ActiveInteractionType)
			{
				this.CurrentObject = Object.Instantiate<GameObject>(tool.Object, tool.ObjectParent.transform);
				break;
			}
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000448C8 File Offset: 0x00042AC8
	private void DeactivateTool()
	{
		this.Active = false;
		this.ActiveInteractionType = Interaction.InteractionType.None;
		this.ActiveInteraction = null;
		this.ToolHide.Hide();
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x000448EA File Offset: 0x00042AEA
	public void HideTool()
	{
		this.ToolOffset.transform.localPosition = Vector3.zero;
		this.ToolOffset.transform.localRotation = Quaternion.identity;
		Object.Destroy(this.CurrentObject);
	}

	// Token: 0x04000C79 RID: 3193
	public bool DebugAlwaysInteract;

	// Token: 0x04000C7A RID: 3194
	[HideInInspector]
	public static ToolController instance;

	// Token: 0x04000C7B RID: 3195
	[HideInInspector]
	public bool Active;

	// Token: 0x04000C7C RID: 3196
	private float ActiveTime = 0.25f;

	// Token: 0x04000C7D RID: 3197
	private float ActiveTimer;

	// Token: 0x04000C7E RID: 3198
	[HideInInspector]
	public bool Interact;

	// Token: 0x04000C7F RID: 3199
	private float InteractTimer;

	// Token: 0x04000C80 RID: 3200
	[Space]
	public float InteractionRange = 4f;

	// Token: 0x04000C81 RID: 3201
	public float InteractionCheckTime = 0.1f;

	// Token: 0x04000C82 RID: 3202
	private float InteractionCheckTimer;

	// Token: 0x04000C83 RID: 3203
	private float RangeCheckTimer;

	// Token: 0x04000C84 RID: 3204
	private bool RangeCheck = true;

	// Token: 0x04000C85 RID: 3205
	[HideInInspector]
	public float ForceActiveTimer;

	// Token: 0x04000C86 RID: 3206
	[HideInInspector]
	public Interaction.InteractionType ActiveInteractionType;

	// Token: 0x04000C87 RID: 3207
	[HideInInspector]
	public Interaction.InteractionType CurrentInteractionType;

	// Token: 0x04000C88 RID: 3208
	[HideInInspector]
	public Interaction ActiveInteraction;

	// Token: 0x04000C89 RID: 3209
	[HideInInspector]
	public Interaction CurrentInteraction;

	// Token: 0x04000C8A RID: 3210
	[HideInInspector]
	public Vector3 CurrentHidePosition;

	// Token: 0x04000C8B RID: 3211
	[HideInInspector]
	public Vector3 CurrentHideRotation;

	// Token: 0x04000C8C RID: 3212
	[HideInInspector]
	public float CurrentHideSpeed;

	// Token: 0x04000C8D RID: 3213
	[HideInInspector]
	public Sprite CurrentSprite;

	// Token: 0x04000C8E RID: 3214
	private float CurrentRange;

	// Token: 0x04000C8F RID: 3215
	private GameObject CurrentObject;

	// Token: 0x04000C90 RID: 3216
	[HideInInspector]
	public Interaction.InteractionType PreviousInteractionType;

	// Token: 0x04000C91 RID: 3217
	[Space]
	public ToolFollowPush ToolFollowPush;

	// Token: 0x04000C92 RID: 3218
	public ToolHide ToolHide;

	// Token: 0x04000C93 RID: 3219
	public Transform ToolFollow;

	// Token: 0x04000C94 RID: 3220
	public Transform ToolOffset;

	// Token: 0x04000C95 RID: 3221
	public ToolFollow ToolHeadbob;

	// Token: 0x04000C96 RID: 3222
	public Transform ToolTargetParent;

	// Token: 0x04000C97 RID: 3223
	public Transform FollowTargetTransform;

	// Token: 0x04000C98 RID: 3224
	private LayerMask Mask;

	// Token: 0x04000C99 RID: 3225
	private LayerMask VisibilityMask;

	// Token: 0x04000C9A RID: 3226
	public List<ToolController.Tool> Tools;

	// Token: 0x04000C9B RID: 3227
	private Camera MainCamera;

	// Token: 0x04000C9C RID: 3228
	private bool InteractInput;

	// Token: 0x04000C9D RID: 3229
	private bool InteractInputDelayed;

	// Token: 0x04000C9E RID: 3230
	private bool DirtFinderInput;

	// Token: 0x04000C9F RID: 3231
	private float DisableTimer;

	// Token: 0x04000CA0 RID: 3232
	public PlayerAvatar playerAvatarScript;

	// Token: 0x020002F9 RID: 761
	[Serializable]
	public class Tool
	{
		// Token: 0x04002546 RID: 9542
		public string Name;

		// Token: 0x04002547 RID: 9543
		public Interaction.InteractionType InteractionType;

		// Token: 0x04002548 RID: 9544
		[Space]
		public GameObject Object;

		// Token: 0x04002549 RID: 9545
		public GameObject ObjectParent;

		// Token: 0x0400254A RID: 9546
		public GameObject playerAvatarPrefab;

		// Token: 0x0400254B RID: 9547
		[Space]
		public Vector3 HidePosition;

		// Token: 0x0400254C RID: 9548
		public Vector3 HideRotation;

		// Token: 0x0400254D RID: 9549
		public float HideSpeed = 2f;

		// Token: 0x0400254E RID: 9550
		[Space]
		public Vector3 OffsetPosition;

		// Token: 0x0400254F RID: 9551
		public Vector3 OffsetRotation;

		// Token: 0x04002550 RID: 9552
		[Space]
		public Sprite Icon;

		// Token: 0x04002551 RID: 9553
		public bool HeadBob = true;

		// Token: 0x04002552 RID: 9554
		public float Range = 1f;
	}
}
