﻿namespace OpenConstructionSet.Models;

/// <summary>
/// Represents a Rotation from the game data files.
/// </summary>
/// <param name="W">W value.</param>
/// <param name="X">X value.</param>
/// <param name="Y">Y value.</param>
/// <param name="Z">Z value.</param>
public sealed record Vector4(float W, float X, float Y, float Z);