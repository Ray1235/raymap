﻿using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Subblocks of a geometric object / R3Mesh
    /// </summary>
    public interface IR3CollideGeometricElement {
        IR3CollideGeometricElement Clone(R3CollideMesh mesh);
    }
}