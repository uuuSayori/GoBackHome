using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockCondition
{
    NONE = -1,
    MOVE_UP = 0,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    LIFE_UP,
    LIFE_DOWN,
}

public class FloorStatus : MonoBehaviour
{

    public bool m_isVisited = false;

    public int m_PosX = 0;
    public int m_PosY = 0;

    public BlockCondition m_blockCondition = BlockCondition.NONE;
    public bool m_isOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        m_isOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
