using Samsonite.Library.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.APP.Helper
{
    public class ServiceHelper
    {
        #region 服务状态集合
        /// <summary>
        /// 服务状态集合
        /// </summary>
        /// <returns></returns>
        private List<DefineEnum> ServiceStatusReflect()
        {
            List<DefineEnum> _result = new List<DefineEnum>()
            {
                new DefineEnum() { ID = (int)ServiceStatus.Stop, Display = "停止中", Css = "text-danger" },
                new DefineEnum() { ID = (int)ServiceStatus.Runing, Display = "运行中", Css = "text-success" },
                new DefineEnum() { ID = (int)ServiceStatus.Pause, Display = "暂停中", Css = "text-warning" }
            };
            return _result;
        }

        /// <summary>
        /// 服务状态列表
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> ServiceStatusObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in ServiceStatusReflect())
            {
                _result.Add(new DefineSelectOption() { Label = _o.Display, Value = _o.ID });
            }
            return _result;
        }

        /// <summary>
        /// 服务状态显示值
        /// </summary>
        /// <param name="objStatus"></param>
        /// <param name="objCss"></param>
        /// <returns></returns>
        public string GetServiceStatusDisplay(int objStatus, bool objCss = false)
        {
            string _result = string.Empty;
            DefineEnum _O = ServiceStatusReflect().Where(p => p.ID == objStatus).SingleOrDefault();
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

        #region 操作类型集合
        /// <summary>
        /// 操作类型集合
        /// </summary>
        /// <returns></returns>
        private List<object[]> JobTypeReflect()
        {
            List<object[]> _result = new List<object[]>()
            {
                new object[] { (int)JobType.Start, "启动" },
                new object[] { (int)JobType.Pause, "暂停" },
                new object[] { (int)JobType.Continue, "继续" }
            };
            return _result;
        }

        /// <summary>
        /// 操作类型列表
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> JobTypeObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in JobTypeReflect())
            {
                _result.Add(new DefineSelectOption() { Label = _o[1].ToString(), Value = _o[0] });
            }
            return _result;
        }

        /// <summary>
        /// 操作类型显示值
        /// </summary>
        /// <param name="objStatus"></param>
        /// <returns></returns>
        public string GetJobTypeDisplay(int objStatus)
        {
            string _result = string.Empty;
            foreach (var _O in JobTypeReflect())
            {
                if ((int)_O[0] == objStatus)
                {
                    _result = _O[1].ToString();
                    break;
                }
            }
            return _result;
        }
        #endregion

        #region 操作状态集合
        /// <summary>
        /// 操作状态集合
        /// </summary>
        /// <returns></returns>
        private List<DefineEnum> JobStatusReflect()
        {
            List<DefineEnum> _result = new List<DefineEnum>()
            {
                new DefineEnum() { ID = (int)JobStatus.Wait, Display = "等待中", Css = "text-primary" },
                new DefineEnum() { ID = (int)JobStatus.Processing, Display = "处理中", Css = "text-warning" },
                new DefineEnum() { ID = (int)JobStatus.Success, Display = "已完成", Css = "text-success" },
                new DefineEnum() { ID = (int)JobStatus.Fail, Display = "处理失败", Css = "text-danger" }
            };
            return _result;
        }

        /// <summary>
        /// 操作状态列表
        /// </summary>
        /// <returns></returns>
        public List<DefineSelectOption> JobStatusObject()
        {
            List<DefineSelectOption> _result = new List<DefineSelectOption>();
            foreach (var _o in JobStatusReflect())
            {
                _result.Add(new DefineSelectOption() { Label = _o.Display, Value = _o.ID });
            }
            return _result;
        }

        /// <summary>
        /// 操作状态显示值
        /// </summary>
        /// <param name="objStatus"></param>
        /// <param name="objCss"></param>
        /// <returns></returns>
        public string GetJobStatusDisplay(int objStatus, bool objCss = false)
        {
            string _result = string.Empty;
            DefineEnum _O = JobStatusReflect().Where(p => p.ID == objStatus).SingleOrDefault();
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