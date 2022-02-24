using Microsoft.EntityFrameworkCore;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.Web.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Samsonite.Library.Business.Web.Basic
{
    public class EmailGroupConfigService : IEmailGroupConfigService
    {
        private IAppLogService _appLogService;
        private appEntities _appDB;
        public EmailGroupConfigService(IAppLogService appLogService, appEntities appEntities)
        {
            _appDB = appEntities;
            _appLogService = appLogService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QueryResponse<SendMailGroup> GetQuery(EmailGroupConfigSearchRequest request)
        {
            QueryResponse<SendMailGroup> _result = new QueryResponse<SendMailGroup>();
            var _list = _appDB.SendMailGroup.AsQueryable();

            //搜索条件
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                _list = _list.Where(p => p.GroupName.Contains(request.Keyword));
            }

            //返回数据
            request.Page = VariableHelper.SaferequestPage(request.Page);
            _result.TotalRecord = _list.Count();
            _result.Items = _list.AsNoTracking().OrderBy(p => p.ID).Skip((request.Page - 1) * request.Rows).Take(request.Rows).ToList();
            return _result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Add(EmailGroupConfigAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GroupName))
                {
                    throw new Exception("组名不能为空");
                }
                else
                {
                    SendMailGroup objSendMailGroup = _appDB.SendMailGroup.Where(p => p.GroupName == request.GroupName).SingleOrDefault();
                    if (objSendMailGroup != null)
                    {
                        throw new Exception("组名已经存在，请勿重复");
                    }
                }

                List<MailAddressesAttr> _mailAddressesAttrs = JsonSerializer.Deserialize<List<MailAddressesAttr>>(request.MailAddresses);
                if (_mailAddressesAttrs.Count > 0)
                {
                    foreach (var item in _mailAddressesAttrs)
                    {
                        if (string.IsNullOrEmpty(item.Value))
                        {
                            throw new Exception("邮件地址不能为空");
                        }
                    }
                }
                else
                {
                    throw new Exception("至少添加一个要发送的邮件地址");
                }

                SendMailGroup objData = new SendMailGroup()
                {
                    GroupName = request.GroupName,
                    MailAddresses = string.Join(",", _mailAddressesAttrs.Select(p => p.Value).ToList()),
                    Remark = request.Remark,
                    CreateTime = DateTime.Now
                };
                _appDB.SendMailGroup.Add(objData);
                _appDB.SaveChanges();
                //添加日志
                _appLogService.InsertLog<SendMailGroup>(objData, objData.ID.ToString());
                //返回信息
                return new PostResponse()
                {
                    Result = true,
                    Message = "数据保存成功"
                };
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PostResponse Edit(EmailGroupConfigEditRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GroupName))
                {
                    throw new Exception("组名不能为空");
                }
                else
                {
                    SendMailGroup objSendMailGroup = _appDB.SendMailGroup.Where(p => p.GroupName == request.GroupName && p.ID != request.ID).SingleOrDefault();
                    if (objSendMailGroup != null)
                    {
                        throw new Exception("组名已经存在，请勿重复");
                    }
                }

                List<MailAddressesAttr> _mailAddressesAttrs = JsonSerializer.Deserialize<List<MailAddressesAttr>>(request.MailAddresses);
                if (_mailAddressesAttrs.Count > 0)
                {
                    foreach (var item in _mailAddressesAttrs)
                    {
                        if (string.IsNullOrEmpty(item.Value))
                        {
                            throw new Exception("邮件地址不能为空");
                        }
                    }
                }
                else
                {
                    throw new Exception("至少添加一个要发送的邮件地址");
                }

                SendMailGroup objData = _appDB.SendMailGroup.Where(p => p.ID == request.ID).SingleOrDefault();
                if (objData != null)
                {
                    objData.GroupName = request.GroupName;
                    objData.MailAddresses = string.Join(",", _mailAddressesAttrs.Select(p => p.Value).ToList());
                    objData.Remark = request.Remark;
                    _appDB.SaveChanges();
                    //添加日志
                    _appLogService.UpdateLog<SendMailGroup>(objData, objData.ID.ToString());
                    //返回信息
                    return new PostResponse()
                    {
                        Result = true,
                        Message = "数据保存成功"
                    };
                }
                else
                {
                    throw new Exception("数据读取失败");
                }
            }
            catch (Exception ex)
            {
                //返回信息
                return new PostResponse()
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public PostResponse Delete(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    throw new Exception("请至少选择一条要操作的数据");
                }

                SendMailGroup objSendMailGroup = new SendMailGroup();
                foreach (var id in ids)
                {
                    objSendMailGroup = _appDB.SendMailGroup.Where(p => p.ID == id).SingleOrDefault();
                    if (objSendMailGroup != null)
                    {
                        _appDB.SendMailGroup.Remove(objSendMailGroup);
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}:{1}", id, "信息不存在或已被删除"));
                    }
                }
                _appDB.SaveChanges();
                //添加日志
                _appLogService.DeleteLog("SendMailGroup", string.Join(",", ids));
                //返回信息
                return new PostResponse
                {
                    Result = true,
                    Message = "数据删除成功"
                };
            }
            catch (Exception ex)
            {
                return new PostResponse
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }
    }
}
