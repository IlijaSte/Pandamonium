using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment {

    public Room room;

    public int xPos;
    public int yPos;

    public Fragment(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;

    }

    public Fragment(int xPos, int yPos, Room room)
    {

        this.xPos = xPos;
        this.yPos = yPos;

        this.room = room;

    }

    public void SetRoom(Room room)
    {
        this.room = room;
    }

    /*public static Fragment GetFragmentAt(Vector2Int pos)
    {

        if (pos.y < 0 || pos.x < 0 || pos.y >= bc.fragmentHeight * bc.heightInFragments || pos.x >= bc.fragmentWidth * bc.widthInFragments)
            return null; ;

        return bc.fragments[pos.y / bc.fragmentHeight][pos.x / bc.fragmentWidth];
    }

    public Fragment GetNeighbor(Direction direction)
    {
        switch (direction)
        {

            case Direction.North:
                if (yPos / context.fragmentHeight + 1 >= context.heightInFragments)
                    return null;

                return context.fragments[yPos / context.fragmentHeight + 1][xPos / context.fragmentWidth];

            case Direction.South:
                if (yPos / context.fragmentHeight - 1 < 0)
                    return null;

                return context.fragments[yPos / context.fragmentHeight - 1][xPos / context.fragmentWidth];

            case Direction.East:
                if (xPos / context.fragmentWidth + 1 >= context.widthInFragments)
                    return null;

                return context.fragments[yPos / context.fragmentHeight][xPos / context.fragmentWidth + 1];

            case Direction.West:
                if (xPos / context.fragmentWidth - 1 < 0)
                    return null;

                return context.fragments[yPos / context.fragmentHeight][xPos / context.fragmentWidth - 1];

        }

        return null;
    }

    public int GetWidth()
    {
        return context.fragmentWidth;
    }

    public int GetHeight()
    {
        return context.fragmentHeight;
    }*/

    public bool IsOccupied()
    {
        return (room != null);
    }

}
