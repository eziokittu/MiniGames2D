using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Cell
{
    // elements
    public bool hasWumpus;
    public bool hasPit;
    public bool hasPlayer;
    public bool hasGold;

    // effects
    public bool effect_hasBreeze;
    public bool effect_hasStench;

    public Cell() {
        hasWumpus = false;
        hasPit = false;
        hasPlayer = false;
        hasGold = false;
        effect_hasBreeze = false;
        effect_hasStench = false;
    }
};

public class GameController : MonoBehaviour
{
    public Transform[] groundTiles;
    [Space(10)]
    public TextMeshProUGUI text_details_hasWumpus;
    public TextMeshProUGUI text_details_hasPlayer;
    public TextMeshProUGUI text_details_hasPit;
    public TextMeshProUGUI text_details_hasGold;
    public TextMeshProUGUI text_details_hasBreeze;
    public TextMeshProUGUI text_details_hasStench;
    [Space(10)]
    public GameObject panel_Win;
    public GameObject panel_Lose;
    public GameObject panel_Controls;

    private Cell[] cells;
    // private GameObject player;
    private int currentPosition = 0;
    private int currentRotation = 0;
    private int count_wumpus = 1;
    private int count_pit = 3;
    private int count_gold = 1;
    [NonSerialized] public int count_arrows = 1;
    private List<int> occupiedCells = new List<int>();
    private List<int> effectedCell_breeze = new List<int>();
    private List<int> effectedCell_stench = new List<int>();

    void Awake() {
        // player = GameObject.FindGameObjectWithTag("Player");

        cells = new Cell[groundTiles.Length];

        // assign all tiles with default values
        for (int i=0; i<groundTiles.Length; i++){
            cells[i] = new Cell();
        }

        // adding the player's starting postion as occupied
        occupiedCells.Add(0); // '0' is the current start position
        cells[0].hasPlayer = true;
        
        if (panel_Lose != null){
            panel_Lose.SetActive(false);
        }
        if (panel_Win != null){
            panel_Win.SetActive(false);
        }
        if (panel_Controls != null){
            panel_Controls.SetActive(true);
        }

        GetGameElements();
    }

    // Start is called before the first frame update
    void Start()
    {
        // colour the ground tiles
        for (int i=0; i<groundTiles.Length; i++){
            if (((i/4)+i)%2==0){
                groundTiles[i].GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        text_details_hasWumpus.text = cells[currentPosition].hasWumpus == true ? "WUMPUS" : "---";
        text_details_hasPlayer.text = cells[currentPosition].hasPlayer == true ? "START" : "---";
        text_details_hasPit.text    = cells[currentPosition].hasPit == true ? "PIT" : "---";
        text_details_hasGold.text   = cells[currentPosition].hasGold == true ? "!!! GOLD !!!" : "---";
        text_details_hasBreeze.text = cells[currentPosition].effect_hasBreeze == true ? "~~Breeze~~" : "---";
        text_details_hasStench.text = cells[currentPosition].effect_hasStench == true ? "~~Stench~~" : "---";

        if (cells[currentPosition].hasWumpus || cells[currentPosition].hasPit){
            panel_Lose.SetActive(true);
        }
        if (cells[currentPosition].hasGold){
            panel_Win.SetActive(true);
        }
    }

    public void UpdatePlayerDirection(int player_rotationAngle){
        currentRotation = player_rotationAngle;
        Debug.Log("Player Direction updated : " + player_rotationAngle);
        // temp1 = "rotation = " + player_rotationAngle.ToString();
    }

    public void UpdatePlayerPosition(int player_position){
        currentPosition = player_position;
        Debug.Log("Player Position updated : " + player_position);
        // temp2 = "position = " + player_position.ToString();
    }

    public int GetPlayerPos(){
        return currentPosition;
    }

    private void GetGameElements(){
        // getting wumpus in any random location
        while (count_wumpus > 0){
            count_wumpus -= 1; // decrementing the count
            int pos_wumpus = getRandomPosition(); // getting a valid random position
            occupiedCells.Add(pos_wumpus); // adding the new occupied cell
            // adding the effected cells as a list
            Debug.Log("wumpus in "+pos_wumpus);
            foreach (int i in GetEffectedCells(pos_wumpus)){
                effectedCell_stench.Add(i);
            }
            cells[pos_wumpus].hasWumpus = true;
        }

        // getting all the pits in the random location
        while (count_pit > 0){
            count_pit -= 1; // decrementing the count
            int pos_pit = getRandomPosition(); // getting a valid random position
            occupiedCells.Add(pos_pit); // adding the new occupied cell
            // adding the effected cells as a list
            Debug.Log("pit in "+pos_pit);
            foreach (int i in GetEffectedCells(pos_pit)){
                effectedCell_breeze.Add(i);
            }
            cells[pos_pit].hasPit = true;
        }

        // getting the gold in the random location
        while (count_gold > 0){
            count_gold -= 1;
            int pos_gold = getRandomPosition();
            occupiedCells.Add(pos_gold);
            Debug.Log("gold in "+pos_gold);
            cells[pos_gold].hasGold = true;
        }
        
        // getting all the effects
        effectedCell_stench = RemoveDuplicates(effectedCell_stench);
        effectedCell_breeze = RemoveDuplicates(effectedCell_breeze);
        foreach (int i in effectedCell_stench){
            Debug.Log("stench in "+i);
            cells[i].effect_hasStench = true;
        }
        foreach (int i in effectedCell_breeze){
            Debug.Log("breeze in "+i);
            cells[i].effect_hasBreeze = true;
        }
    }

    // get a random location that is valid i.e. not pre occupied by pits, wumpous, player ...
    private int getRandomPosition(){
        bool foundPos;
        int pos;
        while (true){
            foundPos = true; // THIS SOLVED ERROR
            pos = UnityEngine.Random.Range(0, 16);
            foreach (int cell in occupiedCells){
                if (cell == pos){
                    foundPos = false;
                    break;
                }
            }
            if (foundPos == true){
                break;
            }
        }
        return pos;
    }

    // get all the effected cells like breeze / stench ... in all valid directions (up, left, down, right)
    private List<int> GetEffectedCells(int pos){
        List<int> effectedCells = new List<int>();

        // left
        if (pos-1>=0 && ((pos/4)==(pos-1)/4)){
            effectedCells.Add(pos-1);
        }
        // right
        if (pos+1<groundTiles.Length && ((pos/4)==(pos+1)/4)){
            effectedCells.Add(pos+1);
        }
        // up
        if (pos-4>=0){
            effectedCells.Add(pos-4);
        }
        // down
        if (pos+4<groundTiles.Length){
            effectedCells.Add(pos+4);
        }

        return effectedCells;
    }

    // Utility Function - removes duplicates from a List<int> and returns it
    private List<int> RemoveDuplicates(List<int> inputList)
    {
        // Create a new list to store unique elements
        List<int> uniqueList = new List<int>();

        // Create a HashSet to keep track of seen elements
        HashSet<int> seenElements = new HashSet<int>();

        foreach (int element in inputList)
        {
            if (!seenElements.Contains(element))
            {
                // If the element is not in the HashSet, add it to the unique list and the HashSet
                uniqueList.Add(element);
                seenElements.Add(element);
            }
        }

        return uniqueList;
    }

    public bool playerCanHitWumpus(){
        // getting all the cells where the arrow will take effect
        List<int> possibleCells = new List<int>();
        if (currentRotation==0){
            for (int i=currentPosition; i>=0; i-=4){
                possibleCells.Add(i);
            }
        }
        else if (currentRotation==180){
            for (int i=currentPosition; i<16; i+=4){
                possibleCells.Add(i);
            }
        }
        else if (currentRotation==90){
            for (int i=currentPosition, j=currentPosition/4; i>=0 && i/4==j; i-=1){
                possibleCells.Add(i);
            }
        }
        else if (currentRotation==270){
            for (int i=currentPosition, j=currentPosition/4; i<16 && i/4==j; i+=1){
                possibleCells.Add(i);
            }
        }

        // if wumpus is present in the cells where arrow is thrown
        foreach (int i in possibleCells){
            if (cells[i].hasWumpus==true){
                cells[i].hasWumpus = false; // wumpus is killed
                count_arrows--;
                return true;
            }
        }

        return false;
    }

    public void ResetWorld(){
        SceneManager.LoadScene("Scene_WumpusWorld_1");
    }
}
