using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class MapModule : MonoBehaviour
{
	// Token: 0x06000CEC RID: 3308 RVA: 0x000713BD File Offset: 0x0006F5BD
	public void Hide()
	{
		if (this.animating)
		{
			return;
		}
		this.animating = true;
		base.StartCoroutine(this.HideAnimation());
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x000713DC File Offset: 0x0006F5DC
	private void Update()
	{
		if (Map.Instance.Active)
		{
			this.graphic.transform.rotation = DirtFinderMapPlayer.Instance.transform.rotation;
			this.graphic.transform.rotation = Quaternion.Euler(new Vector3(90f, this.graphic.transform.rotation.eulerAngles.y, this.graphic.transform.rotation.eulerAngles.z));
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0007146D File Offset: 0x0006F66D
	private IEnumerator HideAnimation()
	{
		while (this.curveLerp < 1f)
		{
			this.curveLerp += this.speed * Time.deltaTime;
			base.transform.localScale = Vector3.one * this.curve.Evaluate(this.curveLerp);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040014AB RID: 5291
	public Module module;

	// Token: 0x040014AC RID: 5292
	public AnimationCurve curve;

	// Token: 0x040014AD RID: 5293
	public float speed;

	// Token: 0x040014AE RID: 5294
	private float curveLerp;

	// Token: 0x040014AF RID: 5295
	private bool animating;

	// Token: 0x040014B0 RID: 5296
	public Transform graphic;
}
