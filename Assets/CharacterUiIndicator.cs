using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUiIndicator : MonoBehaviour {

    /// how fast does indicators fade/show up?
    [Range(0f, 1f)]
    public float fadeRatio;

    #region AnimationIndicators
    [System.Serializable]
    public struct Indicator
    {
        /// casches renderers from obj
        [System.NonSerialized]
        public SpriteRenderer[] renderers;

        /// Maximal alpha value for all sprites
        [System.NonSerialized]
        public float alpha;

        /// data from last raycast
        [System.NonSerialized]
        public RaycastHit2D hit;

        public float rayDistance;

        /// object which owns Sprites 
        public GameObject obj;

        public bool use;
        public bool nonDependedOnInput;
    }
    public Indicator[] animationIndicators;
    
    void UpdateIndicators(Indicator[] indicators, bool rev)
    {
        int i = 0;
        foreach (var ind in indicators)
        {
            foreach (var r in ind.renderers)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, ind.use && (rev || ind.nonDependedOnInput) ? ind.alpha : 0f, fadeRatio));
            }
            ++i;
        }
    }
    public void InitIndicators(Indicator[] indicators, Color color)
    {
        for (int i = 0; i < indicators.Length; ++i)
        {
            indicators[i].renderers = indicators[i].obj.GetComponentsInChildren<SpriteRenderer>();

            if(indicators[i].renderers != null)
                indicators[i].alpha = indicators[i].renderers[0].color.a;
            else
                indicators[i].alpha = color.a;

            foreach (var it in indicators[i].renderers)
                it.color = new Color(color.r, color.g, color.b, 0);
        }
    }
    #endregion AnimationIndicators

    #region EnvironmentIndicators
    [Space]
    public float envRayLength;
    public float envRaySeparation;
    public float envRayInitialDistance;
    
    public Indicator[] environmentIndicators;

    bool CastRay(Vector2 position, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            position + dir * envRayInitialDistance,
            dir, envRayLength
        );

        if (hit.collider && !hit.collider.isTrigger)
        {
            Debug.Assert(hit.collider.gameObject != controller.gameObject);

            environmentIndicators[0].use = true;

            var marker = hit.collider.GetComponent<CharacterUiMarker>();
            if (marker)
            {
                environmentIndicators[marker.type].use = true;
                environmentIndicators[marker.type].hit = hit;
                return true;
            }
        }
        return false;

    }
    void UpdateEnvironmentIndicators()
    {
        Vector2 mouseDir = controller.GetInput().GetDirectionInput().normalized;
        for (int i = 0; i < environmentIndicators.Length; ++i)
        {
            environmentIndicators[i].use = false;
        }

        bool b = false;
        b |= CastRay((Vector2)controller.transform.position, mouseDir);
        b |= CastRay((Vector2)controller.transform.position + new Vector2(-mouseDir.y, mouseDir.x) * envRaySeparation, mouseDir);
        b |= CastRay((Vector2)controller.transform.position - new Vector2(-mouseDir.y, mouseDir.x) * envRaySeparation, mouseDir);
        if(b)
            environmentIndicators[0].use = false;
    }
    #endregion EnvironmentIndicators


    CharacterStateController controller;
    InputManagerBase input
    {
        get
        {
            return controller.GetInput();
        }

    }

    
    public void Awake () {
        controller = GetComponentInParent<CharacterStateController>();
    }
    
    void Update () {
        if (!transform.parent)
            return;

        if(input.isDirectionInputApplied())
        {
            float angle = -Vector2.SignedAngle(input.GetDirectionInput(), Vector2.up);
            transform.position = transform.parent.position + Quaternion.Euler(0, 0, angle) * Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            UpdateEnvironmentIndicators();

            UpdateIndicators(animationIndicators, true);
            UpdateIndicators(environmentIndicators, true);
        }
        else
        {
            UpdateIndicators(animationIndicators, false);
            UpdateIndicators(environmentIndicators, false);
        }
    }

   

}
