using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023A RID: 570
public class BatteryUI : SemiUI
{
	// Token: 0x06001213 RID: 4627 RVA: 0x0009FFE4 File Offset: 0x0009E1E4
	protected override void Start()
	{
		base.Start();
		BatteryUI.instance = this;
		this.batteryState = 6;
		this.originalLocalScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x000A001C File Offset: 0x0009E21C
	private void BatteryLogic()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (!this.batteryImage)
		{
			return;
		}
		if (this.batteryState == 0)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.921f, 0.4f, 0.2f);
		}
		if (this.batteryState == 1)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 2)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.687f, 0.4f, 0.2f);
		}
		if (this.batteryState == 3)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 4)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.4523f, 0.4f, 0.2f);
		}
		if (this.batteryState == 5)
		{
			this.batteryImage.uvRect = new Rect(0.369f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState == 6)
		{
			this.batteryImage.uvRect = new Rect(-0.006f, -0.218f, 0.4f, 0.2f);
		}
		if (this.batteryState > 6)
		{
			this.batteryState = 6;
		}
		if (this.batteryState < 0)
		{
			this.batteryState = 0;
		}
		if (this.batteryState <= 3 && SemiFunc.RunIsLobby())
		{
			float num = 1.8f;
			this.redBlinkTimer += Time.deltaTime * num;
			if (this.redBlinkTimer > 0.5f)
			{
				this.batteryImage.color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				Color color = new Color(1f, 0.7f, 0f, 1f);
				this.batteryImage.color = color;
			}
			if (this.redBlinkTimer > 1f)
			{
				this.redBlinkTimer = 0f;
			}
		}
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x000A0248 File Offset: 0x0009E448
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			base.Hide();
			return;
		}
		this.BatteryFetch();
		this.BatteryLogic();
		if (this.batteryShowTimer > 0f)
		{
			if (!PhysGrabber.instance.grabbed)
			{
				this.batteryShowTimer = 0f;
			}
			this.batteryShowTimer -= Time.deltaTime;
			ItemInfoUI.instance.SemiUIScoot(new Vector2(0f, 20f));
			return;
		}
		base.Hide();
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x000A02CC File Offset: 0x0009E4CC
	public void BatteryFetch()
	{
		if (!this.batteryImage)
		{
			return;
		}
		if (!PhysGrabber.instance.grabbed)
		{
			return;
		}
		if (!SemiFunc.FPSImpulse5())
		{
			return;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (!grabbedPhysGrabObject)
		{
			return;
		}
		ItemBattery component = grabbedPhysGrabObject.GetComponent<ItemBattery>();
		if (!component)
		{
			return;
		}
		if (component.onlyShowWhenItemToggleIsOn && !grabbedPhysGrabObject.GetComponent<ItemToggle>().toggleState)
		{
			return;
		}
		int batteryLifeInt = component.batteryLifeInt;
		if (batteryLifeInt != -1)
		{
			this.batteryState = batteryLifeInt;
			Color red = new Color(1f, 0.7f, 0f, 1f);
			if (this.batteryState == 0)
			{
				red = Color.red;
			}
			this.batteryImage.color = red;
		}
		this.batteryShowTimer = 1f;
	}

	// Token: 0x04001EC8 RID: 7880
	public static BatteryUI instance;

	// Token: 0x04001EC9 RID: 7881
	private int batteryState;

	// Token: 0x04001ECA RID: 7882
	public RawImage batteryImage;

	// Token: 0x04001ECB RID: 7883
	private float redBlinkTimer;

	// Token: 0x04001ECC RID: 7884
	private float batteryShowTimer;

	// Token: 0x04001ECD RID: 7885
	private Vector3 originalLocalScale;
}
