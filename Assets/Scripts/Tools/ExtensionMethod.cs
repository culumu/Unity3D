using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//扩展方法不会去继承其他的类
public static class ExtensionMethod
{
    private const float dotThreadhold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
       
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward,vectorToTarget);
        return dot>=dotThreadhold;
    }

}
