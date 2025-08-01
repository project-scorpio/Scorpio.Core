namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 单例依赖注入标记接口，继承自 <see cref="IDependency"/>
    /// </summary>
    /// <remarks>
    /// 实现此接口的所有类都会自动注册到依赖注入容器中作为单例（Singleton）服务
    /// </remarks>
    public interface ISingletonDependency : IDependency
    {

    }
}
