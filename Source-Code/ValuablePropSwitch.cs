using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class ValuablePropSwitch : MonoBehaviour
{
	// Token: 0x0600141C RID: 5148 RVA: 0x000B0B8D File Offset: 0x000AED8D
	private void Start()
	{
		this.ValuableParent.SetActive(true);
		this.PropParent.SetActive(false);
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x000B0BA8 File Offset: 0x000AEDA8
	public void Setup()
	{
		ValuablePropSwitch[] componentsInParent = base.gameObject.GetComponentsInParent<ValuablePropSwitch>(true);
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (componentsInParent[i] != this)
			{
				Debug.LogError("ValuablePropSwitch: Switches inside switches is not supported...", base.gameObject);
			}
		}
		if (!base.gameObject.GetComponentInChildren<ValuableVolume>(true))
		{
			Debug.LogError(base.gameObject.GetComponentInParent<Module>().gameObject.name + "  |  ValuablePropSwitch: No ValuableVolume found in children...", base.gameObject);
			return;
		}
		bool flag = false;
		ValuableObject[] componentsInChildren = base.GetComponentsInChildren<ValuableObject>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.activeSelf)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.PropParent.SetActive(false);
			this.ValuableParent.SetActive(true);
		}
		else
		{
			this.ValuableParent.SetActive(false);
			this.PropParent.SetActive(true);
		}
		this.SetupComplete = true;
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x000B0C90 File Offset: 0x000AEE90
	public void DebugToggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Valuable Active";
			if (this.ValuableParent != null)
			{
				this.ValuableParent.SetActive(true);
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
			if (this.ValuableParent != null)
			{
				this.ValuableParent.SetActive(false);
			}
		}
	}

	// Token: 0x04002240 RID: 8768
	public GameObject ValuableParent;

	// Token: 0x04002241 RID: 8769
	public GameObject PropParent;

	// Token: 0x04002242 RID: 8770
	internal bool SetupComplete;

	// Token: 0x04002243 RID: 8771
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x04002244 RID: 8772
	[HideInInspector]
	public bool DebugSwitch;

	// Token: 0x04002245 RID: 8773
	[HideInInspector]
	public string ChildValuableString = "...";
}
