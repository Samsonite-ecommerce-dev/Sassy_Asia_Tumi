using Newtonsoft.Json;
using System.Collections.Generic;

namespace Samsonite.Library.Core.Models
{

    #region 菜单栏
    public class DefineMenu
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<MenuChild> Children { get; set; }

        public class MenuChild
        {
            [JsonProperty(PropertyName = "id")]
            public int ID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }

            [JsonProperty(PropertyName = "target")]
            public string Target { get; set; }
        }
    }
    #endregion

    #region 权限
    public class DefineUserPower
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
    #endregion

    #region 结果集
    public class DefineResult
    {
        public bool Success { get; set; }

        public string ErrMessage { get; set; }
    }
    #endregion 

    #region Enum
    public class DefineEnum
    {
        /// <summary>
        /// id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public string Css { get; set; }
    }
    #endregion

    #region 通用框
    public class DefineSelectOption
    {
        /// <summary>
        /// label
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }

    public class DefineGroupSelectOption
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "options")]
        public List<DefineSelectOption> Options { get; set; }
    }

    public class DefineComboBox
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "selected")]
        public bool Selected { get; set; }
    }

    public class DefineComboTree
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "iconCls")]
        public string IconCls { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<Children> Childrens { get; set; }


        public class Children
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            [JsonProperty(PropertyName = "iconCls")]
            public string IconCls { get; set; }
        }
    }

    public class DefineChooseBox
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class List
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "group")]
        public string Group { get; set; }
    }

    public class DefineTree
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<TreeChildren> Children { get; set; }

        public class TreeChildren
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }
        }
    }
    #endregion
}
