using NF.AI.PathFinding.JPSPlus;
using NF.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class GoBoard : MonoBehaviour
{
    // Wall
    // Primary
    // Straight
    // Diagonal
    // Wall Distance
    // Mark Start
    // Mark Goal
    // Path Finding

    GoMenubar mGoMenubar;
    GameObject mPrefab;
    RectTransform mRectTrasnform;
    GoJPSPlusBlock[,] mBlocks;
    HashSet<GoJPSPlusBlock> mWalkableBlock = new HashSet<GoJPSPlusBlock>();
    NF.AI.PathFinding.JPSPlus.JPSPlusMapBaker mBaker;
    private JPSPlusBakedMap mBakedMap;
    private bool[,] mWalls;

    public Int2 StartPos { get; internal set; }
    public Int2 GoalPos { get; internal set; }

    private void Awake()
    {
        mPrefab = Resources.Load(nameof(GoJPSPlusBlock)) as GameObject;
        mRectTrasnform = GetComponent<RectTransform>();
        mGoMenubar = transform.Find(nameof(GoMenubar)).GetComponent<GoMenubar>();
        mGoMenubar.FnBtnBake = OnBtnBake;
        mGoMenubar.FnBtnFindPath = OnBtnFindPath;
        mGoMenubar.FnBtnReset = OnBtnReset;
        mGoMenubar.FnOptPrimary = OnOptPrimary;
        mGoMenubar.FnOptStraight = OnOptStraight;
        mGoMenubar.FnOptDiagonal = OnOptDiagonal;
        mGoMenubar.FnOptWall = OnOptWall;
    }

    private void OnBtnReset()
    {
        mGoMenubar.Reset();
        for (int y = 0; y < mBlocks.GetLength(0); ++y)
        {
            for (int x = 0; x < mBlocks.GetLength(1); ++x)
            {
                mBlocks[y, x].Reset();
            }
        }
        SetWalls(mWalls);
    }

    #region Menubar

    private void OnOptWall(bool isOn)
    {
        foreach (var block in mWalkableBlock)
        {
            var p = block.Position;
            int index = mBaker.BlockLUT[p.Y, p.X];
            if (index >= 0)
            {
                mBlocks[p.Y, p.X].IndicateWall(isOn);
            }
        }
    }

    private void OnOptDiagonal(bool isOn)
    {
        foreach (var block in mWalkableBlock)
        {
            var p = block.Position;
            int index = mBaker.BlockLUT[p.Y, p.X];
            if (index >= 0)
            {
                mBlocks[p.Y, p.X].IndicateDiagonal(isOn);
            }
        }
    }

    private void OnOptStraight(bool isOn)
    {
        foreach (var block in mWalkableBlock)
        {
            var p = block.Position;
            int index = mBaker.BlockLUT[p.Y, p.X];
            if (index >= 0)
            {
                mBlocks[p.Y, p.X].IndicateStraight(isOn);
            }
        }
    }

    private void OnOptPrimary(bool isOn)
    {
        foreach (var block in mWalkableBlock)
        {
            var p = block.Position;
            int index = mBaker.BlockLUT[p.Y, p.X];
            if (index >= 0)
            {
                mBlocks[p.Y, p.X].IndicatePrimary(isOn);
            }
        }
    }

    private void OnBtnFindPath()
    {
        var jpsp = new NF.AI.PathFinding.JPSPlus.JPSPlus();
        jpsp.Init(mBakedMap);

        jpsp.SetStart(StartPos);
        mBlocks[StartPos.Y, StartPos.X].SetStart();

        jpsp.SetGoal(GoalPos);
        mBlocks[GoalPos.Y, GoalPos.X].SetGoal();

        bool isOk = jpsp.StepAll(10000);
        Debug.Assert(isOk);

        foreach (var path in jpsp.GetPaths())
        {
            mBlocks[path.Position.Y, path.Position.X].SetPath();
        }
    }


    private void OnBtnBake()
    {
        mBakedMap = mBaker.Bake();

        foreach (var block in mWalkableBlock)
        {
            var p = block.Position;
            int index = mBaker.BlockLUT[p.Y, p.X];
            if (index >= 0)
            {
                mBlocks[p.Y, p.X].SetBakerBlockInfo(mBaker.Blocks[index]);
            }
        }
        OnOptPrimary(true);
        mGoMenubar.Baked();
    }
    #endregion Menubar

    internal void Init(int width, int height)
    {
        mBlocks = new GoJPSPlusBlock[height, width];
        int cellSize = 94;
        int startY = (height / 2) * cellSize;
        int startX = -(width / 2) * cellSize;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                var block = Instantiate(mPrefab).GetComponent<GoJPSPlusBlock>();
                block.Rect.SetParent(this.mRectTrasnform);
                block.Rect.localScale = Vector3.one;
                block.Rect.localPosition = new Vector2(startX + (x * cellSize), startY - (y * cellSize));
                block.SetPosition(new Int2(x, y));
                block.SetEventHandler(Handler);
                mBlocks[y, x] = block;
            }
        }
    }

    void Handler(Int2 pos)
    {
        Debug.Log(pos);
    }

    internal void SetWalls(bool[,] walls)
    {
        mWalkableBlock.Clear();
        mWalls = walls;
        int height = walls.GetLength(0);
        int width = walls.GetLength(1);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (walls[y, x])
                {
                    mBlocks[y, x].SetWall();
                }
                else
                {
                    mWalkableBlock.Add(mBlocks[y, x]);
                }
            }
        }
        mBaker = new NF.AI.PathFinding.JPSPlus.JPSPlusMapBaker(walls);
    }
}
