using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;

    [Header("Board Settings")]
    [SerializeField] private int Width;
    [SerializeField] private int Height;
    [SerializeField] private Tile[] GroundTiles;
    [SerializeField] private Tile[] WallTiles;
    [SerializeField] private ExitCellObject ExitCellPrefab;

    private List<Vector2Int> m_EmptyCellsList;

    [Header("Prefabs")]
    [SerializeField] private FoodObject[] FoodPrefab;
    [SerializeField] private WallObject[] WallPrefab;
    [SerializeField] private Enemy[] EnemyPrefab;

    public int EnemyCount = 3;

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[Width, Height];

        // 보드 생성
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        // 플레이어 시작 위치 제외
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        // 출구 배치
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    private void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];

        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    private void GenerateFood()
    {
        int foodCount = 5;

        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            int randomFoodIndex = Random.Range(0, FoodPrefab.Length);
            FoodObject newFood = Instantiate(FoodPrefab[randomFoodIndex]);
            AddObject(newFood, coord);
        }
    }

    private void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);

        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            int randomWallIndex = Random.Range(0, WallPrefab.Length);
            WallObject newWall = Instantiate(WallPrefab[randomWallIndex]);
            AddObject(newWall, coord);
        }
    }

    public void Clean()
    {
        if (m_BoardData == null) return;

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

    private void GenerateEnemy()
    {
        if (EnemyPrefab == null || EnemyPrefab.Length == 0)
        {
            Debug.LogWarning("[BoardManager] EnemyPrefabs가 비어있음");
            return;
        }

        int count = Mathf.Min(EnemyCount, m_EmptyCellsList.Count);

        for (int i = 0; i < count; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);

            int prefabIndex = Random.Range(0, EnemyPrefab.Length);
            Enemy prefab = EnemyPrefab[prefabIndex];

            if (prefab == null)
            {
                i--;
                continue;
            }

            Enemy enemy = Instantiate(prefab);

            AddObject(enemy, coord);
        }
    }
}
