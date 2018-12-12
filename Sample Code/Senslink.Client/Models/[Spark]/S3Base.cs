using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Senslink.Client.Models
{


    /// <summary>
    /// Amazon S3 parameters.
    /// </summary>
    public abstract class S3Base
    {
        #region Amazon S3
        /// <summary>
        /// Required, S3 secret key.
        /// </summary>
        [JsonProperty(PropertyName = "SecretKey", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string SecretKey { get; set; }
        /// <summary>
        /// Required, S3 access Key
        /// </summary>
        [JsonProperty(PropertyName = "AccessKey", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string AccessKey { get; set; }
        #endregion
    }

    public class S3GerneralJob : S3Base
    {
        /// <summary>
        /// Model Input Data Path.
        /// </summary>
        [JsonProperty(PropertyName = "InputPath", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string InputPath { get; set; }
    }

    public class S3PredefinedJob : S3Base
    {
        /// <summary>
        /// Data output Path. Only for SubmitPredefinedJobParams
        /// </summary>
        [JsonProperty(PropertyName = "ModelOutput", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string OutputPath { get; set; }

        /// <summary>
        /// Training Data path. Only for SubmitPredefinedJobParams
        /// </summary>
        [JsonProperty(PropertyName = "TrainingData", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string TrainingData { get; set; }
        /// <summary>
        /// Evaluation Data generated path. Only for SubmitPredefinedJobParams
        /// </summary>
        [JsonProperty(PropertyName = "EvaluationPath", NullValueHandling = NullValueHandling.Ignore)]
        public string EvalutaionPath { get; set; }
    }

}