using System;

namespace Senslink.Client.Models
{
    /// <summary>
    /// 簡單使用者參數,無敏感資訊
    /// </summary>
    public class UserBasicInfo
    {
        /// <summary>
        /// [必要] 使用者Id
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// [必要] 使用者帳號名稱(UserName)，需唯一，建立帳號時需要檢查
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// [非必要] 顯示名稱，預設值為Account
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

    }
}
