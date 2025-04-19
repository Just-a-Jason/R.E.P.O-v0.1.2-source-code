using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class ValuableVolume : MonoBehaviour
{
	// Token: 0x06001420 RID: 5152 RVA: 0x000B0D50 File Offset: 0x000AEF50
	private void Start()
	{
		this.Module = base.GetComponentInParent<Module>();
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x000B0D60 File Offset: 0x000AEF60
	public void Setup()
	{
		ValuablePropSwitch componentInParent = base.GetComponentInParent<ValuablePropSwitch>();
		if (componentInParent && base.transform.parent != componentInParent.ValuableParent.transform)
		{
			Debug.LogError("Valuable Volume: Child of ValuablePropSwitch but not valuable parent...", base.gameObject);
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		bool flag = true;
		if (Debug.isDebugBuild)
		{
			base.StartCoroutine(this.SafetyCheck());
			flag = false;
		}
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 2f))
		{
			if (collider.gameObject.CompareTag("Phys Grab Object"))
			{
				ValuableObject componentInParent2 = collider.transform.GetComponentInParent<ValuableObject>();
				if (componentInParent2 && componentInParent2.volumeType == this.VolumeType && Vector3.Distance(componentInParent2.transform.position, base.transform.position) < 0.1f)
				{
					componentInParent2.transform.parent = base.transform.parent;
					break;
				}
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x000B0E79 File Offset: 0x000AF079
	private IEnumerator SafetyCheck()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		Mesh mesh = null;
		switch (this.VolumeType)
		{
		case ValuableVolume.Type.Tiny:
			mesh = AssetManager.instance.valuableMeshTiny;
			break;
		case ValuableVolume.Type.Small:
			mesh = AssetManager.instance.valuableMeshSmall;
			break;
		case ValuableVolume.Type.Medium:
			mesh = AssetManager.instance.valuableMeshMedium;
			break;
		case ValuableVolume.Type.Big:
			mesh = AssetManager.instance.valuableMeshBig;
			break;
		case ValuableVolume.Type.Wide:
			mesh = AssetManager.instance.valuableMeshWide;
			break;
		case ValuableVolume.Type.Tall:
			mesh = AssetManager.instance.valuableMeshTall;
			break;
		case ValuableVolume.Type.VeryTall:
			mesh = AssetManager.instance.valuableMeshVeryTall;
			break;
		}
		Vector3 size = mesh.bounds.size;
		Collider[] array = Physics.OverlapBox(base.transform.position + base.transform.forward * size.z / 2f + base.transform.up * size.y / 2f + Vector3.up * 0.01f, size / 2f, base.transform.rotation, LayerMask.GetMask(new string[]
		{
			"Default"
		}), QueryTriggerInteraction.Ignore);
		if (array.Length != 0)
		{
			Debug.LogError("Valuable Volume: Overlapping colliders:", base.gameObject);
			foreach (Collider collider in array)
			{
				Debug.LogError("     " + collider.gameObject.name, collider.gameObject);
			}
		}
		yield break;
	}

	// Token: 0x04002246 RID: 8774
	public ValuableVolume.Type VolumeType;

	// Token: 0x04002247 RID: 8775
	[HideInInspector]
	public Module Module;

	// Token: 0x04002248 RID: 8776
	private Mesh MeshTiny;

	// Token: 0x04002249 RID: 8777
	private Mesh MeshSmall;

	// Token: 0x0400224A RID: 8778
	private Mesh MeshMedium;

	// Token: 0x0400224B RID: 8779
	private Mesh MeshBig;

	// Token: 0x0400224C RID: 8780
	private Mesh MeshWide;

	// Token: 0x0400224D RID: 8781
	private Mesh MeshTall;

	// Token: 0x0400224E RID: 8782
	private Mesh MeshVeryTall;

	// Token: 0x020003C5 RID: 965
	public enum Type
	{
		// Token: 0x040028E9 RID: 10473
		Tiny,
		// Token: 0x040028EA RID: 10474
		Small,
		// Token: 0x040028EB RID: 10475
		Medium,
		// Token: 0x040028EC RID: 10476
		Big,
		// Token: 0x040028ED RID: 10477
		Wide,
		// Token: 0x040028EE RID: 10478
		Tall,
		// Token: 0x040028EF RID: 10479
		VeryTall
	}
}
