using Microsoft.Extensions.Caching.Memory;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web.Models;
using System;

namespace Samsonite.Library.Core.Web
{
    public class AppConfigService : AppConfigCore, IAppConfigService
    {
        private IMemoryCache _memoryCache;
        public AppConfigService(IMemoryCache memoryCache, appEntities appEntities) : base(appEntities)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        /// <returns></returns>
        public AppConfigModel GetConfig()
        {
            return new AppConfigModel()
            {
                GlobalConfig = this.GetGlobalConfig(),
                SysConfig = this.GetSysConfig()
            };
        }


        /// <summary>
        /// 缓存名
        /// </summary>
        private string _configCacheName
        {
            get
            {
                return $"{this.GetGlobalConfig().CacheKey}_SYSCONFIG";
            }
        }

        //缓存默认365天
        private int _cacheTime = 365;

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        /// <returns></returns>
        public void LoadConfigCache()
        {
            object _object;
            if (!_memoryCache.TryGetValue(this._configCacheName, out _object))
            {
                _object = null;
            }
            if (_object == null)
            {
                AppConfigModel objConfig = this.GetConfig();
                //写入缓存
                _memoryCache.Set(this._configCacheName, objConfig, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(this._cacheTime)));
            }
        }

        /// <summary>
        /// 获取配置信息(从缓存中读取)
        /// </summary>
        /// <returns></returns>
        public AppConfigModel GetConfigCache()
        {
            AppConfigModel _result = new AppConfigModel();
            object _object;
            if (!_memoryCache.TryGetValue(this._configCacheName, out _object))
            {
                _object = null;
            }
            if (_object != null)
            {
                _result = (AppConfigModel)_object;
            }
            else
            {
                _result = this.GetConfig();
                //写入缓存
                _memoryCache.Set(this._configCacheName, _result, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(this._cacheTime)));
            }
            return _result;
        }

        /// <summary>
        /// 重置系统配置缓存
        /// </summary>
        public void ResetConfigCache()
        {
            object _object;
            if (_memoryCache.TryGetValue(this._configCacheName, out _object))
            {
                _memoryCache.Remove(this._configCacheName);
            }
            //重新插入缓存
            _memoryCache.Set(this._configCacheName, this.GetConfig(), new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(this._cacheTime)));
        }
    }
}