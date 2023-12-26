using LOONACIA.Unity.Collections;
using LOONACIA.Unity.Managers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDSystem : MonoBehaviour
{
    private readonly Dictionary<Transform, UIARObjectInfoCard> m_cards = new();

    [SerializeField]
    private PlayerController m_controller;

    [SerializeField]
    private PossessionProcessor m_processor;

    [SerializeField]
    private HUDSettings m_settings;

    private Camera m_camera;

    private void Awake()
    {
        if (m_controller == null)
        {
            m_controller = FindObjectOfType<PlayerController>();
        }
        
        if (m_processor == null)
        {
            m_processor = FindObjectOfType<PossessionProcessor>();
        }
        
        ManagerRoot.UI.ShowSceneUI<UIHUD>().Init(m_settings)
            .RegisterController(m_controller)
            .RegisterProcessor(m_processor);
        m_camera = Camera.main;
    }

    private void FixedUpdate()
    {
        var hits = ArrayPool<RaycastHit>.Shared.Rent(16);
        Transform cameraTransform = m_camera.transform;
        int length = Physics.SphereCastNonAlloc(cameraTransform.position, m_settings.CheckRadius, cameraTransform.forward, hits,
            m_settings.MaxDistance, m_settings.AimLayers);

        if (length > 0)
        {
            foreach (var hit in hits.AsSpan(0, length))
            {
                if (hit.distance < m_settings.MinDistance)
                {
                    continue;
                }

                TryAdd(hit.transform);
            }
        }

        UpdatePosition();
        ArrayPool<RaycastHit>.Shared.Return(hits);
    }

    private void UpdatePosition()
    {
        if (m_cards.Count == 0)
        {
            return;
        }

        using ValueList<int> toRemove = new(stackalloc int[m_cards.Count]);
        foreach (var (root, card) in m_cards)
        {
            if (IsNeedToRemove(root))
            {
                toRemove.Add(root.GetInstanceID());
                continue;
            }

            ProjectToScreen(root);
        }

        foreach (var id in toRemove.AsSpan())
        {
            var card = m_cards.SingleOrDefault(card => card.Key.GetInstanceID() == id);
            if (card.Equals(default(KeyValuePair<Transform, UIARObjectInfoCard>)))
            {
                continue;
            }

            m_cards.Remove(card.Key);
            ManagerRoot.Resource.Release(card.Value.gameObject);
        }
    }

    private bool IsNeedToRemove(Transform target)
    {
        if (target == null)
        {
            return true;
        }

        Vector3 targetPosition = target.position.GetFlatVector();
        Vector3 cameraPosition = m_camera.transform.position.GetFlatVector();
        Vector3 direction = targetPosition - cameraPosition;
        float sqrDistance = direction.sqrMagnitude;
        float sqrMinDistance = m_settings.MinDistance * m_settings.MinDistance;
        float sqrMaxDistance = m_settings.MaxDistance * m_settings.MaxDistance;
        if (sqrDistance < sqrMinDistance || sqrDistance > sqrMaxDistance)
        {
            return true;
        }

        return Vector3.Dot(direction.normalized, m_camera.transform.forward) < 0.975f;
    }

    private void TryAdd(Transform target)
    {
        if (!target.TryGetComponent<IARObject>(out var arObject))
        {
            return;
        }

        if (!m_cards.TryGetValue(target, out var card))
        {
            card = ManagerRoot.UI.ShowSceneUI<UIARObjectInfoCard>();
            m_cards.Add(target, card);
            card.SetInfo(arObject.Info);
        }
    }

    private void ProjectToScreen(Transform target)
    {
        var col = target.GetComponent<Collider>();
        Bounds bounds = col.bounds;
        Vector3 extents = m_camera.transform.TransformDirection(bounds.extents.normalized) * bounds.extents.magnitude;
        Vector3 leftTop = m_camera.WorldToScreenPoint(bounds.center + extents);
        Vector3 rightBottom = m_camera.WorldToScreenPoint(bounds.center - extents);

        if (!m_cards.TryGetValue(target, out var card))
        {
            return;
        }

        (float xMin, float xMax) = leftTop.x < rightBottom.x ? (leftTop.x, rightBottom.x) : (rightBottom.x, leftTop.x);
        (float yMin, float yMax) = leftTop.y < rightBottom.y ? (rightBottom.y, leftTop.y) : (leftTop.y, rightBottom.y);

        card.UpdatePosition(xMin, xMax, yMin, yMax);
    }
}