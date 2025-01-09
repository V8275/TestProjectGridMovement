using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GridAStar gridAStar;

    private void Start()
    {
        gridAStar = FindObjectOfType<GridAStar>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ��� ��� ������ ��������� �����
        {
            Vector2Int startLocation = GetMouseGridPosition();
            gridAStar.SetStartLocation(startLocation);
        }

        if (Input.GetMouseButtonDown(1)) // ��� ��� ������ ������� �����
        {
            Vector2Int targetLocation = GetMouseGridPosition();
            gridAStar.SetTargetLocation(targetLocation);
            gridAStar.FindPath();
        }
    }

    private Vector2Int GetMouseGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2); // ������������ ����

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Hit: {hit.collider.name}");
            int x = Mathf.FloorToInt(hit.point.x);
            int y = Mathf.FloorToInt(hit.point.z); // ����������� z ��� 2D ���������
            print(x + " " + y);
            return new Vector2Int(x, y);
        }
        print("0");
        return Vector2Int.zero; // ���������� ������� �������, ���� ������ �� ���� �������
    }
}
