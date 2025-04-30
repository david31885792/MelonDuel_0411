using UnityEngine;

public class BoundaryBuilder : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public float width = 5f;
    public float height = 5f;
    public float wallThickness = 0.5f;
    public float wallHeight = 1f;
    public bool addFloor = true;

    public PhysicsMaterial wallPhysicsMaterial;
    public bool showWallGizmos = false;          // 💡 시각화 기능 on/off
    public Material testWallMaterial;            // 💡 테스트용 머티리얼 (예: 반투명 빨간색)

    void Start()
    {
        BuildBoundary();
    }

    void BuildBoundary()
    {
        CreateWall("Wall_Left", center + new Vector3(-width / 2f - wallThickness / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, height));
        CreateWall("Wall_Right", center + new Vector3(+width / 2f + wallThickness / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, height));
        CreateWall("Wall_Top", center + new Vector3(0, wallHeight / 2f, +height / 2f + wallThickness / 2f), new Vector3(width + wallThickness * 2f, wallHeight, wallThickness));
        CreateWall("Wall_Bottom", center + new Vector3(0, wallHeight / 2f, -height / 2f - wallThickness / 2f), new Vector3(width + wallThickness * 2f, wallHeight, wallThickness));

        if (addFloor)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.position = center + new Vector3(0, 0, 0);
            floor.transform.localScale = new Vector3(width + wallThickness * 2f, wallThickness, height + wallThickness * 2f);

            BoxCollider col = floor.GetComponent<BoxCollider>();
            if (wallPhysicsMaterial != null && col != null)
                col.material = wallPhysicsMaterial;

            if (showWallGizmos && testWallMaterial != null)
                floor.GetComponent<Renderer>().material = testWallMaterial;
            else
                Destroy(floor.GetComponent<MeshRenderer>());

            floor.transform.SetParent(this.transform);
        }
    }

    void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = position;
        wall.transform.localScale = scale;

        BoxCollider col = wall.GetComponent<BoxCollider>();
        if (wallPhysicsMaterial != null && col != null)
            col.material = wallPhysicsMaterial;

        if (showWallGizmos && testWallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = testWallMaterial;
        }
        else
        {
            Destroy(wall.GetComponent<MeshRenderer>());
        }

        wall.transform.SetParent(this.transform);
    }
}
