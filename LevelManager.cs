using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Board board;
    public List<Sprite> availableSprites;
    public List<Color32> availableColors;
    bool firstStart = true;

    void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public void StartLevel()
    {
        // yield return new WaitForEndOfFrame();
        RandomizeAllCellColors();
        if (firstStart)
        {
            board.InitializeCells();
            firstStart = false;
        }
        board.DetectMatches(board.cells);
    }

    void RandomizeAllCellColors()
    {
        foreach (Cell cell in board.cells)
        {
            cell.cellType = Random.Range(0, availableSprites.Count);
            cell.image.sprite = availableSprites[cell.cellType];
        }
    }
}
