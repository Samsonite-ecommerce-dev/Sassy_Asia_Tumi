using Samsonite.Library.Core.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Samsonite.Library.Core
{
    public class AppLanguageService : IAppLanguageService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IMemoryCache _memoryCache;
        private appEntities _appDB;
        private IResponseCookies _cookie_res => _httpContextAccessor.HttpContext.Response.Cookies;
        private IRequestCookieCollection _cookie_req => _httpContextAccessor.HttpContext.Request.Cookies;
        private AppConfigModel _appConfig;
        public AppLanguageService(IAppConfigService appConfigService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, appEntities appEntities)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _appDB = appEntities;
            _appConfig = appConfigService.GetConfigCache();
            //读取需要加载的语言
            _LoadLanguages = _appConfig.SysConfig.LanguagePacks;
        }

        private string _cacheName
        {
            get
            {
                return $"{_appConfig.GlobalConfig.CacheKey}_LANGUAGE_PACK";
            }
        }

        private string _cookieName
        {
            get
            {
                return $"{_appConfig.GlobalConfig.CookieKey}_LANGUAGE_PACK";
            }
        }

        //默认365天
        public int _cacheTime = 365;

        /// <summary>
        /// 需要加载的语言集合
        /// </summary>
        public List<int> _LoadLanguages = new List<int>();

        /// <summary>
        /// 语言分类
        /// </summary>
        /// <returns></returns>
        public List<LanguageType> LanguageTypeOption()
        {
            //默认语言为英文
            //js文件名称默认值有中文简体,中文繁体和英文
            return _appDB.LanguageType.ToList();
        }

        /// <summary>
        /// 获取默认语言
        /// </summary>
        /// <returns></returns>
        public LanguageType DefaultLanguagePack()
        {
            return this.LanguageTypeOption().Where(p => p.IsDefault).SingleOrDefault();
        }

        /// <summary>
        /// 初始化语言缓存
        /// </summary>
        public void LoadLanguagePacks()
        {
            //从数据库读取语言包
            List<View_LanguagePack> objView_LanguagePack_List = _appDB.View_LanguagePack.Where(p => !p.IsDelete).ToList();
            foreach (var _O in this.LanguageTypeOption())
            {
                if (_LoadLanguages.Contains(_O.ID))
                {
                    //单个语言包缓存格式为_CacheName+'_'+语言包ID
                    object _object;
                    if (!_memoryCache.TryGetValue($"{ this._cacheName}_{_O.ID}", out _object))
                    {
                        _object = null;
                    }
                    if (_object == null)
                    {
                        this.InsertLanguagePackCache(objView_LanguagePack_List, _O.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 重置语言包缓存
        /// </summary>
        public void ResetLanguagePacks()
        {
            //从数据库读取语言包
            List<View_LanguagePack> objView_LanguagePack_List = _appDB.View_LanguagePack.Where(p => !p.IsDelete).ToList();
            foreach (var _O in this.LanguageTypeOption())
            {
                if (_LoadLanguages.Contains(_O.ID))
                {
                    //单个语言包缓存格式为_CacheName+'_'+语言包ID
                    object _object;
                    if (_memoryCache.TryGetValue($"{this._cacheName}_{_O.ID}", out _object))
                    {
                        _memoryCache.Remove($"{this._cacheName}_{_O.ID}");
                    }
                    //重新插入缓存
                    this.InsertLanguagePackCache(objView_LanguagePack_List, _O.ID);
                }
            }
        }

        /// <summary>
        /// 写入语言包缓存
        /// </summary>
        /// <param name="packs"></param>
        /// <param name="typeID"></param>
        private void InsertLanguagePackCache(List<View_LanguagePack> packs, int typeID)
        {
            LanguageType objLanguageType = this.LanguageTypeOption().Where(p => p.ID == typeID).SingleOrDefault();
            if (objLanguageType != null)
            {
                var _languagePacks = packs.Where(p => p.LanguageTypeID == typeID).ToList();
                try
                {
                    Dictionary<string, string> _pack = new Dictionary<string, string>();
                    foreach (var item in _languagePacks)
                    {
                        _pack.Add(item.PackKey, item.PackValue);
                    }
                    _memoryCache.Set(string.Format("{0}_{1}", this._cacheName, objLanguageType.ID), _pack, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(this._cacheTime)));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 设置当前语言包
        /// </summary>
        /// <param name="type"></param>
        public void SetLanguage(int type)
        {
            string _lgPack = EncryptHelper.DESEncrypt(type.ToString());
            _cookie_res.Append($"{this._cookieName}_LgPack", _lgPack, new CookieOptions() { Expires = DateTime.Now.AddDays(30) });
        }

        /// <summary>
        /// 根据站点配置获取当前语言列表
        /// </summary>
        /// <returns></returns>
        public List<LanguageType> CurrentLanguageOption()
        {
            List<LanguageType> _result = new List<LanguageType>();
            List<LanguageType> languageTypes = this.LanguageTypeOption();
            List<int> _ConfigLanguagePack = _appConfig.SysConfig.LanguagePacks;
            foreach (var _O in languageTypes)
            {
                if (_ConfigLanguagePack.Contains(_O.ID))
                {
                    _result.Add(_O);
                }
            }
            return _result;
        }
    }
}
