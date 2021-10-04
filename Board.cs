using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public GameObject cellPrefab;
    public Transform refRefillBoard;
    public Transform refBoard;
    public List<Cell> cells;
    public List<Transform> refCells;
    public List<Transform> refRefillCells;
    public static int currentStreak;
    public static bool isMatching;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cells = GetComponentsInChildren<Cell>().ToList();

        if (refBoard)
        {
            refCells = new List<Transform>();
            foreach (Transform child in refBoard)
                refCells.Add(child);
        }

        if (refRefillBoard)
        {
            refRefillCells = new List<Transform>();
            foreach (Transform child in refRefillBoard)
                refRefillCells.Add(child);
        }
    }

    public void InitializeCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].onEndDragAction += OnCellEndDrag;
            cells[i].pos = new Vector2(i % 7, i / 7);
        }
    }

    public void OnCellEndDrag(Cell cell, Vector2 swapDir)
    {
        StartCoroutine(Swap(cell, swapDir));
    }

    IEnumerator Swap(Cell cell, Vector2 swapDir)
    {
        Direction dir = 0;

        Debug.Log(swapDir);
        if (Mathf.Abs(swapDir.x) > Mathf.Abs(swapDir.y))
        {
            if (swapDir.x > 0)
                dir = Direction.Right;
            else
                dir = Direction.Left;
        }
        else
        {
            if (swapDir.y > 0)
                dir = Direction.Up;
            else
                dir = Direction.Down;
        }

        Cell swapCell = null;

        switch (dir)
        {
            case Direction.Up:
                // swapCell = cells.Find(c => c.pos.y == cell.pos.y - 1 && c.pos.x == cell.pos.x);
                swapCell = cell.UpCell;
                break;
            case Direction.Down:
                // swapCell = cells.Find(c => c.pos.y == cell.pos.y + 1 && c.pos.x == cell.pos.x);
                swapCell = cell.DownCell;
                break;
            case Direction.Left:
                // swapCell = cells.Find(c => c.pos.x == cell.pos.x - 1 && c.pos.y == cell.pos.y);
                swapCell = cell.LeftCell;
                break;
            case Direction.Right:
                // swapCell = cells.Find(c => c.pos.x == cell.pos.x + 1 && c.pos.y == cell.pos.y);
                swapCell = cell.RightCell;
                break;
        }

        if (!swapCell) yield break;

        SwapAnimation(cell, swapCell);
        yield return new WaitForSeconds(0.2f);
        currentStreak = 0;

        if (!DetectMatches(new List<Cell> { cell, swapCell }))
            SwapAnimation(cell, swapCell);

    }

    public bool DetectMatches(List<Cell> targetDetectCells)
    {
        List<List<Cell>> matchedGroups = MatchManager.GetMatchedGroups(targetDetectCells);
        if (matchedGroups.Count >= 1)
        {
            foreach (List<Cell> group in matchedGroups)
            {
                currentStreak++;
                Debug.Log("Found matched group!");
                foreach (Cell c in group)
                {
                    // Destroy(c.gameObject);
                    // Debug.Log(c.pos);
                    StartCoroutine(c.Destroy());
                }
                GameManager.Instance.AddScore(group.Count);
            }
            canvasGroup.blocksRaycasts = false;
            isMatching = true;
            StartCoroutine(MoveDownCells());
            return true;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            isMatching = false;
            return false;
        }
    }

    IEnumerator MoveDownCells()
    {
        List<Tuple<int, int>> emptyTuples = new List<Tuple<int, int>>();
        for (int i = 0; i < 7; i++)
        {
            int emptyCnt = 0;
            for (int j = 6; j >= 0; j--)
            {
                Cell cell = cells.Find(c => c.pos.x == i && c.pos.y == j);
                if (!cell)
                {
                    emptyCnt++;
                    continue;
                }

                while (!cell.DownCell && cell.pos.y < 6)
                {
                    cell.pos.y++;
                }
            }
            emptyTuples.Add(Tuple.Create(i, emptyCnt));
        }

        foreach (Tuple<int, int> t in emptyTuples)
        {
            for (int i = 0; i < t.Item2; i++)
            {
                int refId = t.Item1 + (t.Item2 - i - 1) * 7;
                int refRefillId = t.Item1 + (6 - i) * 7;
                Cell c = Instantiate(cellPrefab, refRefillCells[refRefillId].transform.position, Quaternion.identity, transform).GetComponent<Cell>();
                c.board = this;
                c.pos = new Vector2(t.Item1, t.Item2 - i - 1);
                c.cellType = UnityEngine.Random.Range(0, LevelManager.Instance.availableSprites.Count);
                c.image.sprite = LevelManager.Instance.availableSprites[c.cellType];
                c.onEndDragAction += OnCellEndDrag;
                cells.Add(c);
            }
        }

        yield return new WaitForSeconds(0.3f);
        MoveAllCellsToCorrectPos();

        yield return new WaitForSeconds(0.3f);

        // canvasGroup.blocksRaycasts = true;
        DetectMatches(cells);
    }

    void MoveAllCellsToCorrectPos()
    {
        foreach (Cell c in cells)
        {
            int refId = (int)(c.pos.x + c.pos.y * 7);
            // Debug.Log($"{c.pos} move to refCell[{refId}], pos: {refCells[refId].transform.localPosition}");
            GameManager.RegisterTween(c.gameObject, c.transform.DOLocalMove(refCells[refId].transform.localPosition, 0.3f).SetEase(Ease.InOutCubic));
        }
    }

    void SwapAnimation(Cell cellA, Cell cellB)
    {
        Vector2 tmpLocalPos = cellA.transform.localPosition;
        Vector2 tmpPos = cellA.pos;
        cellA.pos = cellB.pos;
        GameManager.RegisterTween(cellA.gameObject, cellA.transform.DOLocalMove(cellB.transform.localPosition, 0.2f).SetEase(Ease.InOutSine));
        cellB.pos = tmpPos;
        GameManager.RegisterTween(cellB.gameObject, cellB.transform.DOLocalMove(tmpLocalPos, 0.2f).SetEase(Ease.InOutSine));
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
