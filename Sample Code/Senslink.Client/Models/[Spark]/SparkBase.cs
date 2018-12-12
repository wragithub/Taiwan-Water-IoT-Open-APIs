using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;

namespace Senslink.Client.Models
{

    /// <summary>
    /// Spark API parameters
    /// </summary>
    public abstract class SparkBase
    {
        #region Spark Api related

        /// <summary>
        /// Spark Account name.
        /// </summary>
        [JsonProperty(PropertyName = "Account", NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore()]
        public string Account { get; set; }

        /// <summary>
        /// driver
        /// </summary>
        [JsonProperty(PropertyName = "Driver", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string Driver { get; set; }

        /// <summary>
        /// Worker
        /// </summary>
        [JsonProperty(PropertyName = "Worker", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string[] Workers { get; set; }

        /// <summary>
        /// Job Priority it could be normal or high.
        /// </summary>
        [JsonProperty(PropertyName = "Priority", NullValueHandling = NullValueHandling.Ignore)]
        public string Priority { get; set; }
      
        #endregion
    }

    public class SparkGeneral : SparkBase
    {
        /// <summary>
        /// The entry point for the program which contain the Main File.
        /// </summary>
        [JsonProperty(PropertyName = "MainFile", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string MainFile { get; set; }

        /// <summary>
        /// Job Priority it could be normal or high.
        /// </summary>
        [JsonProperty(PropertyName = "arguments", NullValueHandling = NullValueHandling.Ignore)]
        public string Arguments { get; set; }
    }

    public class SparkPredefined : SparkBase
    {
        /// <summary>
        /// The entry point for the program which contain the Main File.
        /// </summary>
        [JsonProperty(PropertyName = "MainFile", NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public string MainFile { get; set; }
    }

}