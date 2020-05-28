namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// A container for methods to validate entities entering the system
    /// </summary>
    static class ValidationHelper
    {
        /// <summary>
        /// Validate fields in an entity; this will only complete if all attributes specified contain valid data.
        /// If any attribute given is null, it will throw an exception containing the names of all such attributes.
        /// If uncaught, this exception will cancel the transaction.
        /// </summary>
        /// <param name="entity"></param> Entity to validate
        /// <param name="attributes"></param> Array of attribute names that must contain non-null data
        public static void ValidateEntity(Entity entity, string[] attributes)
        {
            List<string> failureMessage = null;
            foreach(string s in attributes)
            {
                if (!entity.Contains(s))
                {
                    //Make this down here to avoid massive waste of allocation time on big (successful) imports
                    if (null == failureMessage)
                    {
                        failureMessage = new List<string>();
                        failureMessage.Add("Invalid Customer Asset: missing field(s): " + s);
                    }
                    else
                        failureMessage.Add(", " + s);
                }
            }
            if(null != failureMessage)
                throw new InvalidPluginExecutionException(String.Concat(failureMessage));
        }
    }
}
