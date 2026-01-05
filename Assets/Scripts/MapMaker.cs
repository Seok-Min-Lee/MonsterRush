using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    //
    public GameObject[] tilePrefabs;
    public int tileScale = 1;
    public int radius = 1;
    public int initialPoolSize = 9;

    //
    public Transform player;

    //
    private Vector2Int lastTile;
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();

    private void Start()
    {
        Init();
        lastTile = new Vector2Int(
            Mathf.FloorToInt(player.position.x / tileScale),
            Mathf.FloorToInt(player.position.y / tileScale)
        );
        UpdateTiles();
    }

    private void Update()
    {
        Vector2Int currentTile = new Vector2Int(
            Mathf.FloorToInt(player.position.x / tileScale),
            Mathf.FloorToInt(player.position.y / tileScale)
        );

        if (currentTile != lastTile)
        {
            lastTile = currentTile;
            UpdateTiles();
        }
    }

    // 중심 좌표 기준으로 주변 타일 유지
    HashSet<Vector2Int> neededCoords = new HashSet<Vector2Int>();
    List<Vector2Int> removeCoords = new List<Vector2Int>();
    private void UpdateTiles()
    {
        neededCoords.Clear();
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                Vector2Int coord = lastTile + new Vector2Int(x, y);
                neededCoords.Add(coord);

                if (!activeTiles.ContainsKey(coord))
                {
                    Vector3 pos = new Vector3(coord.x * tileScale, coord.y * tileScale, 0f);

                    GameObject tile;
                    if (tilePool.Count > 0)
                    {
                        tile = tilePool.Dequeue();
                        tile.SetActive(true);
                    }
                    else
                    {
                        tile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], transform);
                    }

                    tile.transform.position = pos;
                    activeTiles.Add(coord, tile);
                }
            }
        }

        // 필요 없는 타일 반환
        removeCoords.Clear();
        foreach (KeyValuePair<Vector2Int, GameObject> pair in activeTiles)
        {
            if (!neededCoords.Contains(pair.Key))
            {
                pair.Value.SetActive(false);
                tilePool.Enqueue(pair.Value);

                removeCoords.Add(pair.Key);
            }
        }

        foreach (Vector2Int coord in removeCoords)
        {
            activeTiles.Remove(coord);
        }
    }
    private void Init()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], transform);
            tile.SetActive(false);
            tilePool.Enqueue(tile);
        }
    }
}
