using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Cell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject psPrefab;
    public Board board;
    public Vector2 pos;
    public int cellType;
    public Color32 color
    {
        get { return color; }
        set { image.color = value; }
    }
    public Image image;
    public Action<Cell, Vector2> onEndDragAction;

    Vector2 beginDragPos;

    public Cell LeftCell => board.cells.Find(c => c.pos.x == pos.x - 1 && c.pos.y == pos.y);
    public Cell RightCell => board.cells.Find(c => c.pos.x == pos.x + 1 && c.pos.y == pos.y);
    public Cell UpCell => board.cells.Find(c => c.pos.y == pos.y - 1 && c.pos.x == pos.x);
    public Cell DownCell => board.cells.Find(c => c.pos.y == pos.y + 1 && c.pos.x == pos.x);

    void Awake()
    {
        image = GetComponent<Image>();
        board = GetComponentInParent<Board>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");
        beginDragPos = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");
        Vector2 dir = (Vector2)Input.mousePosition - beginDragPos;
        onEndDragAction.Invoke(this, dir);
    }

    public IEnumerator Destroy()
    {
        board.cells.Remove(this);
        image.DOFade(0, 0.3f);
        GameObject go = Instantiate(psPrefab, transform.position, Quaternion.identity, transform.parent);
        go.GetComponent<AudioSource>().pitch = 1 + (Board.currentStreak * 0.02f);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
