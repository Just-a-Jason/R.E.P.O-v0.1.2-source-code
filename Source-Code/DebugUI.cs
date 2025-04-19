using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class DebugUI : MonoBehaviour
{
	// Token: 0x06001222 RID: 4642 RVA: 0x000A0496 File Offset: 0x0009E696
	private void Start()
	{
		if (!Application.isEditor)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x000A04AA File Offset: 0x0009E6AA
	private void Update()
	{
		if (SemiFunc.DebugDev() && Input.GetKeyDown(KeyCode.F1))
		{
			this.enableParent.SetActive(!this.enableParent.activeSelf);
		}
	}

	// Token: 0x04001ED2 RID: 7890
	public GameObject enableParent;
}
