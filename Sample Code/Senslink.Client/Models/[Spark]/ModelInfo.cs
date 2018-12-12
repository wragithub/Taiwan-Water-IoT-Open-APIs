using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Senslink.Client.Models
{

    /// <summary>
    /// Model Related Information.
    /// </summary>
    public class Model
    {
        #region Amazon Model Related Info

        private Dictionary<ModelNames, string> _mainFileNameMap = new Dictionary<ModelNames, string>()
        {
            { ModelNames.AssociationRules, $"{ModelNames.AssociationRules.ToString()}Train" },
            { ModelNames.DecisionTree, $"{ModelNames.DecisionTree.ToString()}Train" },
            { ModelNames.FPgrowth, $"{ModelNames.FPgrowth.ToString()}Train" },
            { ModelNames.GradientBoosted, $"{ModelNames.GradientBoosted.ToString()}Train" },

            { ModelNames.LinearSVMWithSGD, $"{ModelNames.LinearSVMWithSGD.ToString()}Train" },
            { ModelNames.LogisticRegression, $"{ModelNames.LogisticRegression.ToString()}Train" },
            { ModelNames.LRWithLasso, $"{ModelNames.LRWithLasso.ToString()}Train" },
            { ModelNames.LRWithRidge, $"{ModelNames.LRWithRidge.ToString()}Train" },

            { ModelNames.LRWithSGD, $"{ModelNames.LRWithSGD.ToString()}Train" },
            { ModelNames.Prefixspan, $"{ModelNames.Prefixspan.ToString()}Train" },
            { ModelNames.RandomForest, $"{ModelNames.RandomForest.ToString()}Train" },
        };

        public Model()
        {
            //initialize the modelNames and mainFileName maps
         //   _mainFileNameMap.Add();
        }

        public enum ModelNames
        {
            AssociationRules,
            DecisionTree,
            FPgrowth,
            GradientBoosted,
            LRWithLasso,
            LRWithRidge,
            LRWithSGD,
            LinearSVMWithSGD,
            LogisticRegression,
            Prefixspan,
            RandomForest
        }

        /// <summary>
        /// Model name
        /// </summary>
        [JsonProperty(PropertyName = "Name", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Model Parameters 
        /// </summary>
        [JsonProperty(PropertyName = "Parameters", NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public string Parameters { get; set; }

        public string GetMainFileName(ModelNames name)
        {
            return _mainFileNameMap[name];
        }

        #endregion

    }
}