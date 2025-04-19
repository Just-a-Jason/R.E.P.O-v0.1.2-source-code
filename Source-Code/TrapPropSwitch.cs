using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class TrapPropSwitch : MonoBehaviour
{
	// Token: 0x060011FA RID: 4602 RVA: 0x0009F4C7 File Offset: 0x0009D6C7
	private void Start()
	{
		this.TrapParent.SetActive(true);
		this.PropParent.SetActive(true);
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x0009F4EE File Offset: 0x0009D6EE
	public IEnumerator Setup()
	{
		while (!TrapDirector.instance.TrapListUpdated)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		Trap componentInChildren = base.GetComponentInChildren<Trap>();
		if (componentInChildren != null && componentInChildren.gameObject.activeSelf)
		{
			this.PropParent.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0009F500 File Offset: 0x0009D700
	public void DebugToggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Trap Active";
			if (this.TrapParent != null)
			{
				this.TrapParent.SetActive(true);
			}
			if (this.PropParent != null)
			{
				this.PropParent.SetActive(false);
				return;
			}
		}
		else
		{
			this.DebugSwitch = true;
			this.DebugState = "Prop Active";
			if (this.PropParent != null)
			{
				this.PropParent.SetActive(true);
			}
			if (this.TrapParent != null)
			{
				this.TrapParent.SetActive(false);
			}
		}
	}

	// Token: 0x04001E4F RID: 7759
	public GameObject TrapParent;

	// Token: 0x04001E50 RID: 7760
	public GameObject PropParent;

	// Token: 0x04001E51 RID: 7761
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x04001E52 RID: 7762
	[HideInInspector]
	public bool DebugSwitch;
}
