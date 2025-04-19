using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000206 RID: 518
public class MenuPageSaves : MonoBehaviour
{
	// Token: 0x06001106 RID: 4358 RVA: 0x00098424 File Offset: 0x00096624
	private void Start()
	{
		this.saveFileInfoPanel = this.saveFileInfo.GetComponentInChildren<Image>();
		List<string> list = StatsManager.instance.SaveFileGetAll();
		float num = 0f;
		foreach (string text in list)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.saveFilePrefab, this.Scroller);
			gameObject.transform.localPosition = this.saveFilePosition.localPosition;
			gameObject.transform.SetSiblingIndex(3);
			MenuElementSaveFile component = gameObject.GetComponent<MenuElementSaveFile>();
			component.saveFileName = text;
			string text2 = StatsManager.instance.SaveFileGetTeamName(text);
			string text3 = StatsManager.instance.SaveFileGetDateAndTime(text);
			int num2 = int.Parse(StatsManager.instance.SaveFileGetRunLevel(text)) + 1;
			component.saveFileHeaderDate.text = text3;
			string text4 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float)num2));
			float time = StatsManager.instance.SaveFileGetTimePlayed(text);
			component.saveFileHeaderLevel.text = string.Concat(new string[]
			{
				"<sprite name=truck> <color=#",
				text4,
				">",
				num2.ToString(),
				"</color>"
			});
			component.saveFileHeader.text = text2;
			Color numberColor = new Color(0.1f, 0.4f, 0.8f);
			Color unitColor = new Color(0.05f, 0.3f, 0.6f);
			component.saveFileInfoRow1.text = "<sprite name=clock>  " + SemiFunc.TimeToString(time, true, numberColor, unitColor);
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + num, gameObject.transform.localPosition.z);
			float num3 = gameObject.GetComponent<RectTransform>().rect.height + 2f;
			num -= num3;
			this.saveFileYOffset = num3;
			this.saveFiles.Add(gameObject.GetComponent<MenuElementSaveFile>());
		}
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			this.gameModeHeader.text = "Multiplayer mode";
			return;
		}
		this.gameModeHeader.text = "Singleplayer mode";
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x00098684 File Offset: 0x00096884
	public void OnGoBack()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x000986A0 File Offset: 0x000968A0
	private void Update()
	{
		if (this.saveFileInfoPanel.color != new Color(0f, 0f, 0f, 1f))
		{
			this.saveFileInfoPanel.color = Color.Lerp(this.saveFileInfoPanel.color, new Color(0f, 0f, 0f, 1f), Time.deltaTime * 10f);
		}
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x00098718 File Offset: 0x00096918
	public void OnNewGame()
	{
		if (this.saveFiles.Count >= 10)
		{
			MenuManager.instance.PageCloseAllAddedOnTop();
			MenuManager.instance.PagePopUp("Save file limit reached", Color.red, "You can only have 10 save files at a time. Please delete some save files to make room for new ones.", "OK");
			return;
		}
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame(null);
			return;
		}
		SemiFunc.MenuActionSingleplayerGame(null);
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x00098771 File Offset: 0x00096971
	public void OnLoadGame()
	{
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame(this.currentSaveFileName);
			return;
		}
		SemiFunc.MenuActionSingleplayerGame(this.currentSaveFileName);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x00098794 File Offset: 0x00096994
	public void OnDeleteGame()
	{
		SemiFunc.SaveFileDelete(this.currentSaveFileName);
		bool flag = false;
		foreach (MenuElementSaveFile menuElementSaveFile in this.saveFiles)
		{
			if (flag && menuElementSaveFile)
			{
				RectTransform component = menuElementSaveFile.GetComponent<RectTransform>();
				component.localPosition = new Vector3(component.localPosition.x, component.localPosition.y + this.saveFileYOffset, component.localPosition.z);
				MenuElementAnimations component2 = menuElementSaveFile.GetComponent<MenuElementAnimations>();
				component2.UIAniNudgeY(10f, 0.2f, 1f);
				component2.UIAniRotate(2f, 0.2f, 1f);
				component2.UIAniNewInitialPosition(new Vector2(component.localPosition.x, component.localPosition.y));
			}
			if (menuElementSaveFile.saveFileName == this.currentSaveFileName)
			{
				Object.Destroy(menuElementSaveFile.gameObject);
				flag = true;
			}
		}
		this.saveFiles.RemoveAll((MenuElementSaveFile x) => x == null);
		this.GoBackToDefaultInfo();
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x000988E0 File Offset: 0x00096AE0
	public void GoBackToDefaultInfo()
	{
		MenuElementAnimations component = this.saveFileInfo.GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX(10f, 0.2f, 1f);
		component.UIAniRotate(2f, 0.2f, 1f);
		this.saveInfoDefault.SetActive(true);
		this.saveInfoSelected.SetActive(false);
		this.saveFileInfoPanel.color = new Color(0.45f, 0f, 0f, 1f);
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00098960 File Offset: 0x00096B60
	private void InfoPlayerNames(TextMeshProUGUI _textMesh, string _fileName)
	{
		_textMesh.text = "";
		List<string> list = StatsManager.instance.SaveFileGetPlayerNames(_fileName);
		list.Sort((string a, string b) => a.Length.CompareTo(b.Length));
		if (list != null)
		{
			int count = list.Count;
			int num = 0;
			foreach (string str in list)
			{
				if (num == count - 1)
				{
					_textMesh.text += str;
				}
				else if (num == count - 2)
				{
					_textMesh.text = _textMesh.text + str + "<color=#444444>   and   </color>";
				}
				else
				{
					_textMesh.text = _textMesh.text + str + "<color=#444444>,</color>   ";
				}
				num++;
			}
		}
		if (list == null || (list != null && list.Count == 0))
		{
			_textMesh.text += "You did it all alone!";
		}
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x00098A70 File Offset: 0x00096C70
	public void SaveFileSelected(string saveFileName)
	{
		MenuElementAnimations component = this.saveFileInfo.GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX(10f, 0.2f, 1f);
		component.UIAniRotate(2f, 0.2f, 1f);
		this.saveInfoDefault.SetActive(false);
		this.saveInfoSelected.SetActive(true);
		this.saveFileInfoPanel.color = new Color(0f, 0.1f, 0.25f, 1f);
		string text = StatsManager.instance.SaveFileGetTeamName(saveFileName);
		string text2 = StatsManager.instance.SaveFileGetDateAndTime(saveFileName);
		this.saveFileHeader.text = text;
		this.saveFileHeaderDate.text = text2;
		this.currentSaveFileName = saveFileName;
		string str = "      ";
		float time = StatsManager.instance.SaveFileGetTimePlayed(saveFileName);
		int num = int.Parse(StatsManager.instance.SaveFileGetRunLevel(saveFileName)) + 1;
		string text3 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float)num));
		string text4 = StatsManager.instance.SaveFileGetRunCurrency(saveFileName);
		this.saveFileInfoRow1.text = string.Concat(new string[]
		{
			"<sprite name=truck>  <color=#",
			text3,
			"><b>",
			num.ToString(),
			"</b></color>"
		});
		TextMeshProUGUI textMeshProUGUI = this.saveFileInfoRow1;
		textMeshProUGUI.text += str;
		TextMeshProUGUI textMeshProUGUI2 = this.saveFileInfoRow1;
		textMeshProUGUI2.text = textMeshProUGUI2.text + "<sprite name=clock>  " + SemiFunc.TimeToString(time, true, new Color(0.1f, 0.4f, 0.8f), new Color(0.05f, 0.3f, 0.6f));
		TextMeshProUGUI textMeshProUGUI3 = this.saveFileInfoRow1;
		textMeshProUGUI3.text += str;
		string text5 = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
		TextMeshProUGUI textMeshProUGUI4 = this.saveFileInfoRow1;
		textMeshProUGUI4.text = string.Concat(new string[]
		{
			textMeshProUGUI4.text,
			"<sprite name=$$>  <b>",
			text4,
			"</b><color=#",
			text5,
			">k</color>"
		});
		string text6 = SemiFunc.DollarGetString(int.Parse(StatsManager.instance.SaveFileGetTotalHaul(saveFileName)));
		this.saveFileInfoRow2.text = string.Concat(new string[]
		{
			"<color=#",
			text5,
			"><sprite name=$$$> TOTAL HAUL:      <b></b>$ </color><b>",
			text6,
			"</b><color=#",
			text5,
			">k</color>"
		});
		this.InfoPlayerNames(this.saveFileInfoRow3, saveFileName);
	}

	// Token: 0x04001C4C RID: 7244
	public RectTransform saveFileInfo;

	// Token: 0x04001C4D RID: 7245
	public GameObject saveInfoDefault;

	// Token: 0x04001C4E RID: 7246
	public GameObject saveInfoSelected;

	// Token: 0x04001C4F RID: 7247
	public TextMeshProUGUI saveFileHeader;

	// Token: 0x04001C50 RID: 7248
	public TextMeshProUGUI saveFileHeaderDate;

	// Token: 0x04001C51 RID: 7249
	public TextMeshProUGUI saveFileInfoRow1;

	// Token: 0x04001C52 RID: 7250
	public TextMeshProUGUI saveFileInfoRow2;

	// Token: 0x04001C53 RID: 7251
	public TextMeshProUGUI saveFileInfoRow3;

	// Token: 0x04001C54 RID: 7252
	private Image saveFileInfoPanel;

	// Token: 0x04001C55 RID: 7253
	public RectTransform Scroller;

	// Token: 0x04001C56 RID: 7254
	public RectTransform saveFilePosition;

	// Token: 0x04001C57 RID: 7255
	public GameObject saveFilePrefab;

	// Token: 0x04001C58 RID: 7256
	internal string currentSaveFileName;

	// Token: 0x04001C59 RID: 7257
	internal List<MenuElementSaveFile> saveFiles = new List<MenuElementSaveFile>();

	// Token: 0x04001C5A RID: 7258
	internal float saveFileYOffset;

	// Token: 0x04001C5B RID: 7259
	public TextMeshProUGUI gameModeHeader;
}
