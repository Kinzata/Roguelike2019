using UnityEngine;

public static class MouseUtilities
{
    public static CellPosition GetCellPositionAtMousePosition(GroundMap map)
    {
        var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPos.x += 0.5f;
        mPos.y += 0.5f;
        var tilePos = map.map.WorldToCell(mPos);
        return new CellPosition(tilePos);
    }
}
