using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 target;
    private Animator animator;
    private Vector3 previousPosition;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(SetNewDestination());
    }

    private void Update()
    {
        if (animator != null)
            animator.SetFloat("MoveSpeed", (transform.position - previousPosition).magnitude / Time.deltaTime);
        timer += Time.deltaTime;
        previousPosition = transform.position;
    }

    // Update is called once per frame
    IEnumerator SetNewDestination()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(Random.Range(-15, 15), 50, Random.Range(-15, 15)), Vector3.down, out hit))
                target = hit.point;

            if (agent.remainingDistance < 0.2f || timer > 15)
            {
                agent.SetDestination(target);
                timer = 0;
            }
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
}
