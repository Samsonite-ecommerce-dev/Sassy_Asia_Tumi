using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Web.Models
{

    #region 菜单栏
    public class DefineMenu
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("children")]
        public List<MenuChild> Children { get; set; }

        public class MenuChild
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("target")]
            public string Target { get; set; }
        }
    }
    #endregion

    #region 权限
    public class DefineUserPower
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
    #endregion

    #region Enum
    public class DefineEnum
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        [JsonPropertyName("display")]
        public string Display { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        [JsonPropertyName("css")]
        public string Css { get; set; }
    }
    #endregion

    #region 通用框
    public class DefineSelectOption
    {
        /// <summary>
        /// label
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    public class DefineGroupSelectOption
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("options")]
        public List<DefineSelectOption> Options { get; set; }
    }
    #endregion
}
