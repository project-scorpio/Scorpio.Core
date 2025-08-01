using System.Collections.Generic;

using Microsoft.Extensions.Options;

namespace Scorpio.Options
{
    /// <summary>
    /// <see cref="IOptionsFactory{TOptions}"/> 接口的实现。
    /// 负责创建和配置选项实例，支持预配置、配置和后配置三个阶段。
    /// </summary>
    /// <typeparam name="TOptions">要创建的选项类型，必须是引用类型且具有无参构造函数</typeparam>
    public class OptionsFactory<TOptions> : IOptionsFactory<TOptions> where TOptions : class, new()
    {
        /// <summary>
        /// 用于配置选项的操作集合
        /// </summary>
        private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
        
        /// <summary>
        /// 用于后配置选项的操作集合
        /// </summary>
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
        
        /// <summary>
        /// 用于预配置选项的操作集合
        /// </summary>
        private readonly IEnumerable<IPreConfigureOptions<TOptions>> _preConfigureOptions;


        /// <summary>
        /// 使用指定的选项配置初始化 <see cref="OptionsFactory{TOptions}"/> 类的新实例。
        /// </summary>
        /// <param name="setups">要运行的配置操作集合</param>
        /// <param name="postConfigures">要运行的后配置操作集合</param>
        /// <param name="preConfigureOptions">要运行的预配置操作集合</param>
        public OptionsFactory(IEnumerable<IConfigureOptions<TOptions>> setups, IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, IEnumerable<IPreConfigureOptions<TOptions>> preConfigureOptions)
        {
            _setups = setups;
            _postConfigures = postConfigures;
            _preConfigureOptions = preConfigureOptions;
        }

        /// <summary>
        /// 创建并返回具有指定名称的已配置 <typeparamref name="TOptions"/> 实例。
        /// 配置过程按以下顺序执行：预配置 -> 配置 -> 后配置。
        /// </summary>
        /// <param name="name">选项配置的名称标识符</param>
        /// <returns>已完全配置的 <typeparamref name="TOptions"/> 实例</returns>
        public TOptions Create(string name)
        {
            // 创建新的选项实例
            var options = new TOptions();
            
            // 第一阶段：执行预配置操作
            foreach (var post in _preConfigureOptions)
            {
                post.PreConfigure(name, options);
            }
            
            // 第二阶段：执行配置操作
            foreach (var setup in _setups)
            {
                // 如果是命名配置选项，使用名称进行配置
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                // 如果是默认名称，执行常规配置
                else if (name == Microsoft.Extensions.Options.Options.DefaultName)
                {
                    setup.Configure(options);
                }
            }
            
            // 第三阶段：执行后配置操作
            foreach (var post in _postConfigures)
            {
                post.PostConfigure(name, options);
            }

            return options;
        }
    }
}
