using TMPro;
using UnityEngine;

public class BookContent : MonoBehaviour
{
    [TextArea(10, 20)]
    public string content;
    [Space]
    [SerializeField] private TextMeshProUGUI leftSide;
    [SerializeField] private TMP_Text rightSide;
    [Space]
    [SerializeField] private TMP_Text leftPagination;
    [SerializeField] private TMP_Text rightPagination;

    private void OnValidate()
    {
        UpdatePagination();

        if (leftSide.text == content) return;

        SetupContent();
    }

    private void Awake()
    {
        UpdatePagination();
        SetupContent();
    }

    private void SetupContent()
    {
        leftSide.text = content;
        rightSide.text = content;
    }

    private void UpdatePagination()
    {
        leftPagination.text = leftSide.pageToDisplay.ToString();
        rightPagination.text = rightSide.pageToDisplay.ToString();
    }

    public void PreviousPage()
    {
        if (leftSide.pageToDisplay < 1)
        {
            leftSide.pageToDisplay = 1;
            return;
        }

        if (leftSide.pageToDisplay - 2 > 1)
        {
            leftSide.pageToDisplay -= 2;
        }
        else
        {
            leftSide.pageToDisplay = 1;
        }

        rightSide.pageToDisplay = leftSide.pageToDisplay + 1;

        UpdatePagination();
    }

    public void NextPage()
    {
        if (rightSide.pageToDisplay >= rightSide.textInfo.pageCount) return;

        if (leftSide.pageToDisplay >= leftSide.textInfo.pageCount -1 )
        {
            leftSide.pageToDisplay = leftSide.textInfo.pageCount - 1;
            rightSide.pageToDisplay = leftSide.textInfo.pageCount + 1;
        }
        else
        {
            leftSide.pageToDisplay += 2;
            rightSide.pageToDisplay = leftSide.pageToDisplay + 1;
        }

        UpdatePagination();
    }
}
