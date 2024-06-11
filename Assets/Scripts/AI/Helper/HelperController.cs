using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HelperController : MonoBehaviour
{
    [SerializeField] private HelperStackHandler helperStackHandler;
    [SerializeField] private Transform wasteBin;
    [SerializeField] private Transform wastePoint;
    [SerializeField] private HelperTargetHandler targetHandler;

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    private Rigidbody rb;

    private Transform currentSaleListTransform;
    private Transform currentAisleTransform;

    private int currentTargetIndex;
    private string currentState;

    private const string HELPER_IDLE = "Idle";
    private const string HELPER_RUN = "Run";
    private const string HELPER_PICK = "Pick";
    private const string HELPER_TURN = "Turn";

    private bool movingToAisle;
    private bool isDropPointFull;
    private bool canGoWasteBin;

    private bool isWaitingForTurn;

    private void Awake()
    {
        InternalEvents.AllDropPointFull += OnAllDropPointFull;
    }

    private void OnDisable()
    {
        InternalEvents.AllDropPointFull -= OnAllDropPointFull;
    }

    private void OnAllDropPointFull()
    {
        isDropPointFull = true;
        StartCoroutine(DroppingDuration());
    }

    private IEnumerator DroppingDuration()
    {
        ChangeAnimationState(HELPER_IDLE);
        yield return new WaitForSeconds(10f);
        canGoWasteBin = true;
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent.stoppingDistance = 0.5f;
        SetNextTarget();
        GoToNextTarget();
    }

    private void Update()
    {
        if (isWaitingForTurn) return;

        if (!isDropPointFull)
        {
            GoToNextTarget();
        }
        else
        {
            if (canGoWasteBin)
            {
                navMeshAgent.SetDestination(wasteBin.position);
                ChangeAnimationState(HELPER_PICK);
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                {
                    helperStackHandler.DropProductToBin(wastePoint);
                    canGoWasteBin = false;
                    isDropPointFull = false;
                    ChangeAnimationState(HELPER_RUN);
                }
            }
        }
    }

    private void SetNextTarget()
    {
        (currentSaleListTransform, currentAisleTransform) = targetHandler.GetNextTarget();
        movingToAisle = false;
        Debug.Log("Next Target Set: " + currentSaleListTransform + ", " + currentAisleTransform);
    }

    private void GoToNextTarget()
    {
        if (currentSaleListTransform == null || currentAisleTransform == null)
        {
            SetNextTarget();
            return;
        }

        if (!movingToAisle)
        {
            Vector3 targetPoint = GetTargetPoint(currentSaleListTransform);
            navMeshAgent.SetDestination(targetPoint);
            ChangeAnimationState(HELPER_RUN);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                ChangeAnimationState(HELPER_PICK);
                movingToAisle = true;
                GoToNextTarget();
            }
        }
        else
        {
            Vector3 targetPoint = GetTargetPoint(currentAisleTransform);
            navMeshAgent.SetDestination(targetPoint);
            ChangeAnimationState(HELPER_RUN);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                ChangeAnimationState(HELPER_RUN);
                SetNextTarget();
            }
        }
    }

    private Vector3 GetTargetPoint(Transform target)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 targetPoint = targetCollider.ClosestPoint(transform.position + directionToTarget * navMeshAgent.stoppingDistance);
            return targetPoint;
        }
        return target.position;
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }
}
