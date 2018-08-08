using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    private int rows = 5, cols = 6, score, total;

    public void ShowResults(Player[] players)
    {
        for (int row = 0; row < rows; row++)
        {
            total = 0;

            for (int col = 0; col < cols; col++)
            {
                #region color

                if (row == 0)
                {
                    if (col == 0 || col == 5)
                        continue;

                    if (col - 1 < players.Length)
                        transform.GetChild(row).GetChild(col).GetComponent<TextMeshPro>().color = players[col - 1].GetColor();
                }
                else
                {
                    if (row - 1 < players.Length)
                        transform.GetChild(row).GetChild(col).GetComponent<TextMeshPro>().color = players[row - 1].GetColor();
                }

                #endregion

                #region score

                if (row == 0 || col == 0)
                    continue;

                if (row > players.Length)
                    continue;

                if (col == 5)
                    transform.GetChild(row).GetChild(col).GetComponent<TextMeshPro>().text = total.ToString();
                else if (row == col || col - 1 >= players.Length)
                    transform.GetChild(row).GetChild(col).GetComponent<TextMeshPro>().text = "-";
                else
                {
                    score = players[row - 1].kills[col - 1];
                    total += score;

                    transform.GetChild(row).GetChild(col).GetComponent<TextMeshPro>().text = score.ToString();
                }

                #endregion
            }
        }
    }
}
