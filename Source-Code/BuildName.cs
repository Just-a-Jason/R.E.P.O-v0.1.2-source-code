using System;
using TMPro;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class BuildName : MonoBehaviour
{
	// Token: 0x0600090E RID: 2318 RVA: 0x0005633E File Offset: 0x0005453E
	private void Start()
	{
		base.GetComponent<TextMeshProUGUI>().text = BuildManager.instance.version.title;
	}
}
