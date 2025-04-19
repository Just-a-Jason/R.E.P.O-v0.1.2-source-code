using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class ItemOrb : MonoBehaviour
{
	// Token: 0x06000BBC RID: 3004 RVA: 0x000686AC File Offset: 0x000668AC
	private void Start()
	{
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.emojiIcon = this.itemAttributes.emojiIcon;
		this.colorPresets = this.itemAttributes.colorPreset;
		this.orbColor = this.colorPresets.GetColorMain();
		this.orbColorLight = this.colorPresets.GetColorLight();
		this.batteryColor = this.orbColorLight;
		this.itemBattery.batteryColor = this.batteryColor;
		this.batteryDrainRate = this.batteryDrainPreset.batteryDrainRate;
		this.itemBattery.batteryDrainRate = this.batteryDrainRate;
		this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		ItemLight component = base.GetComponent<ItemLight>();
		if (component)
		{
			component.itemLight.color = this.orbColor;
		}
		Transform transform = base.transform.Find("Item Orb Mesh/Top/Piece1/Orb Icon");
		if (transform)
		{
			transform.GetComponent<Renderer>().material.SetTexture("_EmissionMap", this.orbIcon);
			transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColor);
		}
		Transform transform2 = null;
		foreach (object obj in base.transform)
		{
			Transform transform3 = (Transform)obj;
			if (transform3.name == "Item Orb Mesh")
			{
				transform2 = transform3;
			}
		}
		if (transform2 == null)
		{
			Debug.LogWarning("Item Orb Mesh not found in" + base.gameObject.name);
		}
		foreach (object obj2 in transform2)
		{
			Transform transform4 = (Transform)obj2;
			foreach (object obj3 in transform4)
			{
				Transform transform5 = (Transform)obj3;
				if (transform5.name.Contains("Piece"))
				{
					this.spherePieces.Add(transform5);
					transform5.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColor);
				}
			}
			if (transform4.name.Contains("Core"))
			{
				this.sphereCore = transform4;
				transform4.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColorLight);
			}
		}
		this.sphereEffectTransform = base.transform.Find("sphere effect");
		Material material = base.transform.Find("sphere effect/AreaEffect/effect").GetComponent<Renderer>().material;
		Material material2 = base.transform.Find("sphere effect/AreaEffect/outline_inside").GetComponent<Renderer>().material;
		Material material3 = base.transform.Find("sphere effect/AreaEffect/outline").GetComponent<Renderer>().material;
		Color color = this.orbColorLight;
		color = new Color(color.r, color.g, color.b, 0.5f);
		Color value = new Color(this.orbColor.r, this.orbColor.g, this.orbColor.b, 0.1f);
		if (material)
		{
			material.SetColor("_Color", value);
		}
		if (material2)
		{
			material2.SetColor("_EdgeColor", color);
		}
		if (material3)
		{
			material3.SetColor("_EdgeColor", color);
		}
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.sphereEffectTransform.transform.localScale = new Vector3(0f, 0f, 0f);
		this.sphereEffectTransform.gameObject.SetActive(false);
		this.orbRadiusOriginal = this.orbRadius;
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x00068AE8 File Offset: 0x00066CE8
	private void Update()
	{
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		this.soundOrbLoop.PlayLoop(this.itemActive, 0.5f, 0.5f, 1f);
		if (!this.itemActive)
		{
			this.onNoBatteryTimer = 0f;
		}
		if (this.orbType == ItemOrb.OrbType.Constant)
		{
			this.OrbConstantLogic();
		}
		if (this.orbType == ItemOrb.OrbType.Pulse)
		{
			this.OrbPulseLogic();
		}
		bool flag = this.itemActive;
		this.itemActive = this.itemToggle.toggleState;
		this.orbRadius = this.orbRadiusOriginal * this.orbRadiusMultiplier;
		if (flag != this.itemActive)
		{
			this.SphereAnimatePiecesBack();
			this.itemBattery.batteryActive = this.itemActive;
			if (this.itemActive)
			{
				this.soundOrbBoot.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.soundOrbShutdown.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		if (this.itemActive)
		{
			this.sphereEffectTransform.gameObject.SetActive(true);
			if (this.itemBattery.batteryLife > 0f)
			{
				this.OrbAnimateAppear();
			}
			this.SphereAnimatePieces();
			if (this.itemBattery.batteryLife <= 0f)
			{
				this.onNoBatteryTimer += Time.deltaTime;
				if (this.onNoBatteryTimer >= 1.5f)
				{
					this.itemToggle.ToggleItem(false, -1);
					this.onNoBatteryTimer = 0f;
				}
			}
		}
		else
		{
			this.OrbAnimateDisappear();
		}
		if (this.sphereEffectTransform.gameObject.activeSelf)
		{
			this.sphereEffectTransform.rotation = Quaternion.identity;
		}
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00068CBC File Offset: 0x00066EBC
	private void SphereAnimatePieces()
	{
		int num = 0;
		foreach (Transform transform in this.spherePieces)
		{
			float num2 = Mathf.Sin(Time.time * 50f + (float)num) * 0.1f;
			transform.localScale = new Vector3(1f + num2, 1f + num2, 1f + num2);
			num++;
		}
		float num3 = Mathf.Sin(Time.time * 30f) * 0.2f;
		this.sphereCore.localScale = new Vector3(1f + num3, 1f + num3, 1f + num3);
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x00068D84 File Offset: 0x00066F84
	private void SphereAnimatePiecesBack()
	{
		foreach (Transform transform in this.spherePieces)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00068DE8 File Offset: 0x00066FE8
	private void OrbConstantLogic()
	{
		if (this.itemBattery.batteryLifeInt == 0)
		{
			this.objectAffected.Clear();
			this.localPlayerAffected = false;
			return;
		}
		if (!this.itemActive)
		{
			return;
		}
		this.sphereCheckTimer += Time.deltaTime;
		if (this.sphereCheckTimer > 0.1f)
		{
			this.objectAffected.Clear();
			this.sphereCheckTimer = 0f;
			if (this.itemBattery.batteryLife <= 0f)
			{
				return;
			}
			if (this.targetEnemies || this.targetNonValuables || this.targetValuables)
			{
				this.objectAffected = SemiFunc.PhysGrabObjectGetAllWithinRange(this.orbRadius, base.transform.position, false, default(LayerMask), null);
				if (!this.targetEnemies || !this.targetNonValuables || !this.targetValuables)
				{
					List<PhysGrabObject> list = new List<PhysGrabObject>();
					foreach (PhysGrabObject physGrabObject in this.objectAffected)
					{
						bool flag = this.customTargetingCondition != null && this.customTargetingCondition.CustomTargetingCondition(physGrabObject.gameObject);
						if (this.customTargetingCondition == null)
						{
							flag = true;
						}
						if (this.targetEnemies && physGrabObject.isEnemy && flag)
						{
							list.Add(physGrabObject);
						}
						if (this.targetNonValuables && physGrabObject.isNonValuable && flag)
						{
							list.Add(physGrabObject);
						}
						if (this.targetValuables && physGrabObject.isValuable && flag)
						{
							list.Add(physGrabObject);
						}
					}
					this.objectAffected.Clear();
					this.objectAffected = list;
				}
			}
			if (this.targetPlayers)
			{
				this.localPlayerAffected = SemiFunc.LocalPlayerOverlapCheck(this.orbRadius, base.transform.position, false);
			}
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00068FD4 File Offset: 0x000671D4
	private void OrbPulseLogic()
	{
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00068FD8 File Offset: 0x000671D8
	private void OrbAnimateAppear()
	{
		float num = Mathf.Lerp(0f, this.orbRadius, this.sphereEffectScaleLerp);
		this.sphereEffectTransform.localScale = new Vector3(num, num, num);
		if (this.sphereEffectScaleLerp < 1f)
		{
			this.sphereEffectScaleLerp += 10f * Time.deltaTime;
			return;
		}
		this.sphereEffectScaleLerp = 1f;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00069040 File Offset: 0x00067240
	private void OrbAnimateDisappear()
	{
		if (this.sphereEffectTransform.gameObject.activeSelf)
		{
			float num = Mathf.Lerp(0f, this.orbRadius, this.sphereEffectScaleLerp);
			this.sphereEffectTransform.localScale = new Vector3(num, num, num);
			if (this.sphereEffectScaleLerp > 0f)
			{
				this.sphereEffectScaleLerp -= 10f * Time.deltaTime;
				return;
			}
			this.sphereEffectScaleLerp = 0f;
			this.sphereEffectTransform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001313 RID: 4883
	[HideInInspector]
	public SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001314 RID: 4884
	public Texture orbIcon;

	// Token: 0x04001315 RID: 4885
	private Material orbEffect;

	// Token: 0x04001316 RID: 4886
	public float orbRadius = 1f;

	// Token: 0x04001317 RID: 4887
	private float orbRadiusOriginal = 1f;

	// Token: 0x04001318 RID: 4888
	private float orbRadiusMultiplier = 1f;

	// Token: 0x04001319 RID: 4889
	private Transform orbTransform;

	// Token: 0x0400131A RID: 4890
	private Transform orbInnerTransform;

	// Token: 0x0400131B RID: 4891
	private ItemToggle itemToggle;

	// Token: 0x0400131C RID: 4892
	[HideInInspector]
	public float batteryDrainRate = 0.1f;

	// Token: 0x0400131D RID: 4893
	[HideInInspector]
	public bool itemActive;

	// Token: 0x0400131E RID: 4894
	private Transform sphereEffectTransform;

	// Token: 0x0400131F RID: 4895
	private float sphereEffectScaleLerp;

	// Token: 0x04001320 RID: 4896
	private PhysGrabObject physGrabObject;

	// Token: 0x04001321 RID: 4897
	internal List<PhysGrabObject> objectAffected = new List<PhysGrabObject>();

	// Token: 0x04001322 RID: 4898
	internal bool localPlayerAffected;

	// Token: 0x04001323 RID: 4899
	private Transform sphereCheckTransform;

	// Token: 0x04001324 RID: 4900
	private float sphereCheckTimer;

	// Token: 0x04001325 RID: 4901
	private List<Transform> spherePieces = new List<Transform>();

	// Token: 0x04001326 RID: 4902
	private Transform sphereCore;

	// Token: 0x04001327 RID: 4903
	[HideInInspector]
	public ColorPresets colorPresets;

	// Token: 0x04001328 RID: 4904
	public BatteryDrainPresets batteryDrainPreset;

	// Token: 0x04001329 RID: 4905
	[HideInInspector]
	public Color orbColor;

	// Token: 0x0400132A RID: 4906
	private Color orbColorLight;

	// Token: 0x0400132B RID: 4907
	[HideInInspector]
	public Color batteryColor;

	// Token: 0x0400132C RID: 4908
	private ItemBattery itemBattery;

	// Token: 0x0400132D RID: 4909
	private float onNoBatteryTimer;

	// Token: 0x0400132E RID: 4910
	private ItemEquippable itemEquippable;

	// Token: 0x0400132F RID: 4911
	private ItemAttributes itemAttributes;

	// Token: 0x04001330 RID: 4912
	public Sound soundOrbBoot;

	// Token: 0x04001331 RID: 4913
	public Sound soundOrbShutdown;

	// Token: 0x04001332 RID: 4914
	public Sound soundOrbLoop;

	// Token: 0x04001333 RID: 4915
	public ItemOrb.OrbType orbType;

	// Token: 0x04001334 RID: 4916
	public bool targetValuables = true;

	// Token: 0x04001335 RID: 4917
	public bool targetPlayers = true;

	// Token: 0x04001336 RID: 4918
	public bool targetEnemies = true;

	// Token: 0x04001337 RID: 4919
	public bool targetNonValuables = true;

	// Token: 0x04001338 RID: 4920
	private ITargetingCondition customTargetingCondition;

	// Token: 0x02000349 RID: 841
	public enum OrbType
	{
		// Token: 0x040026F2 RID: 9970
		Constant,
		// Token: 0x040026F3 RID: 9971
		Pulse
	}
}
