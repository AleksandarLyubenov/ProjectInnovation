using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDrawer : MonoBehaviour
{
    Coroutine drawing;
    public GameObject linePrefab;
    public Transform enemy;
    private LineRenderer currentLine;
    private Vector3 intersectionPoint;

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
        print(currentLine.gameObject.name);

        SetEdgeCollider(currentLine);
        if (Physics2D.Raycast(enemy.position, enemy.transform.right, Mathf.Infinity, lineLayerMask) &&
            Physics2D.Raycast(enemy.position, -enemy.transform.right, Mathf.Infinity, lineLayerMask) &&
            Physics2D.Raycast(enemy.position, enemy.transform.up, Mathf.Infinity, lineLayerMask) &&
            Physics2D.Raycast(enemy.position, -enemy.transform.up, Mathf.Infinity, lineLayerMask))
        {
            print("win");
            GhostMoveToPlayer.Instance.isSpawned = false; // Call the DisableGhost method
        }

        Destroy(currentLine.gameObject);
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

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }
        EdgeCollider2D edgeCollider = currentLine.gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.SetPoints(edges);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(enemy.position, enemy.transform.right);
        Debug.DrawRay(enemy.position, -enemy.transform.right);
        Debug.DrawRay(enemy.position, enemy.transform.up);
        Debug.DrawRay(enemy.position, -enemy.transform.up);
    }
}