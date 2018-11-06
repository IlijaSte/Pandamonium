using UnityEngine;
using System;

public class Room
{
    public int xPos;                      // The x coordinate of the lower left tile of the room.
    public int yPos;                      // The y coordinate of the lower left tile of the room.
    public int roomWidth;                     // How many tiles wide the room is.
    public int roomHeight;                    // How many tiles high the room is.
    public Direction enteringCorridor;    // The direction of the corridor that is entering this room.

    private BoardCreator context;
    public Fragment fragment;

    public Room(BoardCreator context)
    {
        this.context = context;
    }

    // This is used for the first room.  It does not have a Corridor parameter since there are no corridors yet.
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        // Set a random width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        // Set the x and y coordinates so the room is roughly in the middle of the board.
        System.Random random = new System.Random();

        xPos = 1;
        yPos = random.Next(0, rows - roomHeight / 2);

        (fragment = Fragment.GetFragmentAt(context, 0, random.Next(0, rows - roomHeight / 2))).SetRoom(this);

        yPos = random.Next(fragment.yPos, fragment.yPos + 3);
    }


    // This is an overload of the SetupRoom function and has a corridor parameter that represents the corridor entering the room.
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor)
    {
        // Set the entering corridor direction.
        enteringCorridor = corridor.direction;

        // Set random values for width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        fragment = corridor.to;
        fragment.SetRoom(this);

        switch (corridor.direction)
        {
            case Direction.North:

                roomHeight = Mathf.Clamp(roomHeight, 1, fragment.yPos + fragment.GetHeight() - corridor.EndPositionY - 2);

                yPos = corridor.EndPositionY;

                xPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionX - roomWidth, fragment.xPos + 1, corridor.EndPositionX - roomWidth), Mathf.Clamp(fragment.xPos + fragment.GetWidth() - roomWidth - 1, fragment.xPos + 1, fragment.xPos + fragment.GetWidth() - roomWidth - 1));

                do
                {
                    roomWidth = widthRange.Random;
                    xPos = UnityEngine.Random.Range(fragment.xPos, fragment.xPos + fragment.GetWidth() - roomWidth);
                } while (xPos > corridor.EndPositionX || xPos + roomWidth - 1 < corridor.EndPositionX);

                break;
            case Direction.East:

                roomWidth = Mathf.Clamp(roomWidth, 1, fragment.xPos + fragment.GetWidth() - corridor.EndPositionX - 1);

                xPos = corridor.EndPositionX;

                yPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionY - roomHeight, fragment.yPos + 1, corridor.EndPositionY - roomHeight + 1), Mathf.Clamp(fragment.yPos + fragment.GetHeight() - roomHeight - 1, fragment.yPos + 1, fragment.yPos + fragment.GetHeight() - roomHeight - 1));

                do
                {
                    roomHeight = heightRange.Random;
                    yPos = UnityEngine.Random.Range(fragment.yPos, fragment.yPos + fragment.GetHeight() - roomHeight);
                } while (yPos > corridor.EndPositionY || yPos + roomHeight - 1 < corridor.EndPositionY);

                break;
            case Direction.South:

                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY - (fragment.yPos + 1));

                yPos = corridor.EndPositionY - roomHeight; // + 1 ?

                xPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionX - roomWidth, fragment.xPos + 1, corridor.EndPositionX - roomWidth), Mathf.Clamp(fragment.xPos + fragment.GetWidth() - roomWidth - 1, fragment.xPos + 1, fragment.xPos + fragment.GetWidth() - roomWidth - 1));

                do
                {
                    roomWidth = widthRange.Random;
                    xPos = UnityEngine.Random.Range(fragment.xPos, fragment.xPos + fragment.GetWidth() - roomWidth);
                } while (xPos > corridor.EndPositionX || xPos + roomWidth - 1 < corridor.EndPositionX);

                break;
            case Direction.West:

                roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX - (fragment.xPos + 1));

                xPos = corridor.EndPositionX - roomWidth;

                yPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionY - roomHeight, fragment.yPos + 1, corridor.EndPositionY - roomHeight + 1), Mathf.Clamp(fragment.yPos + fragment.GetHeight() - roomHeight - 1, fragment.yPos + 1, fragment.yPos + fragment.GetHeight() - roomHeight - 1));

                do
                {
                    roomHeight = heightRange.Random;
                    yPos = UnityEngine.Random.Range(fragment.yPos, fragment.yPos + fragment.GetHeight() - roomHeight);
                } while (yPos > corridor.EndPositionY || yPos + roomHeight - 1 < corridor.EndPositionY);

                break;
        }
    }

}