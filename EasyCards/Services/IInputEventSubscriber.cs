using UnityEngine.InputSystem;

namespace EasyCards.Services;

public interface IInputEventSubscriber
{
    bool HandlesKey(Key key);
    void OnInputEvent(Key key);
}