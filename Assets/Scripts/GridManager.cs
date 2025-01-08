using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float[] thresholds;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float NoiseScale = 0.1f;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * NoiseScale, y * NoiseScale);
                Debug.Log($"Noise value at ({x}, {y}): {noiseValue}");

                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (noiseValue < thresholds[i])
                    {
                        Instantiate(prefabs[i], new Vector3(x, 0, y), Quaternion.identity).transform.SetParent(transform);
                        break;
                    }
                }
            }
        }
    }
}




