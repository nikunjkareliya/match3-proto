using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private float delay = 0.5f;

    public Button button_StartAutoPlay;
    public Button button_StopAutoPlay;
    private IEnumerator m_routine = null;
    private Board m_board;
    #endregion

    #region UNITY METHODS
    void Start() => m_board = FindObjectOfType<Board>();
    #endregion

    #region UI METHODS
    public void Button_AutoPlayStart() {
        if (m_routine != null) StopCoroutine(m_routine);        
        m_routine = m_routineAutoPlay();
        StartCoroutine(m_routine);
        button_StartAutoPlay.interactable = false;
        button_StopAutoPlay.interactable = true;
    }
    public void Button_AutoPlayStop() {
        if (m_routine != null) { 
            StopCoroutine(m_routine);            
            button_StartAutoPlay.interactable = true;
            button_StopAutoPlay.interactable = false;
        }

        m_board.ClearBoard();
        m_board.ClearLists();
        m_board.SetUpBoard();
    }
    IEnumerator m_routineAutoPlay() {

        while (true) {            
            yield return new WaitForSeconds(delay);
            m_board.FindMatchesAndClear();           
            yield return new WaitForSeconds(delay);
            m_board.CollapseColumn();
            yield return new WaitForSeconds(delay);
            m_board.RefillEmptyTiles();
        }
    }
    #endregion
}
