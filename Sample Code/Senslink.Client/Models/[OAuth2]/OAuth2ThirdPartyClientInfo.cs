using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public class OAuth2ThirdPartyClientInfo
    {
        /// <summary>
        /// [必要] Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// [必要] 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// [必要] 
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Senslink 3.0 User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// [必要] 
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// [必要] 
        /// </summary>
        public string ServiceDescritpion { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// [必要] 
        /// </summary>
        public string CallbackUrl { get; set; }
    }
}
