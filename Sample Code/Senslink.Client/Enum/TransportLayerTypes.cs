namespace Senslink.Client.Enum
{
    public enum TransportLayerTypes
    {
        //RdaqDevice端作為 Client，對遠端設備進行主動連線，用於現場有固定IP的環境
        TcpClient = 0,

        //RdaqDevice端作為 Server，接聽遠端設備連入連線，用於現場沒有固定IP的環境
        TcpServer = 1,

        //RdaqDevice 使用 Udp 與遠端連線
        UdpServer = 3,
    };
}
