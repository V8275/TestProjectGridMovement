using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Vector2Int startLocation;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float moveDelay = 0.5f;
    [SerializeField] private float yPlayerOffset = 0.5f;
    [SerializeField] private string GroundTag = "Ground";

    private GridManager gridManager;
    private Vector2Int targetLocation;
    private int[,] grid;
    private Coroutine calculate;
    private bool move = false;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            var json = PlayerPrefs.GetString("Save");
            Vector2IntSerializable deserializedVector = JsonUtility.FromJson<Vector2IntSerializable>(json);
            startLocation = deserializedVector.ToVector2Int();
        }

        gridManager = FindObjectOfType<GridManager>();
        if (!playerObject) playerObject = gameObject;
        playerObject.transform.position = new Vector3(startLocation.x, yPlayerOffset, startLocation.y);
        grid = gridManager.GetGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !move)
        {
            try
            {
                move = true;
                if(calculate != null)
                StopCoroutine(calculate);
                targetLocation = GetMouseGridPosition();
                calculate = StartCoroutine(MoveAlongPath());
            }
            catch (Exception)
            {
                move = false;
                if(calculate !=null)
                StopCoroutine(calculate);
            }
        }
    }

    private IEnumerator MoveAlongPath()
    {
        var path = GridAStar.FindPath(grid, startLocation, targetLocation);

        if (path != null)
        {
            foreach (var node in path)
            {
                Vector3 targetPosition = new Vector3(node.X, playerObject.transform.position.y, node.Y);
                while (Vector3.Distance(playerObject.transform.position, targetPosition) > 0.1f)
                {
                    playerObject.transform.position = 
                        Vector3.MoveTowards(playerObject.transform.position, targetPosition, Time.deltaTime * 5f);
                    yield return null;
                }

                yield return new WaitForSeconds(moveDelay);
            }
            move = false;
            startLocation = targetLocation;

            Vector2IntSerializable serializableVector = new Vector2IntSerializable(startLocation);
            string json = JsonUtility.ToJson(serializableVector);
            PlayerPrefs.SetString("Save", json);
        }
        else
        {
            Debug.Log("Path not found!");
        }
    }

    private Vector2Int GetMouseGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag(GroundTag))
            {
                int x = Mathf.FloorToInt(hit.point.x);
                int y = Mathf.FloorToInt(hit.point.z);
                return new Vector2Int(x, y);
            }
            //Debug.Log($"Hit: {hit.collider.name}");
            else throw new ArgumentException("This cant be target");
        }
        return Vector2Int.zero;
    }
}
