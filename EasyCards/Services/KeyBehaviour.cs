using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyCards.Services;

public sealed class KeyBehaviour : MonoBehaviour
{
    private readonly ImmutableArray<IInputEventSubscriber> _subscribers;

    public KeyBehaviour()
    {
        _subscribers = Container.Instance.Resolve<IInputEventSubscriber[]>().ToImmutableArray();
    }
    private void LateUpdate()
    {
        var keyControls = Keyboard.current.allKeys.m_Array.Where(i => i.wasPressedThisFrame).ToImmutableArray();
        for (var x = 0; x < _subscribers.Length; x++)
        {
            for (var y = 0; y < keyControls.Length; y++)
            {
                if (!_subscribers[x].HandlesKey(keyControls[y].keyCode)) continue;
                _subscribers[x].OnInputEvent(keyControls[y].keyCode);
            }
        }
    }
}