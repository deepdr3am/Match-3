using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager
{
    public static List<List<Cell>> GetMatchedGroups(List<Cell> targetCells)
    {
        List<List<Cell>> matchedGroups = new List<List<Cell>>();
        List<Cell> matchedCells = new List<Cell>();

        foreach (Cell c in targetCells)
        {
            if (matchedCells.Contains(c)) continue;

            List<Cell> newGroup = new List<Cell>();

            List<Cell> v_Matches = GetVerticalMatches(c);
            List<Cell> h_Matches = GetHorizontalMatches(c);
            if (v_Matches.Count >= 2)
                newGroup.AddRange(v_Matches);

            if (h_Matches.Count >= 2)
                newGroup.AddRange(h_Matches);

            if (newGroup.Count >= 2)
            {
                newGroup.Add(c);
                matchedCells.AddRange(newGroup);
                matchedGroups.Add(newGroup);
            }
        }

        return matchedGroups;
    }

    static List<Cell> GetVerticalMatches(Cell cell)
    {
        List<Cell> matches = new List<Cell>();
        Cell refCell = cell;

        while (refCell.UpCell && refCell.cellType == refCell.UpCell.cellType)
        {
            matches.Add(refCell.UpCell);
            refCell = refCell.UpCell;
        }

        refCell = cell;
        while (refCell.DownCell && refCell.cellType == refCell.DownCell.cellType)
        {
            matches.Add(refCell.DownCell);
            refCell = refCell.DownCell;
        }

        return matches;
    }

    static List<Cell> GetHorizontalMatches(Cell cell)
    {
        List<Cell> matches = new List<Cell>();
        Cell refCell = cell;

        while (refCell.RightCell && refCell.cellType == refCell.RightCell.cellType)
        {
            matches.Add(refCell.RightCell);
            refCell = refCell.RightCell;
        }

        refCell = cell;

        while (refCell.LeftCell && refCell.cellType == refCell.LeftCell.cellType)
        {
            matches.Add(refCell.LeftCell);
            refCell = refCell.LeftCell;
        }

        return matches;
    }
}
