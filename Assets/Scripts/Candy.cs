using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Candy : MonoBehaviour
{
    #region FIELDS
    Vector2 savedPos;
    bool isMoving = false;
    public int xIndex, yIndex;
    public enum CandyColor { 
        Green,
        Orange,
        Purple
    }
    public CandyColor candyColor;
    private Board m_board;
    #endregion

    #region UNITY METHODS
    private void Start() => m_board = FindObjectOfType<Board>();
    #endregion

    #region SETTING PROPERTIES
    public void SetCordinates(int x, int y) {
        xIndex = x;
        yIndex = y;
    }
    #endregion

    #region MOUSE MONOBEHAVIOUR METHODS
    private void OnMouseDown()
    {
        //m_board.test = new Vector2(xIndex, yIndex);
        //m_board.FindMatches(xIndex, yIndex);
        //m_board.ClearCandy(m_board.matches);
        //m_board.ClearLists();
        //m_board.CollapseColumn();

        //Debug.Log(m_board.m_tiles[xIndex, yIndex]);
        if (m_board.startTile == null)
            m_board.startTile = gameObject;
    }
    private void OnMouseEnter()
    {
        if (m_board.startTile != null)
            m_board.endTile = gameObject;
        //Debug.Log("Enter " + gameObject.name);        
    }
    private void OnMouseUp()
    {
        if (m_board.startTile != null && m_board.endTile != null) { 
            m_board.SwapCandy(m_board.startTile, m_board.endTile);
            //Debug.Log("1");

        }


        //if (m_board.startTile.GetComponent<Candy>().candyColor != m_board.endTile.GetComponent<Candy>().candyColor) { 
        //    m_board.SwapCandy(m_board.startTile, m_board.endTile);
        //    Debug.Log("2");
        //}
        //Debug.Log(m_board.startTile.transform.position + " " + m_board.startTile.GetComponent<Candy>().candyColor);
        //Debug.Log(m_board.endTile.transform.position + " " + m_board.endTile.GetComponent<Candy>().candyColor);

        if (m_board.startTile != null)
            m_board.startTile = null;
        if (m_board.endTile != null)
            m_board.endTile = null;

    }
    #endregion

    #region MOVE CANDY
    /// <summary>
    /// Move candy to target position
    /// </summary>    
    public void Move(Vector2 targetPos, float speed) {
        if (!isMoving) { 
            isMoving = true;
            savedPos = targetPos;
            transform.DOMove(targetPos, speed)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => CallBack(targetPos));
        }
    }
    void CallBack(Vector2 targetPos) {        
        isMoving = false;
        SetCordinates((int)targetPos.x, (int)targetPos.y);
        transform.position = new Vector2(xIndex, yIndex);
    }
    #endregion
}
