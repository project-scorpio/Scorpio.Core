using System.Collections.Generic;

namespace Scorpio.Conventional
{
    /// <summary>
    /// 约定注册器列表的内部实现类，继承自 <see cref="List{T}"/>
    /// </summary>
    /// <remarks>
    /// 用于管理和存储实现了 <see cref="IConventionalRegistrar"/> 接口的约定注册器集合
    /// </remarks>
    internal class ConventionalRegistrarList : List<IConventionalRegistrar>
    {

    }
}