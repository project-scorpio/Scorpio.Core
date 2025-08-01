namespace Scorpio.Runtime
{
    /// <summary>
    /// 定义远程服务的标记接口
    /// </summary>
    /// <remarks>
    /// 此接口作为标记接口，用于标识实现了远程服务功能的类型。
    /// 实现此接口的类通常表示：
    /// 1. 可以通过网络调用的服务
    /// 2. 分布式系统中的远程组件
    /// 3. 需要特殊处理的远程调用服务
    /// 
    /// 标记接口不包含任何方法或属性，仅用于类型识别和分类。
    /// 框架可以通过检查类型是否实现此接口来应用特定的处理逻辑，
    /// 如代理生成、网络通信配置、序列化处理等。
    /// </remarks>
    /// <seealso cref="System.Runtime.Remoting"/>
    public interface IRemoteService
    {
    }
}
