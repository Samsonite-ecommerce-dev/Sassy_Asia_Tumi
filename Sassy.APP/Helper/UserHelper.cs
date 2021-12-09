using Samsonite.Library.Core;
using Samsonite.Library.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Helper
{
    public class UserHelper
    {
        private IBaseService _baseService;
        public UserHelper(IBaseService baseService)
        {
            _baseService = baseService;
        }

        #region 账号类型
        /// <summary>
        /// 账号类型集合
        /// </summary>
        /// <returns></returns>
        private List<DefineEnum> UserTypeReflect()
        {
            //加载语言包
            var _LanguagePack = _baseService.CurrentLanguagePack();

            List<DefineEnum> _result = new List<DefineEnum>()
            {
                new DefineEnum() { ID = (int)UserType.InternalStaff, Display = _LanguagePack["users_index_type_1"], Css = "text-primary" },
                new DefineEnum() { ID = (int)UserType.Customer, Display = _LanguagePack["users_index_type_2"], Css = "text-warning" }
            };
            return _result;
        }

        /// <summary>
        /// 全部账号类型列表(包换客户)
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> UserTypeObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in UserTypeReflect())
            {
                _result.Add(new DefineSelectOption() { Label = _o.Display, Value = _o.ID });
            }
            return _result;
        }

        /// <summary>
        /// 账号类型显示值
        /// </summary>
        /// <param name="objStatus"></param>
        /// <param name="objCss"></param>
        /// <returns></returns>
        public string GetUserTypeDisplay(int objStatus, bool objCss = false)
        {
            string _result = string.Empty;
            DefineEnum _O = UserTypeReflect().Where(p => p.ID == objStatus).SingleOrDefault();
            if (_O != null)
            {
                if (objCss)
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