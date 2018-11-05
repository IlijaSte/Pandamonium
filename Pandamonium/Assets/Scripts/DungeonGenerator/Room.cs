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
            // If the corridor entering this room is going north...
            case Direction.North:
                // ... the height of the room mustn't go beyond the board so it must be clamped based
                // on the height of the board (rows) and the end of corridor that leads to the room.
                roomHeight = Mathf.Clamp(roomHeight, 1, fragment.yPos + fragment.GetHeight() - corridor.EndPositionY - 2);

                // The y coordinate of the room must be at the end of the corridor (since the corridor leads to the bottom of the room).
                yPos = corridor.EndPositionY;

                // The x coordinate can be random but the left-most possibility is no further than the width
                // and the right-most possibility is that the end of the corridor is at the position of the room.
                
                xPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionX - roomWidth, fragment.xPos + 1, corridor.EndPositionX - roomWidth), Mathf.Clamp(fragment.xPos + fragment.GetWidth() - roomWidth - 1, fragment.xPos + 1, fragment.xPos + fragment.GetWidth() - roomWidth - 1));

                do
                {
                    roomWidth = widthRange.Random;
                    xPos = UnityEngine.Random.Range(fragment.xPos, fragment.xPos + fragment.GetWidth() - roomWidth);
                } while (xPos > corridor.EndPositionX || xPos + roomWidth - 1 < corridor.EndPositionX);

                // This must be clamped to ensure that the room doesn't go off the board.
                //xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.East:
                //roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
                roomWidth = Mathf.Clamp(roomWidth, 1, fragment.xPos + fragment.GetWidth() - corridor.EndPositionX - 1);

                xPos = corridor.EndPositionX;

                yPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionY - roomHeight, fragment.yPos + 1, corridor.EndPositionY - roomHeight + 1), Mathf.Clamp(fragment.yPos + fragment.GetHeight() - roomHeight - 1, fragment.yPos + 1, fragment.yPos + fragment.GetHeight() - roomHeight - 1));

                do
                {
                    roomHeight = heightRange.Random;
                    yPos = UnityEngine.Random.Range(fragment.yPos, fragment.yPos + fragment.GetHeight() - roomHeight);
                } while (yPos > corridor.EndPositionY || yPos + roomHeight - 1 < corridor.EndPositionY);

                //yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
            case Direction.South:
                //roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY);

                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY - (fragment.yPos + 1));


                yPos = corridor.EndPositionY - roomHeight; // + 1 ?

                xPos = UnityEngine.Random.Range(Mathf.Clamp(corridor.EndPositionX - roomWidth, fragment.xPos + 1, corridor.EndPositionX - roomWidth), Mathf.Clamp(fragment.xPos + fragment.GetWidth() - roomWidth - 1, fragment.xPos + 1, fragment.xPos + fragment.GetWidth() - roomWidth - 1));

                do
                {
                    roomWidth = widthRange.Random;
                    xPos = UnityEngine.Random.Range(fragment.xPos, fragment.xPos + fragment.GetWidth() - roomWidth);
                } while (xPos > corridor.EndPositionX || xPos + roomWidth - 1 < corridor.EndPositionX);

                //xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.West:
                //roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);

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