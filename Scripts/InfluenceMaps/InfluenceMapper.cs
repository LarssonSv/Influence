using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfluenceMapper : MonoBehaviour
{
    [SerializeField] private Mesh _debugMesh;
    [SerializeField] private HazardSystem _hazardSystem;
    [SerializeField] private NpcController _npcController;

    private readonly List<InfluenceMap> _maps = new List<InfluenceMap>();
    private readonly List<float[][]> _drawMaps = new List<float[][]>();
    public readonly List<Vector3[][]> DrawPos = new List<Vector3[][]>();

    private BoundingBox _box;
    public static InfluenceMapper MP;

    private void Awake()
    {
        MP = this;

        _hazardSystem.Init();
        _npcController.Init();

      CreateMap();
      
      Invoke("NpcTest", 1f);
        
    }

    private void Update()
    {
        _hazardSystem.OnUpdate();
        _npcController.OnUpdate();
    }

    private void NpcTest()
    {
        _npcController.Npcs[0].transform.position = new Vector3(5f, 1.5f,1f);
        
        Debug.Log(_npcController.Npcs[0].GridPosition);
        _npcController.Npcs[0].Path = InfluenceAStar.GetPath(
            _npcController.Npcs[0].GridPosition, new Vector2Int(5, 20), _drawMaps[0], DrawPos[0]).ToList();

        _npcController.Npcs[0].TargetLocation = _npcController.Npcs[0].transform.position;
    }


    private void CreateMap()
    {
        _box = new BoundingBox(Vector3.zero, 25, 10, 25);

        InfluenceMap Map = new InfluenceMap("NavMap", _box,
            0f, 1f, 0, 0, true);

        _maps.Add(Map);

        InfluenceMap HazardMap = new InfluenceMap("HazardMap", _box,
            0f, 1f, 1, 0, false);

        _maps.Add(HazardMap);


        Vector3 temp = new Vector3(5.5f, 0, 5.3f);
        Vector2Int temp2 = WorldToGrid(temp);

        //GetMapByName("HazardMap").Grid[temp2.x][temp2.y] = 0.7f;
        //DrawCircleOnMap("HazardMap", 10,10,5f,1f);


        //InfluenceMap finalMap = GetMapByName("NavMap");
        //finalMap.Grid = finalMap.Multiply(GetMapByName("HazardMap").Grid);
        //_drawMaps.Add(finalMap);
        float[][] finalGrid = new float[_box.ScaleX][];

        for (int i = 0; i < _box.ScaleX; i++)
        {
            finalGrid[i] = new float[_box.ScaleZ];
        }

        finalGrid = GetMapByName("NavMap").Multiply(GetMapByName("HazardMap").Grid);
        _drawMaps.Add(finalGrid);
        DrawPos.Add(GetMapByName("NavMap").GridPosition);
    }

    public void CalculateMap()
    {
        _drawMaps[0] = GetMapByName("NavMap").Multiply(GetMapByName("HazardMap").Grid);
    }

    public void ResetMap(string mapName)
    {
        GetMapByName(mapName).Reset();
    }

    public void DrawCircleOnMap(string mapName, int x, int z, int radius, float value)
    {
        InfluenceMap map = GetMapByName(mapName);

        for (int i = radius; i >= 0; i--)
        {
            float angle = 0;
            while (angle < 360)
            {
                float newX = x + i * Mathf.Cos(angle);
                float newZ = z + i * Mathf.Sin(angle);

                if (newX > 0 && newX < _box.ScaleX - 1 && newZ > 0 && newZ < _box.ScaleZ - 1)
                    map.Grid[Mathf.RoundToInt(newX)][Mathf.RoundToInt(newZ)] = value * i;
                angle += 10f;
            }
        }
    }

    public void DrawOnMap(string mapName, int x, int z, float value)
    {
        GetMapByName(mapName).Grid[x][z] = value;
    }

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        Vector3 localPos = pos - _box.Min;
        return new Vector2Int(Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.z));
    }

    private InfluenceMap GetMapByName(string name)
    {
        foreach (InfluenceMap map in _maps)
        {
            if (name == map._name)
                return map;
        }

        Debug.Log("Could not find map!");
        return null;
    }


    private void OnDrawGizmos()
    {
        //Draw Map
        Gizmos.color = new Color(0, 1, 0, 0.5F);

        foreach (float[][] drawMap in _drawMaps)
        {
            for (int x = 0; x < _box.ScaleX; x++)
            {
                for (int z = 0; z < _box.ScaleZ; z++)
                {
                    if (drawMap[x][z] > 0.1f)
                    {
                        Gizmos.color = new Color(0, 1, 0, drawMap[x][z] / 2f);
                        Gizmos.DrawMesh(_debugMesh, DrawPos[0][x][z] + new Vector3(0, 0.1f, 0),
                            Quaternion.Euler(90, 0, 0));
                    }
                }
            }
        }


        if (_box == null)
            return;

        //Draw Box
        Gizmos.color = Color.magenta;

        Gizmos.DrawLine(_box.Min, _box.Min + new Vector3(_box.ScaleX, 0f, 0f));
        Gizmos.DrawLine(_box.Min + new Vector3(_box.ScaleX, 0f, 0f),
            _box.Min + new Vector3(_box.ScaleX, 0f, _box.ScaleZ));

        Gizmos.DrawLine(_box.Min, _box.Min + new Vector3(0, 0, _box.ScaleZ));
        Gizmos.DrawLine(_box.Min + new Vector3(0, 0, _box.ScaleZ),
            _box.Min + new Vector3(_box.ScaleX, 0, _box.ScaleZ));

        Gizmos.DrawLine(_box.Max, _box.Max - new Vector3(_box.ScaleX, 0f, 0f));
        Gizmos.DrawLine(_box.Max - new Vector3(_box.ScaleX, 0f, 0f),
            _box.Max - new Vector3(_box.ScaleX, 0f, _box.ScaleZ));

        Gizmos.DrawLine(_box.Max, _box.Max - new Vector3(0, 0, _box.ScaleZ));
        Gizmos.DrawLine(_box.Max - new Vector3(0, 0, _box.ScaleZ),
            _box.Max - new Vector3(_box.ScaleX, 0, _box.ScaleZ));

        Gizmos.DrawLine(_box.Min, _box.Max - new Vector3(_box.ScaleX, 0, _box.ScaleZ));
        Gizmos.DrawLine(_box.Min + new Vector3(_box.ScaleX, 0, 0), _box.Max - new Vector3(0, 0, _box.ScaleZ));
        Gizmos.DrawLine(_box.Min + new Vector3(0, 0, _box.ScaleZ), _box.Max - new Vector3(_box.ScaleX, 0, 0));
        Gizmos.DrawLine(_box.Min + new Vector3(_box.ScaleX, 0, _box.ScaleZ), _box.Max);
    }
}