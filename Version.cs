using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000108 RID: 264
[CreateAssetMenu(fileName = "Version - ", menuName = "Other/Version", order = 0)]
public class Version : ScriptableObject
{
	// Token: 0x06000910 RID: 2320 RVA: 0x00056364 File Offset: 0x00054564
	private void Discord()
	{
		string text = "@Tester";
		text = text + "\n# __R.E.P.O. " + this.title + " is now up on the tester branch!__ :taxman_laugh:";
		if (this.newList.Count > 0)
		{
			text += "\n\n";
			text += "## NEW";
			foreach (string str in this.newList)
			{
				text = text + "\n> - " + str;
			}
		}
		if (this.changesList.Count > 0)
		{
			text += "\n\n";
			text += "## CHANGES";
			foreach (string str2 in this.changesList)
			{
				text = text + "\n> - " + str2;
			}
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n\n";
			text += "## BALANCING";
			foreach (string str3 in this.balancingList)
			{
				text = text + "\n> - " + str3;
			}
		}
		if (this.fixList.Count > 0)
		{
			text += "\n\n";
			text += "## FIXES";
			foreach (string str4 in this.fixList)
			{
				text = text + "\n> - " + str4;
			}
		}
		text += "\n\n";
		text += "# __Thanks for helping us test! :smile~1:__";
		GUIUtility.systemCopyBuffer = text;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0005656C File Offset: 0x0005476C
	private void Steam()
	{
		string text = "[hr][/hr]";
		text += "[b][list]";
		if (this.newList.Count > 0)
		{
			text += "\n[*][url=#NEW]NEW[/url]";
		}
		if (this.changesList.Count > 0)
		{
			text += "\n[*][url=#CHANGES]CHANGES[/url]";
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n[*][url=#BALANCING]BALANCING[/url]";
		}
		if (this.fixList.Count > 0)
		{
			text += "\n[*][url=#FIXES]FIXES[/url]";
		}
		text += "\n[/list][/b]";
		if (this.newList.Count > 0)
		{
			text += "\n[hr][/hr][h2=NEW]NEW[/h2][list]";
			foreach (string str in this.newList)
			{
				text = text + "\n[*]" + str;
			}
			text += "[/list]";
		}
		if (this.changesList.Count > 0)
		{
			text += "\n[hr][/hr][h2=CHANGES]CHANGES[/h2][list]";
			foreach (string str2 in this.changesList)
			{
				text = text + "\n[*]" + str2;
			}
			text += "[/list]";
		}
		if (this.balancingList.Count > 0)
		{
			text += "\n[hr][/hr][h2=BALANCING]BALANCING[/h2][list]";
			foreach (string str3 in this.balancingList)
			{
				text = text + "\n[*]" + str3;
			}
			text += "[/list]";
		}
		if (this.fixList.Count > 0)
		{
			text += "\n[hr][/hr][h2=FIXES]FIXES[/h2][list]";
			foreach (string str4 in this.fixList)
			{
				text = text + "\n[*]" + str4;
			}
			text += "[/list]";
		}
		text += "\n[hr][/hr]";
		GUIUtility.systemCopyBuffer = text;
	}

	// Token: 0x0400107D RID: 4221
	public string title = "v0.0.0";

	// Token: 0x0400107E RID: 4222
	[TextArea(0, 10)]
	public List<string> newList = new List<string>();

	// Token: 0x0400107F RID: 4223
	[TextArea(0, 10)]
	public List<string> changesList = new List<string>();

	// Token: 0x04001080 RID: 4224
	[TextArea(0, 10)]
	public List<string> balancingList = new List<string>();

	// Token: 0x04001081 RID: 4225
	[TextArea(0, 10)]
	public List<string> fixList = new List<string>();
}
