using NF.AI.PathFinding.Common;
using NF.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GoJPSPlusBlock : MonoBehaviour, IPointerClickHandler
{
    public RectTransform Rect { get; private set; }
    public Int2 Position { get; private set; }

    NF.AI.PathFinding.JPSPlus.JPSPlusMapBakerBlock mBakedBlock;
    Image mBase;
    public Image[] dirImages;
    public Text[] dirTexts;
    public Image imgName;
    public Text txtName;

    bool mIsWallOn;
    bool mIsDiagonalOn;
    bool mIsStraightOn;
    bool isStartOrGoal = false;
    private Action<Int2> mHandler;

    readonly EDirFlags StraightDirs = EDirFlags.SOUTH | EDirFlags.NORTH | EDirFlags.EAST | EDirFlags.WEST;
    readonly EDirFlags DiagonalDirs = EDirFlags.NORTHWEST | EDirFlags.NORTHEAST | EDirFlags.SOUTHWEST | EDirFlags.SOUTHEAST;
    readonly Color InvisibleColor = new Color(0, 0, 0, 0);

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        mBase = transform.Find("base").GetComponent<Image>();

        for (int i = 0; i < 8; ++i)
        {
            dirImages[i].color = InvisibleColor;
            dirTexts[i].text = string.Empty;
        }
        imgName.color = InvisibleColor;
        txtName.text = string.Empty;
    }

    public void SetWall()
    {
        mBase.color = Color.black;
    }

    public void SetBakerBlockInfo(NF.AI.PathFinding.JPSPlus.JPSPlusMapBakerBlock block)
    {
        mBakedBlock = block;
    }

    public void SetGoal()
    {
        isStartOrGoal = true;
        mBase.color = Color.cyan;
    }

    public void SetStart()
    {
        isStartOrGoal = true;
        mBase.color = Color.green;
    }

    internal void Reset()
    {
        isStartOrGoal = false;
        mIsDiagonalOn = false;
        mIsStraightOn = false;
        mIsWallOn = false;
        mBase.color = Color.white;
        for (int i = 0b10000000; i > 0; i >>= 1)
        {
            EDirFlags dir = (EDirFlags)i;
            int index = DirFlags.ToArrayIndex(dir);
            dirImages[index].color = InvisibleColor;
            dirTexts[index].text = string.Empty;
        }
    }

    internal void SetEventHandler(Action<Int2> handler)
    {
        mHandler = handler;
    }

    internal void SetPosition(Int2 pos)
    {
        Position = pos;
    }


    internal void SetPath()
    {
        if (isStartOrGoal)
        {
            return;
        }
        mBase.color = new Color(0.5f, 0, 0.5f);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        mHandler(Position);
    }

    internal void IndicatePrimary(bool isOn)
    {
        if (isStartOrGoal)
        {
            return;
        }

        if (isOn)
        {
            if (mBakedBlock.JumpDirFlags != NF.AI.PathFinding.Common.EDirFlags.NONE)
            {
                mBase.color = new Color(0.7f, 0.7f, 1f);
            }
        }
        else
        {
            mBase.color = Color.white;
        }
    }

    internal void IndicateWall(bool isOn)
    {
        if (isStartOrGoal)
        {
            return;
        }

        mIsWallOn = isOn;
        Indicate(mIsStraightOn, mIsDiagonalOn, mIsWallOn);
    }

    private void Indicate(bool mIsStraightOn, bool mIsDiagonalOn, bool mIsWallOn)
    {
        for (int i = 0b10000000; i > 0; i >>= 1)
        {
            EDirFlags dir = (EDirFlags)i;
            int dist = mBakedBlock.GetDistance(dir);
            int index = DirFlags.ToArrayIndex(dir);
            if (dist > 0)
            {
                if ((dir & StraightDirs) == dir)
                {

                    if (mIsStraightOn)
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = dist.ToString();
                    }
                    else
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = string.Empty;
                    }
                }

                if ((dir & DiagonalDirs) == dir)
                {

                    if (mIsDiagonalOn)
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = dist.ToString();
                    }
                    else
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = string.Empty;
                    }
                }
            }
            else if (dist == 0)
            {
                if (mIsWallOn)
                {
                    dirImages[index].color = Color.red;
                    dirTexts[index].text = dist.ToString();
                }
                else
                {
                    dirImages[index].color = InvisibleColor;
                    dirTexts[index].text = string.Empty;
                }
            }
            else
            {
                if ((dir & StraightDirs) == dir)
                {

                    if (mIsWallOn && mIsStraightOn)
                    {
                        dirImages[index].color = Color.yellow;
                        dirTexts[index].text = dist.ToString();
                    }
                    else
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = string.Empty;
                    }
                }

                if ((dir & DiagonalDirs) == dir)
                {

                    if (mIsWallOn && mIsDiagonalOn)
                    {
                        dirImages[index].color = Color.yellow;
                        dirTexts[index].text = dist.ToString();
                    }
                    else
                    {
                        dirImages[index].color = InvisibleColor;
                        dirTexts[index].text = string.Empty;
                    }
                }
            }
        }
    }

    internal void IndicateDiagonal(bool isOn)
    {
        if (isStartOrGoal)
        {
            return;
        }

        mIsDiagonalOn = isOn;
        Indicate(mIsStraightOn, mIsDiagonalOn, mIsWallOn);

    }

    internal void IndicateStraight(bool isOn)
    {
        if (isStartOrGoal)
        {
            return;
        }
        mIsStraightOn = isOn;
        Indicate(mIsStraightOn, mIsDiagonalOn, mIsWallOn);
    }
    //private void DrawLine(Vector2 pointA, Vector2 pointB)
    //{
    //    float lineWidth = 2;

    //    Vector3 differenceVector = pointB - pointA;
    //    var line = Instantiate(prefabLine, this.GetComponent<Transform>());
    //    var imageRectTransform = line.GetComponent<RectTransform>();
    //    imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude / CenterImg.canvas.scaleFactor, lineWidth);
    //    imageRectTransform.pivot = new Vector2(0, 0.5f);
    //    imageRectTransform.localPosition = new Vector3(pointA.x, pointA.y, CenterImg.transform.position.z);
    //    float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
    //    line.transform.localRotation = Quaternion.Euler(0, 0, angle);
    //}

}
