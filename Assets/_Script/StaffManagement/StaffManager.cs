using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffManager : MonoBehaviour
{
    public static StaffManager Instance;

    public List<Staff> hiredList = new List<Staff>();
    public List<Staff> hirableList = new List<Staff>();

    public GameObject staffEntryHirePrefab; // Assign your prefab here
    public GameObject staffEntryFirePrefab; // Assign your prefab here
    public GameObject staffPrefab; // Prefab for staff members in the game

    public Transform hirableStaffPanel; // Panel for displaying hirable staff
    public Transform hiredStaffPanel; // Panel for displaying hired staff

    public int hirableLimit = 10; // Limit for the number of hirable staff
    public int initialStaffCount = 5; // Number of staff members to generate at start

    private void Start()
    {
        Instance = this;
        GenerateInitialStaff();
        PopulateHirableList();
    }

    private void GenerateInitialStaff()
    {
        // Clear existing lists
        hirableList.Clear();
        hiredList.Clear();

        // Generate initial staff members
        for (int i = 0; i < initialStaffCount; i++)
        {
            Staff newStaff = CreateRandomStaff();
            hirableList.Add(newStaff);
        }
    }

    private Staff CreateRandomStaff()
    {
        // Create a new GameObject for the staff member
        GameObject staffObject = new GameObject("Staff");
        Staff staff = staffObject.AddComponent<Staff>();

        // Assign random values
        staff.staffName = GetRandomName();
        staff.role = GetRandomJobType();
        staff.dailyWage = Random.Range(10, 100); // Example wage range

        return staff;
    }

    private string GetRandomName()
    {
        string[] names = { "Alice", "Bob", "Charlie", "David", "Emily", "Frank", "Grace", "Hannah", "Ivan", "Jack" };
        return names[Random.Range(0, names.Length)];
    }

    private JobType GetRandomJobType()
    {
        JobType[] jobTypes = (JobType[])System.Enum.GetValues(typeof(JobType));
        return jobTypes[Random.Range(0, jobTypes.Length)];
    }

    private void PopulateHirableList()
    {
        // Clear the current hirable staff UI
        foreach (Transform child in hirableStaffPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var staff in hirableList)
        {
            if (hiredList.Count >= hirableLimit)
            {
                Debug.Log("Hirable staff limit reached.");
                break;
            }

            GameObject staffEntry = Instantiate(staffEntryHirePrefab, hirableStaffPanel);
            staffEntry.transform.Find("NameText").GetComponent<TMP_Text>().text = staff.staffName;
            staffEntry.transform.Find("RoleText").GetComponent<TMP_Text>().text = staff.role.ToString();
            staffEntry.transform.Find("WageText").GetComponent<TMP_Text>().text = $"Wage: ${staff.dailyWage}";

            Button hireButton = staffEntry.transform.Find("HireButton").GetComponent<Button>();
            hireButton.onClick.AddListener(() => HireStaff(staff));

            // Set the button interactable based on the current staff limit
            hireButton.interactable = hiredList.Count < hirableLimit;
        }
    }

    public void HireStaff(Staff newStaff)
    {
        if (hiredList.Count >= hirableLimit)
        {
            Debug.Log("Hirable staff limit reached.");
            return;
        }

        hiredList.Add(newStaff);
        hirableList.Remove(newStaff);

        // Instantiate the staff prefab in the game world
        GameObject staffObject = Instantiate(staffPrefab);
        Staff staffComponent = staffObject.GetComponent<Staff>();
        staffComponent.staffName = newStaff.staffName;
        staffComponent.role = newStaff.role;
        staffComponent.dailyWage = newStaff.dailyWage;
        staffComponent.currentState = EmployeeState.Working;

        // Optionally, set the staff position, add it to the game world, etc.
        // For example, set its position to a predefined location
        staffObject.transform.position = new Vector3(0, 0, 0); // Set position as needed

        // Remove the old staff object from the game world
        Destroy(newStaff.gameObject);

        // Move to hired staff UI
        UpdateStaffUI();

        Debug.Log($"{newStaff.staffName} hired as {newStaff.role}.");
    }

    public void FireStaff(Staff staff)
    {
        if (hiredList.Contains(staff))
        {
            hiredList.Remove(staff);
            hirableList.Add(staff); // Add back to the hirable list if needed
            UpdateStaffUI();
            Destroy(staff.gameObject); // Remove the staff member from the game
            Debug.Log($"{staff.staffName} fired.");
        }
        else
        {
            Debug.Log($"{staff.staffName} not found.");
        }
    }

    private void UpdateStaffUI()
    {
        // Update hirable staff UI
        PopulateHirableList();

        // Update hired staff UI
        foreach (Transform child in hiredStaffPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var staff in hiredList)
        {
            GameObject staffEntry = Instantiate(staffEntryFirePrefab, hiredStaffPanel);
            staffEntry.transform.Find("NameText").GetComponent<TMP_Text>().text = staff.staffName;
            staffEntry.transform.Find("RoleText").GetComponent<TMP_Text>().text = staff.role.ToString();
            staffEntry.transform.Find("WageText").GetComponent<TMP_Text>().text = $"Wage: ${staff.dailyWage}";

            Button fireButton = staffEntry.transform.Find("FireButton").GetComponent<Button>();
            fireButton.onClick.AddListener(() => FireStaff(staff));
        }
    }
}
