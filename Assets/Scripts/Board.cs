using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    #region FIELDS
    public float collapseColumnSpeed = .2f;
    public float swapSpeed = .2f;
    public float fallSpeed = .5f;
    public Vector2 test = new Vector2(3, 3);
    [SerializeField] private int boardWidth = 5;
    [SerializeField] private int boardHeight = 5;
    
    public GameObject[] candyPrefabs;
    public GameObject[,] m_tiles;
    public GameObject startTile, endTile;

    public List<GameObject> matches;
    public List<GameObject> matchesHorizontal;
    public List<GameObject> matchesVertical;
    private List<GameObject> matchesLeft, matchesRight, matchesUp, matchesDown;    
    #endregion

    #region UNITY METHODS
    void Start()
    {
        m_tiles = new GameObject[boardWidth, boardHeight];
        Camera.main.transform.position = new Vector3((float)(boardWidth - 1) / 2f, (float)(boardHeight - 1) / 2f, -10f);

        SetUpBoard();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //FindMatches((int)test.x, (int)test.y);
            //ClearCandy(matches);
            //ClearLists();
            //CollapseColumn(2);
            //CollapseColumn();


            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    FindMatches(i, j);
                    break;
                }
            }
            if (matches.Count >= 3)
            {
                ClearCandy(matches);
                ClearLists();
                CollapseColumn();
            }

            RefillEmptyTiles();

        }

    }
    #endregion

    #region CREATING CANDIES
    /// <summary>
    /// Setting up board with random candies
    /// </summary>
    public void SetUpBoard() {

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                CreateCandy(i, j, true);
            }
        }
    }
    
    /// <summary>
    /// Create a candy at specified location of grid
    /// </summary>    
    /// <param name="createAtTop"> Create candies at top for falling animation</param>
    private void CreateCandy(int i, int j, bool createAtTop = false)
    {
        GameObject tmpTile = Instantiate(candyPrefabs[Random.Range(0, candyPrefabs.Length)]);

        m_tiles[i, j] = tmpTile;
        tmpTile.GetComponent<Candy>().SetCordinates(i, j);

        if (!createAtTop)
        {
            tmpTile.transform.position = new Vector3(i, j, 0);
        }
        else { 
            tmpTile.transform.position = new Vector3(i, 10, 0);
            tmpTile.GetComponent<Candy>().Move(new Vector2(i, j), fallSpeed);
        }
        tmpTile.transform.rotation = Quaternion.identity;

        tmpTile.name = "Tile (" + i + "," + j + ")";
        tmpTile.transform.SetParent(transform);
    }
    
    /// <summary>
    /// Identify the empty tiles and refill it with candies
    /// </summary>
    public void RefillEmptyTiles()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                if (m_tiles[i, j] == null)
                {
                    //Debug.Log("NULL Tile: " + " (" + i + ", " + j + ")");
                    CreateCandy(i, j, true);
                }
            }
        }
    }
    #endregion

    #region FINDING MATCHES
    /// <summary>
    /// Find matches across the board
    /// </summary>
    public List<GameObject> FindMatches() {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                matches = FindMatches(i, j);
            }
        }
        return matches;
    }
    /// <summary>
    /// Identify matched candies at specified location(x, y) and store it into list
    /// </summary>    
    public List<GameObject> FindMatches(int x, int y)
    {
        matches = new List<GameObject>();

        matchesLeft = FindMatchesLeft(x, y);
        matchesRight = FindMatchesRight(x, y);
        matchesUp = FindMatchesUp(x, y);
        matchesDown = FindMatchesDown(x, y);

        if (matchesLeft == null)
            matchesLeft = new List<GameObject>();
        if (matchesRight == null)
            matchesRight = new List<GameObject>();
        if (matchesUp == null)
            matchesUp = new List<GameObject>();
        if (matchesDown == null)
            matchesDown = new List<GameObject>();
        if (matchesHorizontal == null)
            matchesHorizontal = new List<GameObject>();
        if (matchesVertical == null)
            matchesVertical = new List<GameObject>();
        
        CheckMatchesHorizontalValidation();
        CheckMatchesVerticalValidation();

        matches = matchesHorizontal.Union(matchesVertical).ToList();
        //CheckMatchesValidation();
        //else { matchesHorizontal.Clear(); }

        // Merge 2 list with out duplicates
        //IEnumerable<GameObject> union_Vertical = matchesUp.Union(matchesDown);
        //matchesVertical = union_Vertical.ToList();

        //if (matchesHorizontal.Count >= 3 || matchesVertical.Count >= 3) { 
        //    IEnumerable<GameObject> union = matchesHorizontal.Union(matchesVertical);
        //    matches = union.ToList();
        //} 
        //else if (matchesHorizontal.Count >= 3) {
        //    return matchesHorizontal;
        //} 
        //else if (matchesVertical.Count >= 3) { 
        //    return matchesVertical;            
        //} 


        //matches = matchesVertical;

        if (matches.Count >= 3)
        {
            return matches;
        }
        else
        {
            matches.Clear();
        }

        return null;
    }

    /// <summary>
    /// Identify matches HORIZONTALLY
    /// </summary>    
    private void CheckMatchesHorizontalValidation()
    {
        if (matchesLeft.Count >= 2 && matchesRight.Count >= 2)
        {
            matchesHorizontal = matchesLeft.Union(matchesRight).ToList();
        }
        else if (matchesLeft.Count >= 2 && matchesRight.Count >= 3)
        {
            matchesHorizontal = matchesLeft.Union(matchesRight).ToList();
        }
        else if (matchesLeft.Count >= 3 && matchesRight.Count >= 2)
        {
            matchesHorizontal = matchesLeft.Union(matchesRight).ToList();
        }        
        else if (matchesLeft.Count == 0 && matchesRight.Count >= 3)
        {
            matchesHorizontal = matchesLeft.Union(matchesRight).ToList();
        }
        else if (matchesLeft.Count >= 3 && matchesRight.Count == 0)
        {
            matchesHorizontal = matchesLeft.Union(matchesRight).ToList();
        }
    }

    /// <summary>
    /// Identify matches VERTICALLY
    /// </summary>    
    private void CheckMatchesVerticalValidation()
    {
        if (matchesUp.Count >= 2 && matchesDown.Count >= 2)
        {
            matchesVertical = matchesUp.Union(matchesDown).ToList();
        }
        else if (matchesUp.Count >= 2 && matchesDown.Count >= 3)
        {
            matchesVertical = matchesUp.Union(matchesDown).ToList();
        }
        else if (matchesUp.Count >= 3 && matchesDown.Count >= 2)
        {
            matchesVertical = matchesUp.Union(matchesDown).ToList();
        }        
        else if (matchesUp.Count == 0 && matchesDown.Count >= 3)
        {
            matchesVertical = matchesUp.Union(matchesDown).ToList();
        }
        else if (matchesUp.Count >= 3 && matchesDown.Count == 0)
        {
            matchesVertical = matchesUp.Union(matchesDown).ToList();
        }
    }    
    
    /// <summary>
    /// Identify matches LEFT side of (x, y) tile
    /// </summary>    
    List<GameObject> FindMatchesLeft(int x, int y) {

        matches = new List<GameObject>();
        
        GameObject startCandy = null;
        if (IsValid(x, y))
        {
            startCandy = m_tiles[x, y]; 
        }

        if (startCandy != null)
        {
            matches.Add(startCandy);
        }
        else {
            return null;
        }

        for (int i = 1; i < 5; i++) {
            int newX = x - i;
            int newY = y;

            if (!IsValid(newX, newY)) { 
                break; 
            }
            GameObject nextCandy = m_tiles[newX, newY];

            if (nextCandy == null) { break; }
            else
            {
                if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
                {
                    matches.Add(nextCandy);
                }
                else break;
            }
            
        }

        if (matches.Count >= 2)
        {
            return matches;
        }
        else {
            matches.Clear();
        }

        return null;
    }
    
    /// <summary>
    /// Identify matches RIGHT side of (x, y) tile
    /// </summary>    
    List<GameObject> FindMatchesRight(int x, int y)
    {
        matches = new List<GameObject>();

        GameObject startCandy = null;
        if (IsValid(x, y))
        {
            startCandy = m_tiles[x, y];
        }

        if (startCandy != null)
        {
            matches.Add(startCandy);
        }
        else
        {
            return null;
        }

        for (int i = 1; i < 5; i++)
        {
            int newX = x + i;
            int newY = y;

            if (!IsValid(newX, newY))
            {
                break;
            }
            GameObject nextCandy = m_tiles[newX, newY];

            if (nextCandy == null) { break; }
            else
            {
                if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
                {
                    matches.Add(nextCandy);
                }
                else break;
            }
            //if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
            //{
            //    matches.Add(nextCandy);
            //}
            //else break;
        }

        if (matches.Count >= 2)
        {
            return matches;
        }
        else
        {
            matches.Clear();
        }

        return null;
    }
    
    /// <summary>
    /// Identify matches ABOVE of (x, y) tile
    /// </summary>    
    List<GameObject> FindMatchesUp(int x, int y)
    {
        matches = new List<GameObject>();

        GameObject startCandy = null;
        if (IsValid(x, y))
        {
            startCandy = m_tiles[x, y];
        }

        if (startCandy != null)
        {
            matches.Add(startCandy);
        }
        else
        {
            return null;
        }

        for (int i = 1; i < 5; i++)
        {
            int newX = x;
            int newY = y + i;

            if (!IsValid(newX, newY))
            {
                break;
            }
            GameObject nextCandy = m_tiles[newX, newY];

            if (nextCandy == null) { break; }
            else { 
                if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
                {
                    matches.Add(nextCandy);
                }
                else break;
            }
        }

        if (matches.Count >= 2)
        {
            return matches;
        }
        else
        {
            matches.Clear();
        }

        return null;
    }
    
    /// <summary>
    /// Identify matches BOTTOM of (x, y) tile
    /// </summary>    
    List<GameObject> FindMatchesDown(int x, int y)
    {
        matches = new List<GameObject>();

        GameObject startCandy = null;
        if (IsValid(x, y))
        {
            startCandy = m_tiles[x, y];
        }

        if (startCandy != null)
        {
            matches.Add(startCandy);
        }
        else
        {
            return null;
        }

        for (int i = 1; i < 5; i++)
        {
            int newX = x;
            int newY = y - i;

            if (!IsValid(newX, newY))
            {
                break;
            }
            GameObject nextCandy = m_tiles[newX, newY];
            
            if (nextCandy == null) { break; }
            else
            {
                if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
                {
                    matches.Add(nextCandy);
                }
                else break;
            }
            //if (nextCandy.GetComponent<Candy>().candyColor == startCandy.GetComponent<Candy>().candyColor && !matches.Contains(nextCandy))
            //{
            //    matches.Add(nextCandy);
            //}
            //else break;
        }

        if (matches.Count >= 2)
        {
            return matches;
        }
        else
        {
            matches.Clear();
        }

        return null;
    }

    /// <summary>
    /// Check weather the tile[x, y] is within the bounds of grid
    /// </summary>    
    bool IsValid(int x, int y) {

        if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight )
        {
            return true;
        }
        return false;
    }
    public void FindMatchesAndClear() {
        
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                FindMatches(i, j);
            }
        }
        if (matches.Count >= 3)
        {
            ClearCandy(matches);
            ClearLists();
        }
    }
    #endregion

    #region CLEARING CANDIES
    /// <summary>
    /// Clear the candy at specified location of grid
    /// </summary>    
    void ClearCandy(int x, int y) {
        
        var tmp = m_tiles[x, y];
        if (tmp != null)
        {
            m_tiles[x, y] = null;
            Destroy(tmp.gameObject);
        }              
    }

    /// <summary>
    /// Clear matched candies only
    /// </summary>
    public void ClearCandy(List<GameObject> matches) {
        for (int i = 0; i < matches.Count; i++) {
            ClearCandy(matches[i].GetComponent<Candy>().xIndex, matches[i].GetComponent<Candy>().yIndex);            
        }
        //matches.Clear();
        ClearLists(); 
    }

    /// <summary>
    /// Clear the all candies of board
    /// </summary>
    public void ClearBoard() {

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                ClearCandy(i, j);
            }
        }        
    }
    public void ClearLists() {
        matches.Clear();        
        matchesHorizontal.Clear();
        matchesVertical.Clear();
    }
    #endregion

    #region COLLAPSE COLUMNS
    /// <summary>
    /// Collapse particular column
    /// </summary>
    private void CollapseColumn(int column) {

        for (int i = 0; i < boardHeight - 1; i++) {

            if (m_tiles[column, i] == null) {
                //Debug.Log("Null At: " + column + ", " + i);
                for (int j = i + 1; j < boardHeight; j++) {
                    if (m_tiles[column, j] != null) {
                        m_tiles[column, j].GetComponent<Candy>().Move(new Vector2(column, i), collapseColumnSpeed);

                        m_tiles[column, i] = m_tiles[column, j];
                        m_tiles[column, i].GetComponent<Candy>().SetCordinates(column, i);
                        m_tiles[column, i].name = "Tile (" + column + "," + i + ")";
                        m_tiles[column, j] = null;

                        break;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Collapse all columns, where the empty cells identified
    /// </summary>
    public void CollapseColumn() {
        for (int i = 0; i < boardWidth; i++) {
            CollapseColumn(i);
        }
    }
    #endregion

    #region SWAPPING
    /// <summary>
    /// Swap the 2 candies, and if there are no matches after swapping, undo the swapping
    /// If matches found, then identify unique candies(with out duplicates) and clear matched candies then collapse the columns
    /// </summary>    
    public void SwapCandy(GameObject startTile, GameObject endTile) {
        StartCoroutine(RoutineSwapCandy(startTile, endTile));
    }    
    IEnumerator RoutineSwapCandy(GameObject startTile, GameObject endTile) {

        Candy startCandy = startTile.GetComponent<Candy>();
        Candy endCandy = endTile.GetComponent<Candy>();

        if (startCandy == null || endCandy == null) yield break;
        
        Candy tmp = startCandy;
        m_tiles[startCandy.xIndex, startCandy.yIndex] = m_tiles[endCandy.xIndex, endCandy.yIndex];
        m_tiles[endCandy.xIndex, endCandy.yIndex] = tmp.gameObject;

        // Check whether the candies are neighbours & not in diagonal
        if (!IsAdjacent(startTile, endTile))
        {
            Debug.Log("No, Not Adjacent");
            yield break;
        }

        if (startCandy != null)
            startCandy.Move(new Vector2(endCandy.xIndex, endCandy.yIndex), swapSpeed);
        if (endCandy != null)
            endCandy.Move(new Vector2(startCandy.xIndex, startCandy.yIndex), swapSpeed);

        startCandy.name = "Tile (" + endCandy.xIndex + "," + endCandy.yIndex + ")";
        endCandy.name = "Tile (" + startCandy.xIndex + "," + startCandy.yIndex + ")";

        yield return new WaitForSeconds(swapSpeed);
        
        List<GameObject> matchesStartTile = FindMatches(startCandy.xIndex, startCandy.yIndex);
        List<GameObject> matchesEndTile = FindMatches(endCandy.xIndex, endCandy.yIndex);

        if (matchesStartTile == null && matchesEndTile == null)        
        {
            Debug.Log("1. No Matches Found");
            if (startCandy == null || endCandy == null) yield break;

            Candy tmp1 = startCandy;
            m_tiles[startCandy.xIndex, startCandy.yIndex] = m_tiles[endCandy.xIndex, endCandy.yIndex];
            m_tiles[endCandy.xIndex, endCandy.yIndex] = tmp1.gameObject;

            startCandy.Move(new Vector2(endCandy.xIndex, endCandy.yIndex), swapSpeed);
            endCandy.Move(new Vector2(startCandy.xIndex, startCandy.yIndex), swapSpeed);

            startCandy.name = "Tile (" + endCandy.xIndex + "," + endCandy.yIndex + ")";
            endCandy.name = "Tile (" + startCandy.xIndex + "," + startCandy.yIndex + ")";            
        }
        else
        {            
            Debug.Log("2. Matches Found");
            
            List<GameObject> union = matchesStartTile.Union(matchesEndTile).ToList();
            Debug.Log("Union count: " + union.Count);

            ClearCandy(union);
            CollapseColumn();
            ClearLists();
        }

    }
    
    /// <summary>
    /// Check whether the candies are neighbours & not in diagonal
    /// </summary>    
    bool IsAdjacent(GameObject startTile, GameObject endTile) {

        Candy startCandy = startTile.GetComponent<Candy>();
        Candy endCandy = endTile.GetComponent<Candy>();

        int diffX = Mathf.Abs(startCandy.xIndex - endCandy.xIndex);
        int diffY = Mathf.Abs(startCandy.yIndex - endCandy.yIndex);
        
        if (diffX > 1 || diffY > 1 || diffX == diffY) return false;
        
        return true; 
    }
    #endregion
}