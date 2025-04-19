using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class LightManager : MonoBehaviour
{
	// Token: 0x060008B8 RID: 2232 RVA: 0x00053A36 File Offset: 0x00051C36
	private void Awake()
	{
		LightManager.instance = this;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00053A3E File Offset: 0x00051C3E
	private void Start()
	{
		this.debugActive = SemiFunc.DebugDev();
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x00053A4C File Offset: 0x00051C4C
	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (!PlayerAvatar.instance)
		{
			return;
		}
		if (!this.lightCullTarget)
		{
			this.lightCullTarget = PlayerAvatar.instance.transform;
		}
		this.LogicUpdate();
		if (this.debugActive)
		{
			int num = 0;
			foreach (PropLight propLight in this.propLights)
			{
				if (propLight && propLight.gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			this.activeLightsAmount = num;
		}
		if (this.updateInstant)
		{
			this.updateInstantTimer -= Time.deltaTime;
			if (this.updateInstantTimer <= 0f)
			{
				this.updateInstant = false;
			}
		}
		if (RoundDirector.instance.allExtractionPointsCompleted && !this.turnOffLights)
		{
			base.StopAllCoroutines();
			this.turningOffLights = true;
			base.StartCoroutine(this.TurnOffLights());
			this.turningOffEmissions = true;
			base.StartCoroutine(this.TurnOffEmissions());
			this.turnOffLights = true;
		}
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00053B78 File Offset: 0x00051D78
	private IEnumerator TurnOffLights()
	{
		int _lightsPerFrame = 5;
		int _lightsPerFrameCounter = 0;
		foreach (PropLight propLight in this.propLights.ToList<PropLight>())
		{
			if (propLight && propLight.levelLight)
			{
				propLight.lightComponent.intensity = 0f;
				propLight.originalIntensity = 0f;
				if (propLight.hasHalo)
				{
					propLight.halo.enabled = false;
				}
				propLight.turnedOff = true;
				int num = _lightsPerFrameCounter;
				_lightsPerFrameCounter = num + 1;
				if (_lightsPerFrameCounter >= _lightsPerFrame)
				{
					_lightsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		List<PropLight>.Enumerator enumerator = default(List<PropLight>.Enumerator);
		this.turningOffLights = false;
		yield break;
		yield break;
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00053B87 File Offset: 0x00051D87
	private IEnumerator TurnOffEmissions()
	{
		int _emissionsPerFrame = 5;
		int _emissionsPerFrameCounter = 0;
		foreach (PropLightEmission propLightEmission in this.propEmissions.ToList<PropLightEmission>())
		{
			if (propLightEmission && propLightEmission.levelLight)
			{
				propLightEmission.material.SetColor("_EmissionColor", Color.black);
				propLightEmission.originalEmission = Color.black;
				propLightEmission.turnedOff = true;
				int num = _emissionsPerFrameCounter;
				_emissionsPerFrameCounter = num + 1;
				if (_emissionsPerFrameCounter >= _emissionsPerFrame)
				{
					_emissionsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		List<PropLightEmission>.Enumerator enumerator = default(List<PropLightEmission>.Enumerator);
		this.turningOffEmissions = false;
		yield break;
		yield break;
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00053B98 File Offset: 0x00051D98
	private void Setup()
	{
		this.setup = true;
		if (this.lightCullTarget)
		{
			this.lastCheckPos = this.lightCullTarget.position;
		}
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Prop Lights"))
		{
			if (gameObject)
			{
				PropLight component = gameObject.GetComponent<PropLight>();
				if (component)
				{
					this.propLights.Add(component);
				}
				else
				{
					Debug.LogError("PropLight component not found in " + gameObject.name, gameObject);
				}
			}
		}
		foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Prop Emission"))
		{
			if (gameObject2)
			{
				PropLightEmission component2 = gameObject2.GetComponent<PropLightEmission>();
				if (component2)
				{
					this.propEmissions.Add(component2);
				}
				else
				{
					Debug.LogError("PropLightEmission component not found in " + gameObject2.name, gameObject2);
				}
			}
		}
		foreach (PropLight propLight in this.propLights)
		{
			if (!propLight.turnedOff)
			{
				this.HandleLightActivation(propLight);
			}
		}
		foreach (PropLightEmission propLightEmission in this.propEmissions)
		{
			if (!propLightEmission.turnedOff)
			{
				this.HandleEmissionActivation(propLightEmission);
			}
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00053D24 File Offset: 0x00051F24
	private void LogicUpdate()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!this.setup)
		{
			this.Setup();
		}
		if (this.logicUpdateTimer > 0f)
		{
			this.logicUpdateTimer -= Time.deltaTime;
			return;
		}
		if (!this.lightCullTarget)
		{
			return;
		}
		bool flag = false;
		if (Mathf.Abs(this.lightCullTarget.eulerAngles.y - this.lastYRotation) >= 20f)
		{
			this.lastYRotation = this.lightCullTarget.eulerAngles.y;
			flag = true;
		}
		if (!this.turningOffLights && !this.turningOffEmissions && (Vector3.Distance(this.lastCheckPos, this.lightCullTarget.position) >= this.checkDistance || flag))
		{
			this.logicUpdateTimer = 0.5f;
			this.UpdateLights();
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00053E00 File Offset: 0x00052000
	private void UpdateLights()
	{
		this.lastCheckPos = this.lightCullTarget.position;
		List<PropLight> list = new List<PropLight>();
		foreach (PropLight propLight in this.propLights)
		{
			if (propLight)
			{
				this.HandleLightActivation(propLight);
			}
			else
			{
				list.Add(propLight);
			}
		}
		foreach (PropLight item in list)
		{
			this.propLights.Remove(item);
		}
		List<PropLightEmission> list2 = new List<PropLightEmission>();
		foreach (PropLightEmission propLightEmission in this.propEmissions)
		{
			if (propLightEmission)
			{
				this.HandleEmissionActivation(propLightEmission);
			}
			else
			{
				list2.Add(propLightEmission);
			}
		}
		foreach (PropLightEmission item2 in list2)
		{
			this.propEmissions.Remove(item2);
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00053F64 File Offset: 0x00052164
	public void RemoveLight(PropLight PropLight)
	{
		if (PropLight && this.propLights.Contains(PropLight))
		{
			this.propLights.Remove(PropLight);
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00053F8C File Offset: 0x0005218C
	private void HandleLightActivation(PropLight propLight)
	{
		if (!this.lightCullTarget)
		{
			return;
		}
		Vector3 position = propLight.transform.position;
		Vector3 position2 = this.lightCullTarget.position;
		bool flag = Vector3.Dot(propLight.transform.position - this.lightCullTarget.position, this.lightCullTarget.forward) <= -0.25f;
		if (SpectateCamera.instance)
		{
			flag = false;
			if (SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				position.y = 0f;
				position2.y = 0f;
			}
		}
		float num = Vector3.Distance(position, position2);
		float num2 = GraphicsManager.instance.lightDistance * propLight.lightRangeMultiplier;
		if (propLight.gameObject.activeInHierarchy && ((num >= num2 && !flag) || (num >= num2 * 0.8f && flag)))
		{
			base.StartCoroutine(this.FadeLightIntensity(propLight, 0f, Random.Range(this.fadeTimeMin, this.fadeTimeMax), delegate
			{
				propLight.gameObject.SetActive(false);
			}));
			return;
		}
		if (!propLight.gameObject.activeInHierarchy && num < num2)
		{
			propLight.gameObject.SetActive(true);
			propLight.lightComponent.intensity = 0f;
			base.StartCoroutine(this.FadeLightIntensity(propLight, propLight.originalIntensity, Random.Range(this.fadeTimeMin, this.fadeTimeMax), null));
		}
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x00054132 File Offset: 0x00052332
	public void UpdateInstant()
	{
		if (!this.setup)
		{
			return;
		}
		this.updateInstant = true;
		this.updateInstantTimer = 0.1f;
		this.UpdateLights();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00054158 File Offset: 0x00052358
	private void HandleEmissionActivation(PropLightEmission _propLightEmission)
	{
		if (!this.lightCullTarget)
		{
			return;
		}
		Vector3 position = _propLightEmission.transform.position;
		Vector3 position2 = this.lightCullTarget.position;
		if (SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
		{
			position.y = 0f;
			position2.y = 0f;
		}
		if (Vector3.Distance(position, position2) >= GraphicsManager.instance.lightDistance)
		{
			base.StartCoroutine(this.FadeEmissionIntensity(_propLightEmission, Color.black, Random.Range(this.fadeTimeMin, this.fadeTimeMax)));
			return;
		}
		base.StartCoroutine(this.FadeEmissionIntensity(_propLightEmission, _propLightEmission.originalEmission, Random.Range(this.fadeTimeMin, this.fadeTimeMax)));
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00054219 File Offset: 0x00052419
	private IEnumerator FadeLightIntensity(PropLight propLight, float targetIntensity, float duration, Action onComplete = null)
	{
		if (!propLight || !propLight.lightComponent)
		{
			yield break;
		}
		float startTime = Time.time;
		float startIntensity = propLight.lightComponent.intensity;
		while (Time.time - startTime < duration && !this.updateInstant)
		{
			if (!propLight || !propLight.lightComponent)
			{
				yield break;
			}
			float time = (Time.time - startTime) / duration;
			propLight.lightComponent.intensity = Mathf.Lerp(startIntensity, targetIntensity, this.fadeCullCurve.Evaluate(time));
			yield return null;
		}
		if (!propLight || !propLight.lightComponent)
		{
			yield break;
		}
		propLight.lightComponent.intensity = targetIntensity;
		if (Mathf.Approximately(targetIntensity, 0f) && propLight.gameObject.CompareTag("Prop Lights"))
		{
			propLight.gameObject.SetActive(false);
		}
		if (onComplete != null)
		{
			onComplete();
		}
		yield break;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00054245 File Offset: 0x00052445
	private IEnumerator FadeEmissionIntensity(PropLightEmission _propLightEmission, Color targetColor, float duration)
	{
		if (!_propLightEmission)
		{
			yield break;
		}
		float startTime = Time.time;
		Color startColor = _propLightEmission.material.GetColor("_EmissionColor");
		while (Time.time - startTime < duration && !this.updateInstant)
		{
			if (!_propLightEmission)
			{
				yield break;
			}
			float time = (Time.time - startTime) / duration;
			_propLightEmission.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, this.fadeCullCurve.Evaluate(time)));
			yield return null;
		}
		if (!_propLightEmission)
		{
			yield break;
		}
		_propLightEmission.material.SetColor("_EmissionColor", targetColor);
		yield break;
	}

	// Token: 0x04000FD7 RID: 4055
	[HideInInspector]
	public Transform lightCullTarget;

	// Token: 0x04000FD8 RID: 4056
	public float checkDistance = 5f;

	// Token: 0x04000FD9 RID: 4057
	public float fadeTimeMin = 1f;

	// Token: 0x04000FDA RID: 4058
	public float fadeTimeMax = 1f;

	// Token: 0x04000FDB RID: 4059
	public AnimationCurve fadeCurve;

	// Token: 0x04000FDC RID: 4060
	public AnimationCurve fadeCullCurve;

	// Token: 0x04000FDD RID: 4061
	public static LightManager instance;

	// Token: 0x04000FDE RID: 4062
	internal List<PropLight> propLights = new List<PropLight>();

	// Token: 0x04000FDF RID: 4063
	private List<PropLightEmission> propEmissions = new List<PropLightEmission>();

	// Token: 0x04000FE0 RID: 4064
	private Vector3 lastCheckPos;

	// Token: 0x04000FE1 RID: 4065
	private float lastYRotation;

	// Token: 0x04000FE2 RID: 4066
	internal int activeLightsAmount;

	// Token: 0x04000FE3 RID: 4067
	internal bool updateInstant;

	// Token: 0x04000FE4 RID: 4068
	internal float updateInstantTimer;

	// Token: 0x04000FE5 RID: 4069
	[Space]
	[Header("Sounds")]
	public Sound lampFlickerSound;

	// Token: 0x04000FE6 RID: 4070
	private bool turnOffLights;

	// Token: 0x04000FE7 RID: 4071
	private bool turningOffLights;

	// Token: 0x04000FE8 RID: 4072
	private bool turningOffEmissions;

	// Token: 0x04000FE9 RID: 4073
	private float logicUpdateTimer;

	// Token: 0x04000FEA RID: 4074
	private bool setup;

	// Token: 0x04000FEB RID: 4075
	private bool debugActive;
}
