using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle _navMeshObstacle;

    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform basketPoint;
    [SerializeField] private Transform boxPoint;

    [SerializeField] private AI_UIController aiUIController;

    [SerializeField] private GameObject basket;
    [SerializeField] private GameObject cartonBox;
    [SerializeField] private GameObject money;

    [SerializeField] private float productSize;
    [SerializeField] private float stoppingDistance;


    private List<Transform> cashierPointList = new();
    private readonly List<Transform> collectedItems = new();
    public List<TargetItemPair> targetsToVisit = new();


    private int productCountToPrize;
    private int currentProductIndex;

    internal Vector3 linePosition;

    private NavMeshAgent agent;

    internal Transform _cashier;
    private Transform exitPoint;
    private TargetItemPair currentTarget;

    public Enums.ProductType type;

    private CheckoutInteractable _checkoutInteractable;

    private GameObject basketGO;
    private GameObject boxGO;
    private GameObject moneyGO;

    private Coroutine productMoveCoroutine;

    internal bool cashProcessEnd;
    private bool hasProcessed;
    internal bool isProductInBox;

    private Animator _animator;

    private const string AI_STANDING = "Standing";
    private const string AI_WALK = "Walking_Bag";
    private const string AI_BOX_WALK = "Walking_Box";

    private string currentState;


    public int CurrentNeededCount
    {
        get
        {
            if (targetsToVisit.Count == 0) return 0;
            return Mathf.Max(0, targetsToVisit[0].ItemCount - collectedItems.Count);
        }
    }

    [Button]
    public void DebugCurrentNeededCount()
    {
        Debug.Log(CurrentNeededCount);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshObstacle.enabled = false;
    }

    private void Update()
    {
        if (!cashProcessEnd)
            MoveTarget();
    }


    private void MoveTarget()
    {
        if (targetsToVisit.Count > 0)
        {
            currentTarget = targetsToVisit[0];
            var target = currentTarget.Target;
            type = currentTarget.ProductType;
            var itemCount = targetsToVisit[0].ItemCount;

            aiUIController.ProductSprite(type);
            aiUIController.MaxItemCount(itemCount);

            if (Vector3.Distance(currentTarget.Target.position, transform.position) > stoppingDistance)
            {
                SetDestination(target);

                if (currentState != AI_WALK) ChangeAnimationState(AI_WALK);
            }
            else
            {
                SetStopped();
                if (currentState != AI_STANDING) ChangeAnimationState(AI_STANDING);
            }
        }

        else
        {
            agent.stoppingDistance = 1.5f;
            CashierIcon();

            if (Vector3.Distance(linePosition, transform.position) > agent.stoppingDistance)
            {
                SetTarget(linePosition);
                if (currentState != AI_WALK) ChangeAnimationState(AI_WALK);
            }

            else
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
                SetStopped();

                if (currentState != AI_STANDING) ChangeAnimationState(AI_STANDING);
            }
        }
    }

    private void SetStopped()
    {
        agent.enabled = false;
        _navMeshObstacle.enabled = true;
    }

    private void SetDestination(Transform target)
    {
        _navMeshObstacle.enabled = false;

        if (!agent.enabled) agent.enabled = true;
        agent.SetDestination(target.position);
    }

    private void SetTarget(Vector3 cashierTarget)
    {
        _navMeshObstacle.enabled = false;

        if (!agent.enabled) agent.enabled = true;

        agent.SetDestination(cashierTarget);
    }

    public void InitializeTargets(List<Transform> targets, int maxItem, Transform cashierPoint,
        List<Transform> productMoveList, Transform spawnPoint, CheckoutInteractable aiCashLine)
    {
        if (targets == null || targets.Count == 0 || cashierPoint == null)
            throw new ArgumentException("Target list cannot be null or empty.");

        _cashier = cashierPoint;
        exitPoint = spawnPoint;
        cashierPointList = productMoveList;
        _checkoutInteractable = aiCashLine;

        var shuffledPoints = new List<Transform>(targets);
        Shuffle(shuffledPoints);

        int targetToVisitCount = Random.Range(1, shuffledPoints.Count + 1);
        var remainingItemCount = maxItem;

        basketGO = Instantiate(basket, basketPoint.position, Quaternion.Euler(0, 90, 0));
        basketGO.transform.SetParent(transform);

        for (var i = 0; i < targetToVisitCount; i++)
        {
            var target = shuffledPoints[i];
            var productTypeIcon = target.GetComponent<ProductTypeIcon>();
            var type = productTypeIcon.productType;

            var randomItem = Random.Range(1, Mathf.Min(remainingItemCount, maxItem) + 1);
            remainingItemCount -= randomItem;

            if (randomItem > 0)
            {
                targetsToVisit.Add(new TargetItemPair(target, type, randomItem));
            }
        }
    }
    
    private void Shuffle(List<Transform> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var swapIndex = Random.Range(0, i + 1);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }

    private void CashierIcon()
    {
        var cashierIcon = _cashier.GetComponent<ProductTypeIcon>();
        var cashierType = cashierIcon.productType;
        aiUIController.ProductSprite(cashierType);
    }

    private void ExitPointIcon()
    {
        var exitPointIcon = exitPoint.GetComponent<ProductTypeIcon>();
        var exitPointType = exitPointIcon.productType;
        aiUIController.ProductSprite(exitPointType);
    }

    public void ProcessItem()
    {
        if (!hasProcessed)
        {
            boxGO = Instantiate(cartonBox, cashierPointList[2].transform.position, Quaternion.identity);
            hasProcessed = true;
            if (collectedItems.Count > 0)
                productMoveCoroutine = StartCoroutine(ProcessNextItem());
        }
    }

    private IEnumerator ProcessNextItem()
    {
        while (currentProductIndex < collectedItems.Count)
        {
            var aiProduct = collectedItems[currentProductIndex];


            yield return aiProduct.DOJump(cashierPointList[0].transform.position, 2, 1, 0.5f)
                .SetEase(Ease.Linear).WaitForCompletion();

            if (cashierPointList.Count > 1 && cashierPointList[1] != null)
                yield return aiProduct.DOMoveX(cashierPointList[1].transform.position.x, 1f)
                    .SetEase(Ease.Linear).WaitForCompletion();

            if (cashierPointList.Count > 2 && cashierPointList[2] != null)
                yield return aiProduct.DOJump(cashierPointList[2].transform.position, 2, 1, 0.5f)
                    .SetEase(Ease.Linear).WaitForCompletion();

            currentProductIndex++;
        }

        isProductInBox = true;
    }

    public void AllProductMoved()
    {
        productCountToPrize = collectedItems.Count*3;

        if (productMoveCoroutine != null)
        {
            StopCoroutine(productMoveCoroutine);
            productMoveCoroutine = null;
        }

        Destroy(basketGO);

        foreach (var collectedItem in collectedItems) Destroy(collectedItem.gameObject);

        collectedItems.Clear();
        currentProductIndex = 0;

        boxGO.transform.DOJump(boxPoint.transform.position, 1, 1, .5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                boxGO.transform.SetParent(transform);

                SpawnMoney();


                cashProcessEnd = true;
                StartCoroutine(DestroyAI());
            });
    }

    private void SpawnMoney()
    {
        var spawnPoint = cashierPointList[3].transform.position;
        var yOffset = 0.2f;
        var rowLimit = 5;
        var numSpawned = 0;
        var row = 0;
        var xOffsetStep = 0.1f;

        for (var i = 0; i < productCountToPrize; i++)
        {
            if (numSpawned >= rowLimit)
            {
                numSpawned = 0;
                row++;
            }

            float xOffset = (numSpawned % 2 == 0 ? 1 : -1) * (numSpawned / 2);
            var moneyPosition = new Vector3(spawnPoint.x + xOffset, spawnPoint.y + row * yOffset, spawnPoint.z);
            moneyGO = Instantiate(money, moneyPosition, Quaternion.identity);
            numSpawned++;
            
            _checkoutInteractable.salesList.Add(moneyGO.transform);
        }
    }


    private IEnumerator DestroyAI()
    {
        SetDestination(exitPoint);
        ChangeAnimationState(AI_BOX_WALK);

        ExitPointIcon();

        yield return new WaitUntil(() => Vector3.Distance(transform.position, exitPoint.position) <= stoppingDistance);
        Destroy(gameObject);
    }


  
    public void CollectAisleItem(TriggeredAreaData triggeredAreaData)
    {
        if (targetsToVisit.Count == 0) return;

        var thisAI = gameObject.GetComponent<AIController>();

        if (triggeredAreaData.productType == Enums.ProductType.Tomato ||triggeredAreaData.productType == Enums.ProductType.Canned ||triggeredAreaData.productType == Enums.ProductType.Egg )
        {
            if (CurrentNeededCount > 0)
            {
                List<Transform> tempProductList = new List<Transform>();

                foreach (var product in triggeredAreaData.salesList)
                {
                    tempProductList.Add(product);
                }
                
                var currentYOffset = targetPoint.position.y;

                for (var i = 0; i < CurrentNeededCount && i < tempProductList.Count; i++)
                {
                    var productToTake = tempProductList[i];

                    productToTake.DOMove(targetPoint.position, 1f+i*0.5f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            productToTake.SetParent(transform);
                            productToTake.localPosition = new Vector3(0, currentYOffset, 0);
                            currentYOffset += productSize;
                            
                            int remainingNeededCount = CurrentNeededCount - 1;
                            
                            if (remainingNeededCount <= 0)
                            {
                                targetsToVisit.RemoveAt(0);

                                if (targetsToVisit.Count > 0)
                                {
                                    collectedItems.Clear();
                                    aiUIController.CurrentItemCount(0);
                                    MoveTarget();
                                }
                                else if (targetsToVisit.Count == 0)
                                {
                                    _checkoutInteractable.AddedAI(thisAI);
                                    MoveTarget();
                                    InternalEvents.FinishedAIProcessing.Invoke(gameObject.transform);
                                }
                            }
                          
                            collectedItems.Add(productToTake);
                            aiUIController.CurrentItemCount(collectedItems.Count);

                            tempProductList.Remove(productToTake);
                            triggeredAreaData.salesList.Remove(productToTake);
                            
                        });
                }
            }
        }
    }


    public void CollectNeededCount(Transform product, Enums.ProductType productType)
    {
        var currentYOffset = targetPoint.position.y;

        if (CurrentNeededCount > 0)
        {
            product.DOMove(targetPoint.position, 1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    product.SetParent(transform);
                    product.localPosition = new Vector3(0, currentYOffset, 0);
                    currentYOffset += productSize;

                    collectedItems.Add(product);
                    aiUIController.CurrentItemCount(collectedItems.Count);
                    
                    int remainingNeededCount = CurrentNeededCount - 1;
                    
                    
                    if (remainingNeededCount <= 0)
                    {
                        targetsToVisit.RemoveAt(0);

                        if (targetsToVisit.Count > 0)
                        {
                            collectedItems.Clear();
                            aiUIController.CurrentItemCount(0);
                            
                            MoveTarget();
                        }
                        else if (targetsToVisit.Count == 0)
                        {
                            _checkoutInteractable.AddedAI(this);
                            MoveTarget();
                            InternalEvents.FinishedAIProcessing.Invoke(gameObject.transform);
                        }
                    }
                });
        }
    }



    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        _animator.Play(newState);

        currentState = newState;
    }
}

public class TargetItemPair
{
    public Transform Target { get; set; }

    public Enums.ProductType ProductType { get; set; }

    public int ItemCount { get; set; }

    public TargetItemPair(Transform target, Enums.ProductType type, int itemCount)
    {
        Target = target;

        ProductType = type;

        ItemCount = itemCount;
    }
}