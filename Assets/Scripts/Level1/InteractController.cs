using TMPro;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] private float interactRadius = 5f;
    [SerializeField] private TextMeshProUGUI scoreText;
    public bool canInteract = true;

    private Collider[] interactingObjects;
    public int points = 0;

    void Start()
    {
        scoreText.text = points.ToString();
    }

    void Update()
    {
        interactingObjects = Physics.OverlapSphere(transform.position, interactRadius);

        foreach (var collider in interactingObjects)
        {
            if (collider.CompareTag("PointsCube") && canInteract)
            {
                // Calculate direction to the collider
                Vector3 directionToCollider = (collider.transform.position - transform.position).normalized;
                float distanceToCollider = Vector3.Distance(transform.position, collider.transform.position);

                // Check if there is a clear line of sight using a raycast
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToCollider, out hit, distanceToCollider))
                {
                    // If the raycast hits the collider, then it's visible
                    if (hit.collider == collider)
                    {
                        Destroy(collider.gameObject);
                        scoreText.text = (++points).ToString();

                        if (points == 18)
                        {
                            EndGame.Instance.GameEnd(0, 18);
                        }
                    }
                }
            }
        }
    }
}
