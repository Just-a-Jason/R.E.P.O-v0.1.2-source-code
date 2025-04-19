using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class TrapTypeIdentifier : MonoBehaviour
{
	// Token: 0x060011EF RID: 4591 RVA: 0x0009F1BC File Offset: 0x0009D3BC
	private void Start()
	{
		Module componentInParent = base.GetComponentInParent<Module>();
		Debug.LogError(string.Concat(new string[]
		{
			"Remove + '",
			this.trapType,
			"' in '",
			componentInParent.gameObject.name,
			"'"
		}));
		TrapDirector.instance.TrapList.Add(base.gameObject);
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0009F224 File Offset: 0x0009D424
	[PunRPC]
	private void DestroyTrigger()
	{
		Object.Destroy(this.Trigger);
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0009F231 File Offset: 0x0009D431
	[PunRPC]
	private void DestroyTrap()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001E3D RID: 7741
	public string trapType;

	// Token: 0x04001E3E RID: 7742
	[Header("Must add the trigger!")]
	public GameObject Trigger;

	// Token: 0x04001E3F RID: 7743
	public bool OnlyRemoveTrigger;

	// Token: 0x04001E40 RID: 7744
	[HideInInspector]
	public bool TriggerRemoved;
}
