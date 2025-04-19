using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class CandleFlame : MonoBehaviour
{
	// Token: 0x06000913 RID: 2323 RVA: 0x00056814 File Offset: 0x00054A14
	private void Awake()
	{
		if (!this.propLight || !this.propLight.lightComponent)
		{
			Debug.LogError("Candle Flame missing Prop Light!", base.gameObject);
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.logicActive)
		{
			base.StartCoroutine(this.LogicCoroutine());
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00056872 File Offset: 0x00054A72
	private void OnEnable()
	{
		if (!this.logicActive)
		{
			base.StartCoroutine(this.LogicCoroutine());
		}
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00056889 File Offset: 0x00054A89
	private void OnDisable()
	{
		this.logicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00056898 File Offset: 0x00054A98
	private IEnumerator LogicCoroutine()
	{
		this.logicActive = true;
		for (;;)
		{
			yield return new WaitForSeconds(this.Logic());
		}
		yield break;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000568A8 File Offset: 0x00054AA8
	private float Logic()
	{
		if (this.propLight.turnedOff)
		{
			base.gameObject.SetActive(false);
			return 999f;
		}
		if (this.propLight.lightComponent.intensity > 0f)
		{
			this.flickerXLerp += Time.deltaTime * this.flickerXSpeed;
			if (this.flickerXLerp >= 1f)
			{
				this.flickerXLerp = 0f;
				this.flickerXOld = this.flickerXNew;
				this.flickerXNew = Random.Range(0.8f, 1.2f);
				this.flickerXSpeed = Random.Range(25f, 75f);
			}
			this.flickerYLerp += Time.deltaTime * this.flickerYSpeed;
			if (this.flickerYLerp >= 1f)
			{
				this.flickerYLerp = 0f;
				this.flickerYOld = this.flickerYNew;
				this.flickerYNew = Random.Range(0.8f, 1.2f);
				this.flickerYSpeed = Random.Range(25f, 75f);
			}
			this.flickerZLerp += Time.deltaTime * this.flickerZSpeed;
			if (this.flickerZLerp >= 1f)
			{
				this.flickerZLerp = 0f;
				this.flickerZOld = this.flickerZNew;
				this.flickerZNew = Random.Range(0.8f, 1.2f);
				this.flickerZSpeed = Random.Range(25f, 75f);
			}
			base.transform.localScale = new Vector3(Mathf.Lerp(this.flickerXOld, this.flickerXNew, this.flickerCurve.Evaluate(this.flickerXLerp)), Mathf.Lerp(this.flickerYOld, this.flickerYNew, this.flickerCurve.Evaluate(this.flickerYLerp)), Mathf.Lerp(this.flickerZOld, this.flickerZNew, this.flickerCurve.Evaluate(this.flickerZLerp)));
			return 0.025f;
		}
		return 2f;
	}

	// Token: 0x04001082 RID: 4226
	private bool logicActive;

	// Token: 0x04001083 RID: 4227
	public PropLight propLight;

	// Token: 0x04001084 RID: 4228
	public AnimationCurve flickerCurve;

	// Token: 0x04001085 RID: 4229
	public AnimationCurve swayCurve;

	// Token: 0x04001086 RID: 4230
	private float flickerXNew;

	// Token: 0x04001087 RID: 4231
	private float flickerXOld;

	// Token: 0x04001088 RID: 4232
	private float flickerXLerp = 1f;

	// Token: 0x04001089 RID: 4233
	private float flickerXSpeed;

	// Token: 0x0400108A RID: 4234
	private float flickerYNew;

	// Token: 0x0400108B RID: 4235
	private float flickerYOld;

	// Token: 0x0400108C RID: 4236
	private float flickerYLerp = 1f;

	// Token: 0x0400108D RID: 4237
	private float flickerYSpeed;

	// Token: 0x0400108E RID: 4238
	private float flickerZNew;

	// Token: 0x0400108F RID: 4239
	private float flickerZOld;

	// Token: 0x04001090 RID: 4240
	private float flickerZLerp = 1f;

	// Token: 0x04001091 RID: 4241
	private float flickerZSpeed;
}
