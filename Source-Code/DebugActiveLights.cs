using System;
using TMPro;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class DebugActiveLights : MonoBehaviour
{
	// Token: 0x06001218 RID: 4632 RVA: 0x000A0390 File Offset: 0x0009E590
	private void Awake()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x000A039E File Offset: 0x0009E59E
	private void Update()
	{
		this.text.text = "Active Lights: " + LightManager.instance.activeLightsAmount.ToString();
	}

	// Token: 0x04001ECE RID: 7886
	private TextMeshProUGUI text;
}
