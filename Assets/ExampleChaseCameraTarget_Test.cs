using UnityEngine;

public class ExampleChaseCameraTarget_Test : BaseManager_Mono<ExampleChaseCameraTarget_Test>
{
    void Awake()
    {
        ExampleChaseCamera_Test.target = transform;
    }
}
