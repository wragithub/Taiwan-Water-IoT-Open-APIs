using System;

namespace Senslink.Client.Models
{
    /// <summary>
    /// 使用者參數
    /// </summary>
    public class UserInfo : UserBasicInfo
    {
        // Last Review 2016/9/7 Abi, Richard
       
        /// <summary>
        /// [必要] 電子郵件
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// [非必要]
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// [必要] 密碼
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// [非必要] 行動電話號碼 PhoneNumber
        /// </summary>
        /// <value>The mobile phone.</value>
        public string MobileNumber { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// [非必要] 圖示檔案(使用者頭像)
        /// </summary>
        /// <value>The logo binary.</value>
        public byte[] LogoBinary { get; set; }

        /// <summary>
        /// [非必要] 延伸設定XML,UserXmlProfile.ToString()
        /// </summary>
        /// <value>The extend profile in XML.</value>
        public string ExtendProfileInXml { get; set; }

        /// <summary>
        /// [非必要] 備註，僅供系統管理者使用
        /// </summary>
        /// <value>The comment.</value>
        public string Memo { get; set; }

        /// <summary>
        /// [必要] 是否已經核准使用 (使用Email認證帳號開通)
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// [非必要] RESRFul 通訊所使用的 Key
        /// </summary>
        /// <value>The rest key.</value>
        public string RestKey { get; set; }

        /// <summary>
        /// [必要] 使用者預設時區(分鐘)，例如時區為8則是480(8*60)
        /// </summary>
        public int UserTimeZone { get; set; }

        /// <summary>
        /// [非必要] 最後一次登入時間
        /// </summary>
        public DateTimeOffset? LastLoginDate { get; set; }

        /// <summary>
        /// [非必要] 建立日期
        /// </summary>
        public DateTimeOffset? CreateDate { get; set; }

        /// <summary>
        /// [非必要] 登入錯誤次數
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// [非必要] 是否鎖住
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// [非必要] 鎖住帳號到期日
        /// </summary>
        public DateTimeOffset? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// [非必要] 
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// [非必要] Max limit of file storage size,預設為0,儲存檔案容量上限
        /// </summary>
        public long FileStorageQouta { get; set; }

    }
}
