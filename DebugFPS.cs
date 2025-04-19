using System;
using TMPro;
using UnityEngine;

// Token: 0x0200023C RID: 572
public class DebugFPS : MonoBehaviour
{
	// Token: 0x0600121B RID: 4635 RVA: 0x000A03CC File Offset: 0x0009E5CC
	private void Awake()
	{
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x000A03D0 File Offset: 0x0009E5D0
	private void Update()
	{
		this.Text.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
	}

	// Token: 0x04001ECF RID: 7887
	public TextMeshProUGUI Text;
}
