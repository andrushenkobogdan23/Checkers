using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField] private Cell blackCell;
    [SerializeField] private GameObject blackCheker;
    [SerializeField] private Cell whiteCell;
    [SerializeField] private GameObject whiteChecker;
    [SerializeField] private CheckerQueen whitecheckerQueen;
    [SerializeField] private CheckerQueen blackcheckerQueen;
    [SerializeField] private UIManager uIManager;
    private float chekerSize;
    private const int RowsCount = 8;
    public Dictionary<Vector2, Checker> DictionaryChecker = new Dictionary<Vector2, Checker>();
    private Dictionary<Vector2, Cell> DictionaryCell = new Dictionary<Vector2, Cell>();
    private bool inChainsStroke = false;
    public bool StepIsLocked = false;
    private Checker selectedCheker;
    public ChekerColor curentStroke = ChekerColor.White;

    private void Start()
    {
        chekerSize = whiteCell.GetComponent<SpriteRenderer>().size.x;
        CreateBoard();
        SetChekers();
    }

    private void SelectCell(Checker selectChecker)
    {
        var possibleStroke = !selectChecker.Canbeat()
            ? selectChecker.GetPossibleStrokes()
            : selectChecker.GetPossibleStrokes().Where(s => s.BeatedCheker != null).ToList();

        foreach (var cell in possibleStroke)
        {
            DictionaryCell[cell.Point].SelectCell();
        }
    }

    private void DeSelectCell()
    {
        foreach (Cell cell in DictionaryCell.Values.Where(c => c.IsSelect))
        {
            if (cell != null)
                cell.DeSelectCell();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            DeSelectCell();
            if (Physics.Raycast(ray, out hit))
                if (hit.transform.tag == "Cheker")
                {
                    var chekker = hit.transform.GetComponent<Checker>();
                    if (chekker.ChekerColor == curentStroke)
                    {
                        OnChekerSelkected(chekker);
                        if (!StepIsLocked && chekker != null)
                            SelectCell(chekker);
                        uIManager.UpdeteText();
                    }
                }
                else if (hit.transform.tag == "Cell" && selectedCheker != null)
                {
                    DeSelectCell();
                    MakeMove(hit.transform.GetComponent<Cell>().Position);
                    uIManager.UpdeteText();
                }
        }
    }

    private void CreateBoard()
    {
        for (var i = 1; i <= RowsCount; i++)
        for (var j = 1; j <= RowsCount; j++)
        {
            var position = transform.TransformPoint(new Vector3(i * chekerSize, j * chekerSize, 1));
            var cell = Instantiate((i + j) % 2 == 0 ? blackCell : whiteCell, position, Quaternion.identity, transform);
            cell.GetComponent<Cell>().Position = new Vector2(i, j);
            DictionaryCell.Add(new Vector2(i, j), cell);
            DictionaryChecker.Add(new Vector2(i, j), null);
        }
    }

    private void SetChekers()
    {
        for (var i = 1; i <= RowsCount; i++)
        for (var j = 1; j <= 3; j++)
        {
            if ((i + j) % 2 != 0)
                continue;
            var pos = transform.TransformPoint(new Vector3(i * chekerSize, j * chekerSize, 0));

            var checker = Instantiate(whiteChecker, pos,
                Quaternion.identity, transform).GetComponent<Checker>();
            checker.Initialize(this, new Vector2(i, j), ChekerColor.White);
            DictionaryChecker[new Vector2(i, j)] = checker;
        }

        for (var i = 1; i <= RowsCount; i++)
        for (var j = 6; j <= 8; j++)
        {
            if ((i + j) % 2 != 0)
                continue;
            var pos = transform.TransformPoint(new Vector3(i * chekerSize, j * chekerSize, 0));
            var checker = Instantiate(blackCheker, pos,
                Quaternion.identity, transform).GetComponent<Checker>();
            checker.Initialize(this, new Vector2(i, j), ChekerColor.Black);
            DictionaryChecker[new Vector2(i, j)] = checker;
        }
    }


    private void MakeMove(Vector2 point)
    {
        Checker beatedCheker;
        if (!selectedCheker.MakeAStroke(point, out beatedCheker))
        {
            Debug.LogWarning("Can't make strroke");
            return;
        }

        selectedCheker.transform.position =
            transform.TransformPoint(new Vector3(point.x * chekerSize, point.y * chekerSize, 0));
        DictionaryChecker[selectedCheker.Position] = null;
        DictionaryChecker[point] = selectedCheker;
        selectedCheker.Position = point;

        if (beatedCheker != null)
        {
            DeleteChecker(beatedCheker);

            if (selectedCheker.Canbeat())
            {
                if (selectedCheker != null)
                    SelectCell(selectedCheker);
                inChainsStroke = true;
                return;
            }
        }

        if (selectedCheker.ChekerColor == ChekerColor.White)
        {
            if (point.y == 8 && !selectedCheker.isQueen)
                MakeSelectedCheckerQueen();
        }
        else if (point.y == 1 && !selectedCheker.isQueen)
        {
            MakeSelectedCheckerQueen();
        }

        DeselectChecker();
        curentStroke = curentStroke == ChekerColor.White ? ChekerColor.Black : ChekerColor.White;
    }

    private void OnChekerSelkected(Checker cheker)
    {
        StepIsLocked = false;

        if (inChainsStroke) return;
        DeselectChecker();

        if (!cheker.Select())
        {
            DeSelectCell();
            Debug.LogWarning("Can't select cheker, no possible sttrokes");
            return;
        }

        var checkersValidated = new List<Checker>();

        if (!cheker.Canbeat())
        {
            foreach (Checker checker in DictionaryChecker.Values.Where(c => c != null && c.ChekerColor == curentStroke))
            {
                if (checker.Canbeat()) checkersValidated.Add(checker);
            }

            if (checkersValidated.Any())
            {
                cheker.Deselect();
                DeSelectCell();
                StepIsLocked = true;
                Debug.LogWarning("Can't select cheker, there are checkers that can beat ");
                return;
            }
        }

        selectedCheker = cheker;
    }

    private void DeselectChecker()
    {
        inChainsStroke = false;

        if (selectedCheker == null) return;
        selectedCheker.Deselect();
        selectedCheker = null;
    }

    private void DeleteChecker(Checker checker)
    {
        DictionaryChecker[checker.Position] = null;
        Destroy(checker.gameObject);
    }

    private void MakeSelectedCheckerQueen()
    {
        var queenPrefab = selectedCheker.ChekerColor == ChekerColor.White ? whitecheckerQueen : blackcheckerQueen;
        var position = selectedCheker.Position;
        var pos = transform.TransformPoint(new Vector3(position.x * chekerSize, position.y * chekerSize, 0));
        DeleteChecker(selectedCheker);

        var queen = Instantiate(queenPrefab, pos, Quaternion.identity, transform);
        queen.Initialize(this, position, selectedCheker.ChekerColor);
        DictionaryChecker[new Vector2(position.x, position.y)] = queen;
    }
}