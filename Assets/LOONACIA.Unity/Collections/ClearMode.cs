namespace LOONACIA.Unity.Collections
{
    /// <summary>
    /// This enum determines the behavior of <see cref="PooledList{T}"/> and <see cref="ValueList{T}"/> when returning the underlying array to the pool.
    /// </summary>
    public enum ClearMode
    {
        /// <summary>
        /// Clear the array if the element type is a reference type or contains reference types.
        /// </summary>
        ReferenceTypeOnly,
        
        /// <summary>
        /// Clear the array always.
        /// </summary>
        Always,
        
        /// <summary>
        /// Never clear the array.
        /// </summary>
        Never
    }
}