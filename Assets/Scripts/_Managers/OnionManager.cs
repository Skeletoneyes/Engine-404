using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class OnionManager
{
	public static List<Onion> _OnionsInScene = new();

	public static bool IsAnyOnionActiveInScene => _OnionsInScene.Any(onion => onion.OnionActive);

	public static Onion GetOnionOfColour(PikminColour colour)
	{
		Onion o = _OnionsInScene.FirstOrDefault(onion => onion.Colour == colour);

		return o != null ? o : _OnionsInScene.FirstOrDefault(x => x.OnionActive);
	}

    public static Onion CreateOnion(Vector3 position, GameObject OnionPrefab)
	{
		Onion newOnion = GameObject.Instantiate<GameObject>(OnionPrefab).GetComponent<Onion>();

		newOnion.transform.position = position;

		return newOnion;
    }
}
