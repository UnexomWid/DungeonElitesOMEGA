using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWalkingBot : MonoBehaviour
{
    [SerializeField] GameObject destination;
    [SerializeField] float speed;

    Animator animator;
    Transform myTransform;
    private void Start()
    {
        animator = GetComponent<Animator>();
        myTransform = transform;
    }

    private void Update()
    {
        if(Vector2.Distance(myTransform.position, destination.transform.position) > 1)
        {
            myTransform.position = Vector2.MoveTowards(myTransform.position, destination.transform.position, speed * PublicVariables.deltaTime);

            Vector2 diff = destination.transform.position - myTransform.position;
            diff.Normalize();

            myTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90);

            animator.Play("run");
        }
        else animator.Play("idle");
    }

    public void Deactivate()
    {
        GetComponent<player>().enabled = true;

        if (GetComponent<wizard>() != null)
            GetComponent<wizard>().enabled = true;

        if (GetComponent<knight>() != null)
            GetComponent<knight>().enabled = true;

        if (GetComponent<archer>() != null)
            GetComponent<archer>().enabled = true;

        if (GetComponent<Tank>() != null)
            GetComponent<Tank>().enabled = true;

        if (GetComponent<support>() != null)
            GetComponent<support>().enabled = true;
    }
}
