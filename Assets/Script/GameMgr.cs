using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_Floors = null;

    [SerializeField] private GameObject m_Player = null;
    [SerializeField] private int m_PlayerLife = 10;
    [SerializeField] private int m_WalkDecrease = 1;

    [SerializeField] private Sprite m_VisitedSprite = null;
    [SerializeField] private Sprite m_CanMoveSprite = null;
    [SerializeField] private Sprite m_CantMoveSprite = null;

    [SerializeField] private GameObject m_ConditionStatus = null;
    [SerializeField] private Sprite[] m_ConditionSprite = null;

    [SerializeField] private GameObject m_MoveStatus = null;
    [SerializeField] private Sprite[] m_MoveSprite = null;

    [SerializeField] private GameObject m_LifeStatus = null;
    [SerializeField] private Sprite[] m_LifeSprite = null;

    [SerializeField] private GameObject m_HPStatus = null;
    [SerializeField] private Sprite[] m_HPSprite = null;

    private bool m_isOnMove = false;

    private int m_CurPlayerX = 0;
    private int m_CurPlayerY = 0;

    public enum MoveDir
    {
        DIR_UP = 0,
        DIR_DOWN,
        DIR_LEFT,
        DIR_RIGHT
    }

    void Awake()
    {
        Screen.SetResolution(960, 640, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CurPlayerX = 0;
        m_CurPlayerY = 0;

        m_isOnMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!JudgeSucOrFail())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.collider.tag == "Floor")
                {
                    if (m_isOnMove)
                    {
                        ClearMoveAndLifeStatus();
                        int disX = hit.collider.gameObject.GetComponent<FloorStatus>().m_PosX - m_CurPlayerX;
                        int disY = hit.collider.gameObject.GetComponent<FloorStatus>().m_PosY - m_CurPlayerY;

                        if (disY == 0)
                        {
                            if (disX == 1)
                            {
                                MovePlayer(MoveDir.DIR_RIGHT);
                                JudgeVisitedBlock();
                                SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                                JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                            }
                            else if (disX == -1)
                            {
                                MovePlayer(MoveDir.DIR_LEFT);
                                JudgeVisitedBlock();
                                SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                                JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                            }

                        }
                        else if (disX == 0)
                        {
                            if (disY == 1)
                            {
                                MovePlayer(MoveDir.DIR_DOWN);
                                JudgeVisitedBlock();
                                SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                                JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                            }
                            else if (disY == -1)
                            {
                                MovePlayer(MoveDir.DIR_UP);
                                JudgeVisitedBlock();
                                SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                                JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                            }
                        }
                    }
                }
                else if (hit.collider.tag == "MoveButton")
                {
                    m_isOnMove = !m_isOnMove;
                    if (m_isOnMove)
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = m_CanMoveSprite;
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = m_CantMoveSprite;
                    }
                }
                else if (hit.collider.tag == "StopButton")
                {
                    OpenBlock(m_CurPlayerX, m_CurPlayerY);
                }

                //After one effective clicking, judge if game is finished.
                if ((hit.collider.tag == "Floor" && m_isOnMove) || hit.collider.tag == "StopButton")
                {
                    UpdateHPStatus();

                    JudgeSucOrFail();
                }
            }
        }
    }

    public void MovePlayer(MoveDir moveDir, int StepNum = 1)
    {
        switch (moveDir)
        {
            case MoveDir.DIR_UP:
                if (m_CurPlayerY > 0)
                {
                    m_CurPlayerY--;
                }
                break;
            case MoveDir.DIR_DOWN:
                if (m_CurPlayerY < 3)
                {
                    m_CurPlayerY++;
                }
                break;
            case MoveDir.DIR_LEFT:
                if (m_CurPlayerX > 0)
                {
                    m_CurPlayerX--;
                }
                break;
            case MoveDir.DIR_RIGHT:
                if (m_CurPlayerX < 3)
                {
                    m_CurPlayerX++;
                }
                break;
        }

        m_Player.transform.position = m_Floors[m_CurPlayerY * 4 + m_CurPlayerX].transform.position;
        m_Floors[m_CurPlayerY * 4 + m_CurPlayerX].GetComponent<SpriteRenderer>().sprite = m_VisitedSprite;
    }

    public void JudgeVisitedBlock()
    {
        //if not visited current block, hp reduce
        if (!m_Floors[m_CurPlayerY * 4 + m_CurPlayerX].GetComponent<FloorStatus>().m_isVisited)
        {
            m_PlayerLife -= m_WalkDecrease;
        }
    }

    public void SetBlockVisited(int BlockX, int BlockY)
    {
        m_Floors[BlockY * 4 + BlockX].GetComponent<FloorStatus>().m_isVisited = true;
    }

    public void OpenBlock(int BlockX, int BlockY)
    {
        if (!m_Floors[BlockY * 4 + BlockX].GetComponent<FloorStatus>().m_isOpened)
        {
            switch (m_Floors[BlockY * 4 + BlockX].GetComponent<FloorStatus>().m_blockCondition)
            {
                case BlockCondition.MOVE_UP:
                    MovePlayer(MoveDir.DIR_UP);
                    SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                    JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                    m_MoveStatus.GetComponent<SpriteRenderer>().sprite = m_MoveSprite[(int)MoveDir.DIR_UP];
                    break;
                case BlockCondition.MOVE_DOWN:
                    MovePlayer(MoveDir.DIR_DOWN);
                    SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                    JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                    m_MoveStatus.GetComponent<SpriteRenderer>().sprite = m_MoveSprite[(int)MoveDir.DIR_DOWN];
                    break;
                case BlockCondition.MOVE_LEFT:
                    MovePlayer(MoveDir.DIR_LEFT);
                    SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                    JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                    m_MoveStatus.GetComponent<SpriteRenderer>().sprite = m_MoveSprite[(int)MoveDir.DIR_LEFT];
                    break;
                case BlockCondition.MOVE_RIGHT:
                    MovePlayer(MoveDir.DIR_RIGHT);
                    SetBlockVisited(m_CurPlayerX, m_CurPlayerY);
                    JudgeConditionGoodOrBad(m_CurPlayerX, m_CurPlayerY);
                    m_MoveStatus.GetComponent<SpriteRenderer>().sprite = m_MoveSprite[(int)MoveDir.DIR_RIGHT];
                    break;
                case BlockCondition.LIFE_UP:
                    m_PlayerLife++;
                    m_LifeStatus.GetComponent<SpriteRenderer>().sprite = m_LifeSprite[0];
                    break;
                case BlockCondition.LIFE_DOWN:
                    m_PlayerLife--;
                    m_LifeStatus.GetComponent<SpriteRenderer>().sprite = m_LifeSprite[1];
                    break;

            }

            m_Floors[BlockY * 4 + BlockX].GetComponent<FloorStatus>().m_isOpened = true;
        }
    }

    public void JudgeConditionGoodOrBad(int BlockX, int BlockY)
    {
        switch (m_Floors[BlockY * 4 + BlockX].GetComponent<FloorStatus>().m_blockCondition)
        {
            case BlockCondition.NONE:
            case BlockCondition.MOVE_UP:
            case BlockCondition.MOVE_DOWN:
            case BlockCondition.MOVE_LEFT:
            case BlockCondition.MOVE_RIGHT:
                m_ConditionStatus.GetComponent<SpriteRenderer>().sprite = m_ConditionSprite[1];
                break;
            case BlockCondition.LIFE_UP:
                m_ConditionStatus.GetComponent<SpriteRenderer>().sprite = m_ConditionSprite[0];
                break;
            case BlockCondition.LIFE_DOWN:
                m_ConditionStatus.GetComponent<SpriteRenderer>().sprite = m_ConditionSprite[2];
                break;
        }
    }

    public void ClearMoveAndLifeStatus()
    {
        m_MoveStatus.GetComponent<SpriteRenderer>().sprite = null;
        m_LifeStatus.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void UpdateHPStatus()
    {
        if (m_PlayerLife >= 0 && m_PlayerLife <= 9)
        {
            m_HPStatus.GetComponent<SpriteRenderer>().sprite = m_HPSprite[m_PlayerLife];
        }
        else
        {
            m_HPStatus.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public bool JudgeSucOrFail()
    {
        if (m_PlayerLife < 0)
        {
            //Fail

            return true;
        }
        else if (m_CurPlayerX == 3 && m_CurPlayerY == 3)
        {
            //Success

            return true;
        }
        else
        {
            //Not Fail and Success

            return false;
        }
    }
}
