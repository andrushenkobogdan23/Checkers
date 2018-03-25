using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckerQueen : Checker
{
    private Dictionary<Vector2, List<Stroke>> beatedEnemiesWithDirection = new Dictionary<Vector2, List<Stroke>>()
    {
        {
            new Vector2(-1, 1), new List<Stroke>()
        },
        {
            new Vector2(-1, -1), new List<Stroke>()
        },
        {
            new Vector2(1, 1), new List<Stroke>()
        },
        {
            new Vector2(1, -1), new List<Stroke>()
        },
    };

    public override List<Stroke> GetPossibleStrokes()
    {
        beatedEnemiesWithDirection[NorthWestDirection] = new List<Stroke>();
        beatedEnemiesWithDirection[SouthWestDirection] = new List<Stroke>();
        beatedEnemiesWithDirection[SouthEathDirection] = new List<Stroke>();
        beatedEnemiesWithDirection[NorthEastDirection] = new List<Stroke>();

        currentStrokes = new List<Stroke>();
        var blockedDirections = new List<Vector2>();
        bool directionIsLocked;

        for (int i = 1; i < 8; i++)
        {
            if (!blockedDirections.Contains(SouthEathDirection) &&
                !beatedEnemiesWithDirection[SouthEathDirection].Any())
            {
                var stroke1 = GetPossibleStrokes(SouthEathDirection, SouthEathDirection * i,
                    CurrentStrokes.Any(s => s.BeatedCheker != null),
                    out directionIsLocked);

                if (!directionIsLocked)
                    currentStrokes.AddRange(stroke1);
                else
                    blockedDirections.Add(SouthEathDirection);
            }

            if (!blockedDirections.Contains(NorthWestDirection) &&
                !beatedEnemiesWithDirection[NorthWestDirection].Any())
            {
                var stroke1 = GetPossibleStrokes(NorthWestDirection, NorthWestDirection * i,
                    CurrentStrokes.Any(s => s.BeatedCheker != null),
                    out directionIsLocked);

                if (!directionIsLocked)
                    currentStrokes.AddRange(stroke1);
                else
                    blockedDirections.Add(NorthWestDirection);
            }

            if (!blockedDirections.Contains(NorthEastDirection) &&
                !beatedEnemiesWithDirection[NorthEastDirection].Any())
            {
                var stroke1 = GetPossibleStrokes(NorthEastDirection, NorthEastDirection * i,
                    CurrentStrokes.Any(s => s.BeatedCheker != null),
                    out directionIsLocked);

                if (!directionIsLocked)
                    currentStrokes.AddRange(stroke1);
                else
                    blockedDirections.Add(NorthEastDirection);
            }

            if (!blockedDirections.Contains(SouthWestDirection) &&
                !beatedEnemiesWithDirection[SouthWestDirection].Any())
            {
                var stroke1 = GetPossibleStrokes(SouthWestDirection, SouthWestDirection * i,
                    CurrentStrokes.Any(s => s.BeatedCheker != null),
                    out directionIsLocked);

                if (!directionIsLocked)
                    currentStrokes.AddRange(stroke1);
                else
                    blockedDirections.Add(SouthWestDirection);
            }
        }

        return currentStrokes;
    }

    private List<Stroke> GetPossibleStrokes(Vector2 direction, Vector2 localPoint, bool mustBeat,
        out bool directionIsBlocked)
    {
        directionIsBlocked = false;
        var possibleStroke = new List<Stroke>();
        if (!board.DictionaryChecker.ContainsKey(Position + localPoint)) return possibleStroke;

        if (board.DictionaryChecker[Position + localPoint] == null && !mustBeat)
            possibleStroke.Add(new Stroke(Position + localPoint, null));
        else if (board.DictionaryChecker.ContainsKey(Position + direction + localPoint))
            if (board.DictionaryChecker[Position + direction + localPoint] == null &&
                board.DictionaryChecker[Position + localPoint] != null)
            {
                var s = new Stroke(Position + direction + localPoint, board.DictionaryChecker[Position + localPoint]);

                if (s.BeatedCheker != null)
                {
                    if (s.BeatedCheker.ChekerColor == ChekerColor)
                    {
                        directionIsBlocked = true;
                        return possibleStroke;
                    }
                    else
                    {
                        if (board.DictionaryChecker.ContainsKey(Position + localPoint - direction) &&
                            board.DictionaryChecker[Position + localPoint - direction] != null &&
                            board.DictionaryChecker[Position + localPoint - direction] != this)
                        {
                            directionIsBlocked = true;
                            return possibleStroke;
                        }
                    }
                }

                possibleStroke.Add(new Stroke(Position + direction + localPoint,
                    board.DictionaryChecker[Position + localPoint]));

                for (int i = 1; i < 8; i++)
                {
                    var storke = (base.GetPossibleStrokes(localPoint + direction * i, false));

                    if (!storke.Any() || (!board.DictionaryChecker.ContainsKey(Position + direction + localPoint * i)))
                    {
                        return possibleStroke;
                    }

                    var resultStroke = new Stroke(Position + localPoint + direction * i,
                        board.DictionaryChecker[Position + localPoint]);

                    possibleStroke.Add(resultStroke);

                    if (board.DictionaryChecker[Position + localPoint] != null)
                        beatedEnemiesWithDirection[direction].Add(resultStroke);
                }
            }

        return possibleStroke;
    }
}