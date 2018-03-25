using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private GameObject selectOutline;

    public virtual List<Stroke> CurrentStrokes
    {
        get { return currentStrokes; }
    }

    protected List<Stroke> currentStrokes = new List<Stroke>();
    protected Board board;
    public Vector2 Position;
    public bool selected;
    public bool isQueen = false;
    public ChekerColor ChekerColor { get; private set; }

    protected readonly Vector2 NorthWestDirection = new Vector2(-1, 1);
    protected readonly Vector2 NorthEastDirection = new Vector2(1, 1);
    protected readonly Vector2 SouthEathDirection = new Vector2(1, -1);
    protected readonly Vector2 SouthWestDirection = new Vector2(-1, -1);

    public void Initialize(Board board, Vector2 position, ChekerColor color)
    {
        this.board = board;
        Position = position;
        ChekerColor = color;
    }

    public bool MakeAStroke(Vector2 point, out Checker beatedCheker)
    {
        if (Canbeat())
        {
            currentStrokes = GetPossibleStrokes();
            beatedCheker = CurrentStrokes.FirstOrDefault(c => c.Point == point && c.BeatedCheker != null).BeatedCheker;
            return currentStrokes.Any(s => s.Point == point && s.BeatedCheker != null);
        }


        currentStrokes = GetPossibleStrokes();
        beatedCheker = CurrentStrokes.FirstOrDefault(c => c.Point == point).BeatedCheker;
        return currentStrokes.Any(s => s.Point == point);
    }

    public bool Canbeat()
    {
        CurrentStrokes.Clear();
        GetPossibleStrokes();

        return CurrentStrokes.Any(s => s.BeatedCheker != null);
    }

    public bool Select()
    {
        currentStrokes = GetPossibleStrokes();

        if (!currentStrokes.Any() || selected)
        {
            Deselect();
            return false;
        }

        selectOutline.SetActive(true);
        selected = true;
        return true;
    }

    public void Deselect()
    {
        selectOutline.SetActive(false);
        CurrentStrokes.Clear();
        selected = false;
    }


    public virtual List<Stroke> GetPossibleStrokes(Vector2 localPoint, bool mustBeat)
    {
        var possibleStroke = new List<Stroke>();

        if (!board.DictionaryChecker.ContainsKey(Position + localPoint)) return possibleStroke;

        if (board.DictionaryChecker[Position + localPoint] == null && !mustBeat)
            possibleStroke.Add(new Stroke(Position + localPoint, null));

        else if (board.DictionaryChecker.ContainsKey(Position + 2 * localPoint))
            if (board.DictionaryChecker[Position + 2 * localPoint] == null &&
                board.DictionaryChecker[Position + localPoint] != null)
            {
                var s = new Stroke(Position + 2 * localPoint, board.DictionaryChecker[Position + localPoint]);

                if (s.BeatedCheker != null)
                {
                    if (s.BeatedCheker.ChekerColor == ChekerColor) return possibleStroke;
                }

                possibleStroke.Add(
                    new Stroke(Position + 2 * localPoint, board.DictionaryChecker[Position + localPoint]));
            }

        return possibleStroke;
    }

    public virtual List<Stroke> GetPossibleStrokes()
    {
        currentStrokes = new List<Stroke>();
        var canStrokeUp = ChekerColor == ChekerColor.White || currentStrokes.Any(s => s.BeatedCheker != null);

        currentStrokes.AddRange(GetPossibleStrokes(NorthWestDirection, !canStrokeUp));
        currentStrokes.AddRange(GetPossibleStrokes(NorthEastDirection, !canStrokeUp));
        currentStrokes.AddRange(GetPossibleStrokes(SouthWestDirection, canStrokeUp));
        currentStrokes.AddRange(GetPossibleStrokes(SouthEathDirection, canStrokeUp));

        return CurrentStrokes;
    }
}


public struct Stroke
{
    public Vector2 Point;
    public Checker BeatedCheker;

    public Stroke(Vector2 point, Checker checker)
    {
        Point = point;
        BeatedCheker = checker;
    }
}

public enum ChekerColor
{
    White,
    Black
}