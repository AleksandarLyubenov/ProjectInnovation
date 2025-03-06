using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDrawer : MonoBehaviour
{
    Coroutine drawing;
    public GameObject linePrefab;
    public List<Transform> enemies;
    private LineRenderer currentLine;
    private PolygonCollider2D polygonCollider;

    public LayerMask lineLayerMask;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            FinishLine();
        }
    }

    void StartLine()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }

    void FinishLine()
    {
        if (drawing != null)
            StopCoroutine(drawing);
        Debug.Log(this.currentLine.gameObject.name);

        SetPolygonCollider(currentLine);

        foreach (Transform enemy in enemies)
        {
            if (this.polygonCollider.OverlapPoint(enemy.position))
            {
                Debug.Log("win");
                GhostMoveToPlayer ghostScript = enemy.GetComponent<GhostMoveToPlayer>();
                if (ghostScript != null)
                {
                    ghostScript.isSpawned = false; // Call the DisableGhost method
                    ghostScript.DisableGhost(); // Disable the ghost
                }
                break; // ensure only one enemy is being deleted (chooses the first enemy in the list)
            }
        }

        Destroy(this.currentLine.gameObject);
    }

    IEnumerator DrawLine()
    {
        GameObject lineObject = Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        LineRenderer line = lineObject.GetComponent<LineRenderer>();
        line.positionCount = 0;

        while (true)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
            currentLine = line;
            yield return null;
        }
    }

    void SetPolygonCollider(LineRenderer lineRenderer)
    {
        List<Vector2> points = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            points.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        if (polygonCollider == null)
        {
            polygonCollider = this.currentLine.gameObject.AddComponent<PolygonCollider2D>();
        }
        this.polygonCollider.SetPath(0, points.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (enemies != null)
        {
            foreach (Transform enemy in enemies)
            {
                Debug.DrawRay(enemy.position, enemy.transform.right);
                Debug.DrawRay(enemy.position, -enemy.transform.right);
                Debug.DrawRay(enemy.position, enemy.transform.up);
                Debug.DrawRay(enemy.position, -enemy.transform.up);
            }
        }
    }
}