using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001FC RID: 508
public class MenuColorSelected : MonoBehaviour
{
	// Token: 0x060010C3 RID: 4291 RVA: 0x00096BA0 File Offset: 0x00094DA0
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.positionSpring = new SpringVector3();
		this.positionSpring.speed = 50f;
		this.positionSpring.damping = 0.55f;
		this.positionSpring.lastPosition = base.transform.position;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00096BFA File Offset: 0x00094DFA
	public void SetColor(Color color, Vector3 position)
	{
		base.StartCoroutine(this.SetColorRoutine(color, position));
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00096C0B File Offset: 0x00094E0B
	private IEnumerator SetColorRoutine(Color color, Vector3 position)
	{
		yield return new WaitForSeconds(0.01f);
		this.rawImage.color = color;
		this.selectedPosition = position;
		this.goTime = true;
		yield break;
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00096C28 File Offset: 0x00094E28
	private void Update()
	{
		if (!this.goTime)
		{
			return;
		}
		if (this.parentPage.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		base.transform.position = SemiFunc.SpringVector3Get(this.positionSpring, this.selectedPosition + Vector3.up * 0.038f + Vector3.right * 0.046f, -1f);
		base.transform.position = new Vector3(base.transform.position.x + 18f, base.transform.position.y + 16f, 1f);
	}

	// Token: 0x04001BFA RID: 7162
	public SpringVector3 positionSpring;

	// Token: 0x04001BFB RID: 7163
	internal Vector3 selectedPosition;

	// Token: 0x04001BFC RID: 7164
	public RawImage rawImage;

	// Token: 0x04001BFD RID: 7165
	private MenuPage parentPage;

	// Token: 0x04001BFE RID: 7166
	private bool goTime;
}
