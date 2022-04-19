using Samsonite.Library.Business.Web.Custom.Models;
using Samsonite.Library.Core.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sassy.APP.Helper
{
    public class SparePartHelper
    {
        #region 分组类别
        /// <summary>
        /// 分组类别集合
        /// </summary>
        /// <returns></returns>
        private List<DefineEnum> SparePartGroupTypeReflect()
        {
            List<DefineEnum> _result = new List<DefineEnum>()
            {
                new DefineEnum() { ID = (int)GroupType.SAP, Display = "SAP", Css = "text-info" },
                new DefineEnum() { ID = (int)GroupType.Custom, Display = "Custom", Css = "text-success" }
            };
            return _result;
        }

        /// <summary>
        /// 分组类别列表
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> SparePartGroupTypeObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in SparePartGroupTypeReflect())
            {
                _result.Add(new DefineSelectOption() { Label = _o.Display, Value = _o.ID });
            }
            return _result;
        }

        /// <summary>
        /// 分组类别显示值
        /// </summary>
        /// <param name="status"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public string GetSparePartGroupTypeDisplay(int status, bool css = false)
        {
            string _result = string.Empty;
            DefineEnum _O = SparePartGroupTypeReflect().Where(p => p.ID == status).SingleOrDefault();
            if (_O != null)
            {
                if (css)
                {
                    _result = string.Format("<label class=\"{0}\">{1}</label>", _O.Css, _O.Display);
                }
                else
                {
                    _result = _O.Display;
                }
            }
            return _result;
        }
        #endregion
    }
}