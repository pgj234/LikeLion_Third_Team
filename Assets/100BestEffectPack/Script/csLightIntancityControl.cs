using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class csLightIntancityControl : MonoBehaviour {
	
	public enum LightStopAction
	{
		None, Disable, Destroy, Callback
    }

	public Light _light;
	float _time = 0;
	public float Delay = 0.5f;
	public float Down = 1;
	public LightStopAction stopAction;
	public UnityAction stopCallback;

	void Update ()
	{
		_time += Time.deltaTime;

		if(_time > Delay)
		{
			if(_light.intensity > 0)
				_light.intensity -= Time.deltaTime*Down;

			if(_light.intensity <= 0)
			{
                _light.intensity = 0;
				switch(stopAction)
				{
					case LightStopAction.Disable:
                        gameObject.SetActive(false);
                        break;
                    case LightStopAction.Destroy:
                        Destroy(gameObject);
                        break;
                    case LightStopAction.Callback:
                        stopCallback?.Invoke();
                        break;
                    default:
                        break;
                }
            }
				
		}
	}
}
