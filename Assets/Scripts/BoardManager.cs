using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    public WallObject WallPrefab;
    public FoodObject FoodPrefab;

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_EmptyCellsList = new List<Vector2Int>();
        // 보드 크기만큼 CellData 배열 생성
        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                // 현재 좌표에 배치할 타일
                Tile tile;

                // 각 칸마다 CellData 객체 생성
                m_BoardData[x, y] = new CellData();

                // 맵의 가장자리(테두리)인지 검사
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    // 벽 타일 중 하나를 랜덤으로 선택
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];

                    // 해당 칸은 이동 불가
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    // 바닥 타일 중 하나를 랜덤으로 선택
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];

                    // 해당 칸은 이동 불가
                    m_BoardData[x, y].Passable = true;

                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                // Tilemap 의 (x, y) 위치에 타일 배치
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);

            }
        }
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
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

            //init the wall
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
}
