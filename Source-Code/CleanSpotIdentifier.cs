using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class CleanSpotIdentifier : MonoBehaviour
{
	// Token: 0x060006C7 RID: 1735 RVA: 0x00040C2D File Offset: 0x0003EE2D
	private void Start()
	{
		CleanDirector.instance.CleanList.Add(base.gameObject);
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00040C44 File Offset: 0x0003EE44
	private void Update()
	{
	}

	// Token: 0x04000B76 RID: 2934
	public Interaction.InteractionType InteractionType;
}
