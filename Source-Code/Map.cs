using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class Map : MonoBehaviour
{
	// Token: 0x06000CD1 RID: 3281 RVA: 0x00070365 File Offset: 0x0006E565
	private void Awake()
	{
		Map.Instance = this;
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0007036D File Offset: 0x0006E56D
	private void Start()
	{
		this.playerTransformSource = PlayerController.instance.transform;
		this.ActiveSet(false);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x00070388 File Offset: 0x0006E588
	private void Update()
	{
		if (this.Active != this.ActivePrevious)
		{
			if (!this.Active)
			{
				foreach (MapLayer mapLayer in this.Layers)
				{
					mapLayer.transform.position = mapLayer.positionStart;
				}
			}
			this.ActivePrevious = this.Active;
		}
		if (this.Active)
		{
			foreach (MapLayer mapLayer2 in this.Layers)
			{
				if (mapLayer2.layer == this.PlayerLayer)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, 0f, mapLayer2.transform.localPosition.z);
				}
				else if (mapLayer2.layer == this.PlayerLayer - 1)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, this.GetLayerPosition(2).y, mapLayer2.transform.localPosition.z);
				}
				else if (mapLayer2.layer == this.PlayerLayer + 1)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, this.GetLayerPosition(3).y, mapLayer2.transform.localPosition.z);
				}
				else
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, -5f, mapLayer2.transform.localPosition.z);
				}
			}
		}
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00070588 File Offset: 0x0006E788
	public void ActiveSet(bool active)
	{
		this.Active = active;
		if (this.ActiveParent != null)
		{
			this.ActiveParent.SetActive(active);
		}
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000705AB File Offset: 0x0006E7AB
	public void EnemyPositionSet(Transform transformTarget, Transform transformSource)
	{
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x000705AD File Offset: 0x0006E7AD
	public void AddEnemy(Enemy enemy)
	{
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x000705B0 File Offset: 0x0006E7B0
	public void CustomPositionSet(Transform transformTarget, Transform transformSource)
	{
		transformTarget.position = transformSource.transform.position * this.Scale + this.OverLayerParent.position;
		transformTarget.localPosition = new Vector3(transformTarget.localPosition.x, 0f, transformTarget.localPosition.z);
		transformTarget.localRotation = Quaternion.Euler(0f, transformSource.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00070638 File Offset: 0x0006E838
	public void AddCustom(MapCustom mapCustom, Sprite sprite, Color color)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.CustomObject, this.OverLayerParent.transform);
		gameObject.gameObject.name = mapCustom.gameObject.name;
		this.CustomPositionSet(gameObject.transform, mapCustom.transform);
		MapCustomEntity component = gameObject.GetComponent<MapCustomEntity>();
		component.Parent = mapCustom.transform;
		component.mapCustom = mapCustom;
		component.spriteRenderer.sprite = sprite;
		component.spriteRenderer.color = color;
		component.StartCoroutine(component.Logic());
		mapCustom.mapCustomEntity = component;
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x000706CC File Offset: 0x0006E8CC
	public void AddFloor(DirtFinderMapFloor floor)
	{
		GameObject gameObject = null;
		MapLayer layerParent = this.GetLayerParent(floor.transform.position.y);
		if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05Curve, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve_Inverted)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05CurveInverted, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x025, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x025Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorTruck, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallTruck, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorUsed, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallUsed, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorInactive, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallInactive, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1Curve, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve_Inverted)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1CurveInverted, layerParent.transform);
		}
		gameObject.gameObject.name = floor.gameObject.name;
		gameObject.transform.localScale = floor.transform.localScale;
		gameObject.transform.position = floor.transform.position * this.Scale + layerParent.transform.position + this.GetLayerPosition(layerParent.layer);
		gameObject.transform.rotation = floor.transform.rotation;
		this.MapObjectSetup(floor.gameObject, gameObject);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0007097C File Offset: 0x0006EB7C
	public void AddWall(DirtFinderMapWall wall)
	{
		MapLayer layerParent = this.GetLayerParent(wall.transform.position.y);
		GameObject gameObject;
		if (wall.Type == DirtFinderMapWall.WallType.Door_1x1)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x2)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x2Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Wizard)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedWizardObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Arctic)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedArcticObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Stairs)
		{
			gameObject = Object.Instantiate<GameObject>(this.StairsObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x05Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x05DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x025)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x025Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x025_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x025DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Wizard)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1WizardObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Arctic)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1ArcticObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1CurveObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05CurveObject, layerParent.transform);
		}
		else
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1Object, layerParent.transform);
		}
		gameObject.gameObject.name = wall.gameObject.name;
		gameObject.transform.position = wall.transform.position * this.Scale + layerParent.transform.position + this.GetLayerPosition(layerParent.layer);
		gameObject.transform.rotation = wall.transform.rotation;
		gameObject.transform.localScale = wall.transform.localScale;
		this.MapObjectSetup(wall.gameObject, gameObject);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00070C84 File Offset: 0x0006EE84
	public MapModule AddRoomVolume(GameObject _parent, Vector3 _position, Quaternion _rotation, Vector3 _scale, Module _module)
	{
		MapLayer component = this.OverLayerParent.GetComponent<MapLayer>();
		GameObject gameObject = Object.Instantiate<GameObject>(this.RoomVolume, component.transform);
		gameObject.gameObject.name = "Room Volume";
		gameObject.transform.position = _position * this.Scale + component.transform.position + this.GetLayerPosition(component.layer);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0f, gameObject.transform.localPosition.z);
		gameObject.transform.rotation = _rotation;
		gameObject.transform.localScale = _scale;
		gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 0.1f, gameObject.transform.localScale.z);
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.RoomVolumeOutline, component.transform);
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		gameObject2.transform.localScale = new Vector3(gameObject.transform.localScale.x + 0.25f, gameObject.transform.localScale.y, gameObject.transform.localScale.z + 0.25f);
		foreach (MapModule mapModule in this.MapModules)
		{
			if (mapModule.module == _module)
			{
				gameObject.transform.SetParent(mapModule.transform);
				gameObject2.transform.SetParent(mapModule.transform);
				return mapModule;
			}
		}
		GameObject gameObject3 = Object.Instantiate<GameObject>(this.ModulePrefab, component.transform);
		MapModule component2 = gameObject3.GetComponent<MapModule>();
		component2.module = _module;
		gameObject3.gameObject.name = _module.gameObject.name;
		gameObject3.transform.position = _module.transform.position * this.Scale + component.transform.position + this.GetLayerPosition(component.layer);
		this.MapModules.Add(component2);
		gameObject.transform.SetParent(gameObject3.transform);
		gameObject2.transform.SetParent(gameObject3.transform);
		return component2;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00070F34 File Offset: 0x0006F134
	public void AddValuable(ValuableObject _valuable)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.ValuableObject, this.OverLayerParent.transform);
		gameObject.gameObject.name = _valuable.gameObject.name;
		gameObject.transform.position = _valuable.transform.position * this.Scale + this.OverLayerParent.position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0f, gameObject.transform.localPosition.z);
		MapValuable component = gameObject.GetComponent<MapValuable>();
		component.target = _valuable;
		if (_valuable.volumeType <= ValuableVolume.Type.Medium)
		{
			component.spriteRenderer.sprite = component.spriteSmall;
			return;
		}
		component.spriteRenderer.sprite = component.spriteBig;
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00071010 File Offset: 0x0006F210
	public GameObject AddDoor(DirtFinderMapDoor door, GameObject doorPrefab)
	{
		MapLayer layerParent = this.GetLayerParent(door.transform.position.y);
		GameObject gameObject = Object.Instantiate<GameObject>(doorPrefab, layerParent.transform);
		gameObject.gameObject.name = door.gameObject.name;
		door.Target = gameObject.transform;
		DirtFinderMapDoorTarget component = gameObject.GetComponent<DirtFinderMapDoorTarget>();
		component.Target = door.transform;
		component.Layer = layerParent;
		this.DoorUpdate(component.HingeTransform, door.transform, layerParent);
		return gameObject;
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00071094 File Offset: 0x0006F294
	public void DoorUpdate(Transform transformTarget, Transform transformSource, MapLayer _layer)
	{
		transformTarget.position = transformSource.transform.position * this.Scale + _layer.transform.position + this.GetLayerPosition(_layer.layer);
		transformTarget.rotation = transformSource.rotation;
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x000710EC File Offset: 0x0006F2EC
	public MapLayer GetLayerParent(float _positionY)
	{
		int num = Mathf.FloorToInt((_positionY + 0.1f) / this.LayerHeight);
		foreach (MapLayer mapLayer in this.Layers)
		{
			if (mapLayer.layer == num)
			{
				return mapLayer;
			}
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.LayerPrefab, base.transform);
		MapLayer component = gameObject.GetComponent<MapLayer>();
		component.layer = num;
		this.Layers.Add(component);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, this.LayerHeight * this.Scale * (float)num, gameObject.transform.localPosition.z);
		gameObject.name = "Layer " + num.ToString();
		this.Layers = (from x in this.Layers
		orderby x.layer
		select x).ToList<MapLayer>();
		this.Layers.Reverse();
		this.OverLayerParent.SetSiblingIndex(0);
		int num2 = 1;
		foreach (MapLayer mapLayer2 in this.Layers)
		{
			mapLayer2.transform.SetSiblingIndex(num2);
			num2++;
		}
		return component;
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00071280 File Offset: 0x0006F480
	public Vector3 GetLayerPosition(int _layerIndex)
	{
		return new Vector3(0f, -(this.LayerHeight * this.Scale) * (float)_layerIndex, 0f);
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000712A4 File Offset: 0x0006F4A4
	private MapObject MapObjectSetup(GameObject _parent, GameObject _object)
	{
		MapObject component = _object.GetComponent<MapObject>();
		if (!component)
		{
			Debug.LogError("Map Object missing component!", _object);
		}
		else
		{
			component.parent = _parent.transform;
			DirtFinderMapFloor component2 = _parent.GetComponent<DirtFinderMapFloor>();
			if (component2)
			{
				component2.MapObject = component;
			}
		}
		return component;
	}

	// Token: 0x04001469 RID: 5225
	public static Map Instance;

	// Token: 0x0400146A RID: 5226
	public bool Active;

	// Token: 0x0400146B RID: 5227
	public bool ActivePrevious;

	// Token: 0x0400146C RID: 5228
	public GameObject ActiveParent;

	// Token: 0x0400146D RID: 5229
	public int PlayerLayer;

	// Token: 0x0400146E RID: 5230
	[Space]
	public GameObject LayerPrefab;

	// Token: 0x0400146F RID: 5231
	public GameObject ModulePrefab;

	// Token: 0x04001470 RID: 5232
	public Transform OverLayerParent;

	// Token: 0x04001471 RID: 5233
	[Space]
	public List<MapLayer> Layers = new List<MapLayer>();

	// Token: 0x04001472 RID: 5234
	public List<MapModule> MapModules = new List<MapModule>();

	// Token: 0x04001473 RID: 5235
	[Space]
	public GameObject EnemyObject;

	// Token: 0x04001474 RID: 5236
	public GameObject CustomObject;

	// Token: 0x04001475 RID: 5237
	public GameObject ValuableObject;

	// Token: 0x04001476 RID: 5238
	[Space]
	public GameObject FloorObject1x1;

	// Token: 0x04001477 RID: 5239
	public GameObject FloorObject1x1Diagonal;

	// Token: 0x04001478 RID: 5240
	public GameObject FloorObject1x1Curve;

	// Token: 0x04001479 RID: 5241
	public GameObject FloorObject1x1CurveInverted;

	// Token: 0x0400147A RID: 5242
	[Space]
	public GameObject FloorObject1x05;

	// Token: 0x0400147B RID: 5243
	public GameObject FloorObject1x05Diagonal;

	// Token: 0x0400147C RID: 5244
	public GameObject FloorObject1x05Curve;

	// Token: 0x0400147D RID: 5245
	public GameObject FloorObject1x05CurveInverted;

	// Token: 0x0400147E RID: 5246
	[Space]
	public GameObject FloorObject1x025;

	// Token: 0x0400147F RID: 5247
	public GameObject FloorObject1x025Diagonal;

	// Token: 0x04001480 RID: 5248
	[Space]
	public GameObject RoomVolume;

	// Token: 0x04001481 RID: 5249
	public GameObject RoomVolumeOutline;

	// Token: 0x04001482 RID: 5250
	[Space]
	public GameObject FloorTruck;

	// Token: 0x04001483 RID: 5251
	public GameObject WallTruck;

	// Token: 0x04001484 RID: 5252
	[Space]
	public GameObject FloorUsed;

	// Token: 0x04001485 RID: 5253
	public GameObject WallUsed;

	// Token: 0x04001486 RID: 5254
	[Space]
	public GameObject FloorInactive;

	// Token: 0x04001487 RID: 5255
	public GameObject WallInactive;

	// Token: 0x04001488 RID: 5256
	[Space]
	public GameObject Wall1x1Object;

	// Token: 0x04001489 RID: 5257
	public GameObject Wall1x1DiagonalObject;

	// Token: 0x0400148A RID: 5258
	public GameObject Wall1x1CurveObject;

	// Token: 0x0400148B RID: 5259
	[Space]
	public GameObject Wall1x05Object;

	// Token: 0x0400148C RID: 5260
	public GameObject Wall1x05DiagonalObject;

	// Token: 0x0400148D RID: 5261
	public GameObject Wall1x05CurveObject;

	// Token: 0x0400148E RID: 5262
	[Space]
	public GameObject Wall1x025Object;

	// Token: 0x0400148F RID: 5263
	public GameObject Wall1x025DiagonalObject;

	// Token: 0x04001490 RID: 5264
	[Space]
	public GameObject Door1x1Object;

	// Token: 0x04001491 RID: 5265
	public GameObject Door1x05Object;

	// Token: 0x04001492 RID: 5266
	public GameObject Door1x1DiagonalObject;

	// Token: 0x04001493 RID: 5267
	public GameObject Door1x05DiagonalObject;

	// Token: 0x04001494 RID: 5268
	public GameObject Door1x2Object;

	// Token: 0x04001495 RID: 5269
	public GameObject Door1x1WizardObject;

	// Token: 0x04001496 RID: 5270
	public GameObject Door1x1ArcticObject;

	// Token: 0x04001497 RID: 5271
	[Space]
	public GameObject DoorBlockedObject;

	// Token: 0x04001498 RID: 5272
	public GameObject DoorBlockedWizardObject;

	// Token: 0x04001499 RID: 5273
	public GameObject DoorBlockedArcticObject;

	// Token: 0x0400149A RID: 5274
	public GameObject DoorDiagonalObject;

	// Token: 0x0400149B RID: 5275
	public GameObject StairsObject;

	// Token: 0x0400149C RID: 5276
	[Space]
	public float Scale = 0.1f;

	// Token: 0x0400149D RID: 5277
	private float LayerHeight = 4f;

	// Token: 0x0400149E RID: 5278
	[Space]
	public Transform playerTransformSource;

	// Token: 0x0400149F RID: 5279
	public Transform playerTransformTarget;

	// Token: 0x040014A0 RID: 5280
	[Space]
	public Transform CompletedTransform;

	// Token: 0x040014A1 RID: 5281
	internal bool debugActive;
}
