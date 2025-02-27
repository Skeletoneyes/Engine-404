/*
 * MathUtils.cs
 * Created by: Ambrosia
 * Created on: 30/4/2020 (dd/mm/yy)
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtil
{
	public const float M_TAU = Mathf.PI * 2;

	public static float AngleDistance(float angle1, float angle2)
	{
		float angle = RoundAngle(angle1 - angle2);

		if (angle >= Mathf.PI)
		{
			angle = -RoundAngle(M_TAU - angle);
		}

		return angle;
	}

	public static float AngleXZ(Vector3 a, Vector3 b)
	{
		float x = a.x - b.x;
		float z = a.z - b.z;
		return RoundAngle(Mathf.Atan2(x, z));
	}

	/// <summary>
	///   Calculates the normalized direction from a point towards another point
	/// </summary>
	/// <param name="from">The origin</param>
	/// <param name="to">The destination</param>
	/// <param name="useY">If we should use the Y axis vector</param>
	/// <returns>A normalized direction around a unit circle</returns>
	public static Vector3 DirectionFromTo(Vector3 from, Vector3 to, bool useY = false)
	{
		Vector3 direction = to - from;

		// Only use the x and z coordinates if we don't want to use the y axis
		if (!useY)
		{
			direction.y = 0;
		}

		// Return a normalized version of the direction vector
		return Vector3.Normalize(direction);
	}

	/// <summary>
	///   Calculates the normalized direction from a point towards another point with length
	/// </summary>
	/// <param name="from">The origin</param>
	/// <param name="to">The destination</param>
	/// <param name="length"></param>
	/// <param name="useY">If we should use the Y axis vector</param>
	/// <returns>A normalized direction around a unit circle</returns>
	public static Vector3 DirectionFromTo(Vector3 from, Vector3 to, out float length, bool useY = false)
	{
		Vector3 direction = to - from;

		// Only use the x and z coordinates if we don't want to use the y axis
		if (!useY)
		{
			direction.y = 0;
		}

		length = direction.magnitude;

		// Return a normalized version of the direction vector
		return Vector3.Normalize(direction);
	}

	/// <summary>
	///   Calculates the distance between two positions, without the SQRT, making it more efficient than Vector3.Distance
	/// </summary>
	/// <param name="first">The first position</param>
	/// <param name="second">The second position</param>
	/// <param name="useY">Whether or not to use the Y axis in the calculation</param>
	/// <returns>The squared distance between 'first' and 'second'</returns>
	public static float DistanceTo(Vector3 first, Vector3 second, bool useY = true)
	{
		if (!useY)
		{
			// Ignore the Y axis if useY is false.
			first.y = second.y = 0;
		}

		// Use Vector3.SqrMagnitude to calculate the squared distance.
		return Vector3.SqrMagnitude(first - second);
	}

	/// <summary>
	///   A basic square function to allow for an Ease-In LERP
	/// </summary>
	/// <param name="t">The time input to LERP</param>
	/// <returns>The eased time</returns>
	public static float EaseIn2(float t)
	{
		return t * t;
	}

	public static float EaseIn3(float t)
	{
		return t * t * t;
	}

	public static float EaseIn4(float t)
	{
		return t * t * t * t;
	}

	public static float EaseOut2(float t)
	{
		return Flip(Flip(t) * Flip(t));
	}

	public static float EaseOut3(float t)
	{
		return Flip(Flip(t) * Flip(t)) * Flip(Flip(t) * Flip(t));
	}

	public static float EaseOut4(float t)
	{
		return Flip(Flip(t) * Flip(t)) * Flip(Flip(t) * Flip(t)) * Flip(Flip(t) * Flip(t));
	}

	public static Collider GetClosestCollider(Vector3 pos, List<Collider> list, bool useY = true)
	{
		if (list == null || list.Count == 0)
		{
			return null;
		}

		// Find the closest collider
		return list.Find(
			i =>
				DistanceTo(pos, i.transform.position, useY) == list.Min(j => DistanceTo(pos, j.transform.position, useY))
		);
	}

	public static Transform GetClosestTransform(Vector3 pos, List<Transform> list, out int index, bool useY = true)
	{
		if (list == null || list.Count == 0)
		{
			index = -1;
			return null;
		}

		index = list.FindIndex(i => DistanceTo(pos, i.position, useY) == list.Min(i2 => DistanceTo(pos, i2.position, useY)));
		return list[index];
	}

	/// <summary>
	///   Calculates the position of an angle on a Unit circle
	/// </summary>
	/// <param name="angle">The angle on the circle in radians</param>
	/// <returns>-1 to 1, -1 to 1</returns>
	public static Vector2 PositionInUnit(float angle)
	{
		return new(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	/// <summary>
	///   Calculates the position of an angle on a Unit circle
	/// </summary>
	/// <param name="angle">The angle on the circle in radians</param>
	/// <param name="radius">The radius of the circle</param>
	/// <returns>-1 to 1, -1 to 1</returns>
	public static Vector2 PositionInUnit(float angle, float radius)
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
	}

	/// <summary>
	///   Calculates the position of an index on a Unit circle
	/// </summary>
	/// <param name="segments">How many corners there are</param>
	/// <param name="index">The index of the corner to calculate the position of</param>
	/// <returns>-1 to 1, -1 to 1</returns>
	public static Vector2 PositionInUnit(int segments, int index)
	{
		float theta = M_TAU / segments * index;
		return new(Mathf.Cos(theta), Mathf.Sin(theta));
	}

	/// <summary>
	///   Calculates the position of an index on a Unit circle with added offset
	/// </summary>
	/// <param name="segments">How many corners there are</param>
	/// <param name="index">The index of the corner to calculate the position of</param>
	/// <param name="offset">How much to offset the position by</param>
	/// <returns>-1 to 1, -1 to 1</returns>
	public static Vector2 PositionInUnit(int segments, int index, float offset)
	{
		float theta = M_TAU / segments * index;
		return new(Mathf.Cos(theta + offset), Mathf.Sin(theta + offset));
	}

	/// <summary>
	///   Rounds an angle to 0 - 2PI
	/// </summary>
	/// <param name="a"></param>
	/// <returns></returns>
	public static float RoundAngle(float a)
	{
		if (a < 0.0f)
		{
			a += M_TAU;
		}

		if (a >= M_TAU)
		{
			a -= M_TAU;
		}

		return a;
	}

	/// <summary>
	///   Converts between 2D and 3D on the X and Z axis
	/// </summary>
	/// <param name="conv">The vector to convert</param>
	/// <param name="y"></param>
	/// <returns>Vector3 with X and Z set to the X and Y of the Vector2</returns>
	public static Vector3 XZToXYZ(Vector2 conv, float y = 0)
	{
		return new(conv.x, y, conv.y);
	}

	static float Flip(float t)
	{
		return 1 - t;
	}
}
