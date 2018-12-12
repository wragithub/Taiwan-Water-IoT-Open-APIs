using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    /// <summary>
    /// File sharing level among the users.
    /// </summary>
    [Flags]
    public enum FileShareLevel
    {
        /// <summary>
        /// Only the owner of the file can access it.
        /// </summary>
        Private = 0,

        /// <summary>
        /// File is open for all user to access it.
        /// </summary>
        Public = 1,

        /// <summary>
        ///File is shared only among a specific users.
        /// </summary>
        SpecificUser = 4
    }
}
