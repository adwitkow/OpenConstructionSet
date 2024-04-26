﻿namespace OpenConstructionSet;

/// <summary>
/// Collection of methods for working with models.
/// </summary>
public static class ModelExtensions
{
    private const string RemovedValueKey = "REMOVED";

    /// <summary>
    /// Returns a copy of <c>item</c> marked as deleted.
    /// </summary>
    /// <param name="item">The item to mark as deleted.</param>
    /// <returns>A copy of the item marked as deleted.</returns>
    public static Item Delete(this Item item) => new(item.Type, item.Id, item.Name, item.StringId, ItemChanges.Changed)
    {
        Values = new Dictionary<string, object> { ["DELETED"] = true }
    };

    /// <summary>
    /// Return a copy of <c>reference</c> marked as deleted.
    /// </summary>
    /// <param name="reference">The reference to mark as deleted.</param>
    /// <returns>A copy of the reference marked as deleted.</returns>
    public static Reference Delete(this Reference reference) => reference with { Values = ReferenceValues.Deleted };

    /// <summary>
    /// Return a copy of <c>instance</c> marked as deleted.
    /// </summary>
    /// <param name="instance">The instance to mark as deleted.</param>
    /// <returns>A copy of the instance marked as deleted.</returns>
    public static Instance Delete(this Instance instance) => instance with { Target = "" };

    /// <summary>
    /// Determines if <c>item</c> is marked as deleted.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if <c>item</c> is marked as deleted.</returns>
    public static bool IsDeleted(this Item item) => item.Values.TryGetValue(RemovedValueKey, out var value)
                                                  && value is bool removed
                                                  && removed;

    /// <summary>
    /// Determines if <c>reference</c> is marked as deleted.
    /// </summary>
    /// <param name="reference">The reference to check.</param>
    /// <returns><c>true</c> if <c>reference</c> is marked as deleted.</returns>
    public static bool IsDeleted(this Reference reference) => reference.Values.Equals(ReferenceValues.Deleted);

    /// <summary>
    /// Determines if <c>instance</c> is marked as deleted.
    /// </summary>
    /// <param name="instance">The instance to check.</param>
    /// <returns><c>true</c> if <c>instance</c> is marked as deleted.</returns>
    public static bool IsDeleted(this Instance instance) => string.IsNullOrEmpty(instance.Target);
}