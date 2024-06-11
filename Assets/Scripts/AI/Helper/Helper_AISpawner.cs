using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper_AISpawner : MonoBehaviour
{
    [SerializeField] private Aisle _aisle;
    [SerializeField] private Manufacture _manufacture;
    [SerializeField] private MiniFarm _miniFarm;
    [SerializeField] private CannedProductionSite _cannedProduction;
    [SerializeField] private GameObject helper;

    [SerializeField] private Transform eggAisle;
    [SerializeField] private Transform tomatoAisle;
    [SerializeField] private Transform cannedAisle;

    private TriggeredAreaData _triggeredAreaData;
    private Transform helper_spawnPosition;
    private GameObject helperGO;


    private void HelperSpawner()
    {
        helperGO = Instantiate(helper, helper_spawnPosition.position, Quaternion.identity);
        HelperController helperController = helperGO.GetComponent<HelperController>();
        // helperController.InitializeHelperTarget(_aisle, _manufacture,_miniFarm, _cannedProduction);

    }
}
