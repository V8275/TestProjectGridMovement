using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float[] thresholds;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float NoiseScale = 0.1f;
    [SerializeField] private string[] tags;

    private int[,] gridInt;

    void Start()
    {
        gridInt = new int[gridWidth, gridHeight];
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * NoiseScale, y * NoiseScale);
                //Debug.Log($"Noise value at ({x}, {y}): {noiseValue}");

                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (noiseValue < thresholds[i])
                    {
                        var cell = Instantiate(prefabs[i], new Vector3(x, 0, y), Quaternion.identity);
                        cell.name = $"Cell_{x}_{y}";
                        cell.transform.SetParent(transform);

                        for (int k = 0; k < tags.Length; k++)
                        {
                            if (cell.CompareTag(tags[k]))
                            {
                                gridInt[x, y] = 1;
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        gridInt[x, y] = 0;
                    }
                }
            }
        }
        yield return null;
    }

    public Vector2Int Size()
    {
        return new Vector2Int(gridWidth, gridHeight);
    }

    public int[,] GetGrid()
    {
        return gridInt;
    }
}