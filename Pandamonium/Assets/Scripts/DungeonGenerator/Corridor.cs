using UnityEngine;

// Enum to specify the direction is heading.
public enum Direction
{
    North, East, South, West,
}

public class Corridor
{
    public int startXPos;         // The x coordinate for the start of the corridor.
    public int startYPos;         // The y coordinate for the start of the corridor.
    public int corridorLength;            // How many units long the corridor is.
    public Direction direction;   // Which direction the corridor is heading from it's room.

    public BoardCreator context;

    public Fragment from;
    public Fragment to;

    // Get the end position of the corridor based on it's start position and which direction it's heading.
    public int EndPositionX
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
                return startXPos;
            if (direction == Direction.East)
                return startXPos + corridorLength - 1;
            return startXPos - corridorLength + 1;
        }
    }


    public int EndPositionY
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
                return startYPos;
            if (direction == Direction.North)
                return startYPos + corridorLength - 1;
            return startYPos - corridorLength + 1;
        }
    }

    public Corridor(BoardCreator context, Fragment from = null)
    {
        this.context = context;
        this.from = from;
    }

    public bool SetupCorridor(Room room, IntRange length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, bool firstCorridor = false, Direction firstDirection = Direction.East)
    {

        Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);

        bool validDirection;

        if (firstCorridor)
        {
            direction = firstDirection;
        }
        else
        {
            direction = (Direction)Random.Range(0, 4);
        }

        int i = 0;

        do
        {
            validDirection = true;
            
            if (!firstCorridor && direction == oppositeDirection)
            {

                direction = (Direction)(((int)direction + 1) % 4);

            }

            to = from.GetNeighbor(direction);

            if (from != null && (to == null || to.IsOccupied()))
            {
                i++;
                validDirection = false;
                direction = (Direction)(((int)direction + 1) % 4);
                continue;
            }

            corridorLength = length.Random;

            switch (direction)
            {

                case Direction.North:

                    startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth - 1);

                    startYPos = room.yPos + room.roomHeight;

                    corridorLength = (to.yPos - startYPos >= corridorLength ? to.yPos - startYPos + length.Random : corridorLength);

                    break;
                case Direction.East:
                    startXPos = room.xPos + room.roomWidth;
                    startYPos = Random.Range(room.yPos, room.yPos + room.roomHeight - 1);

                    corridorLength = (to.xPos - startXPos >= corridorLength ? to.xPos - startXPos + length.Random : corridorLength);

                    break;
                case Direction.South:
                    startXPos = Random.Range(room.xPos + 1, room.xPos + room.roomWidth - 1);
                    startYPos = room.yPos - 1;

                    corridorLength = (startYPos - (to.yPos + to.GetHeight()) >= corridorLength ? startYPos - (to.yPos + to.GetHeight()) + length.Random : corridorLength);

                    break;
                case Direction.West:
                    startXPos = room.xPos - 1;
                    startYPos = Random.Range(room.yPos + 1, room.yPos + room.roomHeight - 1);

                    corridorLength = (startXPos - (to.xPos + to.GetWidth()) >= corridorLength ? startXPos - (to.xPos + to.GetWidth()) + length.Random : corridorLength);

                    break;
            }

        } while (!validDirection && i < 4);

        if(i == 4 && !validDirection)
        {
            return false;
        }

        

        return true;
    }
}