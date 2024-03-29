﻿using System;
using UnityEngine;

namespace AssemblyCSharp.Assets
{
    public class GroundCheck : MonoBehaviour
    {
        private bool CheckGrounded(float radius, LayerMask whatIsGround, GameObject ignoreObject = null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsGround);
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject != ignoreObject)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

