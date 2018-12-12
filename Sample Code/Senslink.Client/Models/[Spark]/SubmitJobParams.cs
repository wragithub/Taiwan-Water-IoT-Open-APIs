using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Senslink.Client.Models
{
    /// <summary>
    ///   Input  parameters including both Spark and Amazon configurations.
    /// </summary>
    public class SubmitGeneralJobParams
    {
        /// <summary>
        /// Spark configuration
        /// </summary>
        [Required]
        public SparkGeneral Spark { get; set; }
        /// <summary>
        /// Amazon configuration
        /// </summary>
        [Required]
        public S3GerneralJob S3 { get; set; }
    }

    public class SubmitPredefinedJobParams
    {
        public SparkPredefined Spark { get; set; }

        public S3PredefinedJob S3 { get; set; }

        /// <summary>
        /// Model Information.
        /// </summary>
        [Required]
        public Model Model { get; set; }
    }
}