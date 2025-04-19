using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class AnimateTextPeriods : MonoBehaviour
{
	// Token: 0x06000F82 RID: 3970 RVA: 0x0008E279 File Offset: 0x0008C479
	private void Awake()
	{
		this.textMesh = base.GetComponent<TextMeshProUGUI>();
		this.textString = this.textMesh.text;
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0008E298 File Offset: 0x0008C498
	private void OnEnable()
	{
		this.animateCoroutine = base.StartCoroutine(this.AnimateDots());
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0008E2AC File Offset: 0x0008C4AC
	private IEnumerator AnimateDots()
	{
		for (;;)
		{
			this.textMesh.text = this.textString + "...".Substring(0, Mathf.FloorToInt(Time.unscaledTime * 8f % 4f));
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0008E2BB File Offset: 0x0008C4BB
	private void OnDisable()
	{
		if (this.animateCoroutine != null)
		{
			base.StopCoroutine(this.animateCoroutine);
		}
	}

	// Token: 0x04001A62 RID: 6754
	private TextMeshProUGUI textMesh;

	// Token: 0x04001A63 RID: 6755
	private string textString;

	// Token: 0x04001A64 RID: 6756
	private Coroutine animateCoroutine;
}
