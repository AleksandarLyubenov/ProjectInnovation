using UnityEngine;

public class CircleInstance : MonoBehaviour
{
    [SerializeField] private float shrinkTime = 1f;
    [SerializeField] private float perfectWindow = 0.2f;

    private float timer;
    private bool wasTapped;
    private CirclesManager manager;
    private Transform innerCircle;

    void Awake()
    {
        manager = FindObjectOfType<CirclesManager>();
        innerCircle = transform.GetChild(0);

        transform.position = new Vector3(
        transform.position.x,
        transform.position.y,
        -5f 
        );
    }

    void Update()
    {
        if (wasTapped) return;

        timer += Time.deltaTime;
        UpdateScale();

        if (timer >= shrinkTime + perfectWindow)
        {
            manager.ReportCircleResult(false);
            Destroy(gameObject);
        }
    }

    void UpdateScale()
    {
        float scale = Mathf.Lerp(2.5f, 1f, timer / shrinkTime);
        innerCircle.localScale = Vector3.one * scale;
    }

    public void HandleTap()
    {
        if (wasTapped) return;

        wasTapped = true;
        bool isPerfect = Mathf.Abs(timer - shrinkTime) <= perfectWindow;
        manager.ReportCircleResult(isPerfect);
        Destroy(gameObject);
    }
}