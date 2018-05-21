﻿using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Subblocks of a geometric object / R3Mesh
    /// </summary>
    public interface IR3GeometricElement {
        IR3GeometricElement Clone(R3Mesh mesh);
    }
}