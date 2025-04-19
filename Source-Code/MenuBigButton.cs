using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DA RID: 474
public class MenuBigButton : MonoBehaviour
{
	// Token: 0x06000FE1 RID: 4065 RVA: 0x000910A6 File Offset: 0x0008F2A6
	private void Start()
	{
		this.behindButtonMainColor = this.behindButtonBG.color;
		this.mainButtonMainColor = this.mainButtonBG.color;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x000910CC File Offset: 0x0008F2CC
	private void Update()
	{
		MenuBigButton.State state = this.state;
		if (state == MenuBigButton.State.Main)
		{
			this.StateMain();
			return;
		}
		if (state != MenuBigButton.State.Edit)
		{
			return;
		}
		this.StateEdit();
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x000910F8 File Offset: 0x0008F2F8
	private void StateMain()
	{
		if (this.menuButton.hovering)
		{
			Color color = new Color(0.7f, 0.2f, 0f, 1f);
			this.mainButtonBG.color = color;
			this.behindButtonBG.color = AssetManager.instance.colorYellow;
		}
		else
		{
			this.mainButtonBG.color = this.mainButtonMainColor;
			this.behindButtonBG.color = this.behindButtonMainColor;
		}
		if (this.menuButton.clicked)
		{
			Color color2 = new Color(1f, 0.5f, 0f, 1f);
			this.mainButtonBG.color = color2;
			this.behindButtonBG.color = Color.white;
		}
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x000911B8 File Offset: 0x0008F3B8
	private void StateEdit()
	{
		this.menuButton.buttonText.text = "[press new button]";
		if (this.menuButton.hovering)
		{
			Color color = new Color(0.5f, 0.1f, 0f, 1f);
			this.mainButtonBG.color = color;
			color = new Color(1f, 0.5f, 0f, 1f);
			this.behindButtonBG.color = color;
		}
		else
		{
			Color color2 = new Color(0.5f, 0.1f, 0f, 1f);
			this.mainButtonBG.color = color2;
			color2 = new Color(0.7f, 0.2f, 0f, 1f);
			this.behindButtonBG.color = color2;
		}
		if (this.menuButton.clicked)
		{
			Color color3 = new Color(1f, 0.5f, 0f, 1f);
			this.mainButtonBG.color = color3;
			this.behindButtonBG.color = Color.white;
		}
	}

	// Token: 0x04001AB4 RID: 6836
	public string buttonTitle = "";

	// Token: 0x04001AB5 RID: 6837
	public string buttonName = "NewButton";

	// Token: 0x04001AB6 RID: 6838
	public RawImage mainButtonBG;

	// Token: 0x04001AB7 RID: 6839
	public RawImage behindButtonBG;

	// Token: 0x04001AB8 RID: 6840
	public MenuButton menuButton;

	// Token: 0x04001AB9 RID: 6841
	public TextMeshProUGUI buttonTitleTextMesh;

	// Token: 0x04001ABA RID: 6842
	private Color mainButtonMainColor;

	// Token: 0x04001ABB RID: 6843
	private Color behindButtonMainColor;

	// Token: 0x04001ABC RID: 6844
	public MenuBigButton.State state;

	// Token: 0x02000389 RID: 905
	public enum State
	{
		// Token: 0x040027E5 RID: 10213
		Main,
		// Token: 0x040027E6 RID: 10214
		Edit
	}
}
