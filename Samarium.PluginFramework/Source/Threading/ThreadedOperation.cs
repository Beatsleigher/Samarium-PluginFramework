using System;

namespace Samarium.PluginFramework.Threading {
    
    using System.Threading;
    using System.Dynamic;

    /// <summary>
    /// Contains information about threads running within the application scope.
    /// </summary>
    public class ThreadedOperation {

        /// <summary>
        /// Gets or sets the name of the running thread.
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// Gets or sets the time at which the thread started execution.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time at which the thread halted.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the current state of the thread.
        /// </summary>
        public ThreadState ThreadState { get; set; }

        /// <summary>
        /// Gets or sets the progress of the running thread.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Gets or sets data required by the thread to execute.
        /// This object is dynamic by nature and can be bent to suit the needs of any operation.
        /// </summary>
        public dynamic ThreadData { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            if (obj is ThreadedOperation obj2) return this == obj2;
            else return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => ThreadName.GetHashCode() ^ StartTime.GetHashCode() ^ EndTime?.GetHashCode() ?? 1024 ^ ThreadState.GetHashCode() ^ (int)Progress ^ ((object)ThreadData).GetHashCode();

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ThreadedOperation obj1, ThreadedOperation obj2) {
            return obj1.GetHashCode() == obj2.GetHashCode();
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ThreadedOperation obj1, ThreadedOperation obj2) => !(obj1 == obj2);
        
    }

    /// <summary>
    /// Mutable object for referencing objects in lists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ref<T> where T : class {
        public T Value { get; set; }
    }

}
