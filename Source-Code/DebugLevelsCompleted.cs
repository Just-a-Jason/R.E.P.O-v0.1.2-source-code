using System;
using TMPro;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class DebugLevelsCompleted : MonoBehaviour
{
	// Token: 0x0600121E RID: 4638 RVA: 0x000A040E File Offset: 0x0009E60E
	private void Update()
	{
		this.Text.text = "Levels Completed: " + RunManager.instance.levelsCompleted.ToString();
	}

	// Token: 0x04001ED0 RID: 7888
	public TextMeshProUGUI Text;
}
