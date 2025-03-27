using TMPro;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] private float interactRadius = 5f;
    [SerializeField] private TextMeshProUGUI scoreText;
    public bool canInteract = true;

    private Collider[] interactingObjects;
    public int points = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = points.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        interactingObjects = Physics.OverlapSphere(transform.position, interactRadius);

        foreach (var collider in interactingObjects)
        {
            if (collider.CompareTag("PointsCube") && canInteract)
            {
                Destroy(collider.gameObject);
                scoreText.text = (++points).ToString();
            }
        }
    }
}
