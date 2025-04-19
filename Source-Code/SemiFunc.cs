using System;
using System.Collections.Generic;
using System.Globalization;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200012E RID: 302
public static class SemiFunc
{
	// Token: 0x06000989 RID: 2441 RVA: 0x000584AC File Offset: 0x000566AC
	public static void EnemyCartJumpReset(Enemy enemy)
	{
		if (enemy.HasJump)
		{
			enemy.Jump.CartJump(0f);
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x000584C6 File Offset: 0x000566C6
	public static void EnemyCartJump(Enemy enemy)
	{
		if (enemy.HasJump)
		{
			enemy.Jump.CartJump(0.1f);
		}
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x000584E0 File Offset: 0x000566E0
	public static Vector3 EnemyGetNearestPhysObject(Enemy enemy)
	{
		PhysGrabObject physGrabObject = null;
		float num = 9999f;
		Collider[] array = Physics.OverlapSphere(enemy.CenterTransform.position, 3f, LayerMask.GetMask(new string[]
		{
			"PhysGrabObject"
		}));
		for (int i = 0; i < array.Length; i++)
		{
			PhysGrabObject componentInParent = array[i].GetComponentInParent<PhysGrabObject>();
			if (componentInParent && !componentInParent.GetComponent<EnemyRigidbody>())
			{
				float num2 = Vector3.Distance(enemy.CenterTransform.position, componentInParent.centerPoint);
				if (num2 < num)
				{
					num = num2;
					physGrabObject = componentInParent;
				}
			}
		}
		if (physGrabObject)
		{
			return physGrabObject.centerPoint;
		}
		return Vector3.zero;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00058588 File Offset: 0x00056788
	public static bool EnemySpawn(Enemy enemy)
	{
		float minDistance = 18f;
		float maxDistance = 35f;
		if (EnemyDirector.instance.debugSpawnClose)
		{
			minDistance = 0f;
			maxDistance = 999f;
		}
		LevelPoint levelPoint = enemy.TeleportToPoint(minDistance, maxDistance);
		if (levelPoint)
		{
			bool flag;
			if (enemy.HasRigidbody)
			{
				flag = !SemiFunc.EnemyPhysObjectBoundingBoxCheck(enemy.transform.position, levelPoint.transform.position, enemy.Rigidbody.rb);
			}
			else
			{
				flag = !SemiFunc.EnemyPhysObjectSphereCheck(levelPoint.transform.position, 1f);
			}
			enemy.EnemyParent.firstSpawnPointUsed = true;
			if (flag)
			{
				return true;
			}
		}
		enemy.EnemyParent.Despawn();
		enemy.EnemyParent.DespawnedTimerSet(Random.Range(2f, 3f), true);
		return false;
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0005864F File Offset: 0x0005684F
	public static Camera MainCamera()
	{
		return GameDirector.instance.MainCamera;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0005865B File Offset: 0x0005685B
	public static bool EnemySpawnIdlePause()
	{
		return EnemyDirector.instance.spawnIdlePauseTimer > 0f;
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00058671 File Offset: 0x00056871
	public static bool EnemyForceLeave(Enemy enemy)
	{
		if (enemy.EnemyParent.forceLeave)
		{
			enemy.EnemyParent.forceLeave = false;
			return true;
		}
		return false;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00058690 File Offset: 0x00056890
	public static bool OnGroundCheck(Vector3 _position, float _distance, PhysGrabObject _notMe = null)
	{
		foreach (RaycastHit raycastHit in Physics.RaycastAll(_position, Vector3.down, _distance, LayerMask.GetMask(new string[]
		{
			"Default",
			"PhysGrabObject",
			"PhysGrabObjectCart",
			"PhysGrabObjectHinge",
			"Enemy",
			"Player"
		})))
		{
			PhysGrabObject componentInParent = raycastHit.collider.GetComponentInParent<PhysGrabObject>();
			if (!componentInParent || componentInParent != _notMe)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0005871C File Offset: 0x0005691C
	public static PlayerAvatar PlayerGetFromSteamID(string _steamID)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.steamID == _steamID)
			{
				return playerAvatar;
			}
		}
		return null;
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00058784 File Offset: 0x00056984
	public static Transform PlayerGetFaceEyeTransform(PlayerAvatar _player)
	{
		if (!_player.isLocal)
		{
			return _player.playerAvatarVisuals.headLookAtTransform;
		}
		return _player.localCameraTransform;
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x000587A0 File Offset: 0x000569A0
	public static PlayerAvatar PlayerGetFromName(string _name)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.playerName == _name)
			{
				return playerAvatar;
			}
		}
		return null;
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x00058808 File Offset: 0x00056A08
	public static Color PlayerGetColorFromSteamID(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerGetFromSteamID(_steamID);
		if (playerAvatar)
		{
			return playerAvatar.playerAvatarVisuals.color;
		}
		return Color.black;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00058838 File Offset: 0x00056A38
	public static void ItemAffectEnemyBatteryDrain(EnemyParent _enemyParent, ItemBattery _itemBattery, float tumbleEnemyTimer, float _deltaTime, float _multiplier = 1f)
	{
		if (_enemyParent && _itemBattery)
		{
			Rigidbody componentInChildren = _enemyParent.GetComponentInChildren<Rigidbody>();
			if (!componentInChildren)
			{
				return;
			}
			Enemy componentInChildren2 = _enemyParent.GetComponentInChildren<Enemy>();
			if (!componentInChildren2)
			{
				return;
			}
			int difficulty = (int)_enemyParent.difficulty;
			if (difficulty == 0)
			{
				float num = componentInChildren.mass * 0.5f;
				num = Mathf.Clamp(num, 3f, 4f) * _multiplier;
				_itemBattery.batteryLife -= num * _deltaTime;
				if (tumbleEnemyTimer > 1.5f && componentInChildren2.HasStateStunned)
				{
					componentInChildren2.StateStunned.Set(1f);
				}
				return;
			}
			if (difficulty == 1)
			{
				float num2 = componentInChildren.mass * 0.85f;
				num2 = Mathf.Clamp(num2, 5f, 6f) * _multiplier;
				_itemBattery.batteryLife -= num2 * _deltaTime;
				if (tumbleEnemyTimer > 3f && componentInChildren2.HasStateStunned)
				{
					componentInChildren2.StateStunned.Set(1f);
				}
				return;
			}
			if (difficulty == 2)
			{
				float num3 = componentInChildren.mass * 1f;
				num3 = Mathf.Clamp(num3, 7f, 8f) * _multiplier;
				_itemBattery.batteryLife -= num3 * _deltaTime;
				if (tumbleEnemyTimer > 4f && componentInChildren2.HasStateStunned)
				{
					componentInChildren2.StateStunned.Set(1f);
				}
			}
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0005898A File Offset: 0x00056B8A
	public static void EnemyInvestigate(Vector3 position, float range)
	{
		EnemyDirector.instance.SetInvestigate(position, range);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00058998 File Offset: 0x00056B98
	public static int EnemyGetIndex(Enemy _enemy)
	{
		int result = -1;
		if (_enemy)
		{
			foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
			{
				if (enemyParent.Enemy == _enemy)
				{
					result = EnemyDirector.instance.enemiesSpawned.IndexOf(enemyParent);
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00058A14 File Offset: 0x00056C14
	public static Enemy EnemyGetFromIndex(int _enemyIndex)
	{
		Enemy result = null;
		if (_enemyIndex == -1)
		{
			return result;
		}
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			if (EnemyDirector.instance.enemiesSpawned.IndexOf(enemyParent) == _enemyIndex)
			{
				result = enemyParent.Enemy;
				break;
			}
		}
		return result;
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00058A8C File Offset: 0x00056C8C
	public static Enemy EnemyGetNearest(Vector3 _position, float _maxDistance, bool _raycast)
	{
		Enemy result = null;
		float num = _maxDistance;
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			if (enemyParent.DespawnedTimer <= 0f)
			{
				Vector3 direction = enemyParent.Enemy.CenterTransform.position - _position;
				if (direction.magnitude < num)
				{
					if (_raycast)
					{
						bool flag = false;
						foreach (RaycastHit raycastHit in Physics.RaycastAll(_position, direction, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct()))
						{
							if (raycastHit.collider.gameObject.CompareTag("Wall"))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					num = direction.magnitude;
					result = enemyParent.Enemy;
				}
			}
		}
		return result;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00058B88 File Offset: 0x00056D88
	public static bool EnemyPhysObjectSphereCheck(Vector3 _position, float _radius)
	{
		return Physics.OverlapSphere(_position, _radius, SemiFunc.LayerMaskGetPhysGrabObject()).Length != 0;
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00058BA4 File Offset: 0x00056DA4
	public static bool EnemyPhysObjectBoundingBoxCheck(Vector3 _currentPosition, Vector3 _checkPosition, Rigidbody _rigidbody)
	{
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		foreach (Collider collider in _rigidbody.GetComponentsInChildren<Collider>())
		{
			if (bounds.size == Vector3.zero)
			{
				bounds = collider.bounds;
			}
			else
			{
				bounds.Encapsulate(collider.bounds);
			}
		}
		Vector3 b = _currentPosition - _rigidbody.transform.position;
		Vector3 b2 = bounds.center - _rigidbody.transform.position;
		bounds.center = _checkPosition - b + b2;
		bounds.size *= 1.2f;
		Collider[] array = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, SemiFunc.LayerMaskGetPhysGrabObject());
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponentInParent<Rigidbody>() != _rigidbody)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00058CAC File Offset: 0x00056EAC
	public static void DebugDrawBounds(Bounds _bounds, Color _color, float _time)
	{
		Vector3 vector = new Vector3(_bounds.min.x, _bounds.min.y, _bounds.min.z);
		Vector3 vector2 = new Vector3(_bounds.max.x, _bounds.min.y, _bounds.min.z);
		Vector3 vector3 = new Vector3(_bounds.max.x, _bounds.min.y, _bounds.max.z);
		Vector3 vector4 = new Vector3(_bounds.min.x, _bounds.min.y, _bounds.max.z);
		Debug.DrawLine(vector, vector2, _color, _time);
		Debug.DrawLine(vector2, vector3, _color, _time);
		Debug.DrawLine(vector3, vector4, _color, _time);
		Debug.DrawLine(vector4, vector, _color, _time);
		Vector3 vector5 = new Vector3(_bounds.min.x, _bounds.max.y, _bounds.min.z);
		Vector3 vector6 = new Vector3(_bounds.max.x, _bounds.max.y, _bounds.min.z);
		Vector3 vector7 = new Vector3(_bounds.max.x, _bounds.max.y, _bounds.max.z);
		Vector3 vector8 = new Vector3(_bounds.min.x, _bounds.max.y, _bounds.max.z);
		Debug.DrawLine(vector5, vector6, _color, _time);
		Debug.DrawLine(vector6, vector7, _color, _time);
		Debug.DrawLine(vector7, vector8, _color, _time);
		Debug.DrawLine(vector8, vector5, _color, _time);
		Debug.DrawLine(vector, vector5, _color, _time);
		Debug.DrawLine(vector2, vector6, _color, _time);
		Debug.DrawLine(vector3, vector7, _color, _time);
		Debug.DrawLine(vector4, vector8, _color, _time);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x00058E89 File Offset: 0x00057089
	public static bool DebugUser(SemiFunc.User _user)
	{
		return SemiFunc.DebugDev() && DebugComputerCheck.instance && DebugComputerCheck.instance.DebugUser == _user;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x00058EAE File Offset: 0x000570AE
	public static bool Axel()
	{
		return SemiFunc.DebugUser(SemiFunc.User.Axel);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00058EB6 File Offset: 0x000570B6
	public static bool Jannek()
	{
		return SemiFunc.DebugUser(SemiFunc.User.Jannek);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00058EBE File Offset: 0x000570BE
	public static bool Robin()
	{
		return SemiFunc.DebugUser(SemiFunc.User.Robin);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00058EC6 File Offset: 0x000570C6
	public static bool Ruben()
	{
		return SemiFunc.DebugUser(SemiFunc.User.Ruben);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00058ECE File Offset: 0x000570CE
	public static bool Walter()
	{
		return SemiFunc.DebugUser(SemiFunc.User.Walter);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00058ED6 File Offset: 0x000570D6
	public static bool DebugKeyDown(SemiFunc.User _user, KeyCode _input)
	{
		return SemiFunc.DebugUser(_user) && Input.GetKeyUp(_input);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00058EE8 File Offset: 0x000570E8
	public static bool KeyDownAxel(KeyCode _input)
	{
		return SemiFunc.DebugKeyDown(SemiFunc.User.Axel, _input);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00058EF1 File Offset: 0x000570F1
	public static bool KeyDownJannek(KeyCode _input)
	{
		return SemiFunc.DebugKeyDown(SemiFunc.User.Jannek, _input);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00058EFA File Offset: 0x000570FA
	public static bool KeyDownRobin(KeyCode _input)
	{
		return SemiFunc.DebugKeyDown(SemiFunc.User.Robin, _input);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00058F03 File Offset: 0x00057103
	public static bool KeyDownRuben(KeyCode _input)
	{
		return SemiFunc.DebugKeyDown(SemiFunc.User.Ruben, _input);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00058F0C File Offset: 0x0005710C
	public static bool KeyDownWalter(KeyCode _input)
	{
		return SemiFunc.DebugKeyDown(SemiFunc.User.Walter, _input);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00058F15 File Offset: 0x00057115
	public static bool DebugKey(SemiFunc.User _user, KeyCode _input)
	{
		return SemiFunc.DebugUser(_user) && Input.GetKey(_input);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00058F27 File Offset: 0x00057127
	public static bool KeyAxel(KeyCode _input)
	{
		return SemiFunc.DebugKey(SemiFunc.User.Axel, _input);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00058F30 File Offset: 0x00057130
	public static bool KeyJannek(KeyCode _input)
	{
		return SemiFunc.DebugKey(SemiFunc.User.Jannek, _input);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00058F39 File Offset: 0x00057139
	public static bool KeyRobin(KeyCode _input)
	{
		return SemiFunc.DebugKey(SemiFunc.User.Robin, _input);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00058F42 File Offset: 0x00057142
	public static bool KeyRuben(KeyCode _input)
	{
		return SemiFunc.DebugKey(SemiFunc.User.Ruben, _input);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00058F4B File Offset: 0x0005714B
	public static bool KeyWalter(KeyCode _input)
	{
		return SemiFunc.DebugKey(SemiFunc.User.Walter, _input);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00058F54 File Offset: 0x00057154
	public static bool DebugDev()
	{
		return SteamManager.instance.developerMode;
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00058F60 File Offset: 0x00057160
	public static float UIMulti()
	{
		return HUDCanvas.instance.rect.sizeDelta.y;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00058F78 File Offset: 0x00057178
	public static string MessageGeneratedGetLeftBehind()
	{
		List<string> list = new List<string>
		{
			"You",
			"They",
			"My team",
			"Everyone",
			"My friends",
			"The squad",
			"The group",
			"All of them",
			"Those I trusted",
			"My companions"
		};
		List<string> list2 = new List<string>
		{
			"left",
			"abandoned",
			"betrayed",
			"forgot",
			"doomed",
			"deserted",
			"ditched",
			"dissed",
			"discarded",
			"forgot"
		};
		List<string> list3 = new List<string>
		{
			"me",
			"lil old me",
			"this lil robot",
			"my life",
			"my only hope",
			"my chance",
			"this poor robot",
			"my feelings",
			"our friendship",
			"my heart",
			"my trust"
		};
		List<string> list4 = new List<string>
		{
			"behind",
			"alone",
			"in the dark",
			"without a word",
			"without warning",
			"in silence",
			"without looking back",
			"with no remorse"
		};
		List<string> list5 = new List<string>
		{
			"I feel lost.",
			"Why didn't they wait?",
			"What did I do wrong?",
			"I can't believe it.",
			"How could they?",
			"This can't be happening.",
			"I'm on my own now.",
			"They were my only hope.",
			"I should have known.",
			"It's so unfair."
		};
		List<string> list6 = new List<string>
		{
			"{subject} {verb} {object}.",
			"{additional_phrase} {subject} {verb} {object}...",
			"{subject} {verb} lil me {adverb}.",
			"{additional_phrase}",
			"They {verb} me {adverb}...",
			"Now, {subject} {verb} {object}.",
			"In the end, {subject} {verb} {object}.",
			"I can't believe {subject} {verb} {object}...",
			"{subject} {verb} {object}. {additional_phrase}",
			"{additional_phrase} {subject} {verb} {object}."
		};
		int index = Random.Range(0, list6.Count);
		return list6[index].Replace("{subject}", list[Random.Range(0, list.Count)]).Replace("{verb}", list2[Random.Range(0, list2.Count)]).Replace("{object}", list3[Random.Range(0, list3.Count)]).Replace("{adverb}", list4[Random.Range(0, list4.Count)]).Replace("{additional_phrase}", list5[Random.Range(0, list5.Count)]);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x000592DA File Offset: 0x000574DA
	public static bool MainMenuIsSingleplayer()
	{
		return MainMenuOpen.instance.MainMenuGetState() == MainMenuOpen.MainMenuGameModeState.SinglePlayer;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000592EC File Offset: 0x000574EC
	public static void MenuActionSingleplayerGame(string saveFileName = null)
	{
		RunManager.instance.ResetProgress();
		if (saveFileName != null)
		{
			Debug.Log("Loading save");
			SemiFunc.SaveFileLoad(saveFileName);
		}
		else
		{
			SemiFunc.SaveFileCreate();
		}
		DataDirector.instance.RunsPlayedAdd();
		if (RunManager.instance.loadLevel == 0)
		{
			RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
			return;
		}
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00059350 File Offset: 0x00057550
	public static void MenuActionHostGame(string saveFileName = null)
	{
		RunManager.instance.ResetProgress();
		if (saveFileName != null)
		{
			SemiFunc.SaveFileLoad(saveFileName);
		}
		else
		{
			SemiFunc.SaveFileCreate();
		}
		GameManager.instance.localTest = false;
		RunManager.instance.waitToChangeScene = true;
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
		MainMenuOpen.instance.NetworkConnect();
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x000593A4 File Offset: 0x000575A4
	public static void SaveFileLoad(string saveFileName)
	{
		StatsManager.instance.LoadGame(saveFileName);
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x000593B1 File Offset: 0x000575B1
	public static void SaveFileDelete(string saveFileName)
	{
		if (string.IsNullOrEmpty(saveFileName))
		{
			return;
		}
		StatsManager.instance.SaveFileDelete(saveFileName);
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x000593C7 File Offset: 0x000575C7
	public static List<string> SaveFileGetAll()
	{
		return StatsManager.instance.SaveFileGetAll();
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x000593D3 File Offset: 0x000575D3
	public static void SaveFileCreate()
	{
		StatsManager.instance.SaveFileCreate();
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000593DF File Offset: 0x000575DF
	public static void SaveFileSave()
	{
		StatsManager.instance.SaveFileSave();
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x000593EB File Offset: 0x000575EB
	public static bool MainMenuIsMultiplayer()
	{
		return MainMenuOpen.instance.MainMenuGetState() == MainMenuOpen.MainMenuGameModeState.MultiPlayer;
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x000593FD File Offset: 0x000575FD
	public static void MainMenuSetSingleplayer()
	{
		MainMenuOpen.instance.MainMenuSetState(0);
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0005940A File Offset: 0x0005760A
	public static void MainMenuSetMultiplayer()
	{
		MainMenuOpen.instance.MainMenuSetState(1);
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00059418 File Offset: 0x00057618
	public static List<PlayerAvatar> PlayerGetAllPlayerAvatarWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled)
			{
				Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				float num = Vector3.Distance(position, position2);
				if (num <= range)
				{
					Vector3 direction = position2 - position;
					bool flag = false;
					if (doRaycastCheck)
					{
						foreach (RaycastHit raycastHit in Physics.RaycastAll(position, direction, num, layerMask))
						{
							if (raycastHit.collider.transform.CompareTag("Wall"))
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						list.Add(playerAvatar);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00059504 File Offset: 0x00057704
	public static List<PlayerAvatar> PlayerGetAllPlayerAvatarWithinRangeAndVision(float range, Vector3 position, PhysGrabObject _thisPhysGrabObject = null)
	{
		LayerMask mask = SemiFunc.LayerMaskGetVisionObstruct();
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled)
			{
				Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				float num = Vector3.Distance(position, position2);
				if (num <= range)
				{
					Vector3 direction = position2 - position;
					bool flag = false;
					foreach (RaycastHit raycastHit in Physics.RaycastAll(position, direction, num, mask))
					{
						if (!(raycastHit.transform.GetComponentInParent<PhysGrabObject>() == _thisPhysGrabObject))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						list.Add(playerAvatar);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x000595F4 File Offset: 0x000577F4
	public static PlayerAvatar PlayerGetNearestPlayerAvatarWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		float num = range;
		PlayerAvatar result = null;
		List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(range, position, doRaycastCheck, layerMask);
		if (list.Count > 0)
		{
			foreach (PlayerAvatar playerAvatar in list)
			{
				Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = playerAvatar;
				}
			}
		}
		return result;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0005967C File Offset: 0x0005787C
	public static float PlayerNearestDistance(Vector3 position)
	{
		float num = 999f;
		float result = 9f;
		List<PlayerAvatar> playerList = GameDirector.instance.PlayerList;
		if (playerList.Count > 0)
		{
			foreach (PlayerAvatar playerAvatar in playerList)
			{
				Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = num;
				}
			}
		}
		return result;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00059708 File Offset: 0x00057908
	public static bool PlayerVisionCheck(Vector3 _position, float _range, PlayerAvatar _player, bool _previouslySeen)
	{
		return SemiFunc.PlayerVisionCheckPosition(_position, _player.PlayerVisionTarget.VisionTransform.position, _range, _player, _previouslySeen);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00059724 File Offset: 0x00057924
	public static bool PlayerVisionCheckPosition(Vector3 _startPosition, Vector3 _endPosition, float _range, PlayerAvatar _player, bool _previouslySeen)
	{
		if (_player.enemyVisionFreezeTimer > 0f)
		{
			return _previouslySeen;
		}
		LayerMask mask = SemiFunc.LayerMaskGetVisionObstruct();
		Vector3 vector = _endPosition - _startPosition;
		if (vector.magnitude > _range)
		{
			return false;
		}
		if (vector.magnitude < _range)
		{
			_range = vector.magnitude;
		}
		RaycastHit[] array = Physics.RaycastAll(_startPosition, vector, _range, mask);
		PlayerAvatar exists = null;
		Transform x = null;
		Transform y = null;
		Vector3 a = Vector3.zero;
		float num = 1000f;
		foreach (RaycastHit raycastHit in array)
		{
			float num2 = Vector3.Distance(_startPosition, raycastHit.point);
			if (num2 < num)
			{
				num = num2;
				y = raycastHit.transform;
				a = raycastHit.point;
				PlayerAvatar playerAvatar = null;
				if (raycastHit.transform.CompareTag("Player"))
				{
					playerAvatar = raycastHit.transform.GetComponentInParent<PlayerAvatar>();
					if (!playerAvatar)
					{
						PlayerController componentInParent = raycastHit.transform.GetComponentInParent<PlayerController>();
						if (componentInParent)
						{
							playerAvatar = componentInParent.playerAvatarScript;
						}
					}
				}
				else
				{
					PlayerTumble componentInParent2 = raycastHit.transform.GetComponentInParent<PlayerTumble>();
					if (componentInParent2)
					{
						playerAvatar = componentInParent2.playerAvatar;
					}
				}
				if (playerAvatar && playerAvatar == _player)
				{
					exists = playerAvatar;
					x = raycastHit.transform;
				}
			}
		}
		if (exists && x == y)
		{
			Debug.DrawRay(_startPosition, vector, Color.green, 0.1f);
			return true;
		}
		Debug.DrawRay(_startPosition, a - _startPosition, Color.red, 0.1f);
		return false;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x000598B0 File Offset: 0x00057AB0
	public static void PlayerEyesOverride(PlayerAvatar _player, Vector3 _position, float _time, GameObject _obj)
	{
		_player.playerAvatarVisuals.playerEyes.Override(_position, _time, _obj);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x000598C8 File Offset: 0x00057AC8
	public static void PlayerEyesOverrideSoft(Vector3 _position, float _time, GameObject _obj, float _radius)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (Vector3.Distance(_position, playerAvatar.transform.position) < _radius)
			{
				playerAvatar.playerAvatarVisuals.playerEyes.OverrideSoft(_position, _time, _obj);
			}
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00059940 File Offset: 0x00057B40
	public static Transform PlayerGetNearestTransformWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		float num = range;
		Transform result = null;
		List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(range, position, doRaycastCheck, layerMask);
		if (list.Count > 0)
		{
			foreach (PlayerAvatar playerAvatar in list)
			{
				Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = playerAvatar.PlayerVisionTarget.VisionTransform;
				}
			}
		}
		return result;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x000599D4 File Offset: 0x00057BD4
	public static List<PlayerAvatar> PlayerGetAll()
	{
		return GameDirector.instance.PlayerList;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x000599E0 File Offset: 0x00057BE0
	public static bool PlayersAllInTruck()
	{
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (!playerAvatar.isDisabled && !playerAvatar.RoomVolumeCheck.inTruck)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00059A48 File Offset: 0x00057C48
	public static void Command(string _command)
	{
		string text = _command.ToLower();
		if (text == "/cinematic")
		{
			GameDirector.instance.CommandRecordingDirectorToggle();
			return;
		}
		if (!(text == "/greenscreen"))
		{
			if (!(text == "/enemy vision"))
			{
				if (!(text == "/slow"))
				{
					if (!(text == "/recording level"))
					{
						if (text == "/clear")
						{
							Debug.ClearDeveloperConsole();
							return;
						}
						if (text == null)
						{
							return;
						}
						int fps;
						if (_command.Length >= 4 && _command.Substring(0, 4) == "/fps" && SemiFunc.DebugDev() && int.TryParse(_command.Substring(5), out fps))
						{
							GameDirector.instance.CommandSetFPS(fps);
						}
					}
					else if (SemiFunc.DebugDev())
					{
						RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Recording);
						return;
					}
				}
				else if (SemiFunc.DebugDev())
				{
					PlayerController.instance.debugSlow = !PlayerController.instance.debugSlow;
					return;
				}
			}
			else if (SemiFunc.DebugDev())
			{
				EnemyDirector.instance.debugNoVision = !EnemyDirector.instance.debugNoVision;
				return;
			}
			return;
		}
		GameDirector.instance.CommandGreenScreenToggle();
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00059B5F File Offset: 0x00057D5F
	public static void CursorUnlock(float _time)
	{
		CursorManager.instance.Unlock(_time);
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00059B6C File Offset: 0x00057D6C
	public static bool OnValidateCheck()
	{
		return false;
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00059B6F File Offset: 0x00057D6F
	public static void LightAdd(PropLight propLight)
	{
		if (!LightManager.instance.propLights.Contains(propLight))
		{
			LightManager.instance.propLights.Add(propLight);
		}
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00059B93 File Offset: 0x00057D93
	public static void LightRemove(PropLight propLight)
	{
		if (LightManager.instance.propLights.Contains(propLight))
		{
			LightManager.instance.propLights.Remove(propLight);
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00059BB8 File Offset: 0x00057DB8
	public static Vector3 EnemyRoamFindPoint(Vector3 _position)
	{
		Vector3 result = Vector3.zero;
		LevelPoint levelPoint = SemiFunc.LevelPointGet(_position, 10f, 25f);
		if (!levelPoint)
		{
			levelPoint = SemiFunc.LevelPointGet(_position, 0f, 999f);
		}
		NavMeshHit navMeshHit;
		if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			result = navMeshHit.position;
		}
		return result;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00059C60 File Offset: 0x00057E60
	public static Vector3 EnemyLeaveFindPoint(Vector3 _position)
	{
		Vector3 result = Vector3.zero;
		LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(_position, 30f, 50f, false);
		if (!levelPoint)
		{
			levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(_position, 5f);
		}
		NavMeshHit navMeshHit;
		if (levelPoint && NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			result = navMeshHit.position;
		}
		return result;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00059D04 File Offset: 0x00057F04
	public static List<LevelPoint> LevelPointGetWithinDistance(Vector3 pos, float minDist, float maxDist)
	{
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			float num = Vector3.Distance(levelPoint.transform.position, pos);
			if (num >= minDist && num <= maxDist)
			{
				list.Add(levelPoint);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00059D88 File Offset: 0x00057F88
	public static List<LevelPoint> LevelPointsGetAll()
	{
		return LevelGenerator.Instance.LevelPathPoints;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00059D94 File Offset: 0x00057F94
	public static LevelPoint LevelPointsGetClosestToPlayer()
	{
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		List<LevelPoint> list2 = SemiFunc.LevelPointsGetAll();
		float num = 999f;
		LevelPoint result = null;
		foreach (PlayerAvatar playerAvatar in list)
		{
			if (!playerAvatar.isDisabled)
			{
				Vector3 position = playerAvatar.transform.position;
				foreach (LevelPoint levelPoint in list2)
				{
					float num2 = Vector3.Distance(position, levelPoint.transform.position);
					if (num2 < num)
					{
						num = num2;
						result = levelPoint;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00059E60 File Offset: 0x00058060
	public static List<LevelPoint> LevelPointsGetAllCloseToPlayers()
	{
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		List<LevelPoint> list2 = SemiFunc.LevelPointsGetAll();
		List<LevelPoint> list3 = new List<LevelPoint>();
		foreach (PlayerAvatar playerAvatar in list)
		{
			float num = 999f;
			LevelPoint item = null;
			if (!playerAvatar.isDisabled)
			{
				Vector3 position = playerAvatar.transform.position;
				foreach (LevelPoint levelPoint in list2)
				{
					float num2 = Vector3.Distance(position, levelPoint.transform.position);
					if (num2 < num)
					{
						num = num2;
						item = levelPoint;
					}
				}
				list3.Add(item);
			}
		}
		return list3;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00059F40 File Offset: 0x00058140
	public static List<LevelPoint> LevelPointsGetInPlayerRooms()
	{
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		List<LevelPoint> list2 = SemiFunc.LevelPointsGetAll();
		List<LevelPoint> list3 = new List<LevelPoint>();
		foreach (PlayerAvatar playerAvatar in list)
		{
			if (!playerAvatar.isDisabled)
			{
				foreach (RoomVolume y in playerAvatar.RoomVolumeCheck.CurrentRooms)
				{
					foreach (LevelPoint levelPoint in list2)
					{
						if (levelPoint.Room == y)
						{
							list3.Add(levelPoint);
						}
					}
				}
			}
		}
		return list3;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0005A038 File Offset: 0x00058238
	public static List<LevelPoint> LevelPointsGetInStartRoom()
	{
		List<LevelPoint> list = SemiFunc.LevelPointsGetAll();
		List<LevelPoint> list2 = new List<LevelPoint>();
		foreach (LevelPoint levelPoint in list)
		{
			if (levelPoint.inStartRoom)
			{
				list2.Add(levelPoint);
			}
		}
		return list2;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0005A09C File Offset: 0x0005829C
	public static LevelPoint LevelPointGetPlayerDistance(Vector3 _position, float _minDistance, float _maxDistance, bool _startRoomOnly = false)
	{
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if ((!_startRoomOnly || levelPoint.inStartRoom) && !levelPoint.Room.Truck)
			{
				float num = 999f;
				bool flag = false;
				Vector3 position = levelPoint.transform.position;
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled)
					{
						float num2 = Vector3.Distance(position, playerAvatar.transform.position);
						if (num2 < num)
						{
							num = num2;
						}
						if (num2 < _maxDistance)
						{
							flag = true;
						}
					}
				}
				if (num > _minDistance && flag)
				{
					list.Add(levelPoint);
				}
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0005A1C8 File Offset: 0x000583C8
	public static LevelPoint LevelPointGetFurthestFromPlayer(Vector3 _position, float _minDistance)
	{
		float num = 0f;
		LevelPoint result = null;
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (!levelPoint.Room.Truck)
			{
				float num2 = 999f;
				float num3 = 0f;
				Vector3 position = levelPoint.transform.position;
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled)
					{
						float num4 = Vector3.Distance(position, playerAvatar.transform.position);
						if (num4 < num2)
						{
							num2 = num4;
						}
						if (num4 > num3)
						{
							num3 = num4;
						}
					}
				}
				if (num2 > _minDistance && num3 > num)
				{
					num = num3;
					result = levelPoint;
				}
			}
		}
		return result;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0005A2D4 File Offset: 0x000584D4
	public static void PhysLookAtPositionWithForce(Rigidbody rb, Transform transform, Vector3 position, float forceMultiplier)
	{
		Vector3 normalized = (position - transform.position).normalized;
		Vector3 a = Vector3.Cross(transform.forward, normalized);
		float magnitude = a.magnitude;
		a.Normalize();
		rb.AddTorque(a * magnitude * forceMultiplier);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0005A325 File Offset: 0x00058525
	public static bool IsNotMasterClient()
	{
		return GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x0005A338 File Offset: 0x00058538
	public static bool IsMasterClientOrSingleplayer()
	{
		return (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient) || !GameManager.Multiplayer();
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0005A352 File Offset: 0x00058552
	public static bool IsMasterClient()
	{
		return GameManager.Multiplayer() && PhotonNetwork.IsMasterClient;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0005A362 File Offset: 0x00058562
	public static bool IsMainMenu()
	{
		return MainMenuOpen.instance;
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x0005A36E File Offset: 0x0005856E
	public static bool IsMultiplayer()
	{
		return GameManager.instance.gameMode == 1;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x0005A37D File Offset: 0x0005857D
	public static bool MenuLevel()
	{
		return MainMenuOpen.instance || LobbyMenuOpen.instance;
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0005A39A File Offset: 0x0005859A
	public static bool RunIsArena()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelArena;
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x0005A3BA File Offset: 0x000585BA
	public static void CameraShake(float strength, float duration)
	{
		GameDirector.instance.CameraShake.Shake(strength, duration);
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x0005A3CD File Offset: 0x000585CD
	public static void CameraShakeDistance(Vector3 position, float strength, float duration, float distanceMin, float distanceMax)
	{
		GameDirector.instance.CameraShake.ShakeDistance(strength, distanceMin, distanceMax, position, duration);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x0005A3E4 File Offset: 0x000585E4
	public static void CameraShakeImpact(float strength, float duration)
	{
		GameDirector.instance.CameraImpact.Shake(strength, duration);
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0005A3F7 File Offset: 0x000585F7
	public static void CameraShakeImpactDistance(Vector3 position, float strength, float duration, float distanceMin, float distanceMax)
	{
		GameDirector.instance.CameraImpact.ShakeDistance(strength, distanceMin, distanceMax, position, duration);
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0005A410 File Offset: 0x00058610
	public static Color ColorDifficultyGet(float minValue, float maxValue, float _currentValue)
	{
		Color[] array = new Color[]
		{
			new Color(0f, 1f, 0f),
			new Color(1f, 1f, 0f),
			new Color(1f, 0.5f, 0f),
			new Color(1f, 0f, 0f)
		};
		int num = Mathf.FloorToInt(Mathf.Lerp(0f, (float)(array.Length - 1), Mathf.InverseLerp(minValue, maxValue, _currentValue)));
		float t = Mathf.InverseLerp(minValue, maxValue, _currentValue) * (float)(array.Length - 1) - (float)num;
		Color a = array[Mathf.Clamp(num, 0, array.Length - 1)];
		Color b = array[Mathf.Clamp(num + 1, 0, array.Length - 1)];
		return Color.Lerp(a, b, t);
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0005A4F0 File Offset: 0x000586F0
	public static string TimeToString(float time, bool fancy = false, Color numberColor = default(Color), Color unitColor = default(Color))
	{
		int num = (int)(time / 3600f);
		int num2 = (int)(time % 3600f / 60f);
		int num3 = (int)(time % 60f);
		string str = "h ";
		string str2 = "m ";
		string str3 = "s";
		if (fancy)
		{
			str = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">h</color> ";
			str2 = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">m</color> ";
			str3 = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">s</color>";
		}
		string text = "";
		if (num > 0)
		{
			if (fancy)
			{
				text = text + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text = text + num.ToString() + str;
		}
		if (num2 > 0 || num > 0)
		{
			if (fancy)
			{
				text = text + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text = text + num2.ToString() + str2;
		}
		if ((num == 0 && num2 == 0) || fancy)
		{
			if (fancy)
			{
				text = text + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text = text + num3.ToString() + str3;
		}
		return text;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0005A628 File Offset: 0x00058828
	public static List<PhysGrabObject> PhysGrabObjectGetAllWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask), PhysGrabObject _thisPhysGrabObject = null)
	{
		List<PhysGrabObject> list = new List<PhysGrabObject>();
		Collider[] array = Physics.OverlapSphere(position, range, LayerMask.GetMask(new string[]
		{
			"PhysGrabObject"
		}));
		for (int i = 0; i < array.Length; i++)
		{
			PhysGrabObject componentInParent = array[i].GetComponentInParent<PhysGrabObject>();
			if (componentInParent != null)
			{
				bool flag = false;
				if (doRaycastCheck)
				{
					Vector3 normalized = (componentInParent.midPoint - position).normalized;
					foreach (RaycastHit raycastHit in Physics.RaycastAll(position, normalized, range, layerMask))
					{
						PhysGrabObject componentInParent2 = raycastHit.collider.GetComponentInParent<PhysGrabObject>();
						if (!(componentInParent2 == _thisPhysGrabObject) && !(componentInParent2 == componentInParent) && (componentInParent2 == null || (componentInParent2 != null && componentInParent2 != componentInParent)))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					list.Add(componentInParent);
				}
			}
		}
		return list;
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x0005A720 File Offset: 0x00058920
	public static bool LocalPlayerOverlapCheck(float range, Vector3 position, bool doRaycastCheck = false)
	{
		foreach (Collider collider in Physics.OverlapSphere(position, range, SemiFunc.LayerMaskGetVisionObstruct()))
		{
			PlayerController exists = null;
			if (collider.transform.CompareTag("Player"))
			{
				exists = collider.GetComponentInParent<PlayerController>();
			}
			else
			{
				PlayerTumble componentInParent = collider.GetComponentInParent<PlayerTumble>();
				if (componentInParent && componentInParent.playerAvatar.isLocal)
				{
					exists = PlayerController.instance;
				}
			}
			if (exists)
			{
				bool flag = false;
				if (doRaycastCheck)
				{
					Vector3 normalized = (collider.transform.position - position).normalized;
					foreach (RaycastHit raycastHit in Physics.RaycastAll(position, normalized, range, LayerMask.GetMask(new string[]
					{
						"Default"
					})))
					{
						if (raycastHit.transform.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0005A81E File Offset: 0x00058A1E
	public static Vector3 PhysFollowPosition(Vector3 _currentPosition, Vector3 _targetPosition, Vector3 _currentVelocity, float _maxSpeed)
	{
		return Vector3.ClampMagnitude((_targetPosition - _currentPosition) / Time.fixedDeltaTime, _maxSpeed) - _currentVelocity;
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0005A840 File Offset: 0x00058A40
	public static Vector3 PhysFollowRotation(Transform _transform, Quaternion _targetRotation, Rigidbody _rigidbody, float _maxSpeed)
	{
		_transform.rotation = Quaternion.RotateTowards(_targetRotation, _transform.rotation, 360f);
		float d;
		Vector3 a;
		(_targetRotation * Quaternion.Inverse(_transform.rotation)).ToAngleAxis(out d, out a);
		a.Normalize();
		Vector3 vector = a * d * 0.017453292f / Time.fixedDeltaTime;
		vector -= _rigidbody.angularVelocity;
		Vector3 point = _transform.InverseTransformDirection(vector);
		point = _rigidbody.inertiaTensorRotation * point;
		point.Scale(_rigidbody.inertiaTensor);
		Vector3 direction = Quaternion.Inverse(_rigidbody.inertiaTensorRotation) * point;
		return Vector3.ClampMagnitude(_transform.TransformDirection(direction), _maxSpeed);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0005A8F8 File Offset: 0x00058AF8
	public static Vector3 PhysFollowDirection(Transform _transform, Vector3 _targetDirection, Rigidbody _rigidbody, float _maxSpeed)
	{
		Vector3 normalized = Vector3.Cross(Vector3.up, _targetDirection).normalized;
		Quaternion rotation = _transform.rotation;
		_transform.Rotate(normalized * 100f, Space.World);
		Quaternion rotation2 = _transform.rotation;
		_transform.rotation = rotation;
		return SemiFunc.PhysFollowRotation(_transform.transform, rotation2, _rigidbody, _maxSpeed);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0005A950 File Offset: 0x00058B50
	public static LevelPoint LevelPointGet(Vector3 _position, float _minDistance, float _maxDistance)
	{
		LevelPoint result = null;
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (!levelPoint.Room.Truck)
			{
				float num = Vector3.Distance(levelPoint.transform.position, _position);
				if (num >= _minDistance && num <= _maxDistance)
				{
					list.Add(levelPoint);
				}
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0005A9F8 File Offset: 0x00058BF8
	public static LevelPoint LevelPointInTargetRoomGet(RoomVolumeCheck _target, float _minDistance, float _maxDistance, LevelPoint ignorePoint = null)
	{
		LevelPoint result = null;
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			foreach (RoomVolume y in _target.CurrentRooms)
			{
				if (!(levelPoint == ignorePoint) && levelPoint.Room == y)
				{
					float num = Vector3.Distance(levelPoint.transform.position, _target.CheckPosition);
					if (num >= _minDistance && num <= _maxDistance)
					{
						list.Add(levelPoint);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0005AAF0 File Offset: 0x00058CF0
	public static bool OnScreen(Vector3 position, float paddWidth, float paddHeight)
	{
		paddWidth = (float)Screen.width * paddWidth;
		paddHeight = (float)Screen.height * paddHeight;
		Vector3 vector = CameraUtils.Instance.MainCamera.WorldToScreenPoint(position);
		vector.x *= (float)Screen.width / RenderTextureMain.instance.textureWidth;
		vector.y *= (float)Screen.height / RenderTextureMain.instance.textureHeight;
		return vector.z > 0f && vector.x > -paddWidth && vector.x < (float)Screen.width + paddWidth && vector.y > -paddHeight && vector.y < (float)Screen.height + paddHeight;
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0005ABA4 File Offset: 0x00058DA4
	public static Quaternion ClampRotation(Quaternion _quaternion, Vector3 _bounds)
	{
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		float num = 114.59156f * Mathf.Atan(_quaternion.x);
		num = Mathf.Clamp(num, -_bounds.x, _bounds.x);
		_quaternion.x = Mathf.Tan(0.008726646f * num);
		float num2 = 114.59156f * Mathf.Atan(_quaternion.y);
		num2 = Mathf.Clamp(num2, -_bounds.y, _bounds.y);
		_quaternion.y = Mathf.Tan(0.008726646f * num2);
		float num3 = 114.59156f * Mathf.Atan(_quaternion.z);
		num3 = Mathf.Clamp(num3, -_bounds.z, _bounds.z);
		_quaternion.z = Mathf.Tan(0.008726646f * num3);
		return _quaternion.normalized;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0005ACA4 File Offset: 0x00058EA4
	public static Vector3 ClampDirection(Vector3 _direction, Vector3 _forward, float _maxAngle)
	{
		Vector3 result = _direction;
		if (Vector3.Angle(_direction, _forward) > _maxAngle)
		{
			Vector3 axis = Vector3.Cross(_forward, _direction);
			result = Quaternion.AngleAxis(_maxAngle, axis) * _forward;
		}
		return result;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0005ACD4 File Offset: 0x00058ED4
	public static List<PlayerAvatar> PlayerGetList()
	{
		return GameDirector.instance.PlayerList;
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0005ACE0 File Offset: 0x00058EE0
	public static int PhotonViewIDPlayerAvatarLocal()
	{
		return PlayerAvatar.instance.GetComponent<PhotonView>().ViewID;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0005ACF1 File Offset: 0x00058EF1
	public static string EmojiText(string inputText)
	{
		inputText = inputText.Replace("{", "<sprite name=");
		inputText = inputText.Replace("}", ">");
		return inputText;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0005AD18 File Offset: 0x00058F18
	public static string DollarGetString(int value)
	{
		return value.ToString("#,0", new CultureInfo("de-DE"));
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0005AD30 File Offset: 0x00058F30
	public static PhysicMaterial PhysicMaterialSticky()
	{
		return AssetManager.instance.physicMaterialStickyExtreme;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0005AD3C File Offset: 0x00058F3C
	public static PhysicMaterial PhysicMaterialSlippery()
	{
		return AssetManager.instance.physicMaterialSlipperyExtreme;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0005AD48 File Offset: 0x00058F48
	public static PhysicMaterial PhysicMaterialSlipperyPlus()
	{
		return AssetManager.instance.physicMaterialSlipperyPlus;
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0005AD54 File Offset: 0x00058F54
	public static PhysicMaterial PhysicMaterialDefault()
	{
		return AssetManager.instance.physicMaterialDefault;
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0005AD60 File Offset: 0x00058F60
	public static PhysicMaterial PhysicMaterialPhysGrabObject()
	{
		return AssetManager.instance.physicMaterialPhysGrabObject;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0005AD6C File Offset: 0x00058F6C
	public static int RunGetLevelsMax()
	{
		return RunManager.instance.levelsMax;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0005AD78 File Offset: 0x00058F78
	public static int RunGetLevelsCompleted()
	{
		return RunManager.instance.levelsCompleted;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0005AD84 File Offset: 0x00058F84
	public static float RunGetDifficultyMultiplier()
	{
		return (float)RunManager.instance.levelsCompleted / (float)RunManager.instance.levelsMax;
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0005AD9D File Offset: 0x00058F9D
	public static bool PhysGrabObjectIsGrabbed(PhysGrabObject physGrabObject)
	{
		return physGrabObject.grabbed;
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0005ADA5 File Offset: 0x00058FA5
	public static List<PhysGrabber> PhysGrabObjectGetPhysGrabbersGrabbing(PhysGrabObject physGrabObject)
	{
		return physGrabObject.playerGrabbing;
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0005ADB0 File Offset: 0x00058FB0
	public static List<PlayerAvatar> PhysGrabObjectGetPlayerAvatarsGrabbing(PhysGrabObject physGrabObject)
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PhysGrabber physGrabber in physGrabObject.playerGrabbing)
		{
			list.Add(physGrabber.playerAvatar);
		}
		return list;
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0005AE10 File Offset: 0x00059010
	public static bool PhysGrabberLocalIsGrabbing()
	{
		return PhysGrabber.instance.grabbed;
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0005AE1C File Offset: 0x0005901C
	public static void PhysGrabberLocalForceDrop()
	{
		if (PhysGrabber.instance.grabbed)
		{
			PhysGrabber.instance.OverrideGrabRelease();
		}
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0005AE34 File Offset: 0x00059034
	public static void PhysGrabberLocalForceGrab(PhysGrabObject physGrabObject)
	{
		PhysGrabber.instance.OverrideGrab(physGrabObject);
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0005AE41 File Offset: 0x00059041
	public static PhysGrabObject PhysGrabberLocalGetGrabbedPhysGrabObject()
	{
		if (!PhysGrabber.instance.grabbed)
		{
			return null;
		}
		return PhysGrabber.instance.grabbedObject.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0005AE60 File Offset: 0x00059060
	public static bool PhysGrabberIsGrabbing(PhysGrabber physGrabber)
	{
		return physGrabber.grabbed;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0005AE68 File Offset: 0x00059068
	public static PhysGrabObject PhysGrabberGetGrabbedPhysGrabObject(PhysGrabber physGrabber)
	{
		if (!physGrabber.grabbed)
		{
			return null;
		}
		return physGrabber.grabbedObject.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x0005AE7F File Offset: 0x0005907F
	public static void PhysGrabberForceDrop(PhysGrabber physGrabber)
	{
		if (physGrabber.grabbed)
		{
			physGrabber.OverrideGrabRelease();
		}
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0005AE8F File Offset: 0x0005908F
	public static void PhysGrabberForceGrab(PhysGrabber physGrabber, PhysGrabObject physGrabObject)
	{
		physGrabber.OverrideGrab(physGrabObject);
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0005AE98 File Offset: 0x00059098
	public static void PhysGrabberLocalChangeAlpha(float alpha)
	{
		PhysGrabber.instance.ChangeBeamAlpha(alpha);
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0005AEA5 File Offset: 0x000590A5
	public static void LightManagerSetCullTargetTransform(Transform target)
	{
		LightManager.instance.lightCullTarget = target;
		LightManager.instance.UpdateInstant();
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0005AEBC File Offset: 0x000590BC
	public static string MenuGetSelectableID(GameObject gameObject)
	{
		return "" + gameObject.GetInstanceID().ToString();
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0005AEE4 File Offset: 0x000590E4
	public static void MenuSelectionBoxTargetSet(MenuPage parentPage, RectTransform rectTransform, Vector2 customOffset = default(Vector2), Vector2 customScale = default(Vector2))
	{
		Vector2 vector = SemiFunc.UIGetRectTransformPositionOnScreen(rectTransform, false);
		Vector2 vector2 = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
		vector += new Vector2(vector2.x / 2f, vector2.y / 2f) + customOffset;
		MenuSelectableElement component = rectTransform.GetComponent<MenuSelectableElement>();
		MenuSelectionBox menuSelectionBox = parentPage.selectionBox;
		Vector2 vector3 = new Vector2(0f, 0f);
		bool isInScrollBox = false;
		if (component && component.isInScrollBox)
		{
			isInScrollBox = true;
			menuSelectionBox = component.menuScrollBox.menuSelectionBox;
			Transform parent = rectTransform.parent;
			int num = 30;
			while (parent && !parent.GetComponent<MenuPage>())
			{
				RectTransform component2 = parent.GetComponent<RectTransform>();
				if (component2 && !component2.GetComponent<MenuSelectableElement>())
				{
					vector3 -= new Vector2(component2.localPosition.x, component2.localPosition.y);
				}
				parent = parent.parent;
				num--;
				if (num <= 0)
				{
					Debug.LogError(rectTransform.name + " - Hover FAIL! Could not find a parent page ");
					break;
				}
			}
		}
		vector += vector3;
		MenuElementAnimations componentInParent = rectTransform.GetComponentInParent<MenuElementAnimations>();
		if (componentInParent)
		{
			float num2 = (float)Screen.width / (float)MenuManager.instance.screenUIWidth;
			float num3 = (float)Screen.height / (float)MenuManager.instance.screenUIHeight;
			componentInParent.GetComponent<RectTransform>();
		}
		menuSelectionBox.MenuSelectionBoxSetTarget(vector, vector2, component.parentPage, isInScrollBox, component.menuScrollBox, customScale);
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0005B091 File Offset: 0x00059291
	public static float MenuGetPitchFromYPos(RectTransform rectTransform)
	{
		return Mathf.Lerp(0.5f, 2f, rectTransform.localPosition.y / (float)Screen.height);
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0005B0B4 File Offset: 0x000592B4
	public static Vector2 UIPositionToUIPosition(Vector3 position)
	{
		Vector3 vector = CameraOverlay.instance.overlayCamera.ScreenToViewportPoint(position) * SemiFunc.UIMulti();
		vector.x *= 1.015f;
		vector.y *= 1.015f;
		vector.x /= (float)Screen.width;
		vector.y /= (float)Screen.height;
		float num = HUDCanvas.instance.rect.sizeDelta.x / HUDCanvas.instance.rect.sizeDelta.y;
		float num2 = HUDCanvas.instance.rect.sizeDelta.x * num / HUDCanvas.instance.rect.sizeDelta.y;
		vector.x *= HUDCanvas.instance.rect.sizeDelta.x * num2;
		vector.y *= HUDCanvas.instance.rect.sizeDelta.y * num;
		vector.x -= 18f;
		vector.y -= 15f;
		return new Vector2(vector.x, vector.y);
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0005B1F4 File Offset: 0x000593F4
	public static Vector2 UIMousePosToUIPos()
	{
		Vector3 vector = CameraOverlay.instance.overlayCamera.ScreenToViewportPoint(Input.mousePosition) * SemiFunc.UIMulti();
		vector.x *= 1.015f;
		vector.y *= 1.015f;
		vector.x /= (float)Screen.width;
		vector.y /= (float)Screen.height;
		float num = HUDCanvas.instance.rect.sizeDelta.x / HUDCanvas.instance.rect.sizeDelta.y;
		float num2 = HUDCanvas.instance.rect.sizeDelta.x * num / HUDCanvas.instance.rect.sizeDelta.y;
		vector.x *= HUDCanvas.instance.rect.sizeDelta.x * num2;
		vector.y *= HUDCanvas.instance.rect.sizeDelta.y * num;
		return new Vector2(vector.x, vector.y);
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0005B318 File Offset: 0x00059518
	public static Vector2 UIGetRectTransformPositionOnScreen(RectTransform rectTransform, bool withScreenMultiplier = true)
	{
		int num = 1;
		int num2 = 1;
		Vector3 position = rectTransform.position;
		Vector3 position2 = rectTransform.GetComponentInParent<MenuPage>().GetComponent<RectTransform>().position;
		Vector3 vector = position - position2;
		vector -= new Vector3(rectTransform.rect.width * rectTransform.pivot.x, rectTransform.rect.height * rectTransform.pivot.y, 0f);
		if (withScreenMultiplier)
		{
			vector = new Vector2(vector.x * (float)num, vector.y * (float)num2);
		}
		return vector;
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0005B3B4 File Offset: 0x000595B4
	public static Vector2 UIMouseGetLocalPositionWithinRectTransform(RectTransform rectTransform)
	{
		Vector2 vector = SemiFunc.UIMousePosToUIPos();
		Vector2 vector2 = SemiFunc.UIGetRectTransformPositionOnScreen(rectTransform, false);
		Vector2 vector3 = new Vector2(vector.x - vector2.x, vector.y - vector2.y);
		float num = rectTransform.rect.width * rectTransform.pivot.x;
		float num2 = rectTransform.rect.height * rectTransform.pivot.y;
		Vector3 lossyScale = rectTransform.lossyScale;
		float num3 = 1f;
		if (lossyScale.y < 1f)
		{
			num3 = 1f + (1f - lossyScale.y);
		}
		if (lossyScale.y > 1f)
		{
			num3 = 1f + (lossyScale.y - 1f);
		}
		vector3 = new Vector2((vector3.x + num) * num3, (vector3.y + num2) * num3);
		return vector3;
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0005B4A0 File Offset: 0x000596A0
	public static bool UIMouseHover(MenuPage parentPage, RectTransform rectTransform, string menuID, float xPadding = 0f, float yPadding = 0f)
	{
		if (parentPage.parentPage && !parentPage.parentPage.pageActive)
		{
			return false;
		}
		Vector2 vector = SemiFunc.UIMousePosToUIPos();
		if (MenuManager.instance.mouseHoldPosition != Vector2.zero)
		{
			vector = MenuManager.instance.mouseHoldPosition;
		}
		int num = 1;
		int num2 = 1;
		MenuScrollBox componentInParent = rectTransform.GetComponentInParent<MenuScrollBox>();
		if (componentInParent)
		{
			float num3 = (componentInParent.transform.position.y - 10f) * (float)num2;
			float num4 = (componentInParent.scrollerEndPosition + 32f) * (float)num2;
			if (vector.y > num4 || vector.y < num3)
			{
				return false;
			}
		}
		Vector2 vector2 = SemiFunc.UIGetRectTransformPositionOnScreen(rectTransform, false);
		float num5 = (vector2.x + (rectTransform.rect.xMin - xPadding)) * (float)num;
		float num6 = (vector2.x + (rectTransform.rect.xMax + xPadding)) * (float)num;
		float num7 = (vector2.y + (rectTransform.rect.yMin - yPadding)) * (float)num2;
		float num8 = (vector2.y + (rectTransform.rect.yMax + yPadding)) * (float)num2;
		bool result;
		if (vector.x >= num5 && vector.x <= num6 && vector.y >= num7 && vector.y <= num8)
		{
			result = true;
			if (menuID != "-1")
			{
				if (MenuManager.instance.currentMenuID == menuID)
				{
					MenuManager.instance.MenuHover();
				}
				if (MenuManager.instance.currentMenuID == "")
				{
					MenuManager.instance.currentMenuID = menuID;
				}
			}
		}
		else
		{
			result = false;
			if (menuID != "-1" && MenuManager.instance.currentMenuID == menuID)
			{
				MenuManager.instance.currentMenuID = "";
			}
		}
		if (menuID != "-1")
		{
			return menuID == MenuManager.instance.currentMenuID;
		}
		return result;
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0005B691 File Offset: 0x00059891
	public static void UIHideAim()
	{
		Aim.instance.SetState(Aim.State.Hidden);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0005B69E File Offset: 0x0005989E
	public static void UIHideTumble()
	{
		TumbleUI.instance.Hide();
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0005B6AA File Offset: 0x000598AA
	public static void UIHideWorldSpace()
	{
		WorldSpaceUIParent.instance.Hide();
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0005B6B6 File Offset: 0x000598B6
	public static void UIHideValuableDiscover()
	{
		ValuableDiscover.instance.Hide();
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0005B6C2 File Offset: 0x000598C2
	public static void UIShowArrow(Vector3 startPosition, Vector3 endPosition, float rotation)
	{
		ArrowUI.instance.ArrowShow(startPosition, endPosition, rotation);
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0005B6D1 File Offset: 0x000598D1
	public static void UIShowArrowWorldPosition(Vector3 startPosition, Vector3 endPosition, float rotation)
	{
		ArrowUI.instance.ArrowShowWorldPos(startPosition, endPosition, rotation);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0005B6E0 File Offset: 0x000598E0
	public static void UIBigMessage(string message, string emoji, float size, Color colorMain, Color colorFlash)
	{
		BigMessageUI.instance.BigMessage(message, emoji, size, colorMain, colorFlash);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0005B6F2 File Offset: 0x000598F2
	public static void UIFocusText(string message, Color colorMain, Color colorFlash, float time = 3f)
	{
		MissionUI.instance.MissionText(message, colorMain, colorFlash, time);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0005B702 File Offset: 0x00059902
	public static void UIItemInfoText(ItemAttributes itemAttributes, string message)
	{
		ItemInfoUI.instance.ItemInfoText(itemAttributes, message, false);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0005B711 File Offset: 0x00059911
	public static void UIHideHealth()
	{
		HealthUI.instance.Hide();
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0005B71D File Offset: 0x0005991D
	public static void UIHideEnergy()
	{
		EnergyUI.instance.Hide();
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0005B729 File Offset: 0x00059929
	public static void UIHideInventory()
	{
		InventoryUI.instance.Hide();
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0005B735 File Offset: 0x00059935
	public static void UIHideHaul()
	{
		HaulUI.instance.Hide();
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0005B741 File Offset: 0x00059941
	public static void UIHideGoal()
	{
		GoalUI.instance.Hide();
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0005B74D File Offset: 0x0005994D
	public static void UIHideCurrency()
	{
		CurrencyUI.instance.Hide();
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0005B759 File Offset: 0x00059959
	public static void UIHideShopCost()
	{
		ShopCostUI.instance.Hide();
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0005B768 File Offset: 0x00059968
	public static void UIShowSpectate()
	{
		if (SemiFunc.IsMultiplayer() && SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Normal) && SpectateNameUI.instance.Text.text != "" && (!Arena.instance || Arena.instance.currentState != Arena.States.GameOver))
		{
			SpectateNameUI.instance.Show();
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0005B7D4 File Offset: 0x000599D4
	public static Vector3 UIWorldToCanvasPosition(Vector3 _worldPosition)
	{
		RectTransform rect = HUDCanvas.instance.rect;
		if (SemiFunc.OnScreen(_worldPosition, 0.5f, 0.5f))
		{
			Vector3 vector = AssetManager.instance.mainCamera.WorldToViewportPoint(_worldPosition);
			vector = new Vector3(vector.x * rect.sizeDelta.x - rect.sizeDelta.x * 0.5f, vector.y * rect.sizeDelta.y - rect.sizeDelta.y * 0.5f, vector.z);
			return vector;
		}
		return new Vector3(-rect.sizeDelta.x * 2f, -rect.sizeDelta.y * 2f, 0f);
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0005B894 File Offset: 0x00059A94
	public static bool FPSImpulse1()
	{
		return GameDirector.instance.fpsImpulse1;
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0005B8A0 File Offset: 0x00059AA0
	public static bool FPSImpulse5()
	{
		return GameDirector.instance.fpsImpulse5;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0005B8AC File Offset: 0x00059AAC
	public static bool FPSImpulse15()
	{
		return GameDirector.instance.fpsImpulse15;
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0005B8B8 File Offset: 0x00059AB8
	public static bool FPSImpulse30()
	{
		return GameDirector.instance.fpsImpulse30;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0005B8C4 File Offset: 0x00059AC4
	public static bool FPSImpulse60()
	{
		return GameDirector.instance.fpsImpulse60;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0005B8D0 File Offset: 0x00059AD0
	public static void LocalPlayerOverrideEnergyUnlimited()
	{
		PlayerController.instance.EnergyCurrent = 100f;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0005B8E1 File Offset: 0x00059AE1
	public static void HUDSpectateSetName(string name)
	{
		SpectateNameUI.instance.SetName(name);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0005B8EE File Offset: 0x00059AEE
	public static int ValuableGetTotalNumber()
	{
		return ValuableDirector.instance.valuableSpawnAmount;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0005B8FA File Offset: 0x00059AFA
	public static bool ValuableTrapActivatedDiceRoll(int rarityLevel)
	{
		if (rarityLevel == 1)
		{
			return Random.Range(1, 3) == 1;
		}
		if (rarityLevel == 2)
		{
			return Random.Range(1, 5) == 1;
		}
		return rarityLevel > 2 && Random.Range(1, 10) == 1;
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0005B92C File Offset: 0x00059B2C
	public static LayerMask LayerMaskGetVisionObstruct()
	{
		return LayerMask.GetMask(new string[]
		{
			"Default",
			"Player",
			"PhysGrabObject",
			"PhysGrabObjectCart",
			"PhysGrabObjectHinge",
			"StaticGrabObject"
		});
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0005B97C File Offset: 0x00059B7C
	public static LayerMask LayerMaskGetShouldHits()
	{
		return LayerMask.GetMask(new string[]
		{
			"Default",
			"Player",
			"PhysGrabObject",
			"PhysGrabObjectCart",
			"PhysGrabObjectHinge",
			"StaticGrabObject",
			"Enemy"
		});
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0005B9D1 File Offset: 0x00059BD1
	public static LayerMask LayerMaskGetPlayersAndPhysObjects()
	{
		return LayerMask.GetMask(new string[]
		{
			"Player",
			"PhysGrabObject"
		});
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0005B9F3 File Offset: 0x00059BF3
	public static LayerMask LayerMaskGetPhysGrabObject()
	{
		return LayerMask.GetMask(new string[]
		{
			"PhysGrabObject",
			"PhysGrabObjectCart",
			"PhysGrabObjectHinge",
			"StaticGrabObject"
		});
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0005BA25 File Offset: 0x00059C25
	public static float BatteryGetChargeRate(int chargeLevel)
	{
		if (chargeLevel == 1)
		{
			return 1f;
		}
		if (chargeLevel == 2)
		{
			return 2f;
		}
		if (chargeLevel >= 3)
		{
			return 5f;
		}
		return 0f;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0005BA4A File Offset: 0x00059C4A
	public static bool BatteryChargeCondition(ItemBattery battery)
	{
		return battery && ((battery.batteryLife < 100f && battery.batteryActive) || (battery.batteryLife < 99f && !battery.batteryActive)) && !battery.isUnchargable;
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0005BA89 File Offset: 0x00059C89
	public static bool InventoryAnyEquipButton()
	{
		return SemiFunc.InputHold(InputKey.Inventory1) || SemiFunc.InputHold(InputKey.Inventory2) || SemiFunc.InputHold(InputKey.Inventory3);
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0005BAA6 File Offset: 0x00059CA6
	public static bool InventoryAnyEquipButtonUp()
	{
		return SemiFunc.InputUp(InputKey.Inventory1) || SemiFunc.InputUp(InputKey.Inventory2) || SemiFunc.InputUp(InputKey.Inventory3);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0005BAC3 File Offset: 0x00059CC3
	public static bool InventoryAnyEquipButtonDown()
	{
		return SemiFunc.InputDown(InputKey.Inventory1) || SemiFunc.InputDown(InputKey.Inventory2) || SemiFunc.InputDown(InputKey.Inventory3);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0005BAE0 File Offset: 0x00059CE0
	public static bool LevelGenDone()
	{
		return LevelGenerator.Instance.Generated;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0005BAEC File Offset: 0x00059CEC
	public static bool RunIsLobbyMenu()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu;
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0005BB07 File Offset: 0x00059D07
	public static bool RunIsShop()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelShop;
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0005BB22 File Offset: 0x00059D22
	public static bool RunIsLobby()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelLobby;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0005BB3D File Offset: 0x00059D3D
	public static bool RunIsTutorial()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelTutorial;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0005BB58 File Offset: 0x00059D58
	public static bool RunIsRecording()
	{
		return RunManager.instance.levelCurrent == RunManager.instance.levelRecording;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0005BB74 File Offset: 0x00059D74
	public static bool RunIsLevel()
	{
		return RunManager.instance.levelCurrent != RunManager.instance.levelShop && RunManager.instance.levelCurrent != RunManager.instance.levelLobby && RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu && RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu && RunManager.instance.levelCurrent != RunManager.instance.levelTutorial && RunManager.instance.levelCurrent != RunManager.instance.levelArena;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0005BC28 File Offset: 0x00059E28
	public static T Singleton<T>(ref T instance, GameObject gameObject) where T : Component
	{
		Debug.Log("Singleton called for type " + typeof(T).Name + " on GameObject " + gameObject.name);
		if (instance == null)
		{
			Debug.Log("No existing instance found, setting up new instance of " + typeof(T).Name);
			T t;
			if ((t = gameObject.GetComponent<T>()) == null)
			{
				t = gameObject.AddComponent<T>();
			}
			instance = t;
			Debug.Log("DontDestroyOnLoad called for " + gameObject.name);
			Object.DontDestroyOnLoad(gameObject);
		}
		else if (instance.gameObject != gameObject)
		{
			Debug.Log("Instance already exists for type " + typeof(T).Name + ", destroying game object " + gameObject.name);
			Object.Destroy(gameObject);
		}
		else
		{
			Debug.Log("Instance matches the current gameObject " + gameObject.name + ", no action needed");
		}
		Debug.Log("Singleton setup completed for type " + typeof(T).Name + " on GameObject " + gameObject.name);
		return instance;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0005BD55 File Offset: 0x00059F55
	public static void StatSetBattery(string itemName, int value)
	{
		StatsManager.instance.ItemUpdateStatBattery(itemName, value, true);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0005BD64 File Offset: 0x00059F64
	public static int StatSetRunLives(int value)
	{
		return PunManager.instance.SetRunStatSet("lives", value);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0005BD76 File Offset: 0x00059F76
	public static int StatSetRunCurrency(int value)
	{
		return PunManager.instance.SetRunStatSet("currency", value);
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0005BD88 File Offset: 0x00059F88
	public static int StatSetRunTotalHaul(int value)
	{
		return PunManager.instance.SetRunStatSet("totalHaul", value);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0005BD9A File Offset: 0x00059F9A
	public static int StatSetRunLevel(int value)
	{
		return PunManager.instance.SetRunStatSet("level", value);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0005BDAC File Offset: 0x00059FAC
	public static int StatSetSaveLevel(int value)
	{
		return PunManager.instance.SetRunStatSet("save level", value);
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0005BDBE File Offset: 0x00059FBE
	public static int StatSetRunFailures(int value)
	{
		return PunManager.instance.SetRunStatSet("failures", value);
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0005BDD0 File Offset: 0x00059FD0
	public static int StatGetItemBattery(string itemName)
	{
		return StatsManager.instance.itemStatBattery[itemName];
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0005BDE2 File Offset: 0x00059FE2
	public static int StatGetItemsPurchased(string itemName)
	{
		return StatsManager.instance.itemsPurchased[itemName];
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0005BDF4 File Offset: 0x00059FF4
	public static int StatGetRunCurrency()
	{
		return StatsManager.instance.GetRunStatCurrency();
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0005BE00 File Offset: 0x0005A000
	public static int StatGetRunTotalHaul()
	{
		return StatsManager.instance.GetRunStatTotalHaul();
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0005BE0C File Offset: 0x0005A00C
	public static int StatUpgradeItemBattery(string itemName)
	{
		return PunManager.instance.UpgradeItemBattery(itemName);
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0005BE19 File Offset: 0x0005A019
	public static void StatSyncAll()
	{
		StatsManager.instance.statsSynced = false;
		PunManager.instance.SyncAllDictionaries();
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0005BE30 File Offset: 0x0005A030
	public static bool StatsSynced()
	{
		return StatsManager.instance.statsSynced;
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0005BE3C File Offset: 0x0005A03C
	public static void ShopPopulateItemVolumes()
	{
		PunManager.instance.ShopPopulateItemVolumes();
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0005BE48 File Offset: 0x0005A048
	public static int ShopGetTotalCost()
	{
		return ShopManager.instance.totalCost;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0005BE54 File Offset: 0x0005A054
	public static void ShopUpdateCost()
	{
		PunManager.instance.ShopUpdateCost();
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0005BE60 File Offset: 0x0005A060
	public static void OnLevelGenDone()
	{
		ItemManager.instance.TurnOffIconLightsAgain();
		if (SemiFunc.RunIsLobby())
		{
			TutorialDirector.instance.TipsShow();
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0005BE80 File Offset: 0x0005A080
	public static void OnSceneSwitch(bool _gameOver, bool _leaveGame)
	{
		ItemManager.instance.itemIconLights.SetActive(true);
		if (SemiFunc.IsMultiplayer())
		{
			ChatManager.instance.ForceSendMessage(":o");
			ChatManager.instance.ClearAllChatBatches();
		}
		if (RunManager.instance.levelCurrent == RunManager.instance.levelLobby)
		{
			TutorialDirector instance = TutorialDirector.instance;
			if (instance)
			{
				instance.UpdateRoundEnd();
				instance.TipsStore();
			}
		}
		StatsManager.instance.StuffNeedingResetAtTheEndOfAScene();
		TutorialDirector.instance.TipCancel();
		ItemManager.instance.FetchLocalPlayersInventory();
		ItemManager.instance.powerCrystals.Clear();
		if (ChargingStation.instance && !_gameOver)
		{
			PunManager.instance.SetRunStatSet("chargingStationCharge", ChargingStation.instance.chargeInt);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !_leaveGame && !_gameOver)
		{
			SemiFunc.SaveFileSave();
		}
		DataDirector.instance.SaveDeleteCheck(_leaveGame);
		if (!_leaveGame)
		{
			SemiFunc.StatSyncAll();
		}
		if (_leaveGame)
		{
			SessionManager.instance.Reset();
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0005BF7C File Offset: 0x0005A17C
	public static PlayerAvatar PlayerAvatarGetFromPhotonID(int photonID)
	{
		PlayerAvatar result = null;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.photonView.ViewID == photonID)
			{
				result = playerAvatar;
			}
		}
		return result;
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0005BFE0 File Offset: 0x0005A1E0
	public static PlayerAvatar PlayerAvatarGetFromSteamID(string _steamID)
	{
		PlayerAvatar result = null;
		if (SemiFunc.IsMultiplayer())
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (playerAvatar.steamID == _steamID)
				{
					result = playerAvatar;
				}
			}
		}
		if (!SemiFunc.IsMultiplayer())
		{
			result = PlayerAvatar.instance;
		}
		return result;
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0005C058 File Offset: 0x0005A258
	public static PlayerAvatar PlayerAvatarGetFromSteamIDshort(int _steamIDshort)
	{
		PlayerAvatar result = null;
		if (SemiFunc.IsMultiplayer())
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (playerAvatar.steamIDshort == _steamIDshort)
				{
					result = playerAvatar;
				}
			}
		}
		if (!SemiFunc.IsMultiplayer())
		{
			result = PlayerAvatar.instance;
		}
		return result;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0005C0CC File Offset: 0x0005A2CC
	public static PlayerAvatar PlayerAvatarLocal()
	{
		return PlayerAvatar.instance;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0005C0D3 File Offset: 0x0005A2D3
	public static string PlayerGetName(PlayerAvatar player)
	{
		if (SemiFunc.IsMultiplayer())
		{
			return player.photonView.Owner.NickName;
		}
		return SteamClient.Name;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0005C0F2 File Offset: 0x0005A2F2
	public static string PlayerGetSteamID(PlayerAvatar player)
	{
		return player.steamID;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0005C0FA File Offset: 0x0005A2FA
	public static void TruckPopulateItemVolumes()
	{
		PunManager.instance.TruckPopulateItemVolumes();
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0005C106 File Offset: 0x0005A306
	public static void LevelSuccessful()
	{
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0005C108 File Offset: 0x0005A308
	public static Quaternion SpringQuaternionGet(SpringQuaternion _attributes, Quaternion _targetRotation, float _deltaTime = -1f)
	{
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		if (!_attributes.setup)
		{
			_attributes.lastRotation = _targetRotation;
			_attributes.setup = true;
		}
		if (float.IsNaN(_attributes.springVelocity.x))
		{
			_attributes.springVelocity = Vector3.zero;
			_attributes.lastRotation = _targetRotation;
		}
		_targetRotation = Quaternion.RotateTowards(_attributes.lastRotation, _targetRotation, 360f);
		Quaternion quaternion = _targetRotation;
		Quaternion currentX = _attributes.lastRotation * SemiFunc.Conjugate(quaternion);
		Vector3 zero = Vector3.zero;
		Quaternion lhs;
		Vector3 a;
		SemiFunc.DampedSpringGeneralSolution(out lhs, out a, currentX, _attributes.springVelocity - zero, _deltaTime, _attributes.damping, _attributes.speed);
		float magnitude = a.magnitude;
		if (magnitude * Time.deltaTime > 3.1415927f)
		{
			a *= 3.1415927f / magnitude;
		}
		_attributes.springVelocity = a + zero;
		_attributes.lastRotation = lhs * quaternion;
		if (_attributes.clamp && Quaternion.Angle(_attributes.lastRotation, _targetRotation) > _attributes.maxAngle)
		{
			_attributes.lastRotation = Quaternion.RotateTowards(_targetRotation, _attributes.lastRotation, _attributes.maxAngle);
		}
		return _attributes.lastRotation;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0005C22C File Offset: 0x0005A42C
	public static float SpringFloatGet(SpringFloat _attributes, float _targetFloat, float _deltaTime = -1f)
	{
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		float currentX = _attributes.lastPosition - _targetFloat;
		float num;
		float springVelocity;
		SemiFunc.DampedSpringGeneralSolution(out num, out springVelocity, currentX, _attributes.springVelocity, _deltaTime, _attributes.damping, _attributes.speed);
		float num2 = num;
		_attributes.springVelocity = springVelocity;
		_attributes.lastPosition = _targetFloat + num2;
		if (_attributes.clamp)
		{
			float lastPosition = _attributes.lastPosition;
			_attributes.lastPosition = Mathf.Clamp(_attributes.lastPosition, _attributes.min, _attributes.max);
			if (lastPosition != _attributes.lastPosition)
			{
				_attributes.springVelocity *= -1f;
			}
		}
		return _attributes.lastPosition;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0005C2D0 File Offset: 0x0005A4D0
	public static Vector3 SpringVector3Get(SpringVector3 _attributes, Vector3 _targetPosition, float _deltaTime = -1f)
	{
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		Vector3 vector = _attributes.lastPosition - _targetPosition;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			float value;
			float value2;
			SemiFunc.DampedSpringGeneralSolution(out value, out value2, vector[i], _attributes.springVelocity[i], _deltaTime, _attributes.damping, _attributes.speed);
			zero[i] = value;
			_attributes.springVelocity[i] = value2;
		}
		_attributes.lastPosition = _targetPosition + zero;
		if (_attributes.clamp && Vector3.Distance(_attributes.lastPosition, _targetPosition) > _attributes.maxDistance)
		{
			_attributes.lastPosition = _targetPosition + (_attributes.lastPosition - _targetPosition).normalized * _attributes.maxDistance;
		}
		return _targetPosition + zero;
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0005C3AC File Offset: 0x0005A5AC
	public static void DampedSpringGeneralSolution(out float _newX, out float _newV, float _currentX, float _currentV, float _time, float _criticality, float _naturalFrequency)
	{
		if (_criticality < 0f)
		{
			_criticality = 0f;
		}
		if (_naturalFrequency <= 0f)
		{
			_naturalFrequency = 1f;
		}
		if (_criticality == 1f)
		{
			float num = _naturalFrequency * _time;
			float num2 = Mathf.Exp(-num);
			float num3 = _currentV + _naturalFrequency * _currentX;
			_newX = num2 * (_currentX + num3 * _time);
			_newV = num2 * (num3 * (1f - num) - _naturalFrequency * _currentX);
			return;
		}
		if (_criticality < 1f)
		{
			float num4 = _naturalFrequency * Mathf.Sqrt(1f - _criticality * _criticality);
			float num5 = _criticality * _naturalFrequency;
			float num6 = 1f / num4 * (num5 * _currentX + _currentV);
			float num7 = Mathf.Exp(-num5 * _time);
			float f = num4 * _time;
			float num8 = Mathf.Cos(f);
			float num9 = Mathf.Sin(f);
			_newX = num7 * (_currentX * num8 + num6 * num9);
			_newV = num7 * (num8 * (num6 * num4 - num5 * _currentX) - num9 * (_currentX * num4 + num6 * num5));
			return;
		}
		float num10 = Mathf.Sqrt(_criticality * _criticality - 1f);
		float num11 = _naturalFrequency * (num10 - _criticality);
		float num12 = -_naturalFrequency * (num10 + _criticality);
		float num13 = (num11 * _currentX - _currentV) / (num11 - num12);
		float num14 = _currentX - num13;
		float num15 = Mathf.Exp(num11 * _time);
		float num16 = Mathf.Exp(num12 * _time);
		float num17 = num14 * num15;
		float num18 = num13 * num16;
		_newX = num17 + num18;
		_newV = num11 * num17 + num12 * num18;
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0005C518 File Offset: 0x0005A718
	public static void DampedSpringGeneralSolution(out Quaternion _newX, out Vector3 _newV, Quaternion _currentX, Vector3 _currentV, float _time, float _criticality, float _naturalFrequency)
	{
		if (_criticality < 0f)
		{
			_criticality = 0f;
		}
		if (_naturalFrequency <= 0f)
		{
			_naturalFrequency = 1f;
		}
		if (_criticality == 1f)
		{
			float num = _naturalFrequency * _time;
			float num2 = Mathf.Exp(-num);
			Vector3 vector = _currentV + SemiFunc.ToAngularVelocity(_currentX, 1f / _naturalFrequency);
			_newX = SemiFunc.QuaternionScale(SemiFunc.ToQuaternionFromAngularVelocityAndTime(vector, _time) * _currentX, num2);
			_newV = num2 * (vector * (1f - num) - SemiFunc.ToAngularVelocity(_currentX, 1f / _naturalFrequency));
			return;
		}
		if (_criticality < 1f)
		{
			float num3 = _naturalFrequency * Mathf.Sqrt(1f - _criticality * _criticality);
			float num4 = _criticality * _naturalFrequency;
			Vector3 vector2 = 1f / num3 * (SemiFunc.ToAngularVelocity(_currentX, 1f / num4) + _currentV);
			float num5 = Mathf.Exp(-num4 * _time);
			float f = num3 * _time;
			float num6 = Mathf.Cos(f);
			float num7 = Mathf.Sin(f);
			_newX = SemiFunc.QuaternionScale(SemiFunc.ToQuaternionFromAngularVelocityAndTime(vector2, num7) * SemiFunc.QuaternionScale(_currentX, num6), num5);
			_newV = num5 * (num6 * (vector2 * num3 - SemiFunc.ToAngularVelocity(_currentX, 1f / num4)) - num7 * (SemiFunc.ToAngularVelocity(_currentX, 1f / num3) + vector2 * num4));
			return;
		}
		float num8 = Mathf.Sqrt(_criticality * _criticality - 1f);
		float num9 = _naturalFrequency * (num8 - _criticality);
		float num10 = -_naturalFrequency * (num8 + _criticality);
		Vector3 vector3 = (SemiFunc.ToAngularVelocity(_currentX, 1f / num9) - _currentV) / (num9 - num10);
		Quaternion quaternion = _currentX * SemiFunc.Conjugate(SemiFunc.ToQuaternionFromAngularVelocityAndTime(vector3, 1f));
		float num11 = Mathf.Exp(num9 * _time);
		float num12 = Mathf.Exp(num10 * _time);
		Quaternion rhs = SemiFunc.QuaternionScale(quaternion, num11);
		Vector3 a = SemiFunc.ToAngularVelocity(quaternion, 1f / num11);
		Vector3 a2 = vector3 * num12;
		Quaternion lhs = SemiFunc.ToQuaternionFromAngularVelocityAndTime(vector3, num12);
		_newX = lhs * rhs;
		_newV = num9 * a + num10 * a2;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0005C77C File Offset: 0x0005A97C
	public static Vector3 ToAngularVelocity(Quaternion _dQ, float _dT)
	{
		float num;
		Vector3 a;
		SemiFunc.ToAngleAndAxis(out num, out a, _dQ);
		return num / _dT * a;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0005C79C File Offset: 0x0005A99C
	public static void ToAngleAndAxis(out float _angleRadians, out Vector3 _axis, Quaternion _Q)
	{
		float num = Mathf.Sqrt(Quaternion.Dot(_Q, _Q));
		_Q.x /= num;
		_Q.y /= num;
		_Q.z /= num;
		_Q.w /= num;
		_axis = new Vector3(_Q.x, _Q.y, _Q.z);
		float magnitude = _axis.magnitude;
		if (Mathf.Abs(_Q.w) <= 0.99f)
		{
			_angleRadians = 2f * Mathf.Acos(_Q.w);
			_axis /= magnitude;
			return;
		}
		_angleRadians = 2f * Mathf.Asin(magnitude);
		if (magnitude == 0f)
		{
			_axis = new Vector3(1f, 0f, 0f);
			return;
		}
		_axis /= magnitude;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0005C885 File Offset: 0x0005AA85
	public static Quaternion Conjugate(Quaternion q)
	{
		return new Quaternion(-q.x, -q.y, -q.z, q.w);
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0005C8A8 File Offset: 0x0005AAA8
	public static Quaternion QuaternionScale(Quaternion _Q, float _power)
	{
		float num;
		Vector3 axis;
		SemiFunc.ToAngleAndAxis(out num, out axis, _Q);
		return SemiFunc.ToQuaternion(num * _power, axis);
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0005C8C8 File Offset: 0x0005AAC8
	public static Quaternion ToQuaternion(float _angleRadians, Vector3 _axis)
	{
		Vector3 normalized = _axis.normalized;
		float num = Mathf.Sin(_angleRadians * 0.5f);
		return new Quaternion(normalized.x * num, normalized.y * num, normalized.z * num, Mathf.Cos(_angleRadians * 0.5f));
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0005C914 File Offset: 0x0005AB14
	public static Quaternion ToQuaternionFromAngularVelocityAndTime(Vector3 _omega, float _time)
	{
		float num = _omega.magnitude * _time;
		if (Mathf.Abs(num) > 1E-15f)
		{
			Vector3 normalized = _omega.normalized;
			float num2 = Mathf.Sin(num * 0.5f);
			return new Quaternion(normalized.x * num2, normalized.y * num2, normalized.z * num2, Mathf.Cos(num * 0.5f));
		}
		return Quaternion.identity;
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0005C97C File Offset: 0x0005AB7C
	public static void Log(object message, GameObject gameObject, Color? color = null)
	{
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0005C980 File Offset: 0x0005AB80
	public static void DoNotLookEffect(GameObject _gameObject, bool _vignette = true, bool _zoom = true, bool _saturation = true, bool _contrast = true, bool _shake = true, bool _glitch = true)
	{
		float speedIn = 3f;
		float speedOut = 1f;
		if (_vignette)
		{
			PostProcessing.Instance.VignetteOverride(new Color(0.16f, 0.2f, 0.26f), 0.5f, 1f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_zoom)
		{
			CameraZoom.Instance.OverrideZoomSet(65f, 0.1f, speedIn, speedOut, _gameObject, 150);
		}
		if (_saturation)
		{
			PostProcessing.Instance.SaturationOverride(-25f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_contrast)
		{
			PostProcessing.Instance.ContrastOverride(10f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_shake)
		{
			GameDirector.instance.CameraImpact.Shake(15f * Time.deltaTime, 0.1f);
			GameDirector.instance.CameraShake.Shake(15f * Time.deltaTime, 1f);
		}
		if (_glitch)
		{
			CameraGlitch.Instance.DoNotLookEffectSet();
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0005CA70 File Offset: 0x0005AC70
	public static void CameraOverrideStopAim()
	{
		CameraAim.Instance.OverrideAimStop();
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0005CA7C File Offset: 0x0005AC7C
	public static ExtractionPoint ExtractionPointGetNearest(Vector3 position)
	{
		ExtractionPoint result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject gameObject in RoundDirector.instance.extractionPointList)
		{
			float num2 = Vector3.Distance(position, gameObject.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = gameObject.GetComponent<ExtractionPoint>();
			}
		}
		return result;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0005CAF8 File Offset: 0x0005ACF8
	public static ExtractionPoint ExtractionPointGetNearestNotActivated(Vector3 position)
	{
		ExtractionPoint result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject gameObject in RoundDirector.instance.extractionPointList)
		{
			if (gameObject.GetComponent<ExtractionPoint>().currentState == ExtractionPoint.State.Idle)
			{
				float num2 = Vector3.Distance(position, gameObject.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = gameObject.GetComponent<ExtractionPoint>();
				}
			}
		}
		return result;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0005CB84 File Offset: 0x0005AD84
	public static float Remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
	{
		float t = Mathf.InverseLerp(origFrom, origTo, value);
		return Mathf.Lerp(targetFrom, targetTo, t);
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0005CBA3 File Offset: 0x0005ADA3
	public static bool InputDown(InputKey key)
	{
		if (Application.isEditor && (key == InputKey.Back || key == InputKey.Menu))
		{
			key = InputKey.BackEditor;
		}
		return InputManager.instance.KeyDown(key);
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0005CBC4 File Offset: 0x0005ADC4
	public static bool InputUp(InputKey key)
	{
		return InputManager.instance.KeyUp(key);
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0005CBD1 File Offset: 0x0005ADD1
	public static bool InputHold(InputKey key)
	{
		return InputManager.instance.KeyHold(key);
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0005CBDE File Offset: 0x0005ADDE
	public static Vector2 InputMousePosition()
	{
		return InputManager.instance.GetMousePosition();
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0005CBEA File Offset: 0x0005ADEA
	public static Vector2 InputMovement()
	{
		return InputManager.instance.GetMovement();
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0005CBF6 File Offset: 0x0005ADF6
	public static float InputMovementX()
	{
		return InputManager.instance.GetMovementX();
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0005CC02 File Offset: 0x0005AE02
	public static float InputMovementY()
	{
		return InputManager.instance.GetMovementY();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0005CC0E File Offset: 0x0005AE0E
	public static float InputScrollY()
	{
		return InputManager.instance.GetScrollY();
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0005CC1A File Offset: 0x0005AE1A
	public static float InputMouseX()
	{
		return InputManager.instance.GetMouseX();
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0005CC26 File Offset: 0x0005AE26
	public static float InputMouseY()
	{
		return InputManager.instance.GetMouseY();
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0005CC32 File Offset: 0x0005AE32
	public static void InputDisableMovement()
	{
		InputManager.instance.DisableMovement();
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0005CC3E File Offset: 0x0005AE3E
	public static void InputDisableAiming()
	{
		InputManager.instance.DisableAiming();
	}

	// Token: 0x02000337 RID: 823
	public enum emojiIcon
	{
		// Token: 0x04002671 RID: 9841
		drone_heal,
		// Token: 0x04002672 RID: 9842
		drone_zero_gravity,
		// Token: 0x04002673 RID: 9843
		drone_indestructible,
		// Token: 0x04002674 RID: 9844
		drone_feather,
		// Token: 0x04002675 RID: 9845
		drone_torque,
		// Token: 0x04002676 RID: 9846
		drone_battery,
		// Token: 0x04002677 RID: 9847
		orb_heal,
		// Token: 0x04002678 RID: 9848
		orb_zero_gravity,
		// Token: 0x04002679 RID: 9849
		orb_indestructible,
		// Token: 0x0400267A RID: 9850
		orb_feather,
		// Token: 0x0400267B RID: 9851
		orb_torque,
		// Token: 0x0400267C RID: 9852
		orb_battery,
		// Token: 0x0400267D RID: 9853
		orb_magnet,
		// Token: 0x0400267E RID: 9854
		grenade_explosive,
		// Token: 0x0400267F RID: 9855
		grenade_stun,
		// Token: 0x04002680 RID: 9856
		weapon_baseball_bat,
		// Token: 0x04002681 RID: 9857
		weapon_sledgehammer,
		// Token: 0x04002682 RID: 9858
		weapon_frying_pan,
		// Token: 0x04002683 RID: 9859
		weapon_sword,
		// Token: 0x04002684 RID: 9860
		weapon_inflatable_hammer,
		// Token: 0x04002685 RID: 9861
		item_health_pack_S,
		// Token: 0x04002686 RID: 9862
		item_health_pack_M,
		// Token: 0x04002687 RID: 9863
		item_health_pack_L,
		// Token: 0x04002688 RID: 9864
		item_gun_handgun,
		// Token: 0x04002689 RID: 9865
		item_gun_shotgun,
		// Token: 0x0400268A RID: 9866
		item_gun_tranq,
		// Token: 0x0400268B RID: 9867
		item_valuable_tracker,
		// Token: 0x0400268C RID: 9868
		item_extraction_tracker,
		// Token: 0x0400268D RID: 9869
		item_grenade_human,
		// Token: 0x0400268E RID: 9870
		item_grenade_duct_taped,
		// Token: 0x0400268F RID: 9871
		item_rubber_duck,
		// Token: 0x04002690 RID: 9872
		item_mine_explosive,
		// Token: 0x04002691 RID: 9873
		item_grenade_shockwave,
		// Token: 0x04002692 RID: 9874
		item_mine_shockwave,
		// Token: 0x04002693 RID: 9875
		item_mine_stun
	}

	// Token: 0x02000338 RID: 824
	public enum itemVolume
	{
		// Token: 0x04002695 RID: 9877
		small,
		// Token: 0x04002696 RID: 9878
		medium,
		// Token: 0x04002697 RID: 9879
		large,
		// Token: 0x04002698 RID: 9880
		large_wide,
		// Token: 0x04002699 RID: 9881
		power_crystal,
		// Token: 0x0400269A RID: 9882
		large_high,
		// Token: 0x0400269B RID: 9883
		upgrade,
		// Token: 0x0400269C RID: 9884
		healthPack,
		// Token: 0x0400269D RID: 9885
		large_plus
	}

	// Token: 0x02000339 RID: 825
	public enum itemType
	{
		// Token: 0x0400269F RID: 9887
		drone,
		// Token: 0x040026A0 RID: 9888
		orb,
		// Token: 0x040026A1 RID: 9889
		cart,
		// Token: 0x040026A2 RID: 9890
		item_upgrade,
		// Token: 0x040026A3 RID: 9891
		player_upgrade,
		// Token: 0x040026A4 RID: 9892
		power_crystal,
		// Token: 0x040026A5 RID: 9893
		grenade,
		// Token: 0x040026A6 RID: 9894
		melee,
		// Token: 0x040026A7 RID: 9895
		healthPack,
		// Token: 0x040026A8 RID: 9896
		gun,
		// Token: 0x040026A9 RID: 9897
		tracker,
		// Token: 0x040026AA RID: 9898
		mine,
		// Token: 0x040026AB RID: 9899
		pocket_cart
	}

	// Token: 0x0200033A RID: 826
	public enum itemSecretShopType
	{
		// Token: 0x040026AD RID: 9901
		none,
		// Token: 0x040026AE RID: 9902
		shop_attic
	}

	// Token: 0x0200033B RID: 827
	public enum User
	{
		// Token: 0x040026B0 RID: 9904
		Walter,
		// Token: 0x040026B1 RID: 9905
		Axel,
		// Token: 0x040026B2 RID: 9906
		Robin,
		// Token: 0x040026B3 RID: 9907
		Jannek,
		// Token: 0x040026B4 RID: 9908
		Ruben,
		// Token: 0x040026B5 RID: 9909
		Builder
	}
}
