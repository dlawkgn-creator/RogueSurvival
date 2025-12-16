using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// 게임 보드(Map)를 생성하고 관리하는 클래스
/// </summary>
public class BoardManager : MonoBehaviour
{
    // 각 칸(cell)에 대한 데이터 클래스
    // 타일 위에 캐릭터가 지나갈 수 있는지 여부를 저장
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    // 전체 보드의 셀 데이터를 2차원 배열로 저장
    private CellData[,] m_BoardData;

    // 아직 아무것도 없는 cell 좌표 목록
    private List<Vector2Int> m_EmptyCellsList;

    // 실제 타일을 그려줄 Tilemap 컴포넌트
    private Tilemap m_Tilemap;
    private Grid m_Grid;

    // 플레이어 스폰 및 제어용 컨트롤러
    public PlayerController Player;

    [Header("Board")]
    public int Width = 10;
    public int Height = 10;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    [Header("Prefabs")]
    public WallObject WallPrefab;
    public FoodObject FoodPrefab;
    public ExitCellObject ExitCellPrefab;

    void Awake()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_BoardData = new CellData[Width, Height];
    }

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[Width, Height];

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
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        // 신은지 작업
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);

        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateWall();
        GenerateFood();
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

    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            WallObject newWall = Instantiate(WallPrefab);

            newWall.Init(coord);

            newWall.transform.position = CellToWorld(coord);

            data.ContainedObject = newWall;
        }
    }

    private void GenerateFood()
    {
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            FoodObject newFood = Instantiate(FoodPrefab);
            newFood.transform.position = CellToWorld(coord);
            data.ContainedObject = newFood;
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public void Clean()
    {

        if (m_BoardData == null)
            return;


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

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }
}
