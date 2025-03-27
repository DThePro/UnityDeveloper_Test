using UnityEngine;

public class Skybox3DRotator : MonoBehaviour
{
    public float rotationSpeedX = 5f;
    public float rotationSpeedY = 5f;
    public float rotationSpeedZ = 5f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeedY);
        // Fake X/Z rotation by manipulating texture offsets
        RenderSettings.skybox.SetTextureOffset("_MainTex", new Vector2(Time.time * rotationSpeedX, Time.time * rotationSpeedZ));
    }
}