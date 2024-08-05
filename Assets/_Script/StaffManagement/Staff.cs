using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Staff : MonoBehaviour
{
    public string staffName;
    public JobType role;
    public int skillLevel;
    public float morale;
    public float dailyWage;

    private Dictionary<JobType, int> jobSkills = new Dictionary<JobType, int>
    {
        { JobType.Blacksmith, 0 },
        { JobType.Carpenter, 0 },
        { JobType.Potioncrafting, 0 },
        { JobType.Alchemy, 0 },
        { JobType.Tailoring, 0 },
        { JobType.Fletcher, 0 },
        { JobType.Herbalist, 0 },
        { JobType.Mason, 0 },
        { JobType.Jeweler, 0 },
        { JobType.Smithing, 0 },
        { JobType.Leatherworking, 0 },
        { JobType.Brewer, 0 },
        { JobType.Enchanter, 0 },
        { JobType.Scribe, 0 },
        { JobType.Weaver, 0 },
        { JobType.Apothecary, 0 },
        { JobType.Glassblower, 0 },
        { JobType.Potter, 0 },
        { JobType.Tanner, 0 },
        { JobType.Arcanist, 0 }
    };

    public EmployeeState currentState = EmployeeState.Idle;
    public Transform assignedStation;
    public Transform courierLocation;
    public Inventory inventory; // Reference to the inventory

    private NavMeshAgent agent;
    private Animator animator; // Reference to the Animator component
    [SerializeField] private IEnumerator currentJob;

    // Flag to indicate if the staff is hired and spawned
    private bool isHired;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(); // Get the Animator component
        jobSkills[role] = skillLevel; // Initialize the skill level for the specific role
        UpdateAnimator();

        // Assuming the staff is hired and spawned if this component is added
        isHired = true;

        courierLocation = FindNearestDeliveryStorage(transform.position).transform;


        // Optionally, you might want to have some other logic to set this flag
        // based on your specific game logic or prefabs.
    }

    private void Update()
    {
        if (!isHired)
        {
            Debug.Log($"{staffName} is not hired. Cannot perform any tasks.");
            return;
        }

        switch (currentState)
        {
            case EmployeeState.Idle:
                UpdateAnimator();
                break;
            case EmployeeState.Working:
                if (currentJob == null)
                {
                    assignedStation = FindNearestAvailableCraftingStation().transform;
                    currentJob = WorkAtStation();
                    StartCoroutine(currentJob);
                }
                break;
            case EmployeeState.Delivering:
                if (currentJob == null)
                {
                    currentJob = DeliverToDropOff();
                    StartCoroutine(currentJob);
                }
                break;
        }
    }

    public void Train(int skillIncrease)
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot train.");
            return;
        }

        if (jobSkills.ContainsKey(role))
        {
            jobSkills[role] += skillIncrease;
            skillLevel = jobSkills[role];
            Debug.Log($"{staffName} trained: Skill Level={skillLevel}");
        }
    }

    public void AdjustMorale(float moraleChange)
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot adjust morale.");
            return;
        }

        morale += moraleChange;
        if (morale > 100) morale = 100;
        if (morale < 0) morale = 0;
        Debug.Log($"{staffName}'s morale adjusted to: {morale}");
    }

    private IEnumerator WorkAtStation()
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot work.");
            yield break;
        }

        Recipe recipe = GetRecipeForStation();
        if (recipe == null)
        {
            Debug.LogError($"{staffName}: No recipe found for the station.");
            yield break;
        }

        Debug.Log($"{staffName}: Recipe found for the station: {recipe.name}");

        var requiredMaterials = recipe.GetRequiredMaterials();
        if (!inventory.HasRequiredMaterials(requiredMaterials))
        {
            Debug.Log($"{staffName}: Missing materials. Gathering materials...");
            currentState = EmployeeState.Idle;
            UpdateAnimator();
            yield return StartCoroutine(GatherMaterials(requiredMaterials));

            if (!inventory.HasRequiredMaterials(requiredMaterials))
            {
                Debug.LogWarning($"{staffName}: Failed to gather all required materials.");
                currentJob = null;
                currentState = EmployeeState.Working;
                UpdateAnimator();
                yield break;
            }
        }

        // Find the nearest available crafting station
        CraftingStation availableStation = FindNearestAvailableCraftingStation();
        if (availableStation == null)
        {
            Debug.Log($"{staffName}: No available crafting stations found. Waiting...");
            yield return new WaitUntil(() => FindNearestAvailableCraftingStation() != null);
        }
        else
        {
            // If staff is already assigned to a different station, reassign
            if (assignedStation != null && assignedStation != availableStation.transform)
            {
                Debug.Log($"{staffName}: Reassigning to a new station at position {availableStation.transform.position}");
            }

            assignedStation = availableStation.transform;
            Debug.Log($"{staffName}: Moving to assigned station at position {assignedStation.position}");
            currentState = EmployeeState.Moving;
            UpdateAnimator();
            yield return StartCoroutine(MoveToPosition(assignedStation.position));

            float distanceToStation = Vector3.Distance(transform.position, assignedStation.position);
            if (distanceToStation < 1.5f)
            {
                Debug.Log($"{staffName}: Working at {assignedStation.name}. Craft time: {recipe.craftTime * recipe.outputQuantity} seconds.");
                currentState = EmployeeState.Working;
                UpdateAnimator();
                yield return new WaitForSeconds(recipe.craftTime * recipe.outputQuantity);

                foreach (var material in requiredMaterials)
                {
                    inventory.RemoveMaterial(material.Key, material.Value);
                }

                inventory.AddMaterial(recipe.output, recipe.outputQuantity);
                currentState = EmployeeState.Delivering;
                UpdateAnimator();
            }
            else
            {
                Debug.Log($"{staffName}: Not at the assigned station yet.");
            }
        }

        currentJob = null;
    }

    private IEnumerator GatherMaterials(Dictionary<Item, int> requiredMaterials)
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot gather materials.");
            yield break;
        }

        bool materialsGathered = false;

        while (!materialsGathered)
        {
            Inventory nearestStorage = FindNearestNonpersonStorage(transform.position);

            if (nearestStorage == null)
            {
                Debug.LogWarning($"{staffName}: No valid Nonperson storage found.");
                yield break;
            }

            yield return StartCoroutine(MoveToPosition(nearestStorage.transform.position));

            if (Vector3.Distance(transform.position, nearestStorage.transform.position) < 1.5f)
            {
                if (nearestStorage.HasRequiredMaterials(requiredMaterials))
                {
                    foreach (var material in requiredMaterials)
                    {
                        nearestStorage.RemoveMaterial(material.Key, material.Value);
                        inventory.AddMaterial(material.Key, material.Value);
                    }
                    materialsGathered = true;
                }
                else
                {
                    Debug.Log($"{staffName}: Nearest Nonperson storage does not have all required materials. Continuing search...");
                }
            }
            else
            {
                Debug.Log($"{staffName}: Not at the nearest Nonperson storage yet. Continuing search...");
            }
        }

        currentState = EmployeeState.Working;
        UpdateAnimator();
        currentJob = null;
    }

    private IEnumerator DeliverToDropOff()
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot deliver.");
            yield break;
        }

        yield return StartCoroutine(MoveToPosition(courierLocation.position));

        Inventory courierInventory = courierLocation.GetComponent<Inventory>();
        if (courierInventory == null || !courierInventory.IsDelivery())
        {
            Debug.LogWarning($"{staffName}: Courier location does not have a valid Nonperson inventory.");
            yield break;
        }

        if (Vector3.Distance(transform.position, courierLocation.position) < 1.5f)
        {
            foreach (var material in inventory.GetMaterials())
            {
                courierInventory.AddMaterial(material.Key, material.Value);
                inventory.RemoveMaterial(material.Key, material.Value);
            }

            Debug.Log($"{staffName} has delivered to {courierLocation.name}");
            currentState = EmployeeState.Working;
            UpdateAnimator();
        }

        currentJob = null;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        if (!isHired)
        {
            Debug.LogWarning($"{staffName} is not hired. Cannot move.");
            yield break;
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned.");
            yield break;
        }

        agent.SetDestination(targetPosition);

        currentState = EmployeeState.Moving;
        UpdateAnimator();

        while (Vector3.Distance(transform.position, targetPosition) > 1.5f)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || (agent.velocity.sqrMagnitude == 0f))
                {
                    break;
                }
            }
            yield return null;
        }

        currentState = EmployeeState.Idle;
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        // Set the animator parameters based on the current state
        animator.SetBool("IsWorking", currentState == EmployeeState.Working);
        animator.SetBool("IsMoving", currentState == EmployeeState.Moving);
    }

    private Recipe GetRecipeForStation()
    {
        // Implement logic based on job type and assigned station
        return assignedStation.GetComponent<CraftingStation>().GetFirstRecipe();
    }

    private Inventory FindNearestNonpersonStorage(Vector3 position)
    {
        Inventory nearestInventory = null;
        float nearestDistance = Mathf.Infinity;
        Inventory[] allInventories = FindObjectsOfType<Inventory>();

        foreach (Inventory inv in allInventories)
        {
            if (inv.IsNonpersonStorage() || inv.IsDelivery())
            {
                float distance = Vector3.Distance(position, inv.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInventory = inv;
                }
            }
        }

        return nearestInventory;
    }

    private Inventory FindNearestDeliveryStorage(Vector3 position)
    {
        Inventory nearestInventory = null;
        float nearestDistance = Mathf.Infinity;
        Inventory[] allInventories = FindObjectsOfType<Inventory>();

        foreach (Inventory inv in allInventories)
        {
            if (inv.IsDelivery())
            {
                float distance = Vector3.Distance(position, inv.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInventory = inv;
                }
            }
        }

        return nearestInventory;
    }


    private CraftingStation FindNearestAvailableCraftingStation()
    {
        CraftingStation[] allStations = FindObjectsOfType<CraftingStation>();
        CraftingStation nearestStation = null;
        float nearestDistance = Mathf.Infinity;

        foreach (CraftingStation station in allStations)
        {
            if (!station.isOccupied)
            {
                float distance = Vector3.Distance(transform.position, station.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestStation = station;
                }
            }
        }

        return nearestStation;
    }
}

public enum EmployeeState
{
    Idle,
    Working,
    Delivering,
    Moving
}

public enum JobType
{
    Blacksmith,
    Carpenter,
    Potioncrafting,
    Alchemy,
    Tailoring,
    Fletcher,
    Herbalist,
    Mason,
    Jeweler,
    Smithing,
    Leatherworking,
    Brewer,
    Enchanter,
    Scribe,
    Weaver,
    Apothecary,
    Glassblower,
    Potter,
    Tanner,
    Arcanist
}
