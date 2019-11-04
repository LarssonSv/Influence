using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Npc : MonoBehaviour
{
    public Vector2Int GridPosition => InfluenceMapper.MP.WorldToGrid(transform.position);
    public Vector3 TargetLocation;
    public List<Vector2Int> Path = new List<Vector2Int>();
    public float Speed = 2f;
}
