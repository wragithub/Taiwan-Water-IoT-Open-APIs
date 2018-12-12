namespace Senslink.Client.Models
{

    //Last Review 2016.9.7 Abi, Brian, Richard

    public class RdaqOpcUaClientInfo : RdaqDeviceBaseInfo
    {
        /// <summary>
        /// [非必要] NodeId前面一段相同的字串
        /// </summary>
        public string OpcUaNodesPath { get; set; }

        /// <summary>
        /// [非必要] Target OPC Server connection UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// [非必要] Target OPC Server connection Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// [非必要] OPC 伺服器憑證位置
        /// </summary>
        public string CertificationPath { get; set; }

        /// <summary>
        /// [必要] 安全模式,預設值請給false
        /// </summary>
        public bool IsSecurity { get; set; }

        /// <summary>
        /// [非必要] 是否自動產生NodeId,尚未實作 
        /// </summary>
        public bool AutoCreateChannel { get; set; }

    }
}
