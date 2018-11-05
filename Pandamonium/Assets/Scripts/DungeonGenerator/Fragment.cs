using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment {

    Room room;

    private BoardCreator context;

    public int xPos;
    public int yPos;

    public Fragment(BoardCreator context)
    {
        this.context = context;
    }

    public Fragment(BoardCreator context, int xPos, int yPos)
    {
        this.context = context;

        this.xPos = xPos;
        this.yPos = yPos;

    }

    public Fragment(BoardCreator context, int xPos, int yPos, Room room)
    {
        this.context = context;

        this.xPos = xPos;
        this.yPos = yPos;

        this.room = room;

    }

    public void SetRoom(Room room)
    {
        this.room = room;
    }

    public static Fragment GetFragmentAt(BoardCreator bc, int x, int y)
    {

        if (y < 0 || x < 0 || y >= bc.fragmentHeight * bc.heightInFragments || x >= bc.fragmentWidth * bc.widthInFragments)
            return null; ;

        return bc.fragments[y / bc.fragmentHeight][x / bc.fragmentWidth];
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
    }

    public bool IsOccupied()
    {
        return (room != null);
    }

}
