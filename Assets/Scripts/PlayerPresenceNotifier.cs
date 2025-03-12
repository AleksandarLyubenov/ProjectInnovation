using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerPresenceNotifier : MonoBehaviour
{
    public static PlayerPresenceNotifier Instance { get; private set; }

    public UnityEvent OnPlayerFound = new UnityEvent();
    public UnityEvent OnPlayerLost = new UnityEvent();

    private GameObject currentPlayer;
    private Coroutine checkCoroutine;
    private bool lastPlayerState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        checkCoroutine = StartCoroutine(PlayerCheckRoutine());
    }

    private IEnumerator PlayerCheckRoutine()
    {
        while (true)
        {
            var player = GameObject.FindWithTag("Player");
            bool playerExists = player != null;

            if (playerExists != lastPlayerState)
            {
                lastPlayerState = playerExists;
                currentPlayer = player;

                if (playerExists)
                    OnPlayerFound.Invoke();
                else
                    OnPlayerLost.Invoke();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public bool HasPlayer() => lastPlayerState;
    public GameObject GetPlayer() => currentPlayer;

    private void OnDestroy()
    {
        if (checkCoroutine != null)
            StopCoroutine(checkCoroutine);
    }
}