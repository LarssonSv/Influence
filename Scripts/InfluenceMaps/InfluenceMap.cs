using UnityEngine;

public class InfluenceMap
{
    public string _name;
    public int _checkLayer;
    public float DefaultValue;
    public float[][] Grid;
    public Vector3[][] GridPosition;

    public BoundingBox Box;

    public InfluenceMap(string name, BoundingBox box, float minValue,
        float maxValue, float defaultValue, int checkLayer, bool navMap = false)
    {
        _name = name;
        Box = box;
        DefaultValue = defaultValue;
        _checkLayer = checkLayer;
        Grid = new float[box.ScaleX][];
        GridPosition = new Vector3[box.ScaleX][];

        for (int i = 0; i < box.ScaleX; i++)
        {
            Grid[i] = new float[box.ScaleX];
            GridPosition[i] = new Vector3[box.ScaleZ];
        }

        for (int x = 0; x < box.ScaleX; x++)
        {
            for (int z = 0; z < box.ScaleZ; z++)
            {
                Grid[x][z] = DefaultValue;

                if (!navMap)
                    continue;

                RaycastHit hit;
                Vector3 rayOrigin = new Vector3(Box.Min.x + x, Box.Max.y, Box.Min.z + z);
                if (Physics.BoxCast(rayOrigin, new Vector3(0.5f, 0.5f, 0.5f), Vector3.down, out hit))
                {
                    if (hit.collider.transform.gameObject.layer == _checkLayer)
                    {
                        RaycastHit centerHit;
                        Physics.Raycast(rayOrigin, Vector3.down, out centerHit, Mathf.Infinity);
                        GridPosition[x][z] = centerHit.point + new Vector3(0.5f, 0, 0.5f);
                        Grid[x][z] = 1f;
                    }
                }
            }
        }
    }

    public float[][] Multiply(float[][] b)
    {
        float[][] newGrid = new float[Box.ScaleX][];
        for (int x = 0; x < Box.ScaleX; x++)
        {
            newGrid[x] = new float[Box.ScaleZ];
            for (int z = 0; z < Box.ScaleZ; z++)
            {
                //Debug.Log(Grid[x][z] + " * " + b[x][z]);
                newGrid[x][z] = this.Grid[x][z] * b[x][z];
            }
        }

        return newGrid;
    }

    public void Reset()
    {
        for (int x = 0; x < Box.ScaleX; x++)
        {
            for (int z = 0; z < Box.ScaleZ; z++)
            {
                Grid[x][z] = DefaultValue;
            }
        }
    }
}