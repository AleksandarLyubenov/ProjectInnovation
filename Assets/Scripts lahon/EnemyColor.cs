using UnityEngine;

public class EnemyColor : MonoBehaviour
{
    public int enemyType; // 1, 2, or 3
    public Color enemyColor; // Color to represent the type
    private Color originalColor; // Store the original sprite color

    private void Start()
    {
        // Store the original sprite color
        originalColor = GetComponent<Renderer>().material.color;
        SetColor();
    }

    public void SetColor()
    {
        // Set the color based on the enemy type
        switch (enemyType)
        {
            case 1:
                enemyColor = Color.red;
                break;
            case 2:
                enemyColor = Color.blue;
                break;
            case 3:
                enemyColor = originalColor;
                break;
            default:
                // Keep the original color for the default case
                enemyColor = originalColor;
                break;
        }

        GetComponent<Renderer>().material.color = enemyColor;
    }

    public void RandomizeColor()
    {
        enemyType = Random.Range(1, 4); // Randomly select between 1, 2, and 3
        SetColor();
    }
}