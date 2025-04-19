using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class DirtFinderCounter : MonoBehaviour
{
	// Token: 0x06000CAB RID: 3243 RVA: 0x0006FB54 File Offset: 0x0006DD54
	private void Start()
	{
		this.PlayerAmount = GameDirector.instance.PlayerList.Count;
		this.UpdateNumbers();
		if (GameManager.Multiplayer() && this.Controller && this.Controller.photonView && !this.Controller.photonView.IsMine)
		{
			this.SoundDown.SpatialBlend = 1f;
			this.SoundUp.SpatialBlend = 1f;
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0006FBD4 File Offset: 0x0006DDD4
	private void OnEnable()
	{
		if (this.Controller.PlayerAvatar.upgradeMapPlayerCount > 0)
		{
			this.HatchObject.SetActive(false);
			this.HatchLightObject.SetActive(true);
			this.active = true;
			return;
		}
		this.HatchObject.SetActive(true);
		this.HatchLightObject.SetActive(false);
		this.active = false;
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0006FC33 File Offset: 0x0006DE33
	private void OnDisable()
	{
		this.PitchUpdated = 0;
		this.UpdateTimer = 0.8f;
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0006FC48 File Offset: 0x0006DE48
	private void Update()
	{
		if (!this.active)
		{
			return;
		}
		if (this.UpdateTimer <= 0f)
		{
			int num = 0;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.isDisabled)
					{
						num++;
					}
				}
			}
			if (this.PlayerAmount != num)
			{
				float pitch = Mathf.Clamp(1f + (float)this.PitchUpdated * this.PitchIncrease, 1f, this.PitchMax);
				this.PitchUpdated++;
				if (this.PlayerAmount > num)
				{
					this.SoundDown.Pitch = pitch;
					this.SoundDown.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.PlayerAmount--;
				}
				else
				{
					this.SoundUp.Pitch = pitch;
					this.SoundUp.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.PlayerAmount++;
				}
				this.UpdateNumbers();
				this.UpdateTimer = this.UpdateTime - this.UpdateTimerDecrease;
				this.UpdateTimer = Mathf.Clamp(this.UpdateTimer, this.UpdateTimeMin, this.UpdateTime);
				this.UpdateTimerDecrease += this.UpdateTimeDecrease;
			}
			else
			{
				this.UpdateTimerDecrease = 0f;
			}
		}
		else
		{
			this.UpdateTimer -= Time.deltaTime;
		}
		if (this.NumberCurveLerp < 1f)
		{
			this.NumberCurveLerp += Time.deltaTime * this.NumberCurveSpeed;
			this.NumberCurveLerp = Mathf.Clamp01(this.NumberCurveLerp);
		}
		this.NumberText.transform.localPosition = new Vector3(this.NumberText.transform.localPosition.x, this.NumberCurve.Evaluate(this.NumberCurveLerp) * this.NumberCurveAmount, this.NumberText.transform.localPosition.z);
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0006FE8C File Offset: 0x0006E08C
	private void UpdateNumbers()
	{
		string text = this.NumberText.text;
		this.NumberText.text = this.PlayerAmount.ToString();
		if (text != this.NumberText.text)
		{
			this.NumberCurveLerp = 0f;
		}
	}

	// Token: 0x0400142B RID: 5163
	public MapToolController Controller;

	// Token: 0x0400142C RID: 5164
	[Space]
	public TextMeshPro NumberText;

	// Token: 0x0400142D RID: 5165
	[Space]
	public GameObject HatchObject;

	// Token: 0x0400142E RID: 5166
	public GameObject HatchLightObject;

	// Token: 0x0400142F RID: 5167
	[Space]
	public float UpdateTime;

	// Token: 0x04001430 RID: 5168
	public float UpdateTimeMin;

	// Token: 0x04001431 RID: 5169
	public float UpdateTimeDecrease;

	// Token: 0x04001432 RID: 5170
	private float UpdateTimer;

	// Token: 0x04001433 RID: 5171
	private float UpdateTimerDecrease;

	// Token: 0x04001434 RID: 5172
	[Space]
	public float NumberCurveAmount;

	// Token: 0x04001435 RID: 5173
	public float NumberCurveSpeed;

	// Token: 0x04001436 RID: 5174
	public AnimationCurve NumberCurve;

	// Token: 0x04001437 RID: 5175
	private float NumberCurveLerp = 1f;

	// Token: 0x04001438 RID: 5176
	[Space]
	public Sound SoundDown;

	// Token: 0x04001439 RID: 5177
	public Sound SoundUp;

	// Token: 0x0400143A RID: 5178
	private int PitchUpdated;

	// Token: 0x0400143B RID: 5179
	private int PlayerAmount;

	// Token: 0x0400143C RID: 5180
	[Space]
	public float PitchIncrease = 0.1f;

	// Token: 0x0400143D RID: 5181
	public float PitchMax = 3f;

	// Token: 0x0400143E RID: 5182
	private bool active;
}
